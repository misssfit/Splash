using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Splash.Common;
using Splash.RemoteServiceContract;
using Splash.SlaveWorker.Data;

namespace Splash.SlaveWorker
{
    public class TaskQueueManager : Singleton<TaskQueueManager>
    {
        private readonly List<RemoteTask> _activeTasks = new List<RemoteTask>();
        private readonly List<RemoteTask> _inactiveTasks = new List<RemoteTask>();
        private readonly List<RemoteTask> _tasksQueue = new List<RemoteTask>();

        private Timer _cleanInactiveTasksTimer;
        private Timer _cleanTimedOutTasksTimer;
        private Timer _workerTimer;


        public TaskQueueManager()
        {
            _cleanInactiveTasksTimer = InitializeNewTimer(CleanInactiveTasks, Configuration.CalculatedTasksTimeoutCheckInterval);
            _cleanTimedOutTasksTimer = InitializeNewTimer(CleanTimedOutTasks, Configuration.TasksCalculationTimeoutCheckInterval);
            _workerTimer = InitializeNewTimer(PerformWork, 1000);
        }

        private Timer InitializeNewTimer(ElapsedEventHandler onTick, int interval)
        {
            var timer = new Timer();
            timer.Elapsed += onTick;
            timer.Interval = interval;
            timer.Enabled = true;
            timer.Start();
            return timer;
        }


        internal bool DeleteTask(string id)
        {
            RemoteTask task = null;
            lock (_tasksQueue)
            {
                if (_tasksQueue.Any(p => p.Id == id) == true)
                {
                    task = _tasksQueue.Single(p => p.Id == id);
                    _tasksQueue.Remove(task);
                    Console.WriteLine("Deleted task from queue with id: " + id);
                    return true;
                }
            }
            lock (_inactiveTasks)
            {
                if (_inactiveTasks.Any(p => p.Id == id) == true)
                {
                    task = _inactiveTasks.Single(p => p.Id == id);
                    _inactiveTasks.Remove(task);
                    Console.WriteLine("Deleted task from inactive task list with id: " + id);
                    return true;
                }
            }
            lock (_activeTasks)
            {
                if (_activeTasks.Any(p => p.Id == id) == true)
                {
                    task = _activeTasks.Single(p => p.Id == id);
                    _activeTasks.Remove(task);
                    try
                    {
                        task.Stop();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Cannot delete/stop task with id: " + id + ". " + ex.Message);
                        return false;
                    }
                    Console.WriteLine("Deleted task from inactive task list with id: " + id);
                    return true;
                }
            }

            Console.WriteLine("Cannot delete task with id: " + id + ". Task does not exist.");
            return false;
        }

        internal CalculationResult GetResult(string id)
        {
            var calculationResult = new CalculationResult();
            lock (_activeTasks)
            {
                if (_inactiveTasks.Any(p => p.Id == id) == true)
                {
                    RemoteTask task = null;
                    lock (_inactiveTasks)
                    {
                        task = _inactiveTasks.Single(p => p.Id == id);
                        calculationResult.Status = task.Status;
                        calculationResult.Info = task.TaskInfo;
                        if (task.Status == TaskStatus.Calculated)
                        {
                            calculationResult.Data = task.Data;
                            _inactiveTasks.Remove(task);
                        }
                        else
                        {
                            _inactiveTasks.Remove(task);
                            Console.WriteLine("Cannot get result for task with id: " + id + ". Task is " + task.Status);
                        }
                    }
                    return calculationResult;
                }
            }
            lock (_activeTasks)
            {
                if (_activeTasks.Any(p => p.Id == id) == true)
                {
                    calculationResult.Status = TaskStatus.InProgress;
                    return calculationResult;
                }
            }
            lock (_tasksQueue)
            {
                if (_tasksQueue.Any(p => p.Id == id) == true)
                {
                    calculationResult.Status = TaskStatus.NotStarted;
                    return calculationResult;
                }
            }
            Console.WriteLine("Cannot get result for task with id: " + id + ". Task does not exist.");
            calculationResult.Status = TaskStatus.Corrupted;
            return calculationResult;
        }

        internal bool PrioritizeTask(string id)
        {
            lock (_tasksQueue)
            {
                if (_tasksQueue.Any(p => p.Id == id) == true)
                {
                    RemoteTask task = _tasksQueue.Single(p => p.Id == id);
                    _tasksQueue.Remove(task);
                    _tasksQueue.Insert(0, task);
                    Console.WriteLine("Task with id: " + id + "moved to the top of the queue.");
                    return true;
                }
            }
            Console.WriteLine("Cannot move up task task with id: " + id + ". Task does not exist.");
            return false;
        }

        internal List<KeyValuePair<TaskPool, List<TaskInfo>>> GetAllTasks()
        {
            var result = new List<KeyValuePair<TaskPool, List<TaskInfo>>>();
            lock (_activeTasks)
            {
                List<TaskInfo> tasks = _activeTasks.Select(p => p.TaskInfo).ToList();
                result.Add(new KeyValuePair<TaskPool, List<TaskInfo>>(TaskPool.Active, tasks));
            }
            lock (_inactiveTasks)
            {
                List<TaskInfo> tasks = _inactiveTasks.Select(p => p.TaskInfo).ToList();
                result.Add(new KeyValuePair<TaskPool, List<TaskInfo>>(TaskPool.Inactive, tasks));
            }
            lock (_tasksQueue)
            {
                List<TaskInfo> tasks = _tasksQueue.Select(p => p.TaskInfo).ToList();
                result.Add(new KeyValuePair<TaskPool, List<TaskInfo>>(TaskPool.Queue, tasks));
            }
            return result;
        }

        internal bool DeleteAll(TaskPool pool)
        {
            Console.WriteLine("[bulk delete]" + pool);
            switch (pool)
            {
                case TaskPool.Queue:
                    lock (_tasksQueue)
                    {
                        _tasksQueue.Clear();
                    }
                    return true;

                case TaskPool.Inactive:
                    lock (_inactiveTasks)
                    {
                        _inactiveTasks.Clear();
                    }
                    return true;
                case TaskPool.Active:
                    lock (_activeTasks)
                    {
                        foreach (RemoteTask task in _activeTasks)
                        {
                            try
                            {
                                task.Stop();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("[bulk delete] Cannot delete/stop task with id: " + task.Id + ". " +
                                                  ex.Message);
                            }
                        }
                    }
                    return true;
                default:
                    return false;
            }
        }


        protected void PerformWork(object sender, ElapsedEventArgs e)
        {
            lock (_activeTasks)
            {
                if (_activeTasks.Any(p => p.IsActive == false) == true)
                {
                    List<RemoteTask> tasksToMove = _activeTasks.Where(p => p.IsActive == false).ToList();
                    foreach (RemoteTask task in tasksToMove)
                    {
                        Console.WriteLine("Finished: " + task);
                        _activeTasks.Remove(task);
                    }
                    lock (_inactiveTasks)
                    {
                        _inactiveTasks.AddRange(tasksToMove);
                    }
                }
                while (_activeTasks.Count < Configuration.ActiveTasksCount && _tasksQueue.Any())
                {
                    lock (_tasksQueue)
                    {
                        if (_tasksQueue.Any() == true)
                        {
                            RemoteTask task = _tasksQueue.First();
                            task.Run();
                            Console.WriteLine("Started: " + task);
                            _tasksQueue.RemoveAt(0);
                            _activeTasks.Add(task);
                        }
                    }
                }
            }
        }

        private void CleanTimedOutTasks(object sender, ElapsedEventArgs e)
        {
            lock (_activeTasks)
            {
                List<RemoteTask> toBeRemoved =
                    _activeTasks.FindAll(
                        p =>
                        DateTime.UtcNow.Subtract(p.CalculationStartTimestamp).TotalMilliseconds >=
                        Configuration.TasksCalculationTimeout);
                //if tasks are in progress, stop threads before deleting them
                toBeRemoved.Where(p => p.Status == TaskStatus.InProgress).ToList().ForEach(p => p.Stop());

                if (toBeRemoved.Count > 0)
                {
                    Console.WriteLine("Removing Timed Out Tasks in Progress | Count: " + toBeRemoved.Count);

                    foreach (RemoteTask calculationTask in toBeRemoved)
                    {
                        Console.WriteLine("* To Remove:  " + calculationTask);
                        Console.WriteLine("** " +
                                          DateTime.UtcNow.Subtract(calculationTask.CreationTimestamp).TotalMilliseconds);
                    }
                    _activeTasks.RemoveAll(
                        p =>
                        DateTime.UtcNow.Subtract(p.CalculationStartTimestamp).TotalMilliseconds >=
                        Configuration.TasksCalculationTimeout);
                }
            }
        }

        private void CleanInactiveTasks(object sender, ElapsedEventArgs e)
        {
            lock (_inactiveTasks)
            {
                List<RemoteTask> toBeRemoved =
                    _inactiveTasks.FindAll(
                        p =>
                        DateTime.UtcNow.Subtract(p.CalculationFinishTimestamp).TotalMilliseconds >=
                        Configuration.CalculatedTasksTimeout);

                if (toBeRemoved.Count > 0)
                {
                    Console.WriteLine("Removing Timed Out inactive tasks | Count: " + toBeRemoved.Count);

                    foreach (RemoteTask calculationTask in toBeRemoved)
                    {
                        Console.WriteLine("* To Remove:  " + calculationTask);
                        Console.WriteLine("** " +
                                          DateTime.UtcNow.Subtract(calculationTask.CreationTimestamp).TotalMilliseconds);
                    }
                    _inactiveTasks.RemoveAll(
                        p =>
                        DateTime.UtcNow.Subtract(p.CalculationFinishTimestamp).TotalMilliseconds >=
                        Configuration.CalculatedTasksTimeout);
                }
            }
        }

        public string GetQueueSize()
        {
            lock (_tasksQueue)
            {
                return _tasksQueue.Count.ToString();
            }
        }

        public string AddNew(RemoteMessage message)
        {
            lock (_tasksQueue)
            {
                var task = new RemoteTask(message);
                _tasksQueue.Add(task);
                Console.WriteLine("Created task: " + task);
                return task.Id;
            }
        }
    }
}