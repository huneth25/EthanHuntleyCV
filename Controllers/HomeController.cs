using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EthanHuntleyCV.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EthanHuntleyCV.Controllers;

public class HomeController : Controller
{
    private readonly EmailSettings _emailSettings;

    public HomeController(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Contact(ContactFormModel model)
    {
        if (!ModelState.IsValid)
            return View("Index", model); // Or whatever your view is

        try
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = model.Subject,
                Body = $"<p><strong>From:</strong> {model.Name} ({model.Email})</p><p>{model.Message}</p>",
                IsBodyHtml = true
            };
            mail.To.Add(_emailSettings.SenderEmail);

            // Here is the updated SMTP configuration
            using (var smtp = new SmtpClient("smtp.gmail.com", 587)) // Use Port 587 for TLS
            {
                smtp.Credentials = new NetworkCredential(
                    _emailSettings.SmtpUsername,
                    _emailSettings.SmtpPassword); // Use your Gmail username and app-specific password

                smtp.EnableSsl = true; // This ensures the connection is encrypted
                smtp.Send(mail); // Send the email
            }

            TempData["SuccessMessage"] = "Your message has been sent successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            return RedirectToAction("Index");
        }


    }

    public IActionResult Index()
    {
        return View();
    }
}
