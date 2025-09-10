using RealEstateCRMWinForms.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class EditContactForm : Form
    {
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private ComboBox cmbType;
        private Button btnSave;
        private Button btnCancel;
        private Contact _contact;

        public EditContactForm(Contact contact)
        {
            _contact = contact;
            InitializeComponent();
            LoadContactData();
        }

        private void InitializeComponent()
        {
            Text = "Edit Contact";
            Size = new Size(450, 320);
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
                Size = new Size(280, 23),
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
                Size = new Size(280, 23),
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

            // Type
            var lblType = new Label
            {
                Text = "Type:",
                Location = new Point(20, 125),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            cmbType = new ComboBox
            {
                Location = new Point(130, 125),
                Size = new Size(120, 23),
                Font = new Font("Segoe UI", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbType.Items.AddRange(new[] { "Renter", "Owner", "Buyer" });

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(250, 220),
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
                Location = new Point(340, 220),
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
                lblType, cmbType,
                btnCancel, btnSave
            });

            CancelButton = btnCancel;
            AcceptButton = btnSave;
        }

        private void LoadContactData()
        {
            txtFullName.Text = _contact.FullName;
            txtEmail.Text = _contact.Email;
            txtPhone.Text = _contact.Phone;
            cmbType.SelectedItem = _contact.Type;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter the contact's full name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter the contact's email address.", "Validation Error",
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

            // Update contact object
            _contact.FullName = txtFullName.Text.Trim();
            _contact.Email = txtEmail.Text.Trim();
            _contact.Phone = txtPhone.Text.Trim();
            _contact.Type = cmbType.SelectedItem?.ToString() ?? "Buyer";

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