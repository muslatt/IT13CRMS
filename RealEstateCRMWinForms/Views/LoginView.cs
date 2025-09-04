using System;
using System.Windows.Forms;
using RealEstateCRMWinForms.Services;

namespace RealEstateCRMWinForms.Views
{
    public partial class LoginView : UserControl
    {
        private readonly AuthenticationService _authService;
        public event EventHandler? LoginSuccess;
        public event EventHandler? RegisterRequested;

        public LoginView(AuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both email and password.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Signing in...";

            var user = _authService.Authenticate(txtEmail.Text.Trim(), txtPassword.Text);

            if (user != null)
            {
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Invalid email or password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnLogin.Enabled = true;
            btnLogin.Text = "Sign In";
        }

        private void linkRegister_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterRequested?.Invoke(this, EventArgs.Empty);
        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}