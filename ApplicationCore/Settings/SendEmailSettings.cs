﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Settings
{
    public class SendEmailSettings
    {
        public const string SendEmailSettingsKey = "SendEmailSettings";
        public string VerifyEmailReturnURL { get; set; }
        public string ForgetPasswordReturnURL { get; set; }
    }
}
