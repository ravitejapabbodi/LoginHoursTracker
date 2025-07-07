using Microsoft.AspNetCore.Mvc;
using LoginHoursTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoginHoursTracker.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new PunchLogViewModel
            {
                RawLogText = "",
                TargetHours = 8
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(PunchLogViewModel model)
        {
            double.TryParse(Request.Form["TargetHoursHours"], out double hours);
            double.TryParse(Request.Form["TargetHoursMinutes"], out double minutes);

            // Combine into one double (e.g. 8.5)
            model.TargetHours = hours + (minutes / 60.0);
            var loginTime = TimeSpan.Zero;
            var breakTime = TimeSpan.Zero;

            var lines = model.RawLogText
                             .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                             .Select(x => x.Trim())
                             .Where(x => !string.IsNullOrWhiteSpace(x))
                             .ToList();

            var punchLogs = new List<PunchLog>();
            //  Reverse parse and extract only DateTime entries
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                string timeLine = lines[i];

                if (DateTime.TryParse(timeLine, out DateTime time))
                {
                    punchLogs.Add(new PunchLog
                    {
                        PunchTime = time
                    });
                }
            }
            if (punchLogs.Count % 2 != 0)
            {
                punchLogs.Add(new PunchLog
                {
                    PunchTime = DateTime.Now
                });
            }

            // Sort by time for accurate calculations
            punchLogs = punchLogs.OrderBy(p => p.PunchTime).ToList();

            for (int i = 0; i < punchLogs.Count - 1; i += 2)
            {
                // Treat every 2 as [In, Out]
                loginTime += punchLogs[i + 1].PunchTime - punchLogs[i].PunchTime;

                // Optional break time (Out to next In)
                if (i + 2 < punchLogs.Count)
                {
                    breakTime += punchLogs[i + 2].PunchTime - punchLogs[i + 1].PunchTime;
                }
            }

            var targetLogin = TimeSpan.FromHours(model.TargetHours);
            var remaining = targetLogin - loginTime;

            if (remaining > TimeSpan.Zero && punchLogs.Any())
            {
                // Set Estimated Logout as last punch-in time + remaining
                model.EstimatedLogout = punchLogs.Last().PunchTime + remaining;
            }

            model.TotalLogin = loginTime;
            model.TotalBreak = breakTime;

            return View(model);
        }

    }
}
