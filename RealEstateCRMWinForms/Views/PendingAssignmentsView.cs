using System.Collections.Generic;
using RealEstateCRMWinForms.Data;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
using RealEstateCRMWinForms.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class PendingAssignmentsView : UserControl
    {
        private readonly DealViewModel _dealViewModel;
        private FlowLayoutPanel _flow;
        private Label _lblHeader;
        private Panel _headerPanel;
        private Label _lblSubtitle;
        private readonly List<Panel> _cardWrappers = new();
        public event EventHandler<Deal>? AssignmentAccepted;
        public event EventHandler<Deal>? AssignmentDeclined;

        private readonly bool _brokerMode;
        private const string AssignMarkerPrefix = "[ASSIGN:";
        private const string AssignIdMarkerPrefix = "[ASSIGNID:";

        public PendingAssignmentsView(bool brokerMode = false)
        {
            _dealViewModel = new DealViewModel();
            _brokerMode = brokerMode;
            BuildUi();

            // DPI-aware scaling and smooth rendering
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            SizeChanged += (_, __) => UpdateCardLayout();
            LoadAssignments();
        }

        private void BuildUi()
        {
            BackColor = Color.FromArgb(248, 249, 250);
            Dock = DockStyle.Fill;

            // Header panel
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Padding = new Padding(24, 24, 24, 24)
            };
            _headerPanel.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                using var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0));
                var panelHeight = _headerPanel.Height;
                g.FillRectangle(shadowBrush, 0, panelHeight - 2, _headerPanel.Width, 2);
            };

            var headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _lblHeader = new Label
            {
                Text = _brokerMode ? "Assignment Monitor" : "Pending Assignments",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize = true,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 2)
            };
            _lblSubtitle = new Label
            {
                Text = _brokerMode ? "Monitor all agent assignments and their status" : "Review and manage your assigned property deals",
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 0, 0),
                MaximumSize = new Size(900, 0)
            };
            headerLayout.Controls.Add(_lblHeader, 0, 0);
            headerLayout.Controls.Add(_lblSubtitle, 0, 1);
            if (!_brokerMode)
            {
                _lblHeader.Visible = false;
                headerLayout.RowStyles[0].SizeType = SizeType.Absolute;
                headerLayout.RowStyles[0].Height = 0f;
                _lblSubtitle.Visible = false;
                headerLayout.RowStyles[1].SizeType = SizeType.Absolute;
                headerLayout.RowStyles[1].Height = 0f;
            }
            _headerPanel.Controls.Add(headerLayout);

            _flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(24),
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            Controls.Add(_flow);
            Controls.Add(_headerPanel);
        }

        // Public entry point for parent views to refresh the list
        public void RefreshAssignments()
        {
            LoadAssignments();
        }

        private bool HasAssignmentMarker(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes)) return false;
            return notes.IndexOf(AssignMarkerPrefix, StringComparison.OrdinalIgnoreCase) >= 0
                || notes.IndexOf(AssignIdMarkerPrefix, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void LoadAssignments()
        {
            var user = UserSession.Instance.CurrentUser;
            var agentName = (user != null) ? ($"{user.FirstName} {user.LastName}".Trim()) : string.Empty;

            _flow.SuspendLayout();
            try
            {
                _flow.Controls.Clear();
                _cardWrappers.Clear();

                using var db = DbContextHelper.CreateDbContext();
                var dealsSnapshot = db.Deals
                    .AsNoTracking()
                    .Include(d => d.Property)
                    .Include(d => d.Contact)
                    .Where(d => d.IsActive)
                    .ToList();

                IEnumerable<Deal> pending;
                if (_brokerMode)
                {
                    // Broker view: show any deal that is pending assignment by either
                    // 1) having the [ASSIGN:Agent] marker, or
                    // 2) legacy/older items that have no owner yet (CreatedBy empty)
                    pending = dealsSnapshot
                        .Where(d => HasAssignmentMarker(d.Notes) || string.IsNullOrWhiteSpace(d.CreatedBy))
                        .OrderByDescending(d => d.CreatedAt)
                        .ToList();
                }
                else
                {
                    var agentId = user?.Id;
                    var idMarker = agentId.HasValue ? $"{AssignIdMarkerPrefix}{agentId.Value}]" : null;
                    var nameMarker = !string.IsNullOrWhiteSpace(agentName) ? $"{AssignMarkerPrefix}{agentName}]" : null;

                    pending = dealsSnapshot
                        .Where(d => HasAssignmentMarker(d.Notes))
                        .Where(d =>
                        {
                            var notes = d.Notes ?? string.Empty;
                            if (!string.IsNullOrEmpty(idMarker) && notes.IndexOf(idMarker, StringComparison.OrdinalIgnoreCase) >= 0)
                                return true;
                            if (!string.IsNullOrEmpty(nameMarker) && notes.IndexOf(nameMarker, StringComparison.OrdinalIgnoreCase) >= 0)
                                return true;
                            return false;
                        })
                        .OrderByDescending(d => d.CreatedAt)
                        .ToList();
                }

                var assignments = pending.ToList();
                if (_brokerMode && _lblSubtitle != null && _lblSubtitle.Visible)
                {
                    var countText = assignments.Count == 1 ? "1 pending assignment" : $"{assignments.Count} pending assignments";
                    var baseline = _brokerMode
                        ? "Monitor all agent assignments and their status"
                        : "Review and manage your assigned property deals";
                    var availableWidth = Math.Max(0, _headerPanel.Width - _headerPanel.Padding.Horizontal);
                    _lblSubtitle.MaximumSize = new Size(availableWidth, 0);
                    _lblSubtitle.Text = $"{baseline} - {countText}";
                }
                if (assignments.Count == 0)
                {
                    var emptyStatePanel = CreateEmptyStatePanel();
                    _flow.Controls.Add(emptyStatePanel);
                    UpdateCardLayout();
                    return;
                }

                foreach (var d in assignments)
                {
                    var cardContainer = BuildAssignmentItem(d);
                    _cardWrappers.Add(cardContainer);
                    _flow.Controls.Add(cardContainer);
                }
                UpdateCardLayout();
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
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Margin = new Padding(20),
                Padding = new Padding(24)
            };

            emptyPanel.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new Rectangle(0, 0, emptyPanel.Width - 1, emptyPanel.Height - 1);
                var path = CreateRoundedRectanglePath(rect, 12);

                var shadowRect = new Rectangle(3, 3, emptyPanel.Width - 3, emptyPanel.Height - 3);
                var shadowPath = CreateRoundedRectanglePath(shadowRect, 12);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
                using (var cardBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(cardBrush, path);
                }
                using (var borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            };

            var iconLabel = new Label
            {
                Text = "\uE8FA",
                Font = new Font("Segoe MDL2 Assets", 56f, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(200, 200, 200),
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0)
            };
            iconLabel.Anchor = AnchorStyles.None;

            var emptyLabel = new Label
            {
                Text = "No pending assignments",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 8, 0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            emptyLabel.Anchor = AnchorStyles.None;

            var subtitleLabel = new Label
            {
                Text = _brokerMode ? "All assignments have been processed" : "You're all caught up!",
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 4, 0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            subtitleLabel.Anchor = AnchorStyles.None;

            var emptyLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                AutoSize = true,
                Padding = new Padding(0)
            };
            emptyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            emptyLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            emptyLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            emptyLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            emptyLayout.Controls.Add(iconLabel, 0, 0);
            emptyLayout.Controls.Add(emptyLabel, 0, 1);
            emptyLayout.Controls.Add(subtitleLabel, 0, 2);
            emptyPanel.Controls.Add(emptyLayout);

            return emptyPanel;
        }

        private Panel BuildAssignmentItem(Deal deal)
        {
            var wrapper = new Panel
            {
                AutoSize = false,
                BackColor = Color.Transparent,
                Margin = new Padding(15),
                Padding = new Padding(0),
                MinimumSize = new Size(320, 0)
            };

            var card = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Padding = new Padding(24),
                Margin = new Padding(0),
                MinimumSize = new Size(320, 220)
            };
            card.Paint += (sender, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                var path = CreateRoundedRectanglePath(rect, 12);
                var shadowRect = new Rectangle(3, 3, card.Width - 3, card.Height - 3);
                var shadowPath = CreateRoundedRectanglePath(shadowRect, 12);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0))) g.FillPath(shadowBrush, shadowPath);
                using (var cardBrush = new SolidBrush(Color.White)) g.FillPath(cardBrush, path);
                using (var borderPen = new Pen(Color.FromArgb(230, 230, 230), 1)) g.DrawPath(borderPen, path);
            };

            var titlePanel = new Panel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Top, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 6) };
            var titleLayout = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };
            var lblTitle = new Label
            {
                Text = string.IsNullOrWhiteSpace(deal.Title) ? "Untitled Deal" : deal.Title,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = true,
                AutoEllipsis = true
            };
            var statusBadge = new Label
            {
                Text = "Pending",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(234, 179, 8),
                AutoSize = true,
                Padding = new Padding(10, 4, 10, 4),
                Margin = new Padding(12, 4, 0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            titleLayout.Controls.Add(lblTitle);
            titleLayout.Controls.Add(statusBadge);
            titlePanel.Controls.Add(titleLayout);

            string addressText = deal.Property?.Address;
            if (string.IsNullOrWhiteSpace(addressText))
            {
                addressText = !string.IsNullOrWhiteSpace(deal.Description) ? deal.Description : "Address not set";
            }
            var addressPanel = new Panel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Top, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 5) };
            var lblAddress = new Label
            {
                Text = $"Address: {addressText}",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(73, 80, 87),
                AutoSize = true,
                MaximumSize = new Size(320, 0)
            };
            addressPanel.Controls.Add(lblAddress);

            var clientPanel = new Panel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Top, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 5) };
            var clientName = (deal.Contact?.FullName?.Trim())?.Length > 0 ? deal.Contact!.FullName : "Unknown";
            var lblClient = new Label
            {
                Text = $"Client: {clientName}",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(73, 80, 87),
                AutoSize = true,
                MaximumSize = new Size(320, 0)
            };
            clientPanel.Controls.Add(lblClient);

            var agentPanel = new Panel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Top, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 5) };
            var assigned = ExtractAssignedAgent(deal.Notes);
            var agentText = !string.IsNullOrWhiteSpace(assigned) ? $"Assigned to: {assigned}" : "Awaiting agent acceptance";
            var agentColor = !string.IsNullOrWhiteSpace(assigned) ? Color.FromArgb(0, 123, 255) : Color.FromArgb(108, 117, 125);
            var lblAgent = new Label
            {
                Text = agentText,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = agentColor,
                AutoSize = true,
                MaximumSize = new Size(320, 0)
            };
            agentPanel.Controls.Add(lblAgent);

            var pricePanel = new Panel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Top, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 10) };
            string valueText;
            var priceValue = deal.Property?.Price;
            if (priceValue.HasValue)
            {
                valueText = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C0}", priceValue.Value);
            }
            else
            {
                valueText = "Not set";
            }
            var lblPrice = new Label
            {
                Text = $"Value: {valueText}",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                AutoSize = true,
                MaximumSize = new Size(320, 0)
            };
            pricePanel.Controls.Add(lblPrice);

            var spacer = new Panel { Height = 10, Dock = DockStyle.Top, BackColor = Color.Transparent };

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 12, 0, 0),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            if (_brokerMode)
            {
                var viewOnlyLabel = new Label
                {
                    Text = "View Only Mode",
                    Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                    ForeColor = Color.FromArgb(108, 117, 125),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(0, 12, 0, 0),
                    AutoSize = true
                };
                actions.Controls.Add(viewOnlyLabel);
            }
            else
            {
                var btnAccept = new Button
                {
                    Text = "Accept",
                    BackColor = Color.FromArgb(34, 197, 94),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Height = 42,
                    Width = 145,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Margin = new Padding(0, 12, 10, 12)
                };
                btnAccept.FlatAppearance.BorderSize = 0;
                btnAccept.Click += async (s, e) => await AcceptAssignmentAsync(deal);
                btnAccept.MouseEnter += (s, e) => btnAccept.BackColor = Color.FromArgb(22, 163, 74);
                btnAccept.MouseLeave += (s, e) => btnAccept.BackColor = Color.FromArgb(34, 197, 94);

                var btnDecline = new Button
                {
                    Text = "Decline",
                    BackColor = Color.FromArgb(239, 68, 68),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Height = 42,
                    Width = 145,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Margin = new Padding(10, 12, 0, 12)
                };
                btnDecline.FlatAppearance.BorderSize = 0;
                btnDecline.Click += async (s, e) => await DeclineAssignmentAsync(deal);
                btnDecline.MouseEnter += (s, e) => btnDecline.BackColor = Color.FromArgb(220, 38, 38);
                btnDecline.MouseLeave += (s, e) => btnDecline.BackColor = Color.FromArgb(239, 68, 68);

                actions.Controls.Add(btnDecline);
                actions.Controls.Add(btnAccept);
            }

            card.Controls.Add(actions);
            card.Controls.Add(spacer);
            card.Controls.Add(pricePanel);
            card.Controls.Add(agentPanel);
            card.Controls.Add(clientPanel);
            card.Controls.Add(addressPanel);
            card.Controls.Add(titlePanel);

            wrapper.Controls.Add(card);
            
            // Remove assignment preview on click (no-op for broker mode)
            
            card.PerformLayout();
            wrapper.Height = card.PreferredSize.Height + wrapper.Padding.Vertical;
            return wrapper;
        }

        // Preview removed per request – no click handling needed

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
                if (!string.IsNullOrWhiteSpace(deal.Notes))
                {
                    var assigned = ExtractAssignedAgent(deal.Notes);
                    if (!string.IsNullOrWhiteSpace(assigned))
                    {
                        deal.Notes = deal.Notes.Replace($"{AssignMarkerPrefix}{assigned}]", string.Empty);
                    }
                    var userId = user?.Id;
                    if (userId.HasValue)
                    {
                        deal.Notes = deal.Notes.Replace($"{AssignIdMarkerPrefix}{userId.Value}]", string.Empty);
                    }
                    // Normalize whitespace
                    deal.Notes = deal.Notes.Trim();
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

        private void UpdateCardLayout()
        {
            if (_flow == null) return;
            var innerWidth = _flow.ClientSize.Width - _flow.Padding.Horizontal;
            if (innerWidth <= 0) return;

            int columns;
            if (innerWidth < 500) columns = 1;
            else if (innerWidth < 900) columns = 2;
            else columns = 3;

            var totalMargins = columns * 30;
            var cardWidth = Math.Max(300, (innerWidth - totalMargins) / columns);

            foreach (var panel in _cardWrappers)
            {
                panel.Width = cardWidth;
                if (panel.Controls.Count > 0 && panel.Controls[0] is Panel cardPanel)
                {
                    cardPanel.MaximumSize = new Size(cardWidth, 0);
                    cardPanel.Width = cardWidth;
                }
            }
        }
    }
}












