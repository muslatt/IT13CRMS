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
        private readonly DealViewModel _dealViewModel;
        private FlowLayoutPanel _flow;
        private Label _lblHeader;
        public event EventHandler<Deal>? AssignmentAccepted;
        public event EventHandler<Deal>? AssignmentDeclined;

        private readonly bool _brokerMode;
        private const string AssignMarkerPrefix = "[ASSIGN:";

        public PendingAssignmentsView(bool brokerMode = false)
        {
            _dealViewModel = new DealViewModel();
            _brokerMode = brokerMode;
            BuildUi();
            LoadAssignments();
        }

        private void BuildUi()
        {
            BackColor = Color.White;
            Dock = DockStyle.Fill;

            _lblHeader = new Label
            {
                Text = _brokerMode ? "Pending Assignments" : "Your Pending Assignments",
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

            _flow.SuspendLayout();
            try
            {
                _flow.Controls.Clear();
                // Refresh deals
                _dealViewModel.LoadDeals();

                IEnumerable<Deal> pending;
                if (_brokerMode)
                {
                    // For brokers: show all active deals that have an assignment marker remaining
                    pending = _dealViewModel.Deals
                        .Where(d => d.IsActive)
                        .Where(d => !string.IsNullOrWhiteSpace(d.Notes) && d.Notes.Contains(AssignMarkerPrefix))
                        .OrderByDescending(d => d.CreatedAt)
                        .ToList();
                }
                else
                {
                    // For agents: only assignments tagged with their name
                    pending = _dealViewModel.Deals
                        .Where(d => d.IsActive)
                        .Where(d => !string.IsNullOrWhiteSpace(d.Notes) && d.Notes.Contains(AssignMarkerPrefix))
                        .Where(d => d.Notes!.IndexOf($"{AssignMarkerPrefix}{agentName}]", StringComparison.OrdinalIgnoreCase) >= 0)
                        .OrderByDescending(d => d.CreatedAt)
                        .ToList();
                }

                var assignments = pending.ToList();
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

                foreach (var d in assignments)
                {
                    var cardContainer = BuildAssignmentItem(d);
                    _flow.Controls.Add(cardContainer);
                }
            }
            finally
            {
                _flow.ResumeLayout();
            }
        }

        private Control BuildAssignmentItem(Deal deal)
        {
            var wrapper = new Panel
            {
                Width = 300,
                Height = 320,
                BackColor = Color.White,
                Margin = new Padding(10),
                Padding = new Padding(0)
            };

            // Build a simple summary panel for deal (property + contact + assigned agent)
            var summary = new Panel { Dock = DockStyle.Top, Height = 260, BackColor = Color.White };
            var lblTitle = new Label { Text = deal.Property?.Title ?? deal.Title, Font = new Font("Segoe UI", 11f, FontStyle.Bold), AutoSize = false, Height = 24, Dock = DockStyle.Top };
            var lblAddr = new Label { Text = deal.Property?.Address ?? "", Font = new Font("Segoe UI", 9.5f), AutoSize = false, Height = 22, Dock = DockStyle.Top, ForeColor = Color.DimGray };
            var lblClient = new Label { Text = deal.Contact != null ? $"Client: {deal.Contact.FullName}" : "Client: -", Font = new Font("Segoe UI", 10f), AutoSize = false, Height = 22, Dock = DockStyle.Top };
            var assigned = ExtractAssignedAgent(deal.Notes);
            var lblAgent = new Label { Text = !string.IsNullOrWhiteSpace(assigned) ? $"Assigned: {assigned}" : "Assigned:", Font = new Font("Segoe UI", 10f), AutoSize = false, Height = 22, Dock = DockStyle.Top };
            var lblPrice = new Label { Text = deal.Property?.Price != null ? $"Price: {deal.Property.Price:C0}" : "", Font = new Font("Segoe UI", 10f, FontStyle.Bold), AutoSize = false, Height = 24, Dock = DockStyle.Top, ForeColor = Color.FromArgb(0,123,255) };
            summary.Controls.Add(lblPrice);
            summary.Controls.Add(lblAgent);
            summary.Controls.Add(lblClient);
            summary.Controls.Add(lblAddr);
            summary.Controls.Add(lblTitle);

            var actions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                BackColor = Color.White,
                Padding = new Padding(8)
            };

            if (_brokerMode)
            {
                // Brokers just view; no accept/decline buttons
                actions.Visible = false;
                actions.Height = 0;
            }
            else
            {
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
                btnAccept.Click += async (s, e) => await AcceptAssignmentAsync(deal);

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
                btnDecline.Click += async (s, e) => await DeclineAssignmentAsync(deal);

                actions.Controls.Add(btnDecline);
                actions.Controls.Add(btnAccept);
            }

            wrapper.Controls.Add(actions);
            wrapper.Controls.Add(summary);
            return wrapper;
        }

        private string ExtractAssignedAgent(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes)) return string.Empty;
            var idx = notes.IndexOf(AssignMarkerPrefix, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return string.Empty;
            var end = notes.IndexOf(']', idx);
            if (end < 0) return string.Empty;
            var inside = notes.Substring(idx + AssignMarkerPrefix.Length, end - (idx + AssignMarkerPrefix.Length));
            return inside.Trim();
        }

        private async Task AcceptAssignmentAsync(Deal deal)
        {
            var user = UserSession.Instance.CurrentUser;
            var agentName = ($"{user?.FirstName} {user?.LastName}".Trim());
            try
            {
                // Remove assignment marker and set CreatedBy to agent
                if (!string.IsNullOrWhiteSpace(deal.Notes))
                {
                    var assigned = ExtractAssignedAgent(deal.Notes);
                    if (!string.IsNullOrWhiteSpace(assigned))
                    {
                        deal.Notes = deal.Notes.Replace($"{AssignMarkerPrefix}{assigned}]", string.Empty);
                    }
                }
                deal.CreatedBy = agentName;

                var ok = await _dealViewModel.UpdateDealAsync(deal);
                if (ok)
                {
                    LoadAssignments();
                    AssignmentAccepted?.Invoke(this, deal);
                    MessageBox.Show("Assignment accepted and moved to your pipeline.", "Accepted",
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

        private async Task DeclineAssignmentAsync(Deal deal)
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

                // Soft delete the pending deal
                if (_dealViewModel.DeleteDeal(deal))
                {
                    LoadAssignments();
                    AssignmentDeclined?.Invoke(this, deal);
                    MessageBox.Show("Assignment declined.", "Declined", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
