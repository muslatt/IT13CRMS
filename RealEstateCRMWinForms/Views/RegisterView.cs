using System;
using System.Windows.Forms;
using RealEstateCRMWinForms.Services;

namespace RealEstateCRMWinForms.Views
{
    public partial class RegisterView : UserControl
    {
        private readonly AuthenticationService _authService;
        public event EventHandler? RegisterSuccess;
        public event EventHandler? BackToLoginRequested;
        public event EventHandler<string>? EmailVerificationRequired;

        public RegisterView(AuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void btnRegister_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRegFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtRegLastName.Text) ||
                string.IsNullOrWhiteSpace(txtRegEmail.Text) ||
                string.IsNullOrWhiteSpace(txtRegPassword.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtRegPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Pre-check: specific error if email already in use
            var emailToUse = txtRegEmail.Text.Trim();
            if (_authService.IsEmailInUse(emailToUse))
            {
                MessageBox.Show("That email is already in use. Please use a different email.", "Email In Use",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnRegister.Enabled = false;
            btnRegister.Text = "Creating Account...";

            var success = await _authService.RegisterAsync(
                txtRegFirstName.Text.Trim(),
                txtRegLastName.Text.Trim(),
                txtRegEmail.Text.Trim(),
                txtRegPassword.Text);

            if (success)
            {
                var isBroker = Services.UserSession.Instance.CurrentUser?.Role == Models.UserRole.Broker;
                if (isBroker)
                {
                    MessageBox.Show("Agent account created successfully! A welcome email has been sent to the agent.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RegisterSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Account created successfully! Please check your email for a verification code.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EmailVerificationRequired?.Invoke(this, txtRegEmail.Text.Trim());
                }
            }
            else
            {
                MessageBox.Show("Failed to create account. Please try again.", "Registration Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnRegister.Enabled = true;
            btnRegister.Text = "Create Account";
        }

        private void lblBackToLogin_Click(object? sender, EventArgs e)
        {
            BackToLoginRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
