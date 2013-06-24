namespace Splash.RemoteServiceContract
{
    public class ObjectParameter : IParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}