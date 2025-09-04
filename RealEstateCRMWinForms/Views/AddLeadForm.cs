using RealEstateCRMWinForms.Models;
using System.Drawing;
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
        private ComboBox cmbSource;
        private Button btnSave;
        private Button btnCancel;

        public AddLeadForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Add New Lead";
            Size = new Size(500, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // Full Name
            var lblFullName = new Label
            {
                Text = "Full Name:",
                Location = new Point(20, 20),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            txtFullName = new TextBox
            {
                Location = new Point(130, 20),
                Size = new Size(320, 23),
                Font = new Font("Segoe UI", 9F)
            };

            // Email
            var lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(20, 55),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            txtEmail = new TextBox
            {
                Location = new Point(130, 55),
                Size = new Size(320, 23),
                Font = new Font("Segoe UI", 9F)
            };

            // Phone
            var lblPhone = new Label
            {
                Text = "Phone:",
                Location = new Point(20, 90),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            txtPhone = new TextBox
            {
                Location = new Point(130, 90),
                Size = new Size(200, 23),
                Font = new Font("Segoe UI", 9F),
                PlaceholderText = "(123) 456-7890"
            };

            // Address
            var lblAddress = new Label
            {
                Text = "Address:",
                Location = new Point(20, 125),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            txtAddress = new TextBox
            {
                Location = new Point(130, 125),
                Size = new Size(320, 23),
                Font = new Font("Segoe UI", 9F)
            };

            // Type
            var lblType = new Label
            {
                Text = "Type:",
                Location = new Point(20, 160),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            cmbType = new ComboBox
            {
                Location = new Point(130, 160),
                Size = new Size(120, 23),
                Font = new Font("Segoe UI", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbType.Items.AddRange(new[] { "Renter", "Owner", "Buyer" });
            cmbType.SelectedIndex = 0; // Default to Renter

            // Status
            var lblStatus = new Label
            {
                Text = "Status:",
                Location = new Point(20, 195),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            cmbStatus = new ComboBox
            {
                Location = new Point(130, 195),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new[] { "New Lead", "Contacted", "Qualified", "Unqualified" });
            cmbStatus.SelectedIndex = 0; // Default to New Lead

            // Source
            var lblSource = new Label
            {
                Text = "Source:",
                Location = new Point(20, 230),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            cmbSource = new ComboBox
            {
                Location = new Point(130, 230),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSource.Items.AddRange(new[] { "Website", "Facebook", "Instagram", "Referral", "Walk-in", "Phone Call", "Email", "Other" });
            cmbSource.SelectedIndex = 0; // Default to Website

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(280, 370),
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
                Text = "Save Lead",
                Location = new Point(370, 370),
                Size = new Size(80, 35),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // Add all controls
            Controls.AddRange(new Control[] {
                lblFullName, txtFullName,
                lblEmail, txtEmail,
                lblPhone, txtPhone,
                lblAddress, txtAddress,
                lblType, cmbType,
                lblStatus, cmbStatus,
                lblSource, cmbSource,
                btnCancel, btnSave
            });

            CancelButton = btnCancel;
            AcceptButton = btnSave;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter the lead's full name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter the lead's email address.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Basic email validation
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Create new lead
            var newLead = new Lead
            {
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Type = cmbType.SelectedItem?.ToString() ?? "Renter",
                Status = cmbStatus.SelectedItem?.ToString() ?? "New Lead",
                Source = cmbSource.SelectedItem?.ToString() ?? "Website",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Set the created lead as a property so the calling form can access it
            CreatedLead = newLead;
            DialogResult = DialogResult.OK;
            Close();
        }

        public Lead? CreatedLead { get; private set; }

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