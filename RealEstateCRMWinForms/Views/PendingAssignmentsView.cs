using RealEstateCRMWinForms.Controls;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
using RealEstateCRMWinForms.ViewModels;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace RealEstateCRMWinForms.Views
{
    public partial class PendingAssignmentsView : UserControl
    {
        private readonly PropertyViewModel _propertyViewModel;
        private readonly DealViewModel _dealViewModel;
        private FlowLayoutPanel _flow;
        private Label _lblHeader;
        public event EventHandler<Property>? AssignmentAccepted;
        public event EventHandler<Property>? AssignmentDeclined;

        public PendingAssignmentsView()
        {
            _propertyViewModel = new PropertyViewModel();
            _dealViewModel = new DealViewModel();
            BuildUi();
            LoadAssignments();
        }

        private void BuildUi()
        {
            BackColor = Color.White;
            Dock = DockStyle.Fill;

            _lblHeader = new Label
            {
                Text = "Your Pending Assignments",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Dock = DockStyle.Top,
                Height = 36,
                Padding = new Padding(12, 8, 12, 8)
            };

            _flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(249, 250, 251),
                Padding = new Padding(16),
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            Controls.Add(_flow);
            Controls.Add(_lblHeader);
        }

        private void LoadAssignments()
        {
            var user = UserSession.Instance.CurrentUser;
            var agentName = (user != null) ? ($"{user.FirstName} {user.LastName}".Trim()) : string.Empty;
            var agentEmail = user?.Email ?? string.Empty;

            _flow.SuspendLayout();
            try
            {
                _flow.Controls.Clear();
                // Refresh in-memory collections
                _propertyViewModel.LoadProperties();
                _dealViewModel.LoadDeals();

                // Use existing Property.Agent string to identify assignments
                var assignments = _propertyViewModel.Properties
                    .Where(p => p.IsActive)
                    .Where(p =>
                        (!string.IsNullOrWhiteSpace(p.Agent) &&
                         (p.Agent.Equals(agentName, System.StringComparison.OrdinalIgnoreCase) ||
                          p.Agent.Contains(user?.FirstName ?? string.Empty, System.StringComparison.OrdinalIgnoreCase) ||
                          p.Agent.Contains(user?.LastName ?? string.Empty, System.StringComparison.OrdinalIgnoreCase) ||
                          p.Agent.Contains(agentEmail, System.StringComparison.OrdinalIgnoreCase))))
                    // Exclude properties that already have an active deal (means accepted)
                    .Where(p => !_dealViewModel.Deals.Any(d => d.IsActive && d.PropertyId == p.Id))
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();

                if (assignments.Count == 0)
                {
                    var empty = new Label
                    {
                        Text = "No pending assignments yet.",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 12f),
                        ForeColor = Color.FromArgb(90, 95, 100),
                        Padding = new Padding(6)
                    };
                    _flow.Controls.Add(empty);
                    return;
                }

                foreach (var p in assignments)
                {
                    var cardContainer = BuildAssignmentItem(p);
                    _flow.Controls.Add(cardContainer);
                }
            }
            finally
            {
                _flow.ResumeLayout();
            }
        }

        private Control BuildAssignmentItem(Property property)
        {
            var wrapper = new Panel
            {
                Width = 300,
                Height = 320,
                BackColor = Color.White,
                Margin = new Padding(10),
                Padding = new Padding(0)
            };

            var card = new PropertyCard { Dock = DockStyle.Top, Height = 260 };
            card.SetProperty(property);

            var actions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                BackColor = Color.White,
                Padding = new Padding(8)
            };

            var btnAccept = new Button
            {
                Text = "Accept",
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 36,
                Width = 100
            };
            btnAccept.FlatAppearance.BorderSize = 0;
            btnAccept.Click += async (s, e) => await AcceptAssignmentAsync(property);

            var btnDecline = new Button
            {
                Text = "Decline",
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 36,
                Width = 100,
                Left = 110
            };
            btnDecline.FlatAppearance.BorderSize = 0;
            btnDecline.Click += async (s, e) => await DeclineAssignmentAsync(property);

            actions.Controls.Add(btnDecline);
            actions.Controls.Add(btnAccept);

            wrapper.Controls.Add(actions);
            wrapper.Controls.Add(card);
            return wrapper;
        }

        private async Task AcceptAssignmentAsync(Property property)
        {
            var user = UserSession.Instance.CurrentUser;
            var agentName = ($"{user?.FirstName} {user?.LastName}".Trim());
            try
            {
                var deal = new Deal
                {
                    Title = string.IsNullOrWhiteSpace(property.Title) ? $"Deal for Property #{property.Id}" : property.Title,
                    Description = $"Auto-created from assignment for {agentName}",
                    PropertyId = property.Id,
                    ContactId = null,
                    Value = property.Price,
                    Status = BoardViewModel.NewBoardName,
                    CreatedBy = agentName,
                    IsActive = true
                };

                var ok = await _dealViewModel.AddDealAsync(deal);
                if (ok)
                {
                    LoadAssignments();
                    AssignmentAccepted?.Invoke(this, property);
                    MessageBox.Show("Assignment accepted. Deal created in New pipeline.", "Accepted",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to create deal for this assignment.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accepting assignment: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeclineAssignmentAsync(Property property)
        {
            try
            {
                var confirm = MessageBox.Show(
                    "Decline this property assignment?",
                    "Decline Assignment",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                if (confirm != DialogResult.Yes) return;

                property.Agent = string.Empty;
                if (_propertyViewModel.UpdateProperty(property))
                {
                    LoadAssignments();
                    AssignmentDeclined?.Invoke(this, property);
                }
                else
                {
                    MessageBox.Show("Failed to decline assignment.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error declining assignment: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
