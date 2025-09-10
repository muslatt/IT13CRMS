using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class EditDealForm : Form
    {
        private readonly DealViewModel _viewModel;
        private Deal _deal;
        private ComboBox cmbProperty;
        private ComboBox cmbContact;
        private Button btnSave;
        private Button btnCancel;

        public EditDealForm(Deal deal)
        {
            _viewModel = new DealViewModel();
            _deal = deal;
            InitializeComponent();
            LoadComboBoxData();
            PopulateFields();
        }

        private void InitializeComponent()
        {
            Text = "Edit Deal";
            Size = new Size(450, 200);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // Property
            var lblProperty = new Label
            {
                Text = "Property:",
                Location = new Point(20, 30),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 10F)
            };

            cmbProperty = new ComboBox
            {
                Location = new Point(130, 30),
                Size = new Size(280, 23),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Contact
            var lblContact = new Label
            {
                Text = "Contact:",
                Location = new Point(20, 70),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 10F)
            };

            cmbContact = new ComboBox
            {
                Location = new Point(130, 70),
                Size = new Size(280, 23),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(250, 120),
                Size = new Size(80, 35),
                Font = new Font("Segoe UI", 9F),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnSave = new Button
            {
                Text = "Save Changes",
                Location = new Point(340, 120),
                Size = new Size(90, 35),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // Add all controls
            Controls.AddRange(new Control[] {
                lblProperty, cmbProperty,
                lblContact, cmbContact,
                btnCancel, btnSave
            });

            CancelButton = btnCancel;
            AcceptButton = btnSave;
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Load properties
                cmbProperty.Items.Clear();
                cmbProperty.Items.Add("(No Property)");
                
                if (_viewModel.Properties != null)
                {
                    foreach (var property in _viewModel.Properties)
                    {
                        // Check if this property is already used in another active deal
                        bool isPropertyInUse = _viewModel.Deals.Any(d => 
                            d.PropertyId == property.Id && 
                            d.IsActive && 
                            d.Id != _deal.Id); // Exclude current deal from check

                        if (isPropertyInUse)
                        {
                            // Add but disable the property if it's already in use
                            cmbProperty.Items.Add($"{property.Title} - {property.Address} (Already in use)");
                        }
                        else
                        {
                            cmbProperty.Items.Add($"{property.Title} - {property.Address}");
                        }
                    }
                }

                // Load contacts
                cmbContact.Items.Clear();
                cmbContact.Items.Add("(No Contact)");
                
                if (_viewModel.Contacts != null)
                {
                    foreach (var contact in _viewModel.Contacts)
                    {
                        cmbContact.Items.Add($"{contact.FullName} - {contact.Email}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFields()
        {
            try
            {
                // Set property selection
                if (_deal.PropertyId.HasValue)
                {
                    var property = _viewModel.Properties.FirstOrDefault(p => p.Id == _deal.PropertyId.Value);
                    if (property != null)
                    {
                        // Find the property in the combo box
                        for (int i = 1; i < cmbProperty.Items.Count; i++) // Start from 1 to skip "(No Property)"
                        {
                            string item = cmbProperty.Items[i].ToString();
                            if (item.StartsWith($"{property.Title} - {property.Address}"))
                            {
                                cmbProperty.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    cmbProperty.SelectedIndex = 0; // "(No Property)"
                }

                // Set contact selection
                if (_deal.ContactId.HasValue)
                {
                    var contact = _viewModel.Contacts.FirstOrDefault(c => c.Id == _deal.ContactId.Value);
                    if (contact != null)
                    {
                        // Find the contact in the combo box
                        for (int i = 1; i < cmbContact.Items.Count; i++) // Start from 1 to skip "(No Contact)"
                        {
                            string item = cmbContact.Items[i].ToString();
                            if (item.StartsWith($"{contact.FullName} - {contact.Email}"))
                            {
                                cmbContact.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    cmbContact.SelectedIndex = 0; // "(No Contact)"
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating fields: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // Get selected property and contact
                Property? selectedProperty = null;
                Contact? selectedContact = null;

                if (cmbProperty.SelectedIndex > 0 && _viewModel.Properties.Count >= cmbProperty.SelectedIndex)
                {
                    var propertyIndex = cmbProperty.SelectedIndex - 1;
                    selectedProperty = _viewModel.Properties[propertyIndex];
                    
                    // Check if trying to select a property that's already in use by another deal
                    string selectedItem = cmbProperty.Items[cmbProperty.SelectedIndex].ToString();
                    if (selectedItem.Contains("(Already in use)"))
                    {
                        MessageBox.Show("This property is already being used in another active deal. Please select a different property.", 
                            "Property Already in Use", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (cmbContact.SelectedIndex > 0 && _viewModel.Contacts.Count >= cmbContact.SelectedIndex)
                {
                    selectedContact = _viewModel.Contacts[cmbContact.SelectedIndex - 1];
                }

                // Validation - at least one must be selected
                if (selectedProperty == null && selectedContact == null)
                {
                    MessageBox.Show("Please select at least a Property or Contact for this deal.", 
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update deal properties
                _deal.PropertyId = selectedProperty?.Id;
                _deal.Property = selectedProperty;
                _deal.ContactId = selectedContact?.Id;
                _deal.Contact = selectedContact;

                // Update title, description, and value based on selections (like AddDealForm does)
                if (selectedProperty != null && selectedContact != null)
                {
                    _deal.Title = $"{selectedProperty.Title} - {selectedContact.FullName}";
                    _deal.Description = selectedProperty.Address;
                    _deal.Value = selectedProperty.Price;
                }
                else if (selectedProperty != null)
                {
                    _deal.Title = selectedProperty.Title;
                    _deal.Description = selectedProperty.Address;
                    _deal.Value = selectedProperty.Price;
                }
                else if (selectedContact != null)
                {
                    _deal.Title = $"Deal with {selectedContact.FullName}";
                    _deal.Description = $"Deal for contact: {selectedContact.FullName}";
                    // Keep existing value if no property selected
                }

                // Update in database
                if (_viewModel.UpdateDeal(_deal))
                {
                    MessageBox.Show("Deal updated successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to update deal. Please try again.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating deal: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}