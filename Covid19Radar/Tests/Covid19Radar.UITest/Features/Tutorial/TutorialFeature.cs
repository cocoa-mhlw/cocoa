using NUnit.Framework;
using Xamarin.UITest;
using Xamariners.EndToEnd.Xamarin.Features;

namespace Covid19Radar.UITest.Features.Tutorial
{
    [TestFixture(Platform.Android)]
#if __Apple__
    [TestFixture(Platform.iOS)]
#endif

    public partial class TutorialFeature : FeatureBase
    {
        public TutorialFeature(Platform platform) : base(platform)
        {
        }
    }
}