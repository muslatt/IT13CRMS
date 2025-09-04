using System;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.Services
{
    // Simple process-wide session store for the authenticated user.
    // Lightweight alternative to a full ViewModel for cross-control user state.
    public sealed class UserSession
    {
        private static readonly Lazy<UserSession> _instance = new(() => new UserSession());
        public static UserSession Instance => _instance.Value;

        private UserSession() { }

        private User? _currentUser;
        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                if (!ReferenceEquals(_currentUser, value))
                {
                    _currentUser = value;
                    CurrentUserChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler? CurrentUserChanged;
    }
}