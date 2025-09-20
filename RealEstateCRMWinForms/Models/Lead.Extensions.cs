// RealEstateCRMWinForms\Models\Lead.Extensions.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateCRMWinForms.Models
{
    /// <summary>
    /// Extensions to the Lead model for UI-specific computed properties
    /// </summary>
    public partial class Lead
    {
        /// <summary>
        /// Calculated score for the lead based on various factors
        /// </summary>
        [NotMapped]
        public int Score => CalculateLeadScore();
        
        /// <summary>
        /// Name of the assigned agent (for UI display)
        /// </summary>
        [NotMapped]
        public string AssignedAgent { get; set; } = string.Empty;
        
        /// <summary>
        /// Avatar path for the assigned agent (for UI display)
        /// </summary>
        [NotMapped]
        public string? AgentAvatarPath { get; set; }
        
        /// <summary>
        /// Calculates a lead score based on status, recency, and other factors
        /// </summary>
        /// <returns>Score from 0-100</returns>
        private int CalculateLeadScore()
        {
            int score = 0;
            
            // Base score based on status
            score += Status switch
            {
                "New Lead" => 30,
                "Contacted" => 50,
                "Qualified" => 80,
                "Unqualified" => 10,
                _ => 20
            };
            
            // Bonus points for recent activity
            if (DaysOld <= 7)
                score += 20;
            else if (DaysOld <= 30)
                score += 10;
            
            // Deduct points for old leads without contact
            if (DaysSinceLastContact.HasValue && DaysSinceLastContact > 30)
                score -= 20;
            
            // Type-based scoring
            score += Type switch
            {
                "Buyer" => 15,
                "Owner" => 10,
                "Renter" => 5,
                _ => 0
            };
            
            return Math.Max(0, Math.Min(100, score));
        }
        
        /// <summary>
        /// Gets a user-friendly description of the lead score
        /// </summary>
        [NotMapped]
        public string ScoreDescription => Score switch
        {
            >= 80 => "Hot Lead",
            >= 60 => "Warm Lead",
            >= 40 => "Moderate Lead",
            >= 20 => "Cold Lead",
            _ => "Very Cold Lead"
        };
        
        /// <summary>
        /// Indicates if this lead needs immediate attention
        /// </summary>
        [NotMapped]
        public bool NeedsAttention => Score >= 70 || (DaysSinceLastContact.HasValue && DaysSinceLastContact > 14);
        
        /// <summary>
        /// Gets the number of days since the lead was created
        /// </summary>
        [NotMapped]
        public int DaysOld => (int)(DateTime.UtcNow - CreatedAt).TotalDays;
        
        /// <summary>
        /// Gets the number of days since last contact (null if never contacted)
        /// </summary>
        [NotMapped]
        public int? DaysSinceLastContact => LastContacted.HasValue ? 
            (int)(DateTime.UtcNow - LastContacted.Value).TotalDays : null;
    }
}