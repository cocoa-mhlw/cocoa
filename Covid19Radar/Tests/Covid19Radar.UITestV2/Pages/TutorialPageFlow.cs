using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

// Aliases Func<AppQuery, AppQuery> with Query
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CovidRadar.UITestV2
{
    /// <summary>
    /// TutorialPageFlowクラス.
    /// </summary>
    public class TutorialPageFlow
    {
        /// <summary>
        /// 初期チュートリアルを実行する.
        /// </summary>
        public void Tutorial()
        {
            TutorialPage1 tutorialPage1 = new TutorialPage1();
            tutorialPage1.AssertTutorialPage1();

            TutorialPage2 tutorialPage2 = tutorialPage1.OpenTutorialPage2();
            tutorialPage2.AssertTutorialPage2();

            TutorialPage3 tutorialPage3 = tutorialPage2.OpenTutorialPage3();
            tutorialPage3.AssertTutorialPage3();

            PrivacyPolicyPage privacyPolicyPage = tutorialPage3.OpenPrivacyPolicyPage();
            privacyPolicyPage.AssertPrivacyPolicyPage();

            TutorialPage4 tutorialPage4 = privacyPolicyPage.OpenTutorialPage4();
            tutorialPage4.AssertTutorialPage4();

            TutorialPage6 tutorialPage6 = tutorialPage4.OpenTutorialPage6();
            tutorialPage6.AssertTutorialPage6();

            tutorialPage6.OpenHomePage();
        }
    }
}
