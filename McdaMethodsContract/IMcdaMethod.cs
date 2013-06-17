using System.Collections.Generic;

namespace McdaMethodsContract
{
    public interface IMcdaMethod
    {
        McdaMethodInfo MethodMetadata { get; }
        double[][] Calculate(List<KeyValuePair<string, double[][]>> _inputParameters);
    }
}