using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
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
                    var currentUser = Services.UserSession.Instance.CurrentUser;
                    var query = dbContext.Properties.Where(p => p.IsActive);

                    // For brokers, only show approved properties
                    if (currentUser != null && currentUser.Role == UserRole.Broker)
                    {
                        query = query.Where(p => p.IsApproved);
                    }

                    var properties = query.ToList();
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

        public async Task<bool> AddDealAsync(Deal deal)
        {
            try
            {
                Console.WriteLine($"Attempting to add deal: {deal.Title}");

                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Ensure foreign key relationships are properly handled
                    Contact? contactForNotification = null;

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
                            contactForNotification = existingContact;
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

                    // Log the action
                    LoggingService.LogAction("Created Deal", $"Deal '{dealToAdd.Title}' created with value ${dealToAdd.Value}");

                    Console.WriteLine($"Successfully saved deal with ID: {dealToAdd.Id}");

                    // Add to local collection with the generated ID
                    deal.Id = dealToAdd.Id;
                    deal.Contact = contactForNotification; // Set for notification
                    Deals.Add(deal);

                    // Send email notification
                    if (contactForNotification != null)
                    {
                        try
                        {
                            var emailService = new RealEstateCRMWinForms.Services.EmailNotificationService();
                            await emailService.SendNewDealNotificationAsync(deal);
                        }
                        catch (Exception emailEx)
                        {
                            Console.WriteLine($"Failed to send new deal email notification: {emailEx.Message}");
                            // Don't fail the entire operation if email fails
                        }
                    }

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

        public bool AddDeal(Deal deal)
        {
            // Synchronous wrapper for backward compatibility
            return AddDealAsync(deal).GetAwaiter().GetResult();
        }

        public async Task<bool> UpdateDealAsync(Deal deal, string? oldStatus = null)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Find the existing deal in the database with contact info
                    var existingDeal = dbContext.Deals
                        .Include(d => d.Contact)
                        .Include(d => d.Property)
                        .FirstOrDefault(d => d.Id == deal.Id);

                    if (existingDeal != null)
                    {
                        // Store old status for notification
                        string previousStatus = oldStatus ?? existingDeal.Status;

                        // Update the properties
                        existingDeal.Title = deal.Title;
                        existingDeal.Description = deal.Description;
                        existingDeal.Value = deal.Value;
                        existingDeal.PropertyId = deal.PropertyId;
                        existingDeal.ContactId = deal.ContactId;
                        existingDeal.Status = deal.Status; // This is crucial for board tracking
                        existingDeal.Notes = deal.Notes;
                        // Track who owns/accepted the deal
                        existingDeal.CreatedBy = deal.CreatedBy;
                        existingDeal.UpdatedAt = DateTime.UtcNow;
                        existingDeal.IsActive = deal.IsActive;

                        // Note: Property deactivation now happens only when client approves the closure request
                        // Automatically mark deal as inactive when status is "Closed/Done" (or contains "closed")
                        if (deal.Status.Contains("Closed", StringComparison.OrdinalIgnoreCase) ||
                            deal.Status.Equals("Lost", StringComparison.OrdinalIgnoreCase))
                        {
                            existingDeal.IsActive = false;
                            existingDeal.ClosedAt = DateTime.UtcNow;
                            Console.WriteLine($"Deal ID {deal.Id} marked as inactive due to {deal.Status} status");

                            // Property is NOT deactivated here anymore - it's handled in ClientDealsView approval
                        }

                        // If a property is linked and an agent has accepted, stamp the property with the agent name
                        if (existingDeal.Property != null && !string.IsNullOrWhiteSpace(existingDeal.CreatedBy))
                        {
                            // Agent assignment removed
                        }

                        dbContext.SaveChanges();

                        Console.WriteLine($"Successfully updated deal ID: {deal.Id}, Status: {deal.Status}");

                        // Send email notification if status changed and contact exists
                        if (previousStatus != deal.Status && existingDeal.Contact != null)
                        {
                            try
                            {
                                var emailService = new RealEstateCRMWinForms.Services.EmailNotificationService();
                                deal.Contact = existingDeal.Contact; // Ensure contact is set for notification
                                deal.Property = existingDeal.Property; // Ensure property is set for notification
                                await emailService.SendDealStatusUpdateNotificationAsync(deal, previousStatus, deal.Status);
                            }
                            catch (Exception emailEx)
                            {
                                Console.WriteLine($"Failed to send deal status update email notification: {emailEx.Message}");
                                // Don't fail the entire operation if email fails
                            }
                        }

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

        public bool UpdateDeal(Deal deal)
        {
            // Synchronous wrapper for backward compatibility
            return UpdateDealAsync(deal).GetAwaiter().GetResult();
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
        public async Task<bool> MoveDealToStatusAsync(Deal deal, string newStatus)
        {
            try
            {
                string oldStatus = deal.Status;
                deal.Status = newStatus;
                deal.UpdatedAt = DateTime.UtcNow;

                bool result = await UpdateDealAsync(deal, oldStatus);
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

        public bool MoveDealToStatus(Deal deal, string newStatus)
        {
            // Synchronous wrapper for backward compatibility
            return MoveDealToStatusAsync(deal, newStatus).GetAwaiter().GetResult();
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
