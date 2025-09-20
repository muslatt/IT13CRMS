using Microsoft.EntityFrameworkCore;
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System.ComponentModel;

namespace RealEstateCRMWinForms.ViewModels
{
    public class PropertyViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        public BindingList<Property> Properties { get; set; }

        // Add this static event
        public static event EventHandler? PropertiesUpdated;

        public event PropertyChangedEventHandler? PropertyChanged;

        public PropertyViewModel()
        {
            _context = DbContextHelper.CreateDbContext();
            Properties = new BindingList<Property>();
            LoadProperties();
        }

        public void LoadProperties()
        {
            // Only load active properties
            var properties = _context.Properties.Where(p => p.IsActive).ToList();
            Properties = new BindingList<Property>(properties);
            OnPropertyChanged(nameof(Properties));

            // Notify global listeners (Dashboard subscribes to this)
            PropertiesUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void LoadAllProperties()
        {
            // Load all properties (including inactive ones)
            var properties = _context.Properties.ToList();
            Properties = new BindingList<Property>(properties);
            OnPropertyChanged(nameof(Properties));

            // Notify global listeners
            PropertiesUpdated?.Invoke(this, EventArgs.Empty);
        }

        public Property? GetPropertyById(int id)
        {
            try
            {
                return _context.Properties.FirstOrDefault(p => p.Id == id);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error getting property by ID: {ex.Message}");
                return null;
            }
        }

        public bool AddProperty(Property property)
        {
            try
            {
                _context.Properties.Add(property);
                _context.SaveChanges();
                LoadProperties(); // Refresh the list and notify
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool UpdateProperty(Property propertyToUpdate)
        {
            try
            {
                var property = _context.Properties.Find(propertyToUpdate.Id);
                if (property != null)
                {
                    _context.Entry(property).CurrentValues.SetValues(propertyToUpdate);
                    _context.SaveChanges();
                    LoadProperties(); // Refresh the list and notify
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteProperty(Property propertyToDelete)
        {
            try
            {
                var property = _context.Properties.Find(propertyToDelete.Id);
                if (property != null)
                {
                    // Soft delete: set IsActive to false instead of removing
                    property.IsActive = false;
                    _context.SaveChanges();
                    LoadProperties(); // Refresh the list and notify
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool RestoreProperty(Property propertyToRestore)
        {
            try
            {
                var property = _context.Properties.Find(propertyToRestore.Id);
                if (property != null)
                {
                    property.IsActive = true;
                    _context.SaveChanges();
                    LoadProperties(); // Refresh the list and notify
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
