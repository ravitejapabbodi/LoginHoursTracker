using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LoginHoursTracker.Models
{
    public class PunchLogViewModel
    {
        [Display(Name = "Raw Punch Logs")]
        public string RawLogText { get; set; }

        [Display(Name = "Target Login Hours")]
        public double TargetHours { get; set; } = 8;

        public TimeSpan TotalLogin { get; set; }
        public TimeSpan TotalBreak { get; set; }
        public DateTime? EstimatedLogout { get; set; }
    }

}
