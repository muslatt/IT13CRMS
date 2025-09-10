// RealEstateCRMWinForms\ViewModels\LeadViewModel.cs
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.ViewModels
{
    public class LeadViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public BindingList<Lead> Leads { get; set; }

        public LeadViewModel()
        {
            Leads = new BindingList<Lead>();
            LoadLeads(); // Load from database
        }

        public void LoadLeads()
        {
            Leads.Clear();
            
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    var leadsFromDb = dbContext.Leads.Where(l => l.IsActive).ToList();
                    
                    foreach (var lead in leadsFromDb)
                    {
                        Leads.Add(lead);
                    }
                    
                    // If no leads in database, load sample data
                    if (Leads.Count == 0)
                    {
                        LoadSampleData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading leads: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Fallback to sample data
                LoadSampleData();
            }
        }

        public bool AddLead(Lead lead)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    dbContext.Leads.Add(lead);
                    dbContext.SaveChanges();
                    
                    // Add to local collection
                    Leads.Add(lead);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding lead: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateLead(Lead lead)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    dbContext.Leads.Update(lead);
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating lead: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeleteLead(Lead lead)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Soft delete - just mark as inactive
                    lead.IsActive = false;
                    dbContext.Leads.Update(lead);
                    dbContext.SaveChanges();
                    
                    // Remove from local collection
                    Leads.Remove(lead);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting lead: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task<bool> MoveLeadToContactAsync(Lead lead)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Create a new contact from the lead
                    var contact = new Contact
                    {
                        FullName = lead.FullName,
                        Email = lead.Email,
                        Phone = lead.Phone,
                        Type = lead.Type,
                        AvatarPath = lead.AvatarPath,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    // Add the contact to the database
                    dbContext.Contacts.Add(contact);
                    
                    // Mark the lead as inactive (soft delete)
                    lead.IsActive = false;
                    dbContext.Leads.Update(lead);
                    
                    // Save changes
                    dbContext.SaveChanges();
                    
                    // Remove from local collection
                    Leads.Remove(lead);

                    // Send notification email
                    try
                    {
                        var emailService = new RealEstateCRMWinForms.Services.EmailNotificationService();
                        await emailService.SendLeadToContactNotificationAsync(contact);
                    }
                    catch (Exception emailEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to send email notification: {emailEx.Message}");
                        // Don't fail the entire operation if email fails
                    }
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error moving lead to contact: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadSampleData()
        {
            // Sample data for initial display
            var sampleLeads = new List<Lead>
            {
                new Lead
                {
                    FullName = "New lead",
                    Status = "New Lead",
                    Source = "Website",
                    Email = "newlead@email.com",
                    Phone = "",
                    Type = "Renter",
                    Address = "",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Lead
                {
                    FullName = "Name 1",
                    Status = "New Lead",
                    Source = "Facebook",
                    Email = "name@email.com",
                    Phone = "(720) 333-4444",
                    Type = "Renter",
                    Address = "Machu Picchu",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    IsActive = true
                },
                new Lead
                {
                    FullName = "Ana Cruz",
                    Status = "Contacted",
                    Source = "Website",
                    Email = "ana.cruz@email.com",
                    Phone = "(555) 123-4567",
                    Type = "Buyer",
                    Address = "Downtown District",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsActive = true
                },
                new Lead
                {
                    FullName = "Jose Santos",
                    Status = "Qualified",
                    Source = "Referral",
                    Email = "jose.santos@email.com",
                    Phone = "(555) 987-6543",
                    Type = "Owner",
                    Address = "Business Center",
                    AvatarPath = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsActive = true
                }
            };

            // Save sample data to database
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    foreach (var lead in sampleLeads)
                    {
                        dbContext.Leads.Add(lead);
                        Leads.Add(lead);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving sample data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // If database save fails, just add to memory
                foreach (var lead in sampleLeads)
                {
                    Leads.Add(lead);
                }
            }
        }
    }
}