using System;
using System.Windows.Forms;
using RealEstateCRMWinForms.Services;

namespace RealEstateCRMWinForms.Views
{
    public partial class LoginView : UserControl
    {
        private readonly AuthenticationService _authService;
        public event EventHandler? LoginSuccess;
        public event EventHandler<string>? EmailVerificationRequested;
        public event EventHandler? RegisterRequested;

        public LoginView(AuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;

            // Enable optimized double buffering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            // Hide self-registration on login screen
            try
            {
                // By default, show the register link so public users can create Client accounts
                lblRegisterQuestion.Visible = true;
                linkRegister.Visible = true;
            }
            catch { }
        }

        // Prevent flicker by skipping default background erase
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.BackgroundImage != null)
            {
                e.Graphics.DrawImage(this.BackgroundImage, this.ClientRectangle);
            }
            else
            {
                base.OnPaintBackground(e);
            }
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

            var trimmedEmail = txtEmail.Text.Trim();
            var user = _authService.Authenticate(trimmedEmail, txtPassword.Text);

            if (user != null)
            {
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Re-query user to inspect lockout/attempt counters
                RealEstateCRMWinForms.Models.User? userRecord = null;
                try
                {
                    using var ctx = RealEstateCRMWinForms.Data.DbContextHelper.CreateDbContext();
                    userRecord = ctx.Users.FirstOrDefault(u => u.Email == trimmedEmail);
                }
                catch { }

                // If locked out
                if (userRecord != null && userRecord.LockoutEnd.HasValue && userRecord.LockoutEnd.Value > DateTime.UtcNow)
                {
                    var remainingLock = userRecord.LockoutEnd.Value - DateTime.UtcNow;
                    MessageBox.Show($"Account locked due to multiple failed attempts. Try again in {(int)remainingLock.TotalMinutes} minute(s).", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // Show remaining attempts if user exists and not locked
                    if (userRecord != null && userRecord.FailedLoginAttempts > 0 && userRecord.FailedLoginAttempts < 5)
                    {
                        int remaining = 5 - userRecord.FailedLoginAttempts;
                        MessageBox.Show($"Invalid email or password. {remaining} attempt(s) remaining before lockout.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                var unverifiedUser = _authService.CheckUnverifiedUser(trimmedEmail);
                if (unverifiedUser)
                {
                    var result = MessageBox.Show("Your email address has not been verified yet. Would you like to resend the verification code?",
                        "Email Not Verified", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        if (_authService.ResendVerificationCode(txtEmail.Text.Trim()))
                        {
                            MessageBox.Show("Verification code sent! Please check your email.", "Code Sent",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to send verification code. Please try again.", "Send Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        EmailVerificationRequested?.Invoke(this, txtEmail.Text.Trim());
                    }
                }
                else if (userRecord == null || userRecord.FailedLoginAttempts == 0)
                {
                    // Generic message when we cannot show remaining attempts
                    MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
