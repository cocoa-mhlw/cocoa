using NUnit.Framework;
using Xamarin.UITest;
using Xamariners.EndToEnd.Xamarin.Features;
using Xamariners.EndToEnd.Xamarin.Infrastructure;

namespace Covid19Radar.UITest.Features.StartTutorialPage
{

    [TestFixture(Platform.Android)]
#if __Apple__
    [TestFixture(Platform.iOS)]
#endif

    public partial class StartTutorialPageFeature : FeatureBase
    {
        public StartTutorialPageFeature(Platform platform) : base(platform)
        {
            var bob = RunnerConfiguration.Current;
        }
    }
}