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
                    Property = property,
                    Margin = new Padding(10)
                };
                
                flowLayoutPanel.Controls.Add(card);
            }
        }
    }
}
