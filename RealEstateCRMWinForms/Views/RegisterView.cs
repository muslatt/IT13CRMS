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

        public RegisterView(AuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private void btnRegister_Click(object? sender, EventArgs e)
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

            btnRegister.Enabled = false;
            btnRegister.Text = "Creating Account...";

            var success = _authService.Register(
                txtRegFirstName.Text.Trim(),
                txtRegLastName.Text.Trim(),
                txtRegEmail.Text.Trim(),
                txtRegPassword.Text);

            if (success)
            {
                MessageBox.Show("Account created successfully! You can now sign in.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RegisterSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Failed to create account. Email may already be in use.", "Registration Failed",
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