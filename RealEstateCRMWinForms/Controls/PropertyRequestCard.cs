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
        private Label lblTitle = null!;
        private Label lblAddress = null!;
        private Label lblPrice = null!;
        private Panel statusPanel = null!;
        private Label lblStatus = null!;
        private Panel? resubmitPanel; // Resubmit badge panel
        private Label? lblResubmit; // Resubmit badge label
        private PictureBox pbBedIcon = null!;
        private Label lblBedValue = null!;
        private PictureBox pbBathIcon = null!;
        private Label lblBathValue = null!;
        private PictureBox pbSqmIcon = null!;
        private Label lblSqmValue = null!;
        private Label lblSubmittedBy = null!;
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
            this.Size = new Size(900, 220);
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

            // Title panel (container for title and status badge)
            var titlePanel = new Panel
            {
                Size = new Size(400, 30),
                Location = new Point(220, 20),
                BackColor = Color.Transparent
            };

            // Title
            lblTitle = new Label
            {
                Name = "lblTitle",
                Text = "Property Title",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(0, 0),
                Size = new Size(350, 25),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent
            };
            titlePanel.Controls.Add(lblTitle);

            // Status panel (property type badge)
            statusPanel = new Panel
            {
                Size = new Size(80, 22),
                Location = new Point(320, 4),
                BackColor = Color.FromArgb(40, 167, 69)
            };

            lblStatus = new Label
            {
                Name = "lblStatus",
                Text = "Type",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(0, 0),
                Size = new Size(80, 22),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            statusPanel.Controls.Add(lblStatus);
            titlePanel.Controls.Add(statusPanel);

            // Resubmit badge panel (initially hidden, shown only for resubmitted properties)
            resubmitPanel = new Panel
            {
                Size = new Size(100, 22),
                Location = new Point(405, 4), // Position next to status badge
                BackColor = Color.FromArgb(255, 193, 7), // Orange/Yellow color for resubmit
                Visible = false
            };

            lblResubmit = new Label
            {
                Name = "lblResubmit",
                Text = "RESUBMIT",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Location = new Point(0, 0),
                Size = new Size(100, 22),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            resubmitPanel.Controls.Add(lblResubmit);
            titlePanel.Controls.Add(resubmitPanel);

            this.Controls.Add(titlePanel);

            // Address
            lblAddress = new Label
            {
                Name = "lblAddress",
                Text = "Address",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(220, 55),
                Size = new Size(400, 20),
                ForeColor = Color.FromArgb(108, 117, 125),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblAddress);

            // Price
            lblPrice = new Label
            {
                Name = "lblPrice",
                Text = "$0",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(220, 80),
                Size = new Size(250, 30),
                ForeColor = Color.FromArgb(40, 167, 69),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblPrice);

            // Feature icons and values
            int featureY = 120;
            int iconSize = 24;

            // Bed icon and value
            pbBedIcon = new PictureBox
            {
                Size = new Size(iconSize, iconSize),
                Location = new Point(220, featureY),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(pbBedIcon);

            lblBedValue = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(250, featureY - 2),
                Size = new Size(40, 24),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblBedValue);

            // Bath icon and value
            pbBathIcon = new PictureBox
            {
                Size = new Size(iconSize, iconSize),
                Location = new Point(310, featureY),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(pbBathIcon);

            lblBathValue = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(340, featureY - 2),
                Size = new Size(40, 24),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblBathValue);

            // Sqm icon and value
            pbSqmIcon = new PictureBox
            {
                Size = new Size(iconSize, iconSize),
                Location = new Point(400, featureY),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(pbSqmIcon);

            lblSqmValue = new Label
            {
                Text = "0 sqm",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(430, featureY - 2),
                Size = new Size(80, 24),
                ForeColor = Color.FromArgb(33, 37, 41),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblSqmValue);

            // Submitted by
            lblSubmittedBy = new Label
            {
                Name = "lblSubmittedBy",
                Text = "Submitted by: Loading...",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(220, 155),
                Size = new Size(300, 20),
                ForeColor = Color.FromArgb(52, 58, 64),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblSubmittedBy);

            // Approve button
            btnApprove = new Button
            {
                Name = "btnApprove",
                Text = "Approve",
                Size = new Size(120, 40),
                Location = new Point(650, 150),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            btnApprove.Click += BtnApprove_Click;
            this.Controls.Add(btnApprove);

            // Reject button
            btnReject = new Button
            {
                Name = "btnReject",
                Text = "Reject",
                Size = new Size(120, 40),
                Location = new Point(780, 150),
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

            // Update property type badge
            UpdatePropertyTypeBadge();

            // Show/hide resubmit badge based on property state
            if (resubmitPanel != null)
            {
                resubmitPanel.Visible = _property.IsResubmitted;
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