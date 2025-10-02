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
        // Centralized password length policy
        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 12;

        private static bool IsPasswordLengthValid(string? password) =>
            !string.IsNullOrEmpty(password) && password.Length >= MinPasswordLength && password.Length <= MaxPasswordLength;

        /// <summary>
        /// Validates password length and returns a user-facing error if invalid.
        /// </summary>
        public static bool TryValidatePassword(string? password, out string? error)
        {
            if (string.IsNullOrEmpty(password))
            {
                error = $"Password is required (must be {MinPasswordLength}-{MaxPasswordLength} characters).";
                return false;
            }
            if (password.Length < MinPasswordLength || password.Length > MaxPasswordLength)
            {
                error = $"Password must be {MinPasswordLength}-{MaxPasswordLength} characters.";
                return false;
            }
            error = null;
            return true;
        }

        public AuthenticationService() { }

        // Centralized lockout policy (applies to ALL roles including broker)
        public const int LockoutThreshold = 5;          // failed attempts before lockout
        public const int LockoutDurationSeconds = 10;    // lockout duration

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
                // Broker shortcut credentials still respect lockout
                if (string.Equals(email?.Trim(), "broker@crm.local", StringComparison.OrdinalIgnoreCase) && string.Equals(password, "broker123"))
                {
                    try
                    {
                        using var brokerCtx = DbContextHelper.CreateDbContext();
                        var existingBroker = brokerCtx.Users.FirstOrDefault(u => u.Email == "broker@crm.local");
                        if (existingBroker != null)
                        {
                            if (existingBroker.LockoutEnd.HasValue && existingBroker.LockoutEnd.Value > DateTime.UtcNow)
                                return null; // locked

                            if (existingBroker.FailedLoginAttempts != 0 || existingBroker.LockoutEnd != null)
                            {
                                existingBroker.FailedLoginAttempts = 0;
                                existingBroker.LockoutEnd = null;
                                brokerCtx.SaveChanges();
                            }
                            UserSession.Instance.CurrentUser = existingBroker;
                            LoggingService.LogAction("User Login", $"Broker {existingBroker.FullName} logged in");
                            return existingBroker;
                        }
                        else
                        {
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
                            brokerCtx.Users.Add(brokerUser);
                            brokerCtx.SaveChanges();
                            UserSession.Instance.CurrentUser = brokerUser;
                            LoggingService.LogAction("User Login", $"Broker {brokerUser.FullName} logged in (seeded)");
                            return brokerUser;
                        }
                    }
                    catch
                    {
                        var fallbackBroker = new User
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
                        UserSession.Instance.CurrentUser = fallbackBroker;
                        LoggingService.LogAction("User Login", "Broker fallback login (DB unavailable)");
                        return fallbackBroker;
                    }
                }

                using var dbContext = DbContextHelper.CreateDbContext();
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.IsActive);
                if (user == null) return null;

                // Lockout checks
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
                    return null;
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value <= DateTime.UtcNow)
                {
                    user.LockoutEnd = null;
                    user.FailedLoginAttempts = 0;
                    dbContext.SaveChanges();
                }

                if (VerifyPassword(password, user.PasswordHash))
                {
                    if (!user.IsEmailVerified) return null;
                    if (user.FailedLoginAttempts != 0 || user.LockoutEnd != null)
                    {
                        user.FailedLoginAttempts = 0;
                        user.LockoutEnd = null;
                        dbContext.SaveChanges();
                    }
                    UserSession.Instance.CurrentUser = user;
                    LoggingService.LogAction("User Login", $"User {user.FullName} logged in");
                    return user;
                }
                else
                {
                    user.FailedLoginAttempts += 1;
                    int remaining = Math.Max(0, LockoutThreshold - user.FailedLoginAttempts);
                    if (user.FailedLoginAttempts >= LockoutThreshold)
                    {
                        user.LockoutEnd = DateTime.UtcNow.AddSeconds(LockoutDurationSeconds);
                        LoggingService.LogAction("Account Locked", $"User {user.Email} locked out after {LockoutThreshold} failed attempts until {user.LockoutEnd:u}");
                    }
                    else
                    {
                        LoggingService.LogAction("Failed Login", $"Failed login for {user.Email}. Attempts={user.FailedLoginAttempts}, Remaining={remaining}");
                    }
                    dbContext.SaveChanges();
                    return null;
                }
            }
            catch
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

                // Enforce password length policy (8-12 chars)
                if (!IsPasswordLengthValid(password))
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

                // If created by a broker and the assigned role is Agent or Client, log explicit creation
                if (createdByBroker && (assignedRole == UserRole.Agent || assignedRole == UserRole.Client))
                {
                    var creatorId = UserSession.Instance.CurrentUser?.Id;
                    LoggingService.LogAction(
                        "Created User Account",
                        $"Broker created {assignedRole} account for {user.Email} (UserId={user.Id})",
                        creatorId);
                }

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

                var originalEmail = user.Email; // capture for logging

                if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = dbContext.Users.Any(u => u.Email == newEmail && u.Id != userId);
                    if (exists) return false;
                    user.Email = newEmail;
                }

                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (!IsPasswordLengthValid(newPassword)) return false;
                    user.PasswordHash = HashPassword(newPassword);
                }

                dbContext.SaveChanges();

                // Log changes (do not include plaintext password)
                var changedParts = new List<string>();
                if (!string.Equals(originalEmail, user.Email, StringComparison.OrdinalIgnoreCase)) changedParts.Add("email");
                if (!string.IsNullOrEmpty(newPassword)) changedParts.Add("password");
                if (changedParts.Count > 0)
                {
                    LoggingService.LogAction("Credentials Updated", $"Updated {string.Join(", ", changedParts)} for userId={user.Id}");
                }
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

                var originalEmail = user.Email;

                newEmail = (newEmail ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(newEmail)) return false;

                // If changing password, verify current
                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (string.IsNullOrWhiteSpace(currentPassword)) return false;
                    if (!VerifyPassword(currentPassword, user.PasswordHash)) return false;
                    if (!IsPasswordLengthValid(newPassword)) return false;
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

                // Log changes (masked)
                var changed = new List<string>();
                if (!string.Equals(originalEmail, user.Email, StringComparison.OrdinalIgnoreCase)) changed.Add("email");
                if (!string.IsNullOrEmpty(newPassword)) changed.Add("password");
                if (changed.Count > 0)
                {
                    LoggingService.LogAction("Credentials Updated (Verified)", $"Updated {string.Join(", ", changed)} for userId={user.Id}");
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
