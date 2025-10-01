using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstateCRMWinForms.Models
{
    public class DealStatusChangeRequest
    {
        public int Id { get; set; }

        [Required]
        public int DealId { get; set; }
        public Deal? Deal { get; set; }

        [Required]
        public int RequestedByUserId { get; set; }
        public User? RequestedBy { get; set; }

        [Required]
        public string PreviousStatus { get; set; } = string.Empty;

        [Required]
        public string RequestedStatus { get; set; } = string.Empty;

        public bool? IsApproved { get; set; } // null = pending, true = approved, false = rejected

        public string? ResponseNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }

        public int? RespondedByUserId { get; set; }
        public User? RespondedBy { get; set; }
    }
}
