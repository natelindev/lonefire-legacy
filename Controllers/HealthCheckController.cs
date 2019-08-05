using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace lonefire.Controllers
{
    public class HealthCheckController : Controller
    {
        private readonly IConfiguration _config;
        public HealthCheckController(IConfiguration config) { _config = config; }

        public TimeSpan GetUptime()
        {
            return DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime;
        }

        public TimeSpan GetLastUpdateTime()
        {
            return DateTime.Now - DateTime.Parse(_config["LastBuildTime"] ?? DateTime.Now.ToString());
        }

        public string FormatTimeSpan(TimeSpan time)
        {
            string res;
            if(time.TotalMinutes < 60)
            {
                res = time.Minutes + "分钟";
            }
            else if(time.TotalHours < 24)
            {
                res = time.Hours + "小时";
            }
            else
            {
                res = time.Days + "天";
            }
            return res;
        }
    }
}