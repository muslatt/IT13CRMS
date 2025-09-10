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
            loginView.RegisterRequested += OnRegisterRequested;
            SwitchView(loginView);
        }

        private void ShowRegisterView()
        {
            var registerView = new RegisterView(_authService);
            registerView.RegisterSuccess += OnRegisterSuccess;
            registerView.BackToLoginRequested += OnBackToLoginRequested;
            registerView.EmailVerificationRequired += OnEmailVerificationRequired;
            SwitchView(registerView);
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

            if (_currentUser != null)
            {
                var fullName = $"{_currentUser.FirstName} {_currentUser.LastName}".Trim();
                mainView.SetCurrentUser(fullName, ""); // pass role if you add it
            }

            SwitchView(mainView);
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
                mv.SetCurrentUser(fullName, "");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // unsubscribe to avoid leaks
            UserSession.Instance.CurrentUserChanged -= OnSessionUserChanged;
            base.OnFormClosed(e);
        }
    }
}