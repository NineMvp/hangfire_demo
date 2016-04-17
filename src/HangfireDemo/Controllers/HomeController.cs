using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using HangfireDemo.Models;
using Microsoft.Win32.SafeHandles;

namespace HangfireDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [ValidateInput(false)]
        public ActionResult AddEmailTask(string fromEmail, string toEmail, 
            string subject, string body,
            string sendMode, string recurringType, DateTime sendDate
            , int month, int dayOfWeek, int day, int hour, int minute)
        {

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var model = new EmailMessage
                    {
                        FromEmail = fromEmail,
                        ToEmail = toEmail,
                        Body = body,
                        Subject = subject,
                        SendNow = sendMode == "1",
                        ScheduleTime = sendDate
                    };
                    db.EmailMessages.Add(model);
                    db.SaveChanges();
                    if (sendMode == "1")
                        BackgroundJob.Enqueue(() => EmailSender.SendEmail(model.Id));
                    else if (sendMode == "2")
                        BackgroundJob.Schedule(() => EmailSender.SendEmail(model.Id), TimeSpan.FromTicks(sendDate.Ticks));
                    else if (sendMode == "3")
                    {
                        string cron = "";
                        switch (recurringType)
                        {
                            case "minutely":
                                cron = Cron.Minutely();
                                break;
                            case "hourly":
                                cron = Cron.Hourly(hour);
                                break;
                            case "daily":
                                cron = Cron.Daily(hour, minute);
                                break;
                            case "weekly":
                                cron = Cron.Weekly((DayOfWeek) Enum.Parse(typeof (DayOfWeek), dayOfWeek.ToString()),
                                    hour, minute);
                                break;
                            case "yearly":
                                cron = Cron.Yearly(month, day, hour);
                                break;
                        }
                        RecurringJob.AddOrUpdate(() => EmailSender.SendEmail(model.Id), cron, TimeZoneInfo.Local);
                    }

                }
                return Json(new {ok = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {ok = false, msg = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
