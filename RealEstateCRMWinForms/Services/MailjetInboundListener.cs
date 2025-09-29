using Microsoft.Extensions.Configuration;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RealEstateCRMWinForms.Services
{
    public class MailjetInboundListener : IDisposable
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task? _loopTask;
        private readonly EmailLogService _logService;
        private readonly ContactViewModel _contacts;
        private readonly MailjetInboundSettings _inbound;

        public MailjetInboundListener(EmailLogService logService, ContactViewModel contacts)
        {
            _logService = logService;
            _contacts = contacts;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var settings = configuration.GetSection("Mailjet").Get<MailjetSettings>() ?? new MailjetSettings();
            _inbound = settings.Inbound ?? new MailjetInboundSettings();
        }

        public bool CanStart => _inbound.Enabled;

        public void Start()
        {
            if (!CanStart) return;
            var prefix = _inbound.Prefix.EndsWith("/") ? _inbound.Prefix : _inbound.Prefix + "/";
            _listener.Prefixes.Add(prefix);
            _listener.Start();
            _loopTask = Task.Run(() => RunAsync(_cts.Token));
        }

        private async Task RunAsync(CancellationToken ct)
        {
            var pathNorm = NormalizePath(_inbound.Path);
            while (!ct.IsCancellationRequested)
            {
                HttpListenerContext? ctx = null;
                try
                {
                    ctx = await _listener.GetContextAsync();
                }
                catch (HttpListenerException)
                {
                    if (ct.IsCancellationRequested) break;
                }
                catch (ObjectDisposedException) { break; }
                if (ctx == null) continue;

                _ = Task.Run(() => HandleAsync(ctx, pathNorm), ct);
            }
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "/";
            var p = path.Trim('/');
            return "/" + p + "/";
        }

        private async Task HandleAsync(HttpListenerContext ctx, string pathNorm)
        {
            try
            {
                var req = ctx.Request;
                var res = ctx.Response;
                res.ContentType = "application/json";

                // route match
                if (!req.Url!.AbsolutePath.EndsWith(pathNorm, StringComparison.OrdinalIgnoreCase))
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    await WriteAsync(res, "{\"status\":\"not_found\"}");
                    return;
                }

                // token check (optional)
                if (!string.IsNullOrWhiteSpace(_inbound.AuthToken))
                {
                    var token = req.QueryString["token"];
                    if (!string.Equals(token, _inbound.AuthToken, StringComparison.Ordinal))
                    {
                        res.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await WriteAsync(res, "{\"error\":\"invalid_token\"}");
                        return;
                    }
                }

                string body;
                using (var reader = new StreamReader(req.InputStream, req.ContentEncoding ?? Encoding.UTF8))
                {
                    body = await reader.ReadToEndAsync();
                }

                var fields = ParseBody(req.ContentType ?? string.Empty, body);

                // Common Mailjet Parse fields (case-insensitive):
                // Sender / From, Subject, "Html-part", "Text-part", "Message-ID", Date
                string from = GetField(fields, new[] { "Sender", "From", "sender", "from" });
                string subject = GetField(fields, new[] { "Subject", "subject" });
                string messageId = GetField(fields, new[] { "Message-ID", "MessageID", "MessageId" });
                string date = GetField(fields, new[] { "Date", "date" });
                string htmlPart = GetField(fields, new[] { "Html-part", "HtmlPart", "html-part", "HTMLPart" });
                string textPart = GetField(fields, new[] { "Text-part", "TextPart", "text-part", "TextPart" });
                string headers = GetField(fields, new[] { "Headers", "headers" });

                if (string.IsNullOrWhiteSpace(from))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await WriteAsync(res, "{\"error\":\"missing_from\"}");
                    return;
                }

                var fromEmail = ExtractEmail(from);
                var contact = _contacts.Contacts.FirstOrDefault(c => string.Equals((c.Email ?? string.Empty).Trim(), fromEmail, StringComparison.OrdinalIgnoreCase));
                if (contact == null)
                {
                    // Unknown sender: accept 200 to avoid retries, but ignore
                    res.StatusCode = (int)HttpStatusCode.OK;
                    await WriteAsync(res, "{\"status\":\"ignored_unknown_sender\"}");
                    return;
                }

                DateTime? sentAt = null;
                if (DateTimeOffset.TryParse(date, out var dto)) sentAt = dto.ToLocalTime().DateTime;

                await _logService.AppendAsync(
                    contact.Id,
                    contact.Email,
                    subject ?? string.Empty,
                    contact.FullName ?? fromEmail,
                    UserRole.Agent,
                    messageId: messageId ?? string.Empty,
                    direction: "Incoming",
                    sentByRoleOverride: "Contact",
                    sentAt: sentAt,
                    bodyHtml: string.IsNullOrWhiteSpace(htmlPart) ? null : htmlPart,
                    bodyText: string.IsNullOrWhiteSpace(textPart) ? null : textPart,
                    headers: string.IsNullOrWhiteSpace(headers) ? null : headers);

                res.StatusCode = (int)HttpStatusCode.OK;
                await WriteAsync(res, "{\"status\":\"ok\"}");
            }
            catch (Exception ex)
            {
                try
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await WriteAsync(ctx.Response, JsonSerializer.Serialize(new { error = ex.Message }));
                }
                catch { }
            }
            finally
            {
                try { ctx.Response.OutputStream.Close(); } catch { }
            }
        }

        private static async Task WriteAsync(HttpListenerResponse res, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            await res.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        }

        private static string ExtractEmail(string input)
        {
            // Formats: "Name <email@domain>" or just "email@domain"
            var lt = input.IndexOf('<');
            var gt = input.IndexOf('>');
            if (lt >= 0 && gt > lt)
            {
                return input.Substring(lt + 1, gt - lt - 1).Trim().ToLowerInvariant();
            }
            return input.Trim().ToLowerInvariant();
        }

        private static Dictionary<string, string> ParseBody(string contentType, string body)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    using var doc = JsonDocument.Parse(body);
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        map[prop.Name] = prop.Value.ToString();
                    }
                    return map;
                }
            }
            catch { }

            if (contentType.Contains("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var pair in body.Split('&', StringSplitOptions.RemoveEmptyEntries))
                {
                    var kv = pair.Split('=');
                    if (kv.Length >= 1)
                    {
                        var key = WebUtility.UrlDecode(kv[0]);
                        var val = kv.Length > 1 ? WebUtility.UrlDecode(kv[1]) : string.Empty;
                        map[key] = val;
                    }
                }
                return map;
            }

            // Fallback: attempt to parse simple multipart by line (very naive)
            // For robustness, prefer configuring Mailjet to send JSON to this endpoint via a relay.
            foreach (var line in body.Split('\n'))
            {
                var idx = line.IndexOf(':');
                if (idx > 0)
                {
                    var key = line.Substring(0, idx).Trim();
                    var val = line.Substring(idx + 1).Trim();
                    if (!string.IsNullOrEmpty(key)) map[key] = val;
                }
            }
            return map;
        }

        private static string GetField(Dictionary<string, string> map, IEnumerable<string> keys)
        {
            foreach (var k in keys)
            {
                if (map.TryGetValue(k, out var v)) return v;
            }
            return string.Empty;
        }

        public void Dispose()
        {
            try { _cts.Cancel(); } catch { }
            try { _listener.Close(); } catch { }
            try { _loopTask?.Wait(1000); } catch { }
        }
    }
}
