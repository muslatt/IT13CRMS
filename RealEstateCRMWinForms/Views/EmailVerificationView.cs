using System;
using System.Windows.Forms;
using RealEstateCRMWinForms.Services;

namespace RealEstateCRMWinForms.Views
{
    public partial class EmailVerificationView : UserControl
    {
        private readonly AuthenticationService _authService;
        private readonly string _email;
        public event EventHandler? VerificationSuccess;
        public event EventHandler? BackToLoginRequested;

        public EmailVerificationView(AuthenticationService authService, string email)
        {
            InitializeComponent();
            _authService = authService;
            _email = email;
            lblEmailAddress.Text = $"We've sent a verification code to: {email}";
        }

        private void btnVerify_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVerificationCode.Text))
            {
                MessageBox.Show("Please enter the verification code.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnVerify.Enabled = false;
            btnVerify.Text = "Verifying...";

            var success = _authService.VerifyEmail(_email, txtVerificationCode.Text.Trim().ToUpper());

            if (success)
            {
                MessageBox.Show("Email verified successfully! You can now sign in.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                VerificationSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Invalid verification code. Please try again.", "Verification Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnVerify.Enabled = true;
            btnVerify.Text = "Verify Email";
        }

        private void btnResendCode_Click(object? sender, EventArgs e)
        {
            btnResendCode.Enabled = false;
            btnResendCode.Text = "Sending...";

            var success = _authService.ResendVerificationCode(_email);

            if (success)
            {
                MessageBox.Show("Verification code sent! Please check your email.", "Code Sent",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to send verification code. Please try again.", "Send Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnResendCode.Enabled = true;
            btnResendCode.Text = "Resend Code";
        }

        private void lblBackToLogin_Click(object? sender, EventArgs e)
        {
            BackToLoginRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}