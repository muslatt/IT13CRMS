using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using RealEstateCRMWinForms.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace RealEstateCRMWinForms.Views
{
    public partial class EditPropertyForm : Form
    {
        private PropertyViewModel _viewModel;
        private Property _property;
        
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

        public EditPropertyForm(Property property)
        {
            _viewModel = new PropertyViewModel();
            _property = property;
            InitializeComponent();
            PopulateFields();
        }

        private void InitializeComponent()
        {
            Text = "Edit Property";
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

            // Image selection
            var lblImage = new Label
            {
                Text = "Property Image:",
                Location = new Point(20, 230),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 9F)
            };

            pictureBoxPreview = new PictureBox
            {
                Location = new Point(130, 230),
                Size = new Size(200, 150),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.LightGray
            };

            btnSelectImage = new Button
            {
                Text = "Change Image",
                Location = new Point(350, 260),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9F),
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
                Font = new Font("Segoe UI", 9F),
                DialogResult = DialogResult.Cancel
            };

            btnSave = new Button
            {
                Text = "Save Changes",
                Location = new Point(490, 420),
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
                lblImage, pictureBoxPreview, btnSelectImage,
                btnCancel, btnSave
            });

            CancelButton = btnCancel;
            AcceptButton = btnSave;
        }

        private void PopulateFields()
        {
            txtTitle.Text = _property.Title;
            txtAddress.Text = _property.Address;
            numPrice.Value = _property.Price;
            numBedrooms.Value = _property.Bedrooms;
            numBathrooms.Value = _property.Bathrooms;
            numSquareMeters.Value = _property.SquareMeters;
            cmbStatus.SelectedItem = _property.Status;

            // Load existing image
            LoadCurrentImage();
        }

        private void LoadCurrentImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_property.ImagePath))
                {
                    string imagePath = Path.Combine(Application.StartupPath, "PropertyImages", _property.ImagePath);
                    if (File.Exists(imagePath))
                    {
                        using (var img = Image.FromFile(imagePath))
                        {
                            pictureBoxPreview.Image = new Bitmap(img);
                        }
                        return;
                    }
                }
                
                SetDefaultPreviewImage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading current image: {ex.Message}");
                SetDefaultPreviewImage();
            }
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
                    string text = "No Image";
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
                        LoadCurrentImage();
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

            // Update property object
            _property.Title = txtTitle.Text.Trim();
            _property.Address = txtAddress.Text.Trim();
            _property.Price = numPrice.Value;
            _property.Bedrooms = (int)numBedrooms.Value;
            _property.Bathrooms = (int)numBathrooms.Value;
            _property.SquareMeters = (int)numSquareMeters.Value;
            _property.Status = cmbStatus.SelectedItem?.ToString() ?? "Sell";

            // Save new image if selected
            if (!string.IsNullOrEmpty(_selectedImagePath))
            {
                string savedImageName = PropertyCard.SavePropertyImage(_selectedImagePath, _property.Id);
                if (!string.IsNullOrEmpty(savedImageName))
                {
                    _property.ImagePath = savedImageName;
                }
            }

            // Update in database
            if (_viewModel.UpdateProperty(_property))
            {
                MessageBox.Show("Property updated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Failed to update property. Please try again.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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