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
        /// Name of the assigned agent (placeholder - will be dynamic in future)
        /// </summary>
        [NotMapped]
        public string AssignedAgent => "Rheniel Personal";
        
        /// <summary>
        /// Avatar path for the assigned agent (placeholder - will be dynamic in future)
        /// </summary>
        [NotMapped]
        public string? AgentAvatarPath => null; // Will use initials for now
        
        /// <summary>
        /// Calculates a lead score based on status, recency, and other factors
        /// </summary>
        /// <returns>Score from 0-100</returns>
        private int CalculateLeadScore()
        {
            int baseScore = 0;
            
            // Score based on status
            baseScore = Status?.ToLower() switch
            {
                "qualified" => 85,
                "contacted" => 70,
                "new" or "new lead" => 60,
                "unqualified" => 30,
                _ => 45
            };
            
            // Adjust score based on how recently they were contacted
            if (LastContacted.HasValue)
            {
                var daysSinceContact = (DateTime.UtcNow - LastContacted.Value).TotalDays;
                
                if (daysSinceContact <= 1)
                    baseScore += 10; // Very recent contact
                else if (daysSinceContact <= 7)
                    baseScore += 5;  // Recent contact
                else if (daysSinceContact > 30)
                    baseScore -= 10; // Old contact, might be getting cold
            }
            else if (Status?.ToLower() != "new" && Status?.ToLower() != "new lead")
            {
                // Non-new lead that has never been contacted - reduce score
                baseScore -= 15;
            }
            
            // Adjust score based on lead age
            var leadAge = (DateTime.UtcNow - CreatedAt).TotalDays;
            if (leadAge <= 1)
                baseScore += 5;  // Fresh lead
            else if (leadAge > 14)
                baseScore -= 5;  // Older lead
            
            // Adjust score based on lead type (some types might be more valuable)
            baseScore += Type?.ToLower() switch
            {
                "buyer" => 10,   // Buyers typically higher value
                "owner" => 5,    // Owners might sell or buy
                "renter" => 0,   // Renters baseline
                _ => 0
            };
            
            // Ensure score stays within 0-100 range
            return Math.Max(0, Math.Min(100, baseScore));
        }
        
        /// <summary>
        /// Gets a user-friendly description of the lead score
        /// </summary>
        [NotMapped]
        public string ScoreDescription => Score switch
        {
            >= 80 => "Hot Lead",
            >= 60 => "Warm Lead",
            >= 40 => "Cool Lead",
            _ => "Cold Lead"
        };
        
        /// <summary>
        /// Indicates if this lead needs immediate attention
        /// </summary>
        [NotMapped]
        public bool NeedsAttention => 
            (Status?.ToLower() == "new" || Status?.ToLower() == "new lead") && 
            (DateTime.UtcNow - CreatedAt).TotalDays > 2;
        
        /// <summary>
        /// Gets the number of days since the lead was created
        /// </summary>
        [NotMapped]
        public int DaysOld => (int)(DateTime.UtcNow - CreatedAt).TotalDays;
        
        /// <summary>
        /// Gets the number of days since last contact (null if never contacted)
        /// </summary>
        [NotMapped]
        public int? DaysSinceLastContact => LastContacted.HasValue 
            ? (int)(DateTime.UtcNow - LastContacted.Value).TotalDays 
            : null;
    }
}