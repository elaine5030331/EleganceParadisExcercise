using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class MailServerOptions
    {
        public const string MailServerSettings = "MailServerSettings";
        public string Host { get; set; }
        public int Port { get; set; }
        public bool IsSSL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
