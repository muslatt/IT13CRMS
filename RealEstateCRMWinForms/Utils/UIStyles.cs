// RealEstateCRMWinForms\Utils\UIStyles.cs
using System.Drawing;

namespace RealEstateCRMWinForms.Utils
{
    /// <summary>
    /// Static class containing UI styling constants and color definitions
    /// </summary>
    public static class UIStyles
    {
        // Status Badge Colors
        public static readonly Color NewStatusColor = Color.FromArgb(220, 53, 69);      // Pink/Red
        public static readonly Color ContactedStatusColor = Color.FromArgb(255, 193, 7); // Yellow
        public static readonly Color QualifiedStatusColor = Color.FromArgb(23, 162, 184); // Blue/Teal
        public static readonly Color DefaultStatusColor = Color.FromArgb(108, 117, 125);  // Gray
        
        // Status Badge Text Colors
        public static readonly Color NewStatusTextColor = Color.White;
        public static readonly Color ContactedStatusTextColor = Color.FromArgb(33, 37, 41); // Dark text for yellow background
        public static readonly Color QualifiedStatusTextColor = Color.White;
        public static readonly Color DefaultStatusTextColor = Color.White;
        
        // Score Colors
        public static readonly Color HighScoreColor = Color.FromArgb(40, 167, 69);       // Green
        public static readonly Color MediumScoreColor = Color.FromArgb(255, 193, 7);     // Yellow
        public static readonly Color LowScoreColor = Color.FromArgb(220, 53, 69);        // Red
        
        // General Styling
        public static readonly Color RowHoverColor = Color.FromArgb(248, 249, 250);
        public static readonly Color SelectedRowColor = Color.FromArgb(232, 244, 255);
        public static readonly Color BorderColor = Color.FromArgb(224, 224, 224);
        
        // Avatar and Badge Settings
        public static readonly int AvatarSize = 32;
        public static readonly int BadgeCornerRadius = 12;
        public static readonly int BadgePadding = 8;
        
        // Typography
        public static readonly Font DefaultFont = new Font("Segoe UI", 12F, FontStyle.Regular);
        public static readonly Font BoldFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        public static readonly Font BadgeFont = new Font("Segoe UI", 10F, FontStyle.Regular);

        // Avatar Colors for initials (when no image available)
        public static readonly Color[] AvatarBackgroundColors = new Color[]
        {
            Color.FromArgb(255, 87, 34),   // Deep Orange
            Color.FromArgb(156, 39, 176),  // Purple
            Color.FromArgb(63, 81, 181),   // Indigo
            Color.FromArgb(33, 150, 243),  // Blue
            Color.FromArgb(0, 150, 136),   // Teal
            Color.FromArgb(76, 175, 80),   // Green
            Color.FromArgb(255, 152, 0),   // Orange
            Color.FromArgb(121, 85, 72),   // Brown
            Color.FromArgb(96, 125, 139),  // Blue Grey
            Color.FromArgb(233, 30, 99)    // Pink
        };
        
        public static readonly Color AvatarTextColor = Color.White;
        
        /// <summary>
        /// Gets the appropriate status badge colors for a given status
        /// </summary>
        /// <param name="status">The status string</param>
        /// <returns>Tuple of background color and text color</returns>
        public static (Color backgroundColor, Color textColor) GetStatusColors(string status)
        {
            return status?.ToLower() switch
            {
                "new" or "new lead" => (NewStatusColor, NewStatusTextColor),
                "contacted" => (ContactedStatusColor, ContactedStatusTextColor),
                "qualified" => (QualifiedStatusColor, QualifiedStatusTextColor),
                _ => (DefaultStatusColor, DefaultStatusTextColor)
            };
        }
        
        /// <summary>
        /// Gets the appropriate color for a score value
        /// </summary>
        /// <param name="score">The score value</param>
        /// <returns>Color for the score</returns>
        public static Color GetScoreColor(int score)
        {
            return score switch
            {
                >= 80 => HighScoreColor,
                >= 60 => MediumScoreColor,
                _ => LowScoreColor
            };
        }
        
        /// <summary>
        /// Gets a consistent avatar background color based on a name
        /// </summary>
        /// <param name="name">The name to generate color for</param>
        /// <returns>Background color for the avatar</returns>
        public static Color GetAvatarBackgroundColor(string name)
        {
            if (string.IsNullOrEmpty(name))
                return AvatarBackgroundColors[0];
                
            var hash = name.GetHashCode();
            var index = Math.Abs(hash) % AvatarBackgroundColors.Length;
            return AvatarBackgroundColors[index];
        }
    }
}