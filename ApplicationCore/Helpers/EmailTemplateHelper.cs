using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Helpers
{
    public class EmailTemplateHelper
    {
        public static string SignupEmailTemplate(string mailTo, string returnURL)
        {
            return string.Format(@"
            <h1>EleganceParadis 註冊驗證信</h1>
            <p>親愛的 {0} 您好，</p>
            <p>本信件為 EleganceParadis 【電子信箱驗證信】，為確保您的資訊安全，請務必於 15 分鐘內點擊以下連結，驗證您的電子信箱，以維護您的權益。</p>
            <p><a href=""{1}"">點此連結進行EMAIL驗證</a></p>", mailTo, returnURL);
        }

        public static string ForgetPasswordEmailTemplate(string mailTo, string returnURL)
        {
            return string.Format(@"
            <h1>EleganceParadis 重設密碼驗證信</h1>
            <p>親愛的 {0} 您好，</p>
            <p>本信件為 EleganceParadis 【重設密碼驗證信】，為確保您的資訊安全，請務必於 15 分鐘內點擊以下連結，重設您的密碼，以維護您的權益。</p>
            <p><a href=""{1}"">點此連結進行密碼更改</a></p>", mailTo, returnURL);
        }
    }
}
