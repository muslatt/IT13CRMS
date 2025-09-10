// RealEstateCRMWinForms\ViewModels\ContactViewModel.cs
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System;
using System.ComponentModel;
using System.Linq;
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
                    var contactsFromDb = dbContext.Contacts.Where(c => c.IsActive).ToList();
                    
                    foreach (var contact in contactsFromDb)
                    {
                        Contacts.Add(contact);
                    }
                    
                    // If no contacts in database, load sample data
                    if (Contacts.Count == 0)
                    {
                        LoadSampleData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading contacts: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Fallback to sample data
                LoadSampleData();
            }
        }

        public bool AddContact(Contact contact)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    dbContext.Contacts.Add(contact);
                    dbContext.SaveChanges();
                    
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

        private void LoadSampleData()
        {
            // Sample data for initial display
            var sampleContacts = new List<Contact>
            {
                new Contact
                {
                    FullName = "Name 1",
                    Email = "email@example.com",
                    Phone = "+1 231 231 2312",
                    Type = "Buyer",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Name 2",
                    Email = "email@example.com",
                    Phone = "+1 231 231 2312",
                    Type = "Buyer",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Name 3",
                    Email = "email@example.com",
                    Phone = "+1 231 231 2312",
                    Type = "Buyer",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Name 4",
                    Email = "email@example.com",
                    Phone = "+1 231 231 2312",
                    Type = "Buyer",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Name 5",
                    Email = "email@example.com",
                    Phone = "+1 231 231 2312",
                    Type = "Buyer",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsActive = true
                },
                new Contact
                {
                    FullName = "Name 6",
                    Email = "email@example.com",
                    Phone = "+1 231 231 2312",
                    Type = "Renter",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    IsActive = true
                }
            };

            // Save sample data to database
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    foreach (var contact in sampleContacts)
                    {
                        dbContext.Contacts.Add(contact);
                        Contacts.Add(contact);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving sample data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // If database save fails, just add to memory
                foreach (var contact in sampleContacts)
                {
                    Contacts.Add(contact);
                }
            }
        }
    }
}