using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using RealEstateCRMWinForms.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace RealEstateCRMWinForms.Views
{
    public partial class AddPropertyForm : Form
    {
        private PropertyViewModel _viewModel;

        private TextBox? txtTitle;
        private TextBox? txtAddress;
        private NumericUpDown? numPrice;
        private NumericUpDown? numBedrooms;
        private NumericUpDown? numBathrooms;
        private NumericUpDown? numLotAreaSqm;
        private NumericUpDown? numFloorAreaSqft;
        private Label? lblLotArea;
        private Label? lblFloorArea;
        private ComboBox? cmbPropertyType;
        private ComboBox? cmbTransactionType;
        private TextBox? txtDescription;
        private PictureBox? pictureBoxPreview;
        private Button? btnSelectImage;
        private FlowLayoutPanel? proofFilesPanel;
        private Button? btnSelectProofFiles;
        private Button? btnCancelProofFile;
        private Button? btnSave;
        private Button? btnCancel;
        private string? _selectedImagePath;
        private List<string> _selectedProofFilePaths;

        // Listing date control
        private DateTimePicker? dtpListingDate;

        public AddPropertyForm()
        {
            _viewModel = new PropertyViewModel();
            _selectedProofFilePaths = new List<string>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Add New Property";
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(248, 249, 250);

            // Maintain current form height
            Size = new Size(950, 965);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Main scrollable container to handle overflow
            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Main container with adjusted height to fit buttons properly
            var mainContainer = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(900, 1000), // Reduced height to fit buttons within form
                Padding = new Padding(25),
                BackColor = Color.White
            };
            mainContainer.Paint += (s, e) =>
            {
                // Add subtle border for modern look
                using (var pen = new Pen(Color.FromArgb(222, 226, 230), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, mainContainer.Width - 1, mainContainer.Height - 1);
                }
            };

            // Optimized positioning with better spacing
            int currentY = 25;
            int leftMargin = 25;
            int labelWidth = 150;
            int controlSpacing = 40; // Slightly reduced spacing
            int formWidth = 850;

            // Title
            var lblTitle = new Label
            {
                Text = "Title:",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtTitle = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY),
                Size = new Size(formWidth - labelWidth - 65, 30),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            currentY += controlSpacing;

            // Address
            var lblAddress = new Label
            {
                Text = "Address:",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtAddress = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY),
                Size = new Size(formWidth - labelWidth - 65, 30),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            currentY += controlSpacing;

            // Price and Bedrooms on same row
            var lblPrice = new Label
            {
                Text = "Price:",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            numPrice = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY),
                Size = new Size(280, 30),
                Font = new Font("Segoe UI", 10F),
                Maximum = 999999999,
                DecimalPlaces = 0,
                ThousandsSeparator = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblBedrooms = new Label
            {
                Text = "Bedrooms:",
                Location = new Point(520, currentY),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            numBedrooms = new NumericUpDown
            {
                Location = new Point(630, currentY),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 10F),
                Minimum = 0,
                Maximum = 20,
                BorderStyle = BorderStyle.FixedSingle
            };

            currentY += controlSpacing;

            // Bathrooms
            var lblBathrooms = new Label
            {
                Text = "Bathrooms:",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            numBathrooms = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 10F),
                Minimum = 0,
                Maximum = 20,
                BorderStyle = BorderStyle.FixedSingle
            };

            currentY += controlSpacing;

            // Lot Area and Floor Area on same row
            lblLotArea = new Label
            {
                Text = "Lot Area (Sqm):",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            numLotAreaSqm = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 10F),
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblFloorArea = new Label
            {
                Text = "Floor Area (Sqft):",
                Location = new Point(380, currentY),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            numFloorAreaSqft = new NumericUpDown
            {
                Location = new Point(530, currentY),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 10F),
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                BorderStyle = BorderStyle.FixedSingle
            };

            currentY += controlSpacing;

            // Property Type and Transaction Type on same row
            var lblPropertyType = new Label
            {
                Text = "Property Type:",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbPropertyType = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmbPropertyType.Items.AddRange(new[] { "Residential", "Commercial", "Raw Land" });
            cmbPropertyType.SelectedIndex = 0;
            cmbPropertyType.SelectedIndexChanged += CmbPropertyType_SelectedIndexChanged;

            var lblTransactionType = new Label
            {
                Text = "Transaction:",
                Location = new Point(450, currentY),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbTransactionType = new ComboBox
            {
                Location = new Point(560, currentY),
                Size = new Size(160, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmbTransactionType.Items.AddRange(new[] { "Buying", "Viewing" });
            cmbTransactionType.SelectedIndex = 0;

            currentY += controlSpacing;

            // Listing Date
            var lblListingDate = new Label
            {
                Text = "Listing Date:",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dtpListingDate = new DateTimePicker
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            currentY += controlSpacing;

            // Description
            var lblDescription = new Label
            {
                Text = "Description:",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtDescription = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY),
                Size = new Size(formWidth - labelWidth - 65, 80), // Reduced height to save space
                Font = new Font("Segoe UI", 10F),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            currentY += 100; // Reduced spacing after description

            // Property Image section
            var lblImage = new Label
            {
                Text = "Property Image:",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pictureBoxPreview = new PictureBox
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY),
                Size = new Size(320, 160), // Slightly reduced size
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            btnSelectImage = new Button
            {
                Text = "Select Image",
                Location = new Point(leftMargin + labelWidth + 350, currentY + 60),
                Size = new Size(130, 35), // Slightly smaller button
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSelectImage.FlatAppearance.BorderSize = 0;
            btnSelectImage.FlatAppearance.MouseOverBackColor = Color.FromArgb(33, 136, 56);
            btnSelectImage.Click += BtnSelectImage_Click;

            currentY += 180; // Reduced spacing after property image

            // Proof of ownership section
            var lblProof = new Label
            {
                Text = "Proof of ownership:",
                Location = new Point(leftMargin, currentY),
                Size = new Size(labelWidth, 30),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblProofSubtext = new Label
            {
                Text = "(land title, tax declaration, deed of sale)",
                Location = new Point(leftMargin, currentY + 30),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                TextAlign = ContentAlignment.MiddleLeft
            };

            proofFilesPanel = new FlowLayoutPanel
            {
                Location = new Point(leftMargin + labelWidth + 15, currentY + 55),
                Size = new Size(320, 160),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true
            };

            btnSelectProofFiles = new Button
            {
                Text = "Select Proof Files",
                Location = new Point(leftMargin + labelWidth + 350, currentY + 55 + 60),
                Size = new Size(140, 35),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSelectProofFiles.FlatAppearance.BorderSize = 0;
            btnSelectProofFiles.FlatAppearance.MouseOverBackColor = Color.FromArgb(33, 136, 56);
            btnSelectProofFiles.Click += BtnSelectProofFiles_Click;

            btnCancelProofFile = new Button
            {
                Text = "✕",
                Location = new Point(leftMargin + labelWidth + 350 + 140, currentY + 55 + 60),
                Size = new Size(35, 35),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Visible = false // Initially hidden
            };
            btnCancelProofFile.FlatAppearance.BorderSize = 0;
            btnCancelProofFile.FlatAppearance.MouseOverBackColor = Color.FromArgb(176, 42, 55);
            btnCancelProofFile.Click += (s, e) => ClearAllProofFiles();

            currentY += 240; // Reduced spacing after proof image

            // Action buttons - positioned to fit within form height
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(formWidth - 260, currentY),
                Size = new Size(110, 40), // Slightly smaller buttons
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(90, 98, 104);

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(formWidth - 140, currentY),
                Size = new Size(110, 40), // Slightly smaller buttons
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 86, 179);
            btnSave.Click += BtnSave_Click;

            // Add all controls to main container
            mainContainer.Controls.AddRange(new Control[] {
                lblTitle, txtTitle,
                lblAddress, txtAddress,
                lblPrice, numPrice, lblBedrooms, numBedrooms,
                lblBathrooms, numBathrooms,
                lblLotArea, numLotAreaSqm,
                lblFloorArea, numFloorAreaSqft,
                lblPropertyType, cmbPropertyType,
                lblTransactionType, cmbTransactionType,
                lblListingDate, dtpListingDate,
                lblDescription, txtDescription,
                lblImage, pictureBoxPreview, btnSelectImage,
                lblProof, lblProofSubtext, proofFilesPanel, btnSelectProofFiles, btnCancelProofFile,
                btnCancel, btnSave
            });

            scrollPanel.Controls.Add(mainContainer);
            Controls.Add(scrollPanel);

            CancelButton = btnCancel;
            AcceptButton = btnSave;

            // Set default image
            SetDefaultPreviewImage();
        }

        private void SetDefaultPreviewImage()
        {
            var defaultBitmap = new Bitmap(320, 160); // Adjusted to match new size
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.FromArgb(248, 249, 250));
                using (var brush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
                {
                    string text = "No Image Selected";
                    var textSize = g.MeasureString(text, font);
                    var x = (defaultBitmap.Width - textSize.Width) / 2;
                    var y = (defaultBitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }

            // Assign default to property image preview
            pictureBoxPreview!.Image?.Dispose();
            pictureBoxPreview!.Image = defaultBitmap;
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
                            pictureBoxPreview!.Image?.Dispose();
                            pictureBoxPreview!.Image = new Bitmap(img);
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

        private void BtnSelectProofFiles_Click(object? sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Proof of Ownership Files";
                openFileDialog.Filter = "All Files|*.*|Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = true; // Allow multiple file selection

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Add selected files to the list
                        _selectedProofFilePaths.AddRange(openFileDialog.FileNames);

                        // Clear existing cards and recreate them
                        proofFilesPanel!.Controls.Clear();

                        foreach (var filePath in _selectedProofFilePaths)
                        {
                            var fileCard = CreateProofFileCard(filePath);
                            proofFilesPanel!.Controls.Add(fileCard);
                        }

                        // Show cancel button and update select button text
                        btnCancelProofFile!.Visible = true;
                        btnSelectProofFiles!.Text = "Add More Files";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading proof files: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _selectedProofFilePaths.Clear();
                    }
                }
            }
        }

        private Control CreateProofFileCard(string filePath)
        {
            var cardPanel = new Panel
            {
                Size = new Size(140, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(2)
            };

            // File icon (top)
            var iconLabel = new Label
            {
                Text = "📄",
                Font = new Font("Segoe UI", 16F),
                Location = new Point(55, 5),
                Size = new Size(30, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // File name (truncated if too long)
            var fileName = Path.GetFileName(filePath);
            if (fileName.Length > 15)
                fileName = fileName.Substring(0, 12) + "...";
            var nameLabel = new Label
            {
                Text = fileName,
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                Location = new Point(5, 35),
                Size = new Size(130, 14),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            // File size
            var fileInfo = new FileInfo(filePath);
            var sizeLabel = new Label
            {
                Text = FormatFileSize(fileInfo.Length),
                Font = new Font("Segoe UI", 6F),
                Location = new Point(5, 50),
                Size = new Size(130, 12),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            // Remove button
            var btnRemove = new Button
            {
                Text = "✕",
                Size = new Size(20, 20),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(115, 5)
            };
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Tag = filePath; // Store the file path reference
            btnRemove.Click += (s, e) => RemoveProofFile(filePath);

            cardPanel.Controls.AddRange(new Control[] { iconLabel, nameLabel, sizeLabel, btnRemove });

            return cardPanel;
        }

        private void RemoveProofFile(string filePath)
        {
            _selectedProofFilePaths.Remove(filePath);
            proofFilesPanel!.Controls.Clear();

            // Recreate cards for remaining files
            foreach (var path in _selectedProofFilePaths)
            {
                var fileCard = CreateProofFileCard(path);
                proofFilesPanel!.Controls.Add(fileCard);
            }

            // Hide cancel button if no files left
            if (_selectedProofFilePaths.Count == 0)
            {
                btnCancelProofFile!.Visible = false;
                btnSelectProofFiles!.Text = "Select Proof Files";
            }
        }

        private void ClearAllProofFiles()
        {
            _selectedProofFilePaths.Clear();
            proofFilesPanel!.Controls.Clear();

            // Hide cancel button
            btnCancelProofFile!.Visible = false;

            // Reset the select button text
            btnSelectProofFiles!.Text = "Select Proof Files";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtTitle!.Text))
            {
                MessageBox.Show("Please enter a property title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle!.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddress!.Text))
            {
                MessageBox.Show("Please enter a property address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress!.Focus();
                return;
            }

            if (numPrice!.Value <= 0)
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numPrice!.Focus();
                return;
            }

            // Check if proof files are required for clients
            var currentUser = Services.UserSession.Instance.CurrentUser;
            if (currentUser != null && currentUser.Role == UserRole.Client && (_selectedProofFilePaths == null || _selectedProofFilePaths.Count == 0))
            {
                MessageBox.Show("Proof of ownership files are required for client submissions. Please select at least one proof file to continue.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSelectProofFiles!.Focus();
                return;
            }

            // Create new property
            var newProperty = new Property
            {
                Title = txtTitle!.Text.Trim(),
                Address = txtAddress!.Text.Trim(),
                Price = numPrice!.Value,
                Bedrooms = (int)numBedrooms!.Value,
                Bathrooms = (int)numBathrooms!.Value,
                LotAreaSqm = numLotAreaSqm?.Value ?? 0,
                FloorAreaSqft = numFloorAreaSqft?.Value ?? 0,
                CreatedAt = dtpListingDate?.Value ?? DateTime.Now,
                IsActive = true,
                IsApproved = true, // Default to approved
                SubmittedByUserId = null // Default to null
            };

            // Set approval status based on current user role
            if (currentUser != null && currentUser.Role == UserRole.Client)
            {
                newProperty.IsApproved = false;
                newProperty.SubmittedByUserId = currentUser.Id;
            }

            // Try to set commonly named string properties on the Property model via reflection.
            TrySetStringProperty(newProperty, new[] { "PropertyType", "Type" }, cmbPropertyType!.SelectedItem?.ToString() ?? "Residential");
            TrySetStringProperty(newProperty, new[] { "TransactionType", "Type", "ListingType", "ListingMode" }, cmbTransactionType!.SelectedItem?.ToString() ?? "Buying");
            TrySetStringProperty(newProperty, new[] { "Description", "Notes", "Details" }, txtDescription!.Text?.Trim() ?? string.Empty);

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

                // Save proof files if selected
                if (_selectedProofFilePaths != null && _selectedProofFilePaths.Count > 0)
                {
                    foreach (var proofFilePath in _selectedProofFilePaths)
                    {
                        string savedProofName = PropertyCard.SavePropertyImage(proofFilePath, newProperty.Id);
                        if (!string.IsNullOrEmpty(savedProofName))
                        {
                            var proofFile = new PropertyProofFile
                            {
                                PropertyId = newProperty.Id,
                                FileName = Path.GetFileName(proofFilePath),
                                FilePath = Path.Combine("PropertyImages", savedProofName),
                                UploadDate = DateTime.Now,
                                FileSize = new FileInfo(proofFilePath).Length
                            };
                            _viewModel.AddPropertyProofFile(proofFile);
                        }
                    }
                }

                // Show appropriate success message based on user role
                if (currentUser != null && currentUser.Role == UserRole.Client)
                {
                    MessageBox.Show(
                        "Your property has been successfully submitted for approval!\n\n" +
                        "Our team will review your submission and you will be notified once it's approved.",
                        "Property Submitted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Property added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        // Helper: set the first writable string property found from a set of candidate names
        private void TrySetStringProperty(object target, string[] candidateNames, string value)
        {
            if (target == null || candidateNames == null) return;
            var t = target.GetType();
            foreach (var name in candidateNames)
            {
                var pi = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (pi != null && pi.CanWrite && pi.PropertyType == typeof(string))
                {
                    pi.SetValue(target, value);
                    return;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pictureBoxPreview?.Image?.Dispose();
                // Dispose images in proof file cards
                if (proofFilesPanel != null)
                {
                    foreach (Control control in proofFilesPanel.Controls)
                    {
                        if (control is Panel panel)
                        {
                            foreach (Control child in panel.Controls)
                            {
                                if (child is PictureBox pb && pb.Image != null)
                                {
                                    pb.Image.Dispose();
                                }
                            }
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }

        private void CmbPropertyType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Disable Floor Area field when Property Type is "Raw Land"
            bool isRawLand = cmbPropertyType?.SelectedItem?.ToString() == "Raw Land";
            if (numFloorAreaSqft != null)
                numFloorAreaSqft.Enabled = !isRawLand;
            if (lblFloorArea != null)
                lblFloorArea.Enabled = !isRawLand;

            if (isRawLand && numFloorAreaSqft != null)
            {
                numFloorAreaSqft.Value = 0;
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }
    }
}