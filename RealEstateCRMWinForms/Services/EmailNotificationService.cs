using RealEstateCRMWinForms.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RealEstateCRMWinForms.Services
{
    public class EmailNotificationService
    {
        private readonly MailjetSettings _mailjet;

        public EmailNotificationService()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _mailjet = configuration.GetSection("Mailjet").Get<MailjetSettings>() ?? new MailjetSettings();
        }

        // Public helper to send arbitrary emails (used by Contact dialog)
        public Task SendCustomEmailAsync(string toEmail, string subject, string body)
        {
            return SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendLeadToContactNotificationAsync(RealEstateCRMWinForms.Models.Contact contact)
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
                if (string.IsNullOrWhiteSpace(_mailjet.ApiKey) || string.IsNullOrWhiteSpace(_mailjet.SecretKey) || string.IsNullOrWhiteSpace(_mailjet.SenderEmail))
                {
                    System.Diagnostics.Debug.WriteLine($"Mailjet config missing. Would send to {toEmail}: {subject}");
                    return;
                }

                var fromEmail = _mailjet.SenderEmail;
                var fromName = string.IsNullOrWhiteSpace(_mailjet.SenderName) ? "Real Estate CRM" : _mailjet.SenderName;

                using var http = new HttpClient();
                var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_mailjet.ApiKey}:{_mailjet.SecretKey}"));
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

                var payload = new
                {
                    Messages = new[]
                    {
                        new
                        {
                            From = new { Email = fromEmail, Name = fromName },
                            To = new[] { new { Email = toEmail, Name = "" } },
                            Subject = subject ?? string.Empty,
                            HTMLPart = body ?? string.Empty
                        }
                    }
                };
                var json = JsonSerializer.Serialize(payload);
                var resp = await http.PostAsync("https://api.mailjet.com/v3.1/send", new StringContent(json, Encoding.UTF8, "application/json"));
                if (!resp.IsSuccessStatusCode)
                {
                    var err = await resp.Content.ReadAsStringAsync();
                    throw new Exception($"Mailjet send failed: {(int)resp.StatusCode} {resp.ReasonPhrase} {err}");
                }
                System.Diagnostics.Debug.WriteLine($"Mailjet: email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to send email to {toEmail}: {ex.Message}");
                throw;
            }
        }
    }
}
