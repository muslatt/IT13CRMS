// RealEstateCRMWinForms\Views\ContactsView.cs
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
using System.Reflection;

namespace RealEstateCRMWinForms.Views
{
    public partial class ContactsView : UserControl
    {
        private readonly ContactViewModel _viewModel;
        private BindingSource _bindingSource;
        private ContextMenuStrip _contextMenu;

        private const int PageSize = 10;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private List<Contact> _currentContacts = new();
        private bool _isDataInitialized;

        // FIX: track hovered row to avoid per-cell churn
        private int _hoveredRowIndex = -1;

        public ContactsView()
        {
            _viewModel = new ContactViewModel();
            _bindingSource = new BindingSource();

            InitializeComponent();

            EnableFlickerReduction();   // <- call on this control

            ConfigureGridAppearance();
            InitializeData();
            CreateContextMenu();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_COMPOSITED = 0x02000000;
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }

        private void EnableFlickerReduction()
        {
            // OK to call protected members on 'this'
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            // Double-buffer the DataGridView via reflection
            if (dataGridViewContacts != null)
            {
                var prop = typeof(DataGridView).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                prop?.SetValue(dataGridViewContacts, true, null);
            }
        }

        private void ConfigureGridAppearance()
        {
            if (dataGridViewContacts == null) return;

            // Disable auto-generation to control columns manually
            dataGridViewContacts.AutoGenerateColumns = false;
            dataGridViewContacts.Columns.Clear();

            // Enable inline editing only for Type dropdown
            dataGridViewContacts.EnableHeadersVisualStyles = false;
            dataGridViewContacts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewContacts.RowHeadersVisible = false;
            dataGridViewContacts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewContacts.MultiSelect = false;
            dataGridViewContacts.AllowUserToAddRows = false;
            dataGridViewContacts.AllowUserToDeleteRows = false;
            dataGridViewContacts.ReadOnly = false;
            dataGridViewContacts.EditMode = DataGridViewEditMode.EditOnEnter;

            // Add border to the table and header
            dataGridViewContacts.BorderStyle = BorderStyle.FixedSingle;
            dataGridViewContacts.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewContacts.GridColor = Color.FromArgb(224, 224, 224);

            // Add border below header
            dataGridViewContacts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewContacts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridViewContacts.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            dataGridViewContacts.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 249, 250);

            // Set font size to 12 and styling using UIStyles
            dataGridViewContacts.ColumnHeadersDefaultCellStyle.Font = UIStyles.BoldFont;
            dataGridViewContacts.DefaultCellStyle.Font = UIStyles.DefaultFont;
            dataGridViewContacts.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            dataGridViewContacts.DefaultCellStyle.BackColor = Color.White;
            dataGridViewContacts.DefaultCellStyle.SelectionBackColor = UIStyles.SelectedRowColor;
            dataGridViewContacts.DefaultCellStyle.SelectionForeColor = Color.FromArgb(33, 37, 41);
            dataGridViewContacts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dataGridViewContacts.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            // Increase row height for avatars and larger font
            dataGridViewContacts.RowTemplate.Height = Math.Max(UIStyles.AvatarSize + 8, 44);

            // Wire up events
            dataGridViewContacts.CellContentClick += DataGridViewContacts_CellContentClick;
            dataGridViewContacts.CellFormatting += DataGridViewContacts_CellFormatting;
            dataGridViewContacts.MouseClick += DataGridViewContacts_MouseClick;

            // FIX: replace per-cell hover repaint storms
            dataGridViewContacts.CellMouseEnter += DataGridViewContacts_CellMouseEnter_Optimized;
            dataGridViewContacts.MouseLeave += DataGridViewContacts_MouseLeave_Reset;

            dataGridViewContacts.CellValueChanged += DataGridViewContacts_CellValueChanged;
            dataGridViewContacts.CurrentCellDirtyStateChanged += DataGridViewContacts_CurrentCellDirtyStateChanged;
            dataGridViewContacts.CellBeginEdit += DataGridViewContacts_CellBeginEdit;
            dataGridViewContacts.CellDoubleClick += DataGridViewContacts_CellDoubleClick;

            // FIX: ensure DGV double buffer on constructed instance
            var prop = typeof(DataGridView).GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            prop?.SetValue(dataGridViewContacts, true, null);
        }

        private void CreateContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            var editMenuItem = new ToolStripMenuItem("Edit Contact");
            editMenuItem.Click += EditMenuItem_Click;

            var deleteMenuItem = new ToolStripMenuItem("Delete Contact");
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

                _bindingSource.DataSource = _viewModel.Contacts;
                dataGridViewContacts.DataSource = _bindingSource;
                if (sortComboBox != null)
                    sortComboBox.SelectedIndex = 0;

                ApplyFilterAndSort(true);
                _isDataInitialized = true;
                UpdatePaginationControls();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing ContactsView data: {ex.Message}");
                InitializeBasicColumns();
            }
        }

        private void ShowDatabaseConnectionInfo()
        {
            var infoLabel = new Label
            {
                Text = "Database connection not available. Some features may be limited.",
                ForeColor = Color.Orange,
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10)
            };

            this.Controls.Add(infoLabel);
            infoLabel.BringToFront();

            CreateGridColumns();
            _bindingSource.DataSource = _viewModel.Contacts;
            dataGridViewContacts.DataSource = _bindingSource;
        }

        /// <summary>
        /// Fallback method to initialize basic columns if custom columns fail
        /// </summary>
        private void InitializeBasicColumns()
        {
            try
            {
                dataGridViewContacts.Columns.Clear();
                dataGridViewContacts.AutoGenerateColumns = true;
                _bindingSource.DataSource = _viewModel.Contacts;
                dataGridViewContacts.DataSource = _bindingSource;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in fallback initialization: {ex.Message}");
                var errorLabel = new Label
                {
                    Text = "Unable to load contacts data. Please check your configuration.",
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
            dataGridViewContacts.Columns.Add(nameColumn);

            var typeColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Name = "Type",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False },
                DropDownWidth = 120,
                MaxDropDownItems = 5,
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            typeColumn.Items.AddRange(new string[] { "Contact", "Lead", "Buyer", "Owner", "Renter" });
            dataGridViewContacts.Columns.Add(typeColumn);

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
            dataGridViewContacts.Columns.Add(agentColumn);

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Name = "Email",
                Width = 220,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Phone",
                HeaderText = "Phone",
                Name = "Phone",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Occupation",
                HeaderText = "Occupation",
                Name = "Occupation",
                Width = 160,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Salary",
                HeaderText = "Salary",
                Name = "Salary",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.False,
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CreatedAt",
                HeaderText = "Date Added",
                Name = "CreatedAt",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });
        }

        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void DataGridViewContacts_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.RowIndex >= dataGridViewContacts.Rows.Count) return;
                var contact = dataGridViewContacts.Rows[e.RowIndex].DataBoundItem as Contact;
                if (contact == null) return;

                using var dlg = new ContactDetailsDialog(contact);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _viewModel.LoadContacts();
                    ApplyFilterAndSort(resetPage: false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening contact: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            if (!_isDataInitialized) return;
            ApplyFilterAndSort(true);
        }

        private void BtnAddContact_Click(object? sender, EventArgs e)
        {
            var addContactForm = new AddContactForm();
            if (addContactForm.ShowDialog() == DialogResult.OK && addContactForm.CreatedContact != null)
            {
                if (_viewModel.AddContact(addContactForm.CreatedContact))
                {
                    RefreshContactsView();
                }
            }
        }

        private void RefreshContactsView()
        {
            try
            {
                int? selectedId = null;
                if (dataGridViewContacts.SelectedRows.Count > 0 && dataGridViewContacts.SelectedRows[0].DataBoundItem is Contact selectedContact)
                {
                    selectedId = selectedContact.Id;
                }

                _viewModel.LoadContacts();

                if (searchBox != null)
                    searchBox.Text = string.Empty;

                _bindingSource.DataSource = _viewModel.Contacts;
                ApplySort();

                if (selectedId.HasValue)
                {
                    foreach (DataGridViewRow row in dataGridViewContacts.Rows)
                    {
                        if (row.DataBoundItem is Contact contact && contact.Id == selectedId)
                        {
                            row.Selected = true;
                            dataGridViewContacts.FirstDisplayedScrollingRowIndex = row.Index;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing contacts: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridViewContacts_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = dataGridViewContacts.HitTest(e.X, e.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell && hitTestInfo.RowIndex >= 0)
                {
                    dataGridViewContacts.ClearSelection();
                    dataGridViewContacts.Rows[hitTestInfo.RowIndex].Selected = true;
                    _contextMenu.Show(dataGridViewContacts, e.Location);
                }
            }
        }

        private void EditMenuItem_Click(object? sender, EventArgs e)
        {
            if (dataGridViewContacts.SelectedRows.Count > 0 && dataGridViewContacts.SelectedRows[0].DataBoundItem is Contact contact)
            {
                var editForm = new EditContactForm(contact);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    if (_viewModel.UpdateContact(contact))
                    {
                        RefreshContactsView();
                    }
                }
            }
        }

        private void DeleteMenuItem_Click(object? sender, EventArgs e)
        {
            if (dataGridViewContacts.SelectedRows.Count > 0 && dataGridViewContacts.SelectedRows[0].DataBoundItem is Contact contact)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the contact '{contact.FullName}'?", "Delete Contact",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (_viewModel.DeleteContact(contact))
                    {
                        RefreshContactsView();
                    }
                }
            }
        }

        private void DataGridViewContacts_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = dataGridViewContacts.Columns[e.ColumnIndex].Name;

            if (columnName == "Phone" && e.Value != null)
            {
                e.Value = FormatPhilippinePhoneNumber(e.Value.ToString());
                e.FormattingApplied = true;
            }
            else if (columnName == "CreatedAt" && e.Value is DateTime dt)
            {
                e.Value = dt.ToString("MMM dd, yyyy");
                e.FormattingApplied = true;
            }
            else if (columnName == "Salary" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out var sal))
                {
                    e.Value = $"₱{sal:N0}";
                    e.FormattingApplied = true;
                }
            }
        }

        private void DataGridViewContacts_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Handle any button clicks if needed
        }

        // NEW: Only allow editing of the Type column and offer conversion to Lead
        private void DataGridViewContacts_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dataGridViewContacts.Columns[e.ColumnIndex].Name != "Type")
            {
                e.Cancel = true;
                return;
            }

            if (dataGridViewContacts.Rows[e.RowIndex].DataBoundItem is Contact contact &&
                dataGridViewContacts.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell typeCell)
            {
                typeCell.Items.Clear();
                typeCell.Items.Add(contact.Type);
                if (!string.Equals(contact.Type, "Lead", StringComparison.OrdinalIgnoreCase))
                {
                    typeCell.Items.Add("Lead");
                }
            }
        }

        private void DataGridViewContacts_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dataGridViewContacts.IsCurrentCellDirty && dataGridViewContacts.CurrentCell?.OwningColumn?.Name == "Type")
            {
                dataGridViewContacts.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private async void DataGridViewContacts_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dataGridViewContacts.Columns[e.ColumnIndex].Name != "Type") return;

            var contact = dataGridViewContacts.Rows[e.RowIndex].DataBoundItem as Contact;
            if (contact == null) return;

            var newValue = dataGridViewContacts.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
            var originalType = contact.Type;

            if (string.Equals(newValue, "Lead", StringComparison.OrdinalIgnoreCase))
            {
                await HandleContactToLeadConversion(contact, e.RowIndex, originalType);
            }
            else
            {
                if (e.RowIndex < dataGridViewContacts.Rows.Count && dataGridViewContacts.Rows[e.RowIndex].DataBoundItem == contact)
                {
                    dataGridViewContacts.Rows[e.RowIndex].Cells["Type"].Value = originalType;
                }
            }
        }

        private async System.Threading.Tasks.Task HandleContactToLeadConversion(Contact contact, int rowIndex, string originalType)
        {
            var contactName = contact.FullName;
            var confirmation = MessageBox.Show(
                "Move '" + contactName + "' to Leads?\n\n" +
                "This will remove them from the Contacts list.",
                "Move Contact to Leads",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirmation != DialogResult.Yes)
            {
                if (rowIndex < dataGridViewContacts.Rows.Count && dataGridViewContacts.Rows[rowIndex].DataBoundItem == contact)
                {
                    dataGridViewContacts.Rows[rowIndex].Cells["Type"].Value = originalType;
                }
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                bool success = await _viewModel.MoveContactToLeadAsync(contact);

                if (success)
                {
                    ApplyFilterAndSort(true);
                    MessageBox.Show(contactName + " has been moved to the Leads list.", "Contact Updated",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (rowIndex < dataGridViewContacts.Rows.Count && dataGridViewContacts.Rows[rowIndex].DataBoundItem == contact)
                    {
                        dataGridViewContacts.Rows[rowIndex].Cells["Type"].Value = originalType;
                    }
                    MessageBox.Show("Failed to move the contact to Leads. Please try again.", "Conversion Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error during conversion: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                if (rowIndex < dataGridViewContacts.Rows.Count && dataGridViewContacts.Rows[rowIndex].DataBoundItem == contact)
                {
                    dataGridViewContacts.Rows[rowIndex].Cells["Type"].Value = originalType;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // FIX: optimized hover handling
        private void DataGridViewContacts_CellMouseEnter_Optimized(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (_hoveredRowIndex == e.RowIndex) return; // no change, avoid repaint

            // reset previous row bg
            if (_hoveredRowIndex >= 0 && _hoveredRowIndex < dataGridViewContacts.Rows.Count)
            {
                var prev = dataGridViewContacts.Rows[_hoveredRowIndex];
                if (!prev.Selected)
                {
                    prev.DefaultCellStyle.BackColor =
                        (_hoveredRowIndex % 2 == 1)
                            ? dataGridViewContacts.AlternatingRowsDefaultCellStyle.BackColor
                            : Color.White;
                }
            }

            // set new hover bg
            _hoveredRowIndex = e.RowIndex;
            var row = dataGridViewContacts.Rows[_hoveredRowIndex];
            if (!row.Selected)
            {
                row.DefaultCellStyle.BackColor = UIStyles.RowHoverColor;
            }
        }

        private void DataGridViewContacts_MouseLeave_Reset(object? sender, EventArgs e)
        {
            if (_hoveredRowIndex >= 0 && _hoveredRowIndex < dataGridViewContacts.Rows.Count)
            {
                var row = dataGridViewContacts.Rows[_hoveredRowIndex];
                if (!row.Selected)
                {
                    row.DefaultCellStyle.BackColor =
                        (_hoveredRowIndex % 2 == 1)
                            ? dataGridViewContacts.AlternatingRowsDefaultCellStyle.BackColor
                            : Color.White;
                }
            }
            _hoveredRowIndex = -1;
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

            return phoneNumber;
        }

        // Pagination + filtering/sorting
        private void ApplyFilterAndSort(bool resetPage)
        {
            var query = searchBox?.Text?.Trim() ?? string.Empty;
            IEnumerable<Contact> data = _viewModel.Contacts;

            if (!string.IsNullOrWhiteSpace(query))
            {
                data = data.Where(c =>
                    (c.FullName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Email?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Phone?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Occupation?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Type?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            var filtered = data.ToList();
            var sorted = SortContacts(filtered, sortComboBox?.SelectedItem as string);

            SetCurrentContacts(sorted, resetPage);
        }

        private static List<Contact> SortContacts(List<Contact> contacts, string? sortOption)
        {
            return (sortOption ?? "Newest to Oldest") switch
            {
                "Oldest to Newest" => contacts.OrderBy(c => c.CreatedAt).ToList(),
                "Name A-Z" => contacts.OrderBy(c => c.FullName ?? string.Empty).ToList(),
                "Name Z-A" => contacts.OrderByDescending(c => c.FullName ?? string.Empty).ToList(),
                _ => contacts.OrderByDescending(c => c.CreatedAt).ToList()
            };
        }

        private void SetCurrentContacts(IEnumerable<Contact> contacts, bool resetPage)
        {
            _currentContacts = contacts?.ToList() ?? new List<Contact>();

            if (_currentContacts.Count == 0)
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
            var wasVisible = dataGridViewContacts.Visible;
            dataGridViewContacts.Visible = false;
            dataGridViewContacts.SuspendLayout();
            try
            {
                if (_currentContacts.Count == 0)
                {
                    _totalPages = 1;
                    _bindingSource.DataSource = new BindingList<Contact>();
                }
                else
                {
                    _totalPages = (int)Math.Ceiling(_currentContacts.Count / (double)PageSize);
                    if (_currentPage > _totalPages)
                    {
                        _currentPage = _totalPages;
                    }

                    var pageContacts = _currentContacts
                        .Skip((_currentPage - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

                    _bindingSource.DataSource = new BindingList<Contact>(pageContacts);

                    // Reset scroll to top if rows exist
                    try
                    {
                        if (dataGridViewContacts.Rows.Count > 0)
                            dataGridViewContacts.FirstDisplayedScrollingRowIndex = 0;
                    }
                    catch { /* ignore */ }
                }
            }
            finally
            {
                dataGridViewContacts.ResumeLayout(true);
                dataGridViewContacts.Visible = wasVisible;
            }

            UpdatePaginationControls();
        }

        private void UpdatePaginationControls()
        {
            var totalItems = _currentContacts.Count;
            if (totalItems == 0)
            {
                lblContactPageInfo.Text = "No contacts to display";
            }
            else
            {
                var start = ((_currentPage - 1) * PageSize) + 1;
                var end = Math.Min(start + PageSize - 1, totalItems);
                lblContactPageInfo.Text = $"Showing {start}–{end} of {totalItems}";
            }

            btnPrevContactPage.Enabled = _currentPage > 1;
            btnNextContactPage.Enabled = _currentPage < _totalPages;
        }

        private void BtnPrevContactPage_Click(object? sender, EventArgs e)
        {
            if (_currentPage <= 1) return;
            _currentPage--;
            ApplyPagination();
        }

        private void BtnNextContactPage_Click(object? sender, EventArgs e)
        {
            if (_currentPage >= _totalPages) return;
            _currentPage++;
            ApplyPagination();
        }
    }
}
