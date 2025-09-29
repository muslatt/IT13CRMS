using System.ComponentModel;
using System.Linq;
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.ViewModels
{
    public class AgentsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public BindingList<User> Agents { get; } = new BindingList<User>();

        public void LoadAgents()
        {
            LoadAgents(null, null, "Name", false);
        }

        public void LoadAgents(bool? isActive, bool? isVerified, string? sortBy, bool desc)
        {
            Agents.Clear();
            try
            {
                using var db = DbContextHelper.CreateDbContext();
                var q = db.Users.Where(u => u.Role == UserRole.Agent);

                if (isActive.HasValue)
                    q = q.Where(u => u.IsActive == isActive.Value);
                if (isVerified.HasValue)
                    q = q.Where(u => u.IsEmailVerified == isVerified.Value);

                IOrderedQueryable<User>? ordered = null;
                switch ((sortBy ?? "Name").Trim().ToLowerInvariant())
                {
                    case "created":
                        ordered = desc ? q.OrderByDescending(u => u.CreatedAt) : q.OrderBy(u => u.CreatedAt);
                        break;
                    case "email":
                        ordered = desc ? q.OrderByDescending(u => u.Email) : q.OrderBy(u => u.Email);
                        break;
                    case "verified":
                        // bool sort: false < true; for "Verified first" we want desc
                        ordered = desc ? q.OrderByDescending(u => u.IsEmailVerified).ThenBy(u => u.LastName).ThenBy(u => u.FirstName)
                                       : q.OrderBy(u => u.IsEmailVerified).ThenBy(u => u.LastName).ThenBy(u => u.FirstName);
                        break;
                    case "active":
                        ordered = desc ? q.OrderByDescending(u => u.IsActive).ThenBy(u => u.LastName).ThenBy(u => u.FirstName)
                                       : q.OrderBy(u => u.IsActive).ThenBy(u => u.LastName).ThenBy(u => u.FirstName);
                        break;
                    case "name":
                    default:
                        ordered = desc
                            ? q.OrderByDescending(u => u.LastName).ThenByDescending(u => u.FirstName)
                            : q.OrderBy(u => u.LastName).ThenBy(u => u.FirstName);
                        break;
                }

                var list = (ordered ?? q.OrderBy(u => u.LastName).ThenBy(u => u.FirstName)).ToList();
                foreach (var a in list) Agents.Add(a);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to load agents: {ex.Message}");
            }
        }
    }
}
