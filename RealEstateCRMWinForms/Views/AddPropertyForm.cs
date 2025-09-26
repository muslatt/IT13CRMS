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
        
        private TextBox txtTitle;
        private TextBox txtAddress;
        private NumericUpDown numPrice;
        private NumericUpDown numBedrooms;
        private NumericUpDown numBathrooms;
        private NumericUpDown numSquareMeters;
        private ComboBox cmbStatus;
        private ComboBox cmbPropertyType;
        private ComboBox cmbTransactionType;
        private ComboBox cmbAgent;
        private TextBox txtDescription;
        private PictureBox pictureBoxPreview;
        private Button btnSelectImage;
        private Button btnSave;
        private Button btnCancel;
        private string _selectedImagePath;

        // New controls for calculated values
        private Label lblCalculatedHeader;
        private Label lblSQFTLabel;
        private Label lblSQFTValue;
        private Label lblPricePerSqftLabel;
        private Label lblPricePerSqftValue;

        // Listing date control
        private DateTimePicker dtpListingDate;

        public AddPropertyForm()
        {
            _viewModel = new PropertyViewModel();
            InitializeComponent();
        }

        private bool AgentHasCapacity(string agentDisplayName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(agentDisplayName)) return true; // no agent assignment

                var dealVm = new DealViewModel();
                dealVm.LoadDeals();
                _viewModel.LoadProperties();

                string name = agentDisplayName.Trim();
                // All properties currently assigned to this agent (active)
                var assignedProps = _viewModel.Properties
                    .Where(p => p.IsActive && !string.IsNullOrWhiteSpace(p.Agent) && string.Equals(p.Agent.Trim(), name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                int activeAssignments = 0;
                foreach (var p in assignedProps)
                {
                    // If there is a closed deal for this property, it no longer blocks capacity
                    bool hasClosed = dealVm.Deals.Any(d => d.IsActive && d.PropertyId == p.Id && string.Equals(d.Status, BoardViewModel.ClosedBoardName, StringComparison.OrdinalIgnoreCase));
                    if (!hasClosed) activeAssignments++;
                }

                return activeAssignments < 3; // max 3 concurrent assignments
            }
            catch
            {
                // In case of any error, don't block saving
                return true;
            }
        }

        private void InitializeComponent()
        {
            Text = "Add New Property";
            // Set the form font (size 12) so the form's displayed font is 12pt
            Font = new Font("Segoe UI", 12F);

            // Increased size and better spacing for less-clumpy layout
            Size = new Size(860, 820);
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
                Size = new Size(660, 28),
                Font = new Font("Segoe UI", 12F)
            };

            // Address
            var lblAddress = new Label
            {
                Text = "Address:",
                Location = new Point(20, 60),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            txtAddress = new TextBox
            {
                Location = new Point(160, 60),
                Size = new Size(660, 28),
                Font = new Font("Segoe UI", 12F)
            };

            // Price
            var lblPrice = new Label
            {
                Text = "Price:",
                Location = new Point(20, 100),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            numPrice = new NumericUpDown
            {
                Location = new Point(160, 100),
                Size = new Size(240, 28),
                Font = new Font("Segoe UI", 12F),
                Maximum = 999999999,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };
            numPrice.ValueChanged += OnAreaOrPriceChanged;

            // Bedrooms
            var lblBedrooms = new Label
            {
                Text = "Bedrooms:",
                Location = new Point(420, 100),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            numBedrooms = new NumericUpDown
            {
                Location = new Point(540, 100),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F),
                Minimum = 0,
                Maximum = 20
            };

            // Bathrooms
            var lblBathrooms = new Label
            {
                Text = "Bathrooms:",
                Location = new Point(20, 140),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            numBathrooms = new NumericUpDown
            {
                Location = new Point(160, 140),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F),
                Minimum = 0,
                Maximum = 20
            };

            // Square Meters
            var lblSquareMeters = new Label
            {
                Text = "Square Meters:",
                Location = new Point(300, 140),
                Size = new Size(140, 28),
                Font = new Font("Segoe UI", 12F)
            };

            numSquareMeters = new NumericUpDown
            {
                Location = new Point(450, 140),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F),
                Minimum = 1,
                Maximum = 10000
            };
            numSquareMeters.ValueChanged += OnAreaOrPriceChanged;

            // Calculated Values header and labels (after square meters)
            lblCalculatedHeader = new Label
            {
                Text = "Calculated Values",
                Location = new Point(20, 180),
                Size = new Size(200, 22),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            lblSQFTLabel = new Label
            {
                Text = "SQFT:",
                Location = new Point(30, 210),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 10F)
            };

            lblSQFTValue = new Label
            {
                Text = "-",
                Location = new Point(150, 210),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            lblPricePerSqftLabel = new Label
            {
                Text = "Price Per SQFT:",
                Location = new Point(340, 210),
                Size = new Size(140, 20),
                Font = new Font("Segoe UI", 10F)
            };

            lblPricePerSqftValue = new Label
            {
                Text = "-",
                Location = new Point(480, 210),
                Size = new Size(260, 20),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            // Status
            var lblStatus = new Label
            {
                Text = "Status:",
                Location = new Point(20, 250),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            cmbStatus = new ComboBox
            {
                Location = new Point(160, 250),
                Size = new Size(180, 28),
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new[] { "Sell", "Rent" });
            cmbStatus.SelectedIndex = 0;

            // Property Type
            var lblPropertyType = new Label
            {
                Text = "Property Type:",
                Location = new Point(360, 250),
                Size = new Size(140, 28),
                Font = new Font("Segoe UI", 12F)
            };

            cmbPropertyType = new ComboBox
            {
                Location = new Point(520, 250),
                Size = new Size(300, 28),
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPropertyType.Items.AddRange(new[] { "Residential", "Commercial", "Raw Land" });
            cmbPropertyType.SelectedIndex = 0;

            // Transaction Type (Buying, Viewing)
            var lblTransactionType = new Label
            {
                Text = "Type:",
                Location = new Point(20, 290),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            cmbTransactionType = new ComboBox
            {
                Location = new Point(160, 290),
                Size = new Size(180, 28),
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTransactionType.Items.AddRange(new[] { "Buying", "Viewing" });
            cmbTransactionType.SelectedIndex = 0;

            // Agent
            var lblAgent = new Label
            {
                Text = "Agent:",
                Location = new Point(360, 290),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            cmbAgent = new ComboBox
            {
                Location = new Point(520, 290),
                Size = new Size(300, 28),
                Font = new Font("Segoe UI", 12F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Populate Agents from Users table
            cmbAgent.Items.Add("(No Agent)");
            foreach (var name in Services.AgentDirectory.GetAgentDisplayNames())
                cmbAgent.Items.Add(name);
            cmbAgent.SelectedIndex = 0;

            // Listing Date
            var lblListingDate = new Label
            {
                Text = "Listing Date:",
                Location = new Point(20, 330),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            dtpListingDate = new DateTimePicker
            {
                Location = new Point(160, 330),
                Size = new Size(180, 28),
                Font = new Font("Segoe UI", 12F),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Description
            var lblDescription = new Label
            {
                Text = "Description:",
                Location = new Point(20, 370),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            txtDescription = new TextBox
            {
                Location = new Point(160, 370),
                Size = new Size(660, 120),
                Font = new Font("Segoe UI", 12F),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Image selection (moved down)
            var lblImage = new Label
            {
                Text = "Property Image:",
                Location = new Point(20, 480),
                Size = new Size(120, 28),
                Font = new Font("Segoe UI", 12F)
            };

            pictureBoxPreview = new PictureBox
            {
                Location = new Point(160, 480),
                Size = new Size(360, 200),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.LightGray
            };

            btnSelectImage = new Button
            {
                Text = "Select Image",
                Location = new Point(540, 520),
                Size = new Size(160, 40),
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
                Location = new Point(560, 720),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 12F),
                DialogResult = DialogResult.Cancel
            };

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(700, 720),
                Size = new Size(120, 40),
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
                lblPrice, numPrice, lblBedrooms, numBedrooms,
                lblBathrooms, numBathrooms,
                lblSquareMeters, numSquareMeters,
                lblCalculatedHeader, lblSQFTLabel, lblSQFTValue, lblPricePerSqftLabel, lblPricePerSqftValue,
                lblStatus, cmbStatus,
                lblPropertyType, cmbPropertyType,
                lblTransactionType, cmbTransactionType,
                lblAgent, cmbAgent,
                lblListingDate, dtpListingDate,
                lblDescription, txtDescription,
                lblImage, pictureBoxPreview, btnSelectImage,
                btnCancel, btnSave
            });

            CancelButton = btnCancel;
            AcceptButton = btnSave;

            // Set default image
            SetDefaultPreviewImage();

            // initialize calculated values
            UpdateCalculatedValues();
        }

        private void SetDefaultPreviewImage()
        {
            var defaultBitmap = new Bitmap(360, 200);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.LightGray);
                using (var brush = new SolidBrush(Color.Gray))
                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
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

            // Determine selected agent before creating the entity (for capacity check)
            var selectedAgent = cmbAgent.SelectedIndex > 0 ? (cmbAgent.SelectedItem?.ToString() ?? string.Empty) : string.Empty;

            // If assigning to an Agent, enforce capacity: max 3 active allocations until one is closed
            if (!string.IsNullOrWhiteSpace(selectedAgent))
            {
                if (!AgentHasCapacity(selectedAgent))
                {
                    MessageBox.Show($"{selectedAgent} has reached the maximum of 3 active property allocations. Assign to another agent or wait until one is closed.",
                        "Allocation Limit Reached",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
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
                CreatedAt = dtpListingDate?.Value ?? DateTime.Now,
                IsActive = true
            };

            // Try to set commonly named string properties on the Property model via reflection.
            TrySetStringProperty(newProperty, new[] { "PropertyType", "Type" }, cmbPropertyType.SelectedItem?.ToString() ?? "Residential");
            TrySetStringProperty(newProperty, new[] { "TransactionType", "Type", "ListingType", "ListingMode" }, cmbTransactionType.SelectedItem?.ToString() ?? "Buying");
            TrySetStringProperty(newProperty, new[] { "Agent", "AgentName", "AssignedAgent" }, selectedAgent);
            TrySetStringProperty(newProperty, new[] { "Description", "Notes", "Details" }, txtDescription.Text?.Trim() ?? string.Empty);

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
            }
            base.Dispose(disposing);
        }

        // Event handler when area (sqm) or price changes — update calculated values
        private void OnAreaOrPriceChanged(object? sender, EventArgs e)
        {
            UpdateCalculatedValues();
        }

        // Calculate SQFT and PricePerSQFT and update UI
        private void UpdateCalculatedValues()
        {
            // Area in square meters
            decimal area = numSquareMeters?.Value ?? 0m;
            decimal sqft = 0m;

            if (area > 0)
            {
                // 1 sqm = 10.7639 sqft
                sqft = area * 10.7639m;
                lblSQFTValue.Text = $"{sqft:N2} sqft";
            }
            else
            {
                lblSQFTValue.Text = "-";
            }

            // Price per sqft
            decimal price = numPrice?.Value ?? 0m;
            if (price > 0 && sqft > 0)
            {
                decimal ppsqft = price / sqft;
                lblPricePerSqftValue.Text = $"₱ {ppsqft:N2}";
            }
            else
            {
                lblPricePerSqftValue.Text = "-";
            }
        }
    }
}
