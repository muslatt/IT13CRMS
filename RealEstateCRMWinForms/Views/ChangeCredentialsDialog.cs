using System;
using System.Windows.Forms;
using RealEstateCRMWinForms.Services;

namespace RealEstateCRMWinForms.Views
{
    public partial class ChangeCredentialsDialog : Form
    {
        private readonly AuthenticationService _authService;

        public ChangeCredentialsDialog(AuthenticationService authService, string currentEmail)
        {
            InitializeComponent();
            _authService = authService;
            txtEmail.Text = currentEmail;
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            var newEmail = txtEmail.Text.Trim();
            var newPassword = txtPassword.Text;
            var confirm = txtConfirmPassword.Text;
            var currentPassword = txtCurrentPassword.Text;

            if (string.IsNullOrWhiteSpace(newEmail))
            {
                MessageBox.Show("Email cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(currentPassword))
                {
                    MessageBox.Show("Please enter your current password to change it.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!string.Equals(newPassword, confirm))
                {
                    MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var user = UserSession.Instance.CurrentUser;
            if (user == null)
            {
                MessageBox.Show("No active session.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnSave.Enabled = false;
            var ok = _authService.UpdateCredentialsRequiringCurrentPassword(
                user.Id,
                newEmail,
                string.IsNullOrEmpty(newPassword) ? null : newPassword,
                string.IsNullOrEmpty(newPassword) ? null : currentPassword);
            btnSave.Enabled = true;

            if (!ok)
            {
                MessageBox.Show("Failed to update credentials. Check your current password or email may be in use.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update session email for immediate UI reflection
            user.Email = newEmail;
            UserSession.Instance.CurrentUser = user; // triggers session changed
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
