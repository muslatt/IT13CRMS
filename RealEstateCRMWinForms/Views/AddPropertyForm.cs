using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class AddPropertyForm : Form
    {
        private PropertyViewModel _viewModel;
        
        private TextBox txtTitle;
        private TextBox txtAddress;
        private NumericUpDown numPrice;
        private NumericUpDown numBedrooms;
        private NumericUpDown numBathrooms;
        private NumericUpDown numSquareMeters;
        private ComboBox cmbStatus;
        private Button btnSave;
        private Button btnCancel;

        public AddPropertyForm()
        {
            _viewModel = new PropertyViewModel();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Add New Property";
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Title
            var lblTitle = new Label
            {
                Text = "Title:",
                Location = new Point(20, 20),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            txtTitle = new TextBox
            {
                Location = new Point(130, 20),
                Size = new Size(320, 23),
                Font = new Font("Segoe UI", 9F)
            };

            // Address
            var lblAddress = new Label
            {
                Text = "Address:",
                Location = new Point(20, 55),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            txtAddress = new TextBox
            {
                Location = new Point(130, 55),
                Size = new Size(320, 23),
                Font = new Font("Segoe UI", 9F)
            };

            // Price
            var lblPrice = new Label
            {
                Text = "Price:",
                Location = new Point(20, 90),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            numPrice = new NumericUpDown
            {
                Location = new Point(130, 90),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 9F),
                Maximum = 999999999,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            // Bedrooms
            var lblBedrooms = new Label
            {
                Text = "Bedrooms:",
                Location = new Point(20, 125),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            numBedrooms = new NumericUpDown
            {
                Location = new Point(130, 125),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9F),
                Minimum = 0,
                Maximum = 20
            };

            // Bathrooms
            var lblBathrooms = new Label
            {
                Text = "Bathrooms:",
                Location = new Point(250, 125),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9F)
            };

            numBathrooms = new NumericUpDown
            {
                Location = new Point(340, 125),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 9F),
                Minimum = 0,
                Maximum = 20
            };

            // Square Meters
            var lblSquareMeters = new Label
            {
                Text = "Square Meters:",
                Location = new Point(20, 160),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            numSquareMeters = new NumericUpDown
            {
                Location = new Point(130, 160),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F),
                Minimum = 1,
                Maximum = 10000
            };

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
                Size = new Size(120, 23),
                Font = new Font("Segoe UI", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new[] { "Sell", "Rent" });
            cmbStatus.SelectedIndex = 0;

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(280, 320),
                Size = new Size(80, 35),
                Font = new Font("Segoe UI", 9F),
                DialogResult = DialogResult.Cancel
            };

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(370, 320),
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
                lblTitle, txtTitle,
                lblAddress, txtAddress,
                lblPrice, numPrice,
                lblBedrooms, numBedrooms,
                lblBathrooms, numBathrooms,
                lblSquareMeters, numSquareMeters,
                lblStatus, cmbStatus,
                btnCancel, btnSave
            });

            CancelButton = btnCancel;
            AcceptButton = btnSave;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a property title.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please enter a property address.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return;
            }

            if (numPrice.Value <= 0)
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numPrice.Focus();
                return;
            }

            // Create new property
            var newProperty = new Property
            {
                Title = txtTitle.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Price = numPrice.Value,
                Bedrooms = (int)numBedrooms.Value,
                Bathrooms = (int)numBathrooms.Value,
                SquareMeters = (int)numSquareMeters.Value,
                Status = cmbStatus.SelectedItem?.ToString() ?? "Sell",
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            // Save to database
            if (_viewModel.AddProperty(newProperty))
            {
                MessageBox.Show("Property added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}