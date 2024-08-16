using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Settings
{
    public class AdminInfoSettings
    {
        public const string AdminInfoSettingsKey = "AdminInfoSettings";
        public string AccountName { get; set; }
        public string Password { get; set; }
    }
}
