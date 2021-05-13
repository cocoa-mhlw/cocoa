/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Covid19Radar.Services.Migration;

namespace Covid19Radar.iOS.Services.Migration
{
    public class PlatformVersionMigrationService : IVersionMigration
    {
        public Task MigrateTo123Async()
        {
            // nothing to do
            return Task.CompletedTask;
        }

        public Task MigrateTo122Async()
        {
            // nothing to do
            return Task.CompletedTask;
        }

        public Task Initialize100Async()
        {
            // nothing to do
            return Task.CompletedTask;
        }
    }
}
