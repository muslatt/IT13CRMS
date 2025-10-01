using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateCRMWinForms.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal LotAreaSqm { get; set; } // Lot area in square meters
        public decimal FloorAreaSqft { get; set; } // Floor area in square feet
        public string? ImagePath { get; set; } // Path to property image
        // public string? ProofFilePath { get; set; } // Path to proof of ownership file - REMOVED for multiple files support
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Approval fields for client submissions
        public bool IsApproved { get; set; } = true; // Default true for existing properties
        public int? SubmittedByUserId { get; set; } // Client who submitted the property
        public string? RejectionReason { get; set; } // Reason for rejection if property was rejected
        public bool IsResubmitted { get; set; } = false; // Indicates if the property was resubmitted after rejection

        // Additional fields added from AddPropertyForm
        public string PropertyType { get; set; } = string.Empty; // "Residential", "Commercial", "Raw Land"
        public string TransactionType { get; set; } = string.Empty; // "Buying", "Viewing"
        public string Description { get; set; } = string.Empty; // Property description

        // Navigation property for multiple proof files
        public ICollection<PropertyProofFile> ProofFiles { get; set; } = new List<PropertyProofFile>();
    }
}
