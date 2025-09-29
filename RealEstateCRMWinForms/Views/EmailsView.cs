using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RealEstateCRMWinForms.Services;
using RealEstateCRMWinForms.ViewModels;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.Views
{
    public class EmailsView : UserControl
    {
        private readonly EmailLogService _logService = new EmailLogService();
        private readonly ContactViewModel _contactVm = new ContactViewModel();

        private ListBox lstThreads = null!;
        private ListBox lstMessages = null!;
        private WebBrowser wbViewer = null!;
        private Label lblInfo = null!;
        private Button btnRefresh = null!;
        private EmailInboundService _inboundService;

        private class ThreadItem
        {
            public int ContactId { get; set; }
            public string Subject { get; set; } = string.Empty; // normalized
            public string Display { get; set; } = string.Empty;
            public override string ToString() => Display;
        }

        private class MessageItem
        {
            public EmailLog Log { get; set; }
            public string Display { get; set; }
            public MessageItem(EmailLog log, string display)
            {
                Log = log;
                Display = display;
            }
            public override string ToString() => Display;
        }

        public EmailsView()
        {
            _inboundService = new EmailInboundService(_logService, _contactVm);
            InitializeComponent();
            LoadThreads();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(249, 250, 251);

            var top = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.White, Padding = new Padding(12) };
            btnRefresh = new Button { Text = "Refresh", Width = 100, Height = 28, BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) =>
            {
                await SyncInboundAsync();
                LoadThreads();
            };
            top.Controls.Add(btnRefresh);
            Controls.Add(top);

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 320, BackColor = Color.FromArgb(249, 250, 251) };
            Controls.Add(split);
            split.BringToFront();

            lstThreads = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false };
            lstThreads.SelectedIndexChanged += (s, e) => LoadThreadMessages();
            split.Panel1.Controls.Add(lstThreads);

            var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            split.Panel2.Controls.Add(rightPanel);

            lblInfo = new Label { Dock = DockStyle.Top, Height = 44, Text = "Select a thread", Padding = new Padding(12), AutoSize = false };
            rightPanel.Controls.Add(lblInfo);

            var rightSplit = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 240 };
            rightPanel.Controls.Add(rightSplit);

            lstMessages = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false };
            lstMessages.SelectedIndexChanged += (s, e) => ShowSelectedMessage();
            lstMessages.DoubleClick += (s, e) => OpenMessageDialog();
            rightSplit.Panel1.Controls.Add(lstMessages);

            wbViewer = new WebBrowser { Dock = DockStyle.Fill };
            rightSplit.Panel2.Controls.Add(wbViewer);
        }

        private async void LoadThreads()
        {
            try
            {
                var user = UserSession.Instance.CurrentUser;
                var currentName = user?.FullName ?? string.Empty;
                // Pull latest replies before rebuilding threads
                await SyncInboundAsync();
                var logs = await _logService.GetAllAsync(-1); // load everything then filter

                // Include threads either sent by current user OR any incoming replies from contacts
                var relevant = logs.Where(l =>
                        string.Equals(l.SentBy, currentName, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(l.Direction, "Incoming", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                var grouped = relevant
                    .GroupBy(l => new { l.ContactId, Subject = NormalizeSubject(l.Subject) })
                    .OrderByDescending(g => g.Max(x => x.SentAt))
                    .ToList();

                lstThreads.Items.Clear();
                foreach (var g in grouped)
                {
                    var contact = _contactVm.Contacts.FirstOrDefault(c => c.Id == g.Key.ContactId);
                    var contactName = contact?.FullName ?? g.First().ContactEmail;
                    var item = new ThreadItem
                    {
                        ContactId = g.Key.ContactId,
                        Subject = g.Key.Subject,
                        Display = $"{contactName} - {g.Key.Subject}"
                    };
                    lstThreads.Items.Add(item);
                }

                if (lstThreads.Items.Count > 0) lstThreads.SelectedIndex = 0;
                else
                {
                    lstMessages.Items.Clear();
                    lblInfo.Text = "No email threads yet. Send an email from a contact to start a thread.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load emails: {ex.Message}");
            }
        }

        private string NormalizeSubject(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var subj = s.Trim();
            while (subj.StartsWith("Re:", StringComparison.OrdinalIgnoreCase) || subj.StartsWith("Fwd:", StringComparison.OrdinalIgnoreCase))
            {
                subj = subj.Substring(subj.IndexOf(':') + 1).Trim();
            }
            return subj;
        }

        private async void LoadThreadMessages()
        {
            try
            {
                var user = UserSession.Instance.CurrentUser;
                var currentName = user?.FullName ?? string.Empty;
                if (lstThreads.SelectedItem is not ThreadItem item) return;
                var contactId = item.ContactId;
                var normalizedSubject = item.Subject;

                // Fetch all logs for this contact/subject regardless of direction, but keep only
                // threads involving current user (outgoing) and contact (incoming)
                var all = await _logService.GetAllAsync(-1);
                var threadLogs = all
                    .Where(l => l.ContactId == contactId)
                    .Where(l => string.Equals(NormalizeSubject(l.Subject), normalizedSubject, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(l => l.SentAt)
                    .ToList();

                lstMessages.Items.Clear();
                foreach (var m in threadLogs)
                {
                    var isIncoming = string.Equals(m.Direction, "Incoming", StringComparison.OrdinalIgnoreCase);
                    var display = isIncoming
                        ? $"{m.SentAt:g} | {m.ContactEmail} -> Me: {m.Subject}"
                        : $"{m.SentAt:g} | Me -> {m.ContactEmail}: {m.Subject}";
                    lstMessages.Items.Add(new MessageItem(m, display));
                }

                lblInfo.Text = "Thread messages (synced).";
                if (lstMessages.Items.Count > 0) lstMessages.SelectedIndex = lstMessages.Items.Count - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load thread: {ex.Message}");
            }
        }

        private async Task SyncInboundAsync()
        {
            // Try to sync inbound replies for the past 30 days
            var since = DateTime.Now.AddDays(-30);
            try { await _inboundService.SyncRepliesAsync(since); }
            catch { /* ignore */ }
        }

        private void ShowSelectedMessage()
        {
            try
            {
                if (lstMessages.SelectedItem is not MessageItem item)
                {
                    wbViewer.DocumentText = "";
                    return;
                }
                var log = item.Log;
                var html = !string.IsNullOrWhiteSpace(log.BodyHtml) ? log.BodyHtml : null;
                if (string.IsNullOrWhiteSpace(html))
                {
                    var text = log.BodyText ?? string.Empty;
                    var encoded = System.Net.WebUtility.HtmlEncode(text).Replace("\r\n", "<br>").Replace("\n", "<br>");
                    html = $"<html><body style='font-family:Segoe UI, Arial; font-size: 12pt;'>{encoded}</body></html>";
                }
                wbViewer.DocumentText = html;
            }
            catch { }
        }

        private void OpenMessageDialog()
        {
            try
            {
                if (lstMessages.SelectedItem is not MessageItem item) return;
                using var dlg = new MessageViewerDialog(item.Log);
                dlg.ShowDialog(FindForm());
            }
            catch { }
        }
    }
}
