using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CosmeticShop.Helper
{
    public class MailHelper
    {
        public static bool Send(string fromaddresss, string toaddress, string subject, string content)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                var host = configuration["Gmail:Host"];
                var port = int.Parse(configuration["Gmail:Port"]);
                var username = configuration["Gmail:Username"];
                var password = configuration["Gmail:Password"];
                var enable = bool.Parse(configuration["Gmail:SMTP:starttls:enable"]);

                var smtpClient = new SmtpClient()
                {
                    Host = host,
                    Port = port,
                    EnableSsl = enable,
                    Credentials = new NetworkCredential(username, password)
                };
                var message = new MailMessage(fromaddresss, toaddress);
                message.Subject = subject;
                message.Body = content;
                message.IsBodyHtml = true;
                smtpClient.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
