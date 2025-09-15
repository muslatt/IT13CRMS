using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using RealEstateCRMWinForms.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

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
        private PictureBox pictureBoxPreview;
        private Button btnSelectImage;
        private Button btnSave;
        private Button btnCancel;
        private string _selectedImagePath;

        public AddPropertyForm()
        {
            _viewModel = new PropertyViewModel();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Add New Property";
            // Set the form font (size 12) so the form's displayed font is 12pt
            Font = new Font("Segoe UI", 12F);
            Size = new Size(600, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Title
            var lblTitle = new Label
            {
                Text = "Title:",
                Location = new Point(20, 20),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            txtTitle = new TextBox
            {
                Location = new Point(160, 20),
                Size = new Size(320, 23),
                Font = new Font("Segoe UI", 12F)
            };

            // Address
            var lblAddress = new Label
            {
                Text = "Address:",
                Location = new Point(20, 55),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            txtAddress = new TextBox
            {
                Location = new Point(160, 55),
                Size = new Size(320, 23),
                Font = new Font("Segoe UI", 12F)
            };

            // Price
            var lblPrice = new Label
            {
                Text = "Price:",
                Location = new Point(20, 90),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            numPrice = new NumericUpDown
            {
                Location = new Point(160, 90),
                Size = new Size(150, 23),
                Font = new Font("Segoe UI", 12F),
                Maximum = 999999999,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            // Bedrooms
            var lblBedrooms = new Label
            {
                Text = "Bedrooms:",
                Location = new Point(20, 125),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            numBedrooms = new NumericUpDown
            {
                Location = new Point(160, 125),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 12F),
                Minimum = 0,
                Maximum = 20
            };

            // Bathrooms
            var lblBathrooms = new Label
            {
                Text = "Bathrooms:",
                Location = new Point(250, 125),
                Size = new Size(100, 28),
                Font = new Font("Segoe UI", 12F)
            };

            numBathrooms = new NumericUpDown
            {
                Location = new Point(370, 125),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 12F),
                Minimum = 0,
                Maximum = 20
            };

            // Square Meters
            var lblSquareMeters = new Label
            {
                Text = "Square Meters:",
                Location = new Point(20, 160),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            numSquareMeters = new NumericUpDown
            {
                Location = new Point(160, 160),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 12F),
                Minimum = 1,
                Maximum = 10000
            };

            // Status
            var lblStatus = new Label
            {
                Text = "Status:",
                Location = new Point(20, 195),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            cmbStatus = new ComboBox
            {
                Location = new Point(160, 195),
                Size = new Size(120, 23),
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new[] { "Sell", "Rent" });
            cmbStatus.SelectedIndex = 0;

            // Image selection
            var lblImage = new Label
            {
                Text = "Property Image:",
                Location = new Point(20, 230),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            pictureBoxPreview = new PictureBox
            {
                Location = new Point(160, 230),
                Size = new Size(200, 150),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.LightGray
            };

            btnSelectImage = new Button
            {
                Text = "Select Image",
                Location = new Point(400, 260),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSelectImage.FlatAppearance.BorderSize = 0;
            btnSelectImage.Click += BtnSelectImage_Click;

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(400, 420),
                Size = new Size(80, 35),
                Font = new Font("Segoe UI", 12F),
                DialogResult = DialogResult.Cancel
            };

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(490, 420),
                Size = new Size(80, 35),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
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
                lblImage, pictureBoxPreview, btnSelectImage,
                btnCancel, btnSave
            });

            CancelButton = btnCancel;
            AcceptButton = btnSave;

            // Set default image
            SetDefaultPreviewImage();
        }

        private void SetDefaultPreviewImage()
        {
            var defaultBitmap = new Bitmap(200, 150);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.LightGray);
                using (var brush = new SolidBrush(Color.Gray))
                using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                {
                    string text = "No Image Selected";
                    var textSize = g.MeasureString(text, font);
                    var x = (defaultBitmap.Width - textSize.Width) / 2;
                    var y = (defaultBitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            pictureBoxPreview.Image = defaultBitmap;
        }

        private void BtnSelectImage_Click(object? sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Property Image";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _selectedImagePath = openFileDialog.FileName;
                        
                        // Load and display preview
                        using (var img = Image.FromFile(_selectedImagePath))
                        {
                            pictureBoxPreview.Image?.Dispose();
                            pictureBoxPreview.Image = new Bitmap(img);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _selectedImagePath = null;
                        SetDefaultPreviewImage();
                    }
                }
            }
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

            // Save to database first to get the ID
            if (_viewModel.AddProperty(newProperty))
            {
                // Save image if selected
                if (!string.IsNullOrEmpty(_selectedImagePath))
                {
                    string savedImageName = PropertyCard.SavePropertyImage(_selectedImagePath, newProperty.Id);
                    if (!string.IsNullOrEmpty(savedImageName))
                    {
                        newProperty.ImagePath = savedImageName;
                        _viewModel.UpdateProperty(newProperty); // Update with image path
                    }
                }

                MessageBox.Show("Property added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pictureBoxPreview?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}