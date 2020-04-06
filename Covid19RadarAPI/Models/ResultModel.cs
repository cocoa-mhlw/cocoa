using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public class ResultModel
    {
        public static ResultModel Success = new ResultModel() { message = "Success" };
        public string message { get; set; }
    }
}
