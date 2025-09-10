using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace RealEstateCRMWinForms.Services
{
    public class AuthenticationService
    {
        private readonly EmailSettings _emailSettings;

        public AuthenticationService()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
        }
        public User? Authenticate(string email, string password)
        {
            try
            {
                using var dbContext = DbContextHelper.CreateDbContext();
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.IsActive);

                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    if (!user.IsEmailVerified)
                    {
                        // Block login if email not verified
                        return null;
                    }
                    UserSession.Instance.CurrentUser = user;
                    return user;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Register(string firstName, string lastName, string email, string password)
        {
            try
            {
                using var dbContext = DbContextHelper.CreateDbContext();

                if (dbContext.Users.Any(u => u.Email == email))
                {
                    return false;
                }

                var verificationToken = GenerateVerificationToken();
                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PasswordHash = HashPassword(password),
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsEmailVerified = false,
                    EmailVerificationToken = verificationToken,
                    EmailVerificationSentAt = DateTime.Now
                };

                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                SendVerificationEmail(email, verificationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "RealEstateCRM_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash);
        }

        private string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6).ToUpper();
        }

        private void SendVerificationEmail(string email, string token)
        {
            try
            {
                // Skip email sending if configuration is not properly set
                if (string.IsNullOrEmpty(_emailSettings.SenderEmail) || 
                    string.IsNullOrEmpty(_emailSettings.SenderPassword) ||
                    _emailSettings.SenderEmail == "your-email@gmail.com")
                {
                    System.Diagnostics.Debug.WriteLine($"Email configuration not set. Verification code for {email}: {token}");
                    return;
                }

                var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new System.Net.NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = _emailSettings.EnableSsl,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = "Email Verification - Real Estate CRM",
                    Body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                                <h2 style='color: #2980b9; text-align: center;'>Welcome to Real Estate CRM!</h2>
                                <p>Thank you for registering with us. To complete your registration, please use the verification code below:</p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <span style='background-color: #2980b9; color: white; padding: 15px 25px; font-size: 24px; letter-spacing: 3px; border-radius: 5px; font-weight: bold;'>{token}</span>
                                </div>
                                <p style='color: #666;'>This code will expire in 24 hours.</p>
                                <p style='color: #666;'>If you didn't create an account with us, please ignore this email.</p>
                                <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                                <p style='color: #999; font-size: 12px;'>Best regards,<br>Real Estate CRM Team</p>
                            </div>
                        </body>
                        </html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                smtpClient.Send(mailMessage);
                System.Diagnostics.Debug.WriteLine($"Verification email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                // Log the error and fall back to debug output for development
                System.Diagnostics.Debug.WriteLine($"Failed to send email: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Verification code for {email}: {token}");
                
                // In production, you might want to throw the exception or handle it differently
                // For now, we'll continue as if the email was sent
            }
        }

        public bool VerifyEmail(string email, string token)
        {
            try
            {
                using var dbContext = DbContextHelper.CreateDbContext();
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && !u.IsEmailVerified);
                
                if (user != null && user.EmailVerificationToken == token)
                {
                    // Check if token hasn't expired (24 hours)
                    if (user.EmailVerificationSentAt.HasValue && 
                        DateTime.Now.Subtract(user.EmailVerificationSentAt.Value).TotalHours > 24)
                    {
                        return false; // Token expired
                    }

                    user.IsEmailVerified = true;
                    user.EmailVerificationToken = null;
                    user.EmailVerificationSentAt = null;
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ResendVerificationCode(string email)
        {
            try
            {
                using var dbContext = DbContextHelper.CreateDbContext();
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && !u.IsEmailVerified);
                
                if (user != null)
                {
                    // Generate new verification token
                    var newToken = GenerateVerificationToken();
                    user.EmailVerificationToken = newToken;
                    user.EmailVerificationSentAt = DateTime.Now;
                    
                    dbContext.SaveChanges();
                    SendVerificationEmail(email, newToken);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckUnverifiedUser(string email)
        {
            try
            {
                using var dbContext = DbContextHelper.CreateDbContext();
                return dbContext.Users.Any(u => u.Email == email && !u.IsEmailVerified && u.IsActive);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}