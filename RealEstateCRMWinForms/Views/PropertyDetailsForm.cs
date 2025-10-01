using RealEstateCRMWinForms.Models;
using System.Globalization;
using RealEstateCRMWinForms.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace RealEstateCRMWinForms.Views
{
    public partial class PropertyDetailsForm : Form
    {
        private Property _property;
        private PictureBox pictureBoxMain;
        private PictureBox pictureBoxProof;
        private FlowLayoutPanel proofFilesPanel;
        private Label lblTitle;
        private Label lblAddress;
        private Label lblPrice;
        private Panel statusPanel;
        private Label lblStatus;
        private Panel detailsPanel;
        private Panel descriptionPanel;
        private Panel proofPanel;
        private Label lblDescription;
        private Button btnClose;
        private Button btnEdit;
        private Button btnResubmit;
        private Button btnViewProof;
        private Button btnDownloadProof;
        private Button btnRequestInfo; // Inquire button for clients
        private bool _propertyWasModified = false;
        private bool _isClientBrowseMode = false;

        public event EventHandler<PropertyEventArgs> PropertyUpdated;

        public PropertyDetailsForm(Property property)
        {
            _property = property;
            _isClientBrowseMode = false;
            InitializeComponent();
            LoadPropertyDetails();
        }

        public PropertyDetailsForm(Property property, bool isClientBrowseMode)
        {
            _property = property;
            _isClientBrowseMode = isClientBrowseMode;
            InitializeComponent();
            LoadPropertyDetails();
        }

        private void InitializeComponent()
        {
            Text = "Property Details";
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(248, 249, 250);

            // Compact form size to ensure buttons are visible
            ClientSize = new Size(1000, 750);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Create main container with padding
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(248, 249, 250),
                AutoScroll = true
            };

            // Header section with image and basic info - more compact
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(960, 280),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Add subtle shadow effect
            headerPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, headerPanel.Width, headerPanel.Height);
                using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }
                e.Graphics.FillRectangle(Brushes.White, rect);
            };

            // Property image - more compact
            pictureBoxMain = new PictureBox
            {
                Location = new Point(20, 20),
                Size = new Size(350, 240),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(233, 236, 239)
            };

            // Property info section
            var infoPanel = new Panel
            {
                Location = new Point(390, 20),
                Size = new Size(550, 240),
                BackColor = Color.Transparent
            };

            // Title with modern typography
            lblTitle = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(550, 35),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Property Title",
                AutoSize = false
            };

            // Address with icon
            lblAddress = new Label
            {
                Location = new Point(0, 40),
                Size = new Size(550, 22),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(108, 117, 125),
                Text = "📍 Property Address",
                AutoSize = false
            };

            // Price with emphasis
            lblPrice = new Label
            {
                Location = new Point(0, 70),
                Size = new Size(550, 32),
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Text = "₱ 0",
                AutoSize = false
            };

            // Status badge
            statusPanel = new Panel
            {
                Location = new Point(0, 110),
                Size = new Size(110, 28),
                BackColor = Color.FromArgb(0, 123, 255)
            };
            statusPanel.Paint += (s, e) =>
            {
                var rect = statusPanel.ClientRectangle;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 14;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(rect.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, rect.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    statusPanel.Region = new Region(path);
                }
            };

            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "For Sale"
            };
            statusPanel.Controls.Add(lblStatus);

            // Action buttons in header for better visibility
            var headerButtonPanel = new Panel
            {
                Location = new Point(0, 150),
                Size = new Size(550, 80),
                BackColor = Color.Transparent
            };

            // Primary action button (changes based on mode)
            btnRequestInfo = new Button
            {
                Text = "📩 INQUIRE NOW",
                Size = new Size(200, 45),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = ColorTranslator.FromHtml("#2563eb"), // Blue for inquire
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, 0),
                Visible = false
            };
            btnRequestInfo.FlatAppearance.BorderSize = 0;
            btnRequestInfo.Click += BtnRequestInfo_Click;

            btnEdit = new Button
            {
                Text = "✏️ Edit Property",
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, 0),
                Visible = false
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;

            btnResubmit = new Button
            {
                Text = "🔄 Resubmit",
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(160, 0),
                Visible = false
            };
            btnResubmit.FlatAppearance.BorderSize = 0;
            btnResubmit.Click += BtnResubmit_Click;

            headerButtonPanel.Controls.AddRange(new Control[] { btnRequestInfo, btnEdit, btnResubmit });
            infoPanel.Controls.AddRange(new Control[] { lblTitle, lblAddress, lblPrice, statusPanel, headerButtonPanel });

            // Property details card - more compact
            detailsPanel = new Panel
            {
                Location = new Point(0, 290),
                Size = new Size(960, 140),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            detailsPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, detailsPanel.Width, detailsPanel.Height);
                using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }
                e.Graphics.FillRectangle(Brushes.White, rect);
            };

            var detailsHeader = new Label
            {
                Location = new Point(20, 12),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Property Details"
            };

            CreateDetailsGrid();

            // Description card - more compact
            descriptionPanel = new Panel
            {
                Location = new Point(0, 440),
                Size = new Size(960, 80),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            descriptionPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, descriptionPanel.Width, descriptionPanel.Height);
                using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }
                e.Graphics.FillRectangle(Brushes.White, rect);
            };

            var descriptionHeader = new Label
            {
                Location = new Point(20, 8),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Description"
            };

            lblDescription = new Label
            {
                Location = new Point(20, 30),
                Size = new Size(920, 40),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(73, 80, 87),
                Text = "No description available.",
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Proof of ownership card - more compact
            proofPanel = new Panel
            {
                Location = new Point(0, 530),
                Size = new Size(960, 120),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            proofPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, proofPanel.Width, proofPanel.Height);
                using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }
                e.Graphics.FillRectangle(Brushes.White, rect);
            };

            var proofHeader = new Label
            {
                Location = new Point(20, 8),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Proof of Ownership"
            };

            pictureBoxProof = new PictureBox
            {
                Location = new Point(20, 32),
                Size = new Size(200, 80),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(233, 236, 239),
                BorderStyle = BorderStyle.FixedSingle
            };

            proofFilesPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 32),
                Size = new Size(920, 80),
                BackColor = Color.FromArgb(248, 249, 250),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true
            };

            // Bottom button panel - always visible
            var bottomButtonPanel = new Panel
            {
                Location = new Point(0, 660),
                Size = new Size(960, 50),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            btnClose = new Button
            {
                Text = "Close",
                Size = new Size(100, 36),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(850, 7),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += BtnClose_Click;

            btnViewProof = new Button
            {
                Text = "📄 View Proof",
                Size = new Size(110, 36),
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(730, 7),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            btnViewProof.FlatAppearance.BorderSize = 0;

            btnDownloadProof = new Button
            {
                Text = "⬇️ Download",
                Size = new Size(110, 36),
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(23, 162, 184),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(610, 7),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            btnDownloadProof.FlatAppearance.BorderSize = 0;

            bottomButtonPanel.Controls.AddRange(new Control[] { btnClose, btnViewProof, btnDownloadProof });

            // Add all controls
            headerPanel.Controls.AddRange(new Control[] { pictureBoxMain, infoPanel });
            detailsPanel.Controls.Add(detailsHeader);
            descriptionPanel.Controls.AddRange(new Control[] { descriptionHeader, lblDescription });
            proofPanel.Controls.AddRange(new Control[] { proofHeader, pictureBoxProof, proofFilesPanel });

            mainContainer.Controls.AddRange(new Control[] {
                headerPanel, detailsPanel, descriptionPanel, proofPanel, bottomButtonPanel
            });

            Controls.Add(mainContainer);
            CancelButton = btnClose;
        }

        private void CreateDetailsGrid()
        {
            var detailsContainer = new Panel
            {
                Location = new Point(20, 40),
                Size = new Size(920, 88),
                BackColor = Color.Transparent
            };

            var details = new[]
            {
                new { Icon = "🛏️", Label = "Bedrooms", Value = "0", Position = new Point(0, 0) },
                new { Icon = "🛁", Label = "Bathrooms", Value = "0", Position = new Point(230, 0) },
                new { Icon = "📐", Label = "Lot Area", Value = "0 sqm", Position = new Point(460, 0) },
                new { Icon = "🏠", Label = "Floor Area", Value = "0 sqft", Position = new Point(690, 0) },
                new { Icon = "📅", Label = "Listed", Value = "-", Position = new Point(0, 44) },
                new { Icon = "🏢", Label = "Type", Value = "-", Position = new Point(230, 44) },
                new { Icon = "💼", Label = "Transaction", Value = "-", Position = new Point(460, 44) }
            };

            foreach (var detail in details)
            {
                var detailCard = new Panel
                {
                    Location = detail.Position,
                    Size = new Size(220, 40),
                    BackColor = Color.FromArgb(248, 249, 250),
                    Tag = detail.Label
                };

                detailCard.Paint += (s, e) =>
                {
                    var rect = detailCard.ClientRectangle;
                    using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        int radius = 6;
                        path.AddArc(0, 0, radius, radius, 180, 90);
                        path.AddArc(rect.Width - radius, 0, radius, radius, 270, 90);
                        path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
                        path.AddArc(0, rect.Height - radius, radius, radius, 90, 90);
                        path.CloseAllFigures();
                        detailCard.Region = new Region(path);
                    }
                };

                var iconLabel = new Label
                {
                    Text = detail.Icon,
                    Location = new Point(8, 6),
                    Size = new Size(20, 20),
                    Font = new Font("Segoe UI Emoji", 12F),
                    BackColor = Color.Transparent
                };

                var titleLabel = new Label
                {
                    Text = detail.Label,
                    Location = new Point(8, 24),
                    Size = new Size(100, 12),
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.FromArgb(108, 117, 125),
                    BackColor = Color.Transparent
                };

                var valueLabel = new Label
                {
                    Text = detail.Value,
                    Location = new Point(130, 14),
                    Size = new Size(80, 16),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(33, 37, 41),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleRight,
                    Tag = detail.Label + "_Value"
                };

                detailCard.Controls.AddRange(new Control[] { iconLabel, titleLabel, valueLabel });
                detailsContainer.Controls.Add(detailCard);
            }

            detailsPanel.Controls.Add(detailsContainer);
        }

        private void LoadPropertyDetails()
        {
            if (_property == null) return;

            // Basic info
            lblTitle.Text = _property.Title;
            lblAddress.Text = $"📍 {_property.Address}";
            lblPrice.Text = _property.Price.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-PH"));

            // Status badge
            var typeText = string.IsNullOrWhiteSpace(_property.PropertyType) ? "N/A" : _property.PropertyType;
            lblStatus.Text = typeText;

            statusPanel.BackColor = typeText switch
            {
                "Residential" => Color.FromArgb(40, 167, 69),
                "Commercial" => Color.FromArgb(0, 123, 255),
                "Raw Land" => Color.FromArgb(108, 117, 125),
                _ => Color.FromArgb(108, 117, 125),
            };

            // Update detail values
            UpdateDetailValue("Bedrooms_Value", _property.Bedrooms.ToString());
            UpdateDetailValue("Bathrooms_Value", _property.Bathrooms.ToString());
            UpdateDetailValue("Lot Area_Value", $"{_property.LotAreaSqm:N0} sqm");
            UpdateDetailValue("Floor Area_Value", $"{_property.FloorAreaSqft:N0} sqft");
            UpdateDetailValue("Listed_Value", _property.CreatedAt.ToString("MMM dd, yyyy"));
            UpdateDetailValue("Type_Value", GetPropertyValue("PropertyType") ?? "Not specified");
            UpdateDetailValue("Transaction_Value", GetPropertyValue("TransactionType") ?? "Not specified");

            lblDescription.Text = GetPropertyValue("Description") ?? GetPropertyValue("Notes") ?? "No description available.";

            LoadPropertyImage();
            LoadProofImage();

            // Configure button visibility based on mode
            if (_isClientBrowseMode)
            {
                // Client browse mode - show prominent Inquire button
                btnRequestInfo.Visible = true;
                btnRequestInfo.BringToFront();
                btnEdit.Visible = false;
                btnResubmit.Visible = false;
                AcceptButton = btnRequestInfo;

                System.Diagnostics.Debug.WriteLine("PropertyDetailsForm: Client Browse Mode - Inquire button should be visible");
            }
            else
            {
                // Owner/broker mode - show edit buttons
                btnRequestInfo.Visible = false;
                btnEdit.Visible = true;

                // Show resubmit button only if property is rejected
                bool isRejected = !_property.IsApproved && !string.IsNullOrEmpty(_property.RejectionReason);
                btnResubmit.Visible = isRejected;

                AcceptButton = btnEdit;

                System.Diagnostics.Debug.WriteLine($"PropertyDetailsForm: Owner Mode - Edit buttons visible, Resubmit={isRejected} (IsApproved={_property.IsApproved})");
            }
        }

        private void UpdateDetailValue(string tag, string value)
        {
            foreach (Control panel in detailsPanel.Controls)
            {
                if (panel is Panel detailsContainer && detailsContainer.Location.X == 20)
                {
                    foreach (Control card in detailsContainer.Controls)
                    {
                        if (card is Panel)
                        {
                            foreach (Control control in card.Controls)
                            {
                                if (control is Label label && label.Tag?.ToString() == tag)
                                {
                                    label.Text = value;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
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

            var defaultBitmap = new Bitmap(350, 240);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.FromArgb(233, 236, 239));
                using (var brush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
                {
                    string text = "🏠\nNo Image Available";
                    var textSize = g.MeasureString(text, font);
                    var x = (defaultBitmap.Width - textSize.Width) / 2;
                    var y = (defaultBitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            pictureBoxMain.Image = defaultBitmap;
        }

        private void LoadProofImage()
        {
            try
            {
                proofFilesPanel.Controls.Clear();

                if (_property.ProofFiles != null && _property.ProofFiles.Any())
                {
                    foreach (var proofFile in _property.ProofFiles.OrderByDescending(f => f.UploadDate))
                    {
                        var fileCard = CreateProofFileCard(proofFile);
                        proofFilesPanel.Controls.Add(fileCard);
                    }

                    pictureBoxProof.Visible = false;
                    proofFilesPanel.Visible = true;
                }
                else
                {
                    SetDefaultProofImage();
                    pictureBoxProof.Visible = true;
                    proofFilesPanel.Visible = false;
                }

                btnViewProof.Visible = false;
                btnDownloadProof.Visible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading proof files: {ex.Message}");
                SetDefaultProofImage();
                pictureBoxProof.Visible = true;
                proofFilesPanel.Visible = false;
                btnViewProof.Visible = false;
                btnDownloadProof.Visible = false;
            }
        }

        private Control CreateProofFileCard(PropertyProofFile proofFile)
        {
            var cardPanel = new Panel
            {
                Size = new Size(140, 70),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(3)
            };

            var iconLabel = new Label
            {
                Text = "📄",
                Font = new Font("Segoe UI", 16F),
                Location = new Point(60, 4),
                Size = new Size(20, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var fileName = proofFile.FileName.Length > 15
                ? proofFile.FileName.Substring(0, 12) + "..."
                : proofFile.FileName;
            var nameLabel = new Label
            {
                Text = fileName,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Location = new Point(4, 26),
                Size = new Size(132, 12),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            var sizeLabel = new Label
            {
                Text = FormatFileSize(proofFile.FileSize),
                Font = new Font("Segoe UI", 7F),
                Location = new Point(4, 38),
                Size = new Size(132, 10),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            var btnView = new Button
            {
                Text = "View",
                Size = new Size(60, 20),
                Font = new Font("Segoe UI", 7F),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(6, 48)
            };
            btnView.FlatAppearance.BorderSize = 0;
            btnView.Tag = proofFile;
            btnView.Click += (s, e) => ViewProofFile(proofFile);

            var btnDownload = new Button
            {
                Text = "DL",
                Size = new Size(30, 20),
                Font = new Font("Segoe UI", 7F),
                BackColor = Color.FromArgb(23, 162, 184),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(70, 48)
            };
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.Tag = proofFile;
            btnDownload.Click += (s, e) => DownloadProofFile(proofFile);

            cardPanel.Controls.AddRange(new Control[] { iconLabel, nameLabel, sizeLabel, btnView, btnDownload });
            return cardPanel;
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
                RefreshPropertyFromDatabase();
                LoadPropertyDetails();
                _propertyWasModified = true;
                PropertyUpdated?.Invoke(this, new PropertyEventArgs(_property));
            }
        }

        private void BtnResubmit_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            bool isRejected = !_property.IsApproved && !string.IsNullOrEmpty(_property.RejectionReason);

            string message;
            if (isRejected)
            {
                message = $"You are about to edit and resubmit this rejected property:\n\n" +
                         $"Property: {_property.Title}\n" +
                         $"Rejection Reason: {_property.RejectionReason}\n\n" +
                         $"After making changes, the property will be resubmitted for broker approval.";
            }
            else if (_property.IsApproved)
            {
                message = $"You are about to resubmit this approved property:\n\n" +
                         $"Property: {_property.Title}\n\n" +
                         $"The property will be marked as pending and require broker approval again.";
            }
            else
            {
                message = $"You are about to resubmit this property:\n\n" +
                         $"Property: {_property.Title}\n\n" +
                         $"After making changes, the property will be resubmitted for broker approval.";
            }

            var confirmResult = MessageBox.Show(
                message,
                "Resubmit Property",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (confirmResult != DialogResult.OK)
                return;

            var editForm = new EditPropertyForm(_property);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var db = Data.DbContextHelper.CreateDbContext();
                    var propertyToUpdate = db.Properties.FirstOrDefault(p => p.Id == _property.Id);

                    if (propertyToUpdate != null)
                    {
                        propertyToUpdate.IsResubmitted = true;
                        propertyToUpdate.IsApproved = false;
                        propertyToUpdate.RejectionReason = null;

                        if (!propertyToUpdate.SubmittedByUserId.HasValue)
                        {
                            var currentUser = Services.UserSession.Instance.CurrentUser;
                            if (currentUser != null)
                            {
                                propertyToUpdate.SubmittedByUserId = currentUser.Id;
                            }
                        }

                        db.SaveChanges();

                        MessageBox.Show(
                            "Property has been successfully resubmitted for broker approval!",
                            "Resubmitted Successfully",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        RefreshPropertyFromDatabase();
                        LoadPropertyDetails();
                        _propertyWasModified = true;
                        PropertyUpdated?.Invoke(this, new PropertyEventArgs(_property));

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error resubmitting property: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void SetDefaultProofImage()
        {
            var defaultBitmap = new Bitmap(200, 80);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.FromArgb(233, 236, 239));
                using (var brush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                {
                    string text = "📄 No Proof Available";
                    var textSize = g.MeasureString(text, font);
                    var x = (defaultBitmap.Width - textSize.Width) / 2;
                    var y = (defaultBitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            pictureBoxProof.Image = defaultBitmap;
        }

        private void ViewProofFile(PropertyProofFile proofFile)
        {
            try
            {
                if (File.Exists(proofFile.FilePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = proofFile.FilePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show($"File not found: {proofFile.FileName}", "File Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DownloadProofFile(PropertyProofFile proofFile)
        {
            try
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.FileName = proofFile.FileName;
                    saveDialog.Filter = "All files (*.*)|*.*";
                    saveDialog.Title = "Save Proof File";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(proofFile.FilePath))
                        {
                            File.Copy(proofFile.FilePath, saveDialog.FileName, true);
                            MessageBox.Show("File downloaded successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Source file not found: {proofFile.FileName}", "File Not Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void BtnRequestInfo_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            // Show a dialog to get the inquiry message from the client
            var requestForm = new RequestInfoForm(_property);
            if (requestForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(
                    "Your inquiry has been sent successfully! The broker will contact you soon.",
                    "Inquiry Sent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void PictureBoxProof_Click(object? sender, EventArgs e)
        {
            // This method is no longer used
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