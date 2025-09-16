// RealEstateCRMWinForms\Utils\DataGridHelper.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using RealEstateCRMWinForms.Controls;

namespace RealEstateCRMWinForms.Utils
{
    /// <summary>
    /// Helper class for DataGrid operations and validation
    /// </summary>
    public static class DataGridHelper
    {
        /// <summary>
        /// Applies consistent styling to a DataGridView
        /// </summary>
        /// <param name="dataGridView">The DataGridView to style</param>
        public static void ApplyModernStyling(DataGridView dataGridView)
        {
            if (dataGridView == null) return;
            
            try
            {
                // Basic appearance
                dataGridView.EnableHeadersVisualStyles = false;
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dataGridView.RowHeadersVisible = false;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.MultiSelect = false;
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.ReadOnly = true;
                dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
                
                // Scrolling
                dataGridView.ScrollBars = ScrollBars.Both;
                dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                
                // Borders and colors
                dataGridView.BorderStyle = BorderStyle.FixedSingle;
                dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                dataGridView.GridColor = UIStyles.BorderColor;
                dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                
                // Header styling
                dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
                dataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 249, 250);
                dataGridView.ColumnHeadersDefaultCellStyle.Font = UIStyles.BoldFont;
                dataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
                
                // Cell styling
                dataGridView.DefaultCellStyle.Font = UIStyles.DefaultFont;
                dataGridView.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
                dataGridView.DefaultCellStyle.BackColor = Color.White;
                dataGridView.DefaultCellStyle.SelectionBackColor = UIStyles.SelectedRowColor;
                dataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(33, 37, 41);
                dataGridView.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
                dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                
                // Alternating row styling
                dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                dataGridView.AlternatingRowsDefaultCellStyle.WrapMode = DataGridViewTriState.False;
                
                // Row height
                dataGridView.RowTemplate.Height = Math.Max(UIStyles.AvatarSize + 8, 44);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying modern styling: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Validates that a DataGridView has been properly configured with custom columns
        /// </summary>
        /// <param name="dataGridView">The DataGridView to validate</param>
        /// <returns>True if validation passes</returns>
        public static bool ValidateDataGridConfiguration(DataGridView dataGridView)
        {
            if (dataGridView == null) return false;
            
            try
            {
                // Check that we have columns
                if (dataGridView.Columns.Count == 0) return false;
                
                // Check for custom column types
                bool hasCustomColumns = false;
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    if (column is DataGridViewImageTextColumn || 
                        column is DataGridViewBadgeColumn || 
                        column is DataGridViewScoreColumn)
                    {
                        hasCustomColumns = true;
                        break;
                    }
                }
                
                // Check basic styling
                bool hasProperStyling = 
                    dataGridView.DefaultCellStyle.Font != null &&
                    dataGridView.ColumnHeadersDefaultCellStyle.Font != null &&
                    dataGridView.RowTemplate.Height >= UIStyles.AvatarSize;
                
                return hasCustomColumns && hasProperStyling;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating DataGrid configuration: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Adds hover effects to a DataGridView
        /// </summary>
        /// <param name="dataGridView">The DataGridView to enhance</param>
        public static void AddHoverEffects(DataGridView dataGridView)
        {
            if (dataGridView == null) return;
            
            dataGridView.CellMouseEnter += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var row = dataGridView.Rows[e.RowIndex];
                    if (!row.Selected)
                    {
                        row.DefaultCellStyle.BackColor = UIStyles.RowHoverColor;
                    }
                }
            };
            
            dataGridView.CellMouseLeave += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var row = dataGridView.Rows[e.RowIndex];
                    if (!row.Selected)
                    {
                        // Reset to alternating row color or default
                        row.DefaultCellStyle.BackColor = (e.RowIndex % 2 == 1) 
                            ? dataGridView.AlternatingRowsDefaultCellStyle.BackColor 
                            : Color.White;
                    }
                }
            };
        }
        
        /// <summary>
        /// Creates a standard ImageText column for names with avatars
        /// </summary>
        /// <param name="headerText">Header text for the column</param>
        /// <param name="width">Column width</param>
        /// <param name="imagePropertyName">Property name for the image path</param>
        /// <param name="textPropertyName">Property name for the text</param>
        /// <returns>Configured DataGridViewImageTextColumn</returns>
        public static DataGridViewImageTextColumn CreateNameColumn(string headerText, int width, 
            string imagePropertyName = "AvatarPath", string textPropertyName = "FullName")
        {
            return new DataGridViewImageTextColumn
            {
                ImagePropertyName = imagePropertyName,
                TextPropertyName = textPropertyName,
                HeaderText = headerText,
                Name = textPropertyName,
                Width = width,
                ShowInitialsWhenNoImage = true,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    WrapMode = DataGridViewTriState.False,
                    Font = UIStyles.DefaultFont,
                    Padding = new Padding(8, 4, 8, 4)
                }
            };
        }
        
        /// <summary>
        /// Creates a standard Badge column for status values
        /// </summary>
        /// <param name="headerText">Header text for the column</param>
        /// <param name="width">Column width</param>
        /// <param name="dataPropertyName">Property name for the status data</param>
        /// <returns>Configured DataGridViewBadgeColumn</returns>
        public static DataGridViewBadgeColumn CreateStatusColumn(string headerText, int width, 
            string dataPropertyName = "Status")
        {
            return new DataGridViewBadgeColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = dataPropertyName,
                Width = width,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            };
        }
        
        /// <summary>
        /// Creates a standard Score column for numeric scores
        /// </summary>
        /// <param name="headerText">Header text for the column</param>
        /// <param name="width">Column width</param>
        /// <param name="dataPropertyName">Property name for the score data</param>
        /// <returns>Configured DataGridViewScoreColumn</returns>
        public static DataGridViewScoreColumn CreateScoreColumn(string headerText, int width, 
            string dataPropertyName = "Score")
        {
            return new DataGridViewScoreColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = dataPropertyName,
                Width = width,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    WrapMode = DataGridViewTriState.False,
                    Font = UIStyles.BoldFont
                }
            };
        }
    }
}