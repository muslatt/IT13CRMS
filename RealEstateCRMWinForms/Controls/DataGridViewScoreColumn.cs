// RealEstateCRMWinForms\Controls\DataGridViewScoreColumn.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using RealEstateCRMWinForms.Utils;

namespace RealEstateCRMWinForms.Controls
{
    /// <summary>
    /// Custom DataGridView column that displays score values with color coding
    /// </summary>
    public class DataGridViewScoreColumn : DataGridViewColumn
    {
        /// <summary>
        /// Threshold for high scores (green color)
        /// </summary>
        public int HighScoreThreshold { get; set; } = 80;
        
        /// <summary>
        /// Threshold for medium scores (yellow color)
        /// </summary>
        public int MediumScoreThreshold { get; set; } = 60;
        
        /// <summary>
        /// Color for high scores
        /// </summary>
        public Color HighScoreColor { get; set; } = UIStyles.HighScoreColor;
        
        /// <summary>
        /// Color for medium scores
        /// </summary>
        public Color MediumScoreColor { get; set; } = UIStyles.MediumScoreColor;
        
        /// <summary>
        /// Color for low scores
        /// </summary>
        public Color LowScoreColor { get; set; } = UIStyles.LowScoreColor;
        
        /// <summary>
        /// Whether to use bold font for scores
        /// </summary>
        public bool UseBoldFont { get; set; } = true;
        
        public DataGridViewScoreColumn() : base(new DataGridViewScoreCell())
        {
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.DefaultCellStyle.Font = UIStyles.BoldFont;
            this.DefaultCellStyle.Padding = new Padding(4);
        }
        
        /// <summary>
        /// Gets the appropriate color for a score value
        /// </summary>
        /// <param name="score">The score value</param>
        /// <returns>Color for the score</returns>
        public Color GetScoreColor(int score)
        {
            return score switch
            {
                var s when s >= HighScoreThreshold => HighScoreColor,
                var s when s >= MediumScoreThreshold => MediumScoreColor,
                _ => LowScoreColor
            };
        }
        
        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewScoreCell)))
                {
                    throw new InvalidCastException("Must be a DataGridViewScoreCell");
                }
                base.CellTemplate = value;
            }
        }
    }
    
    /// <summary>
    /// Custom DataGridView cell that renders score values with color coding
    /// </summary>
    public class DataGridViewScoreCell : DataGridViewCell
    {
        public DataGridViewScoreCell()
        {
            this.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        
        public override Type EditType => null; // Read-only cell
        
        public override Type ValueType => typeof(int);
        
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, 
            int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, 
            string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, 
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
                var column = this.OwningColumn as DataGridViewScoreColumn;
                if (column == null)
                {
                    // Fallback to default rendering
                    base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, 
                        errorText, cellStyle, advancedBorderStyle, paintParts);
                    return;
                }
                
                // Parse the score value
                if (!TryParseScore(value, out int score))
                {
                    // If we can't parse the score, show the original value
                    DrawText(graphics, cellBounds, formattedValue?.ToString() ?? "", cellStyle, cellState, Color.Gray);
                    return;
                }
                
                // Get the color for this score
                var scoreColor = column.GetScoreColor(score);
                
                // Determine text color based on selection state
                var textColor = cellState.HasFlag(DataGridViewElementStates.Selected) 
                    ? cellStyle.SelectionForeColor : scoreColor;
                
                // Draw the score
                var scoreText = score.ToString();
                var font = column.UseBoldFont ? UIStyles.BoldFont : cellStyle.Font;
                
                DrawText(graphics, cellBounds, scoreText, cellStyle, cellState, textColor, font);
                
                // Optionally draw a subtle background indicator for very high/low scores
                if (score >= column.HighScoreThreshold || score < column.MediumScoreThreshold)
                {
                    DrawScoreIndicator(graphics, cellBounds, score, column, cellState);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error painting DataGridViewScoreCell: {ex.Message}");
                
                // Fallback to default rendering
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, 
                    errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }
        
        /// <summary>
        /// Attempts to parse a score value from the cell value
        /// </summary>
        private bool TryParseScore(object value, out int score)
        {
            score = 0;
            
            if (value == null) return false;
            
            if (value is int intValue)
            {
                score = intValue;
                return true;
            }
            
            if (value is double doubleValue)
            {
                score = (int)Math.Round(doubleValue);
                return true;
            }
            
            if (value is decimal decimalValue)
            {
                score = (int)Math.Round(decimalValue);
                return true;
            }
            
            return int.TryParse(value.ToString(), out score);
        }
        
        /// <summary>
        /// Draws text in the cell with the specified color and font
        /// </summary>
        private void DrawText(Graphics graphics, Rectangle cellBounds, string text, 
            DataGridViewCellStyle cellStyle, DataGridViewElementStates cellState, Color textColor, Font? font = null)
        {
            if (string.IsNullOrEmpty(text)) return;
            
            var contentBounds = new Rectangle(
                cellBounds.X + cellStyle.Padding.Left,
                cellBounds.Y + cellStyle.Padding.Top,
                cellBounds.Width - cellStyle.Padding.Horizontal,
                cellBounds.Height - cellStyle.Padding.Vertical
            );
            
            using (var textBrush = new SolidBrush(textColor))
            {
                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                
                var drawFont = font ?? cellStyle.Font;
                graphics.DrawString(text, drawFont, textBrush, contentBounds, stringFormat);
            }
        }
        
        /// <summary>
        /// Draws a subtle background indicator for exceptional scores
        /// </summary>
        private void DrawScoreIndicator(Graphics graphics, Rectangle cellBounds, int score, 
            DataGridViewScoreColumn column, DataGridViewElementStates cellState)
        {
            // Don't draw indicator if cell is selected (to avoid visual clutter)
            if (cellState.HasFlag(DataGridViewElementStates.Selected)) return;
            
            Color indicatorColor;
            int alpha = 20; // Very subtle
            
            if (score >= column.HighScoreThreshold)
            {
                indicatorColor = Color.FromArgb(alpha, column.HighScoreColor);
            }
            else if (score < column.MediumScoreThreshold)
            {
                indicatorColor = Color.FromArgb(alpha, column.LowScoreColor);
            }
            else
            {
                return; // No indicator for medium scores
            }
            
            // Draw a subtle background tint
            using (var brush = new SolidBrush(indicatorColor))
            {
                var indicatorBounds = new Rectangle(
                    cellBounds.X + 2,
                    cellBounds.Y + 2,
                    cellBounds.Width - 4,
                    cellBounds.Height - 4
                );
                
                graphics.FillRectangle(brush, indicatorBounds);
            }
        }
        
        public override object Clone()
        {
            return (DataGridViewScoreCell)base.Clone();
        }
    }
}