using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateCRMWinForms.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? Details { get; set; }

        // Optional linkage to a specific property lifecycle
        public int? PropertyId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("PropertyId")]
        public Property? Property { get; set; }
    }
}
