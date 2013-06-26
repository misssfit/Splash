using System;
using System.Collections.Generic;

namespace Splash.RemoteMethodsContract
{
    public class RemoteMethodInfo
    {
        public string Name { get; set; }
        public List<string> Input { get; set; }

        public Type ObjectType { get; set; }
    }
}