using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;

namespace RealEstateCRMWinForms.Services
{
    public class EmailInboundService
    {
        private readonly EmailSettings _settings;
        private readonly EmailLogService _logService;
        private readonly ContactViewModel _contacts;

        public EmailInboundService(EmailLogService logService, ContactViewModel contacts)
        {
            _logService = logService;
            _contacts = contacts;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _settings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
        }

        private (string host, int port, bool useSsl, string username, string password)? ResolveImap()
        {
            var host = _settings.ImapServer ?? InferImapHost(_settings.SenderEmail);
            var port = _settings.ImapPort ?? 993;
            var useSsl = _settings.ImapUseSsl ?? true;
            var user = _settings.ImapUsername ?? _settings.SenderEmail;
            var pass = _settings.ImapPassword ?? _settings.SenderPassword;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                return null;
            return (host, port, useSsl, user, pass);
        }

        private static string InferImapHost(string senderEmail)
        {
            if (string.IsNullOrWhiteSpace(senderEmail)) return string.Empty;
            var domain = senderEmail.Split('@').LastOrDefault()?.ToLowerInvariant() ?? string.Empty;
            return domain switch
            {
                "gmail.com" => "imap.gmail.com",
                "outlook.com" => "imap-mail.outlook.com",
                "hotmail.com" => "imap-mail.outlook.com",
                "live.com" => "imap-mail.outlook.com",
                "yahoo.com" => "imap.mail.yahoo.com",
                _ => $"imap.{domain}"
            };
        }

        private static string NormalizeSubject(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var subj = s.Trim();
            while (subj.StartsWith("Re:", StringComparison.OrdinalIgnoreCase) || subj.StartsWith("Fwd:", StringComparison.OrdinalIgnoreCase))
            {
                var idx = subj.IndexOf(':');
                if (idx < 0) break;
                subj = subj[(idx + 1)..].Trim();
            }
            return subj;
        }

        public async Task<int> SyncRepliesAsync(DateTime? since = null, CancellationToken ct = default)
        {
            var imap = ResolveImap();
            if (imap == null)
            {
                // IMAP not configured
                return 0;
            }

            int added = 0;
            using var client = new ImapClient();
            var (host, port, useSsl, user, pass) = imap.Value;
            try
            {
                await client.ConnectAsync(host, port, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable, ct);
                await client.AuthenticateAsync(user, pass, ct);
                await client.Inbox.OpenAsync(MailKit.FolderAccess.ReadOnly, ct);

                var query = since.HasValue ? SearchQuery.DeliveredAfter(since.Value) : SearchQuery.All;
                var uids = await client.Inbox.SearchAsync(query, ct);
                if (uids == null || uids.Count == 0) return 0;

                // Load all logs once for quick dedupe checks
                var allLogs = await _logService.GetAllAsync(-1);

                foreach (var uid in uids)
                {
                    ct.ThrowIfCancellationRequested();
                    var msg = await client.Inbox.GetMessageAsync(uid, ct);
                    if (msg == null) continue;

                    var fromMailbox = msg.From.Mailboxes.FirstOrDefault();
                    if (fromMailbox == null) continue;
                    var fromEmail = fromMailbox.Address?.Trim().ToLowerInvariant() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(fromEmail)) continue;

                    // Map to a known contact
                    var contact = _contacts.Contacts.FirstOrDefault(c => string.Equals(c.Email?.Trim(), fromEmail, StringComparison.OrdinalIgnoreCase));
                    if (contact == null) continue; // skip unknown senders

                    var subjNorm = NormalizeSubject(msg.Subject);

                    // Dedupe by MessageId
                    var messageId = msg.MessageId ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(messageId) && allLogs.Any(l => l.MessageId == messageId))
                        continue;

                    // Append as incoming
                    await _logService.AppendAsync(
                        contact.Id,
                        contact.Email,
                        msg.Subject ?? string.Empty,
                        contact.FullName ?? fromEmail,
                        UserRole.Agent, // not used; override role string below
                        messageId: messageId,
                        direction: "Incoming",
                        sentByRoleOverride: "Contact",
                        sentAt: msg.Date.ToLocalTime().DateTime);

                    added++;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"IMAP sync failed: {ex.Message}");
            }
            finally
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true, ct);
                }
            }

            return added;
        }
    }
}
