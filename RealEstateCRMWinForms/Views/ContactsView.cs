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

namespace RealEstateCRMWinForms.Views
{
    public partial class ContactsView : UserControl
    {
        private readonly ContactViewModel _viewModel;
        private BindingSource _bindingSource;
        private ContextMenuStrip _contextMenu;

        public ContactsView()
        {
            _viewModel = new ContactViewModel();
            _bindingSource = new BindingSource();
            InitializeComponent();
            ConfigureGridAppearance();
            InitializeData();
            CreateContextMenu();
        }

        private void ConfigureGridAppearance()
        {
            if (dataGridViewContacts == null) return;

            // Disable auto-generation to control columns manually
            dataGridViewContacts.AutoGenerateColumns = false;
            dataGridViewContacts.Columns.Clear();

            // Make table read-only (no inline editing)
            dataGridViewContacts.EnableHeadersVisualStyles = false;
            dataGridViewContacts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewContacts.RowHeadersVisible = false;
            dataGridViewContacts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewContacts.MultiSelect = false;
            dataGridViewContacts.AllowUserToAddRows = false;
            dataGridViewContacts.AllowUserToDeleteRows = false;
            dataGridViewContacts.ReadOnly = true;
            dataGridViewContacts.EditMode = DataGridViewEditMode.EditProgrammatically;

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
            dataGridViewContacts.CellMouseEnter += DataGridViewContacts_CellMouseEnter;
            dataGridViewContacts.CellMouseLeave += DataGridViewContacts_CellMouseLeave;
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
                // Create columns in the desired order
                CreateGridColumns();

                _bindingSource.DataSource = _viewModel.Contacts;
                dataGridViewContacts.DataSource = _bindingSource;
                if (sortComboBox != null)
                    sortComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing ContactsView data: {ex.Message}");
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
                dataGridViewContacts.Columns.Clear();
                dataGridViewContacts.AutoGenerateColumns = true;
                _bindingSource.DataSource = _viewModel.Contacts;
                dataGridViewContacts.DataSource = _bindingSource;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in fallback initialization: {ex.Message}");
                MessageBox.Show("Error loading contacts data. Please restart the application.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateGridColumns()
        {
            // 1. Contact Name with Avatar (using custom ImageText column)
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

            // 2. Email
            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Email", 
                HeaderText = "Email", 
                Name = "Email",
                Width = 220,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            // 3. Phone
            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Phone", 
                HeaderText = "Phone", 
                Name = "Phone",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            // 4. Type
            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Type", 
                HeaderText = "Type", 
                Name = "Type",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            // 5. Date Added
            dataGridViewContacts.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "CreatedAt", 
                HeaderText = "Date Added", 
                Name = "CreatedAt",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False }
            });

            // 6. Agent with Avatar (using custom ImageText column)
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
                _bindingSource.DataSource = _viewModel.Contacts;
            }
            else
            {
                var filtered = _viewModel.Contacts.Where(c =>
                    (c.FullName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Email?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Phone?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Type?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

                _bindingSource.DataSource = new BindingList<Contact>(filtered);
            }
        }

        private void ApplySort()
        {
            var selectedSort = sortComboBox?.SelectedItem as string;
            List<Contact> sortedContacts;

            var currentData = (_bindingSource.DataSource as BindingList<Contact>)?.ToList() ?? _viewModel.Contacts.ToList();

            sortedContacts = selectedSort switch
            {
                "Oldest to Newest" => currentData.OrderBy(c => c.CreatedAt).ToList(),
                "Name A-Z" => currentData.OrderBy(c => c.FullName).ToList(),
                "Name Z-A" => currentData.OrderByDescending(c => c.FullName).ToList(),
                _ => currentData.OrderByDescending(c => c.CreatedAt).ToList()
            };

            _bindingSource.DataSource = new BindingList<Contact>(sortedContacts);
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
        }

        private void DataGridViewContacts_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Handle any button clicks if needed
        }

        private void DataGridViewContacts_CellMouseEnter(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var row = dataGridViewContacts.Rows[e.RowIndex];
                if (!row.Selected)
                {
                    row.DefaultCellStyle.BackColor = UIStyles.RowHoverColor;
                }
            }
        }

        private void DataGridViewContacts_CellMouseLeave(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var row = dataGridViewContacts.Rows[e.RowIndex];
                if (!row.Selected)
                {
                    // Reset to alternating row color or default
                    row.DefaultCellStyle.BackColor = (e.RowIndex % 2 == 1) 
                        ? dataGridViewContacts.AlternatingRowsDefaultCellStyle.BackColor 
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
    }
}