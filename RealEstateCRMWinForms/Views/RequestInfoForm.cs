using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public class RequestInfoForm : Form
    {
        private Property _property;
        private TextBox txtMessage;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private Button btnSend;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblPropertyInfo;

        public RequestInfoForm(Property property)
        {
            _property = property;
            InitializeComponent();
            LoadPropertyInfo();
        }

        private void InitializeComponent()
        {
            Text = "Request Property Information";
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(248, 249, 250);
            ClientSize = new Size(500, 550);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Main container
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Title
            lblTitle = new Label
            {
                Text = "Request Information",
                Location = new Point(0, 0),
                Size = new Size(452, 35),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            // Property info label
            lblPropertyInfo = new Label
            {
                Location = new Point(0, 45),
                Size = new Size(452, 50),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = false
            };

            // Email label
            var lblEmail = new Label
            {
                Text = "Your Email (optional):",
                Location = new Point(0, 105),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81)
            };

            // Email textbox
            txtEmail = new TextBox
            {
                Location = new Point(0, 130),
                Size = new Size(452, 30),
                Font = new Font("Segoe UI", 11F),
                PlaceholderText = "your.email@example.com"
            };

            // Phone label
            var lblPhone = new Label
            {
                Text = "Your Phone (optional):",
                Location = new Point(0, 170),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81)
            };

            // Phone textbox
            txtPhone = new TextBox
            {
                Location = new Point(0, 195),
                Size = new Size(452, 30),
                Font = new Font("Segoe UI", 11F),
                PlaceholderText = "+63 9XX XXX XXXX"
            };

            // Message label
            var lblMessage = new Label
            {
                Text = "Your Message:",
                Location = new Point(0, 235),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81)
            };

            // Message textbox
            txtMessage = new TextBox
            {
                Location = new Point(0, 260),
                Size = new Size(452, 120),
                Font = new Font("Segoe UI", 11F),
                Multiline = true,
                PlaceholderText = "I'm interested in this property and would like more information about...",
                ScrollBars = ScrollBars.Vertical
            };

            // Button panel
            var buttonPanel = new Panel
            {
                Location = new Point(0, 390),
                Size = new Size(452, 50),
                BackColor = Color.Transparent
            };

            // Send button
            btnSend = new Button
            {
                Text = "Send Inquiry",
                Size = new Size(140, 40),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(202, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += BtnSend_Click;

            // Cancel button
            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(352, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnSend, btnCancel });

            mainContainer.Controls.AddRange(new Control[] {
                lblTitle, lblPropertyInfo, lblEmail, txtEmail,
                lblPhone, txtPhone, lblMessage, txtMessage, buttonPanel
            });

            Controls.Add(mainContainer);

            AcceptButton = btnSend;
            CancelButton = btnCancel;
        }

        private void LoadPropertyInfo()
        {
            lblPropertyInfo.Text = $"Property: {_property.Title}\n" +
                                   $"Location: {_property.Address}\n" +
                                   $"Price: {_property.Price:C0}";

            // Pre-fill email and phone from current user if available
            var currentUser = UserSession.Instance.CurrentUser;
            if (currentUser != null)
            {
                txtEmail.Text = currentUser.Email ?? string.Empty;
                // You can add phone field to User model if needed
            }
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            // Validate message
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                MessageBox.Show(
                    "Please enter a message for your inquiry.",
                    "Message Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtMessage.Focus();
                return;
            }

            try
            {
                var currentUser = UserSession.Instance.CurrentUser;
                if (currentUser == null)
                {
                    MessageBox.Show(
                        "You must be logged in to send an inquiry.",
                        "Authentication Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    DialogResult = DialogResult.Cancel;
                    Close();
                    return;
                }

                // Create inquiry
                using var db = Data.DbContextHelper.CreateDbContext();
                var inquiry = new Inquiry
                {
                    PropertyId = _property.Id,
                    ClientId = currentUser.Id,
                    Message = txtMessage.Text.Trim(),
                    ContactEmail = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                    ContactPhone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                    Status = InquiryStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                db.Inquiries.Add(inquiry);
                db.SaveChanges();

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error sending inquiry: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
