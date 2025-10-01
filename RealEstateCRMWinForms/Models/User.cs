﻿using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateCRMWinForms.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationSentAt { get; set; }
        [Column("Role")]
        public int RoleInt { get; set; } = 0;

        // Temporarily stores broker-set password (encrypted) until agent verifies email
        public string? PendingPasswordEncrypted { get; set; }
        /// <summary>
        /// Gets the full name of the user (FirstName + LastName)
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [NotMapped]
        public UserRole Role => Enum.IsDefined(typeof(UserRole), RoleInt) ? (UserRole)RoleInt : UserRole.Agent;
    }
}
