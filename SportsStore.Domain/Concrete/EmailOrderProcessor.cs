using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Text;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "songwei@zhen-tao.com";
        public string MailFromAddress = "sportsstore@example.com";
        public bool UseSsl = true;
        public string Username = "382529946@qq.com";
        public string Password = "";
        public string ServerName = "smtp.qq.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"C:\sports_store_emails";
    }
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;
        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            //using (var smtpClient = new SmtpClient())
            //{
            //    smtpClient.EnableSsl = emailSettings.UseSsl;
            //    smtpClient.Host = emailSettings.ServerName;
            //    smtpClient.Port = emailSettings.ServerPort;
            //    smtpClient.UseDefaultCredentials = false;
            //    smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
            if (emailSettings.WriteAsFile)
            {
                //smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                //smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                //smtpClient.EnableSsl = false;
            }
            StringBuilder body = new StringBuilder()
                .AppendLine("A new order has been submitted")
                .AppendLine("-- - ")
                .AppendLine("Items:");
            foreach (var line in cart.Lines)
            {
                var subtotal = line.Product.Price * line.Quantity;
                body.AppendFormat("{0} x {1} (subtotal: {2:c}",
                line.Quantity,
                line.Product.Name,
                subtotal);
            }
            body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                .AppendLine("---")
                .AppendLine("Ship to:")
                .AppendLine(shippingInfo.Name)
                .AppendLine(shippingInfo.Line1)
                .AppendLine(shippingInfo.Line2 ?? "")
                .AppendLine(shippingInfo.Line3 ?? "")
                .AppendLine(shippingInfo.City)
                .AppendLine(shippingInfo.State ?? "")
                .AppendLine(shippingInfo.Country)
                .AppendLine(shippingInfo.Zip)
                .AppendLine("---")
                .AppendFormat("Gift wrap: {0}", shippingInfo.GiftWrap ? "Yes" : "No");
            //MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress, emailSettings.MailToAddress, "New Order Submit", body.ToString());
            //if (emailSettings.WriteAsFile)
            //{
            //    mailMessage.BodyEncoding = Encoding.ASCII;
            //}
            //smtpClient.Send(mailMessage);

            SendStrMail(emailSettings.MailToAddress, "New Order Submit", body.ToString());
            //}
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="data">邮件内容</param>
        private void SendStrMail(string mail_to, string mail_subject, string mail_body, bool IsHtml = false)
        {
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                if (mail_to.IndexOf(',') > -1)
                {
                    string[] mails = mail_to.Split(',');//多个收信地址用逗号隔开
                    for (int counti = 0; counti < mails.Length; counti++)
                    {
                        if (mails[counti].Trim() != "")
                        {
                            msg.To.Add(mails[counti]);
                        }
                    }
                }
                else
                {
                    msg.To.Add(mail_to);//添加单一收信地址
                }


                msg.From = new System.Net.Mail.MailAddress("MMS@zhen-tao.com", "MMS", System.Text.Encoding.UTF8);
                /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
                msg.Subject = mail_subject;//邮件标题 
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码

                msg.Body = mail_body;
                msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码 
                msg.IsBodyHtml = IsHtml;//是否是HTML邮件 
                msg.Priority = System.Net.Mail.MailPriority.Normal;

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                //client.Credentials = new System.Net.NetworkCredential("songwei@zhen-tao.com", "*********");
                //在zj.com注册的邮箱和密码 
                client.Host = "10.32.32.33";//邮件发送服务器，上面对应的是该服务器上的发信帐号和密码

                client.Send(msg);
            }
            catch (Exception)
            {

            }
        }
    }
}
