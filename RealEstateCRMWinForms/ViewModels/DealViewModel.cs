using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace RealEstateCRMWinForms.ViewModels
{
    public class DealViewModel : INotifyPropertyChanged
    {
        public BindingList<Deal> Deals { get; set; }
        public BindingList<Property> Properties { get; set; }
        public BindingList<Contact> Contacts { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public DealViewModel()
        {
            Deals = new BindingList<Deal>();
            Properties = new BindingList<Property>();
            Contacts = new BindingList<Contact>();
            LoadDeals();
            LoadProperties();
            LoadContacts();
        }

        public void LoadDeals()
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    var dealsFromDb = dbContext.Deals
                        .Include(d => d.Property)
                        .Include(d => d.Contact)
                        .Where(d => d.IsActive)
                        .ToList();
                    
                    Deals.Clear();
                    foreach (var deal in dealsFromDb)
                    {
                        Deals.Add(deal);
                    }
                    
                    Console.WriteLine($"Loaded {Deals.Count} active deals from database");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading deals: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Don't load sample data anymore - just log the error
                Deals.Clear();
            }
            
            OnPropertyChanged(nameof(Deals));
        }

        public void LoadProperties()
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    var properties = dbContext.Properties.Where(p => p.IsActive).ToList();
                    Properties.Clear();
                    foreach (var property in properties)
                    {
                        Properties.Add(property);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading properties: {ex.Message}");
            }
            
            OnPropertyChanged(nameof(Properties));
        }

        public void LoadContacts()
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    var contacts = dbContext.Contacts.Where(c => c.IsActive).ToList();
                    Contacts.Clear();
                    foreach (var contact in contacts)
                    {
                        Contacts.Add(contact);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading contacts: {ex.Message}");
            }
            
            OnPropertyChanged(nameof(Contacts));
        }

        public bool AddDeal(Deal deal)
        {
            try
            {
                Console.WriteLine($"Attempting to add deal: {deal.Title}");
                
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Ensure foreign key relationships are properly handled
                    if (deal.PropertyId.HasValue)
                    {
                        var existingProperty = dbContext.Properties.Find(deal.PropertyId.Value);
                        if (existingProperty != null)
                        {
                            deal.Property = existingProperty;
                        }
                        else
                        {
                            deal.PropertyId = null;
                            deal.Property = null;
                        }
                    }

                    if (deal.ContactId.HasValue)
                    {
                        var existingContact = dbContext.Contacts.Find(deal.ContactId.Value);
                        if (existingContact != null)
                        {
                            deal.Contact = existingContact;
                        }
                        else
                        {
                            deal.ContactId = null;
                            deal.Contact = null;
                        }
                    }

                    // Set the navigation properties to null to avoid tracking issues
                    var dealToAdd = new Deal
                    {
                        Title = deal.Title,
                        Description = deal.Description,
                        Value = deal.Value,
                        PropertyId = deal.PropertyId,
                        ContactId = deal.ContactId,
                        Status = deal.Status,
                        Notes = deal.Notes,
                        CreatedAt = deal.CreatedAt,
                        IsActive = deal.IsActive,
                        CreatedBy = deal.CreatedBy
                    };

                    dbContext.Deals.Add(dealToAdd);
                    dbContext.SaveChanges();
                    
                    Console.WriteLine($"Successfully saved deal with ID: {dealToAdd.Id}");
                    
                    // Add to local collection with the generated ID
                    deal.Id = dealToAdd.Id;
                    Deals.Add(deal);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding deal: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public bool UpdateDeal(Deal deal)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Find the existing deal in the database
                    var existingDeal = dbContext.Deals.Find(deal.Id);
                    if (existingDeal != null)
                    {
                        // Update the properties
                        existingDeal.Title = deal.Title;
                        existingDeal.Description = deal.Description;
                        existingDeal.Value = deal.Value;
                        existingDeal.PropertyId = deal.PropertyId;
                        existingDeal.ContactId = deal.ContactId;
                        existingDeal.Status = deal.Status; // This is crucial for board tracking
                        existingDeal.Notes = deal.Notes;
                        existingDeal.UpdatedAt = DateTime.UtcNow;
                        existingDeal.IsActive = deal.IsActive;

                        dbContext.SaveChanges();
                        
                        Console.WriteLine($"Successfully updated deal ID: {deal.Id}, Status: {deal.Status}");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Deal with ID {deal.Id} not found in database");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating deal: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public bool DeleteDeal(Deal deal)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Find the existing deal in the database
                    var existingDeal = dbContext.Deals.Find(deal.Id);
                    if (existingDeal != null)
                    {
                        // Soft delete
                        existingDeal.IsActive = false;
                        existingDeal.UpdatedAt = DateTime.UtcNow;
                        dbContext.SaveChanges();
                        
                        Console.WriteLine($"Successfully soft deleted deal ID: {deal.Id}");
                        
                        // Remove from local collection
                        Deals.Remove(deal);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Deal with ID {deal.Id} not found in database");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting deal: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        // Method to get deals by status (for board management)
        public List<Deal> GetDealsByStatus(string status)
        {
            return Deals.Where(d => d.Status == status && d.IsActive).ToList();
        }

        // Method to move deal to different status/board
        public bool MoveDealToStatus(Deal deal, string newStatus)
        {
            try
            {
                deal.Status = newStatus;
                deal.UpdatedAt = DateTime.UtcNow;
                
                bool result = UpdateDeal(deal);
                if (result)
                {
                    Console.WriteLine($"Successfully moved deal '{deal.Title}' to status '{newStatus}'");
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error moving deal to status: {ex.Message}");
                return false;
            }
        }

        // Utility method to clean up any sample/test data
        public void CleanupSampleData()
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Find deals with typical sample data names
                    var sampleDeals = dbContext.Deals.Where(d => 
                        d.Title == "Test" || 
                        d.Title == "Test Again" ||
                        d.Description == "Test" ||
                        d.Description == "Again").ToList();

                    if (sampleDeals.Any())
                    {
                        Console.WriteLine($"Found {sampleDeals.Count} sample deals to clean up");
                        
                        foreach (var deal in sampleDeals)
                        {
                            deal.IsActive = false;
                            deal.UpdatedAt = DateTime.UtcNow;
                        }
                        
                        dbContext.SaveChanges();
                        Console.WriteLine("Sample data cleanup completed");
                        
                        // Reload deals after cleanup
                        LoadDeals();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning up sample data: {ex.Message}");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}