using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.WebPages;
using HangfireDemo.Models;

namespace HangfireDemo
{
    public class EmailSender
    {
        public static bool SendEmail(int id)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var email = db.EmailMessages.SingleOrDefault(o => o.Id == id && o.Status != 2);
                    if (email != null)
                    {

                        MailMessage message = new MailMessage();
                        SmtpClient smtp = new SmtpClient();

                        message.From = new MailAddress(email.FromEmail);
                        message.To.Add(new MailAddress(email.ToEmail));
                        message.Subject = email.Subject;
                        message.Body = email.Body;
                        message.IsBodyHtml = true;

                        smtp.Port = ConfigurationManager.AppSettings["smtp.port"].AsInt();
                        smtp.Host = ConfigurationManager.AppSettings["smtp.server"];
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtp.user"], ConfigurationManager.AppSettings["smtp.password"]);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(message);

                        email.ExecutedWhen = DateTime.Now;
                        email.Status = 2;
                        db.SaveChanges();

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}