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
            
            var editMenuItem = new ToolStripMenuItem("Edit Property")
            {
                Image = SystemIcons.Application.ToBitmap() // You can use a better icon
            };
            editMenuItem.Click += EditMenuItem_Click;

            var deleteMenuItem = new ToolStripMenuItem("Delete Property")
            {
                Image = SystemIcons.Error.ToBitmap() // You can use a better icon
            };
            deleteMenuItem.Click += DeleteMenuItem_Click;

            _contextMenu.Items.AddRange(new ToolStripItem[] { editMenuItem, deleteMenuItem });
            
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

        private void EditMenuItem_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            var editForm = new Views.EditPropertyForm(_property);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Update the card display
                UpdateCardUI();
                
                // Notify parent that property was updated
                PropertyUpdated?.Invoke(this, new PropertyEventArgs(_property));
            }
        }

        private void DeleteMenuItem_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

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
            
            lblTitle.Text = _property.Title;
            lblAddress.Text = _property.Address;
            lblPrice.Text = $"₱ {_property.Price:N0}";
            lblBedrooms.Text = $"🛏️ {_property.Bedrooms}";
            lblBathrooms.Text = $"🚿 {_property.Bathrooms}";
            lblSquareMeters.Text = $"📐 {_property.SquareMeters} sqm";
            lblStatus.Text = _property.Status;
            
            // Set status color
            statusPanel.BackColor = _property.Status == "Rent" 
                ? Color.FromArgb(108, 117, 125) 
                : Color.FromArgb(0, 123, 255);

            // Load property image
            LoadPropertyImage();
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
