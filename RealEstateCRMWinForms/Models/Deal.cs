using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstateCRMWinForms.Models
{
    public class Deal
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public int? PropertyId { get; set; }
        public Property? Property { get; set; }
        
        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }
        
        public decimal? Value { get; set; }
        
        [Required]
        public string Status { get; set; } = "New"; // New, Offer Made, Negotiation, Contract Draft, Closed Won, Closed Lost
        
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string? CreatedBy { get; set; }
    }
}