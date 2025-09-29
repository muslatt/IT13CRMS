namespace RealEstateCRMWinForms.Models
{
    public class MailjetSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string? SenderName { get; set; }

        public MailjetInboundSettings Inbound { get; set; } = new();
    }

    public class MailjetInboundSettings
    {
        public bool Enabled { get; set; } = false;
        // Full HttpListener prefix, must end with '/'
        public string Prefix { get; set; } = "http://localhost:5020/";
        // Relative path under the prefix; e.g., "mailjet/inbound" (trailing slash optional)
        public string Path { get; set; } = "mailjet/inbound";
        // Optional shared token to validate requests: URL must include '?token=...'
        public string? AuthToken { get; set; }
    }
}
