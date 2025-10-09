using Microsoft.EntityFrameworkCore;
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
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
            LoadProperties(false);
        }

        public void LoadProperties(bool isClientView)
        {
            LoadProperties(isClientView, false);
        }

        public void LoadProperties(bool isClientView, bool isBrowseMode)
        {
            LoadProperties(isClientView, isBrowseMode, false);
        }

        public void LoadProperties(bool isClientView, bool isBrowseMode, bool isMyListingsMode)
        {
            LoadProperties(isClientView, isBrowseMode, isMyListingsMode, false);
        }

        public void LoadProperties(bool isClientView, bool isBrowseMode, bool isMyListingsMode, bool showApprovedOnly)
        {
            LoadProperties(isClientView, isBrowseMode, isMyListingsMode, showApprovedOnly, false, false);
        }

        public void LoadProperties(bool isClientView, bool isBrowseMode, bool isMyListingsMode, bool showApprovedOnly, bool showPendingApproval)
        {
            LoadProperties(isClientView, isBrowseMode, isMyListingsMode, showApprovedOnly, showPendingApproval, false);
        }

        public void LoadProperties(bool isClientView, bool isBrowseMode, bool isMyListingsMode, bool showApprovedOnly, bool showPendingApproval, bool showRejected)
        {
            var currentUser = Services.UserSession.Instance.CurrentUser;
            var query = _context.Properties
                .Include(p => p.ProofFiles)
                .Where(p => p.IsActive);

            // Exclude properties that have closed deals (either active with "Closed" status or inactive deals that were closed)
            var closedPropertyIds = _context.Deals
                .Where(d => (d.Status.ToLower() == "closed" || d.Status.ToLower() == "lost") ||
                           (!d.IsActive && d.ClosedAt != null))
                .Select(d => d.PropertyId)
                .Distinct()
                .ToList();

            query = query.Where(p => !closedPropertyIds.Contains(p.Id));

            if (isBrowseMode)
            {
                // In browse mode, clients see only approved properties from all users
                query = query.Where(p => p.IsApproved);
            }
            else if (isMyListingsMode && currentUser != null && currentUser.Role == UserRole.Client)
            {
                // In My Listings mode for clients, show:
                // 1. Properties they submitted themselves
                // 2. Properties they have active deals on (where Contact.Email matches their email)

                // Get property IDs from active deals where the client is the contact
                var dealsPropertyIds = _context.Deals
                    .Include(d => d.Contact)
                    .Where(d => d.IsActive &&
                                d.Contact != null &&
                                d.Contact.Email == currentUser.Email &&
                                d.Status.ToLower() != "closed" &&
                                d.Status.ToLower() != "lost")
                    .Select(d => d.PropertyId)
                    .Distinct()
                    .ToList();

                // Show properties that the client submitted OR properties they have active deals on
                query = query.Where(p => p.SubmittedByUserId == currentUser.Id || dealsPropertyIds.Contains(p.Id));

                // Apply filters based on approval status
                if (showApprovedOnly && !showPendingApproval && !showRejected)
                {
                    // Show only approved properties
                    query = query.Where(p => p.IsApproved && string.IsNullOrEmpty(p.RejectionReason));
                }
                else if (!showApprovedOnly && showPendingApproval && !showRejected)
                {
                    // Show only pending approval properties
                    query = query.Where(p => !p.IsApproved && string.IsNullOrEmpty(p.RejectionReason));
                }
                else if (!showApprovedOnly && !showPendingApproval && showRejected)
                {
                    // Show only rejected properties
                    query = query.Where(p => !string.IsNullOrEmpty(p.RejectionReason));
                }
                else if (showApprovedOnly && showPendingApproval && !showRejected)
                {
                    // Show approved and pending (not rejected)
                    query = query.Where(p => string.IsNullOrEmpty(p.RejectionReason));
                }
                else if (showApprovedOnly && !showPendingApproval && showRejected)
                {
                    // Show approved and rejected
                    query = query.Where(p => p.IsApproved || !string.IsNullOrEmpty(p.RejectionReason));
                }
                else if (!showApprovedOnly && showPendingApproval && showRejected)
                {
                    // Show pending and rejected
                    query = query.Where(p => !p.IsApproved || !string.IsNullOrEmpty(p.RejectionReason));
                }
                // If all are selected or none are selected, show all properties
            }
            else if (isClientView && currentUser != null && currentUser.Role == UserRole.Client)
            {
                // Clients see all their properties (approved, pending, and rejected)
                query = query.Where(p => p.SubmittedByUserId == currentUser.Id);
            }
            else
            {
                // Brokers/agents see all approved properties
                query = query.Where(p => p.IsApproved);
            }

            var properties = query.ToList();
            Properties = new BindingList<Property>(properties);
            OnPropertyChanged(nameof(Properties));

            // Notify global listeners (Dashboard subscribes to this)
            PropertiesUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void LoadAllProperties()
        {
            // Load all properties (including inactive ones)
            var properties = _context.Properties
                .Include(p => p.ProofFiles)
                .ToList();
            Properties = new BindingList<Property>(properties);
            OnPropertyChanged(nameof(Properties));

            // Notify global listeners
            PropertiesUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void LoadPropertyRequests()
        {
            // Load pending property requests (not approved)
            // Exclude properties that have closed deals
            var closedPropertyIds = _context.Deals
                .Where(d => (d.Status.ToLower() == "closed" || d.Status.ToLower() == "lost") ||
                           (!d.IsActive && d.ClosedAt != null))
                .Select(d => d.PropertyId)
                .Distinct()
                .ToList();

            var properties = _context.Properties
                .Include(p => p.ProofFiles)
                .Where(p => p.IsActive && !p.IsApproved && !closedPropertyIds.Contains(p.Id))
                .ToList();
            Properties = new BindingList<Property>(properties);
            OnPropertyChanged(nameof(Properties));

            // Notify global listeners
            PropertiesUpdated?.Invoke(this, EventArgs.Empty);
        }

        public Property? GetPropertyById(int id)
        {
            try
            {
                return _context.Properties
                    .Include(p => p.ProofFiles)
                    .FirstOrDefault(p => p.Id == id);
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

                // Log the action (property-scoped)
                LoggingService.LogAction("Created Property", $"Property '{property.Title}' created", propertyId: property.Id);

                // Instead of reloading, add to local list
                Properties.Add(property);
                OnPropertyChanged(nameof(Properties));
                // Notify global listeners (Dashboard subscribes to this)
                PropertiesUpdated?.Invoke(this, EventArgs.Empty);
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

                    // Log the action (property-scoped)
                    LoggingService.LogAction("Updated Property", $"Property '{propertyToUpdate.Title}' updated", propertyId: propertyToUpdate.Id);

                    // Instead of reloading, update local list
                    var existing = Properties.FirstOrDefault(p => p.Id == propertyToUpdate.Id);
                    if (existing != null)
                    {
                        var index = Properties.IndexOf(existing);
                        Properties[index] = propertyToUpdate;
                        OnPropertyChanged(nameof(Properties));
                        // Notify global listeners
                        PropertiesUpdated?.Invoke(this, EventArgs.Empty);
                    }
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

                    // Log the action (property-scoped)
                    LoggingService.LogAction("Deleted Property", $"Property '{propertyToDelete.Title}' deleted", propertyId: propertyToDelete.Id);

                    // Instead of reloading, remove from local list
                    var existing = Properties.FirstOrDefault(p => p.Id == propertyToDelete.Id);
                    if (existing != null)
                    {
                        Properties.Remove(existing);
                        OnPropertyChanged(nameof(Properties));
                        // Notify global listeners
                        PropertiesUpdated?.Invoke(this, EventArgs.Empty);
                    }
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

        public bool AddPropertyProofFile(PropertyProofFile proofFile)
        {
            try
            {
                _context.PropertyProofFiles.Add(proofFile);
                _context.SaveChanges();

                // Log the action (property-scoped)
                LoggingService.LogAction("Added Proof File", $"Proof file '{proofFile.FileName}' added", propertyId: proofFile.PropertyId);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error adding proof file: {ex.Message}");
                return false;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
