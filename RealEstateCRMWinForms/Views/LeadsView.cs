// RealEstateCRMWinForms\Views\LeadsView.cs
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class LeadsView : UserControl
    {
        private readonly LeadViewModel _viewModel;
        private BindingSource _bindingSource;

        public LeadsView()
        {
            _viewModel = new LeadViewModel();
            _bindingSource = new BindingSource();
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            _bindingSource.DataSource = _viewModel.Leads;
            dataGridViewLeads.DataSource = _bindingSource;
            sortComboBox.SelectedIndex = 0;
        }

        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void SortComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ApplySort();
        }

        private void ApplyFilter()
        {
            var query = searchBox.Text?.Trim() ?? string.Empty;
            
            if (string.IsNullOrEmpty(query))
            {
                _bindingSource.DataSource = _viewModel.Leads;
            }
            else
            {
                var filtered = _viewModel.Leads.Where(l =>
                    l.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    l.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    l.Phone.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    l.Address.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    l.Type.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    l.Status.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

                _bindingSource.DataSource = new BindingList<Lead>(filtered);
            }
        }

        private void ApplySort()
        {
            var selectedSort = sortComboBox.SelectedItem as string;
            List<Lead> sortedLeads;

            var currentData = _bindingSource.DataSource as BindingList<Lead> ?? _viewModel.Leads;

            sortedLeads = selectedSort switch
            {
                "Oldest to Newest" => currentData.OrderBy(l => l.CreatedAt).ToList(),
                "Name A-Z" => currentData.OrderBy(l => l.FullName).ToList(),
                "Name Z-A" => currentData.OrderByDescending(l => l.FullName).ToList(),
                _ => currentData.OrderByDescending(l => l.CreatedAt).ToList() // Newest to Oldest (default)
            };

            _bindingSource.DataSource = new BindingList<Lead>(sortedLeads);
        }

        private void BtnAddLead_Click(object? sender, EventArgs e)
        {
            var addLeadForm = new AddLeadForm();
            if (addLeadForm.ShowDialog() == DialogResult.OK && addLeadForm.CreatedLead != null)
            {
                // Add the new lead to the database via ViewModel
                if (_viewModel.AddLead(addLeadForm.CreatedLead))
                {
                    // Clear any search filter to show all leads including the new one
                    searchBox.Text = string.Empty;
                    
                    // Reset to show the original leads collection
                    _bindingSource.DataSource = _viewModel.Leads;
                    
                    // Reapply current sorting to show the new lead in correct position
                    ApplySort();
                    
                    // Refresh the DataGridView
                    dataGridViewLeads.Refresh();
                    
                    // Optionally, select the newly added lead (it will be at the top for "Newest to Oldest")
                    if (dataGridViewLeads.Rows.Count > 0)
                    {
                        dataGridViewLeads.ClearSelection();
                        dataGridViewLeads.Rows[0].Selected = true;
                        dataGridViewLeads.FirstDisplayedScrollingRowIndex = 0;
                    }
                }
            }
        }

        // Alternative method to refresh the entire view
        private void RefreshLeadsView()
        {
            // Reload from database to ensure we have the latest data
            _viewModel.LoadLeads();
            
            // Reset the binding source
            _bindingSource.DataSource = _viewModel.Leads;
            
            // Reapply current filter and sort
            ApplyFilter();
            ApplySort();
            
            // Refresh the DataGridView
            dataGridViewLeads.Refresh();
        }

        // Prevent column header click highlighting
        private void DataGridViewLeads_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            // Do nothing to prevent sorting and blue highlighting
            // The sorting is handled by our custom sort dropdown instead
        }

        private void DataGridViewLeads_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            // No special formatting needed for the new columns
        }

        private void DataGridViewLeads_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return; // Skip header row

            var lead = (Lead)dataGridViewLeads.Rows[e.RowIndex].DataBoundItem;

            // Avatar column
            if (dataGridViewLeads.Columns[e.ColumnIndex].Name == "Avatar")
            {
                e.PaintBackground(e.ClipBounds, true);
                DrawAvatar(e.Graphics, e.CellBounds, lead.FullName);
                e.Handled = true;
            }
            // Status column
            else if (dataGridViewLeads.Columns[e.ColumnIndex].Name == "Status")
            {
                e.PaintBackground(e.ClipBounds, true);
                DrawStatusBadge(e.Graphics, e.CellBounds, lead.Status);
                e.Handled = true;
            }
            // Create Contact button column
            else if (dataGridViewLeads.Columns[e.ColumnIndex].Name == "CreateContact")
            {
                e.PaintBackground(e.ClipBounds, true);
                DrawContactButton(e.Graphics, e.CellBounds);
                e.Handled = true;
            }
            // Type column
            else if (dataGridViewLeads.Columns[e.ColumnIndex].Name == "Type")
            {
                e.PaintBackground(e.ClipBounds, true);
                DrawTypeBadge(e.Graphics, e.CellBounds, lead.Type);
                e.Handled = true;
            }
        }

        private void DrawAvatar(Graphics g, Rectangle bounds, string name)
        {
            var avatarSize = 36;
            var x = bounds.X + (bounds.Width - avatarSize) / 2;
            var y = bounds.Y + (bounds.Height - avatarSize) / 2;
            var avatarRect = new Rectangle(x, y, avatarSize, avatarSize);
            
            // Draw circle background
            using (var brush = new SolidBrush(Color.FromArgb(230, 230, 230)))
                g.FillEllipse(brush, avatarRect);
            
            // Draw initials
            var initials = GetInitials(name);
            using (var font = new Font("Segoe UI", 11F, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(80, 80, 80)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(initials, font, brush, avatarRect, sf);
            }
        }

        private void DrawStatusBadge(Graphics g, Rectangle bounds, string status)
        {
            var badgeWidth = Math.Max(90, TextRenderer.MeasureText(status, new Font("Segoe UI", 8F)).Width + 20);
            var badgeHeight = 24;
            var x = bounds.X + 15;
            var y = bounds.Y + (bounds.Height - badgeHeight) / 2;
            var badgeRect = new Rectangle(x, y, badgeWidth, badgeHeight);
            
            Color fillColor = status switch
            {
                "New Lead" => Color.FromArgb(255, 235, 175), // Orange
                "Contacted" => Color.FromArgb(255, 248, 220), // Yellow
                "Qualified" => Color.FromArgb(220, 247, 247), // Light blue
                "Unqualified" => Color.FromArgb(255, 230, 230), // Light red
                _ => Color.FromArgb(240, 240, 240)
            };
            
            Color textColor = status switch
            {
                "New Lead" => Color.FromArgb(180, 83, 9), // Orange text
                "Contacted" => Color.FromArgb(146, 124, 0), // Yellow text
                "Qualified" => Color.FromArgb(5, 150, 155), // Blue text
                "Unqualified" => Color.FromArgb(185, 28, 28), // Red text
                _ => Color.FromArgb(108, 117, 125)
            };
            
            DrawRoundedBadge(g, badgeRect, fillColor, textColor, status);
        }

        private void DrawContactButton(Graphics g, Rectangle bounds)
        {
            var buttonWidth = Math.Min(120, bounds.Width - 20);
            var buttonHeight = 28;
            var x = bounds.X + 10;
            var y = bounds.Y + (bounds.Height - buttonHeight) / 2;
            var buttonRect = new Rectangle(x, y, buttonWidth, buttonHeight);
            
            // Green button background
            using (var brush = new SolidBrush(Color.FromArgb(40, 167, 69)))
                DrawRoundedButton(g, buttonRect, brush);
            
            // Button text
            using (var font = new Font("Segoe UI", 7F, FontStyle.Regular))
            using (var brush = new SolidBrush(Color.White))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("Move to Contacts", font, brush, buttonRect, sf);
            }
        }

        private void DrawTypeBadge(Graphics g, Rectangle bounds, string type)
        {
            var badgeWidth = Math.Max(70, TextRenderer.MeasureText(type, new Font("Segoe UI", 8F)).Width + 20);
            var badgeHeight = 24;
            var x = bounds.X + 15;
            var y = bounds.Y + (bounds.Height - badgeHeight) / 2;
            var badgeRect = new Rectangle(x, y, badgeWidth, badgeHeight);
            
            Color fillColor = type switch
            {
                "Renter" => Color.FromArgb(255, 192, 203), // Pink
                "Owner" => Color.FromArgb(144, 238, 144), // Light green
                "Buyer" => Color.FromArgb(173, 216, 230), // Light blue
                _ => Color.FromArgb(240, 240, 240)
            };
            
            Color textColor = type switch
            {
                "Renter" => Color.FromArgb(139, 69, 19), // Brown
                "Owner" => Color.FromArgb(0, 100, 0), // Green
                "Buyer" => Color.FromArgb(25, 25, 112), // Dark blue
                _ => Color.FromArgb(108, 117, 125)
            };
            
            DrawRoundedBadge(g, badgeRect, fillColor, textColor, type);
        }

        private void DrawRoundedBadge(Graphics g, Rectangle rect, Color fillColor, Color textColor, string text)
        {
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                var radius = 12;
                path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();
                
                using (var brush = new SolidBrush(fillColor))
                    g.FillPath(brush, path);
            }
            
            using (var font = new Font("Segoe UI", 8F, FontStyle.Regular))
            using (var brush = new SolidBrush(textColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, font, brush, rect, sf);
            }
        }

        private void DrawRoundedButton(Graphics g, Rectangle rect, Brush brush)
        {
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                var radius = 8;
                path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();
                
                g.FillPath(brush, path);
            }
        }

        private void DataGridViewLeads_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridViewLeads.Columns[e.ColumnIndex].Name == "CreateContact")
            {
                var lead = (Lead)dataGridViewLeads.Rows[e.RowIndex].DataBoundItem;
                
                var result = MessageBox.Show($"Move {lead.FullName} to Contacts?", "Move to Contacts", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    // Delete the lead from database and UI
                    if (_viewModel.DeleteLead(lead))
                    {
                        // Refresh the view to remove the deleted lead
                        RefreshLeadsView();
                        MessageBox.Show($"{lead.FullName} has been moved to contacts.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();
            return (parts[0][0].ToString() + parts[1][0].ToString()).ToUpperInvariant();
        }
    }
}