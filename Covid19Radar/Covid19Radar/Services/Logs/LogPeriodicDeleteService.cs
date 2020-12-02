using Xamarin.Forms;

namespace Covid19Radar.Services.Logs
{
    public interface ILogPeriodicDeleteService
    {
        void Init();
    }

    public class LogPeriodicDeleteService : ILogPeriodicDeleteService
    {
        public void Init() => DependencyService.Get<ILogPeriodicDeleteService>().Init();
    }
}
