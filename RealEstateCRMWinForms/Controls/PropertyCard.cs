using RealEstateCRMWinForms.Models;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Controls
{
    public partial class PropertyCard : UserControl
    {
        private Property? _property;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Property? Property
        {
            get => _property;
            set
            {
                _property = value;
                UpdateDisplay();
            }
        }

        public PropertyCard()
        {
            InitializeComponent();
        }

        private void UpdateDisplay()
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
        }
    }
}
