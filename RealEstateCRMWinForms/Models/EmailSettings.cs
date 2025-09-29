namespace RealEstateCRMWinForms.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;

        // IMAP settings for receiving replies
        public string? ImapServer { get; set; }
        public int? ImapPort { get; set; }
        public bool? ImapUseSsl { get; set; }
        public string? ImapUsername { get; set; }
        public string? ImapPassword { get; set; }

        // Mailjet API sending settings
        public string? Provider { get; set; } // "Mailjet" to enable Mailjet API
        public string? MailjetApiKey { get; set; }
        public string? MailjetApiSecret { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public bool UseSandbox { get; set; } = false;
    }
}
