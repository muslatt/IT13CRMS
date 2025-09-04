// RealEstateCRMWinForms\Models\Lead.cs
using System;

namespace RealEstateCRMWinForms.Models
{
    public class Lead
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Status { get; set; } = "New Lead"; // e.g. New Lead, Contacted, Qualified, Unqualified
        public string Source { get; set; } = "Website";
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Type { get; set; } = "Renter"; // Only: Renter, Owner, Buyer
        public string Address { get; set; } = string.Empty;
        public string? AvatarPath { get; set; } // optional path to avatar image
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}