using Microsoft.Extensions.Options;
using RecipeManager.ServicesInterfaces;
using RecipeManager.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RecipeManager.Services
{
	public class EmailService : IEmailService
	{
		public EmailService(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}

		public EmailSettings _emailSettings { get; }

		public Task SendEmailAsync(string email, string subject, string message)
		{

			Execute(email, subject, message).Wait();
			return Task.FromResult(0);
		}

		public async Task Execute(string email, string subject, string message)
		{
			try
			{
				string toEmail = string.IsNullOrEmpty(email)
								 ? _emailSettings.ToEmail
								 : email;
				MailMessage mail = new MailMessage()
				{
					From = new MailAddress(_emailSettings.UsernameEmail, "Przepisowo")
				};
				mail.To.Add(new MailAddress(toEmail));

				mail.Subject = subject;
				mail.Body = message;
				mail.IsBodyHtml = true;
				mail.Priority = MailPriority.High;

				using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.SecondaryPort))
				{
					smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
					smtp.EnableSsl = true;
					await smtp.SendMailAsync(mail);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Błąd wywyłania e-mail");
			}
		}
	}
}
