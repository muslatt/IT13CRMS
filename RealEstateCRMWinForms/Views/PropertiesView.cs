using RealEstateCRMWinForms.Controls;
using RealEstateCRMWinForms.ViewModels;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class PropertiesView : UserControl
    {
        private readonly PropertyViewModel _viewModel;

        public PropertiesView()
        {
            _viewModel = new PropertyViewModel();
            InitializeComponent();
            LoadProperties();
        }

        private void BtnAddProperty_Click(object? sender, EventArgs e)
        {
            var addPropertyForm = new AddPropertyForm();
            if (addPropertyForm.ShowDialog() == DialogResult.OK)
            {
                // Refresh the properties list
                _viewModel.LoadProperties();
                LoadProperties();
            }
        }

        private void LoadProperties()
        {
            flowLayoutPanel.Controls.Clear();
            
            foreach (var property in _viewModel.Properties)
            {
                var card = new PropertyCard
                {
                    Margin = new Padding(10)
                };
                card.SetProperty(property);
                
                // Subscribe to property events
                card.PropertyUpdated += Card_PropertyUpdated;
                card.PropertyDeleted += Card_PropertyDeleted;
                
                flowLayoutPanel.Controls.Add(card);
            }
        }

        private void Card_PropertyUpdated(object? sender, PropertyEventArgs e)
        {
            // Refresh the properties list to reflect any changes
            _viewModel.LoadProperties();
            LoadProperties();
        }

        private void Card_PropertyDeleted(object? sender, PropertyEventArgs e)
        {
            if (sender is PropertyCard card)
            {
                // Remove the card from the UI
                flowLayoutPanel.Controls.Remove(card);
                
                // Dispose of the card to free resources
                card.Dispose();
                
                // Optionally refresh the entire list to ensure consistency
                _viewModel.LoadProperties();
                LoadProperties();
            }
        }
    }
}
