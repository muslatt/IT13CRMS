using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class AddLeadForm : Form
    {
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private ComboBox cmbType;
        private ComboBox cmbStatus;
        private ComboBox cmbAgent;
        private DateTimePicker dtpLastContacted;
        private Button btnSave;
        private Button btnCancel;

        private readonly PropertyViewModel _propertyViewModel;

        public Lead? CreatedLead { get; private set; }

        public AddLeadForm()
        {
            _propertyViewModel = new PropertyViewModel();
            InitializeComponent();
            LoadAgents();
        }

        private void InitializeComponent()
        {
            Text = "Add New Lead";
            Size = new Size(450, 460);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // Use 12pt font for the form and controls
            Font = new Font("Segoe UI", 12F);

            // Controls Initialization
            var lblFullName = new Label { Text = "Full Name:", Location = new Point(20, 25), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            txtFullName = new TextBox { Location = new Point(140, 20), Size = new Size(250, 28), Font = new Font("Segoe UI", 12F) };

            var lblEmail = new Label { Text = "Email:", Location = new Point(20, 55), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            txtEmail = new TextBox { Location = new Point(140, 50), Size = new Size(250, 28), Font = new Font("Segoe UI", 12F) };

            var lblPhone = new Label { Text = "Phone:", Location = new Point(20, 85), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            txtPhone = new TextBox { Location = new Point(140, 80), Size = new Size(250, 28), Font = new Font("Segoe UI", 12F) };

            var lblAddress = new Label { Text = "Address:", Location = new Point(20, 115), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            txtAddress = new TextBox { Location = new Point(140, 110), Size = new Size(250, 28), Font = new Font("Segoe UI", 12F) };

            var lblType = new Label { Text = "Type:", Location = new Point(20, 145), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            cmbType = new ComboBox { Location = new Point(140, 140), Size = new Size(150, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12F) };
            cmbType.Items.AddRange(new[] { "Renter", "Owner", "Buyer" });
            cmbType.SelectedIndex = 0;

            var lblStatus = new Label { Text = "Status:", Location = new Point(20, 175), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            cmbStatus = new ComboBox { Location = new Point(140, 170), Size = new Size(150, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12F) };
            cmbStatus.Items.AddRange(new[] { "New Lead", "Contacted", "Qualified", "Unqualified" });
            cmbStatus.SelectedIndex = 0;

            var lblAgent = new Label { Text = "Agent:", Location = new Point(20, 205), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            cmbAgent = new ComboBox { Location = new Point(140, 200), Size = new Size(250, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12F) };

            var lblLastContacted = new Label { Text = "Last Contacted:", Location = new Point(20, 235), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            dtpLastContacted = new DateTimePicker
            {
                Location = new Point(140, 230),
                Size = new Size(150, 28),
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true, // Allows user to specify if a date is set
                Checked = false,     // Unchecked by default, meaning null
                Font = new Font("Segoe UI", 12F)
            };

            btnSave = new Button { Text = "Save", Location = new Point(290, 370), Size = new Size(110, 35), DialogResult = DialogResult.OK, Font = new Font("Segoe UI", 12F) };
            btnCancel = new Button { Text = "Cancel", Location = new Point(180, 370), Size = new Size(110, 35), DialogResult = DialogResult.Cancel, Font = new Font("Segoe UI", 12F) };

            btnSave.Click += BtnSave_Click;

            Controls.AddRange(new Control[] {
                lblFullName, txtFullName, lblEmail, txtEmail, lblPhone, txtPhone, lblAddress, txtAddress,
                lblType, cmbType, lblStatus, cmbStatus, lblAgent, cmbAgent, lblLastContacted, dtpLastContacted,
                btnSave, btnCancel
            });

            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        private void LoadAgents()
        {
            try
            {
                cmbAgent.Items.Clear();
                cmbAgent.Items.Add("(No Agent)");

                // Load agents from properties - get unique agent names
                var agents = _propertyViewModel.Properties
                    .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Agent))
                    .Select(p => p.Agent)
                    .Distinct()
                    .OrderBy(agent => agent)
                    .ToList();

                foreach (var agent in agents)
                {
                    cmbAgent.Items.Add(agent);
                }

                cmbAgent.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading agents: {ex.Message}");
                // If loading fails, at least have "No Agent" option
                if (cmbAgent.Items.Count == 0)
                {
                    cmbAgent.Items.Add("(No Agent)");
                    cmbAgent.SelectedIndex = 0;
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                DialogResult = DialogResult.None; // Keep form open
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                DialogResult = DialogResult.None; // Keep form open
                return;
            }

            // Basic email validation
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                DialogResult = DialogResult.None; // Keep form open
                return;
            }

            // Get selected agent information
            string assignedAgent = string.Empty;
            if (cmbAgent.SelectedIndex > 0) // Skip "(No Agent)" option
            {
                assignedAgent = cmbAgent.SelectedItem?.ToString() ?? string.Empty;
            }

            CreatedLead = new Lead
            {
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Type = cmbType.SelectedItem?.ToString() ?? "Renter",
                Status = cmbStatus.SelectedItem?.ToString() ?? "New Lead",
                LastContacted = dtpLastContacted.Checked ? dtpLastContacted.Value : (DateTime?)null,
                AssignedAgent = assignedAgent, // Set the NotMapped property
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}