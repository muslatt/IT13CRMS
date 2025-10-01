using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using RealEstateCRMWinForms.Services;
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
        private TextBox txtOccupation;
        private TextBox txtSalary;
        private Button btnSave;
        private Button btnCancel;

        private readonly PropertyViewModel _propertyViewModel;

        public Lead? CreatedLead { get; private set; }

        public AddLeadForm()
        {
            _propertyViewModel = new PropertyViewModel();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Add New Lead";
            Size = new Size(450, 300);
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

            var lblOccupation = new Label { Text = "Occupation:", Location = new Point(20, 115), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            txtOccupation = new TextBox { Location = new Point(140, 110), Size = new Size(250, 28), Font = new Font("Segoe UI", 12F) };

            var lblSalary = new Label { Text = "Salary:", Location = new Point(20, 145), AutoSize = true, Font = new Font("Segoe UI", 12F) };
            txtSalary = new TextBox { Location = new Point(140, 140), Size = new Size(200, 28), Font = new Font("Segoe UI", 12F), PlaceholderText = "Optional" };

            btnSave = new Button { Text = "Save", Location = new Point(290, 210), Size = new Size(110, 35), DialogResult = DialogResult.OK, Font = new Font("Segoe UI", 12F) };
            btnCancel = new Button { Text = "Cancel", Location = new Point(180, 210), Size = new Size(110, 35), DialogResult = DialogResult.Cancel, Font = new Font("Segoe UI", 12F) };

            btnSave.Click += BtnSave_Click;

            Controls.AddRange(new Control[] {
                lblFullName, txtFullName, lblEmail, txtEmail, lblPhone, txtPhone,
                lblOccupation, txtOccupation, lblSalary, txtSalary,
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

            // Parse salary if provided
            decimal? salary = null;
            if (!string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                if (decimal.TryParse(txtSalary.Text, out decimal parsedSalary))
                {
                    salary = parsedSalary;
                }
                else
                {
                    MessageBox.Show("Please enter a valid salary amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalary.Focus();
                    DialogResult = DialogResult.None; // Keep form open
                    return;
                }
            }

            // Get selected agent information - now auto-assigned to current user
            string assignedAgent = string.Empty;
            var currentUser = UserSession.Instance.CurrentUser;
            if (currentUser != null)
            {
                if (currentUser.Role == UserRole.Broker)
                {
                    assignedAgent = "Broker";
                }
                else if (currentUser.Role == UserRole.Agent)
                {
                    assignedAgent = currentUser.FullName;
                }
                // For other roles, leave as empty
            }

            CreatedLead = new Lead
            {
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Occupation = txtOccupation.Text.Trim(),
                Salary = salary,
                Type = "Lead", // Set default type to "Lead" for new entries
                AssignedAgent = assignedAgent, // NotMapped property provided by Lead.Extensions
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
