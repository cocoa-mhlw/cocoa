using System.Windows.Input;

namespace Covid19Radar.Model
{
    public class HomeMenuModel
    {
        public string Title { get; set; }
        public ICommand Command { get; set; }
    }
}
