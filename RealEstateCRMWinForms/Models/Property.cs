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
        public int SquareMeters { get; set; }
        public string Status { get; set; } = string.Empty; // "Sell", "Rent", etc.
        public string? ImagePath { get; set; } // Path to property image
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        
        // Additional fields added from AddPropertyForm
        public string PropertyType { get; set; } = string.Empty; // "Residential", "Commercial", "Raw Land"
        public string TransactionType { get; set; } = string.Empty; // "Buying", "Viewing"
        public string Agent { get; set; } = string.Empty; // Agent name/info
        public string Description { get; set; } = string.Empty; // Property description
    }
}
