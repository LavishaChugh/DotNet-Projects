using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace DotNet_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {


        //Sending EMAILS
        [HttpPost]
        public IActionResult SendEmail(string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("alva.quigley@ethereal.email"));  //the sender address
            email.To.Add(MailboxAddress.Parse("alva.quigley@ethereal.email"));  //the receiver address
            email.Subject = "First Email";
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);          //server,port 
            smtp.Authenticate("alva.quigley@ethereal.email", "CKx7np3DNVqKy7Gj8v");         //username,password
            smtp.Send(email);
            smtp.Disconnect(true);

            return Ok();
        }
    }
}
