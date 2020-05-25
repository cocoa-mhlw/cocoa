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
    }
}
