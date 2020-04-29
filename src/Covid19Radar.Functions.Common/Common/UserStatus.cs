using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Common
{
    public enum UserStatus
    {
        None,
        Contacted,
        OnSet,
        Suspected,
        Inspection,
        Infection,
        Treatment,
        Recovery
    }
}
