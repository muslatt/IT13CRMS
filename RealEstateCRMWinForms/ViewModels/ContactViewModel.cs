// RealEstateCRMWinForms\ViewModels\ContactViewModel.cs
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.ViewModels
{
    public class ContactViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public BindingList<Contact> Contacts { get; set; }

        public ContactViewModel()
        {
            Contacts = new BindingList<Contact>();
            LoadContacts(); // Load from database
        }

        public void LoadContacts()
        {
            Contacts.Clear();

            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();

                    var contactsFromDb = dbContext.Contacts.Where(c => c.IsActive).ToList();

                    foreach (var contact in contactsFromDb)
                    {
                        Contacts.Add(contact);
                    }

                    // NOTE: Removed sample data insertion — the list will remain empty if DB has no contacts.
                }
            }
            catch (Exception ex)
            {
                // Log the specific error for debugging
                System.Diagnostics.Debug.WriteLine($"Error loading contacts: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                // Don't show error dialog in constructor to prevent UI blocking
                // Instead, log and continue with empty list
                Console.WriteLine($"Database connection error: {ex.Message}");
            }
        }

        public bool AddContact(Contact contact)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();

                    dbContext.Contacts.Add(contact);
                    dbContext.SaveChanges();

                    // Log the action
                    LoggingService.LogAction("Created Contact", $"Contact '{contact.FullName}' created");

                    // Add to local collection
                    Contacts.Add(contact);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding contact: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateContact(Contact contact)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();

                    dbContext.Contacts.Update(contact);
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating contact: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeleteContact(Contact contact)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();

                    // Soft delete - just mark as inactive
                    contact.IsActive = false;
                    dbContext.Contacts.Update(contact);
                    dbContext.SaveChanges();

                    // Remove from local collection
                    Contacts.Remove(contact);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting contact: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task<bool> MoveContactToLeadAsync(Contact contact)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    dbContext.Database.EnsureCreated();

                    var contactEntity = await dbContext.Contacts.FindAsync(contact.Id);
                    if (contactEntity == null)
                    {
                        MessageBox.Show("The contact could not be found. It may have been deleted.", "Not Found",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    var lead = new Lead
                    {
                        FullName = contactEntity.FullName,
                        Email = contactEntity.Email,
                        Phone = contactEntity.Phone,
                        Type = "Lead",
                        AvatarPath = contactEntity.AvatarPath,
                        Occupation = contactEntity.Occupation,
                        Salary = contactEntity.Salary,
                        CreatedAt = contactEntity.CreatedAt,
                        IsActive = true
                    };

                    var affectedDeals = dbContext.Deals
                        .Where(d => d.ContactId == contactEntity.Id && d.IsActive)
                        .ToList();

                    foreach (var deal in affectedDeals)
                    {
                        deal.ContactId = null;
                        deal.Contact = null;
                        deal.UpdatedAt = DateTime.UtcNow;
                    }

                    dbContext.Leads.Add(lead);
                    dbContext.Contacts.Remove(contactEntity);

                    await dbContext.SaveChangesAsync();

                    var existingContact = Contacts.FirstOrDefault(c => c.Id == contactEntity.Id);
                    if (existingContact != null)
                    {
                        Contacts.Remove(existingContact);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving contact back to leads: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        /// <summary>
        /// Test database connection and create database if needed
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    dbContext.Database.EnsureCreated();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection test failed: {ex.Message}");
                return false;
            }
        }
    }
}


