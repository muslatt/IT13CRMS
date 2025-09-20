// RealEstateCRMWinForms\Controls\DataGridViewBadgeColumn.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using RealEstateCRMWinForms.Utils;

namespace RealEstateCRMWinForms.Controls
{
    /// <summary>
    /// Represents the style configuration for a status badge
    /// </summary>
    public class BadgeStyle
    {
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public Font? Font { get; set; }
        
        public BadgeStyle(Color backgroundColor, Color textColor, Font? font = null)
        {
            BackgroundColor = backgroundColor;
            TextColor = textColor;
            Font = font;
        }
    }
    
    /// <summary>
    /// Custom DataGridView column that displays status values as colored badges
    /// </summary>
    public class DataGridViewBadgeColumn : DataGridViewColumn
    {
        /// <summary>
        /// Dictionary mapping status values to their visual styles
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, BadgeStyle> StatusStyles { get; set; }
        
        /// <summary>
        /// Default style for unknown status values
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BadgeStyle DefaultStyle { get; set; }
        
        /// <summary>
        /// Corner radius for the badge rounded rectangle
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius { get; set; } = UIStyles.BadgeCornerRadius;
        
        /// <summary>
        /// Padding inside the badge
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BadgePadding { get; set; } = UIStyles.BadgePadding;
        
        public DataGridViewBadgeColumn() : base(new DataGridViewBadgeCell())
        {
            StatusStyles = new Dictionary<string, BadgeStyle>(StringComparer.OrdinalIgnoreCase);
            
            // Initialize with default status styles
            InitializeDefaultStyles();
            
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.DefaultCellStyle.Padding = new Padding(4);
        }
        
        /// <summary>
        /// Initializes the default status styles based on UIStyles
        /// </summary>
        private void InitializeDefaultStyles()
        {
            AddStatusStyle("New", UIStyles.NewStatusColor, UIStyles.NewStatusTextColor);
            AddStatusStyle("New Lead", UIStyles.NewStatusColor, UIStyles.NewStatusTextColor);
            AddStatusStyle("Contacted", UIStyles.ContactedStatusColor, UIStyles.ContactedStatusTextColor);
            AddStatusStyle("Qualified", UIStyles.QualifiedStatusColor, UIStyles.QualifiedStatusTextColor);
            
            DefaultStyle = new BadgeStyle(UIStyles.DefaultStatusColor, UIStyles.DefaultStatusTextColor, UIStyles.BadgeFont);
        }
        
        /// <summary>
        /// Adds or updates a status style
        /// </summary>
        /// <param name="status">The status value</param>
        /// <param name="backgroundColor">Background color for the badge</param>
        /// <param name="textColor">Text color for the badge</param>
        /// <param name="font">Optional custom font</param>
        public void AddStatusStyle(string status, Color backgroundColor, Color textColor, Font? font = null)
        {
            StatusStyles[status] = new BadgeStyle(backgroundColor, textColor, font ?? UIStyles.BadgeFont);
        }
        
        /// <summary>
        /// Gets the style for a given status value
        /// </summary>
        /// <param name="status">The status value</param>
        /// <returns>BadgeStyle for the status</returns>
        public BadgeStyle GetStyleForStatus(string? status)
        {
            if (string.IsNullOrEmpty(status))
                return DefaultStyle;
                
            return StatusStyles.TryGetValue(status, out var style) ? style : DefaultStyle;
        }
        
        public override DataGridViewCell? CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewBadgeCell)))
                {
                    throw new InvalidCastException("Must be a DataGridViewBadgeCell");
                }
                base.CellTemplate = value;
            }
        }
    }
    
    /// <summary>
    /// Custom DataGridView cell that renders status values as colored badges
    /// </summary>
    public class DataGridViewBadgeCell : DataGridViewCell
    {
        public DataGridViewBadgeCell()
        {
            this.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        
        public override Type? EditType => null; // Read-only cell
        
        public override Type ValueType => typeof(string);
        
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, 
            int rowIndex, DataGridViewElementStates cellState, object? value, object? formattedValue, 
            string? errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, 
            DataGridViewPaintParts paintParts)
        {
            try
            {
                // Paint the cell background
                if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
                {
                    using (var backBrush = new SolidBrush(cellState.HasFlag(DataGridViewElementStates.Selected) 
                        ? cellStyle.SelectionBackColor : cellStyle.BackColor))
                    {
                        graphics.FillRectangle(backBrush, cellBounds);
                    }
                }
                
                // Paint the border
                if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
                {
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
                }
                
                // Get the column configuration
                var column = this.OwningColumn as DataGridViewBadgeColumn;
                if (column == null) return;
                
                // Get the status value
                var statusValue = value?.ToString() ?? "";
                if (string.IsNullOrEmpty(statusValue)) return;
                
                // Get the style for this status
                var badgeStyle = column.GetStyleForStatus(statusValue);
                
                // Calculate badge bounds
                var contentBounds = new Rectangle(
                    cellBounds.X + cellStyle.Padding.Left,
                    cellBounds.Y + cellStyle.Padding.Top,
                    cellBounds.Width - cellStyle.Padding.Horizontal,
                    cellBounds.Height - cellStyle.Padding.Vertical
                );
                
                // Measure text to determine badge size
                var font = badgeStyle.Font ?? cellStyle.Font;
                var textSize = graphics.MeasureString(statusValue, font);
                
                var badgeWidth = (int)textSize.Width + (column.BadgePadding * 2);
                var badgeHeight = (int)textSize.Height + (column.BadgePadding / 2);
                
                // Center the badge in the cell
                var badgeBounds = new Rectangle(
                    contentBounds.X + (contentBounds.Width - badgeWidth) / 2,
                    contentBounds.Y + (contentBounds.Height - badgeHeight) / 2,
                    badgeWidth,
                    badgeHeight
                );
                
                // Draw the badge
                DrawBadge(graphics, badgeBounds, statusValue, badgeStyle, column.CornerRadius);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error painting DataGridViewBadgeCell: {ex.Message}");
                
                // Fallback to default text rendering
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, 
                    errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }
        
        /// <summary>
        /// Draws a rounded rectangle badge with text
        /// </summary>
        private void DrawBadge(Graphics graphics, Rectangle bounds, string text, BadgeStyle style, int cornerRadius)
        {
            try
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create rounded rectangle path
                using (var path = CreateRoundedRectanglePath(bounds, cornerRadius))
                {
                    // Fill the badge background
                    using (var brush = new SolidBrush(style.BackgroundColor))
                    {
                        graphics.FillPath(brush, path);
                    }
                    
                    // Draw the badge border (optional subtle border)
                    using (var pen = new Pen(Color.FromArgb(30, Color.Black), 1))
                    {
                        graphics.DrawPath(pen, path);
                    }
                }
                
                // Draw the text
                var font = style.Font ?? UIStyles.BadgeFont;
                using (var textBrush = new SolidBrush(style.TextColor))
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap
                    };
                    
                    graphics.DrawString(text, font, textBrush, bounds, stringFormat);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error drawing badge: {ex.Message}");
                
                // Fallback to simple rectangle
                using (var brush = new SolidBrush(style.BackgroundColor))
                using (var textBrush = new SolidBrush(style.TextColor))
                {
                    graphics.FillRectangle(brush, bounds);
                    graphics.DrawString(text, UIStyles.BadgeFont, textBrush, bounds);
                }
            }
        }
        
        /// <summary>
        /// Creates a GraphicsPath for a rounded rectangle
        /// </summary>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int cornerRadius)
        {
            var path = new GraphicsPath();
            
            if (cornerRadius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            
            // Ensure corner radius doesn't exceed half the smaller dimension
            var maxRadius = Math.Min(bounds.Width, bounds.Height) / 2;
            cornerRadius = Math.Min(cornerRadius, maxRadius);
            
            var diameter = cornerRadius * 2;
            
            // Top-left corner
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            
            // Top-right corner
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            
            // Bottom-right corner
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            
            // Bottom-left corner
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            
            path.CloseFigure();
            return path;
        }
        
        public override object Clone()
        {
            return (DataGridViewBadgeCell)base.Clone();
        }
    }
}