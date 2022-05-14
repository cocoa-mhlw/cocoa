/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar
{
    public interface IExposureNotificationEventCallback
    {
        public void OnEnabled() { }
        public void OnDeclined() { }

        public void OnGetTekHistoryAllowed() { }
        public void OnGetTekHistoryDecline() { }

        public void OnPreauthorizeAllowed() { }
    }
}
