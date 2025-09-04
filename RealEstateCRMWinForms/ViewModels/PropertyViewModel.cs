using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.ViewModels
{
    public class PropertyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public BindingList<Property> Properties { get; set; }

        public PropertyViewModel()
        {
            Properties = new BindingList<Property>();
            LoadProperties(); // Load from database
        }

        public void LoadProperties()
        {
            Properties.Clear();
            
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    var propertiesFromDb = dbContext.Properties.Where(p => p.IsActive).ToList();
                    
                    foreach (var property in propertiesFromDb)
                    {
                        Properties.Add(property);
                    }
                    
                    // If no properties in database, load sample data
                    if (Properties.Count == 0)
                    {
                        LoadSampleData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading properties: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Fallback to sample data
                LoadSampleData();
            }
        }

        public bool AddProperty(Property property)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    dbContext.Properties.Add(property);
                    dbContext.SaveChanges();
                    
                    // Add to local collection
                    Properties.Add(property);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding property: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadSampleData()
        {
            // Sample data for initial display
            for (int i = 1; i <= 8; i++)
            {
                Properties.Add(new Property
                {
                    Id = i,
                    Title = $"Property {i}",
                    Address = "1234 Greenfield Avenue",
                    Price = 9999999,
                    Bedrooms = 4,
                    Bathrooms = 2,
                    SquareMeters = 99,
                    Status = i % 3 == 0 ? "Rent" : "Sell",
                    ImagePath = null
                });
            }
        }
    }
}
