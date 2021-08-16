using FinalProject.DataAccesLayer;
using FinalProject.Models;
using FinalProject.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public ContactController(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        #region Message To Me
        public async Task<IActionResult> MessageToMe(string Email, string Subject, string Message, string Name)
        {

            if (Subject == null || Message == null)
            {
                return Content("You can't send message!");
            }

            else
            {
                if (User.Identity.IsAuthenticated)
                {
                    User user = await _userManager.FindByNameAsync(User.Identity.Name);
                    EmailToMe emailToMe = new EmailToMe
                    {
                        Name = user.Name,
                        Email = user.Email,
                        Messages = Message,
                        Subjects = Subject,
                    };
                    await _dbContext.EmailToMes.AddAsync(emailToMe);
                    await _dbContext.SaveChangesAsync();
                    string bodyMes = $"{Message} from  {user.Email}";
                    SendEmail("nihat.osmanov.01@mail.ru", Subject, bodyMes);
                    return Content("You send Message successfull ! Will be in touch with You during the day!");

                }
                else
                {
                    if (Email != null && Name != null && Subject != null && Message != null)
                    {
                        EmailToMe emailToMe = new EmailToMe
                        {
                            Name = Name,
                            Email = Email,
                            Messages = Message,
                            Subjects = Subject,
                        };
                        await _dbContext.EmailToMes.AddAsync(emailToMe);
                        await _dbContext.SaveChangesAsync();
                        string bodyMes = $" Email : {Email} <br> From : {Name} <br> Message : {Message} ";
                        SendEmail("nihat.osmanov.01@mail.ru", Subject, bodyMes);
                        return Content("You send Message successfull ! Will be in touch with You during the day!");
                    }

                    else
                    {
                        return Content("You can't send message!");
                    }
                }

            }
        }

        #endregion

        public void SendEmail(string toMail, string subject, string mesBody)
        {
            string toEmail = toMail;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("nihad.osmanov.no@gmail.com", "9856522nn");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage message = new MailMessage("nihad.osmanov.no@gmail.com", toEmail);
            message.Subject = subject;
            message.Body = mesBody;

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            client.Send(message);

        }
    }
}
