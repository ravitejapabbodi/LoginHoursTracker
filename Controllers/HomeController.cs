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
                TargetHoursHours = 8,
                TargetHoursMinutes=0
            };

            return View(model);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Index(PunchLogViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // return the view with the same model to show validation messages
                return View(model);
            }

            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            double totalTarget = model.TargetHours;
            var loginTime = TimeSpan.Zero;
            var breakTime = TimeSpan.Zero;

            var lines = model.RawLogText
                             .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                             .Select(x => x.Trim())
                             .Where(x => !string.IsNullOrWhiteSpace(x))
                             .ToList();

            var punchLogs = new List<PunchLog>();

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
                    PunchTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istZone)
                });
            }

            punchLogs = punchLogs.OrderBy(p => p.PunchTime).ToList();

            for (int i = 0; i < punchLogs.Count - 1; i += 2)
            {
                loginTime += punchLogs[i + 1].PunchTime - punchLogs[i].PunchTime;

                if (i + 2 < punchLogs.Count)
                {
                    breakTime += punchLogs[i + 2].PunchTime - punchLogs[i + 1].PunchTime;
                }
            }

            var targetLogin = TimeSpan.FromHours(model.TargetHours);
            var remaining = targetLogin - loginTime;

            if (remaining > TimeSpan.Zero && punchLogs.Any())
            {
                model.EstimatedLogout = punchLogs.Last().PunchTime + remaining;
            }

            model.TotalLogin = loginTime;
            model.TotalBreak = breakTime;

            return View(model);
        }

    }
}
