using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xamariners.EndToEnd.Xamarin.Infrastructure;

namespace Covid19Radar.UITest.SharedSteps
{
    [Binding]
    public class ExampleSharedSteps : StepBase
    {
        public ExampleSharedSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
            // create any custom steps on either a shared step class
            // or a step class for a specific feature
        }

        [When(@"I do something with ""(.*)""")]
        public void WhenIDoSomethingWith(string something)
        {
           // implement your steps as per example ot doc
           // RunnerConfiguration.Current.App.PressEnter();
           throw new NotImplementedException();
        }

        [Given(@"I See the Repl")]
        [When(@"I See the Repl")]
        [Then(@"I See the Repl")]
        public void ThenISeeTheRepl()
        {
            RunnerConfiguration.Current.App.Repl();
        }

        [When(@"I tap on MasterDetail Hamburger")]
        public void WhenITapOnMasterDetailHamburger()
        {
            if (RunnerConfiguration.Current.App.Query(x => x.Class("ImageButton").Marked("OK")).Any())
                RunnerConfiguration.Current.App.Tap(x => x.Class("ImageButton").Marked("OK"));
            else if (RunnerConfiguration.Current.App.Query(x => x.Class("AppCompatImageButton").Marked("OK")).Any())
                RunnerConfiguration.Current.App.Tap(x => x.Class("AppCompatImageButton").Marked("OK"));
        }
    }
}
