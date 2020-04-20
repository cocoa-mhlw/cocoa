using Covid19Radar.UITest.Features;
using NUnit.Framework;
using Xamarin.UITest;

namespace Covid19Radar.UITest
{

    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public partial class StartTutorialPageFeature : BaseLocalFeature
    {
        public StartTutorialPageFeature(Platform platform) : base(platform)
        {
        }
    }
}