// RealEstateCRMWinForms\Models\Contact.Extensions.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateCRMWinForms.Models
{
    /// <summary>
    /// Extensions to the Contact model for UI-specific computed properties
    /// </summary>
    public partial class Contact
    {
        /// <summary>
        /// Name of the assigned agent (for UI display)
        /// </summary>
        [NotMapped]
        public string AssignedAgent { get; set; } = string.Empty;

        /// <summary>
        /// Avatar path for the assigned agent (placeholder - will be dynamic in future)
        /// </summary>
        [NotMapped]
        public string? AgentAvatarPath => null; // Will use initials for now

        /// <summary>
        /// Gets the number of days since the contact was created
        /// </summary>
        [NotMapped]
        public int DaysAsContact => (int)(DateTime.UtcNow - CreatedAt).TotalDays;

        /// <summary>
        /// Gets a user-friendly description of how long they've been a contact
        /// </summary>
        [NotMapped]
        public string ContactDuration
        {
            get
            {
                var days = DaysAsContact;
                return days switch
                {
                    0 => "Today",
                    1 => "1 day ago",
                    < 7 => $"{days} days ago",
                    < 30 => $"{days / 7} week{(days / 7 > 1 ? "s" : "")} ago",
                    < 365 => $"{days / 30} month{(days / 30 > 1 ? "s" : "")} ago",
                    _ => $"{days / 365} year{(days / 365 > 1 ? "s" : "")} ago"
                };
            }
        }

        /// <summary>
        /// Indicates if this is a new contact (less than 7 days old)
        /// </summary>
        [NotMapped]
        public bool IsNewContact => DaysAsContact < 7;

        /// <summary>
        /// Gets a priority level for the contact based on type and recency
        /// </summary>
        [NotMapped]
        public string Priority
        {
            get
            {
                if (IsNewContact)
                    return "High";

                return Type?.ToLower() switch
                {
                    "buyer" => "High",
                    "owner" => "Medium",
                    "renter" => "Normal",
                    _ => "Normal"
                };
            }
        }

        /// <summary>
        /// Gets the contact's initials for display purposes
        /// </summary>
        [NotMapped]
        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName))
                    return "??";

                var parts = FullName.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                    return "??";

                if (parts.Length == 1)
                {
                    var name = parts[0];
                    return name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
                }

                var firstInitial = parts[0][0];
                var lastInitial = parts[parts.Length - 1][0];

                return $"{firstInitial}{lastInitial}".ToUpper();
            }
        }
    }
}