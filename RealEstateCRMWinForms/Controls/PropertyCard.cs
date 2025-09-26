using RealEstateCRMWinForms.Models;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Controls
{
    public partial class PropertyCard : UserControl
    {
        private Property _property;
        private ContextMenuStrip _contextMenu;

        public PropertyCard()
        {
            InitializeComponent();
            SetDefaultImage();
            CreateContextMenu();
            
            // Reduce flicker
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
            
            // Add click events to make the card clickable
            this.Click += PropertyCard_Click;
            this.Cursor = Cursors.Hand;
            
            // Make all child controls clickable too
            MakeChildControlsClickable(this);
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

        private void PropertyCard_Click(object? sender, EventArgs e)
        {
            if (_property != null)
            {
                ShowPropertyDetails();
            }
        }

        private void ShowPropertyDetails()
        {
            var detailsForm = new Views.PropertyDetailsForm(_property);
            
            // Subscribe to the PropertyUpdated event
            detailsForm.PropertyUpdated += (sender, e) => {
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
            
            var viewMenuItem = new ToolStripMenuItem("View Details")
            {
                Image = SystemIcons.Information.ToBitmap()
            };
            viewMenuItem.Click += ViewMenuItem_Click;

            var editMenuItem = new ToolStripMenuItem("Edit Property")
            {
                Image = SystemIcons.Application.ToBitmap()
            };
            editMenuItem.Click += EditMenuItem_Click;

            var deleteMenuItem = new ToolStripMenuItem("Delete Property")
            {
                Image = SystemIcons.Error.ToBitmap()
            };
            deleteMenuItem.Click += DeleteMenuItem_Click;

            _contextMenu.Items.AddRange(new ToolStripItem[] { viewMenuItem, editMenuItem, deleteMenuItem });
            
            // Assign context menu to the card and its child controls
            this.ContextMenuStrip = _contextMenu;
            AssignContextMenuToChildren(this);
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
                // Brokers cannot delete a property if it is assigned to an Agent
                if (user != null && user.Role == RealEstateCRMWinForms.Models.UserRole.Broker)
                {
                    if (!string.IsNullOrWhiteSpace(_property.Agent))
                    {
                        MessageBox.Show("This property is assigned to an Agent and cannot be deleted by a Broker.",
                            "Deletion Restricted",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }
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
            
            // Title / address / price / status
            lblTitle.Text = _property.Title;
            lblAddress.Text = _property.Address;
            lblPrice.Text = _property.Price.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-PH"));
            lblStatus.Text = _property.Status;

            // Set status color
            statusPanel.BackColor = _property.Status == "Rent" 
                ? Color.FromArgb(108, 117, 125) 
                : Color.FromArgb(0, 123, 255);

            // --- FEATURE ICONS (render into small bitmaps for consistent sizing) ---
            // Create small icon bitmaps using emoji rendered with Segoe UI Emoji for visual clarity.

            // Bed icon
            pbBedIcon.Image?.Dispose();
            pbBedIcon.Image = CreateEmojiIconBitmap("🛏", 24, 24);

            // Bath icon
            pbBathIcon.Image?.Dispose();
            pbBathIcon.Image = CreateEmojiIconBitmap("🛁", 24, 24);

            // SQM icon (use a ruler-like emoji)
            pbSqmIcon.Image?.Dispose();
            pbSqmIcon.Image = CreateEmojiIconBitmap("📐", 24, 24);

            // Numeric values: larger, bold and clearly distinguished
            lblBedValue.Text = _property.Bedrooms.ToString();
            lblBedValue.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            lblBathValue.Text = _property.Bathrooms.ToString();
            lblBathValue.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            lblSqmValue.Text = _property.SquareMeters > 0 ? $"{_property.SquareMeters} sqm" : "N/A";
            lblSqmValue.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            // Load property image
            LoadPropertyImage();
        }

        // Helper that generates a small bitmap with an emoji centered. Keeps icons consistent size.
        private Bitmap CreateEmojiIconBitmap(string emoji, int width, int height)
        {
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                // Use Segoe UI Emoji for best emoji rendering on Windows
                using (var font = new Font("Segoe UI Emoji", Math.Max(12, height - 4), FontStyle.Regular, GraphicsUnit.Pixel))
                using (var brush = new SolidBrush(Color.FromArgb(90, 95, 100)))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(emoji, font, brush, new RectangleF(0, 0, width, height), sf);
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
                            pictureBox.Image?.Dispose(); // Dispose existing image first
                            pictureBox.Image = new Bitmap(img);
                        }
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

        private void SetDefaultImage()
        {
            // Dispose existing image first
            pictureBox.Image?.Dispose();
            
            // Create a default placeholder image
            var defaultBitmap = new Bitmap(264, 180);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.LightGray);
                
                // Draw a simple house icon placeholder
                using (var brush = new SolidBrush(Color.Gray))
                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                {
                    string text = "No Image";
                    var textSize = g.MeasureString(text, font);
                    var x = (defaultBitmap.Width - textSize.Width) / 2;
                    var y = (defaultBitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            pictureBox.Image = defaultBitmap;
        }

        private string GetPropertyImagePath(string imagePath)
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

        public PropertyEventArgs(Property property)
        {
            Property = property;
        }
    }
}




