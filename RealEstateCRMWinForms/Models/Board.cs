using System.ComponentModel.DataAnnotations;

namespace RealEstateCRMWinForms.Models
{
    public class Board
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Color { get; set; } = "#CCCCCC"; // Hex color for the board
        
        public int Order { get; set; } // Display order of boards
        
        public bool IsDefault { get; set; } // Whether this is a default board
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public string CreatedBy { get; set; } = string.Empty; // User who created the board
    }
}