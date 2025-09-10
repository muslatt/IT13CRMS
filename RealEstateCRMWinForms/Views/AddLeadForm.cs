using RealEstateCRMWinForms.Models;
using System;
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
        private DateTimePicker dtpLastContacted;
        private Button btnSave;
        private Button btnCancel;

        public Lead? CreatedLead { get; private set; }

        public AddLeadForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Add New Lead";
            Size = new Size(450, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // Controls Initialization
            var lblFullName = new Label { Text = "Full Name:", Location = new Point(20, 25), AutoSize = true };
            txtFullName = new TextBox { Location = new Point(140, 20), Size = new Size(250, 23) };

            var lblEmail = new Label { Text = "Email:", Location = new Point(20, 55), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(140, 50), Size = new Size(250, 23) };

            var lblPhone = new Label { Text = "Phone:", Location = new Point(20, 85), AutoSize = true };
            txtPhone = new TextBox { Location = new Point(140, 80), Size = new Size(250, 23) };

            var lblAddress = new Label { Text = "Address:", Location = new Point(20, 115), AutoSize = true };
            txtAddress = new TextBox { Location = new Point(140, 110), Size = new Size(250, 23) };

            var lblType = new Label { Text = "Type:", Location = new Point(20, 145), AutoSize = true };
            cmbType = new ComboBox { Location = new Point(140, 140), Size = new Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new[] { "Renter", "Owner", "Buyer" });
            cmbType.SelectedIndex = 0;

            var lblStatus = new Label { Text = "Status:", Location = new Point(20, 175), AutoSize = true };
            cmbStatus = new ComboBox { Location = new Point(140, 170), Size = new Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new[] { "New Lead", "Contacted", "Qualified", "Unqualified" });
            cmbStatus.SelectedIndex = 0;

            var lblLastContacted = new Label { Text = "Last Contacted:", Location = new Point(20, 205), AutoSize = true };
            dtpLastContacted = new DateTimePicker
            {
                Location = new Point(140, 200),
                Size = new Size(150, 23),
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true, // Allows user to specify if a date is set
                Checked = false      // Unchecked by default, meaning null
            };

            btnSave = new Button { Text = "Save", Location = new Point(290, 320), Size = new Size(100, 30), DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancel", Location = new Point(180, 320), Size = new Size(100, 30), DialogResult = DialogResult.Cancel };

            btnSave.Click += BtnSave_Click;

            Controls.AddRange(new Control[] {
                lblFullName, txtFullName, lblEmail, txtEmail, lblPhone, txtPhone, lblAddress, txtAddress,
                lblType, cmbType, lblStatus, cmbStatus, lblLastContacted, dtpLastContacted,
                btnSave, btnCancel
            });

            AcceptButton = btnSave;
            CancelButton = btnCancel;
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

            CreatedLead = new Lead
            {
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Type = cmbType.SelectedItem?.ToString() ?? "Renter",
                Status = cmbStatus.SelectedItem?.ToString() ?? "New Lead",
                LastContacted = dtpLastContacted.Checked ? dtpLastContacted.Value : (DateTime?)null,
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