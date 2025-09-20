// RealEstateCRMWinForms\Controls\DataGridViewImageTextColumn.cs
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using RealEstateCRMWinForms.Utils;

namespace RealEstateCRMWinForms.Controls
{
    /// <summary>
    /// Custom DataGridView column that displays an image (avatar) alongside text
    /// </summary>
    public class DataGridViewImageTextColumn : DataGridViewColumn
    {
        /// <summary>
        /// Property name for the image/avatar path in the data source
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ImagePropertyName { get; set; } = "AvatarPath";
        
        /// <summary>
        /// Property name for the text to display in the data source
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TextPropertyName { get; set; } = "FullName";
        
        /// <summary>
        /// Whether to show initials when no image is available
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowInitialsWhenNoImage { get; set; } = true;
        
        /// <summary>
        /// Size of the avatar image
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int AvatarSize { get; set; } = UIStyles.AvatarSize;
        
        public DataGridViewImageTextColumn() : base(new DataGridViewImageTextCell())
        {
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
        }
        
        public override DataGridViewCell? CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewImageTextCell)))
                {
                    throw new InvalidCastException("Must be a DataGridViewImageTextCell");
                }
                base.CellTemplate = value;
            }
        }
    }
    
    /// <summary>
    /// Custom DataGridView cell that renders an image (avatar) alongside text
    /// </summary>
    public class DataGridViewImageTextCell : DataGridViewCell
    {
        private Image? _cachedAvatar;
        private string? _cachedAvatarKey;
        
        public DataGridViewImageTextCell()
        {
            this.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        
        public override Type? EditType => null; // Read-only cell
        
        public override Type ValueType => typeof(object);
        
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
                var column = this.OwningColumn as DataGridViewImageTextColumn;
                if (column == null) return;
                
                // Get data from the row
                var dataItem = this.DataGridView?.Rows[rowIndex].DataBoundItem;
                if (dataItem == null) return;
                
                var avatarPath = GetPropertyValue(dataItem, column.ImagePropertyName) as string;
                var displayText = GetPropertyValue(dataItem, column.TextPropertyName)?.ToString() ?? "";
                
                // Calculate layout
                var contentBounds = new Rectangle(
                    cellBounds.X + cellStyle.Padding.Left,
                    cellBounds.Y + cellStyle.Padding.Top,
                    cellBounds.Width - cellStyle.Padding.Horizontal,
                    cellBounds.Height - cellStyle.Padding.Vertical
                );
                
                var avatarSize = Math.Min(column.AvatarSize, contentBounds.Height - 4);
                var avatarBounds = new Rectangle(
                    contentBounds.X + 4,
                    contentBounds.Y + (contentBounds.Height - avatarSize) / 2,
                    avatarSize,
                    avatarSize
                );
                
                var textBounds = new Rectangle(
                    avatarBounds.Right + 8,
                    contentBounds.Y,
                    contentBounds.Width - avatarBounds.Width - 12,
                    contentBounds.Height
                );
                
                // Draw avatar
                DrawAvatar(graphics, avatarBounds, avatarPath, displayText, column.ShowInitialsWhenNoImage);
                
                // Draw text
                if (!string.IsNullOrEmpty(displayText) && textBounds.Width > 0)
                {
                    var textColor = cellState.HasFlag(DataGridViewElementStates.Selected) 
                        ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                    
                    using (var textBrush = new SolidBrush(textColor))
                    {
                        var stringFormat = new StringFormat
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,
                            FormatFlags = StringFormatFlags.NoWrap
                        };
                        
                        graphics.DrawString(displayText, cellStyle.Font, textBrush, textBounds, stringFormat);
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback to default rendering if custom painting fails
                System.Diagnostics.Debug.WriteLine($"Error painting DataGridViewImageTextCell: {ex.Message}");
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, 
                    errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }
        
        /// <summary>
        /// Draws the avatar image in the specified bounds
        /// </summary>
        private void DrawAvatar(Graphics graphics, Rectangle bounds, string? avatarPath, string displayText, bool showInitials)
        {
            try
            {
                // Create cache key
                var cacheKey = $"{avatarPath}|{displayText}|{bounds.Width}";
                
                // Check if we have a cached avatar
                if (_cachedAvatar == null || _cachedAvatarKey != cacheKey)
                {
                    // Dispose old cached avatar
                    _cachedAvatar?.Dispose();
                    
                    // Generate new avatar
                    if (showInitials)
                    {
                        _cachedAvatar = AvatarGenerator.LoadOrGenerateAvatar(avatarPath, displayText, bounds.Width);
                    }
                    else if (!string.IsNullOrEmpty(avatarPath))
                    {
                        _cachedAvatar = AvatarGenerator.LoadOrGenerateAvatar(avatarPath, displayText, bounds.Width);
                    }
                    else
                    {
                        _cachedAvatar = AvatarGenerator.GenerateDefaultAgentAvatar(bounds.Width);
                    }
                    
                    _cachedAvatarKey = cacheKey;
                }
                
                // Draw the avatar
                if (_cachedAvatar != null)
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(_cachedAvatar, bounds);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error drawing avatar: {ex.Message}");
                
                // Draw a simple placeholder circle if avatar generation fails
                using (var brush = new SolidBrush(UIStyles.DefaultStatusColor))
                {
                    graphics.FillEllipse(brush, bounds);
                }
            }
        }
        
        /// <summary>
        /// Gets a property value from an object using reflection
        /// </summary>
        private object? GetPropertyValue(object obj, string propertyName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                return property?.GetValue(obj);
            }
            catch
            {
                return null;
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cachedAvatar?.Dispose();
                _cachedAvatar = null;
            }
            base.Dispose(disposing);
        }
        
        public override object Clone()
        {
            var clone = (DataGridViewImageTextCell)base.Clone();
            clone._cachedAvatar = null;
            clone._cachedAvatarKey = null;
            return clone;
        }
    }
}