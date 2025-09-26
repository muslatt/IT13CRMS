using RealEstateCRMWinForms.Models;
using System.Globalization;
using RealEstateCRMWinForms.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace RealEstateCRMWinForms.Views
{
    public partial class PropertyDetailsForm : Form
    {
        private Property _property;
        private PictureBox pictureBoxMain;
        private Label lblTitle;
        private Label lblAddress;
        private Label lblPrice;
        private Label lblStatus;
        private Label lblBedrooms;
        private Label lblBathrooms;
        private Label lblSquareMeters;
        private Label lblSQFT;
        private Label lblPricePerSqft;
        private Label lblListingDate;
        private Label lblPropertyType;
        private Label lblTransactionType;
        private Label lblAgent;
        private Label lblDescription;
        private TableLayoutPanel detailsGrid;
        private Button btnClose;
        private Button btnEdit;
        private bool _propertyWasModified = false;

        // Add this event to notify when property is updated
        public event EventHandler<PropertyEventArgs> PropertyUpdated;

        public PropertyDetailsForm(Property property)
        {
            _property = property;
            InitializeComponent();
            LoadPropertyDetails();
        }

        private void InitializeComponent()
        {
            Text = "Property Details";
            Font = new Font("Segoe UI", 12F);

            // Use ClientSize so controls position relative to client area and avoid chrome clipping
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(920, 740);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Padding = new Padding(12);

            // Main image
            pictureBoxMain = new PictureBox
            {
                Location = new Point(20, 20),
                Size = new Size(400, 300),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.LightGray,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            // Title
            lblTitle = new Label
            {
                Location = new Point(440, 20),
                Size = new Size(ClientSize.Width - 460, 36),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Property Title",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Address
            lblAddress = new Label
            {
                Location = new Point(440, 65),
                Size = new Size(ClientSize.Width - 460, 24),
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.FromArgb(108, 117, 125),
                Text = "Property Address",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Price
            lblPrice = new Label
            {
                Location = new Point(440, 100),
                Size = new Size(ClientSize.Width - 460, 32),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Text = "? 0",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Status
            lblStatus = new Label
            {
                Location = new Point(440, 140),
                Size = new Size(100, 28),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "For Sale",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Property details header
            var lblDetailsHeader = new Label
            {
                Text = "Property Details",
                Location = new Point(20, 340),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblBedrooms = new Label
            {
                Location = new Point(20, 380),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 12F),
            Text = "?? Bedrooms: 0",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblBathrooms = new Label
            {
                Location = new Point(240, 380),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 12F),
            Text = "?? Bathrooms: 0",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblSquareMeters = new Label
            {
                Location = new Point(460, 380),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 12F),
            Text = "?? Area: 0 sqm",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblSQFT = new Label
            {
                Location = new Point(20, 415),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 12F),
            Text = "?? SQFT: 0",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblPricePerSqft = new Label
            {
                Location = new Point(240, 415),
                Size = new Size(300, 24),
                Font = new Font("Segoe UI", 12F),
            Text = "?? Price per SQFT: ? 0",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblListingDate = new Label
            {
                Location = new Point(20, 450),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 12F),
            Text = "?? Listed: ",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblPropertyType = new Label
            {
                Location = new Point(240, 450),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 12F),
            Text = "?? Type: ",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblTransactionType = new Label
            {
                Location = new Point(460, 450),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 12F),
            Text = "?? Transaction: ",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblAgent = new Label
            {
                Location = new Point(20, 485),
                Size = new Size(ClientSize.Width - 40, 24),
                Font = new Font("Segoe UI", 12F),
                Text = "👤 Agent: ",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            var lblDescriptionHeader = new Label
            {
                Text = "Description",
                Location = new Point(20, 520),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            lblDescription = new Label
            {
                Location = new Point(20, 555),
                Size = new Size(ClientSize.Width - 40, 80), // responsive width
                Font = new Font("Segoe UI", 12F),
                Text = "No description available.",
                AutoSize = false,
                AutoEllipsis = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Buttons: position relative to client area and anchor to bottom-right
            btnClose = new Button
            {
                Text = "Close",
                Size = new Size(110, 40),
                Font = new Font("Segoe UI", 12F),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += BtnClose_Click;

            btnEdit = new Button
            {
                Text = "Edit Property",
                Size = new Size(130, 40),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;

            // compute button positions using client area so they're never clipped
            int bottomMargin = 16;
            int btnSpacing = 12;
            int btnEditX = ClientSize.Width - Padding.Right - btnEdit.Width - 12;
            int btnCloseX = btnEditX - btnSpacing - btnClose.Width;
            int btnY = ClientSize.Height - Padding.Bottom - btnEdit.Height - bottomMargin;

            btnClose.Location = new Point(btnCloseX, btnY);
            btnEdit.Location = new Point(btnEditX, btnY);

            // Build details grid (replaces manual absolute labels)
            detailsGrid = new TableLayoutPanel
            {
                Location = new Point(20, 372),
                Size = new Size(ClientSize.Width - 40, 120),
                ColumnCount = 3,
                RowCount = 3,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            detailsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            detailsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            detailsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));
            detailsGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            detailsGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            detailsGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var infoFont = new Font("Segoe UI", 12F);
            lblBedrooms = new Label { AutoSize = true, Font = infoFont, Text = "Bedrooms: -", Dock = DockStyle.Fill, Margin = new Padding(3, 6, 3, 6) };
            lblBathrooms = new Label { AutoSize = true, Font = infoFont, Text = "Bathrooms: -", Dock = DockStyle.Fill, Margin = new Padding(3, 6, 3, 6) };
            lblSquareMeters = new Label { AutoSize = true, Font = infoFont, Text = "Area: - sqm", Dock = DockStyle.Fill, Margin = new Padding(3, 6, 3, 6) };
            lblSQFT = new Label { AutoSize = true, Font = infoFont, Text = "SQFT: -", Dock = DockStyle.Fill, Margin = new Padding(3, 6, 3, 6) };
            lblPricePerSqft = new Label { AutoSize = true, Font = infoFont, Text = "Price per SQFT: -", Dock = DockStyle.Fill, Margin = new Padding(3, 6, 3, 6) };
            lblListingDate = new Label { AutoSize = true, Font = infoFont, Text = "Listed: -", Dock = DockStyle.Fill, Margin = new Padding(3, 6, 3, 6) };
            lblPropertyType = new Label { AutoSize = true, Font = infoFont, Text = "Type: -", Dock = DockStyle.Fill, Margin = new Padding(3, 6, 3, 6) };
            lblTransactionType = new Label { AutoSize = true, Font = infoFont, Text = "Transaction: -", Dock = DockStyle.Fill, Margin = new Padding(3, 6, 3, 6) };
            lblAgent = new Label { AutoSize = true, Font = infoFont, Text = "Agent: -", Dock = DockStyle.Fill, Margin = new Padding(3, 6, 3, 6) };

            detailsGrid.Controls.Add(lblBedrooms, 0, 0);
            detailsGrid.Controls.Add(lblBathrooms, 1, 0);
            detailsGrid.Controls.Add(lblSquareMeters, 2, 0);
            detailsGrid.Controls.Add(lblSQFT, 0, 1);
            detailsGrid.Controls.Add(lblPricePerSqft, 1, 1);
            detailsGrid.Controls.Add(lblListingDate, 2, 1);
            detailsGrid.Controls.Add(lblPropertyType, 0, 2);
            detailsGrid.Controls.Add(lblTransactionType, 1, 2);
            detailsGrid.Controls.Add(lblAgent, 2, 2);

            // Add all controls (use grid instead of individual detail labels)
            Controls.AddRange(new Control[] {
                pictureBoxMain, lblTitle, lblAddress, lblPrice, lblStatus,
                lblDetailsHeader, detailsGrid,
                lblDescriptionHeader, lblDescription,
                btnClose, btnEdit
            });

            CancelButton = btnClose;
            AcceptButton = btnEdit;
        }

        private void LoadPropertyDetails()
        {
            if (_property == null) return;

            // Basic info
            lblTitle.Text = _property.Title;
            lblAddress.Text = _property.Address;
            lblPrice.Text = _property.Price.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-PH"));
            lblStatus.Text = _property.Status;
            
            // Set status color
            lblStatus.BackColor = _property.Status == "Rent" 
                ? Color.FromArgb(108, 117, 125) 
                : Color.FromArgb(0, 123, 255);

            // Property details
            var bedWord = _property.Bedrooms == 1 ? "Bedroom" : "Bedrooms";
            var bathWord = _property.Bathrooms == 1 ? "Bathroom" : "Bathrooms";
            lblBedrooms.Text = $"{bedWord}: {_property.Bedrooms}";
            lblBathrooms.Text = $"{bathWord}: {_property.Bathrooms}";
            lblSquareMeters.Text = $"Area: {_property.SquareMeters} sqm";

            // Calculate SQFT and Price per SQFT
            decimal sqft = _property.SquareMeters * 10.7639m;
            if (sqft > 0)
            {
                decimal pricePerSqft = _property.Price / sqft;
                lblSQFT.Text = $"SQFT: {sqft:N2}";
                lblPricePerSqft.Text = $"Price per SQFT: {pricePerSqft.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("en-PH"))}";
            }
            else
            {
                lblSQFT.Text = "SQFT: N/A";
                lblPricePerSqft.Text = "Price per SQFT: N/A";
            }

            lblListingDate.Text = $"Listed: {_property.CreatedAt:MMM dd, yyyy}";

            // Try to get additional properties via reflection (safe if they don't exist)
            lblPropertyType.Text = $"Type: {GetPropertyValue("PropertyType") ?? GetPropertyValue("Type") ?? "Not specified"}";
            lblTransactionType.Text = $"Transaction: {GetPropertyValue("TransactionType") ?? GetPropertyValue("ListingType") ?? "Not specified"}";
            lblAgent.Text = $"Agent: {GetPropertyValue("Agent") ?? GetPropertyValue("AgentName") ?? "Not specified"}";
            lblDescription.Text = GetPropertyValue("Description") ?? GetPropertyValue("Notes") ?? "No description available.";

            // Load image
            LoadPropertyImage();
        }

        private string? GetPropertyValue(string propertyName)
        {
            try
            {
                var property = _property.GetType().GetProperty(propertyName);
                return property?.GetValue(_property)?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private void LoadPropertyImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_property?.ImagePath))
                {
                    string imagePath = GetPropertyImagePath(_property.ImagePath);
                    if (File.Exists(imagePath))
                    {
                        using (var img = Image.FromFile(imagePath))
                        {
                            pictureBoxMain.Image?.Dispose();
                            pictureBoxMain.Image = new Bitmap(img);
                        }
                        return;
                    }
                }
                
                SetDefaultImage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading property image: {ex.Message}");
                SetDefaultImage();
            }
        }

        private void SetDefaultImage()
        {
            pictureBoxMain.Image?.Dispose();
            
            var defaultBitmap = new Bitmap(400, 300);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.LightGray);
                using (var brush = new SolidBrush(Color.Gray))
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                {
                    string text = "No Image Available";
                    var textSize = g.MeasureString(text, font);
                    var x = (defaultBitmap.Width - textSize.Width) / 2;
                    var y = (defaultBitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            pictureBoxMain.Image = defaultBitmap;
        }

        private string GetPropertyImagePath(string imagePath)
        {
            string imagesDir = Path.Combine(Application.StartupPath, "PropertyImages");
            return Path.Combine(imagesDir, imagePath);
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            var editForm = new EditPropertyForm(_property);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Refresh the property from database to get latest data
                RefreshPropertyFromDatabase();
                
                // Refresh the details display
                LoadPropertyDetails();
                _propertyWasModified = true;
                
                // Notify any listeners (like PropertyCard) that the property was updated
                PropertyUpdated?.Invoke(this, new PropertyEventArgs(_property));
            }
        }

        private void RefreshPropertyFromDatabase()
        {
            if (_property == null) return;

            try
            {
                var viewModel = new ViewModels.PropertyViewModel();
                var refreshedProperty = viewModel.GetPropertyById(_property.Id);
                
                if (refreshedProperty != null)
                {
                    _property = refreshedProperty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing property from database: {ex.Message}");
            }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            DialogResult = _propertyWasModified ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pictureBoxMain?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}


