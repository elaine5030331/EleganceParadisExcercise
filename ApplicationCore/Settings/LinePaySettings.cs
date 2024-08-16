using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Settings
{
    public class LinePaySettings
    {
        public const string LinePaySettingsKey = "LinePaySettings";
        public string ChannelId { get; set; }
        public string ChannelSecret { get; set; }
        public bool IsSandBox { get; set; }
        public string ComfirmURL { get; set; }
        public string CancelURL { get; set; }
    }
}
