using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System;
using System.ComponentModel;
using System.Linq;

namespace RealEstateCRMWinForms.ViewModels
{
    public class PropertyViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        public BindingList<Property> Properties { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public PropertyViewModel()
        {
            _context = DbContextHelper.CreateDbContext();
            LoadProperties();
        }

        public void LoadProperties()
        {
            // Only load active properties
            var properties = _context.Properties.Where(p => p.IsActive).ToList();
            Properties = new BindingList<Property>(properties);
            OnPropertyChanged(nameof(Properties));
        }

        public void LoadAllProperties()
        {
            // Load all properties (including inactive ones)
            var properties = _context.Properties.ToList();
            Properties = new BindingList<Property>(properties);
            OnPropertyChanged(nameof(Properties));
        }

        public bool AddProperty(Property property)
        {
            try
            {
                _context.Properties.Add(property);
                _context.SaveChanges();
                LoadProperties(); // Refresh the list
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
                    LoadProperties(); // Refresh the list
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
                    LoadProperties(); // Refresh the list
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
                    LoadProperties(); // Refresh the list
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
