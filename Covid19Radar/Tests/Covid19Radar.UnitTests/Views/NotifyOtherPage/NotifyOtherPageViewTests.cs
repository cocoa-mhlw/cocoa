// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Covid19Radar.Views;
using Prism.Navigation;
using Xunit;

namespace Covid19Radar.UnitTests.Views
{
    public class NotifyOtherPageViewTests
    {
        public NotifyOtherPageViewTests()
        {
        }

        [Theory]
        [InlineData("00000000")]
        [InlineData("27475385")]
        [InlineData("94531418")]
        [InlineData("38338692")]
        [InlineData("44146246")]
        [InlineData("07788600")]
        [InlineData("34028700")]
        [InlineData("81802074")]
        [InlineData("18113949")]
        [InlineData("91595442")]
        [InlineData("10266747")]
        public void BuildNuvigationParams_Success(string processNumber)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters = NotifyOtherPage.BuildNavigationParams(processNumber, navigationParameters);

            Assert.True(navigationParameters.ContainsKey("processNumber"));
            Assert.Equal(processNumber, navigationParameters["processNumber"]);
        }


        [Theory]
        [InlineData("")]
        [InlineData("0")]
        [InlineData("102966")]
        [InlineData("4801434")]
        [InlineData("042835603")]
        [InlineData("3361857071")]
        public void BuildNuvigationParams_InvalidLength(string processNumber)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters = NotifyOtherPage.BuildNavigationParams(processNumber, navigationParameters);

            Assert.False(navigationParameters.ContainsKey("processNumber"));
        }

        [Theory]
        [InlineData("abcdefgh")]
        [InlineData("あいうえおかきく")]
        [InlineData("😘😘😘😘😘😘😘😘")]
        [InlineData("a4801434")]
        [InlineData("a48014349")]
        [InlineData("4801abc4")]
        [InlineData(" 48014349")]
        [InlineData("48014341 ")]
        [InlineData("4801434 ")]
        [InlineData("4801434 1")]
        [InlineData("48014341 1")]
        [InlineData("%204801434")]
        [InlineData("()_+_($#")]
        public void BuildNuvigationParams_InvalidCharactor(string processNumber)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters = NotifyOtherPage.BuildNavigationParams(processNumber, navigationParameters);

            Assert.False(navigationParameters.ContainsKey("processNumber"));
        }
    }
}
