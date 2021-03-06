﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Common;

namespace ServiceRegistry
{
    internal class SlaveController : ActiveObject
    {
        private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();
        private readonly JavaScriptSerializer _javaScriptSerializer = new JavaScriptSerializer();
        private object _lock = new object();
        private Dictionary<string, Socket> _socketHolder = new Dictionary<string, Socket>();

        protected override void PerformAction()
        {
            var myList = new TcpListener(IPAddress.Any, 8001);

            /* Start Listeneting at the specified port */
            myList.Start();

            Console.WriteLine("The SlaveController is running at port 8001");
            Console.WriteLine("The local End point is  :" + myList.LocalEndpoint);
            Console.WriteLine("Waiting for a connection.....");
            while (IsActive)
            {
                // Accept will block until someone connects                       
                Socket socket = myList.AcceptSocket();
                Console.WriteLine("Connection accepted from " + socket.RemoteEndPoint);

                var remoteEndpoint = socket.RemoteEndPoint.ToString();

                lock (_lock)
                {
                    if (_socketHolder.ContainsKey(remoteEndpoint) == false)
                    {
                        _socketHolder.Add(remoteEndpoint, socket);
                        var thread = new Thread(ReadSocket);
                        thread.Start(remoteEndpoint);
                    }
                }

            }

            myList.Stop();
        }

        public void ReadSocket(object remoteEndpoint)
        {
            Socket socket = _socketHolder[(string)remoteEndpoint];
            while (IsActive)
            {
                if (socket.Connected)
                {
                    try
                    {
                        ReceiveMessage(socket);
                    }
                    catch (Exception)
                    {
                        IsActive = false;
                    }
                }
                else
                {
                    IsActive = false;
                }
            }
            _socketHolder.Remove((string)remoteEndpoint);
        }

        private void ReceiveMessage(Socket socket)
        {
            var buffer = new byte[1024];
            int iRx = socket.Receive(buffer);
            var chars = new char[iRx];

            _decoder.GetChars(buffer, 0, iRx, chars, 0);
            var recv = new string(chars);
            HandleMessage(recv);
        }

        private void HandleMessage(string receivedMessage)
        {
            //todo what if read more than one json msg ?
            var measurement = _javaScriptSerializer.Deserialize<Measurement>(receivedMessage);
            Console.WriteLine(measurement);

            SlaveRegistry.Instance.UpdateServiceStatus(measurement);
        }
    }
}