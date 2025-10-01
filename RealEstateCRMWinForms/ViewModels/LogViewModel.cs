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
                dbContext.Database.EnsureCreated();

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
                        UserFullName = l.User != null ? l.User.FullName : "Unknown User"
                    })
                    .ToList();

                return logs;
            }
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
    }
}