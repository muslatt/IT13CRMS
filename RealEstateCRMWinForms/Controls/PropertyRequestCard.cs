using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using RealEstateCRMWinForms.Views;

namespace RealEstateCRMWinForms.Controls
{
    public class PropertyRequestCard : UserControl
    {
        private Property _property;

        public event EventHandler<PropertyEventArgs>? PropertyApproved;
        public event EventHandler<PropertyEventArgs>? PropertyRejected;

        // UI Controls
        private PictureBox pbImage = null!;
        private SmoothLabel lblTitle = null!;
        private SmoothLabel lblAddress = null!;
        private SmoothLabel lblPrice = null!;
        private Panel statusPanel = null!;
        private SmoothLabel lblStatus = null!;
        private Panel? resubmitPanel; // Resubmit badge panel
        private SmoothLabel? lblResubmit; // Resubmit badge label
        private PictureBox pbBedIcon = null!;
        private SmoothLabel lblBedValue = null!;
        private PictureBox pbBathIcon = null!;
        private SmoothLabel lblBathValue = null!;
        private PictureBox pbSqmIcon = null!;
        private SmoothLabel lblSqmValue = null!;
        private SmoothLabel lblSubmittedBy = null!;
        private Button btnApprove = null!;
        private Button btnReject = null!;

        public PropertyRequestCard(Property property)
        {
            _property = property;
            InitializeComponent();

            // Reduce flicker and enable modern rendering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            // Add click events to make the card clickable
            this.Click += PropertyRequestCard_Click;
            this.Cursor = Cursors.Hand;

            // Make all child controls clickable too
            MakeChildControlsClickable(this);

            // Apply modern styling
            ApplyModernStyling();
            LoadPropertyData();
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi; // Scale properly on high-DPI to avoid clipped text
            this.Size = new Size(920, 240); // Increased height to prevent overlapping
            this.BackColor = Color.White;
            this.Padding = new Padding(0);
            this.Margin = new Padding(12);

            // Property image
            pbImage = new PictureBox
            {
                Name = "pbImage",
                Size = new Size(180, 140),
                Location = new Point(20, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pbImage);

            // Title panel (container for title and status/resubmit badges)
            var titlePanel = new Panel
            {
                Size = new Size(650, 40), // initial, will be widened below
                Location = new Point(220, 20),
                BackColor = Color.Transparent
            };
            // Allow the title panel to stretch with the card width so right-aligned badges are not clipped
            titlePanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            // Widen to use available width with a small right padding
            titlePanel.Width = this.Width - titlePanel.Left - 20;

            // Title
            lblTitle = new SmoothLabel
            {
                Name = "lblTitle",
                Text = "Property Title",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(0, 0),
                Size = new Size(400, 32), // Taller to avoid clipping
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent,
                AutoEllipsis = true, // Add ellipsis for long titles
                TextAlign = ContentAlignment.MiddleLeft,
                UseCompatibleTextRendering = true
            };
            titlePanel.Controls.Add(lblTitle);

            // Status panel (property type badge)
            statusPanel = new Panel
            {
                Size = new Size(100, 26), // Slightly larger for better visibility and to prevent chopping
                Location = new Point(410, 6), // Center within taller title panel
                BackColor = Color.FromArgb(40, 167, 69),
                Visible = false // Hide property type badge in Requests view
            };

            lblStatus = new SmoothLabel
            {
                Name = "lblStatus",
                Text = "Type",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                UseCompatibleTextRendering = true
            };
            statusPanel.Controls.Add(lblStatus);
            titlePanel.Controls.Add(statusPanel);

            // Resubmit badge panel (initially hidden, shown only for resubmitted properties)
            resubmitPanel = new Panel
            {
                Size = new Size(110, 26),
                BackColor = Color.FromArgb(255, 193, 7), // Orange/Yellow color for resubmit
                Visible = false
            };
            // Right-align inside the title panel to prevent clipping when moved right
            resubmitPanel.Location = new Point(titlePanel.Width - resubmitPanel.Width, 6);
            resubmitPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            lblResubmit = new SmoothLabel
            {
                Name = "lblResubmit",
                Text = "RESUBMIT",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                UseCompatibleTextRendering = true
            };
            resubmitPanel.Controls.Add(lblResubmit);
            titlePanel.Controls.Add(resubmitPanel);

            this.Controls.Add(titlePanel);

            // Address
            lblAddress = new SmoothLabel
            {
                Name = "lblAddress",
                Text = "Address",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(220, 62), // Slight tweak
                Size = new Size(650, 24), // Taller to prevent chopping
                ForeColor = Color.FromArgb(108, 117, 125),
                BackColor = Color.Transparent,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft,
                UseCompatibleTextRendering = true
            };
            this.Controls.Add(lblAddress);

            // Price
            lblPrice = new SmoothLabel
            {
                Name = "lblPrice",
                Text = "$0",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(220, 86), // Adjusted Y position
                Size = new Size(320, 44), // Taller to fully render currency glyphs
                ForeColor = Color.FromArgb(40, 167, 69),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                UseCompatibleTextRendering = true
            };
            this.Controls.Add(lblPrice);

            // Feature icons and values - positioned with proper spacing
            int featureY = 132; // Adjusted Y position for taller price label
            int iconSize = 24;
            int featureSpacing = 100; // Consistent spacing between features

            // Bed icon and value
            pbBedIcon = new PictureBox
            {
                Size = new Size(iconSize, iconSize),
                Location = new Point(220, featureY),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(pbBedIcon);

            lblBedValue = new SmoothLabel
            {
                Text = "0",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(250, featureY - 2),
                Size = new Size(60, 28), // Taller to avoid clipping
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                UseCompatibleTextRendering = true
            };
            this.Controls.Add(lblBedValue);

            // Bath icon and value
            pbBathIcon = new PictureBox
            {
                Size = new Size(iconSize, iconSize),
                Location = new Point(220 + featureSpacing, featureY),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(pbBathIcon);

            lblBathValue = new SmoothLabel
            {
                Text = "0",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(250 + featureSpacing, featureY - 2),
                Size = new Size(60, 28),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                UseCompatibleTextRendering = true
            };
            this.Controls.Add(lblBathValue);

            // Sqm icon and value
            pbSqmIcon = new PictureBox
            {
                Size = new Size(iconSize, iconSize),
                Location = new Point(220 + (featureSpacing * 2), featureY),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(pbSqmIcon);

            lblSqmValue = new SmoothLabel
            {
                Text = "0 sqm",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(250 + (featureSpacing * 2), featureY - 2),
                Size = new Size(110, 28), // Taller and a bit wider
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                UseCompatibleTextRendering = true
            };
            this.Controls.Add(lblSqmValue);

            // Submitted by - positioned with more space
            lblSubmittedBy = new SmoothLabel
            {
                Name = "lblSubmittedBy",
                Text = "Submitted by: Loading...",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(220, 168), // Adjusted for new layout
                Size = new Size(380, 22), // Taller to avoid clipping
                ForeColor = Color.FromArgb(52, 58, 64),
                BackColor = Color.Transparent,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft,
                UseCompatibleTextRendering = true
            };
            this.Controls.Add(lblSubmittedBy);

            // Approve button - repositioned to avoid overlap
            btnApprove = new Button
            {
                Name = "btnApprove",
                Text = "Approve",
                Size = new Size(120, 40),
                Location = new Point(650, 185), // Moved down and adjusted position
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            btnApprove.Click += BtnApprove_Click;
            this.Controls.Add(btnApprove);

            // Reject button - repositioned to avoid overlap
            btnReject = new Button
            {
                Name = "btnReject",
                Text = "Reject",
                Size = new Size(120, 40),
                Location = new Point(780, 185), // Moved down and adjusted position
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            btnReject.Click += BtnReject_Click;
            this.Controls.Add(btnReject);
        }

        private void ApplyModernStyling()
        {
            // Modern card styling with rounded corners and shadow effect
            this.BackColor = Color.White;
            this.Padding = new Padding(0);
            this.Margin = new Padding(12);

            // Override paint to add modern card appearance
            this.Paint += PropertyRequestCard_Paint;
        }

        private void PropertyRequestCard_Paint(object? sender, PaintEventArgs e)
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

        private void LoadPropertyData()
        {
            if (pbImage != null)
            {
                if (!string.IsNullOrEmpty(_property.ImagePath))
                {
                    try
                    {
                        var imagePath = PropertyCard.GetPropertyImagePath(_property.ImagePath);
                        if (File.Exists(imagePath))
                        {
                            pbImage.Image = Image.FromFile(imagePath);
                        }
                        else
                        {
                            SetDefaultImage(pbImage);
                        }
                    }
                    catch
                    {
                        SetDefaultImage(pbImage);
                    }
                }
                else
                {
                    SetDefaultImage(pbImage);
                }
            }

            if (lblTitle != null) lblTitle.Text = _property.Title ?? string.Empty;
            if (lblAddress != null) lblAddress.Text = _property.Address ?? string.Empty;
            if (lblPrice != null) lblPrice.Text = _property.Price.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-PH"));

            // In Requests view do not show the property type/status badge panel
            if (statusPanel != null) statusPanel.Visible = false;

            // Use the right-positioned badge panel to display 'REJECTED' when in Rejected list
            if (resubmitPanel != null)
            {
                bool isRejected = !_property.IsApproved && !string.IsNullOrEmpty(_property.RejectionReason);
                resubmitPanel.Visible = isRejected;
                if (isRejected)
                {
                    if (lblResubmit != null) lblResubmit.Text = "REJECTED";
                    resubmitPanel.BackColor = Color.FromArgb(220, 53, 69); // Red badge for rejected
                }
            }

            // Update feature icons
            UpdateFeatureIcons();

            // Update feature values
            UpdateFeatureValues();

            // Load submitter information
            if (lblSubmittedBy != null)
            {
                if (_property.SubmittedByUserId.HasValue)
                {
                    try
                    {
                        using var db = RealEstateCRMWinForms.Data.DbContextHelper.CreateDbContext();
                        var user = db.Users.FirstOrDefault(u => u.Id == _property.SubmittedByUserId.Value);
                        lblSubmittedBy.Text = user != null ? $"Submitted by: {user.FirstName} {user.LastName}" : $"Submitted by: User {_property.SubmittedByUserId}";
                    }
                    catch
                    {
                        lblSubmittedBy.Text = $"Submitted by: User {_property.SubmittedByUserId}";
                    }
                }
                else
                {
                    lblSubmittedBy.Text = "Submitted by: Unknown";
                }
            }

            // Hide approve and reject buttons if property is rejected
            if (!_property.IsApproved && !string.IsNullOrEmpty(_property.RejectionReason))
            {
                if (btnApprove != null)
                {
                    btnApprove.Visible = false;
                }
                if (btnReject != null)
                {
                    btnReject.Visible = false;
                }
            }
        }

        private void UpdatePropertyTypeBadge()
        {
            var typeText = _property.PropertyType ?? string.Empty;
            if (lblStatus != null)
            {
                lblStatus.Text = typeText;
            }

            // Modern gradient colors for property types
            Color badgeColor = typeText switch
            {
                "Residential" => Color.FromArgb(40, 167, 69),
                "Commercial" => Color.FromArgb(0, 123, 255),
                "Raw Land" => Color.FromArgb(108, 117, 125),
                _ => Color.FromArgb(90, 95, 100),
            };

            // Apply color
            if (statusPanel != null)
            {
                statusPanel.BackColor = badgeColor;
            }
        }

        private void UpdateFeatureIcons()
        {
            // Modern minimalist icons with consistent styling
            if (pbBedIcon != null)
            {
                pbBedIcon.Image?.Dispose();
                pbBedIcon.Image = CreateModernFeatureIcon("🛏", Color.FromArgb(73, 80, 87));
            }

            if (pbBathIcon != null)
            {
                pbBathIcon.Image?.Dispose();
                pbBathIcon.Image = CreateModernFeatureIcon("🛁", Color.FromArgb(73, 80, 87));
            }

            if (pbSqmIcon != null)
            {
                pbSqmIcon.Image?.Dispose();
                pbSqmIcon.Image = CreateModernFeatureIcon("📐", Color.FromArgb(73, 80, 87));
            }
        }

        private void UpdateFeatureValues()
        {
            // Modern typography for feature values
            if (lblBedValue != null)
            {
                lblBedValue.Text = _property.Bedrooms.ToString();
            }

            if (lblBathValue != null)
            {
                lblBathValue.Text = _property.Bathrooms.ToString();
            }

            if (lblSqmValue != null)
            {
                lblSqmValue.Text = _property.LotAreaSqm > 0 ? $"{_property.LotAreaSqm:N0} sqm" : "N/A";
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

                // Draw emoji
                using (var font = new Font("Segoe UI Emoji", 14, FontStyle.Regular))
                using (var brush = new SolidBrush(color))
                {
                    var size = g.MeasureString(emoji, font);
                    var x = (bmp.Width - size.Width) / 2;
                    var y = (bmp.Height - size.Height) / 2;
                    g.DrawString(emoji, font, brush, x, y);
                }
            }
            return bmp;
        }

        private void SetDefaultImage(PictureBox pbImage)
        {
            // Create a default property image placeholder
            var bmp = new Bitmap(150, 120);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                using (var brush = new SolidBrush(Color.DarkGray))
                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                {
                    var text = "No Image";
                    var size = g.MeasureString(text, font);
                    var x = (bmp.Width - size.Width) / 2;
                    var y = (bmp.Height - size.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            pbImage.Image = bmp;
        }

        private void BtnApprove_Click(object? sender, EventArgs e)
        {
            PropertyApproved?.Invoke(this, new PropertyEventArgs(_property));
        }

        private void BtnReject_Click(object? sender, EventArgs e)
        {
            using var rejectionDialog = new PropertyRejectionDialog();
            if (rejectionDialog.ShowDialog() == DialogResult.OK)
            {
                // Create a new PropertyEventArgs with the rejection reason
                var args = new PropertyEventArgs(_property);
                args.RejectionReason = rejectionDialog.RejectionReason;
                PropertyRejected?.Invoke(this, args);
            }
        }

        private void MakeChildControlsClickable(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                // Don't make buttons clickable as they have their own functionality
                if (control is Button) continue;

                control.Click += PropertyRequestCard_Click;
                control.Cursor = Cursors.Hand;
                if (control.HasChildren)
                {
                    MakeChildControlsClickable(control);
                }
            }
        }

        private void PropertyRequestCard_Click(object? sender, EventArgs e)
        {
            if (_property != null)
            {
                ShowPropertyDetails();
            }
        }

        private void ShowPropertyDetails()
        {
            var propertyToDisplay = _property;

            try
            {
                using var db = DbContextHelper.CreateDbContext();
                var refreshedProperty = db.Properties
                    .Include(p => p.ProofFiles)
                    .FirstOrDefault(p => p.Id == _property.Id);

                if (refreshedProperty != null)
                {
                    propertyToDisplay = refreshedProperty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing property details: {ex.Message}");
            }

            var detailsForm = new Views.PropertyDetailsForm(propertyToDisplay);
            detailsForm.ShowDialog();
        }
    }
}