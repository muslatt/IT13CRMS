using RealEstateCRMWinForms.Models;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace RealEstateCRMWinForms.Services
{
    public class EmailNotificationService
    {
        private readonly EmailSettings _emailSettings;

        public EmailNotificationService()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
        }

        public async Task SendLeadToContactNotificationAsync(Contact contact)
        {
            try
            {
                var subject = "Welcome to Our Real Estate Services!";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                            <h2 style='color: #2980b9; text-align: center;'>Welcome to Our Real Estate Family!</h2>
                            <p>Dear {contact.FullName},</p>
                            <p>We're excited to inform you that you've been added to our contact database as a valued client.</p>
                            <p>As one of our contacts, you'll receive:</p>
                            <ul>
                                <li>Priority access to new property listings</li>
                                <li>Market updates and insights</li>
                                <li>Personalized property recommendations</li>
                                <li>Exclusive invitations to property viewings</li>
                            </ul>
                            <p>Our team is committed to helping you find the perfect property that meets your needs.</p>
                            <p>If you have any questions or would like to discuss your property requirements, please don't hesitate to contact us.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                            <p style='color: #666; font-size: 12px;'>
                                Best regards,<br>
                                Real Estate CRM Team<br>
                                Contact Type: {contact.Type}
                            </p>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(contact.Email, subject, body);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to send lead to contact notification: {ex.Message}");
            }
        }

        public async Task SendDealStatusUpdateNotificationAsync(Deal deal, string oldStatus, string newStatus)
        {
            try
            {
                if (deal.Contact == null) return;

                var subject = $"Deal Status Update - {deal.Title}";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                            <h2 style='color: #2980b9; text-align: center;'>Deal Status Update</h2>
                            <p>Dear {deal.Contact.FullName},</p>
                            <p>We wanted to update you on the status of your deal:</p>
                            <div style='background-color: white; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                <h3 style='color: #333; margin-top: 0;'>{deal.Title}</h3>
                                <p><strong>Description:</strong> {deal.Description}</p>
                                <p><strong>Previous Status:</strong> <span style='color: #dc3545;'>{oldStatus}</span></p>
                                <p><strong>New Status:</strong> <span style='color: #28a745;'>{newStatus}</span></p>
                                {(deal.Value.HasValue ? $"<p><strong>Deal Value:</strong> ${deal.Value:N2}</p>" : "")}
                                {(!string.IsNullOrEmpty(deal.Notes) ? $"<p><strong>Notes:</strong> {deal.Notes}</p>" : "")}
                            </div>
                            <p>We'll continue to keep you updated as your deal progresses.</p>
                            <p>If you have any questions about this update, please feel free to contact us.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                            <p style='color: #666; font-size: 12px;'>
                                Best regards,<br>
                                Real Estate CRM Team<br>
                                Updated on: {DateTime.Now:MMM dd, yyyy 'at' h:mm tt}
                            </p>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(deal.Contact.Email, subject, body);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to send deal status update notification: {ex.Message}");
            }
        }

        public async Task SendNewDealNotificationAsync(Deal deal)
        {
            try
            {
                if (deal.Contact == null) return;

                var subject = $"New Deal Created - {deal.Title}";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                            <h2 style='color: #2980b9; text-align: center;'>New Deal Created</h2>
                            <p>Dear {deal.Contact.FullName},</p>
                            <p>We're excited to inform you that a new deal has been created for you:</p>
                            <div style='background-color: white; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                <h3 style='color: #333; margin-top: 0;'>{deal.Title}</h3>
                                <p><strong>Description:</strong> {deal.Description}</p>
                                <p><strong>Status:</strong> <span style='color: #28a745;'>{deal.Status}</span></p>
                                {(deal.Value.HasValue ? $"<p><strong>Deal Value:</strong> ${deal.Value:N2}</p>" : "")}
                                {(deal.Property != null ? $"<p><strong>Property:</strong> {deal.Property.Title}</p>" : "")}
                                {(!string.IsNullOrEmpty(deal.Notes) ? $"<p><strong>Notes:</strong> {deal.Notes}</p>" : "")}
                            </div>
                            <p>Our team will be working diligently to move this deal forward. We'll keep you updated on any progress.</p>
                            <p>If you have any questions about this deal, please don't hesitate to contact us.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                            <p style='color: #666; font-size: 12px;'>
                                Best regards,<br>
                                Real Estate CRM Team<br>
                                Created on: {deal.CreatedAt:MMM dd, yyyy 'at' h:mm tt}
                            </p>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(deal.Contact.Email, subject, body);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to send new deal notification: {ex.Message}");
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Skip email sending if configuration is not properly set
                if (string.IsNullOrEmpty(_emailSettings.SenderEmail) || 
                    string.IsNullOrEmpty(_emailSettings.SenderPassword) ||
                    _emailSettings.SenderEmail == "your-email@gmail.com")
                {
                    System.Diagnostics.Debug.WriteLine($"Email configuration not set. Would send to {toEmail}: {subject}");
                    return;
                }

                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new System.Net.NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = _emailSettings.EnableSsl,
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await smtpClient.SendMailAsync(mailMessage);
                System.Diagnostics.Debug.WriteLine($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to send email to {toEmail}: {ex.Message}");
                throw;
            }
        }
    }
}