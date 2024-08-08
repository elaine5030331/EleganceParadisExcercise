using System.Text;

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

        public static string CreateOrderEmailTemplate(CreateOrderEmailRequest request)
        {
            var sb = new StringBuilder();
            foreach(var item in request.OrderDetails)
            {
                var html = @$"
                            <tr>
                                <td> {item.ProductName} </td>
                                <td> {item.Quantity} </td >
                                <td> NT${item.UnitPrice:G29} </td>
                                <td> NT${(item.UnitPrice * item.Quantity):G29} </td>
                            </tr>";
                sb.Append(html);
            }
            var orderDetailItemsHtml = sb.ToString();

            return string.Format(@"
            <div style=""width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; background-color: #f9f9f9; font-family: Arial, sans-serif; color: #333; line-height: 1.6;"">
            <p style=""margin: 10px 0;"">***此為系統信請勿直接回信***</p>
            <h1 style=""color: #6b4f96; text-align: center;"">EleganceParadis 訂單成立</h1>
            <p style=""margin: 10px 0;"">親愛的 <span style=""color: #6b4f96; font-weight: bold;"">{0}</span> 您好，</p>
            <p style=""margin: 10px 0;"">您已於 <span style=""color: #6b4f96; font-weight: bold;"">{1}</span> 建立訂單，總金額為 <span style=""color: #6b4f96; font-weight: bold;"">{2}</span></p>
            <p style=""margin: 10px 0;"">以下為您的訂單編號 {3} 及訂購項目：</p>

            <table style=""width: 100%; border-collapse: collapse; margin: 20px 0;"">
                <thead>
                    <tr>
                        <th style=""padding: 10px; border: 1px solid #ddd; text-align: left; background-color: #f2f2f2;"">商品名稱</th>
                        <th style=""padding: 10px; border: 1px solid #ddd; text-align: left; background-color: #f2f2f2;"">數量</th>
                        <th style=""padding: 10px; border: 1px solid #ddd; text-align: left; background-color: #f2f2f2;"">單價</th>
                        <th style=""padding: 10px; border: 1px solid #ddd; text-align: left; background-color: #f2f2f2;"">小計</th>
                    </tr>
                </thead>
                <tbody>
                    {4}
                </tbody>
            </table>

            <p style=""text-align: right; margin: 10px 0;"">訂單小計: <span style=""color: #6b4f96; font-weight: bold;"">NT$ {6}</span></p>
            <p style=""text-align: right; margin: 10px 0;"">運費: <span style=""color: #6b4f96; font-weight: bold;"">NT$ {5}</span></p>
            <p style=""text-align: right; margin: 10px 0;"">訂單總計: <span style=""color: #6b4f96; font-weight: bold;"">NT$ {2}</span></p>
            </div>",
            request.Purchaser,
            request.OredreDate,
            request.TotalAmount.ToString("G29"),
            request.OrderNo,
            orderDetailItemsHtml,
            request.ShippingFee.ToString("G29"),
            request.SumSubTotal.ToString("G29"));
        }
    }
}
