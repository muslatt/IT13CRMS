// RealEstateCRMWinForms\ViewModels\LeadViewModel.cs
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
                    
                    // Get available agents from properties
                    var availableAgents = dbContext.Properties
                        .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Agent))
                        .Select(p => p.Agent)
                        .Distinct()
                        .ToList();
                    
                    var random = new Random();
                    
                    foreach (var lead in leadsFromDb)
                    {
                        // Assign a random agent from available agents for display purposes
                        if (availableAgents.Any())
                        {
                            lead.AssignedAgent = availableAgents[random.Next(availableAgents.Count)];
                        }
                        else
                        {
                            lead.AssignedAgent = "No Agent";
                        }
                        
                        Leads.Add(lead);
                    }

                    // NOTE: removed sample data insertion � the list will remain empty if DB has no leads.
                }
            }
            catch (Exception ex)
            {
                // Fail silently for UI stability; log to Debug and show a single minimal message if desired.
                System.Diagnostics.Debug.WriteLine($"Error loading leads: {ex.Message}");
                MessageBox.Show($"Error loading leads. Check database connection.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool AddLead(Lead lead)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // The AssignedAgent property is NotMapped, so it won't be saved to DB
                    // but it will be preserved in memory for display
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
    }
}