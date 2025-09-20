// RealEstateCRMWinForms\Views\LeadsView.cs
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using RealEstateCRMWinForms.Controls;
using RealEstateCRMWinForms.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class LeadsView : UserControl
    {
        private readonly LeadViewModel _viewModel;
        private BindingSource _bindingSource;
        private ContextMenuStrip _contextMenu;

        public LeadsView()
        {
            _viewModel = new LeadViewModel();
            _bindingSource = new BindingSource();
            InitializeComponent();
            ConfigureGridAppearance();
            InitializeData();
            CreateContextMenu();
        }

        private void ConfigureGridAppearance()
        {
            if (dataGridViewLeads == null) return;

            // Disable auto-generation to control columns manually
            dataGridViewLeads.AutoGenerateColumns = false;
            dataGridViewLeads.Columns.Clear();

            // Make table read-only (no inline editing)
            dataGridViewLeads.EnableHeadersVisualStyles = false;
            // Changed from Fill to None to prevent column shrinking and enable horizontal scrolling
            dataGridViewLeads.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridViewLeads.RowHeadersVisible = false;
            dataGridViewLeads.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewLeads.MultiSelect = false;
            dataGridViewLeads.AllowUserToAddRows = false;
            dataGridViewLeads.AllowUserToDeleteRows = false;
            dataGridViewLeads.ReadOnly = true;
            dataGridViewLeads.EditMode = DataGridViewEditMode.EditProgrammatically;

            // Enable horizontal scrolling
            dataGridViewLeads.ScrollBars = ScrollBars.Both;
            dataGridViewLeads.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            // Add border to the table and header
            dataGridViewLeads.BorderStyle = BorderStyle.FixedSingle;
            dataGridViewLeads.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewLeads.GridColor = Color.FromArgb(224, 224, 224);

            // Add border below header
            dataGridViewLeads.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 249, 250);

            // Set font size to 12 and styling using UIStyles
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.Font = UIStyles.BoldFont;
            dataGridViewLeads.DefaultCellStyle.Font = UIStyles.DefaultFont;
            dataGridViewLeads.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            dataGridViewLeads.DefaultCellStyle.BackColor = Color.White;
            dataGridViewLeads.DefaultCellStyle.SelectionBackColor = UIStyles.SelectedRowColor;
            dataGridViewLeads.DefaultCellStyle.SelectionForeColor = Color.FromArgb(33, 37, 41);
            dataGridViewLeads.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dataGridViewLeads.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            // Prevent text wrapping in headers and cells
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dataGridViewLeads.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dataGridViewLeads.AlternatingRowsDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // Increase row height for avatars and larger font
            dataGridViewLeads.RowTemplate.Height = Math.Max(UIStyles.AvatarSize + 8, 44);

            // Wire up events
            dataGridViewLeads.CellContentClick += DataGridViewLeads_CellContentClick;
            dataGridViewLeads.CellFormatting += DataGridViewLeads_CellFormatting;
            dataGridViewLeads.MouseClick += DataGridViewLeads_MouseClick;
            dataGridViewLeads.CellMouseEnter += DataGridViewLeads_CellMouseEnter;
            dataGridViewLeads.CellMouseLeave += DataGridViewLeads_CellMouseLeave;
        }

        private void CreateContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            var editMenuItem = new ToolStripMenuItem("Edit Lead");
            editMenuItem.Click += EditMenuItem_Click;

            var deleteMenuItem = new ToolStripMenuItem("Delete Lead");
            deleteMenuItem.Click += DeleteMenuItem_Click;

            _contextMenu.Items.Add(editMenuItem);
            _contextMenu.Items.Add(deleteMenuItem);
        }

        private void InitializeData()
        {
            try
            {
                // Create columns in the desired order
                CreateGridColumns();

                _bindingSource.DataSource = _viewModel.Leads;
                dataGridViewLeads.DataSource = _bindingSource;
                if (sortComboBox != null)
                    sortComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing LeadsView data: {ex.Message}");
                // Fallback to basic initialization if custom columns fail
                InitializeBasicColumns();
            }
        }

        /// <summary>
        /// Fallback method to initialize basic columns if custom columns fail
        /// </summary>
        private void InitializeBasicColumns()
        {
            try
            {
                dataGridViewLeads.Columns.Clear();
                dataGridViewLeads.AutoGenerateColumns = true;
                _bindingSource.DataSource = _viewModel.Leads;
                dataGridViewLeads.DataSource = _bindingSource;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in fallback initialization: {ex.Message}");
                MessageBox.Show("Error loading leads data. Please restart the application.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateGridColumns()
        {
            // 1. Lead Name with Avatar (using custom ImageText column)
            var nameColumn = new DataGridViewImageTextColumn
            {
                ImagePropertyName = "AvatarPath",
                TextPropertyName = "FullName",
                HeaderText = "Name",
                Name = "FullName",
                Width = 200,
                ShowInitialsWhenNoImage = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Font = UIStyles.DefaultFont,
                    Padding = new Padding(8, 4, 8, 4)
                }
            };
            dataGridViewLeads.Columns.Add(nameColumn);

            // 2. Status with colored badges (using custom Badge column)
            var statusColumn = new DataGridViewBadgeColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                Name = "Status",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            };
            dataGridViewLeads.Columns.Add(statusColumn);

            // 3. Source
            dataGridViewLeads.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Source",
                HeaderText = "Source",
                Name = "Source",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            // 4. Last Contacted
            dataGridViewLeads.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LastContacted",
                HeaderText = "Last Contacted",
                Name = "LastContacted",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            // 5. Agent with Avatar (using custom ImageText column)
            var agentColumn = new DataGridViewImageTextColumn
            {
                ImagePropertyName = "AgentAvatarPath",
                TextPropertyName = "AssignedAgent",
                HeaderText = "Agent",
                Name = "Agent",
                Width = 180,
                ShowInitialsWhenNoImage = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Font = UIStyles.DefaultFont,
                    Padding = new Padding(8, 4, 8, 4)
                }
            };
            dataGridViewLeads.Columns.Add(agentColumn);

            // 6. Move to Contacts Button (keep existing functionality)
            dataGridViewLeads.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Actions",
                Name = "CreateContact",
                Text = "Move to Contacts",
                UseColumnTextForButtonValue = true,
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            // Additional columns for detailed view (can be toggled)
            dataGridViewLeads.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Name = "Email",
                Width = 200,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            dataGridViewLeads.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Phone",
                HeaderText = "Phone",
                Name = "Phone",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            dataGridViewLeads.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Name = "Type",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });
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
            var query = searchBox?.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(query))
            {
                _bindingSource.DataSource = _viewModel.Leads;
            }
            else
            {
                var filtered = _viewModel.Leads.Where(l =>
                    (l.FullName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Email?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Phone?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Address?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Type?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Status?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

                _bindingSource.DataSource = new BindingList<Lead>(filtered);
            }
        }

        private void ApplySort()
        {
            var selectedSort = sortComboBox?.SelectedItem as string;
            List<Lead> sortedLeads;

            var currentData = (_bindingSource.DataSource as BindingList<Lead>)?.ToList() ?? _viewModel.Leads.ToList();

            sortedLeads = selectedSort switch
            {
                "Oldest to Newest" => currentData.OrderBy(l => l.CreatedAt).ToList(),
                "Name A-Z" => currentData.OrderBy(l => l.FullName).ToList(),
                "Name Z-A" => currentData.OrderByDescending(l => l.FullName).ToList(),
                _ => currentData.OrderByDescending(l => l.CreatedAt).ToList()
            };

            _bindingSource.DataSource = new BindingList<Lead>(sortedLeads);
        }

        private void BtnAddLead_Click(object? sender, EventArgs e)
        {
            var addLeadForm = new AddLeadForm();
            if (addLeadForm.ShowDialog() == DialogResult.OK && addLeadForm.CreatedLead != null)
            {
                if (_viewModel.AddLead(addLeadForm.CreatedLead))
                {
                    RefreshLeadsView();
                    // Show confirmation message
                    MessageBox.Show($"Lead '{addLeadForm.CreatedLead.FullName}' has been successfully added!",
                        "Lead Added",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void RefreshLeadsView()
        {
            try
            {
                int? selectedId = null;
                if (dataGridViewLeads.SelectedRows.Count > 0 && dataGridViewLeads.SelectedRows[0].DataBoundItem is Lead selectedLead)
                {
                    selectedId = selectedLead.Id;
                }

                _viewModel.LoadLeads();

                if (searchBox != null)
                    searchBox.Text = string.Empty;

                _bindingSource.DataSource = _viewModel.Leads;
                ApplySort();

                if (selectedId.HasValue)
                {
                    foreach (DataGridViewRow row in dataGridViewLeads.Rows)
                    {
                        if (row.DataBoundItem is Lead lead && lead.Id == selectedId)
                        {
                            row.Selected = true;
                            dataGridViewLeads.FirstDisplayedScrollingRowIndex = row.Index;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing leads: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridViewLeads_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = dataGridViewLeads.HitTest(e.X, e.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell && hitTestInfo.RowIndex >= 0)
                {
                    dataGridViewLeads.ClearSelection();
                    dataGridViewLeads.Rows[hitTestInfo.RowIndex].Selected = true;
                    _contextMenu.Show(dataGridViewLeads, e.Location);
                }
            }
        }

        private void EditMenuItem_Click(object? sender, EventArgs e)
        {
            if (dataGridViewLeads.SelectedRows.Count > 0 && dataGridViewLeads.SelectedRows[0].DataBoundItem is Lead lead)
            {
                var editForm = new EditLeadForm(lead);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    if (_viewModel.UpdateLead(lead))
                    {
                        RefreshLeadsView();
                        // Show confirmation message
                        MessageBox.Show($"Lead '{lead.FullName}' has been successfully updated!",
                            "Lead Updated",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void DeleteMenuItem_Click(object? sender, EventArgs e)
        {
            if (dataGridViewLeads.SelectedRows.Count > 0 && dataGridViewLeads.SelectedRows[0].DataBoundItem is Lead lead)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the lead '{lead.FullName}'?", "Delete Lead",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (_viewModel.DeleteLead(lead))
                    {
                        RefreshLeadsView();
                        // Show confirmation message
                        MessageBox.Show($"Lead '{lead.FullName}' has been successfully deleted!",
                            "Lead Deleted",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void DataGridViewLeads_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = dataGridViewLeads.Columns[e.ColumnIndex].Name;

            if (columnName == "Phone" && e.Value != null)
            {
                e.Value = FormatPhilippinePhoneNumber(e.Value.ToString());
                e.FormattingApplied = true;
            }
            else if (columnName == "LastContacted" && e.Value is DateTime dt)
            {
                e.Value = dt.ToString("MMM dd, yyyy");
                e.FormattingApplied = true;
            }
        }

        private async void DataGridViewLeads_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dataGridViewLeads.Columns[e.ColumnIndex].Name == "CreateContact" && dataGridViewLeads.Rows[e.RowIndex].DataBoundItem is Lead lead)
            {
                var result = MessageBox.Show($"This will move '{lead.FullName}' to Contacts and remove them from Leads.\n\nThey will receive a welcome email notification. Continue?", "Move to Contacts",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Disable the button to prevent multiple clicks
                    if (sender is DataGridView dgv)
                    {
                        dgv.Enabled = false;
                    }

                    try
                    {
                        if (await _viewModel.MoveLeadToContactAsync(lead))
                        {
                            RefreshLeadsView();
                            MessageBox.Show($"'{lead.FullName}' has been successfully moved to contacts and notified via email.", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    finally
                    {
                        // Re-enable the grid
                        if (sender is DataGridView gridView)
                        {
                            gridView.Enabled = true;
                        }
                    }
                }
            }
        }

        private void DataGridViewLeads_CellMouseEnter(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var row = dataGridViewLeads.Rows[e.RowIndex];
                if (!row.Selected)
                {
                    row.DefaultCellStyle.BackColor = UIStyles.RowHoverColor;
                }
            }
        }

        private void DataGridViewLeads_CellMouseLeave(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var row = dataGridViewLeads.Rows[e.RowIndex];
                if (!row.Selected)
                {
                    // Reset to alternating row color or default
                    row.DefaultCellStyle.BackColor = (e.RowIndex % 2 == 1)
                        ? dataGridViewLeads.AlternatingRowsDefaultCellStyle.BackColor
                        : Color.White;
                }
            }
        }

        private static string FormatPhilippinePhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            string digits = Regex.Replace(phoneNumber, @"[^\d]", "");

            if (digits.StartsWith("63") && digits.Length == 12)
                return $"+{digits.Substring(0, 2)} {digits.Substring(2, 3)} {digits.Substring(5, 3)} {digits.Substring(8)}";
            if (digits.StartsWith("0") && digits.Length == 11)
                return $"{digits.Substring(0, 4)} {digits.Substring(4, 3)} {digits.Substring(7)}";
            if (digits.Length == 10)
                return $"0{digits.Substring(0, 3)} {digits.Substring(3, 3)} {digits.Substring(6)}";

            return phoneNumber; // Return original if no match
        }

        private void dataGridViewLeads_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}