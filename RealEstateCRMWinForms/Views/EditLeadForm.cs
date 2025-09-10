using RealEstateCRMWinForms.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class EditLeadForm : Form
    {
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private ComboBox cmbType;
        private ComboBox cmbStatus;
        private ComboBox cmbSource;
        private DateTimePicker dtpLastContacted;
        private Button btnSave;
        private Button btnCancel;
        private Lead _lead;

        public EditLeadForm(Lead lead)
        {
            _lead = lead;
            InitializeComponent();
            LoadLeadData();
        }

        private void InitializeComponent()
        {
            Text = "Edit Lead";
            Size = new Size(500, 480);
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
                PlaceholderText = "0XXX XXX XXXX"
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

            // Last Contacted
            var lblLastContacted = new Label
            {
                Text = "Last Contacted:",
                Location = new Point(20, 265),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            dtpLastContacted = new DateTimePicker
            {
                Location = new Point(130, 265),
                Size = new Size(200, 23),
                Font = new Font("Segoe UI", 9F),
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true, // Allow null values by checking/unchecking
                Checked = false
            };

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(280, 390),
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
                Location = new Point(370, 390),
                Size = new Size(100, 35),
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
                lblLastContacted, dtpLastContacted,
                btnCancel, btnSave
            });

            CancelButton = btnCancel;
            AcceptButton = btnSave;
        }

        private void LoadLeadData()
        {
            txtFullName.Text = _lead.FullName;
            txtEmail.Text = _lead.Email;
            txtPhone.Text = _lead.Phone;
            txtAddress.Text = _lead.Address;
            
            cmbType.SelectedItem = _lead.Type;
            cmbStatus.SelectedItem = _lead.Status;
            cmbSource.SelectedItem = _lead.Source;

            if (_lead.LastContacted.HasValue)
            {
                dtpLastContacted.Checked = true;
                dtpLastContacted.Value = _lead.LastContacted.Value;
            }
            else
            {
                dtpLastContacted.Checked = false;
                dtpLastContacted.Value = DateTime.Now;
            }
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

            // Update lead object
            _lead.FullName = txtFullName.Text.Trim();
            _lead.Email = txtEmail.Text.Trim();
            _lead.Phone = txtPhone.Text.Trim();
            _lead.Address = txtAddress.Text.Trim();
            _lead.Type = cmbType.SelectedItem?.ToString() ?? "Renter";
            _lead.Status = cmbStatus.SelectedItem?.ToString() ?? "New Lead";
            _lead.Source = cmbSource.SelectedItem?.ToString() ?? "Website";
            _lead.LastContacted = dtpLastContacted.Checked ? dtpLastContacted.Value : null;

            DialogResult = DialogResult.OK;
            Close();
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