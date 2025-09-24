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
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();
                    
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

                    // NOTE: removed sample data insertion – the list will remain empty if DB has no leads.
                }
            }
            catch (Exception ex)
            {
                // Log the specific error for debugging
                System.Diagnostics.Debug.WriteLine($"Error loading leads: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Don't show error dialog in constructor to prevent UI blocking
                // Instead, log and continue with empty list
                Console.WriteLine($"Database connection error: {ex.Message}");
            }
        }

        public bool AddLead(Lead lead)
        {
            try
            {
                using (var dbContext = DbContextHelper.CreateDbContext())
                {
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();
                    
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
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();
                    
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
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();
                    
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
                    // Ensure database is created
                    dbContext.Database.EnsureCreated();
                    
                    // Create a new contact from the lead with ALL fields transferred
                    var contact = new Contact
                    {
                        FullName = lead.FullName,
                        Email = lead.Email,
                        Phone = lead.Phone,
                        Type = "Contact", // Set type to Contact for the new record
                        AvatarPath = lead.AvatarPath,
                        Occupation = lead.Occupation, // Transfer occupation
                        Salary = lead.Salary, // Transfer salary
                        CreatedAt = lead.CreatedAt, // Preserve original creation date
                        IsActive = true
                    };

                    // Add the contact to the database
                    dbContext.Contacts.Add(contact);
                    
                    // HARD DELETE: Completely remove the lead from the Leads table
                    var leadToDelete = dbContext.Leads.Find(lead.Id);
                    if (leadToDelete != null)
                    {
                        dbContext.Leads.Remove(leadToDelete);
                    }
                    
                    // Save changes in a single transaction to ensure data integrity
                    await dbContext.SaveChangesAsync();
                    
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
                System.Diagnostics.Debug.WriteLine($"Error moving lead to contact: {ex.Message}");
                MessageBox.Show($"Error moving lead to contact: {ex.Message}", "Error", 
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