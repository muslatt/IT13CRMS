using RealEstateCRMWinForms.Controls;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
using RealEstateCRMWinForms.ViewModels;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace RealEstateCRMWinForms.Views
{
    public partial class PendingAssignmentsView : UserControl
    {
        private readonly DealViewModel _dealViewModel;
        private FlowLayoutPanel _flow;
        private Label _lblHeader;
        private Panel _headerPanel;
        private Label _lblSubtitle;
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
            BackColor = Color.FromArgb(248, 249, 250);
            Dock = DockStyle.Fill;

            // Modern header panel with proper spacing
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90, // Increased height to prevent overlap
                BackColor = Color.White,
                Padding = new Padding(24, 20, 24, 20) // Increased padding
            };

            // Add subtle shadow to header
            _headerPanel.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    g.FillRectangle(shadowBrush, 0, _headerPanel.Height - 2, _headerPanel.Width, 2);
                }
            };

           

            _lblSubtitle = new Label
            {
                Text = _brokerMode ? "Monitor all agent assignments and their status" : "Review and manage your assigned property deals",
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(0, 35) // Adjusted position to prevent overlap
            };

            _headerPanel.Controls.Add(_lblSubtitle);
            _headerPanel.Controls.Add(_lblHeader);

            _flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(24),
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            // Enable double buffering for smoother scrolling
            typeof(FlowLayoutPanel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, _flow, new object[] { true });

            Controls.Add(_flow);
            Controls.Add(_headerPanel);
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
                    var emptyStatePanel = CreateEmptyStatePanel();
                    _flow.Controls.Add(emptyStatePanel);
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

        private Panel CreateEmptyStatePanel()
        {
            var emptyPanel = new Panel
            {
                Width = 420, // Increased width
                Height = 320, // Increased height
                BackColor = Color.White,
                Margin = new Padding(20)
            };

            // Add rounded corners and shadow
            emptyPanel.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new Rectangle(0, 0, emptyPanel.Width - 1, emptyPanel.Height - 1);
                var path = CreateRoundedRectanglePath(rect, 12);

                // Draw shadow
                var shadowRect = new Rectangle(3, 3, emptyPanel.Width - 3, emptyPanel.Height - 3);
                var shadowPath = CreateRoundedRectanglePath(shadowRect, 12);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }

                // Draw card background
                using (var cardBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(cardBrush, path);
                }

                // Draw border
                using (var borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            };

            var iconLabel = new Label
            {
                Text = "📋",
                Font = new Font("Segoe UI Emoji", 48f),
                ForeColor = Color.FromArgb(200, 200, 200),
                AutoSize = true,
                Location = new Point(185, 90) // Centered properly
            };

            var emptyLabel = new Label
            {
                Text = "No pending assignments",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(130, 170) // Adjusted position
            };

            var subtitleLabel = new Label
            {
                Text = _brokerMode ? "All assignments have been processed" : "You're all caught up!",
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Location = new Point(140, 200) // Adjusted position
            };

            emptyPanel.Controls.Add(subtitleLabel);
            emptyPanel.Controls.Add(emptyLabel);
            emptyPanel.Controls.Add(iconLabel);

            return emptyPanel;
        }

        private Control BuildAssignmentItem(Deal deal)
        {
            var wrapper = new Panel
            {
                Width = 360, // Increased width to prevent content overflow
                Height = 420, // Increased height to accommodate all content
                BackColor = Color.Transparent,
                Margin = new Padding(15),
                Padding = new Padding(0)
            };

            // Create modern card with rounded corners and shadow
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(24) // Increased padding
            };

            // Add custom paint for rounded corners and shadow
            card.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                var path = CreateRoundedRectanglePath(rect, 12);

                // Draw shadow
                var shadowRect = new Rectangle(3, 3, card.Width - 3, card.Height - 3);
                var shadowPath = CreateRoundedRectanglePath(shadowRect, 12);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }

                // Draw card background
                using (var cardBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(cardBrush, path);
                }

                // Draw border
                using (var borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            };

            // Property type badge - positioned to avoid overlap
            var badgePanel = new Panel
            {
                Size = new Size(85, 26), // Slightly larger
                Location = new Point(225, 15), // Adjusted position
                BackColor = Color.FromArgb(0, 123, 255)
            };

            badgePanel.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, badgePanel.Width, badgePanel.Height);
                var path = CreateRoundedRectanglePath(rect, 12);
                using (var brush = new SolidBrush(Color.FromArgb(0, 123, 255)))
                {
                    g.FillPath(brush, path);
                }
            };

            var badgeLabel = new Label
            {
                Text = "PENDING",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            badgePanel.Controls.Add(badgeLabel);

            // Title with icon - proper spacing
            var titlePanel = new Panel
            {
                Height = 40, // Increased height
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 8) // Add bottom margin
            };

            var propertyIcon = new Label
            {
                Text = "🏠",
                Font = new Font("Segoe UI Emoji", 16f),
                AutoSize = true,
                Location = new Point(0, 8)
            };

            var lblTitle = new Label
            {
                Text = deal.Property?.Title ?? deal.Title,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold), // Slightly smaller font
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(30, 10),
                Size = new Size(180, 28), // Adjusted size
                AutoEllipsis = true
            };

            titlePanel.Controls.Add(badgePanel);
            titlePanel.Controls.Add(lblTitle);
            titlePanel.Controls.Add(propertyIcon);

            // Address with icon - proper spacing
            var addressPanel = new Panel
            {
                Height = 35, // Increased height
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 5) // Add bottom margin
            };

            var addressIcon = new Label
            {
                Text = "📍",
                Font = new Font("Segoe UI Emoji", 12f),
                AutoSize = true,
                Location = new Point(5, 8)
            };

            var lblAddr = new Label
            {
                Text = deal.Property?.Address ?? "No address specified",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(30, 8),
                Size = new Size(280, 22), // Adjusted size
                AutoEllipsis = true
            };

            addressPanel.Controls.Add(lblAddr);
            addressPanel.Controls.Add(addressIcon);

            // Client info with icon - proper spacing
            var clientPanel = new Panel
            {
                Height = 35, // Increased height
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 5) // Add bottom margin
            };

            var clientIcon = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI Emoji", 12f),
                AutoSize = true,
                Location = new Point(5, 8)
            };

            var lblClient = new Label
            {
                Text = deal.Contact != null ? $"Client: {deal.Contact.FullName}" : "Client: Not specified",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(73, 80, 87),
                Location = new Point(30, 8),
                Size = new Size(280, 22), // Adjusted size
                AutoEllipsis = true
            };

            clientPanel.Controls.Add(lblClient);
            clientPanel.Controls.Add(clientIcon);

            // Assigned agent with icon - proper spacing
            var agentPanel = new Panel
            {
                Height = 35, // Increased height
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 5) // Add bottom margin
            };

            var agentIcon = new Label
            {
                Text = "🎯",
                Font = new Font("Segoe UI Emoji", 12f),
                AutoSize = true,
                Location = new Point(5, 8)
            };

            var assigned = ExtractAssignedAgent(deal.Notes);
            var lblAgent = new Label
            {
                Text = !string.IsNullOrWhiteSpace(assigned) ? $"Assigned to: {assigned}" : "Assigned to: Unknown",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(30, 8),
                Size = new Size(280, 22), // Adjusted size
                AutoEllipsis = true
            };

            agentPanel.Controls.Add(lblAgent);
            agentPanel.Controls.Add(agentIcon);

            // Price with icon - proper spacing
            var pricePanel = new Panel
            {
                Height = 45, // Increased height
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 10) // Add bottom margin
            };

            var priceIcon = new Label
            {
                Text = "💰",
                Font = new Font("Segoe UI Emoji", 16f),
                AutoSize = true,
                Location = new Point(5, 12)
            };

            var lblPrice = new Label
            {
                Text = deal.Property?.Price != null ? deal.Property.Price.ToString("C0") : "Price not set",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold), // Slightly smaller font
                ForeColor = Color.FromArgb(40, 167, 69),
                Location = new Point(35, 12),
                Size = new Size(280, 28) // Adjusted size
            };

            pricePanel.Controls.Add(lblPrice);
            pricePanel.Controls.Add(priceIcon);

            // Spacer for better separation
            var spacer = new Panel
            {
                Height = 15, // Reduced spacer height
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };

            // Action buttons panel - positioned at bottom with proper spacing
            var actions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 65, // Increased height
                BackColor = Color.Transparent,
                Padding = new Padding(0, 12, 0, 0) // Top padding
            };

            if (_brokerMode)
            {
                // Brokers get a view-only indicator
                var viewOnlyLabel = new Label
                {
                    Text = "👁️ View Only Mode",
                    Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                    ForeColor = Color.FromArgb(108, 117, 125),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                actions.Controls.Add(viewOnlyLabel);
            }
            else
            {
                var btnAccept = new Button
                {
                    Text = "✅ Accept",
                    BackColor = Color.FromArgb(34, 197, 94),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Height = 42, // Increased height
                    Width = 145, // Increased width
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Location = new Point(0, 12)
                };
                btnAccept.FlatAppearance.BorderSize = 0;
                btnAccept.Click += async (s, e) => await AcceptAssignmentAsync(deal);

                // Add hover effect
                btnAccept.MouseEnter += (s, e) => btnAccept.BackColor = Color.FromArgb(22, 163, 74);
                btnAccept.MouseLeave += (s, e) => btnAccept.BackColor = Color.FromArgb(34, 197, 94);

                var btnDecline = new Button
                {
                    Text = "❌ Decline",
                    BackColor = Color.FromArgb(239, 68, 68),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Height = 42, // Increased height
                    Width = 145, // Increased width
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Location = new Point(155, 12) // Adjusted position with proper spacing
                };
                btnDecline.FlatAppearance.BorderSize = 0;
                btnDecline.Click += async (s, e) => await DeclineAssignmentAsync(deal);

                // Add hover effect
                btnDecline.MouseEnter += (s, e) => btnDecline.BackColor = Color.FromArgb(220, 38, 38);
                btnDecline.MouseLeave += (s, e) => btnDecline.BackColor = Color.FromArgb(239, 68, 68);

                actions.Controls.Add(btnDecline);
                actions.Controls.Add(btnAccept);
            }

            // Add all panels to card in reverse order (due to Dock.Top)
            card.Controls.Add(actions);
            card.Controls.Add(spacer);
            card.Controls.Add(pricePanel);
            card.Controls.Add(agentPanel);
            card.Controls.Add(clientPanel);
            card.Controls.Add(addressPanel);
            card.Controls.Add(titlePanel);

            wrapper.Controls.Add(card);
            return wrapper;
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            var diameter = cornerRadius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
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
            // Confirm before accepting the assignment
            var confirmResult = MessageBox.Show(
                $"Are you sure you want to accept the assignment for '{deal.Title}'?\n\nThis will move the deal to your pipeline.",
                "Confirm Accept Assignment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            if (confirmResult != DialogResult.Yes)
                return;

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
                    MessageBox.Show(
                        $"Assignment for '{deal.Title}' has been successfully accepted and moved to your pipeline!",
                        "Assignment Accepted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
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