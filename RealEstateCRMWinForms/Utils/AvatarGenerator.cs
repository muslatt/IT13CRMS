// RealEstateCRMWinForms\Utils\AvatarGenerator.cs
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;

namespace RealEstateCRMWinForms.Utils
{
    /// <summary>
    /// Utility class for generating and managing avatar images
    /// </summary>
    public static class AvatarGenerator
    {
        /// <summary>
        /// Generates a circular avatar image with initials
        /// </summary>
        /// <param name="fullName">The full name to extract initials from</param>
        /// <param name="size">The size of the avatar (width and height)</param>
        /// <returns>Generated avatar image</returns>
        public static Image GenerateInitialsAvatar(string fullName, int size = 32)
        {
            var initials = GetInitials(fullName);
            var backgroundColor = UIStyles.GetAvatarBackgroundColor(fullName);
            
            var bitmap = new Bitmap(size, size);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Enable high quality rendering
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                
                // Draw circular background
                using (var brush = new SolidBrush(backgroundColor))
                {
                    graphics.FillEllipse(brush, 0, 0, size, size);
                }
                
                // Draw initials text
                var fontSize = size * 0.4f; // 40% of avatar size
                using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold))
                using (var textBrush = new SolidBrush(UIStyles.AvatarTextColor))
                {
                    var textSize = graphics.MeasureString(initials, font);
                    var x = (size - textSize.Width) / 2;
                    var y = (size - textSize.Height) / 2;
                    
                    graphics.DrawString(initials, font, textBrush, x, y);
                }
            }
            
            return bitmap;
        }
        
        /// <summary>
        /// Loads an avatar image from path or generates initials avatar as fallback
        /// </summary>
        /// <param name="avatarPath">Path to the avatar image file</param>
        /// <param name="fullName">Full name for fallback initials avatar</param>
        /// <param name="size">Size of the avatar</param>
        /// <returns>Avatar image</returns>
        public static Image LoadOrGenerateAvatar(string? avatarPath, string fullName, int size = 32)
        {
            // Try to load from file first
            if (!string.IsNullOrEmpty(avatarPath) && File.Exists(avatarPath))
            {
                try
                {
                    var originalImage = Image.FromFile(avatarPath);
                    return ResizeAndMakeCircular(originalImage, size);
                }
                catch (Exception)
                {
                    // If loading fails, fall back to initials avatar
                }
            }
            
            // Generate initials avatar as fallback
            return GenerateInitialsAvatar(fullName, size);
        }
        
        /// <summary>
        /// Extracts initials from a full name
        /// </summary>
        /// <param name="fullName">The full name</param>
        /// <returns>Initials (up to 2 characters)</returns>
        public static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "??";
                
            var parts = fullName.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
                return "??";
            
            if (parts.Length == 1)
            {
                // Single name - use first two characters
                var name = parts[0];
                return name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
            }
            
            // Multiple names - use first character of first and last name
            var firstInitial = parts[0][0];
            var lastInitial = parts[parts.Length - 1][0];
            
            return $"{firstInitial}{lastInitial}".ToUpper();
        }
        
        /// <summary>
        /// Resizes an image and makes it circular
        /// </summary>
        /// <param name="originalImage">The original image</param>
        /// <param name="size">Target size</param>
        /// <returns>Resized circular image</returns>
        private static Image ResizeAndMakeCircular(Image originalImage, int size)
        {
            var bitmap = new Bitmap(size, size);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create circular clipping path
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, size, size);
                    graphics.SetClip(path);
                    
                    // Draw the resized image within the circular clip
                    graphics.DrawImage(originalImage, 0, 0, size, size);
                }
            }
            
            return bitmap;
        }
        
        /// <summary>
        /// Creates a default "no agent" avatar
        /// </summary>
        /// <param name="size">Size of the avatar</param>
        /// <returns>Default avatar image</returns>
        public static Image GenerateDefaultAgentAvatar(int size = 32)
        {
            var bitmap = new Bitmap(size, size);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Draw gray circular background
                using (var brush = new SolidBrush(UIStyles.DefaultStatusColor))
                {
                    graphics.FillEllipse(brush, 0, 0, size, size);
                }
                
                // Draw user icon (simplified)
                var iconSize = size * 0.6f;
                var iconX = (size - iconSize) / 2;
                var iconY = (size - iconSize) / 2;
                
                using (var pen = new Pen(Color.White, 2))
                {
                    // Draw simple user icon
                    var headRadius = iconSize * 0.25f;
                    var headX = iconX + (iconSize - headRadius * 2) / 2;
                    var headY = iconY + iconSize * 0.1f;
                    
                    graphics.DrawEllipse(pen, headX, headY, headRadius * 2, headRadius * 2);
                    
                    // Draw body (arc)
                    var bodyY = headY + headRadius * 1.5f;
                    var bodyWidth = iconSize * 0.8f;
                    var bodyHeight = iconSize * 0.4f;
                    var bodyX = iconX + (iconSize - bodyWidth) / 2;
                    
                    graphics.DrawArc(pen, bodyX, bodyY, bodyWidth, bodyHeight, 0, 180);
                }
            }
            
            return bitmap;
        }
    }
}