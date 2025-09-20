using RealEstateCRMWinForms.ViewModels;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace RealEstateCRMWinForms.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly PropertyViewModel _propertyViewModel;
        private readonly LeadViewModel _leadViewModel;
        private readonly ContactViewModel _contactViewModel;
        private readonly DealViewModel _dealViewModel;

        // Backing fields for statistics card labels that are created at runtime
        private Label? lblActiveListingsIcon;
        private Label? lblActiveListingsTitle;
        private Label? lblActiveListingsValue;
        private Label? lblActiveListingsDesc;
        private Label? lblAvgDaysIcon;
        private Label? lblAvgDaysTitle;
        private Label? lblAvgDaysValue;
        private Label? lblAvgDaysDesc;
        private Label? lblAvgPriceIcon;
        private Label? lblAvgPriceTitle;
        private Label? lblAvgPricePerSqftValue;
        private Label? lblAvgPricePerSqftDesc;

        public DashboardView()
        {
            InitializeComponent();
            
            // Initialize ViewModels
            _propertyViewModel = new PropertyViewModel();
            _leadViewModel = new LeadViewModel();
            _contactViewModel = new ContactViewModel();
            _dealViewModel = new DealViewModel();

            // Subscribe to property updates so dashboard refreshes in real-time
            PropertyViewModel.PropertiesUpdated += PropertyViewModel_PropertiesUpdated;

            // Load data after all controls are initialized
            this.Load += DashboardView_Load;
        }

        private void DashboardView_Load(object? sender, EventArgs e)
        {
            try
            {
                // Initialize the statistics cards first
                InitializeStatisticsCards();
                
                // Ensure all controls are properly initialized before loading data
                LoadDashboardData();
                LoadRecentSections();

                // Hide the large title label from the header
                if (lblTitle != null)
                    lblTitle.Visible = false;

                // Position the subtitle vertically centered and left-aligned
                PositionSubtitleLeftMiddle();
                if (headerPanel != null)
                {
                    headerPanel.SizeChanged -= HeaderPanel_SizeChanged;
                    headerPanel.SizeChanged += HeaderPanel_SizeChanged;
                }
                
                // Add resize handlers for recent sections to ensure proper layout
                AddResizeHandlers();
                
                // Position the "View All" links properly
                PositionViewAllLinks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeStatisticsCards()
        {
            try
            {
                // Initialize Active Listings Card - check if labels already exist
                if (activeListingsCard != null)
                {
                    lblActiveListingsIcon = new Label
                    {
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        AutoSize = true,
                        Font = new Font("Segoe UI", 14F),
                        ForeColor = Color.FromArgb(59, 130, 246),
                        Text = "$",
                        Location = new Point(Math.Max(0, activeListingsCard.ClientSize.Width - 36), 12)
                    };

                    lblActiveListingsTitle = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 11.25F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(29, 78, 216),
                        Text = "Active Listings Value",
                        Location = new Point(18, 12)
                    };

                    lblActiveListingsValue = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(17, 24, 39),
                        Text = "₱ 0",
                        Location = new Point(18, 42)
                    };

                    lblActiveListingsDesc = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(120, 130, 140),
                        Text = "Total value of active properties",
                        Location = new Point(18, 78)
                    };

                    activeListingsCard.Controls.AddRange(new Control[] { 
                        lblActiveListingsIcon, lblActiveListingsTitle, lblActiveListingsValue, lblActiveListingsDesc 
                    });
                }

                // Initialize Average Days Card - check if labels already exist
                if (avgDaysCard != null)
                {
                    lblAvgDaysIcon = new Label
                    {
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        AutoSize = true,
                        Font = new Font("Segoe UI", 14F),
                        ForeColor = Color.FromArgb(249, 115, 22),
                        Text = "⏱",
                        Location = new Point(Math.Max(0, avgDaysCard.ClientSize.Width - 36), 12)
                    };

                    lblAvgDaysTitle = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 11.25F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(220, 110, 40),
                        Text = "Avg Days on Market",
                        Location = new Point(18, 12)
                    };

                    lblAvgDaysValue = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(17, 24, 39),
                        Text = "0 Days",
                        Location = new Point(18, 42)
                    };

                    lblAvgDaysDesc = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(120, 130, 140),
                        Text = "Average time on market",
                        Location = new Point(18, 78)
                    };

                    avgDaysCard.Controls.AddRange(new Control[] { 
                        lblAvgDaysIcon, lblAvgDaysTitle, lblAvgDaysValue, lblAvgDaysDesc 
                    });
                }

                // Initialize Average Price Card - check if labels already exist
                if (avgPriceCard != null)
                {
                    lblAvgPriceIcon = new Label
                    {
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        AutoSize = true,
                        Font = new Font("Segoe UI", 14F),
                        ForeColor = Color.FromArgb(34, 197, 94),
                        Text = "📈",
                        Location = new Point(Math.Max(0, avgPriceCard.ClientSize.Width - 36), 12)
                    };

                    lblAvgPriceTitle = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 11.25F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(34, 197, 94),
                        Text = "Avg Price per SQFT",
                        Location = new Point(18, 12)
                    };

                    lblAvgPricePerSqftValue = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(17, 24, 39),
                        Text = "₱ 0",
                        Location = new Point(18, 42)
                    };

                    lblAvgPricePerSqftDesc = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(120, 130, 140),
                        Text = "Per square foot pricing",
                        Location = new Point(18, 78)
                    };

                    avgPriceCard.Controls.AddRange(new Control[] { 
                        lblAvgPriceIcon, lblAvgPriceTitle, lblAvgPricePerSqftValue, lblAvgPricePerSqftDesc 
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing statistics cards: {ex.Message}");
            }
        }

        private void PositionViewAllLinks()
        {
            try
            {
                // Position the View All links on the right side of each card header
                if (lblViewAllRecentProperties != null && recentPropertiesCard != null)
                {
                    int cardWidth = recentPropertiesCard.ClientSize.Width;
                    lblViewAllRecentProperties.Location = new Point(cardWidth - lblViewAllRecentProperties.Width - 20, 2);
                }
                if (lblViewAllRecentDeals != null && recentDealsCard != null)
                {
                    int cardWidth = recentDealsCard.ClientSize.Width;
                    lblViewAllRecentDeals.Location = new Point(cardWidth - lblViewAllRecentDeals.Width - 20, 2);
                }
                if (lblViewAllRecentContacts != null && recentContactsCard != null)
                {
                    int cardWidth = recentContactsCard.ClientSize.Width;
                    lblViewAllRecentContacts.Location = new Point(cardWidth - lblViewAllRecentContacts.Width - 20, 2);
                }
                if (lblViewAllRecentLeads != null && recentLeadsCard != null)
                {
                    int cardWidth = recentLeadsCard.ClientSize.Width;
                    lblViewAllRecentLeads.Location = new Point(cardWidth - lblViewAllRecentLeads.Width - 20, 2);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error positioning view all links: {ex.Message}");
            }
        }

        private void AddResizeHandlers()
        {
            try
            {
                if (recentPropertiesList != null)
                {
                    recentPropertiesList.Resize += (s, e) => RefreshRecentItemsLayout();
                }
                if (recentDealsList != null)
                {
                    recentDealsList.Resize += (s, e) => RefreshRecentItemsLayout();
                }
                if (recentContactsList != null)
                {
                    recentContactsList.Resize += (s, e) => RefreshRecentItemsLayout();
                }
                if (recentLeadsList != null)
                {
                    recentLeadsList.Resize += (s, e) => RefreshRecentItemsLayout();
                }
                
                // Add resize handlers for cards to reposition View All links
                if (recentPropertiesCard != null)
                {
                    recentPropertiesCard.Resize += (s, e) => PositionViewAllLinks();
                }
                if (recentDealsCard != null)
                {
                    recentDealsCard.Resize += (s, e) => PositionViewAllLinks();
                }
                if (recentContactsCard != null)
                {
                    recentContactsCard.Resize += (s, e) => PositionViewAllLinks();
                }
                if (recentLeadsCard != null)
                {
                    recentLeadsCard.Resize += (s, e) => PositionViewAllLinks();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding resize handlers: {ex.Message}");
            }
        }

        private void RefreshRecentItemsLayout()
        {
            // Refresh the layout of all recent items when the container is resized
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                BeginInvoke(new Action(() =>
                {
                    try
                    {
                        LoadRecentSections();
                        PositionViewAllLinks();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error refreshing recent items layout: {ex.Message}");
                    }
                }));
            }
        }

        private void LoadRecentSections()
        {
            try
            {
                LoadRecentProperties();
                LoadRecentDeals(); 
                LoadRecentContacts();
                LoadRecentLeads();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recent sections: {ex.Message}");
            }
        }

        private void LoadRecentProperties()
        {
            try
            {
                if (recentPropertiesList == null) return;

                recentPropertiesList.Controls.Clear();

                var recentProperties = _propertyViewModel.Properties
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .ToList();

                if (recentProperties.Any())
                {
                    foreach (var property in recentProperties)
                    {
                        var itemPanel = CreateRecentPropertyItem(property);
                        // Dock=Top stacking; latest should appear first — add in reverse to keep order top->bottom
                        recentPropertiesList.Controls.Add(itemPanel);
                        recentPropertiesList.Controls.SetChildIndex(itemPanel, 0);
                    }

                    if (recentPropertiesList.Controls.Count > 0)
                    {
                        var last = recentPropertiesList.Controls[recentPropertiesList.Controls.Count - 1];
                        recentPropertiesList.AutoScrollMinSize = new Size(0, last.Bottom + 10);
                    }
                }
                else
                {
                    var emptyLabel = new Label
                    {
                        Text = "No recent properties available",
                        Font = new Font("Segoe UI", 10F),
                        ForeColor = Color.FromArgb(107, 114, 128),
                        Location = new Point(15, 20),
                        AutoSize = true
                    };
                    recentPropertiesList.Controls.Add(emptyLabel);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recent properties: {ex.Message}");
            }
        }

        private Panel CreateRecentPropertyItem(RealEstateCRMWinForms.Models.Property property)
        {
            var panel = new Panel
            {
                Height = 95,  // Increased height for more spacing
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Top,
                Padding = new Padding(30, 20, 30, 20),  // Increased padding: 30px horizontal, 20px vertical
                Margin = new Padding(0, 0, 0, 4)  // Increased gap between items
            };

            // Clean bottom border only
            panel.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235), 1);
                e.Graphics.DrawLine(pen, 30, panel.Height - 1, panel.Width - 30, panel.Height - 1);
            };

            // Subtle hover effects
            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(248, 250, 252);
            panel.MouseLeave += (s, e) => panel.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = property.Title ?? "Untitled Property",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(30, 16),  // Adjusted for new padding
                AutoSize = false,
                Size = new Size(400, 26),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            var addressLabel = new Label
            {
                Text = property.Address ?? "No address",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(30, 44),  // Adjusted for new padding
                AutoSize = false,
                Size = new Size(400, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            var priceLabel = new Label
            {
                Text = $"₱{property.Price:N0}",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize = false,
                Size = new Size(180, 26),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var dateLabel = new Label
            {
                Text = property.CreatedAt.ToString("MMM dd"),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                Size = new Size(100, 22),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Resize handler for responsive layout
            panel.SizeChanged += (s, e) =>
            {
                int panelWidth = panel.ClientSize.Width;
                int rightMargin = 30;  // Increased margin
                
                // Position right-aligned elements
                priceLabel.Location = new Point(panelWidth - priceLabel.Width - rightMargin, 16);
                dateLabel.Location = new Point(panelWidth - dateLabel.Width - rightMargin, 44);
                
                // Adjust left elements to use available space
                int availableWidth = panelWidth - 60 - priceLabel.Width - 30; // Adjusted for new padding
                titleLabel.Size = new Size(Math.Max(300, availableWidth), 26);
                addressLabel.Size = new Size(Math.Max(300, availableWidth), 22);
            };

            panel.Controls.AddRange(new Control[] { titleLabel, addressLabel, priceLabel, dateLabel });
            panel.Click += (s, e) => LblViewAllProperties_Click(s, e);
            
            return panel;
        }

        private void LoadRecentDeals()
        {
            try
            {
                if (recentDealsList == null) return;
                recentDealsList.Controls.Clear();

                var recentDeals = _dealViewModel.Deals
                    .Where(d => d.IsActive)
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(5)
                    .ToList();

                if (recentDeals.Any())
                {
                    foreach (var deal in recentDeals)
                    {
                        var itemPanel = CreateRecentDealItem(deal);
                        recentDealsList.Controls.Add(itemPanel);
                        recentDealsList.Controls.SetChildIndex(itemPanel, 0);
                    }
                    if (recentDealsList.Controls.Count > 0)
                    {
                        var last = recentDealsList.Controls[recentDealsList.Controls.Count - 1];
                        recentDealsList.AutoScrollMinSize = new Size(0, last.Bottom + 10);
                    }
                }
                else
                {
                    recentDealsList.Controls.Add(new Label
                    {
                        Text = "No recent deals available",
                        Font = new Font("Segoe UI", 10F),
                        ForeColor = Color.FromArgb(107, 114, 128),
                        Location = new Point(15, 20),
                        AutoSize = true
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recent deals: {ex.Message}");
            }
        }

        private Panel CreateRecentDealItem(RealEstateCRMWinForms.Models.Deal deal)
        {
            var panel = new Panel
            {
                Height = 95,  // Match Properties height
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Top,
                Padding = new Padding(30, 20, 30, 20),  // Match Properties padding
                Margin = new Padding(0, 0, 0, 4)  // Match Properties margin
            };

            panel.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235), 1);
                e.Graphics.DrawLine(pen, 30, panel.Height - 1, panel.Width - 30, panel.Height - 1);
            };
            
            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(248, 250, 252);
            panel.MouseLeave += (s, e) => panel.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = deal.Title,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(30, 16),  // Adjusted for new padding
                AutoSize = false,
                Size = new Size(400, 26),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            var contactLabel = new Label
            {
                Text = deal.Contact?.FullName ?? "No contact assigned",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(30, 44),  // Adjusted for new padding
                AutoSize = false,
                Size = new Size(400, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            var statusLabel = new Label
            {
                Text = deal.Status,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = GetStatusColor(deal.Status),
                AutoSize = false,
                Size = new Size(140, 26),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var dateLabel = new Label
            {
                Text = deal.CreatedAt.ToString("MMM dd"),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                Size = new Size(100, 22),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            panel.SizeChanged += (s, e) =>
            {
                int panelWidth = panel.ClientSize.Width;
                int rightMargin = 30;  // Increased margin
                
                statusLabel.Location = new Point(panelWidth - statusLabel.Width - rightMargin, 16);
                dateLabel.Location = new Point(panelWidth - dateLabel.Width - rightMargin, 44);
                
                int availableWidth = panelWidth - 60 - statusLabel.Width - 30; // Adjusted for new padding
                titleLabel.Size = new Size(Math.Max(300, availableWidth), 26);
                contactLabel.Size = new Size(Math.Max(300, availableWidth), 22);
            };

            panel.Controls.AddRange(new Control[] { titleLabel, contactLabel, statusLabel, dateLabel });
            panel.Click += (s, e) => LblViewAllDeals_Click(s, e);
            return panel;
        }

        private void LoadRecentContacts()
        {
            try
            {
                if (recentContactsList == null) return;
                recentContactsList.Controls.Clear();

                var recentContacts = _contactViewModel.Contacts
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .ToList();

                if (recentContacts.Any())
                {
                    foreach (var contact in recentContacts)
                    {
                        var itemPanel = CreateRecentContactItem(contact);
                        recentContactsList.Controls.Add(itemPanel);
                        recentContactsList.Controls.SetChildIndex(itemPanel, 0);
                    }
                    if (recentContactsList.Controls.Count > 0)
                    {
                        var last = recentContactsList.Controls[recentContactsList.Controls.Count - 1];
                        recentContactsList.AutoScrollMinSize = new Size(0, last.Bottom + 10);
                    }
                }
                else
                {
                    recentContactsList.Controls.Add(new Label
                    {
                        Text = "No recent contacts available",
                        Font = new Font("Segoe UI", 10F),
                        ForeColor = Color.FromArgb(107, 114, 128),
                        Location = new Point(15, 20),
                        AutoSize = true
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recent contacts: {ex.Message}");
            }
        }

        private Panel CreateRecentContactItem(RealEstateCRMWinForms.Models.Contact contact)
        {
            var panel = new Panel
            {
                Height = 95,  // Match Properties height
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Top,
                Padding = new Padding(30, 20, 30, 20),  // Match Properties padding
                Margin = new Padding(0, 0, 0, 4)  // Match Properties margin
            };

            // Clean bottom border only
            panel.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235), 1);
                e.Graphics.DrawLine(pen, 30, panel.Height - 1, panel.Width - 30, panel.Height - 1);
            };
            
            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(248, 250, 252);
            panel.MouseLeave += (s, e) => panel.BackColor = Color.White;

            // Create avatar circle
            var avatar = new Label
            {
                Text = GetInitials(contact.FullName),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = GetAvatarColor(contact.FullName),
                Location = new Point(30, 20),  // Aligned with new padding
                Size = new Size(32, 32),
                TextAlign = ContentAlignment.MiddleCenter
            };
            avatar.Paint += (s, e) =>
            {
                using var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddEllipse(0, 0, avatar.Width - 1, avatar.Height - 1);
                avatar.Region = new Region(path);
            };

            var nameLabel = new Label
            {
                Text = contact.FullName,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),  // Match Properties
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(70, 16),  // After avatar with new padding
                AutoSize = false,
                Size = new Size(400, 26),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            var typeLabel = new Label
            {
                Text = contact.Type,
                Font = new Font("Segoe UI", 11F),  // Match Properties
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(70, 44),  // Adjusted for new padding
                AutoSize = false,
                Size = new Size(400, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            var dateLabel = new Label
            {
                Text = contact.CreatedAt.ToString("MMM dd"),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                Size = new Size(100, 22),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            panel.SizeChanged += (s, e) =>
            {
                int panelWidth = panel.ClientSize.Width;
                int rightMargin = 30;  // Increased margin
                
                // Position right-aligned date
                dateLabel.Location = new Point(panelWidth - dateLabel.Width - rightMargin, 32);
                
                // Adjust name and type labels to use available space
                int availableWidth = panelWidth - 100 - dateLabel.Width - 30; // Account for avatar + margins
                nameLabel.Size = new Size(Math.Max(200, availableWidth), 26);
                typeLabel.Size = new Size(Math.Max(200, availableWidth), 22);
            };

            panel.Controls.AddRange(new Control[] { avatar, nameLabel, typeLabel, dateLabel });
            panel.Click += (s, e) => LblViewAllContacts_Click(s, e);
            return panel;
        }

        private void LoadRecentLeads()
        {
            try
            {
                if (recentLeadsList == null) return;
                recentLeadsList.Controls.Clear();

                var recentLeads = _leadViewModel.Leads
                    .Where(l => l.IsActive)
                    .OrderByDescending(l => l.CreatedAt)
                    .Take(5)
                    .ToList();

                if (recentLeads.Any())
                {
                    foreach (var lead in recentLeads)
                    {
                        var itemPanel = CreateRecentLeadItem(lead);
                        recentLeadsList.Controls.Add(itemPanel);
                        recentLeadsList.Controls.SetChildIndex(itemPanel, 0);
                    }
                    if (recentLeadsList.Controls.Count > 0)
                    {
                        var last = recentLeadsList.Controls[recentLeadsList.Controls.Count - 1];
                        recentLeadsList.AutoScrollMinSize = new Size(0, last.Bottom + 10);
                    }
                }
                else
                {
                    recentLeadsList.Controls.Add(new Label
                    {
                        Text = "No recent leads available",
                        Font = new Font("Segoe UI", 10F),
                        ForeColor = Color.FromArgb(107, 114, 128),
                        Location = new Point(15, 20),
                        AutoSize = true
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recent leads: {ex.Message}");
            }
        }

        private Panel CreateRecentLeadItem(RealEstateCRMWinForms.Models.Lead lead)
        {
            var panel = new Panel
            {
                Height = 85,  // Match Properties/Deals height
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Top,
                Padding = new Padding(20, 16, 20, 16),  // Match Properties/Deals padding
                Margin = new Padding(0, 0, 0, 2)
            };

            // Clean bottom border only
            panel.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235), 1);
                e.Graphics.DrawLine(pen, 20, panel.Height - 1, panel.Width - 20, panel.Height - 1);
            };
            
            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(248, 250, 252);
            panel.MouseLeave += (s, e) => panel.BackColor = Color.White;

            // Create avatar circle
            var avatar = new Label
            {
                Text = GetInitials(lead.FullName),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = GetAvatarColor(lead.FullName),
                Location = new Point(20, 16),  // Aligned with padding
                Size = new Size(32, 32),
                TextAlign = ContentAlignment.MiddleCenter
            };
            avatar.Paint += (s, e) =>
            {
                using var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddEllipse(0, 0, avatar.Width - 1, avatar.Height - 1);
                avatar.Region = new Region(path);
            };

            var nameLabel = new Label
            {
                Text = lead.FullName,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),  // Match Properties/Deals
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(60, 12),  // After avatar
                AutoSize = false,
                Size = new Size(400, 26),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            var statusLabel = new Label
            {
                Text = lead.Status,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = GetLeadStatusColor(lead.Status),
                Location = new Point(60, 38),
                AutoSize = false,
                Size = new Size(120, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };

            var dateLabel = new Label
            {
                Text = lead.CreatedAt.ToString("MMM dd"),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                Size = new Size(100, 22),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            panel.SizeChanged += (s, e) =>
            {
                int panelWidth = panel.ClientSize.Width;
                int rightMargin = 20;
                
                // Position right-aligned date
                dateLabel.Location = new Point(panelWidth - dateLabel.Width - rightMargin, 26);
                
                // Adjust name and status labels to use available space
                int availableWidth = panelWidth - 80 - dateLabel.Width - 30; // Adjusted for new padding
                nameLabel.Size = new Size(Math.Max(200, availableWidth), 26);
                statusLabel.Size = new Size(Math.Max(150, availableWidth), 22);
            };

            panel.Controls.AddRange(new Control[] { avatar, nameLabel, statusLabel, dateLabel });
            panel.Click += (s, e) => LblViewAllLeads_Click(s, e);
            return panel;
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "?";
            
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
        }

        private Color GetAvatarColor(string name)
        {
            var colors = new[]
            {
                Color.FromArgb(239, 68, 68),   // Red
                Color.FromArgb(245, 158, 11),  // Orange  
                Color.FromArgb(34, 197, 94),   // Green
                Color.FromArgb(59, 130, 246),  // Blue
                Color.FromArgb(139, 92, 246),  // Purple
                Color.FromArgb(236, 72, 153),  // Pink
                Color.FromArgb(16, 185, 129),  // Emerald
                Color.FromArgb(168, 85, 247)   // Violet
            };
            
            int hash = Math.Abs(name.GetHashCode());
            return colors[hash % colors.Length];
        }

        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "New" => Color.FromArgb(59, 130, 246),      // Blue
                "Offer Made" => Color.FromArgb(245, 158, 11), // Orange
                "Negotiation" => Color.FromArgb(245, 158, 11), // Orange
                "Contract Draft" => Color.FromArgb(34, 197, 94), // Green
                "Closed" => Color.FromArgb(34, 197, 94),    // Green
                "Contract Signed" => Color.FromArgb(34, 197, 94), // Green
                _ => Color.FromArgb(107, 114, 128)          // Gray
            };
        }

        private Color GetLeadStatusColor(string status)
        {
            return status switch
            {
                "New Lead" => Color.FromArgb(59, 130, 246),    // Blue
                "Contacted" => Color.FromArgb(245, 158, 11),   // Orange
                "Qualified" => Color.FromArgb(34, 197, 94),    // Green
                "Unqualified" => Color.FromArgb(239, 68, 68),  // Red
                _ => Color.FromArgb(107, 114, 128)             // Gray
            };
        }

        // Handle cleanup when the control is disposed
        private void OnHandleDestroyed()
        {
            // Unsubscribe from static event to prevent memory leaks
            PropertyViewModel.PropertiesUpdated -= PropertyViewModel_PropertiesUpdated;
        }

        // Override HandleDestroyed instead of Dispose
        protected override void OnHandleDestroyed(EventArgs e)
        {
            OnHandleDestroyed();
            base.OnHandleDestroyed(e);
        }

        private void PropertyViewModel_PropertiesUpdated(object? sender, EventArgs e)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                if (this.InvokeRequired)
                {
                    BeginInvoke(new Action(() => { LoadDashboardData(); LoadRecentSections(); }));
                    return;
                }
                LoadDashboardData();
                LoadRecentSections();
            }
        }

        private void HeaderPanel_SizeChanged(object? sender, EventArgs e)
        {
            PositionSubtitleLeftMiddle();
        }

        private void PositionSubtitleLeftMiddle()
        {
            if (lblSubtitle == null || headerPanel == null) return;

            try
            {
                // Measure to get current rendered size
                lblSubtitle.AutoSize = true;

                // Compute vertical center within headerPanel content area (respect padding)
                int availableHeight = Math.Max(0, headerPanel.ClientSize.Height - headerPanel.Padding.Top - headerPanel.Padding.Bottom);
                int top = headerPanel.Padding.Top + Math.Max(0, (availableHeight - lblSubtitle.Height) / 2);

                // Place subtitle at left padding, vertically centered
                int left = headerPanel.Padding.Left;
                lblSubtitle.Location = new System.Drawing.Point(left, top);

                // Ensure left alignment of the text
                lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error positioning subtitle: {ex.Message}");
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                // Check if controls are initialized
                if (lblPropertiesValue == null) 
                    return;

                // Load data from ViewModels
                var properties = _propertyViewModel.Properties ?? new System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Property>();
                var leads = _leadViewModel.Leads ?? new System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Lead>();
                var contacts = _contactViewModel.Contacts ?? new System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Contact>();
                var deals = _dealViewModel.Deals ?? new System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Deal>();

                // Update all dashboard cards with proper formatting
                UpdatePropertiesCard(properties);
                UpdateActiveListingsCard(properties);
                UpdateAvgDaysCard(properties);
                UpdatePriceSqftCard(properties);
                UpdateDealsCard(deals);
                UpdateContactsCard(contacts);
                UpdateLeadsCard(leads);

                // Update last updated timestamp
                if (lblLastUpdated != null)
                    lblLastUpdated.Text = $"Last updated: {DateTime.Now:MMM dd, yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
            }
        }

        private void UpdatePropertiesCard(System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Property> properties)
        {
            if (lblPropertiesValue == null) return;
            
            lblPropertiesValue.Text = properties.Count.ToString();
            var activeProperties = properties.Count(p => p.IsActive);
            
            if (lblPropertiesDesc != null)
                lblPropertiesDesc.Text = $"{activeProperties} Active Listings";
        }

        private void UpdateActiveListingsCard(System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Property> properties)
        {
            if (lblActiveListingsValue == null) return;
            
            decimal activeValue = properties.Where(p => p.IsActive).Sum(p => p.Price);
            lblActiveListingsValue.Text = $"₱{activeValue:N0}";
        }

        private void UpdateAvgDaysCard(System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Property> properties)
        {
            if (lblAvgDaysValue == null) return;
            
            var activeWithDates = properties.Where(p => p.IsActive && p.CreatedAt != default).ToList();
            if (activeWithDates.Any())
            {
                var avgDays = (int)Math.Round(activeWithDates.Average(p => (DateTime.Now - p.CreatedAt).TotalDays));
                lblAvgDaysValue.Text = $"{avgDays} Days";
            }
            else
            {
                lblAvgDaysValue.Text = "0 Days";
            }
        }

        private void UpdatePriceSqftCard(System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Property> properties)
        {
            if (lblAvgPricePerSqftValue == null) return;
            
            var activeWithArea = properties.Where(p => p.IsActive && p.SquareMeters > 0).ToList();
            if (activeWithArea.Any())
            {
                decimal avgPerSqft = activeWithArea
                    .Select(p => p.Price / (p.SquareMeters * 10.7639m))
                    .Average();
                lblAvgPricePerSqftValue.Text = $"₱{Math.Round(avgPerSqft, 0):N0}";
            }
            else
            {
                lblAvgPricePerSqftValue.Text = "₱0";
            }
        }

        private void UpdateDealsCard(System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Deal> deals)
        {
            if (lblDealsValue == null) return;
            
            lblDealsValue.Text = deals.Count.ToString();
            
            if (lblDealsDesc != null)
            {
                var closedDeals = deals.Count(d => d.Status == "Closed" || d.Status == "Contract Signed");
                var inProgressDeals = deals.Count(d => d.Status != "Closed" && d.Status != "Contract Signed" && d.IsActive);
                lblDealsDesc.Text = $"{closedDeals} Closed | {inProgressDeals} In Progress";
            }
        }

        private void UpdateContactsCard(System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Contact> contacts)
        {
            if (lblContactsValue == null) return;
            
            lblContactsValue.Text = contacts.Count.ToString();
            
            if (lblContactsDesc != null)
            {
                var agents = contacts.Count(c => c.Type == "Agent");
                var clients = contacts.Count(c => c.Type != "Agent");
                lblContactsDesc.Text = $"{agents} Agents | {clients} Clients";
            }
        }

        private void UpdateLeadsCard(System.ComponentModel.BindingList<RealEstateCRMWinForms.Models.Lead> leads)
        {
            if (lblLeadsValue == null) return;
            
            lblLeadsValue.Text = leads.Count.ToString();
            
            if (lblLeadsDesc != null)
            {
                var qualifiedLeads = leads.Count(l => l.Status == "Qualified");
                lblLeadsDesc.Text = $"{qualifiedLeads} Qualified | {leads.Count - qualifiedLeads} Prospects";
            }
        }

        // Navigation event handlers for the "View All" links
        private void LblViewAllProperties_Click(object sender, EventArgs e)
        {
            NavigateToSection("Properties");
        }

        private void LblViewAllDeals_Click(object sender, EventArgs e)
        {
            NavigateToSection("Deals");
        }

        private void LblViewAllContacts_Click(object sender, EventArgs e)
        {
            NavigateToSection("Contacts");
        }

        private void LblViewAllLeads_Click(object sender, EventArgs e)
        {
            NavigateToSection("Leads");
        }

        private void NavigateToSection(string sectionName)
        {
            try
            {
                // Find the MainView form that contains this UserControl
                var mainForm = this.FindForm();

                // Check if the form is MainView by checking its type name
                if (mainForm != null && mainForm.GetType().Name == "MainView")
                {
                    // Use reflection to call SwitchSection method
                    var switchSectionMethod = mainForm.GetType().GetMethod("SwitchSection");
                    if (switchSectionMethod != null)
                    {
                        switchSectionMethod.Invoke(mainForm, new object[] { sectionName });
                        return;
                    }
                }

                // Alternative approach: try to find MainView through parent controls
                Control parent = this.Parent;
                while (parent != null)
                {
                    if (parent.GetType().Name == "MainView")
                    {
                        var switchSectionMethod = parent.GetType().GetMethod("SwitchSection");
                        if (switchSectionMethod != null)
                        {
                            switchSectionMethod.Invoke(parent, new object[] { sectionName });
                            return;
                        }
                    }
                    parent = parent.Parent;
                }

                // If we can't find MainView, just show a message
                MessageBox.Show($"Navigating to {sectionName} section...", "Navigation",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Public method to refresh the dashboard data
        public void RefreshDashboard()
        {
            LoadDashboardData();
            LoadRecentSections();
            PositionSubtitleLeftMiddle();
            PositionViewAllLinks();
        }

        private void lblContactsIcon_Click(object sender, EventArgs e)
        {

        }

        private void lblPropertiesValue_Click(object sender, EventArgs e)
        {

        }

        private void dealsCard_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}