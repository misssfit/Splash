using System;
using System.Collections.Generic;

namespace McdaMethodsContract
{
    public class McdaMethodInfo
    {
        public string Name { get; set; }
        public List<string> Input { get; set; }

        public Type ObjectType { get; set; }
    }
}