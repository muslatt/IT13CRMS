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
        private ComboBox cmbAgent;
        private Button btnSave;
        private Button btnCancel;

        // Keep filtered lists to map ComboBox selection back to entities
        private List<Property> _filteredProperties = new();
        private List<Contact> _filteredContacts = new();

        public Deal? CreatedDeal { get; private set; }

        public AddDealForm(List<string>? availableStatuses = null, string? defaultStatus = null)
        {
            try
            {
                _viewModel = new DealViewModel();
                _availableStatuses = availableStatuses ?? new List<string> { BoardViewModel.NewBoardName, "Offer Made", "Negotiation", "Contract Draft" };
                _defaultStatus = string.IsNullOrWhiteSpace(defaultStatus) ? BoardViewModel.NewBoardName : defaultStatus;
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
            Size = new Size(450, 260);
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

            // Agent
            var lblAgent = new Label
            {
                Text = "Agent:",
                Location = new Point(20, 110),
                Size = new Size(100, 28),
                Font = new Font("Segoe UI", 12F)
            };

            cmbAgent = new ComboBox
            {
                Location = new Point(130, 110),
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
                lblAgent, cmbAgent,
                btnCancel, btnSave
            });

            // Align buttons below the Contact dropdown
            int dropdownRight = cmbProperty.Location.X + cmbProperty.Width;
            int spacing = 10; // horizontal spacing between buttons
            int gapAboveButtons = 40; // vertical gap from Contact dropdown
            int buttonsY = cmbAgent.Location.Y + cmbAgent.Height + gapAboveButtons;

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
                // Build a set of property IDs that are already used by active deals
                var usedPropertyIds = new HashSet<int>(
                    _viewModel.Deals
                        .Where(d => d.IsActive && d.PropertyId.HasValue)
                        .Select(d => d.PropertyId!.Value)
                );

                // Build a set of contact IDs that are already used by active deals
                var usedContactIds = new HashSet<int>(
                    _viewModel.Deals
                        .Where(d => d.IsActive && d.ContactId.HasValue)
                        .Select(d => d.ContactId!.Value)
                );

                // Load properties excluding those already used in active deals
                cmbProperty.Items.Clear();
                cmbProperty.Items.Add("(No Property)");
                _filteredProperties = _viewModel.Properties
                    .Where(p => !usedPropertyIds.Contains(p.Id))
                    .ToList();
                foreach (var property in _filteredProperties)
                {
                    cmbProperty.Items.Add($"{property.Title} - {property.Address}");
                }
                cmbProperty.SelectedIndex = 0;

                // Load contacts excluding those already used in active deals
                cmbContact.Items.Clear();
                cmbContact.Items.Add("(No Contact)");
                _filteredContacts = _viewModel.Contacts
                    .Where(c => !usedContactIds.Contains(c.Id))
                    .ToList();
                foreach (var contact in _filteredContacts)
                {
                    cmbContact.Items.Add($"{contact.FullName} - {contact.Email}");
                }
                cmbContact.SelectedIndex = 0;

                // Load agents
                cmbAgent.Items.Clear();
                cmbAgent.Items.Add("(No Agent)");
                foreach (var name in Services.AgentDirectory.GetAgentDisplayNames())
                {
                    cmbAgent.Items.Add(name);
                }
                cmbAgent.SelectedIndex = 0;
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

                if (cmbProperty.SelectedIndex > 0 && _filteredProperties.Count >= cmbProperty.SelectedIndex)
                {
                    selectedProperty = _filteredProperties[cmbProperty.SelectedIndex - 1];
                }

                if (cmbContact.SelectedIndex > 0 && _filteredContacts.Count >= cmbContact.SelectedIndex)
                {
                    selectedContact = _filteredContacts[cmbContact.SelectedIndex - 1];
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

                // If an agent is selected, mark this deal as a pending assignment to that agent
                if (cmbAgent.SelectedIndex > 0)
                {
                    var agentName = cmbAgent.SelectedItem?.ToString() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(agentName))
                    {
                        var marker = $"[ASSIGN:{agentName}]";
                        CreatedDeal.Notes = string.IsNullOrWhiteSpace(CreatedDeal.Notes) ? marker : ($"{CreatedDeal.Notes} {marker}");
                    }
                }

                // Set CreatedBy to current user (Broker) if available
                var user = RealEstateCRMWinForms.Services.UserSession.Instance.CurrentUser;
                if (user != null)
                {
                    CreatedDeal.CreatedBy = ($"{user.FirstName} {user.LastName}").Trim();
                }

                MessageBox.Show(
                    $"Deal '{dealTitle}' has been successfully created!",
                    "Deal Created",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

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
