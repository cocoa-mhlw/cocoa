using NUnit.Framework;
using Xamariners.EndToEnd.Xamarin.Infrastructure;

namespace Covid19Radar.UITest
{
    [SetUpFixture]
    public class NUnitAssemblyHooks : NUnitAssemblyHooksBase
    {
        static NUnitAssemblyHooks()
        {
            RunnerConfiguration.CurrentAssembly = typeof(NUnitAssemblyHooks).Assembly;
        }
    }
}