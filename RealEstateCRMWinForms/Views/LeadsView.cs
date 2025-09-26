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
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer; 

namespace RealEstateCRMWinForms.Views
{
    public partial class LeadsView : UserControl
    {
        private readonly LeadViewModel _viewModel;
        private BindingSource _bindingSource;
        private ContextMenuStrip _contextMenu;

        private const int PageSize = 10;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private List<Lead> _currentLeads = new();
        private bool _isDataInitialized;

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

            // Allow editing only for Type column (dropdown functionality)
            dataGridViewLeads.EnableHeadersVisualStyles = false;
            // Fill available width to avoid empty space on the right
            dataGridViewLeads.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewLeads.RowHeadersVisible = false;
            dataGridViewLeads.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewLeads.MultiSelect = false;
            dataGridViewLeads.AllowUserToAddRows = false;
            dataGridViewLeads.AllowUserToDeleteRows = false;
            dataGridViewLeads.ReadOnly = false; // Allow editing for dropdown functionality
            dataGridViewLeads.EditMode = DataGridViewEditMode.EditOnEnter;

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
            dataGridViewLeads.CellValueChanged += DataGridViewLeads_CellValueChanged;
            dataGridViewLeads.CurrentCellDirtyStateChanged += DataGridViewLeads_CurrentCellDirtyStateChanged;
            dataGridViewLeads.CellBeginEdit += DataGridViewLeads_CellBeginEdit;
            dataGridViewLeads.EditingControlShowing += DataGridViewLeads_EditingControlShowing;
            dataGridViewLeads.DataError += DataGridViewLeads_DataError;
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
                _isDataInitialized = false;

                if (!_viewModel.TestConnection())
                {
                    ShowDatabaseConnectionInfo();
                    return;
                }

                CreateGridColumns();
                dataGridViewLeads.DataSource = _bindingSource;

                if (sortComboBox != null && sortComboBox.Items.Count > 0)
                {
                    sortComboBox.SelectedIndex = 0;
                }

                ApplyFilterAndSort(true);
                _isDataInitialized = true;
                UpdatePaginationControls();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing LeadsView data: {ex.Message}");
                InitializeBasicColumns();
            }
        }


        private void ShowDatabaseConnectionInfo()
        {
            // Create a label to show database status instead of blocking error dialog
            var infoLabel = new Label
            {
                Text = "Database connection not available. Some features may be limited.",
                ForeColor = Color.Orange,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10)
            };

            // Insert at the top of the control
            this.Controls.Add(infoLabel);
            infoLabel.BringToFront();

            // Still initialize the grid with empty data
            CreateGridColumns();
            _bindingSource.DataSource = _viewModel.Leads;
            dataGridViewLeads.DataSource = _bindingSource;
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
                // Create a simple message instead of blocking dialog
                var errorLabel = new Label
                {
                    Text = "Unable to load leads data. Please check your configuration.",
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                this.Controls.Add(errorLabel);
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
                ReadOnly = true, // Name column should not be editable
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Font = UIStyles.DefaultFont,
                    Padding = new Padding(8, 4, 8, 4)
                }
            };
            nameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            nameColumn.FillWeight = 180f;
            dataGridViewLeads.Columns.Add(nameColumn);

            // Type column with dropdown functionality - UPDATED
            var typeColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Name = "Type",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False },
                DropDownWidth = 120,
                MaxDropDownItems = 5,
                FlatStyle = FlatStyle.Flat
            };
            
            // Add dropdown items - only "Contact" for conversion functionality
            // Include known types so existing values display correctly
            typeColumn.Items.AddRange(new string[] { "Lead", "Contact", "Buyer", "Owner", "Renter" });
            typeColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing; // Show dropdown arrow only on edit
            
            typeColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewLeads.Columns.Add(typeColumn);

            // NOTE: Removed Status, Source, Last Contacted, and Actions columns per request.

            // 2. Agent with Avatar (using custom ImageText column)
            var agentColumn = new DataGridViewImageTextColumn
            {
                ImagePropertyName = "AgentAvatarPath",
                TextPropertyName = "AssignedAgent",
                HeaderText = "Agent",
                Name = "Agent",
                Width = 180,
                ShowInitialsWhenNoImage = true,
                ReadOnly = true, // Agent column should not be editable
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Font = UIStyles.DefaultFont,
                    Padding = new Padding(8, 4, 8, 4)
                }
            };
            agentColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            agentColumn.FillWeight = 140f;
            dataGridViewLeads.Columns.Add(agentColumn);

            // Additional columns for detailed view (can be toggled)
            var emailColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Name = "Email",
                Width = 200,
                ReadOnly = true, // Email column should not be editable
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            };
            emailColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            emailColumn.FillWeight = 180f;
            dataGridViewLeads.Columns.Add(emailColumn);

            var phoneColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Phone",
                HeaderText = "Phone",
                Name = "Phone",
                Width = 140,
                ReadOnly = true, // Phone column should not be editable
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            };
            phoneColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewLeads.Columns.Add(phoneColumn);

            // New: Occupation column
            var occupationColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Occupation",
                HeaderText = "Occupation",
                Name = "Occupation",
                Width = 160,
                ReadOnly = true, // Occupation column should not be editable
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            };
            occupationColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            occupationColumn.FillWeight = 140f;
            dataGridViewLeads.Columns.Add(occupationColumn);

            // New: Salary column
            var salaryColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Salary",
                HeaderText = "Salary",
                Name = "Salary",
                Width = 120,
                ReadOnly = true, // Salary column should not be editable
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    // We'll format the salary in CellFormatting handler for currency display
                }
            };
            salaryColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewLeads.Columns.Add(salaryColumn);
        }

        // NEW EVENT HANDLERS FOR DROPDOWN FUNCTIONALITY
        private void DataGridViewLeads_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            // Only allow editing of the Type column
            if (dataGridViewLeads.Columns[e.ColumnIndex].Name != "Type")
            {
                e.Cancel = true;
                return;
            }

            // Get the current lead and its type
            if (e.RowIndex >= 0 && dataGridViewLeads.Rows[e.RowIndex].DataBoundItem is Lead lead)
            {
                if (dataGridViewLeads.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell typeCell)
                {
                    typeCell.Items.Clear();
                    typeCell.Items.Add(lead.Type);
                    if (!string.Equals(lead.Type, "Contact", StringComparison.OrdinalIgnoreCase))
                    {
                        typeCell.Items.Add("Contact");
                    }
                }
            }
        }
        private void DataGridViewLeads_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            // Commit changes immediately when dropdown selection changes
            if (dataGridViewLeads.IsCurrentCellDirty && dataGridViewLeads.CurrentCell?.OwningColumn?.Name == "Type")
            {
                dataGridViewLeads.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private async void DataGridViewLeads_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            // Handle Type column changes
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                dataGridViewLeads.Columns[e.ColumnIndex].Name == "Type")
            {
                var lead = dataGridViewLeads.Rows[e.RowIndex].DataBoundItem as Lead;
                if (lead != null)
                {
                    var newValue = dataGridViewLeads.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    var originalType = lead.Type; // Store original type for potential reversion

                    if (newValue == "Contact")
                    {
                        await HandleLeadToContactConversion(lead, e.RowIndex, originalType);
                    }
                    else
                    {
                        // Update the lead type in the model for other type changes
                        lead.Type = newValue ?? "Renter";
                        if (_viewModel.UpdateLead(lead))
                        {
                            // Check if row still exists before accessing it
                            if (e.RowIndex < dataGridViewLeads.Rows.Count && dataGridViewLeads.Rows[e.RowIndex].DataBoundItem == lead)
                            {
                                // Show brief confirmation with visual feedback
                                var originalColor = dataGridViewLeads.Rows[e.RowIndex].DefaultCellStyle.BackColor;
                                dataGridViewLeads.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;

                            
                                var timer = new Timer(); // NOW THIS WILL WORK CORRECTLY
                                timer.Interval = 1500;
                                timer.Tick += (s, args) =>
                                {
                                    // Double-check row still exists and matches the lead
                                    if (e.RowIndex < dataGridViewLeads.Rows.Count &&
                                        dataGridViewLeads.Rows[e.RowIndex].DataBoundItem == lead)
                                    {
                                        dataGridViewLeads.Rows[e.RowIndex].DefaultCellStyle.BackColor = originalColor;
                                    }
                                    timer.Stop();
                                    timer.Dispose();
                                };
                                timer.Start();
                            }
                        }
                        else
                        {
                            // If update failed, revert to original type
                            lead.Type = originalType;
                            // Check if row still exists before accessing it
                            if (e.RowIndex < dataGridViewLeads.Rows.Count && dataGridViewLeads.Rows[e.RowIndex].DataBoundItem == lead)
                            {
                                dataGridViewLeads.Rows[e.RowIndex].Cells["Type"].Value = originalType;
                            }
                            MessageBox.Show("Failed to update lead type. Please try again.", "Update Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private async Task HandleLeadToContactConversion(Lead lead, int rowIndex, string originalType)
        {
            try
            {
                // Show confirmation dialog
                var result = MessageBox.Show(
                    $"Convert '{lead.FullName}' from Lead to Contact?\n\n" +
                    "This will MOVE the lead to the Contacts section and REMOVE it from the Leads list.\n" +
                    "This action cannot be undone.",
                    "Convert to Contact",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Show processing indicator
                    this.Cursor = Cursors.WaitCursor;
                    
                    try
                    {
                        // Use the existing MoveLeadToContactAsync method from LeadViewModel
                        // This method should:
                        // 1. Create a new Contact with the lead's data
                        // 2. Set the lead's IsActive to false (soft delete)
                        // 3. Remove the lead from the local collection
                        bool conversionSuccess = await _viewModel.MoveLeadToContactAsync(lead);
                        
                        if (conversionSuccess)
                        {
                            // Refresh the leads view to remove the converted lead
                            RefreshLeadsView();
                            
                            // Show success message
                            MessageBox.Show(
                                $"'{lead.FullName}' has been successfully converted to a Contact!\n\n" +
                                "You can now find them in the Contacts section.",
                                "Conversion Successful",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Conversion failed, revert the dropdown selection
                            // Check if row still exists before accessing it
                            if (rowIndex < dataGridViewLeads.Rows.Count && dataGridViewLeads.Rows[rowIndex].DataBoundItem == lead)
                            {
                                dataGridViewLeads.Rows[rowIndex].Cells["Type"].Value = originalType;
                            }
                            lead.Type = originalType;
                            
                            MessageBox.Show(
                                "Failed to convert lead to contact. The database operation was not successful.\n\n" +
                                "Please check your database connection and try again.",
                                "Conversion Failed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception conversionEx)
                    {
                        // Handle any exceptions during conversion
                        // Check if row still exists before accessing it
                        if (rowIndex < dataGridViewLeads.Rows.Count && dataGridViewLeads.Rows[rowIndex].DataBoundItem == lead)
                        {
                            dataGridViewLeads.Rows[rowIndex].Cells["Type"].Value = originalType;
                        }
                        lead.Type = originalType;
                        
                        MessageBox.Show(
                            $"An error occurred during conversion:\n{conversionEx.Message}\n\n" +
                            "The lead type has been reverted to its original value.",
                            "Conversion Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
                else
                {
                    // User cancelled, revert the dropdown selection
                    // Check if row still exists before accessing it
                    if (rowIndex < dataGridViewLeads.Rows.Count && dataGridViewLeads.Rows[rowIndex].DataBoundItem == lead)
                    {
                        dataGridViewLeads.Rows[rowIndex].Cells["Type"].Value = originalType;
                    }
                    lead.Type = originalType;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error during conversion process: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                // Revert the dropdown selection on error
                // Check if row still exists before accessing it
                if (rowIndex < dataGridViewLeads.Rows.Count && dataGridViewLeads.Rows[rowIndex].DataBoundItem == lead)
                {
                    dataGridViewLeads.Rows[rowIndex].Cells["Type"].Value = originalType;
                }
                lead.Type = originalType;
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
            else if (columnName == "Salary" && e.Value != null)
            {
                // Format salary as currency with no decimal places (₱)
                if (decimal.TryParse(e.Value.ToString(), out var sal))
                {
                    e.Value = $"₱{sal:N0}";
                    e.FormattingApplied = true;
                }
                else
                {
                    // If it's not parseable leave as-is
                }
            }
        }

        private async void DataGridViewLeads_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Actions column removed; keep handler empty for future use.
            return;
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
            ApplyFilterAndSort(true);
        }

        private void ApplySort()
        {
            if (!_isDataInitialized)
            {
                return;
            }

            ApplyFilterAndSort(true);
        }
        private void ApplyFilterAndSort(bool resetPage)
        {
            var query = searchBox?.Text?.Trim() ?? string.Empty;
            IEnumerable<Lead> data = _viewModel.Leads;

            if (!string.IsNullOrWhiteSpace(query))
            {
                data = data.Where(l =>
                    (l.FullName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Email?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Phone?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Occupation?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Type?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            var filtered = data.ToList();
            var sorted = SortLeads(filtered, sortComboBox?.SelectedItem as string);

            SetCurrentLeads(sorted, resetPage);
        }

        private static List<Lead> SortLeads(List<Lead> leads, string? sortOption)
        {
            return (sortOption ?? "Newest to Oldest") switch
            {
                "Oldest to Newest" => leads.OrderBy(l => l.CreatedAt).ToList(),
                "Name A-Z" => leads.OrderBy(l => l.FullName ?? string.Empty).ToList(),
                "Name Z-A" => leads.OrderByDescending(l => l.FullName ?? string.Empty).ToList(),
                _ => leads.OrderByDescending(l => l.CreatedAt).ToList()
            };
        }

        private void SetCurrentLeads(IEnumerable<Lead> leads, bool resetPage)
        {
            _currentLeads = leads?.ToList() ?? new List<Lead>();

            if (_currentLeads.Count == 0)
            {
                _currentPage = 1;
            }
            else if (resetPage || _currentPage < 1)
            {
                _currentPage = 1;
            }
            else if (_currentPage > _totalPages)
            {
                _currentPage = _totalPages;
            }

            ApplyPagination();
        }

        private void ApplyPagination()
        {
            var wasVisible = dataGridViewLeads.Visible;
            dataGridViewLeads.Visible = false;
            dataGridViewLeads.SuspendLayout();
            dataGridViewLeads.SuspendDrawing();
            try
            {
                if (_currentLeads.Count == 0)
                {
                    _totalPages = 1;
                    _bindingSource.DataSource = new BindingList<Lead>();
                }
                else
                {
                    _totalPages = (int)Math.Ceiling(_currentLeads.Count / (double)PageSize);
                    if (_currentPage > _totalPages)
                    {
                        _currentPage = _totalPages;
                    }

                    var pageLeads = _currentLeads
                        .Skip((_currentPage - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

                    _bindingSource.DataSource = new BindingList<Lead>(pageLeads);

                    // Reset scroll to top if rows exist
                    try { if (dataGridViewLeads.Rows.Count > 0) dataGridViewLeads.FirstDisplayedScrollingRowIndex = 0; } catch { }
                }

                dataGridViewLeads.PerformLayout();
                dataGridViewLeads.Invalidate(true);
                dataGridViewLeads.Update();
            }
            finally
            {
                dataGridViewLeads.ResumeLayout(true);
                dataGridViewLeads.ResumeDrawing();
                dataGridViewLeads.Visible = wasVisible;
                this.Invalidate(true);
                this.Update();
                try { Application.DoEvents(); } catch { }
            }

            UpdatePaginationControls();
        }

        private void UpdatePaginationControls()
        {
            var totalItems = _currentLeads.Count;
            if (totalItems == 0)
            {
                lblLeadPageInfo.Text = "No leads to display";
            }
            else
            {
                var start = ((_currentPage - 1) * PageSize) + 1;
                var end = Math.Min(start + PageSize - 1, totalItems);
                lblLeadPageInfo.Text = $"Showing {start}–{end} of {totalItems}";
            }

            btnPrevLeadPage.Enabled = _currentPage > 1;
            btnNextLeadPage.Enabled = _currentPage < _totalPages;
        }

        private void BtnPrevLeadPage_Click(object? sender, EventArgs e)
        {
            if (_currentPage <= 1)
            {
                return;
            }

            _currentPage--;
            ApplyPagination();
        }

        private void BtnNextLeadPage_Click(object? sender, EventArgs e)
        {
            if (_currentPage >= _totalPages)
            {
                return;
            }

            _currentPage++;
            ApplyPagination();
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

        private void DataGridViewLeads_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridViewLeads.CurrentCell?.OwningColumn?.Name != "Type")
            {
                if (e.Control is ComboBox comboReset)
                {
                    comboReset.SelectionChangeCommitted -= DataGridViewLeads_TypeComboBox_SelectionChangeCommitted;
                    comboReset.KeyPress -= DataGridViewLeads_TypeComboBox_KeyPress;
                    comboReset.Tag = null;
                }
                return;
            }

            if (e.Control is ComboBox combo)
            {
                combo.SelectionChangeCommitted -= DataGridViewLeads_TypeComboBox_SelectionChangeCommitted;
                combo.KeyPress -= DataGridViewLeads_TypeComboBox_KeyPress;

                var lead = dataGridViewLeads.CurrentCell?.OwningRow?.DataBoundItem as Lead;
                var currentType = lead?.Type ?? dataGridViewLeads.CurrentCell?.Value?.ToString() ?? string.Empty;

                combo.DropDownStyle = ComboBoxStyle.DropDownList;
                combo.Items.Clear();

                if (!string.IsNullOrEmpty(currentType))
                {
                    combo.Items.Add(currentType);
                }

                if (!string.Equals(currentType, "Contact", StringComparison.OrdinalIgnoreCase))
                {
                    combo.Items.Add("Contact");
                }

                combo.SelectedItem = currentType;
                combo.Tag = currentType;

                combo.SelectionChangeCommitted += DataGridViewLeads_TypeComboBox_SelectionChangeCommitted;
                combo.KeyPress += DataGridViewLeads_TypeComboBox_KeyPress;
            }
        }

        private void DataGridViewLeads_TypeComboBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGridViewLeads_TypeComboBox_SelectionChangeCommitted(object? sender, EventArgs e)
        {
            if (sender is ComboBox combo && combo.Tag is string currentType)
            {
                if (combo.SelectedItem is string selected && string.Equals(selected, currentType, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedItem = currentType;
                }
            }
        }
        private void DataGridViewLeads_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Formatting ||
                e.Context == DataGridViewDataErrorContexts.Display)
            {
                e.ThrowException = false;
                e.Cancel = true; // Suppress the error dialog

                if (dataGridViewLeads.Columns[e.ColumnIndex].Name == "Type")
                {
                    var lead = dataGridViewLeads.Rows[e.RowIndex].DataBoundItem as Lead;
                    var fallbackType = lead?.Type ?? dataGridViewLeads.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "Lead";
                    dataGridViewLeads.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = fallbackType;
                }
            }
        }
    }
}










