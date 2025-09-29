using System.Text.Json;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.Services
{
    public class EmailLogService
    {
        private readonly string _logPath;

        public EmailLogService()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RealEstateCRMWinForms");
            Directory.CreateDirectory(dir);
            _logPath = Path.Combine(dir, "email_logs.json");
        }

        public async Task AppendAsync(
            int contactId,
            string contactEmail,
            string subject,
            string sentByName,
            UserRole sentByRole,
            string messageId = "",
            string direction = "Outgoing",
            string? sentByRoleOverride = null,
            DateTime? sentAt = null,
            string? bodyHtml = null,
            string? bodyText = null,
            string? headers = null)
        {
            var items = await LoadAsync();
            // Avoid duplicates when messageId is known
            if (!string.IsNullOrWhiteSpace(messageId) && items.Any(i => i.MessageId == messageId))
            {
                return;
            }

            items.Add(new EmailLog
            {
                ContactId = contactId,
                ContactEmail = contactEmail,
                Subject = subject,
                SentAt = sentAt ?? DateTime.Now,
                SentBy = sentByName,
                SentByRole = sentByRoleOverride ?? sentByRole.ToString(),
                MessageId = messageId,
                Direction = direction,
                BodyHtml = bodyHtml ?? string.Empty,
                BodyText = bodyText ?? string.Empty,
                Headers = headers ?? string.Empty
            });
            var json = JsonSerializer.Serialize(items);
            await File.WriteAllTextAsync(_logPath, json);
        }

        public async Task<EmailLog?> GetLastAsync(int contactId)
        {
            var items = await LoadAsync();
            return items.Where(x => x.ContactId == contactId).OrderByDescending(x => x.SentAt).FirstOrDefault();
        }

        public async Task<List<EmailLog>> GetAllAsync(int contactId)
        {
            var items = await LoadAsync();
            // If a negative contactId is provided, return all logs (useful for global views)
            if (contactId < 0)
            {
                return items
                    .OrderByDescending(x => x.SentAt)
                    .ToList();
            }

            return items
                .Where(x => x.ContactId == contactId)
                .OrderByDescending(x => x.SentAt)
                .ToList();
        }

        private async Task<List<EmailLog>> LoadAsync()
        {
            try
            {
                if (!File.Exists(_logPath)) return new List<EmailLog>();
                var json = await File.ReadAllTextAsync(_logPath);
                return JsonSerializer.Deserialize<List<EmailLog>>(json) ?? new List<EmailLog>();
            }
            catch { return new List<EmailLog>(); }
        }
    }

    public class EmailLog
    {
        public int ContactId { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public string SentBy { get; set; } = string.Empty;
        public string SentByRole { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty; // optional, helps dedupe
        public string Direction { get; set; } = "Outgoing";   // "Outgoing" or "Incoming"
        public string BodyHtml { get; set; } = string.Empty;
        public string BodyText { get; set; } = string.Empty;
        public string Headers { get; set; } = string.Empty;
    }
}
