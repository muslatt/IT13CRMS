using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using RealEstateCRMWinForms.Services;

namespace RealEstateCRMWinForms.Views
{
    public class ContactDetailsDialog : Form
    {
        private readonly ContactViewModel _vm;
        private readonly Contact _contact;
        private readonly EmailNotificationService _emailService = new EmailNotificationService();
        private readonly EmailLogService _emailLog = new EmailLogService();

        private TextBox txtName, txtAgent, txtEmail, txtPhone, txtOccupation;
        private ComboBox cmbType;
        private NumericUpDown numSalary;
        private DateTimePicker dtpDateAdded;
        private Label lblLastContacted;
        private ListBox lstHistory;
        private TextBox txtSubject;
        private RichTextBox rtbBody;
        private Button btnSave, btnSend, btnArchive;
        private System.Collections.Generic.List<RealEstateCRMWinForms.Services.EmailLog> _history = new();

        public ContactDetailsDialog(Contact contact)
        {
            _vm = new ContactViewModel();
            _contact = contact;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Contact Details";
            Font = new Font("Segoe UI", 10F);
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 600);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(10) };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            // Details panel
            var details = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 3, Padding = new Padding(8) };
            for (int i = 0; i < 4; i++) details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            details.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            details.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            details.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            txtName = new TextBox();
            cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new object[] { "Buyer", "Renter", "Owner", "Lead" });
            txtAgent = new TextBox();
            txtEmail = new TextBox();
            txtPhone = new TextBox();
            txtOccupation = new TextBox();
            numSalary = new NumericUpDown { Maximum = 1000000000, DecimalPlaces = 2, ThousandsSeparator = true };
            dtpDateAdded = new DateTimePicker { Format = DateTimePickerFormat.Short };

            Add(details, new Label { Text = "Name", AutoSize = true }, 0, 0); Add(details, txtName, 0, 1);
            Add(details, new Label { Text = "Type", AutoSize = true }, 1, 0); Add(details, cmbType, 1, 1);
            Add(details, new Label { Text = "Agent", AutoSize = true }, 2, 0); Add(details, txtAgent, 2, 1);
            Add(details, new Label { Text = "Email", AutoSize = true }, 3, 0); Add(details, txtEmail, 3, 1);
            Add(details, new Label { Text = "Phone", AutoSize = true }, 0, 2); Add(details, txtPhone, 0, 3);
            Add(details, new Label { Text = "Occupation", AutoSize = true }, 1, 2); Add(details, txtOccupation, 1, 3);
            Add(details, new Label { Text = "Salary", AutoSize = true }, 2, 2); Add(details, numSalary, 2, 3);
            Add(details, new Label { Text = "Date Added", AutoSize = true }, 3, 2); Add(details, dtpDateAdded, 3, 3);

            btnSave = new Button { Text = "Save", BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 100, Height = 28 };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => SaveContact();
            btnArchive = new Button { Text = "Archive", BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 100, Height = 28 };
            btnArchive.FlatAppearance.BorderSize = 0;
            btnArchive.Click += (s, e) => ArchiveContact();
            var flowTop = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 36 };
            flowTop.Controls.Add(btnSave);
            flowTop.Controls.Add(btnArchive);
            var detailsHost = new Panel { Dock = DockStyle.Fill };
            detailsHost.Controls.Add(details);
            detailsHost.Controls.Add(flowTop);
            root.Controls.Add(detailsHost, 0, 0);

            // Bottom split: Last Contacted (left) and Email Field (right)
            var split = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.Controls.Add(split, 0, 1);

            // Last Contacted panel
            var lastPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };
            var lblHeaderLast = new Label { Text = "Contact History", Font = new Font("Segoe UI", 12F, FontStyle.Bold), AutoSize = true, Dock = DockStyle.Top };
            lblLastContacted = new Label { Text = "Never", AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(0, 6, 0, 6) };
            lstHistory = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false };
            lstHistory.SelectedIndexChanged += (s, e) => UpdateDetailsFromSelection();
            lastPanel.Controls.Add(lstHistory);
            lastPanel.Controls.Add(lblLastContacted);
            lastPanel.Controls.Add(lblHeaderLast);
            split.Controls.Add(lastPanel, 0, 0);

            // Email compose panel
            var emailPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };
            var lblHeaderEmail = new Label { Text = "Email", Font = new Font("Segoe UI", 12F, FontStyle.Bold), AutoSize = true, Dock = DockStyle.Top };
            var subjectRow = new Panel { Dock = DockStyle.Top, Height = 34 };
            var lblSubject = new Label { Text = "Subject", AutoSize = true, Location = new Point(0, 8) };
            txtSubject = new TextBox { Left = 70, Top = 4, Width = 480, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            subjectRow.Controls.Add(lblSubject);
            subjectRow.Controls.Add(txtSubject);

            // Body fills remaining space above the bottom button bar
            rtbBody = new RichTextBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };

            // Bottom send button bar (always visible)
            var btnBar = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 42, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 6, 0, 0) };
            btnSend = new Button { Text = "Send", BackColor = Color.FromArgb(34, 197, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 110, Height = 30 };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += async (s, e) => await SendEmailAsync();
            btnBar.Controls.Add(btnSend);

            emailPanel.Controls.Add(rtbBody);
            emailPanel.Controls.Add(btnBar);
            emailPanel.Controls.Add(subjectRow);
            emailPanel.Controls.Add(lblHeaderEmail);
            split.Controls.Add(emailPanel, 1, 0);
        }

        private void Add(TableLayoutPanel tl, Control c, int col, int row)
        {
            while (tl.RowStyles.Count <= row) tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tl.Controls.Add(c, col, row);
            c.Dock = DockStyle.Fill;
            if (c is TextBox tb) tb.Margin = new Padding(5, 2, 5, 2);
        }

        private void LoadData()
        {
            txtName.Text = _contact.FullName;
            cmbType.SelectedItem = string.IsNullOrWhiteSpace(_contact.Type) ? "Buyer" : _contact.Type;
            txtAgent.Text = (_contact as Contact).AssignedAgent;
            txtEmail.Text = _contact.Email;
            txtPhone.Text = _contact.Phone;
            txtOccupation.Text = _contact.Occupation;
            numSalary.Value = _contact.Salary ?? 0;
            dtpDateAdded.Value = _contact.CreatedAt;
            _ = RefreshHistoryAsync();
        }

        private void SaveContact()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Name is required."); return; }
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) { MessageBox.Show("Email is required."); return; }

            _contact.FullName = txtName.Text.Trim();
            _contact.Type = cmbType.SelectedItem?.ToString() ?? _contact.Type;
            _contact.Email = txtEmail.Text.Trim();
            _contact.Phone = txtPhone.Text.Trim();
            _contact.Occupation = txtOccupation.Text.Trim();
            _contact.Salary = numSalary.Value;
            _contact.CreatedAt = dtpDateAdded.Value;

            if (_vm.UpdateContact(_contact))
            {
                MessageBox.Show("Contact updated.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
        }

        private void UpdateDetailsFromSelection()
        {
            try
            {
                if (_history == null || _history.Count == 0)
                {
                    lblLastContacted.Text = "Never";
                    return;
                }
                var idx = lstHistory.SelectedIndex;
                if (idx < 0 || idx >= _history.Count) idx = 0;
                var log = _history[idx];
                lblLastContacted.Text = $"{log.SentAt:g}\nBy {log.SentBy} ({log.SentByRole})\nSubject: {log.Subject}";
            }
            catch { }
        }

        private async Task RefreshHistoryAsync()
        {
            try
            {
                var all = await _emailLog.GetAllAsync(_contact.Id);
                _history = all;
                lstHistory.Items.Clear();
                foreach (var item in all)
                {
                    lstHistory.Items.Add($"{item.SentAt:g} — {item.SentBy} ({item.SentByRole}) — {item.Subject}");
                }
                if (lstHistory.Items.Count == 0)
                {
                    lstHistory.Items.Add("No emails sent yet.");
                    lblLastContacted.Text = "Never";
                }
                else
                {
                    lstHistory.SelectedIndex = 0;
                    UpdateDetailsFromSelection();
                }
            }
            catch { }
        }

        private async Task SendEmailAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text)) { MessageBox.Show("Contact email is empty."); return; }
                var subject = string.IsNullOrWhiteSpace(txtSubject.Text) ? "Message from RealEstateCRM" : txtSubject.Text.Trim();

                // Always send HTML. Convert plain text to simple HTML.
                string bodyHtml;
                var text = rtbBody.Text ?? string.Empty;
                if (string.IsNullOrWhiteSpace(text))
                {
                    bodyHtml = "";
                }
                else
                {
                    var encoded = System.Net.WebUtility.HtmlEncode(text);
                    bodyHtml = encoded.Replace("\r\n", "<br>").Replace("\n", "<br>").Replace("\r", "<br>");
                }
                await _emailService.SendCustomEmailAsync(txtEmail.Text.Trim(), subject, bodyHtml);

                var user = UserSession.Instance.CurrentUser;
                var senderName = user != null ? user.FullName : Environment.UserName;
                var role = user != null ? user.Role : UserRole.Agent;
                await _emailLog.AppendAsync(
                    _contact.Id,
                    _contact.Email,
                    subject,
                    senderName,
                    role,
                    messageId: string.Empty,
                    direction: "Outgoing",
                    sentByRoleOverride: null,
                    sentAt: DateTime.Now,
                    bodyHtml: bodyHtml,
                    bodyText: text);
                await RefreshHistoryAsync();
                MessageBox.Show("Email sent.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send email: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ArchiveContact()
        {
            var result = MessageBox.Show("Archive this contact? It will be hidden from lists.", "Archive Contact", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                if (_vm.DeleteContact(_contact))
                {
                    MessageBox.Show("Contact archived.", "Archived", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to archive contact.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error archiving contact: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
