using Xamarin.Forms;

namespace Covid19Radar.Model
{
    public class StepModel
    {
        public string Description { get; set; }
        public string? Description2 { get; set; }
        public bool HasStepNumber => StepNumber != null;
        public bool HasImage2 => Image2 != null;
        public bool HasDescription2 => Description2 != null;
        public string Image { get; set; }
        public string? Image2 { get; set; }
        public int? StepNumber { get; set; }
        public string Title { get; set; }
    }
}
