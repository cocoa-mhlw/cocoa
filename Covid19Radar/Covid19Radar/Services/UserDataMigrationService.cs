using System;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    public static class UserDataMigrationService
    {
        public async static Task Migrate()
        {
            await MigratePropertiesToSecureStorage(AppConstants.StorageKey.Secret);
            await MigratePropertiesToSecureStorage(AppConstants.StorageKey.UserData);
        }

        private async static Task MigratePropertiesToSecureStorage(string key)
        {
            var maybeMigratedValue = await SecureStorage.GetAsync(key);
            if (maybeMigratedValue == null && Application.Current.Properties.TryGetValue(key, out var originalValue))
            {
                if (originalValue == null)
                {
                    return;
                }
                await SecureStorage.SetAsync(key, originalValue as string);

                Application.Current.Properties.Remove(key);
                await Application.Current.SavePropertiesAsync();
            }
        }
    }
}
