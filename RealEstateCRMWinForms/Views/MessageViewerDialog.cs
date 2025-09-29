using System;
using System.Drawing;
using System.Windows.Forms;
using RealEstateCRMWinForms.Services;

namespace RealEstateCRMWinForms.Views
{
    public class MessageViewerDialog : Form
    {
        private readonly EmailLog _log;
        private TabControl tabs;
        private WebBrowser wbHtml;
        private TextBox txtPlain;
        private TextBox txtHeaders;
        private Label lblMeta;

        public MessageViewerDialog(EmailLog log)
        {
            _log = log;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Message";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(860, 600);
            Font = new Font("Segoe UI", 10);

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            lblMeta = new Label { Dock = DockStyle.Fill, Padding = new Padding(12), AutoSize = false };
            root.Controls.Add(lblMeta, 0, 0);

            tabs = new TabControl { Dock = DockStyle.Fill };
            root.Controls.Add(tabs, 0, 1);

            var tpHtml = new TabPage("HTML");
            var tpText = new TabPage("Text");
            var tpHeaders = new TabPage("Headers");
            tabs.TabPages.Add(tpHtml);
            tabs.TabPages.Add(tpText);
            tabs.TabPages.Add(tpHeaders);

            wbHtml = new WebBrowser { Dock = DockStyle.Fill };
            tpHtml.Controls.Add(wbHtml);

            txtPlain = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Both, ReadOnly = true, Font = new Font("Consolas", 10) };
            tpText.Controls.Add(txtPlain);

            txtHeaders = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Both, ReadOnly = true, Font = new Font("Consolas", 9) };
            tpHeaders.Controls.Add(txtHeaders);
        }

        private void LoadData()
        {
            lblMeta.Text = $"From: {(_log.Direction.Equals("Incoming", StringComparison.OrdinalIgnoreCase) ? _log.ContactEmail : "Me")}\r\n" +
                           $"To: {(_log.Direction.Equals("Incoming", StringComparison.OrdinalIgnoreCase) ? "Me" : _log.ContactEmail)}\r\n" +
                           $"Subject: {_log.Subject}\r\n" +
                           $"Date: {_log.SentAt:g}    Direction: {_log.Direction}";

            if (!string.IsNullOrWhiteSpace(_log.BodyHtml))
            {
                wbHtml.DocumentText = _log.BodyHtml;
            }
            else
            {
                var text = _log.BodyText ?? string.Empty;
                var encoded = System.Net.WebUtility.HtmlEncode(text).Replace("\r\n", "<br>").Replace("\n", "<br>");
                wbHtml.DocumentText = $"<html><body style='font-family:Segoe UI, Arial; font-size: 12pt;'>{encoded}</body></html>";
            }

            txtPlain.Text = !string.IsNullOrWhiteSpace(_log.BodyText)
                ? _log.BodyText
                : StripHtmlToText(_log.BodyHtml);

            txtHeaders.Text = _log.Headers ?? string.Empty;
        }

        private static string StripHtmlToText(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            try
            {
                // quick and simple: remove tags
                var withoutTags = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", string.Empty);
                // decode entities
                return System.Net.WebUtility.HtmlDecode(withoutTags);
            }
            catch { return html; }
        }
    }
}

