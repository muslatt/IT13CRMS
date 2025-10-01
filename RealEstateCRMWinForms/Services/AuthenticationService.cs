using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace RealEstateCRMWinForms.Services
{
    public class AuthenticationService
    {
        public AuthenticationService() { }
        public bool IsEmailInUse(string email)
        {
            try
            {
                using var db = DbContextHelper.CreateDbContext();
                email = (email ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(email)) return false;
                return db.Users.Any(u => u.Email == email);
            }
            catch
            {
                // Be conservative; treat as in-use if we cannot verify
                return true;
            }
        }
        public User? Authenticate(string email, string password)
        {
            try
            {
                // Hard-coded Broker account (temporary)
                // Email: broker@crm.local, Password: broker123
                if (string.Equals(email?.Trim(), "broker@crm.local", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(password, "broker123"))
                {
                    // Set session immediately so broker can log in even if DB is unavailable
                    var brokerUser = new User
                    {
                        FirstName = "Broker",
                        LastName = "Admin",
                        Email = "broker@crm.local",
                        PasswordHash = HashPassword("broker123"),
                        CreatedAt = DateTime.Now,
                        IsActive = true,
                        IsEmailVerified = true,
                        RoleInt = (int)UserRole.Broker
                    };
                    UserSession.Instance.CurrentUser = brokerUser;

                    // Best-effort: persist to DB if possible, but do not fail login if DB is down
                    try
                    {
                        using var seedContext = DbContextHelper.CreateDbContext();
                        var persisted = seedContext.Users.FirstOrDefault(u => u.Email == "broker@crm.local");
                        if (persisted == null)
                        {
                            seedContext.Users.Add(brokerUser);
                            seedContext.SaveChanges();
                        }
                        else
                        {
                            // Replace session with persisted entity for consistency
                            brokerUser = persisted;
                            UserSession.Instance.CurrentUser = brokerUser;
                        }
                    }
                    catch { /* ignore persistence errors */ }

                    LoggingService.LogAction("User Login", $"Broker {brokerUser.FullName} logged in");
                    return brokerUser;
                }

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
                    LoggingService.LogAction("User Login", $"User {user.FullName} logged in");
                    return user;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Register(string firstName, string lastName, string email, string password, Models.UserRole? role = null)
        {
            // Backwards-compatible wrapper that uses the async implementation
            return RegisterAsync(firstName, lastName, email, password, role).GetAwaiter().GetResult();
        }

        public async Task<bool> RegisterAsync(string firstName, string lastName, string email, string password, Models.UserRole? role = null)
        {
            try
            {
                using var dbContext = DbContextHelper.CreateDbContext();

                if (dbContext.Users.Any(u => u.Email == email))
                {
                    return false;
                }

                var createdByBroker = UserSession.Instance.CurrentUser?.Role == UserRole.Broker;
                var isFirstUser = !dbContext.Users.Any();

                // Determine assigned role: explicit role parameter wins; otherwise first user becomes Broker, others default to Agent
                var assignedRole = role ?? (isFirstUser ? UserRole.Broker : UserRole.Agent);

                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PasswordHash = HashPassword(password),
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    // If the account is being created by a Broker (session), mark as verified and do not send token
                    IsEmailVerified = createdByBroker,
                    EmailVerificationToken = createdByBroker ? null : GenerateVerificationToken(),
                    EmailVerificationSentAt = createdByBroker ? null : DateTime.Now,
                    RoleInt = (int)assignedRole
                };

                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                // If the user is registering as a Client, automatically create a Lead entry
                if (assignedRole == UserRole.Client)
                {
                    var lead = new Models.Lead
                    {
                        FullName = $"{firstName} {lastName}".Trim(),
                        Email = email,
                        Phone = "", // Can be updated later
                        Type = "Lead",
                        Occupation = "",
                        Salary = null,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    dbContext.Leads.Add(lead);
                    dbContext.SaveChanges();

                    LoggingService.LogAction("Lead Created from Registration",
                        $"Lead '{lead.FullName}' automatically created from client registration");
                }

                // Log the action
                LoggingService.LogAction("User Registered", $"User '{user.FullName}' registered as {user.Role}");

                if (createdByBroker)
                {
                    // Send a welcome button email and credentials immediately
                    await SendWelcomeEmailAsync(email, firstName, lastName);
                    await SendCredentialsEmailAsync(email, password);
                }
                else
                {
                    if (!string.IsNullOrEmpty(user.EmailVerificationToken))
                    {
                        await SendVerificationEmailAsync(email, user.EmailVerificationToken);
                    }
                }

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

        private async Task SendVerificationEmailAsync(string email, string token)
        {
            try
            {
                var subject = "Email Verification - Real Estate CRM";
                var bodyHtml = $@"
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
                        </html>";

                var mail = new EmailNotificationService();
                await mail.SendCustomEmailAsync(email, subject, bodyHtml);
                System.Diagnostics.Debug.WriteLine($"Verification email sent via provider to {email}");
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

        private async Task SendWelcomeEmailAsync(string email, string firstName, string lastName)
        {
            try
            {
                var displayName = ($"{firstName} {lastName}").Trim();
                var confirmUrl = "https://example.com/welcome"; // Placeholder action button
                var body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                                <h2 style='color: #2980b9; text-align: center;'>Welcome to Real Estate CRM!</h2>
                                <p>Hello {System.Net.WebUtility.HtmlEncode(displayName)},</p>
                                <p>Your account was created by a Broker and is ready to use.</p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='{confirmUrl}' style='background-color: #2980b9; color: white; padding: 12px 20px; font-size: 16px; border-radius: 5px; font-weight: bold; text-decoration:none;'>Confirm Account</a>
                                </div>
                                <p style='color: #666;'>If you did not expect this email, please contact your Broker.</p>
                                <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                                <p style='color: #999; font-size: 12px;'>Best regards,<br>Real Estate CRM Team</p>
                            </div>
                        </body>
                        </html>";
                var mail = new EmailNotificationService();
                await mail.SendCustomEmailAsync(email, "Your Real Estate CRM Account", body);
                System.Diagnostics.Debug.WriteLine($"Welcome email sent via provider to {email}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to send welcome email: {ex.Message}");
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

                    // If this user was created by a Broker, we may have stored the password to send now
                    if (!string.IsNullOrEmpty(user.PendingPasswordEncrypted))
                    {
                        var plain = CredentialProtector.Unprotect(user.PendingPasswordEncrypted);
                        if (!string.IsNullOrWhiteSpace(plain))
                        {
                            try
                            {
                                SendCredentialsEmailAsync(user.Email, plain);
                            }
                            catch { /* log if needed */ }
                        }

                        // Clear the pending credential regardless
                        user.PendingPasswordEncrypted = null;
                        dbContext.SaveChanges();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task SendCredentialsEmailAsync(string email, string password)
        {
            try
            {
                var subject = "Your Real Estate CRM Credentials";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                            <h2 style='color: #2980b9; text-align: center;'>Welcome Aboard!</h2>
                            <p>Your email has been verified successfully. Here are your login credentials:</p>
                            <div style='background-color: #ffffff; padding: 15px; border-radius: 6px; border: 1px solid #e9ecef;'>
                                <p><strong>Email:</strong> {System.Net.WebUtility.HtmlEncode(email)}</p>
                                <p><strong>Password:</strong> {System.Net.WebUtility.HtmlEncode(password)}</p>
                            </div>
                            <p style='margin-top: 16px;'>For security, consider changing your password after you first log in.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                            <p style='color: #999; font-size: 12px;'>Best regards,<br>Real Estate CRM Team</p>
                        </div>
                    </body>
                    </html>";
                var mail = new EmailNotificationService();
                await mail.SendCustomEmailAsync(email, subject, body);
                System.Diagnostics.Debug.WriteLine($"Credentials email sent to {email}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to send credentials email: {ex.Message}");
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
                    SendVerificationEmailAsync(email, newToken);
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

        public bool UpdateCredentials(int userId, string newEmail, string? newPassword)
        {
            try
            {
                using var dbContext = DbContextHelper.CreateDbContext();
                var user = dbContext.Users.FirstOrDefault(u => u.Id == userId && u.IsActive);
                if (user == null) return false;

                newEmail = (newEmail ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(newEmail)) return false;

                if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = dbContext.Users.Any(u => u.Email == newEmail && u.Id != userId);
                    if (exists) return false;
                    user.Email = newEmail;
                }

                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (newPassword.Length < 6) return false;
                    user.PasswordHash = HashPassword(newPassword);
                }

                dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates credentials and, when a password change is requested, requires the current password to match.
        /// Email changes do not require the current password.
        /// </summary>
        public bool UpdateCredentialsRequiringCurrentPassword(int userId, string newEmail, string? newPassword, string? currentPassword)
        {
            try
            {
                using var dbContext = DbContextHelper.CreateDbContext();
                var user = dbContext.Users.FirstOrDefault(u => u.Id == userId && u.IsActive);
                if (user == null) return false;

                newEmail = (newEmail ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(newEmail)) return false;

                // If changing password, verify current
                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (string.IsNullOrWhiteSpace(currentPassword)) return false;
                    if (!VerifyPassword(currentPassword, user.PasswordHash)) return false;
                    if (newPassword.Length < 6) return false;
                }

                if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = dbContext.Users.Any(u => u.Email == newEmail && u.Id != userId);
                    if (exists) return false;
                    user.Email = newEmail;
                }

                if (!string.IsNullOrEmpty(newPassword))
                {
                    user.PasswordHash = HashPassword(newPassword);
                }

                dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
