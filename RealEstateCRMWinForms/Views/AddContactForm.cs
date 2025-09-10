using RealEstateCRMWinForms.Models;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class AddContactForm : Form
    {
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private ComboBox cmbType;
        private Button btnSave;
        private Button btnCancel;

        public AddContactForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Add New Contact";
            Size = new Size(500, 350);
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
                PlaceholderText = "+1 231 231 2312"
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
            cmbType.Items.AddRange(new[] { "Buyer", "Renter", "Owner" });
            cmbType.SelectedIndex = 0; // Default to Buyer

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(280, 270),
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
                Text = "Save Contact",
                Location = new Point(370, 270),
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
                lblType, cmbType,
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

            // Create new contact
            var newContact = new Contact
            {
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Type = cmbType.SelectedItem?.ToString() ?? "Buyer",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Set the created contact as a property so the calling form can access it
            CreatedContact = newContact;
            DialogResult = DialogResult.OK;
            Close();
        }

        public Contact? CreatedContact { get; private set; }

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