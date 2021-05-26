/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Windows.Input;

namespace Covid19Radar.Model
{
    public class HomeMenuModel
    {
        public string Title { get; set; }
        public ICommand Command { get; set; }
    }
}
