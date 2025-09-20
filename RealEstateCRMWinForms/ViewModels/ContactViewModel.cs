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

                    // NOTE: Removed sample data insertion — the list will remain empty if DB has no contacts.
                }
            }
            catch (Exception ex)
            {
                // Log and show a single minimal message for UI stability
                System.Diagnostics.Debug.WriteLine($"Error loading contacts: {ex.Message}");
                MessageBox.Show($"Error loading contacts. Check database connection.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}