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
        /// Calculates a lead score based on recency, type and other available factors.
        /// Adapted to only use properties present on the Lead model.
        /// </summary>
        /// <returns>Score from 0-100</returns>
        private int CalculateLeadScore()
        {
            int score = 0;

            // Boost score for recent leads
            if (DaysOld <= 7)
                score += 30;
            else if (DaysOld <= 30)
                score += 15;
            else if (DaysOld <= 90)
                score += 5;

            // Type-based scoring (Buyer/Owner/Renter)
            score += Type switch
            {
                "Buyer" => 40,
                "Owner" => 25,
                "Renter" => 10,
                _ => 5
            };

            // Bonus for having salary info (indicates qualification)
            if (Salary.HasValue && Salary > 0)
                score += 10;

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
        public bool NeedsAttention => Score >= 70;
        
        /// <summary>
        /// Gets the number of days since the lead was created
        /// </summary>
        [NotMapped]
        public int DaysOld => (int)(DateTime.UtcNow - CreatedAt).TotalDays;
    }
}