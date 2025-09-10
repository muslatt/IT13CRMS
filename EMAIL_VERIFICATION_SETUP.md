# Email Verification Setup Guide

## Overview
Email verification has been successfully added to your Real Estate CRM WinForms application. Users must now verify their email address after registration before they can log in.

## Features Added

### 1. Database Changes
- Added email verification fields to the User model:
  - `IsEmailVerified` (bool): Tracks if email is verified
  - `EmailVerificationToken` (string): Stores the 6-digit verification code
  - `EmailVerificationSentAt` (DateTime?): Tracks when the code was sent

### 2. New Views
- **EmailVerificationView**: A dedicated form for users to enter their verification code
- Clean, user-friendly interface with resend functionality

### 3. Enhanced Authentication Flow
- Registration now generates and sends a verification code
- Login is blocked for unverified users with helpful messaging
- Users can resend verification codes if needed
- Verification codes expire after 24 hours

### 4. Email Configuration
Email settings are configured in `appsettings.json`:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password",
    "SenderName": "Real Estate CRM",
    "EnableSsl": true
  }
}
```

## Setup Instructions

### 1. Configure Email Settings
1. Open `RealEstateCRMWinForms/appsettings.json`
2. Replace the placeholder email settings with your actual SMTP configuration:
   - For Gmail: Use an App Password (not your regular password)
   - For other providers: Update the SMTP server and port accordingly

### 2. Gmail Setup (Recommended)
1. Enable 2-Factor Authentication on your Gmail account
2. Generate an App Password:
   - Go to Google Account settings
   - Security → 2-Step Verification → App passwords
   - Generate a password for "Mail"
3. Use this App Password in the `SenderPassword` field

### 3. Alternative Email Providers
- **Outlook/Hotmail**: `smtp-mail.outlook.com`, Port 587
- **Yahoo**: `smtp.mail.yahoo.com`, Port 587
- **Custom SMTP**: Update server and port as needed

## User Flow

### Registration Process
1. User fills out registration form
2. System creates account with `IsEmailVerified = false`
3. Verification code is generated and sent via email
4. User is redirected to email verification screen
5. User enters the 6-digit code
6. Account is activated and user can log in

### Login Process
1. User attempts to log in
2. If email is not verified:
   - Login is blocked
   - User is offered option to resend verification code
3. If email is verified: Normal login proceeds

## Development Mode
If email configuration is not set up (default placeholder values), the system will:
- Log verification codes to the debug console
- Continue normal operation without sending actual emails
- This allows development and testing without email setup

## Security Features
- Verification codes are 6 digits, uppercase alphanumeric
- Codes expire after 24 hours
- Users can request new codes (invalidates old ones)
- Email verification is required before any login attempt

## Testing
1. Register a new account
2. Check debug output for verification code (if email not configured)
3. Enter the code in the verification screen
4. Verify that login works after verification
5. Test that unverified accounts cannot log in

## Troubleshooting

### Email Not Sending
- Check SMTP settings in appsettings.json
- Verify App Password for Gmail
- Check firewall/antivirus blocking SMTP connections
- Look at debug console for error messages

### Verification Code Issues
- Codes expire after 24 hours
- Use "Resend Code" button for new codes
- Codes are case-insensitive but displayed in uppercase

### Database Issues
- Migration was automatically applied
- If issues occur, run: `dotnet ef database update`

## Files Modified/Added
- `Models/User.cs` - Added verification fields
- `Models/EmailSettings.cs` - New configuration model
- `Services/AuthenticationService.cs` - Enhanced with email verification
- `Views/EmailVerificationView.cs` - New verification form
- `Views/RegisterView.cs` - Updated registration flow
- `Views/LoginView.cs` - Enhanced login with verification checks
- `Views/MainContainerForm.cs` - Added verification view management
- `appsettings.json` - Added email configuration
- Database migration for new fields

The email verification system is now fully integrated and ready for use!