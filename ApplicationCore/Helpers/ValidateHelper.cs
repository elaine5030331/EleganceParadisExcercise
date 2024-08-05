using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApplicationCore.Helpers
{
    public class ValidateHelper
    {
        private const string mobilePattern = @"^09\d{8}$";
        private const string emailPattern = @".*@.*\..*";

        public static bool TryValidateMobile(string mobile, out string mobileErrorMsg)
        {
            mobileErrorMsg = string.Empty;  

            if (!Regex.IsMatch(mobile, mobilePattern))
            {
                mobileErrorMsg = ("電話號碼格式有誤");
                return false;
            }
            return true;
        }

        public static bool TryValidateEmail(string email, out string emailErrorMsg)
        {
            emailErrorMsg = string.Empty;

            if (!Regex.IsMatch(email, emailPattern))
            {
                emailErrorMsg = ("電子郵件格式有誤");
                return false;
            }

            return true;
        }
    }
}
