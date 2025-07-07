using System;
using System.ComponentModel.DataAnnotations;

namespace LoginHoursTracker.Models
{
    public class PunchLogViewModel
    {
        [Display(Name = "Raw Punch Logs")]
        [Required(ErrorMessage = "Please enter punch log text.")]
        public string RawLogText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select hours.")]
        [Range(0, 23, ErrorMessage = "Hours must be between 0 and 23.")]
        public int? TargetHoursHours { get; set; }

        [Required(ErrorMessage = "Please select minutes.")]
        [Range(0, 59, ErrorMessage = "Minutes must be between 0 and 59.")]
        public int? TargetHoursMinutes { get; set; }

        // Calculates combined target hours as decimal
        public double TargetHours => (TargetHoursHours ?? 0) + (TargetHoursMinutes ?? 0) / 60.0;

        public TimeSpan TotalLogin { get; set; }
        public TimeSpan TotalBreak { get; set; }
        public DateTime? EstimatedLogout { get; set; }
    }
}
