using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Controls
{
    public partial class DealCard : UserControl
    {
        private Deal _deal;
        private Label lblTitle;
        private Label lblDescription;
        private Label lblValue;
        private Panel dragBar; // Colored drag bar at the top
        private PictureBox imgProperty; // Property image placeholder
        private ContextMenuStrip _contextMenu;

        public DealCard()
        {
            InitializeComponent();
            CreateContextMenu();
        }

        public void SetDeal(Deal deal)
        {
            _deal = deal;
            UpdateCardUI();
        }

        public Deal GetDeal()
        {
            return _deal;
        }

        // Events for when deal is updated or deleted
        public event EventHandler<DealEventArgs> DealUpdated;
        public event EventHandler<DealEventArgs> DealDeleted;

        private void CreateContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            
            var editMenuItem = new ToolStripMenuItem("Edit Deal")
            {
                Image = SystemIcons.Application.ToBitmap()
            };
            editMenuItem.Click += EditMenuItem_Click;

            var deleteMenuItem = new ToolStripMenuItem("Delete Deal")
            {
                Image = SystemIcons.Error.ToBitmap()
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
            if (_deal == null) return;

            var editForm = new Views.EditDealForm(_deal);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Update the card display
                UpdateCardUI();
                
                // Notify parent that deal was updated
                DealUpdated?.Invoke(this, new DealEventArgs(_deal));
            }
        }

        private void DeleteMenuItem_Click(object? sender, EventArgs e)
        {
            if (_deal == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the deal '{_deal.Title}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                var viewModel = new DealViewModel();
                if (viewModel.DeleteDeal(_deal))
                {
                    MessageBox.Show("Deal deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Notify parent that deal was deleted
                    DealDeleted?.Invoke(this, new DealEventArgs(_deal));
                }
                else
                {
                    MessageBox.Show("Failed to delete deal. Please try again.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Size(280, 240); // Increased height to accommodate larger image like PropertyCard
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Margin = new Padding(0, 0, 0, 10); // Bottom margin for spacing between cards
            this.Cursor = Cursors.Hand;

            // Colored drag bar at the top (matches board header colors)
            dragBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 8,
                BackColor = Color.FromArgb(214, 133, 133), // Default to "New" color
                Cursor = Cursors.SizeAll
            };

            // Property image - matching PropertyCard size (264x180)
            imgProperty = new PictureBox
            {
                Location = new Point(8, 16), // Matching PropertyCard padding
                Size = new Size(264, 120), // Adjusted height to fit within card but maintain aspect ratio
                BackColor = Color.FromArgb(240, 242, 245),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Title label
            lblTitle = new Label
            {
                Location = new Point(8, 145),
                Size = new Size(264, 25),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoEllipsis = true
            };

            // Description label
            lblDescription = new Label
            {
                Location = new Point(8, 170),
                Size = new Size(264, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoEllipsis = true
            };

            // Value label (positioned at bottom, styled like PropertyCard price)
            lblValue = new Label
            {
                Location = new Point(8, 195),
                Size = new Size(264, 25),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold), // Matching PropertyCard price font
                ForeColor = Color.FromArgb(33, 37, 41),
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.Controls.AddRange(new Control[] {
                dragBar, imgProperty, lblTitle, lblDescription, lblValue
            });

            // Enable drag functionality on the entire card
            this.MouseDown += DealCard_MouseDown;
            dragBar.MouseDown += DragBar_MouseDown;
            imgProperty.MouseDown += DragBar_MouseDown;

            // Set default image
            SetDefaultPropertyImage();
        }

        private void SetDefaultPropertyImage()
        {
            // Dispose existing image first
            imgProperty.Image?.Dispose();

            // Create a default placeholder image with PropertyCard proportions
            var defaultBitmap = new Bitmap(264, 120);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.FromArgb(240, 242, 245));
                
                // Draw a simple house icon placeholder (scaled for new size)
                using (var brush = new SolidBrush(Color.FromArgb(180, 180, 180)))
                {
                    // Scale the house icon for the new dimensions
                    int houseWidth = 60;
                    int houseHeight = 30;
                    int houseX = (264 - houseWidth) / 2;
                    int houseY = (120 - houseHeight) / 2 - 10;
                    
                    g.FillRectangle(brush, houseX, houseY + 15, houseWidth, houseHeight - 15);
                    g.FillPolygon(brush, new Point[] {
                        new Point(houseX - 10, houseY + 15),
                        new Point(houseX + houseWidth / 2, houseY),
                        new Point(houseX + houseWidth + 10, houseY + 15)
                    });
                }
                
                // Add "Property Image" text
                using (var font = new Font("Segoe UI", 10F))
                using (var textBrush = new SolidBrush(Color.FromArgb(150, 150, 150)))
                {
                    var text = "Property Image";
                    var textSize = g.MeasureString(text, font);
                    g.DrawString(text, font, textBrush, 
                        (264 - textSize.Width) / 2, 
                        (120 - textSize.Height) / 2 + 20);
                }
            }
            imgProperty.Image = defaultBitmap;
        }

        private void LoadPropertyImage()
        {
            try
            {
                if (_deal?.Property?.ImagePath != null && !string.IsNullOrEmpty(_deal.Property.ImagePath))
                {
                    string imagePath = GetPropertyImagePath(_deal.Property.ImagePath);
                    if (File.Exists(imagePath))
                    {
                        using (var img = Image.FromFile(imagePath))
                        {
                            imgProperty.Image?.Dispose(); // Dispose existing image first
                            imgProperty.Image = new Bitmap(img);
                        }
                        return;
                    }
                }
                
                // Set default image if no image or file doesn't exist
                SetDefaultPropertyImage();
            }
            catch (Exception ex)
            {
                // Log error and set default image
                Console.WriteLine($"Error loading property image: {ex.Message}");
                SetDefaultPropertyImage();
            }
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

        private void DealCard_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.DoDragDrop(this, DragDropEffects.Move);
            }
        }

        private void DragBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.DoDragDrop(this, DragDropEffects.Move);
            }
        }

        private void UpdateCardUI()
        {
            if (_deal == null) return;
            
            lblTitle.Text = _deal.Title;
            lblDescription.Text = _deal.Description;
            lblValue.Text = _deal.Value.HasValue ? $"₱ {_deal.Value:N0}" : "₱ 0";
            
            // Set drag bar color to match board header colors (from DealsView)
            var boardColor = _deal.Status switch
            {
                "New" => Color.FromArgb(214, 133, 133),
                "Offer Made" => Color.FromArgb(182, 200, 133),
                "Negotiation" => Color.FromArgb(255, 255, 153),
                "Contract Draft" => Color.FromArgb(144, 238, 144),
                // Default colors for custom boards
                _ => Color.FromArgb(173, 216, 230) // Light blue for unknown statuses
            };

            dragBar.BackColor = boardColor; 
            
            // Load property image
            LoadPropertyImage();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Properly dispose of the image to avoid file locks
                if (imgProperty?.Image != null)
                {
                    imgProperty.Image.Dispose();
                    imgProperty.Image = null;
                }
            }
            base.Dispose(disposing);
        }
    }

    // Event args for deal events
    public class DealEventArgs : EventArgs
    {
        public Deal Deal { get; }

        public DealEventArgs(Deal deal)
        {
            Deal = deal;
        }
    }
}