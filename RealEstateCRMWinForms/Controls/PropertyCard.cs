using RealEstateCRMWinForms.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace RealEstateCRMWinForms.Controls
{
    public partial class PropertyCard : UserControl
    {
        private Property _property;
        private ContextMenuStrip _contextMenu;
        private bool _isReadOnly;
        private bool _isBrowseMode;
        private Label? lblRejectionReason;

        public PropertyCard()
        {
            _isReadOnly = false;
            _isBrowseMode = false;
            InitializeComponent();
            SetDefaultImage();
            CreateContextMenu();
            InitializeRejectionReasonLabel();

            // Reduce flicker and enable modern rendering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            // Add click events to make the card clickable
            this.Click += PropertyCard_Click;
            this.Cursor = Cursors.Hand;

            // Make all child controls clickable too
            MakeChildControlsClickable(this);

            // Apply modern styling
            ApplyModernStyling();
        }

        public PropertyCard(bool isReadOnly) : this()
        {
            _isReadOnly = isReadOnly;
            _isBrowseMode = false;
            CreateContextMenu(); // Recreate menu based on read-only
        }

        public PropertyCard(bool isReadOnly, bool isBrowseMode) : this()
        {
            _isReadOnly = isReadOnly;
            _isBrowseMode = isBrowseMode;
            CreateContextMenu(); // Recreate menu based on read-only
        }

        private void ApplyModernStyling()
        {
            // Modern card styling with rounded corners and shadow effect
            this.BackColor = Color.White;
            this.Padding = new Padding(0);
            this.Margin = new Padding(12);

            // Override paint to add modern card appearance
            this.Paint += PropertyCard_Paint;
        }

        private void PropertyCard_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create rounded rectangle path
            var rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            var path = CreateRoundedRectanglePath(rect, 12);

            // Draw shadow effect
            var shadowRect = new Rectangle(3, 3, this.Width - 3, this.Height - 3);
            var shadowPath = CreateRoundedRectanglePath(shadowRect, 12);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            // Draw main card background
            using (var cardBrush = new SolidBrush(Color.White))
            {
                g.FillPath(cardBrush, path);
            }

            // Draw subtle border
            using (var borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
            {
                g.DrawPath(borderPen, path);
            }
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            var diameter = cornerRadius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void MakeChildControlsClickable(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                control.Click += PropertyCard_Click;
                control.Cursor = Cursors.Hand;
                if (control.HasChildren)
                {
                    MakeChildControlsClickable(control);
                }
            }
        }

        private void InitializeRejectionReasonLabel()
        {
            lblRejectionReason = new Label
            {
                Name = "lblRejectionReason",
                Location = new Point(8, 265), // Position above the price
                Size = new Size(344, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 53, 69), // Red color for rejection
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false // Initially hidden
            };

            this.Controls.Add(lblRejectionReason);
        }

        private void PropertyCard_Click(object? sender, EventArgs e)
        {
            if (_property != null)
            {
                ShowPropertyDetails();
            }
        }

        private void ShowPropertyDetails()
        {
            bool isClientBrowseContext = _isBrowseMode;

            if (!isClientBrowseContext && _isReadOnly)
            {
                var currentUser = RealEstateCRMWinForms.Services.UserSession.Instance.CurrentUser;
                if (currentUser?.Role == UserRole.Client)
                {
                    isClientBrowseContext = true;
                }
            }

            var detailsForm = new Views.PropertyDetailsForm(_property, isClientBrowseContext);

            // Subscribe to the PropertyUpdated event
            detailsForm.PropertyUpdated += (sender, e) =>
            {
                // Refresh this card when the property is updated from the details form
                RefreshPropertyFromDatabase();

                // Also notify the parent (PropertiesView) that the property was updated
                PropertyUpdated?.Invoke(this, e);
            };

            var result = detailsForm.ShowDialog();

            // If the property was modified in the details form, refresh the card
            if (result == DialogResult.OK)
            {
                RefreshPropertyFromDatabase();
            }
        }

        public void SetProperty(Property property)
        {
            _property = property;
            UpdateCardUI();
            // Recreate context menu based on property state (rejected or not)
            CreateContextMenu();
        }

        public Property GetProperty()
        {
            return _property;
        }

        // Event for when property is updated or deleted
        public event EventHandler<PropertyEventArgs> PropertyUpdated;
        public event EventHandler<PropertyEventArgs> PropertyDeleted;

        private void CreateContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            _contextMenu.BackColor = Color.White;
            _contextMenu.ForeColor = Color.FromArgb(73, 80, 87);
            _contextMenu.Font = new Font("Segoe UI", 9F);

            var viewMenuItem = new ToolStripMenuItem("View Details")
            {
                Image = CreateModernIcon("👁", Color.FromArgb(0, 123, 255))
            };
            viewMenuItem.Click += ViewMenuItem_Click;

            _contextMenu.Items.Add(viewMenuItem);

            if (!_isReadOnly)
            {
                // Check if this is a rejected property
                bool isRejected = _property != null && !_property.IsApproved && !string.IsNullOrEmpty(_property.RejectionReason);

                if (isRejected)
                {
                    // For rejected properties, show "Edit & Resubmit" option
                    var resubmitMenuItem = new ToolStripMenuItem("Edit & Resubmit")
                    {
                        Image = CreateModernIcon("🔄", Color.FromArgb(255, 193, 7)) // Orange/Yellow color
                    };
                    resubmitMenuItem.Click += ResubmitMenuItem_Click;
                    _contextMenu.Items.Add(resubmitMenuItem);
                }
                else
                {
                    // For non-rejected properties, show normal edit option
                    var editMenuItem = new ToolStripMenuItem("Edit Property")
                    {
                        Image = CreateModernIcon("✏", Color.FromArgb(40, 167, 69))
                    };
                    editMenuItem.Click += EditMenuItem_Click;
                    _contextMenu.Items.Add(editMenuItem);
                }

                var deleteMenuItem = new ToolStripMenuItem("Delete Property")
                {
                    Image = CreateModernIcon("🗑", Color.FromArgb(220, 53, 69))
                };
                deleteMenuItem.Click += DeleteMenuItem_Click;

                _contextMenu.Items.Add(deleteMenuItem);
            }

            // Assign context menu to the card and its child controls
            this.ContextMenuStrip = _contextMenu;
            AssignContextMenuToChildren(this);
        }

        private Bitmap CreateModernIcon(string emoji, Color color)
        {
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                using (var font = new Font("Segoe UI Emoji", 12, FontStyle.Regular))
                using (var brush = new SolidBrush(color))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(emoji, font, brush, new RectangleF(0, 0, 16, 16), sf);
                }
            }
            return bmp;
        }

        private void AssignContextMenuToChildren(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                control.ContextMenuStrip = _contextMenu;
                if (control.HasChildren)
                {
                    AssignContextMenuToChildren(control);
                }
            }
        }

        private void ViewMenuItem_Click(object? sender, EventArgs e)
        {
            ShowPropertyDetails();
        }

        private void EditMenuItem_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            var editForm = new Views.EditPropertyForm(_property);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Refresh property data from database after edit
                RefreshPropertyFromDatabase();

                // Notify parent that property was updated
                PropertyUpdated?.Invoke(this, new PropertyEventArgs(_property));
            }
        }

        private void ResubmitMenuItem_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            // Show a message to the user about resubmission
            var confirmResult = MessageBox.Show(
                $"You are about to edit and resubmit this property:\n\n" +
                $"Property: {_property.Title}\n" +
                $"Rejection Reason: {_property.RejectionReason}\n\n" +
                $"After making changes, the property will be resubmitted for broker approval.",
                "Edit & Resubmit Property",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (confirmResult != DialogResult.OK)
                return;

            var editForm = new Views.EditPropertyForm(_property);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Update the property to mark it as resubmitted and reset approval status
                try
                {
                    using var db = Data.DbContextHelper.CreateDbContext();
                    var propertyToUpdate = db.Properties.FirstOrDefault(p => p.Id == _property.Id);

                    if (propertyToUpdate != null)
                    {
                        // Mark as resubmitted and reset approval status
                        propertyToUpdate.IsResubmitted = true;
                        propertyToUpdate.IsApproved = false; // Set to pending approval
                        propertyToUpdate.RejectionReason = null; // Clear the rejection reason

                        db.SaveChanges();

                        MessageBox.Show(
                            "Property has been successfully resubmitted for broker approval!",
                            "Resubmitted Successfully",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Refresh property data from database after resubmit
                        RefreshPropertyFromDatabase();

                        // Notify parent that property was updated
                        PropertyUpdated?.Invoke(this, new PropertyEventArgs(_property));
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
                    UpdateCardUI();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing property from database: {ex.Message}");
            }
        }

        private void DeleteMenuItem_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            try
            {
                var user = RealEstateCRMWinForms.Services.UserSession.Instance.CurrentUser;
                // Brokers can delete properties (Agent check removed)
            }
            catch { }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the property '{_property.Title}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                var viewModel = new ViewModels.PropertyViewModel();
                if (viewModel.DeleteProperty(_property))
                {
                    MessageBox.Show("Property deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Notify parent that property was deleted
                    PropertyDeleted?.Invoke(this, new PropertyEventArgs(_property));
                }
                else
                {
                    MessageBox.Show("Failed to delete property. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateCardUI()
        {
            if (_property == null) return;

            // Suspend layout to prevent flicker during updates
            this.SuspendLayout();

            try
            {
                // Apply modern typography and spacing
                ApplyModernTypography();

                // Title with modern styling
                lblTitle.Text = _property.Title ?? string.Empty;
                lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
                lblTitle.ForeColor = Color.FromArgb(33, 37, 41);
                lblTitle.Invalidate();
                lblTitle.Update();

                // Address with subtle styling
                lblAddress.Text = _property.Address ?? string.Empty;
                lblAddress.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                lblAddress.ForeColor = Color.FromArgb(108, 117, 125);
                lblAddress.Invalidate();
                lblAddress.Update();

                // Price with emphasis and modern currency formatting
                lblPrice.Text = _property.Price.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-PH"));
                lblPrice.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                lblPrice.ForeColor = Color.FromArgb(40, 167, 69);
                lblPrice.Invalidate();
                lblPrice.Update();

                // Modern property type badge
                UpdatePropertyTypeBadge();

                // Show rejection reason if property is rejected
                if (lblRejectionReason != null)
                {
                    if (!string.IsNullOrEmpty(_property.RejectionReason))
                    {
                        lblRejectionReason.Text = $"Reason: {_property.RejectionReason}";
                        lblRejectionReason.Visible = true;
                    }
                    else
                    {
                        lblRejectionReason.Visible = false;
                    }
                }

                // Show active deal information for clients in My Listings
                UpdateActiveDealInfo();

                // Update feature icons with modern design
                UpdateFeatureIcons();

                // Feature values with modern styling
                UpdateFeatureValues();

                // Load property image with modern styling
                LoadPropertyImage();

                // Force the entire card to refresh
                this.Invalidate(true);
                this.Update();
            }
            finally
            {
                this.ResumeLayout(true);
            }
        }

        private void ApplyModernTypography()
        {
            // Ensure all labels have proper modern styling
            foreach (Control control in this.Controls)
            {
                if (control is Label label)
                {
                    label.BackColor = Color.Transparent;
                }
            }
        }

        private void UpdatePropertyTypeBadge()
        {
            var currentUser = RealEstateCRMWinForms.Services.UserSession.Instance.CurrentUser;
            string statusText;
            Color badgeColor;

            if (currentUser != null && currentUser.Role == Models.UserRole.Client)
            {
                if (!string.IsNullOrEmpty(_property.RejectionReason))
                {
                    // For clients viewing their rejected properties
                    statusText = "Rejected";
                    badgeColor = Color.FromArgb(220, 53, 69); // Red for rejected
                }
                else if (!_property.IsApproved)
                {
                    // For clients viewing their pending properties
                    statusText = "Pending";
                    badgeColor = Color.FromArgb(255, 193, 7); // Yellow/Orange for pending
                }
                else
                {
                    // For clients viewing their approved properties
                    statusText = _property.PropertyType ?? string.Empty;
                    badgeColor = statusText switch
                    {
                        "Residential" => Color.FromArgb(40, 167, 69),
                        "Commercial" => Color.FromArgb(0, 123, 255),
                        "Raw Land" => Color.FromArgb(108, 117, 125),
                        _ => Color.FromArgb(90, 95, 100),
                    };
                }
            }
            else
            {
                // Show property type for brokers
                statusText = _property.PropertyType ?? string.Empty;
                badgeColor = statusText switch
                {
                    "Residential" => Color.FromArgb(40, 167, 69),
                    "Commercial" => Color.FromArgb(0, 123, 255),
                    "Raw Land" => Color.FromArgb(108, 117, 125),
                    _ => Color.FromArgb(90, 95, 100),
                };
            }

            lblStatus.Text = statusText;

            // Apply color
            statusPanel.BackColor = badgeColor;

            // Ensure lblStatus has the intended font/color
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.ForeColor = Color.White;

            // Measure label preferred size (taking padding into account) and resize statusPanel accordingly
            using (var g = CreateGraphics())
            {
                var proposedSize = new Size(int.MaxValue, int.MaxValue);
                var preferred = lblStatus.GetPreferredSize(proposedSize);

                // Add a small horizontal margin so the rounded badge has breathing room
                int panelWidth = Math.Max(preferred.Width, 40);
                int panelHeight = Math.Max(preferred.Height, 20);

                statusPanel.Size = new Size(panelWidth, panelHeight);

                // Position the statusPanel anchored to the right side of titlePanel with a fixed margin
                int rightMargin = 10;
                statusPanel.Location = new Point(titlePanel.Width - statusPanel.Width - rightMargin, (titlePanel.Height - statusPanel.Height) / 2);

                // Reduce lblTitle width so it doesn't get overlapped by the badge.
                // Keep a small spacing between title and badge.
                int spacing = 8;
                int availableWidth = Math.Max(40, titlePanel.Width - statusPanel.Width - spacing);
                lblTitle.Width = Math.Min(lblTitle.Width, availableWidth);
            }

            // Ensure we only attach a single paint handler to draw the rounded background
            if (!_paintHandlerAttached)
            {
                statusPanel.Paint += StatusPanel_Paint;
                _paintHandlerAttached = true;
            }

            statusPanel.Invalidate();
            statusPanel.Update();
            lblStatus.Invalidate();
            lblStatus.Update();
        }

        private bool _paintHandlerAttached = false;

        private void StatusPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, statusPanel.Width, statusPanel.Height);
            var path = CreateRoundedRectanglePath(rect, 6);
            using (var brush = new SolidBrush(statusPanel.BackColor))
            {
                g.FillPath(brush, path);
            }
        }

        private void UpdateFeatureIcons()
        {
            // Modern minimalist icons with consistent styling
            pbBedIcon.Image?.Dispose();
            pbBedIcon.Image = CreateModernFeatureIcon("🛏", Color.FromArgb(73, 80, 87));
            pbBedIcon.Invalidate();
            pbBedIcon.Update();

            pbBathIcon.Image?.Dispose();
            pbBathIcon.Image = CreateModernFeatureIcon("🛁", Color.FromArgb(73, 80, 87));
            pbBathIcon.Invalidate();
            pbBathIcon.Update();

            pbSqmIcon.Image?.Dispose();
            pbSqmIcon.Image = CreateModernFeatureIcon("📐", Color.FromArgb(73, 80, 87));
            pbSqmIcon.Invalidate();
            pbSqmIcon.Update();
        }

        private void UpdateFeatureValues()
        {
            // Modern typography for feature values
            var featureFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            var featureColor = Color.FromArgb(33, 37, 41);

            lblBedValue.Text = _property.Bedrooms.ToString();
            lblBedValue.Font = featureFont;
            lblBedValue.ForeColor = featureColor;
            lblBedValue.Invalidate();
            lblBedValue.Update();

            lblBathValue.Text = _property.Bathrooms.ToString();
            lblBathValue.Font = featureFont;
            lblBathValue.ForeColor = featureColor;
            lblBathValue.Invalidate();
            lblBathValue.Update();

            lblSqmValue.Text = _property.LotAreaSqm > 0 ? $"{_property.LotAreaSqm:N0} sqm" : "N/A";
            lblSqmValue.Font = featureFont;
            lblSqmValue.ForeColor = featureColor;
            lblSqmValue.Invalidate();
            lblSqmValue.Update();
        }

        private void UpdateActiveDealInfo()
        {
            // Check if current user is a client and if this property has an active deal
            var currentUser = RealEstateCRMWinForms.Services.UserSession.Instance.CurrentUser;
            if (currentUser == null || currentUser.Role != Models.UserRole.Client || _property == null)
                return;

            try
            {
                using var db = Data.DbContextHelper.CreateDbContext();

                // Find active deal for this property where the client is the contact
                var activeDeal = db.Deals
                    .Include(d => d.Contact)
                    .FirstOrDefault(d => d.PropertyId == _property.Id &&
                                        d.IsActive &&
                                        d.Contact != null &&
                                        d.Contact.Email == currentUser.Email &&
                                        d.Status.ToLower() != "closed" &&
                                        d.Status.ToLower() != "lost");

                if (activeDeal != null)
                {
                    // Add deal status info to the address label
                    lblAddress.Text = $"{_property.Address ?? string.Empty}\n" +
                                     $"🤝 Active Deal • Status: {activeDeal.Status}";
                    lblAddress.ForeColor = Color.FromArgb(25, 135, 84); // Green to indicate active deal
                }
            }
            catch
            {
                // Silently fail if there's an error checking for deals
            }
        }

        private Bitmap CreateModernFeatureIcon(string emoji, Color color)
        {
            var bmp = new Bitmap(28, 28);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                // Create circular background
                var circleRect = new Rectangle(2, 2, 24, 24);
                using (var backgroundBrush = new SolidBrush(Color.FromArgb(248, 249, 250)))
                {
                    g.FillEllipse(backgroundBrush, circleRect);
                }

                // Draw border
                using (var borderPen = new Pen(Color.FromArgb(222, 226, 230), 1))
                {
                    g.DrawEllipse(borderPen, circleRect);
                }

                // Draw emoji
                using (var font = new Font("Segoe UI Emoji", 14, FontStyle.Regular))
                using (var brush = new SolidBrush(color))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(emoji, font, brush, new RectangleF(0, 0, 28, 28), sf);
                }
            }
            return bmp;
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
                            pictureBox.Image?.Dispose();
                            pictureBox.Image = new Bitmap(img);
                        }

                        // Apply modern image styling
                        ApplyModernImageStyling();
                        pictureBox.Invalidate();
                        pictureBox.Update();
                        return;
                    }
                }

                // Set default image if no image or file doesn't exist
                SetDefaultImage();
            }
            catch (Exception ex)
            {
                // Log error and set default image
                Console.WriteLine($"Error loading property image: {ex.Message}");
                SetDefaultImage();
            }
        }

        private void ApplyModernImageStyling()
        {
            // Add rounded corners to image
            pictureBox.Paint += (s, e) =>
            {
                if (pictureBox.Image != null)
                {
                    var g = e.Graphics;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    var rect = new Rectangle(0, 0, pictureBox.Width, pictureBox.Height);
                    var path = CreateRoundedRectanglePath(rect, 8);

                    g.SetClip(path);
                    g.DrawImage(pictureBox.Image, rect);
                    g.ResetClip();

                    // Draw subtle border
                    using (var borderPen = new Pen(Color.FromArgb(222, 226, 230), 1))
                    {
                        g.DrawPath(borderPen, path);
                    }
                }
            };
        }

        private void SetDefaultImage()
        {
            // Dispose existing image first
            pictureBox.Image?.Dispose();

            // Create a modern placeholder image
            var defaultBitmap = new Bitmap(264, 180);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                // Modern gradient background
                using (var brush = new LinearGradientBrush(
                    new Rectangle(0, 0, 264, 180),
                    Color.FromArgb(248, 249, 250),
                    Color.FromArgb(233, 236, 239),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, 0, 0, 264, 180);
                }

                // Modern placeholder icon and text
                using (var iconBrush = new SolidBrush(Color.FromArgb(173, 181, 189)))
                using (var iconFont = new Font("Segoe UI Emoji", 32, FontStyle.Regular))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("🏠", iconFont, iconBrush, new RectangleF(0, 40, 264, 60), sf);
                }

                using (var textBrush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                using (var textFont = new Font("Segoe UI", 11, FontStyle.Regular))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("No Image Available", textFont, textBrush, new RectangleF(0, 110, 264, 30), sf);
                }
            }
            pictureBox.Image = defaultBitmap;
            ApplyModernImageStyling();
            pictureBox.Invalidate();
            pictureBox.Update();
        }

        public static string GetPropertyImagePath(string imagePath)
        {
            // Create images directory if it doesn't exist
            string imagesDir = Path.Combine(Application.StartupPath, "PropertyImages");
            if (!Directory.Exists(imagesDir))
            {
                Directory.CreateDirectory(imagesDir);
            }

            // Return full path to image
            return Path.Combine(imagesDir, imagePath);
        }

        public static string SavePropertyImage(string sourceImagePath, int propertyId)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceImagePath) || !File.Exists(sourceImagePath))
                    return null;

                // Create images directory if it doesn't exist
                string imagesDir = Path.Combine(Application.StartupPath, "PropertyImages");
                if (!Directory.Exists(imagesDir))
                {
                    Directory.CreateDirectory(imagesDir);
                }

                // Generate unique filename
                string extension = Path.GetExtension(sourceImagePath);
                string fileName = $"property_{propertyId}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                string destinationPath = Path.Combine(imagesDir, fileName);

                // Copy the image to our project folder
                File.Copy(sourceImagePath, destinationPath, true);

                // Return just the filename (not full path) to store in database
                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving property image: {ex.Message}");
                return null;
            }
        }
    }

    // Event args for property events
    public class PropertyEventArgs : EventArgs
    {
        public Property Property { get; }
        public string RejectionReason { get; set; } = string.Empty;

        public PropertyEventArgs(Property property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }
    }
}