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

        private TextBox? txtTitle;
        private TextBox? txtAddress;
        private NumericUpDown? numPrice;
        private NumericUpDown? numBedrooms;
        private NumericUpDown? numBathrooms;
        private NumericUpDown? numSquareMeters;
        private ComboBox? cmbStatus;
        // New controls
        private NumericUpDown? numLotArea;
        private NumericUpDown? numFloorArea;
        private ComboBox? cmbPropertyType;
        private ComboBox? cmbTransaction;
        private DateTimePicker? dtpListed;
        private PictureBox? pictureBoxPreview;
        private Button? btnSelectImage;
        private Button? btnSave;
        private Button? btnCancel;
        private string? _selectedImagePath;

        // Added description textbox
        private TextBox? txtDescription;

        // Proof of ownership fields
        private FlowLayoutPanel? proofFilesPanel;
        private Button? btnSelectProofFiles;
        private Button? btnCancelProofFile;
        private List<string> _selectedProofFilePaths;
        private List<int> _proofFilesToDelete; // Track proof files marked for deletion

        public EditPropertyForm(Property property)
        {
            _viewModel = new PropertyViewModel();
            _property = property;
            _selectedProofFilePaths = new List<string>();
            _proofFilesToDelete = new List<int>();

            // Refresh property from database to ensure ProofFiles are loaded
            var refreshedProperty = _viewModel.GetPropertyById(property.Id);
            if (refreshedProperty != null)
            {
                _property = refreshedProperty;
            }

            InitializeComponent();
            PopulateFields();
        }

        private void InitializeComponent()
        {
            Text = "Edit Property";
            Size = new Size(900, 700);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.FromArgb(248, 249, 250);

            // Main container with padding
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Create main layout - left side for form, right side for image
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Left panel for form fields
            var leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(25),
                Margin = new Padding(0, 0, 10, 0)
            };
            leftPanel.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(222, 226, 230)), 0, 0, leftPanel.Width - 1, leftPanel.Height - 1);
            };

            // Right panel for image
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
                Margin = new Padding(10, 0, 0, 0)
            };
            rightPanel.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(222, 226, 230)), 0, 0, rightPanel.Width - 1, rightPanel.Height - 1);
            };

            // Form fields layout
            var formLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 11,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 60) // Space for buttons
            };

            // Column styles
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Row styles - consistent spacing
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Title
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Address
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Price
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F)); // Bedrooms/Bathrooms
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F)); // Lot/Floor Areas
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Property Type
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Transaction
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Listed Date
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F)); // Description
            formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Spacer
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Buttons

            int row = 0;

            // Title
            var lblTitle = CreateLabel("Title:");
            txtTitle = CreateTextBox();
            formLayout.Controls.Add(lblTitle, 0, row);
            formLayout.Controls.Add(txtTitle, 1, row++);

            // Address
            var lblAddress = CreateLabel("Address:");
            txtAddress = CreateTextBox();
            formLayout.Controls.Add(lblAddress, 0, row);
            formLayout.Controls.Add(txtAddress, 1, row++);

            // Price
            var lblPrice = CreateLabel("Price:");
            numPrice = new NumericUpDown
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Maximum = 999999999,
                DecimalPlaces = 0,
                ThousandsSeparator = true,
                Font = new Font("Segoe UI", 9F),
                Height = 25
            };
            formLayout.Controls.Add(lblPrice, 0, row);
            formLayout.Controls.Add(numPrice, 1, row++);

            // Bedrooms and Bathrooms row
            var lblBeds = CreateLabel("Beds:");
            var bedsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                WrapContents = false,
                Margin = new Padding(0)
            };

            numBedrooms = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 20,
                Width = 60,
                Font = new Font("Segoe UI", 9F)
            };

            var lblBaths = new Label
            {
                Text = "Baths:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(73, 80, 87),
                Margin = new Padding(20, 3, 5, 0)
            };

            numBathrooms = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 20,
                Width = 60,
                Font = new Font("Segoe UI", 9F)
            };

            bedsPanel.Controls.Add(numBedrooms);
            bedsPanel.Controls.Add(lblBaths);
            bedsPanel.Controls.Add(numBathrooms);

            formLayout.Controls.Add(lblBeds, 0, row);
            formLayout.Controls.Add(bedsPanel, 1, row++);

            // Lot Area and Floor Area row
            var lblLot = CreateLabel("Lot (Sqm):");
            var areasPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                WrapContents = false,
                Margin = new Padding(0)
            };

            numLotArea = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                Width = 80,
                Font = new Font("Segoe UI", 9F)
            };

            var lblFloor = new Label
            {
                Text = "Floor (Sqm):",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(73, 80, 87),
                Margin = new Padding(15, 3, 5, 0)
            };

            numFloorArea = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                Width = 80,
                Font = new Font("Segoe UI", 9F)
            };

            areasPanel.Controls.Add(numLotArea);
            areasPanel.Controls.Add(lblFloor);
            areasPanel.Controls.Add(numFloorArea);

            formLayout.Controls.Add(lblLot, 0, row);
            formLayout.Controls.Add(areasPanel, 1, row++);

            // Property Type
            var lblPropertyType = CreateLabel("Property Type:");
            cmbPropertyType = CreateComboBox();
            cmbPropertyType.Items.AddRange(new[] { "Residential", "Commercial", "Raw Land" });
            formLayout.Controls.Add(lblPropertyType, 0, row);
            formLayout.Controls.Add(cmbPropertyType, 1, row++);

            // Transaction Type
            var lblTransaction = CreateLabel("Transaction:");
            cmbTransaction = CreateComboBox();
            cmbTransaction.Items.AddRange(new[] { "Buying", "Viewing" });
            formLayout.Controls.Add(lblTransaction, 0, row);
            formLayout.Controls.Add(cmbTransaction, 1, row++);

            // Listed Date
            var lblListed = CreateLabel("Listed Date:");
            dtpListed = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F),
                Height = 25
            };
            formLayout.Controls.Add(lblListed, 0, row);
            formLayout.Controls.Add(dtpListed, 1, row++);

            // Description
            var lblDescription = CreateLabel("Description:");
            lblDescription.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            txtDescription = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle
            };
            formLayout.Controls.Add(lblDescription, 0, row);
            formLayout.Controls.Add(txtDescription, 1, row++);

            // Skip spacer row
            row++;

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Anchor = AnchorStyles.Right,
                AutoSize = true,
                Margin = new Padding(0)
            };

            btnSave = new Button
            {
                Text = "Save Changes",
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 0, 0, 0),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            buttonsPanel.Controls.Add(btnSave);
            buttonsPanel.Controls.Add(btnCancel);

            formLayout.Controls.Add(new Label(), 0, row); // Empty cell
            formLayout.Controls.Add(buttonsPanel, 1, row);

            leftPanel.Controls.Add(formLayout);

            // Image panel setup
            var imageLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                BackColor = Color.Transparent
            };
            imageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));    // Image label
            imageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));     // Image preview
            imageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));    // Change image button
            imageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));    // Proof label
            imageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 15F));    // Proof subtext
            imageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));     // Proof files panel
            imageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));    // Attach button (moved to bottom)

            var lblImage = new Label
            {
                Text = "Property Image",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(73, 80, 87),
                Anchor = AnchorStyles.Left,
                AutoSize = true
            };

            pictureBoxPreview = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                Margin = new Padding(0, 5, 0, 5)
            };

            btnSelectImage = new Button
            {
                Text = "Change Image",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };
            btnSelectImage.FlatAppearance.BorderSize = 0;
            btnSelectImage.Click += BtnSelectImage_Click;

            imageLayout.Controls.Add(lblImage, 0, 0);
            imageLayout.Controls.Add(pictureBoxPreview, 0, 1);
            imageLayout.Controls.Add(btnSelectImage, 0, 2);

            // Proof of ownership section
            var lblProof = new Label
            {
                Text = "Proof of Ownership",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(73, 80, 87),
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            };

            var lblProofSubtext = new Label
            {
                Text = "You can attach multiple files",
                Font = new Font("Segoe UI", 7F, FontStyle.Italic),
                ForeColor = Color.FromArgb(108, 117, 125),
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 0)
            };

            // Container panel for proof files (top bar now only holds Clear)
            var proofContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            var proofTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = Color.Transparent
            };

            btnCancelProofFile = new Button
            {
                Text = "Clear",
                Dock = DockStyle.Right,
                Width = 70,
                Height = 28,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Visible = false,
                Margin = new Padding(0, 2, 0, 0)
            };
            btnCancelProofFile.FlatAppearance.BorderSize = 0;
            btnCancelProofFile.Click += (s, e) => ClearAllProofFiles();
            proofTopBar.Controls.Add(btnCancelProofFile);

            proofFilesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Margin = new Padding(0, 0, 0, 5)
            };

            proofContainer.Controls.Add(proofFilesPanel);
            proofContainer.Controls.Add(proofTopBar);

            imageLayout.Controls.Add(lblProof, 0, 3);
            imageLayout.Controls.Add(lblProofSubtext, 0, 4);
            imageLayout.Controls.Add(proofContainer, 0, 5);

            // Bottom full-width Attach button (moved from top bar)
            btnSelectProofFiles = new Button
            {
                Text = "Attach",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 5, 0, 0)
            };
            btnSelectProofFiles.FlatAppearance.BorderSize = 0;
            btnSelectProofFiles.Click += BtnSelectProofFiles_Click;
            imageLayout.Controls.Add(btnSelectProofFiles, 0, 6);

            rightPanel.Controls.Add(imageLayout);

            // Add panels to main layout
            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(rightPanel, 1, 0);

            mainPanel.Controls.Add(mainLayout);
            Controls.Add(mainPanel);

            // Form settings
            CancelButton = btnCancel;
            AcceptButton = btnSave;
            MinimumSize = new Size(850, 650);
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(73, 80, 87),
                Margin = new Padding(0, 5, 0, 0)
            };
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle,
                Height = 25
            };
        }

        private ComboBox CreateComboBox()
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F),
                Height = 25
            };
        }

        private void PopulateFields()
        {
            if (_property == null) return;

            txtTitle!.Text = _property.Title;
            txtAddress!.Text = _property.Address;

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
            numBedrooms!.Value = Math.Min(Math.Max(_property.Bedrooms, (int)numBedrooms.Minimum), (int)numBedrooms.Maximum);
            numBathrooms!.Value = Math.Min(Math.Max(_property.Bathrooms, (int)numBathrooms.Minimum), (int)numBathrooms.Maximum);

            // Populate lot and floor area
            if (numLotArea != null)
            {
                try { numLotArea.Value = (decimal)Math.Min(Math.Max((double)_property.LotAreaSqm, (double)numLotArea.Minimum), (double)numLotArea.Maximum); }
                catch { numLotArea.Value = 0; }
            }
            if (numFloorArea != null)
            {
                try { numFloorArea.Value = (decimal)Math.Min(Math.Max((double)_property.FloorAreaSqm, (double)numFloorArea.Minimum), (double)numFloorArea.Maximum); }
                catch { numFloorArea.Value = 0; }
            }

            // Populate property type / transaction / listed date
            if (cmbPropertyType != null)
                cmbPropertyType.SelectedItem = string.IsNullOrWhiteSpace(_property.PropertyType) ? (object?)null : _property.PropertyType;
            if (cmbTransaction != null)
                cmbTransaction.SelectedItem = string.IsNullOrWhiteSpace(_property.TransactionType) ? (object?)null : _property.TransactionType;
            if (dtpListed != null)
                dtpListed.Value = _property.CreatedAt == default ? DateTime.Now : _property.CreatedAt;

            // Populate description field
            txtDescription!.Text = _property.Description;

            // Load existing image
            LoadCurrentImage();

            // Load existing proof files
            LoadExistingProofFiles();
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
                            pictureBoxPreview!.Image?.Dispose();
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
            pictureBoxPreview!.Image?.Dispose();
            var defaultBitmap = new Bitmap(240, 160);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.FromArgb(248, 249, 250));
                using (var brush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
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
                            pictureBoxPreview!.Image?.Dispose();
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

        private void LoadExistingProofFiles()
        {
            try
            {
                proofFilesPanel!.Controls.Clear();

                if (_property.ProofFiles != null && _property.ProofFiles.Any())
                {
                    foreach (var proofFile in _property.ProofFiles.OrderByDescending(f => f.UploadDate))
                    {
                        var fileCard = CreateExistingProofFileCard(proofFile);
                        proofFilesPanel.Controls.Add(fileCard);
                    }

                    btnCancelProofFile!.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading existing proof files: {ex.Message}");
            }
        }

        private void BtnSelectProofFiles_Click(object? sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Proof of Ownership Files";
                openFileDialog.Filter = "All Files|*.*|Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|PDF Files|*.pdf|Documents|*.doc;*.docx";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        int addedCount = 0;
                        int duplicateCount = 0;

                        // Add file cards for new files (skip duplicates)
                        foreach (var filePath in openFileDialog.FileNames)
                        {
                            // Check if file is already in the list
                            if (_selectedProofFilePaths.Contains(filePath))
                            {
                                duplicateCount++;
                                continue;
                            }

                            // Add to list and create card
                            _selectedProofFilePaths.Add(filePath);
                            var fileCard = CreateNewProofFileCard(filePath);
                            proofFilesPanel!.Controls.Add(fileCard);
                            addedCount++;
                        }

                        // Show cancel button if there are files
                        if (proofFilesPanel!.Controls.Count > 0)
                        {
                            btnCancelProofFile!.Visible = true;
                        }

                        // Show feedback message
                        if (addedCount > 0 && duplicateCount > 0)
                        {
                            MessageBox.Show($"Added {addedCount} file(s). {duplicateCount} duplicate(s) skipped.",
                                "Files Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (duplicateCount > 0)
                        {
                            MessageBox.Show($"All {duplicateCount} file(s) are already attached.",
                                "Duplicate Files", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading proof files: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private Control CreateExistingProofFileCard(PropertyProofFile proofFile)
        {
            var cardPanel = new Panel
            {
                Size = new Size(110, 85),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(2)
            };

            // File icon
            var iconLabel = new Label
            {
                Text = "📄",
                Font = new Font("Segoe UI", 14F),
                Location = new Point(42, 5),
                Size = new Size(25, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // File name (truncated)
            var fileName = proofFile.FileName.Length > 13
                ? proofFile.FileName.Substring(0, 10) + "..."
                : proofFile.FileName;
            var nameLabel = new Label
            {
                Text = fileName,
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                Location = new Point(3, 32),
                Size = new Size(104, 12),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            // File size
            var sizeLabel = new Label
            {
                Text = FormatFileSize(proofFile.FileSize),
                Font = new Font("Segoe UI", 6F),
                Location = new Point(3, 45),
                Size = new Size(104, 10),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            // Status label (existing file)
            var statusLabel = new Label
            {
                Text = "Existing",
                Font = new Font("Segoe UI", 6F, FontStyle.Italic),
                Location = new Point(3, 56),
                Size = new Size(104, 10),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(40, 167, 69)
            };

            // Remove button
            var btnRemove = new Button
            {
                Text = "✕",
                Size = new Size(18, 18),
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(88, 3)
            };
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Tag = proofFile;
            btnRemove.Click += (s, e) => RemoveExistingProofFile(proofFile, cardPanel);

            cardPanel.Controls.AddRange(new Control[] { iconLabel, nameLabel, sizeLabel, statusLabel, btnRemove });

            return cardPanel;
        }

        private Control CreateNewProofFileCard(string filePath)
        {
            var cardPanel = new Panel
            {
                Size = new Size(110, 85),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(2)
            };

            // File icon
            var iconLabel = new Label
            {
                Text = "📄",
                Font = new Font("Segoe UI", 14F),
                Location = new Point(42, 5),
                Size = new Size(25, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // File name (truncated)
            var fileName = Path.GetFileName(filePath);
            if (fileName.Length > 13)
                fileName = fileName.Substring(0, 10) + "...";
            var nameLabel = new Label
            {
                Text = fileName,
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                Location = new Point(3, 32),
                Size = new Size(104, 12),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            // File size
            var fileInfo = new FileInfo(filePath);
            var sizeLabel = new Label
            {
                Text = FormatFileSize(fileInfo.Length),
                Font = new Font("Segoe UI", 6F),
                Location = new Point(3, 45),
                Size = new Size(104, 10),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            // Status label (new file)
            var statusLabel = new Label
            {
                Text = "New",
                Font = new Font("Segoe UI", 6F, FontStyle.Italic),
                Location = new Point(3, 56),
                Size = new Size(104, 10),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            // Remove button
            var btnRemove = new Button
            {
                Text = "✕",
                Size = new Size(18, 18),
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(88, 3)
            };
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Tag = filePath;
            btnRemove.Click += (s, e) => RemoveNewProofFile(filePath, cardPanel);

            cardPanel.Controls.AddRange(new Control[] { iconLabel, nameLabel, sizeLabel, statusLabel, btnRemove });

            return cardPanel;
        }

        private void RemoveExistingProofFile(PropertyProofFile proofFile, Panel cardPanel)
        {
            var confirmResult = MessageBox.Show(
                $"Are you sure you want to remove '{proofFile.FileName}'?\nThis file will be deleted when you save.",
                "Remove Proof File",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                _proofFilesToDelete.Add(proofFile.Id);
                proofFilesPanel!.Controls.Remove(cardPanel);

                // Hide cancel button if no files remain
                if (proofFilesPanel.Controls.Count == 0)
                {
                    btnCancelProofFile!.Visible = false;
                }
            }
        }

        private void RemoveNewProofFile(string filePath, Panel cardPanel)
        {
            _selectedProofFilePaths.Remove(filePath);
            proofFilesPanel!.Controls.Remove(cardPanel);

            // Hide cancel button if no files remain
            if (proofFilesPanel.Controls.Count == 0)
            {
                btnCancelProofFile!.Visible = false;
            }
        }

        private void ClearAllProofFiles()
        {
            var confirmResult = MessageBox.Show(
                "Are you sure you want to remove all proof files?\nExisting files will be deleted when you save.",
                "Clear All Proof Files",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                // Mark all existing files for deletion
                if (_property.ProofFiles != null)
                {
                    foreach (var proofFile in _property.ProofFiles)
                    {
                        if (!_proofFilesToDelete.Contains(proofFile.Id))
                        {
                            _proofFilesToDelete.Add(proofFile.Id);
                        }
                    }
                }

                // Clear new files
                _selectedProofFilePaths.Clear();

                // Clear UI
                proofFilesPanel!.Controls.Clear();
                btnCancelProofFile!.Visible = false;
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtTitle!.Text))
            {
                MessageBox.Show("Please enter a property title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddress!.Text))
            {
                MessageBox.Show("Please enter a property address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return;
            }

            if (numPrice!.Value <= 0)
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numPrice.Focus();
                return;
            }

            // Update property object
            _property.Title = txtTitle!.Text.Trim();
            _property.Address = txtAddress!.Text.Trim();
            if (numPrice != null) _property.Price = numPrice.Value;
            if (numBedrooms != null) _property.Bedrooms = (int)numBedrooms.Value;
            if (numBathrooms != null) _property.Bathrooms = (int)numBathrooms.Value;
            if (numLotArea != null) _property.LotAreaSqm = numLotArea.Value;
            if (numFloorArea != null) _property.FloorAreaSqm = numFloorArea.Value;
            if (cmbPropertyType != null) _property.PropertyType = cmbPropertyType.SelectedItem?.ToString() ?? string.Empty;
            if (cmbTransaction != null) _property.TransactionType = cmbTransaction.SelectedItem?.ToString() ?? string.Empty;
            if (dtpListed != null) _property.CreatedAt = dtpListed.Value;

            // Update description
            _property.Description = txtDescription!.Text?.Trim() ?? string.Empty;

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
                // Handle proof file deletions
                if (_proofFilesToDelete.Count > 0)
                {
                    using var db = Data.DbContextHelper.CreateDbContext();
                    foreach (var proofFileId in _proofFilesToDelete)
                    {
                        var proofFileToDelete = db.PropertyProofFiles.FirstOrDefault(pf => pf.Id == proofFileId);
                        if (proofFileToDelete != null)
                        {
                            // Delete the physical file if it exists
                            if (File.Exists(proofFileToDelete.FilePath))
                            {
                                try
                                {
                                    File.Delete(proofFileToDelete.FilePath);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error deleting proof file: {ex.Message}");
                                }
                            }

                            // Remove from database
                            db.PropertyProofFiles.Remove(proofFileToDelete);
                        }
                    }
                    db.SaveChanges();
                }

                // Save new proof files
                if (_selectedProofFilePaths != null && _selectedProofFilePaths.Count > 0)
                {
                    foreach (var proofFilePath in _selectedProofFilePaths)
                    {
                        string savedProofName = PropertyCard.SavePropertyImage(proofFilePath, _property.Id);
                        if (!string.IsNullOrEmpty(savedProofName))
                        {
                            var proofFile = new PropertyProofFile
                            {
                                PropertyId = _property.Id,
                                FileName = Path.GetFileName(proofFilePath),
                                FilePath = Path.Combine("PropertyImages", savedProofName),
                                UploadDate = DateTime.Now,
                                FileSize = new FileInfo(proofFilePath).Length
                            };
                            _viewModel.AddPropertyProofFile(proofFile);
                        }
                    }
                }

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