using TechTalk.SpecFlow;

namespace Covid19Radar.UITest.Steps
{
    [Binding]
    public class CommonSteps : Xamariners.EndToEnd.Xamarin.SharedSteps.CommonSteps
    {
        public CommonSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            // used for saving screenshots on local runs
            ScreenshotPath = @"C:\screenshots";
        }
    }
}