using Microsoft.EntityFrameworkCore;
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using System.ComponentModel;
using System.Linq;

namespace RealEstateCRMWinForms.ViewModels
{
    public class PropertyRequestsViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        public BindingList<Property> Properties { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public PropertyRequestsViewModel()
        {
            _context = DbContextHelper.CreateDbContext();
            Properties = new BindingList<Property>();
        }

        public void LoadPropertyRequests()
        {
            LoadPropertyRequests(false);
        }

        public void LoadPropertyRequests(bool showRejected)
        {
            IQueryable<Property> query = _context.Properties
                .Include(p => p.ProofFiles)
                .Where(p => p.SubmittedByUserId.HasValue);

            if (showRejected)
            {
                // Load rejected properties
                query = query.Where(p => !p.IsApproved && !string.IsNullOrEmpty(p.RejectionReason));
            }
            else
            {
                // Load unapproved properties that haven't been rejected yet
                query = query.Where(p => !p.IsApproved && string.IsNullOrEmpty(p.RejectionReason));
            }

            var properties = query.ToList();
            Properties = new BindingList<Property>(properties);
            OnPropertyChanged(nameof(Properties));
        }

        public void ApproveProperty(int propertyId)
        {
            var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);
            if (property != null)
            {
                property.IsApproved = true;
                _context.SaveChanges();

                // Log broker approval scoped to this property
                try
                {
                    Services.LoggingService.LogAction(
                        "Broker Approved Property",
                        $"Approved '{property.Title}'",
                        propertyId: property.Id);
                }
                catch { /* logging safety */ }
            }
        }

        public void RejectProperty(int propertyId, string rejectionReason)
        {
            var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);
            if (property != null)
            {
                property.IsApproved = false; // Keep as not approved
                property.RejectionReason = rejectionReason;
                _context.SaveChanges();

                // Log broker rejection with reason scoped to this property
                try
                {
                    Services.LoggingService.LogAction(
                        "Broker Rejected Property",
                        $"Reason: {rejectionReason}",
                        propertyId: property.Id);
                }
                catch { /* logging safety */ }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
