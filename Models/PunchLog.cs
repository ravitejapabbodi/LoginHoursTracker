// Models/PunchLog.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace LoginHoursTracker.Models
{
    public class PunchLog
    {
        [Display(Name = "Punch Time")]
        public DateTime PunchTime { get; set; }
    }
}
