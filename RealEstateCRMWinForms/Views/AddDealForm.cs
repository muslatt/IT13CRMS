using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class AddDealForm : Form
    {
        private DealViewModel _viewModel;
        private List<string> _availableStatuses;
        private string _defaultStatus;
        private ComboBox cmbProperty;
        private ComboBox cmbContact;
        private Button btnSave;
        private Button btnCancel;

        public Deal? CreatedDeal { get; private set; }

        public AddDealForm(List<string>? availableStatuses = null, string? defaultStatus = null)
        {
            try
            {
                _viewModel = new DealViewModel();
                _availableStatuses = availableStatuses ?? new List<string> { "New", "Offer Made", "Negotiation", "Contract Draft" };
                _defaultStatus = defaultStatus ?? "New";
                InitializeComponent();
                LoadComboBoxData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            Text = "Add New Deal";
            Size = new Size(450, 220);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // Use 12pt font for the form and controls
            Font = new Font("Segoe UI", 12F);

            // Property
            var lblProperty = new Label
            {
                Text = "Property:",
                Location = new Point(20, 30),
                Size = new Size(100, 28),
                Font = new Font("Segoe UI", 12F)
            };

            cmbProperty = new ComboBox
            {
                Location = new Point(130, 30),
                Size = new Size(280, 28),
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Contact
            var lblContact = new Label
            {
                Text = "Contact:",
                Location = new Point(20, 70),
                Size = new Size(100, 28),
                Font = new Font("Segoe UI", 12F)
            };

            cmbContact = new ComboBox
            {
                Location = new Point(130, 70),
                Size = new Size(280, 28),
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(110, 35),
                Font = new Font("Segoe UI", 12F),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnSave = new Button
            {
                Text = "Create Deal",
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
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

            // Align buttons below the Contact dropdown
            int dropdownRight = cmbProperty.Location.X + cmbProperty.Width;
            int spacing = 10; // horizontal spacing between buttons
            int gapAboveButtons = 40; // vertical gap from Contact dropdown
            int buttonsY = cmbContact.Location.Y + cmbContact.Height + gapAboveButtons;

            // Place Save button so its right edge aligns with dropdown right edge
            btnSave.Location = new Point(dropdownRight - btnSave.Width, buttonsY);
            // Place Cancel to the left of Save
            btnCancel.Location = new Point(btnSave.Location.X - btnCancel.Width - spacing, buttonsY);

            // Optional: anchor buttons to bottom-right (remove if you want fixed under dropdown only)
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

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
                        cmbProperty.Items.Add($"{property.Title} - {property.Address}");
                    }
                }
                cmbProperty.SelectedIndex = 0;

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
                cmbContact.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
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
                    selectedProperty = _viewModel.Properties[cmbProperty.SelectedIndex - 1];
                }

                if (cmbContact.SelectedIndex > 0 && _viewModel.Contacts.Count >= cmbContact.SelectedIndex)
                {
                    selectedContact = _viewModel.Contacts[cmbContact.SelectedIndex - 1];
                }

                // Validation - at least one must be selected
                if (selectedProperty == null && selectedContact == null)
                {
                    MessageBox.Show("Please select at least a Property or Contact to create a deal.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Generate a title based on the selections
                string dealTitle = "New Deal";
                string dealDescription = "No description";
                decimal dealValue = 0;

                if (selectedProperty != null && selectedContact != null)
                {
                    dealTitle = $"{selectedProperty.Title} - {selectedContact.FullName}";
                    dealDescription = selectedProperty.Address;
                    dealValue = selectedProperty.Price;
                }
                else if (selectedProperty != null)
                {
                    dealTitle = selectedProperty.Title;
                    dealDescription = selectedProperty.Address;
                    dealValue = selectedProperty.Price;
                }
                else if (selectedContact != null)
                {
                    dealTitle = $"Deal with {selectedContact.FullName}";
                    dealDescription = $"New deal for contact: {selectedContact.FullName}";
                }

                // Create new deal with default status from default board
                CreatedDeal = new Deal
                {
                    Title = dealTitle,
                    Description = dealDescription,
                    Value = dealValue,
                    PropertyId = selectedProperty?.Id,
                    Property = selectedProperty,
                    ContactId = selectedContact?.Id,
                    Contact = selectedContact,
                    Status = _defaultStatus, // Use the default status from default board
                    Notes = "",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating deal: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
