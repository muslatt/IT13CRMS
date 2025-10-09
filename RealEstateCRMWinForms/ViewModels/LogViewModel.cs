using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RealEstateCRMWinForms.ViewModels
{
    public class LogViewModel
    {
        public List<LogEntry> GetLogs()
        {
            using (var dbContext = DbContextHelper.CreateDbContext())
            {
                var logs = dbContext.Logs
                    .Include(l => l.User)
                    .OrderByDescending(l => l.Timestamp)
                    .Select(l => new LogEntry
                    {
                        Id = l.Id,
                        Action = l.Action,
                        UserId = l.UserId,
                        Timestamp = l.Timestamp,
                        Details = l.Details,
                        UserFullName = l.User != null ? l.User.FullName : "Unknown User",
                        PropertyId = l.PropertyId
                    })
                    .ToList();

                return logs
                    .Where(IsPropertyRelatedLog)
                    .ToList();
            }
        }

        public List<LogEntry> GetLogsForProperty(int propertyId)
        {
            using (var dbContext = DbContextHelper.CreateDbContext())
            {
                // Try to get property title for fallback matching of older logs
                string? propertyTitle = dbContext.Properties
                    .Where(p => p.Id == propertyId)
                    .Select(p => p.Title)
                    .FirstOrDefault();

                var logs = dbContext.Logs
                    .Include(l => l.User)
                    .Where(l =>
                        l.PropertyId == propertyId ||
                        (
                            l.Details != null && (
                                // Fallback for older logs that referenced the id in details
                                l.Details.Contains("property " + propertyId) ||
                                // Fallback for older logs that referenced the title in details
                                (propertyTitle != null && propertyTitle != string.Empty && l.Details.Contains(propertyTitle))
                            )
                        )
                    )
                    .OrderByDescending(l => l.Timestamp)
                    .Select(l => new LogEntry
                    {
                        Id = l.Id,
                        Action = l.Action,
                        UserId = l.UserId,
                        Timestamp = l.Timestamp,
                        Details = l.Details,
                        UserFullName = l.User != null ? l.User.FullName : "Unknown User",
                        PropertyId = l.PropertyId
                    })
                    .ToList();

                return logs;
            }
        }

        private static bool IsPropertyRelatedLog(LogEntry log)
        {
            if (log == null)
            {
                return false;
            }

            if (log.PropertyId.HasValue)
            {
                return true;
            }

            bool actionMentionsProperty = !string.IsNullOrEmpty(log.Action) &&
                log.Action.IndexOf("Property", StringComparison.OrdinalIgnoreCase) >= 0;

            bool detailsMentionProperty = !string.IsNullOrEmpty(log.Details) &&
                log.Details.IndexOf("property", StringComparison.OrdinalIgnoreCase) >= 0;

            return actionMentionsProperty || detailsMentionProperty;
        }
    }

    public class LogEntry
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public int? PropertyId { get; set; }
    }
}
