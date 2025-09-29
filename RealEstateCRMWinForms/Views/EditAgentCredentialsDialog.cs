using System;
using System.Drawing;
using System.Windows.Forms;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;

namespace RealEstateCRMWinForms.Views
{
    public class EditAgentCredentialsDialog : Form
    {
        private readonly AuthenticationService _auth;
        private readonly User _agent;

        private TextBox txtEmail = null!;
        private TextBox txtCurrent = null!;
        private TextBox txtPassword = null!;
        private TextBox txtConfirm = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public bool Updated { get; private set; }

        public EditAgentCredentialsDialog(AuthenticationService auth, User agent)
        {
            _auth = auth;
            _agent = agent;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Edit Agent Credentials";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(420, 320);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10F);

            int labelX = 20, controlX = 140, width = 240, y = 20, spacing = 34;

            Controls.Add(new Label { Text = "Email", Location = new Point(labelX, y + 6), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(controlX, y), Width = width, Text = _agent.Email };
            Controls.Add(txtEmail); y += spacing;

            Controls.Add(new Label { Text = "Current Password", Location = new Point(labelX, y + 6), AutoSize = true });
            txtCurrent = new TextBox { Location = new Point(controlX, y), Width = width, UseSystemPasswordChar = true };
            Controls.Add(txtCurrent); y += spacing;

            Controls.Add(new Label { Text = "New Password", Location = new Point(labelX, y + 6), AutoSize = true });
            txtPassword = new TextBox { Location = new Point(controlX, y), Width = width, UseSystemPasswordChar = true, PlaceholderText = "leave blank to keep" };
            Controls.Add(txtPassword); y += spacing;

            Controls.Add(new Label { Text = "Confirm Password", Location = new Point(labelX, y + 6), AutoSize = true });
            txtConfirm = new TextBox { Location = new Point(controlX, y), Width = width, UseSystemPasswordChar = true };
            Controls.Add(txtConfirm); y += spacing + 10;

            btnSave = new Button { Text = "Save", Location = new Point(controlX + width - 180, y), Width = 80 };
            btnCancel = new Button { Text = "Cancel", Location = new Point(controlX + width - 90, y), Width = 80 };
            btnSave.Click += OnSave;
            btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private void OnSave(object? sender, EventArgs e)
        {
            var email = (txtEmail.Text ?? string.Empty).Trim();
            var current = txtCurrent.Text ?? string.Empty;
            var pw = txtPassword.Text ?? string.Empty;
            var confirm = txtConfirm.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Email cannot be empty.");
                return;
            }
            if (!string.IsNullOrEmpty(pw))
            {
                if (pw.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(current))
                {
                    MessageBox.Show("Please enter the current password to change it.");
                    return;
                }
                if (!string.Equals(pw, confirm))
                {
                    MessageBox.Show("Passwords do not match.");
                    return;
                }
            }

            btnSave.Enabled = false;
            try
            {
                var ok = _auth.UpdateCredentialsRequiringCurrentPassword(
                    _agent.Id,
                    email,
                    string.IsNullOrWhiteSpace(pw) ? null : pw,
                    string.IsNullOrWhiteSpace(pw) ? null : current);
                if (!ok)
                {
                    MessageBox.Show("Failed to update. Check the current password or email may already be in use.");
                    return;
                }
                Updated = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            finally
            {
                btnSave.Enabled = true;
            }
        }
    }
}
