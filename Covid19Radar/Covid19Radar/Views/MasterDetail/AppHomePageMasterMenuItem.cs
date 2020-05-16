using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Views
{

    public class AppHomePageMasterMenuItem
    {
        public AppHomePageMasterMenuItem()
        {
            TargetType = typeof(AppHomePageMasterMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}