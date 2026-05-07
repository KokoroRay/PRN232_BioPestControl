using identity_service.Services.Interfaces;

namespace identity_service.Services.Implements
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            // Dummy implementation for now until actual SMTP keys are provided
            Console.WriteLine($"[EMAIL SENT TO {to}] Subject: {subject} | Body: {body}");
            return Task.CompletedTask;
        }
    }
}
