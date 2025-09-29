using System;
using System.Security.Cryptography;
using System.Text;

namespace RealEstateCRMWinForms.Services
{
    public static class CredentialProtector
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("RealEstateCRMWinForms_CredentialEntropy_v1");

        public static string Protect(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            var data = Encoding.UTF8.GetBytes(plainText);
            var protectedBytes = ProtectedData.Protect(data, Entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        public static string Unprotect(string protectedBase64)
        {
            if (string.IsNullOrWhiteSpace(protectedBase64)) return string.Empty;
            try
            {
                var bytes = Convert.FromBase64String(protectedBase64);
                var unprotected = ProtectedData.Unprotect(bytes, Entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(unprotected);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

