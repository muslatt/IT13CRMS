using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System.Security.Cryptography;
using System.Text;

namespace RealEstateCRMWinForms.Services
{
    public class AuthenticationService
    {
        public User? Authenticate(string email, string password)
        {
            try
            {
                using var dbContext = DbContextHelper.CreateDbContext();
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.IsActive);

                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    // Mark the session with the authenticated user
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

                // Check if user already exists
                if (dbContext.Users.Any(u => u.Email == email))
                {
                    return false;
                }

                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PasswordHash = HashPassword(password),
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                dbContext.Users.Add(user);
                dbContext.SaveChanges();
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
    }
}