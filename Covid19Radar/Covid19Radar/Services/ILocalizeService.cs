using System.Globalization;

namespace Covid19Radar.Services
{
    public interface ILocalizeService
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}