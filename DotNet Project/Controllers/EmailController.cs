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
        [HttpPost] // Specify that this method should handle HTTP POST requests
        public IActionResult SendEmail(string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("alva.quigley@ethereal.email"));
            email.To.Add(MailboxAddress.Parse("alva.quigley@ethereal.email"));
            email.Subject = "First Email";
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("alva.quigley@ethereal.email", "CKx7np3DNVqKy7Gj8v");
            smtp.Send(email);
            smtp.Disconnect(true);

            return Ok();
        }
    }
}
