using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System;

namespace RealEstateCRMWinForms.Services
{
    public class LoggingService
    {
        public static void LogAction(string action, string? details = null, int? userId = null)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    dbContext.Database.EnsureCreated();

                    // If no userId provided, try to get from current session
                    if (userId == null)
                    {
                        userId = UserSession.Instance.CurrentUser?.Id;
                    }

                    if (userId == null)
                    {
                        // Skip logging if no user context
                        return;
                    }

                    if (userId.Value <= 0)
                    {
                        // Skip logging if user ID is invalid
                        return;
                    }

                    var log = new Log
                    {
                        Action = action,
                        UserId = userId.Value,
                        Timestamp = DateTime.Now,
                        Details = details
                    };

                    dbContext.Logs.Add(log);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log to debug output if database logging fails
                System.Diagnostics.Debug.WriteLine($"Failed to log action '{action}': {ex.Message}");
            }
        }
    }
}