using NUnit.Framework;
using Xamarin.UITest;
using Xamariners.EndToEnd.Xamarin.Features;

namespace Covid19Radar.UITest.Features.HomePage
{
    [TestFixture(Platform.Android)]
#if __Apple__
    [TestFixture(Platform.iOS)]
#endif

    public partial class LicenseAgreementFeature : FeatureBase
    {
        public LicenseAgreementFeature(Platform platform) : base(platform)
        {
        }
    }
}