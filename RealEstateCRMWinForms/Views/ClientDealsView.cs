using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace RealEstateCRMWinForms.Views
{
    public partial class ClientDealsView : UserControl
    {
        private FlowLayoutPanel flowLayoutPanel;
        private Label lblNoPendingRequests;
        private Button btnRefresh;
        private System.Windows.Forms.Timer refreshTimer;

        public ClientDealsView()
        {
            InitializeComponent();
            SetupAutoRefresh();
            LoadDealsWithPendingRequests();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Header panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };

            var lblTitle = new Label
            {
                Text = string.Empty,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Left
            };

            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Font = new Font("Segoe UI", 10F),

                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            var rightHost = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, BackColor = Color.Transparent, Padding = new Padding(0), Margin = new Padding(0) }; rightHost.Controls.Add(btnRefresh); headerPanel.Controls.Add(rightHost); headerPanel.Controls.Add(lblTitle);

            // Flow layout panel for deal cards with improved settings
            flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(245, 245, 245),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Handle resize to ensure cards fit properly
            flowLayoutPanel.Resize += (s, e) =>
            {
                foreach (Control control in flowLayoutPanel.Controls)
                {
                    if (control is Panel card && control != lblNoPendingRequests)
                    {
                        var newWidth = Math.Max(flowLayoutPanel.ClientSize.Width - 60, 400);
                        card.Width = newWidth;

                        // Update status badge position for all cards
                        UpdateStatusBadgePosition(card, newWidth);
                    }
                }
            };

            // No pending requests label
            lblNoPendingRequests = new Label
            {
                Text = "You have no pending deal status change requests.",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            flowLayoutPanel.Controls.Add(lblNoPendingRequests);

            this.Controls.Add(flowLayoutPanel);
            this.Controls.Add(headerPanel);
            this.BackColor = Color.FromArgb(245, 245, 245);

            this.ResumeLayout(false);
        }

        private void UpdateStatusBadgePosition(Panel card, int cardWidth)
        {
            // Find the status badge panel in the card
            foreach (Control control in card.Controls)
            {
                if (control is Panel statusPanel && control.Tag?.ToString() == "StatusBadge")
                {
                    statusPanel.Location = new Point(cardWidth - 145, statusPanel.Location.Y);
                    break;
                }
            }
        }

        private void SetupAutoRefresh()
        {
            // Set up timer to auto-refresh every 30 seconds
            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 30000, // 30 seconds
                Enabled = true
            };
            refreshTimer.Tick += (s, e) => LoadDealsWithPendingRequests();
        }

        // Override to handle visibility changes and refresh content
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
            if (value && this.IsHandleCreated)
            {
                // Refresh content when becoming visible
                this.BeginInvoke(new Action(() => LoadDealsWithPendingRequests()));
            }
        }

        // Handle when the control becomes visible (e.g., tab switch)
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                LoadDealsWithPendingRequests();
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadDealsWithPendingRequests();
        }

        private void LoadDealsWithPendingRequests()
        {
            // Prevent multiple simultaneous loads
            if (flowLayoutPanel.Tag as bool? == true) return;
            flowLayoutPanel.Tag = true;

            try
            {
                // Suspend layout to prevent flickering
                this.SuspendLayout();
                flowLayoutPanel.SuspendLayout();

                // Clear existing cards except the "no requests" label
                var controlsToRemove = flowLayoutPanel.Controls.Cast<Control>()
                    .Where(c => c != lblNoPendingRequests)
                    .ToList();

                foreach (var control in controlsToRemove)
                {
                    flowLayoutPanel.Controls.Remove(control);
                    control.Dispose();
                }

                var currentUser = UserSession.Instance.CurrentUser;
                if (currentUser == null || currentUser.Role != UserRole.Client)
                {
                    lblNoPendingRequests.Text = "Access denied. This section is for clients only.";
                    lblNoPendingRequests.Visible = true;
                    return;
                }

                try
                {
                    using var db = Data.DbContextHelper.CreateDbContext();

                    // Get ALL active deals where the client is involved (excluding closed/lost deals)
                    var allActiveDeals = db.Deals
                        .Include(d => d.Property)
                        .Include(d => d.Contact)
                        .Where(d => d.Contact != null &&
                                   d.Contact.Email == currentUser.Email &&
                                   d.IsActive &&
                                   !d.Status.ToLower().Contains("closed") &&
                                   d.Status.ToLower() != "lost")
                        .OrderByDescending(d => d.UpdatedAt ?? d.CreatedAt)
                        .ToList();

                    if (!allActiveDeals.Any())
                    {
                        lblNoPendingRequests.Text = "You have no active deals at the moment.";
                        lblNoPendingRequests.Visible = true;
                        return;
                    }

                    lblNoPendingRequests.Visible = false;

                    // For each deal, check if there are pending requests and create appropriate card
                    foreach (var deal in allActiveDeals)
                    {
                        var pendingRequests = db.DealStatusChangeRequests
                            .Include(r => r.RequestedBy)
                            .Where(r => r.DealId == deal.Id && r.IsApproved == null)
                            .OrderByDescending(r => r.CreatedAt)
                            .ToList();

                        Panel card;
                        if (pendingRequests.Any())
                        {
                            // Deal has pending approval requests
                            card = CreateDealCardWithApproval(deal, pendingRequests);
                        }
                        else
                        {
                            // Deal has no pending requests, just show active deal status
                            card = CreateActiveDealCard(deal);
                        }

                        flowLayoutPanel.Controls.Add(card);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading deals: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                // Resume layout and force complete refresh
                flowLayoutPanel.ResumeLayout(true);
                this.ResumeLayout(true);

                // Force complete layout recalculation
                flowLayoutPanel.PerformLayout();
                this.PerformLayout();

                // Refresh the display
                flowLayoutPanel.Invalidate();
                this.Invalidate();

                // Reset loading flag
                flowLayoutPanel.Tag = false;
            }
        }

        private Panel CreateDealCardWithApproval(Deal deal, System.Collections.Generic.List<DealStatusChangeRequest> pendingRequests)
        {
            var cardWidth = Math.Max(flowLayoutPanel.ClientSize.Width - 60, 400);

            var card = new Panel
            {
                Width = cardWidth,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(15),
                Margin = new Padding(0, 0, 0, 15),
                AutoSize = false // We'll calculate height manually
            };

            // Add shadow effect
            card.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(230, 230, 230), 1),
                    0, 0, card.Width - 1, card.Height - 1);
            };

            int yPos = 15;
            int contentWidth = cardWidth - 30; // Account for padding

            // Deal title and current status badge
            var lblDealTitle = new Label
            {
                Text = deal.Title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(15, yPos),
                Size = new Size(contentWidth - 150, 0), // Leave space for status badge
                AutoSize = true
            };
            card.Controls.Add(lblDealTitle);

            // Current status badge (top-right corner) - Fixed positioning
            var statusColor = GetStatusColor(deal.Status);
            var pnlStatusBadge = new Panel
            {
                Location = new Point(cardWidth - 145, yPos), // This will be updated after card is added
                Size = new Size(130, 30),
                BackColor = statusColor,
                Tag = "StatusBadge" // Tag to identify this as a status badge
            };

            var lblStatusBadge = new Label
            {
                Text = deal.Status,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(130, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlStatusBadge.Controls.Add(lblStatusBadge);
            card.Controls.Add(pnlStatusBadge);

            yPos += Math.Max(lblDealTitle.Height, 30) + 15;

            // Property info
            var propertyInfo = $"Property: {deal.Property?.Title ?? "N/A"}";
            if (!string.IsNullOrEmpty(deal.Property?.Address))
            {
                propertyInfo += $"\nAddress: {deal.Property.Address}";
            }

            var lblInfo = new Label
            {
                Text = propertyInfo,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                Location = new Point(15, yPos),
                Size = new Size(contentWidth, 0),
                AutoSize = true
            };
            card.Controls.Add(lblInfo);

            yPos += lblInfo.Height + 20;

            // Show the most recent pending request
            var latestRequest = pendingRequests.First();

            var lblRequestTitle = new Label
            {
                Text = "⚠️ Status Change Request",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 140, 0),
                Location = new Point(15, yPos),
                AutoSize = true
            };
            card.Controls.Add(lblRequestTitle);

            yPos += lblRequestTitle.Height + 8;

            var requestText = $"Agent {latestRequest.RequestedBy?.FullName ?? "Unknown"} requests to move this deal from " +
                           $"'{latestRequest.PreviousStatus}' to '{latestRequest.RequestedStatus}'";

            var lblRequestDetails = new Label
            {
                Text = requestText,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(15, yPos),
                Size = new Size(contentWidth, 0),
                AutoSize = true
            };
            card.Controls.Add(lblRequestDetails);

            yPos += lblRequestDetails.Height + 8;

            var lblRequestDate = new Label
            {
                Text = $"Requested on: {latestRequest.CreatedAt.ToString("MMM dd, yyyy hh:mm tt")}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Location = new Point(15, yPos),
                AutoSize = true
            };
            card.Controls.Add(lblRequestDate);

            yPos += lblRequestDate.Height + 20;

            // Action buttons
            var btnApprove = new Button
            {
                Text = "✓ Approve",
                Size = new Size(120, 35),
                Location = new Point(15, yPos),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Tag = latestRequest
            };
            btnApprove.FlatAppearance.BorderSize = 0;
            btnApprove.Click += BtnApprove_Click;
            card.Controls.Add(btnApprove);

            var btnReject = new Button
            {
                Text = "✗ Reject",
                Size = new Size(120, 35),
                Location = new Point(145, yPos),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Tag = latestRequest
            };
            btnReject.FlatAppearance.BorderSize = 0;
            btnReject.Click += BtnReject_Click;
            card.Controls.Add(btnReject);

            // Set final card height based on content
            yPos += 35 + 15; // button height + bottom padding
            card.Height = yPos;

            // Fix status badge position after card is fully constructed
            card.HandleCreated += (s, e) =>
            {
                UpdateStatusBadgePosition(card, card.Width);
            };

            return card;
        }

        private Panel CreateActiveDealCard(Deal deal)
        {
            var cardWidth = Math.Max(flowLayoutPanel.ClientSize.Width - 60, 400);

            var card = new Panel
            {
                Width = cardWidth,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(15),
                Margin = new Padding(0, 0, 0, 15),
                AutoSize = false // We'll calculate height manually
            };

            // Add shadow effect
            card.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(230, 230, 230), 1),
                    0, 0, card.Width - 1, card.Height - 1);
            };

            int yPos = 15;
            int contentWidth = cardWidth - 30; // Account for padding

            // Deal title and current status badge
            var lblDealTitle = new Label
            {
                Text = deal.Title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(15, yPos),
                Size = new Size(contentWidth - 150, 0), // Leave space for status badge
                AutoSize = true
            };
            card.Controls.Add(lblDealTitle);

            // Current status badge (top-right corner) - Fixed positioning
            var statusColor = GetStatusColor(deal.Status);
            var pnlStatusBadge = new Panel
            {
                Location = new Point(cardWidth - 145, yPos), // This will be updated after card is added
                Size = new Size(130, 30),
                BackColor = statusColor,
                Tag = "StatusBadge" // Tag to identify this as a status badge
            };

            var lblStatusBadge = new Label
            {
                Text = deal.Status,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(130, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlStatusBadge.Controls.Add(lblStatusBadge);
            card.Controls.Add(pnlStatusBadge);

            yPos += Math.Max(lblDealTitle.Height, 30) + 15;

            // Property info
            var propertyInfo = $"Property: {deal.Property?.Title ?? "N/A"}";
            if (!string.IsNullOrEmpty(deal.Property?.Address))
            {
                propertyInfo += $"\nAddress: {deal.Property.Address}";
            }

            var lblInfo = new Label
            {
                Text = propertyInfo,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                Location = new Point(15, yPos),
                Size = new Size(contentWidth, 0),
                AutoSize = true
            };
            card.Controls.Add(lblInfo);

            yPos += lblInfo.Height + 15;

            // Deal value
            if (deal.Value.HasValue && deal.Value.Value > 0)
            {
                var lblValue = new Label
                {
                    Text = $"Deal Value: {deal.Value.Value.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-PH"))}",
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(40, 167, 69),
                    Location = new Point(15, yPos),
                    AutoSize = true
                };
                card.Controls.Add(lblValue);

                yPos += lblValue.Height + 12;
            }

            // Status info
            var lblStatusInfo = new Label
            {
                Text = $"✓ Active deal in progress",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(25, 135, 84),
                Location = new Point(15, yPos),
                AutoSize = true
            };
            card.Controls.Add(lblStatusInfo);

            yPos += lblStatusInfo.Height + 8;

            // Last updated info
            var lastUpdated = deal.UpdatedAt ?? deal.CreatedAt;
            var lblLastUpdated = new Label
            {
                Text = $"Last updated: {lastUpdated.ToString("MMM dd, yyyy hh:mm tt")}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Location = new Point(15, yPos),
                AutoSize = true
            };
            card.Controls.Add(lblLastUpdated);

            // Set final card height based on content
            yPos += lblLastUpdated.Height + 15; // label height + bottom padding
            card.Height = yPos;

            // Fix status badge position after card is fully constructed
            card.HandleCreated += (s, e) =>
            {
                UpdateStatusBadgePosition(card, card.Width);
            };

            return card;
        }

        private void BtnApprove_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not DealStatusChangeRequest request)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to approve moving this deal to '{request.RequestedStatus}'?",
                "Approve Status Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                using var db = Data.DbContextHelper.CreateDbContext();

                // Find the request
                var dbRequest = db.DealStatusChangeRequests.Find(request.Id);
                if (dbRequest == null)
                {
                    MessageBox.Show("Status change request not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Find the deal with property included
                var deal = db.Deals
                    .Include(d => d.Property)
                    .FirstOrDefault(d => d.Id == dbRequest.DealId);

                if (deal == null)
                {
                    MessageBox.Show("Deal not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update the deal status
                deal.Status = dbRequest.RequestedStatus;
                deal.UpdatedAt = DateTime.UtcNow;

                // When a client approves a Closed/Done move, keep the deal/property active so brokers can clear them manually
                if (dbRequest.RequestedStatus.Contains("Closed", StringComparison.OrdinalIgnoreCase))
                {
                    deal.IsActive = true; // keep visible in pipeline
                    deal.ClosedAt = DateTime.UtcNow;

                    if (deal.Property != null)
                    {
                        deal.Property.IsActive = true;
                    }
                }
                // Mark the request as approved
                dbRequest.IsApproved = true;
                dbRequest.RespondedAt = DateTime.UtcNow;
                dbRequest.RespondedByUserId = UserSession.Instance.CurrentUser?.Id;

                db.SaveChanges();

                // Log the approval
                var currentUser = UserSession.Instance.CurrentUser;
                if (currentUser != null)
                {
                    LoggingService.LogAction(
                        "Deal Status Change Approved",
                        $"Client approved moving deal '{deal.Title}' from '{dbRequest.PreviousStatus}' to '{dbRequest.RequestedStatus}'",
                        currentUser.Id);
                }

                MessageBox.Show("Status change approved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload the deals
                LoadDealsWithPendingRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error approving status change: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReject_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not DealStatusChangeRequest request)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to reject the request to move this deal to '{request.RequestedStatus}'?",
                "Reject Status Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                using var db = Data.DbContextHelper.CreateDbContext();

                // Find the request
                var dbRequest = db.DealStatusChangeRequests.Find(request.Id);
                if (dbRequest == null)
                {
                    MessageBox.Show("Status change request not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Mark the request as rejected
                dbRequest.IsApproved = false;
                dbRequest.RespondedAt = DateTime.UtcNow;
                dbRequest.RespondedByUserId = UserSession.Instance.CurrentUser?.Id;

                db.SaveChanges();

                // Log the rejection
                var currentUser = UserSession.Instance.CurrentUser;
                if (currentUser != null)
                {
                    LoggingService.LogAction(
                        "Deal Status Change Rejected",
                        $"Client rejected moving deal from '{dbRequest.PreviousStatus}' to '{dbRequest.RequestedStatus}'",
                        currentUser.Id);
                }

                MessageBox.Show("Status change rejected.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload the deals
                LoadDealsWithPendingRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error rejecting status change: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Color GetStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "qualified" => Color.FromArgb(23, 162, 184),      // Teal
                "proposal" => Color.FromArgb(255, 193, 7),        // Amber
                "negotiation" => Color.FromArgb(255, 152, 0),     // Orange
                "closed" => Color.FromArgb(40, 167, 69),          // Green
                "lost" => Color.FromArgb(220, 53, 69),            // Red
                _ => Color.FromArgb(108, 117, 125)                // Gray (default)
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer?.Stop();
                refreshTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

