// RealEstateCRMWinForms\Models\Lead.cs
using System;

namespace RealEstateCRMWinForms.Models
{
    public partial class Lead
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Type { get; set; } = "Renter"; // Only: Renter, Owner, Buyer
        public string? AvatarPath { get; set; } // optional path to avatar image
        // New fields for Leads list
        public string Occupation { get; set; } = string.Empty;
        public decimal? Salary { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}