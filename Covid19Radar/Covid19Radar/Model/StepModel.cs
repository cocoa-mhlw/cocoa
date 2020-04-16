using Xamarin.Forms;

namespace Covid19Radar.Model
{
    public class StepModel
    {
        public string Description { get; set; }
        public bool HasStepNumber => StepNumber != null;
        public ImageSource Image { get; set; }
        public int? StepNumber { get; set; }
        public string Title { get; set; }
    }
}
