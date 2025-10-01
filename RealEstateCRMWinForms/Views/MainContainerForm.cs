using System;
using System.Windows.Forms;
using RealEstateCRMWinForms.Services;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.Views
{
    public partial class MainContainerForm : Form
    {
        private readonly AuthenticationService _authService;
        private UserControl? _currentView;
        private User? _currentUser;

        public MainContainerForm()
        {
            InitializeComponent();
            _authService = new AuthenticationService();

            // Keep header in sync whenever session user changes
            UserSession.Instance.CurrentUserChanged += OnSessionUserChanged;

            ShowLoginView();
        }

        private void ShowLoginView()
        {
            var loginView = new LoginView(_authService);
            loginView.LoginSuccess += OnLoginSuccess;
            loginView.EmailVerificationRequested += (s, email) => ShowEmailVerificationView(email);
            loginView.RegisterRequested += (s, e) => ShowRegisterViewForClients();
            SwitchView(loginView);
        }

        private void ShowRegisterViewForClients()
        {
            // When coming from public login, register users should become Clients
            var registerView = new RegisterView(_authService, Models.UserRole.Client);
            registerView.RegisterSuccess += (s, e) => ReturnToMainAfterRegister();
            registerView.BackToLoginRequested += (s, e) => ShowLoginView();
            registerView.EmailVerificationRequired += (s, email) => ShowEmailVerificationView(email);
            SwitchView(registerView);
        }

        private void ShowRegisterView()
        {
            var registerView = new RegisterView(_authService);
            // When opened from Broker menu, return to Main after completing
            registerView.RegisterSuccess += (s, e) => ReturnToMainAfterRegister();
            registerView.BackToLoginRequested += (s, e) => ShowLoginView();
            // For non-broker self-registration, go to verification screen
            registerView.EmailVerificationRequired += (s, email) => ShowEmailVerificationView(email);
            SwitchView(registerView);
        }

        private void ReturnToMainAfterRegister(bool showInfo = false)
        {
            if (showInfo)
            {
                MessageBox.Show("Agent account created. Verification email sent (if configured).", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            var user = UserSession.Instance.CurrentUser;
            ShowMainView(user);
        }

        private void ShowEmailVerificationView(string email)
        {
            var verificationView = new EmailVerificationView(_authService, email);
            verificationView.VerificationSuccess += OnVerificationSuccess;
            verificationView.BackToLoginRequested += OnBackToLoginRequested;
            SwitchView(verificationView);
        }

        private void ShowMainView(User? user)
        {
            _currentUser = user;
            var mainView = new MainView();
            mainView.LogoutRequested += OnLogoutRequested;
            mainView.RegisterAgentRequested += OnRegisterAgentRequested;

            SwitchView(mainView);

            if (_currentUser != null)
            {
                var fullName = $"{_currentUser.FirstName} {_currentUser.LastName}".Trim();
                var roleName = _currentUser.Role.ToString();
                mainView.SetCurrentUser(fullName, roleName);
            }
        }

        private void OnRegisterAgentRequested(object? sender, EventArgs e)
        {
            var current = UserSession.Instance.CurrentUser;
            if (current == null || current.Role != Models.UserRole.Broker)
            {
                MessageBox.Show("Only Brokers can register new agents.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ShowRegisterView();
        }

        private void SwitchView(UserControl newView)
        {
            if (_currentView != null)
            {
                Controls.Remove(_currentView);
                _currentView.Dispose();
            }

            _currentView = newView;
            _currentView.Dock = DockStyle.Fill;
            Controls.Add(_currentView);
        }

        // Change the OnLoginSuccess method signature to match EventHandler
        private void OnLoginSuccess(object? sender, EventArgs e)
        {
            // The LoginView should pass the logged-in user via a property or a custom event args.
            // For now, retrieve the user from UserSession or LoginView (if available).
            var user = UserSession.Instance.CurrentUser;
            if (user != null)
                ShowMainView(user);
        }

        private void OnRegisterRequested(object? sender, EventArgs e) => ShowRegisterView();

        private void OnRegisterSuccess(object? sender, EventArgs e)
        {
            MessageBox.Show("Registration successful. Please sign in.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowLoginView();
        }

        private void OnEmailVerificationRequired(object? sender, string email)
        {
            ShowEmailVerificationView(email);
        }

        private void OnVerificationSuccess(object? sender, EventArgs e)
        {
            MessageBox.Show("Email verified successfully! You can now sign in.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowLoginView();
        }

        private void OnBackToLoginRequested(object? sender, EventArgs e) => ShowLoginView();

        private void OnLogoutRequested(object? sender, EventArgs e)
        {
            // clear session and go back to login
            UserSession.Instance.CurrentUser = null;
            ShowLoginView();
        }

        // When session user changes, update header if MainView is active
        private void OnSessionUserChanged(object? sender, EventArgs e)
        {
            var user = UserSession.Instance.CurrentUser;
            if (_currentView is MainView mv)
            {
                var fullName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : string.Empty;
                var roleName = user != null ? user.Role.ToString() : string.Empty;
                mv.SetCurrentUser(fullName, roleName);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // unsubscribe to avoid leaks
            UserSession.Instance.CurrentUserChanged -= OnSessionUserChanged;
            base.OnFormClosed(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Only prompt on user‑initiated close (title bar X, Alt+F4, etc.)
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to exit?",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }
    }
}
