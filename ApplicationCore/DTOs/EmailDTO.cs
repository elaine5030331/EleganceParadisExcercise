using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs
{
    public class EmailDTO
    {
        /// <summary>
        /// 寄件人(預設：EleganceParadis Servie)
        /// </summary>
        public string MailFrom { get; set; }
        /// <summary>
        /// 寄件人信箱(預設：eleganceparadis@gmail.com)
        /// </summary>
        public string MailFromEmail { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string MailTo { get; set; }
        /// <summary>
        /// 收件人信箱
        /// </summary>
        public string MailToEmail { get; set; }
        /// <summary>
        /// 主旨
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 信件內容
        /// </summary>
        public string HTMLContent { get; set; }
    }
}
