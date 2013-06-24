using System.Collections.Generic;

namespace Splash.McdaMethodsContract
{
    public interface IMcdaMethod
    {
        McdaMethodInfo MethodMetadata { get; }
        double[][] Calculate(List<KeyValuePair<string, double[][]>> _inputParameters);
    }
}