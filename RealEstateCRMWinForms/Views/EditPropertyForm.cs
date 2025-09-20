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

        // Added description textbox
        private TextBox txtDescription;

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
            // Slightly larger to accommodate a cleaner layout
            Size = new Size(760, 480);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 10F);

            // Create a table layout for a cleaner, less clumpy layout
            var tl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 8,
                Padding = new Padding(12),
                AutoSize = false
            };

            // Columns: label (auto), control (percent), image column (fixed)
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F)); // right column for image

            // Row styles (labels + controls rows)
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F)); // Title
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F)); // Address
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F)); // Price
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F)); // Features (bigger to accommodate icons)
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F)); // Status
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F)); // Description (taller)
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Image area
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F)); // Buttons

            // Title
            var lblTitle = new Label { Text = "Title:", Anchor = AnchorStyles.Left, AutoSize = true };
            txtTitle = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            tl.Controls.Add(lblTitle, 0, 0);
            tl.Controls.Add(txtTitle, 1, 0);
            // reserve image column cell for top rows (empty for title)
            tl.SetColumnSpan(txtTitle, 1);

            // Address
            var lblAddress = new Label { Text = "Address:", Anchor = AnchorStyles.Left, AutoSize = true };
            txtAddress = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            tl.Controls.Add(lblAddress, 0, 1);
            tl.Controls.Add(txtAddress, 1, 1);

            // Price
            var lblPrice = new Label { Text = "Price:", Anchor = AnchorStyles.Left, AutoSize = true };
            numPrice = new NumericUpDown
            {
                Anchor = AnchorStyles.Left,
                Maximum = 999999999,
                DecimalPlaces = 0,
                ThousandsSeparator = true,
                Width = 160
            };
            tl.Controls.Add(lblPrice, 0, 2);
            tl.Controls.Add(numPrice, 1, 2);

            // Beds / Baths / Sqm - put controls inline inside a flow panel to keep them compact
            var lblBedsBaths = new Label { Text = "Bedrooms:", Anchor = AnchorStyles.Left, AutoSize = true };
            var pnlBeds = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Anchor = AnchorStyles.Left };
            numBedrooms = new NumericUpDown { Minimum = 0, Maximum = 20, Width = 70 };
            var lblBathroomsInline = new Label { Text = "Bathrooms:", TextAlign = ContentAlignment.MiddleLeft, AutoSize = true, Margin = new Padding(12, 6, 0, 0) };
            numBathrooms = new NumericUpDown { Minimum = 0, Maximum = 20, Width = 70 };
            var lblSqmInline = new Label { Text = "Sqm:", TextAlign = ContentAlignment.MiddleLeft, AutoSize = true, Margin = new Padding(12, 6, 0, 0) };
            numSquareMeters = new NumericUpDown { Minimum = 1, Maximum = 10000, Width = 90 };

            pnlBeds.Controls.Add(numBedrooms);
            pnlBeds.Controls.Add(lblBathroomsInline);
            pnlBeds.Controls.Add(numBathrooms);
            pnlBeds.Controls.Add(lblSqmInline);
            pnlBeds.Controls.Add(numSquareMeters);

            tl.Controls.Add(lblBedsBaths, 0, 3);
            tl.Controls.Add(pnlBeds, 1, 3);

            // Status
            var lblStatus = new Label { Text = "Status:", Anchor = AnchorStyles.Left, AutoSize = true };
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left };
            cmbStatus.Items.AddRange(new[] { "Sell", "Rent" });
            cmbStatus.Width = 140;
            tl.Controls.Add(lblStatus, 0, 4);
            tl.Controls.Add(cmbStatus, 1, 4);

            // Description (multiline)
            var lblDescription = new Label { Text = "Description:", Anchor = AnchorStyles.Left | AnchorStyles.Top, AutoSize = true };
            txtDescription = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical, Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom };
            tl.Controls.Add(lblDescription, 0, 5);
            tl.Controls.Add(txtDescription, 1, 5);

            // Right column: image + change image button stacked
            var pnlImage = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(6)
            };
            pnlImage.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlImage.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            pnlImage.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));

            var lblImage = new Label { Text = "Property Image", Anchor = AnchorStyles.Left, AutoSize = true };
            pictureBoxPreview = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };
            btnSelectImage = new Button
            {
                Text = "Change Image",
                Anchor = AnchorStyles.Right,
                AutoSize = true,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSelectImage.FlatAppearance.BorderSize = 0;
            btnSelectImage.Click += BtnSelectImage_Click;

            pnlImage.Controls.Add(lblImage, 0, 0);
            pnlImage.Controls.Add(pictureBoxPreview, 0, 1);
            pnlImage.Controls.Add(btnSelectImage, 0, 2);

            // Place the image panel spanning rows 0..6 in column 2
            tl.Controls.Add(pnlImage, 2, 0);
            tl.SetRowSpan(pnlImage, 7);

            // Buttons row (Cancel + Save) aligned to right
            var pnlButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0),
                Anchor = AnchorStyles.Right
            };

            btnSave = new Button
            {
                Text = "Save",
                AutoSize = true,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                AutoSize = true,
                DialogResult = DialogResult.Cancel
            };

            pnlButtons.Controls.Add(btnSave);
            pnlButtons.Controls.Add(btnCancel);

            // Add an empty label in left column for alignment then add buttons spanning columns 0..1
            tl.Controls.Add(new Label() { AutoSize = true }, 0, 7);
            tl.Controls.Add(pnlButtons, 1, 7);

            Controls.Add(tl);

            // Wire form-level buttons
            CancelButton = btnCancel;
            AcceptButton = btnSave; // Fix: make Enter trigger Save

            // Set a sensible minimum size for nicer resizing
            MinimumSize = new Size(720, 420);
        }

        private void PopulateFields()
        {
            if (_property == null) return;

            txtTitle.Text = _property.Title;
            txtAddress.Text = _property.Address;

            // Ensure numeric values are in-range before assigning to NumericUpDowns
            if (numPrice != null)
            {
                try
                {
                    if (_property.Price >= numPrice.Minimum && _property.Price <= numPrice.Maximum)
                        numPrice.Value = _property.Price;
                    else
                        numPrice.Value = Math.Min(Math.Max(_property.Price, numPrice.Minimum), numPrice.Maximum);
                }
                catch { numPrice.Value = 0; }
            }

            numBedrooms.Value = Math.Min(Math.Max(_property.Bedrooms, (int)numBedrooms.Minimum), (int)numBedrooms.Maximum);
            numBathrooms.Value = Math.Min(Math.Max(_property.Bathrooms, (int)numBathrooms.Minimum), (int)numBathrooms.Maximum);
            numSquareMeters.Value = Math.Min(Math.Max(_property.SquareMeters, (int)numSquareMeters.Minimum), (int)numSquareMeters.Maximum);
            cmbStatus.SelectedItem = string.IsNullOrEmpty(_property.Status) ? "Sell" : _property.Status;

            // Populate description field
            txtDescription.Text = _property.Description;

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
                            pictureBoxPreview.Image?.Dispose();
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
            pictureBoxPreview.Image?.Dispose();
            var defaultBitmap = new Bitmap(240, 160);
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

            // Update description
            _property.Description = txtDescription.Text?.Trim() ?? string.Empty;

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