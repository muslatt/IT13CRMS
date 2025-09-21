using RealEstateCRMWinForms.ViewModels;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace RealEstateCRMWinForms.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly PropertyViewModel _propertyViewModel;
        private readonly LeadViewModel _leadViewModel;
        private readonly ContactViewModel _contactViewModel;
        private readonly DealViewModel _dealViewModel;

        public DashboardView()
        {
            InitializeComponent();

            // Initialize ViewModels
            _propertyViewModel = new PropertyViewModel();
            _leadViewModel = new LeadViewModel();
            _contactViewModel = new ContactViewModel();
            _dealViewModel = new DealViewModel();

            // Load data after all controls are initialized
            this.Load += DashboardView_Load;
        }

        private void DashboardView_Load(object? sender, EventArgs e)
        {
            try
            {
                // Initialize all analytics components
                InitializeKPICards();
                InitializeCharts();

                InitializeTotalCounts();
                InitializeAnalyticsCards();

                // Load analytics data
                LoadAnalyticsData();

                // Update last updated timestamp
                UpdateLastUpdatedTime();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading analytics dashboard: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeKPICards()
        {
            try
            {
                // Total Revenue Card
                CreateKPICard(totalRevenueCard, "💰", "Total Revenue", "₱0", "From closed deals", Color.FromArgb(16, 185, 129));

                // Average Deal Value Card
                CreateKPICard(avgDealValueCard, "📊", "Avg Deal Value", "₱0", "Per closed deal", Color.FromArgb(59, 130, 246));

                // Conversion Rate Card
                CreateKPICard(conversionRateCard, "🎯", "Conversion Rate", "0%", "Leads to deals", Color.FromArgb(245, 158, 11));

                // Average Days to Close Card
                CreateKPICard(avgDaysToCloseCard, "⏱️", "Avg Days to Close", "0 days", "Deal lifecycle", Color.FromArgb(139, 92, 246));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing KPI cards: {ex.Message}");
            }
        }

        private void CreateKPICard(Panel card, string icon, string title, string value, string description, Color accentColor)
        {
            if (card == null) return;

            card.Controls.Clear();

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24F),
                ForeColor = accentColor,
                Location = new Point(20, 15),
                AutoSize = true
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(20, 55),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(20, 75),
                AutoSize = true,
                Name = $"value_{card.Name}"
            };

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(20, 105),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { iconLabel, titleLabel, valueLabel, descLabel });
        }

        private void InitializeCharts()
        {
            try
            {
                // Sales Chart Card
                CreateChartCard(salesChartCard, "Monthly Sales", "Track sales performance over time");

                // Property Status Chart Card  
                CreateChartCard(propertyStatusChartCard, "Property Status", "Distribution of property statuses");

                // Lead Conversion Chart Card
                CreateChartCard(leadConversionChartCard, "Lead Conversion", "Lead conversion funnel");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing charts: {ex.Message}");
            }
        }

        private void CreateChartCard(Panel card, string title, string description)
        {
            if (card == null) return;

            card.Controls.Clear();

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(20, 40),
                AutoSize = true
            };

            // Placeholder for chart content
            var chartArea = new Panel
            {
                BackColor = Color.FromArgb(249, 250, 251),
                Location = new Point(20, 70),
                Size = new Size(card.Width - 40, card.Height - 90),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Name = $"chartArea_{card.Name}"
            };

            card.Controls.AddRange(new Control[] { titleLabel, descLabel, chartArea });
        }



        private void InitializeTotalCounts()
        {
            try
            {
                // Total Counts Card
                CreateAnalyticsCard(totalCountsCard, "System Overview", "📊");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing total counts: {ex.Message}");
            }
        }

        private void InitializeAnalyticsCards()
        {
            try
            {
                // Revenue Analytics Card
                CreateAnalyticsCard(revenueAnalyticsCard, "Revenue Analytics", "💰");

                // Property Analytics Card
                CreateAnalyticsCard(propertyAnalyticsCard, "Property Analytics", "🏠");

                // Performance Analytics Card
                CreateAnalyticsCard(performanceAnalyticsCard, "Performance Metrics", "📈");

                // Trends Analytics Card
                CreateAnalyticsCard(trendsAnalyticsCard, "Market Trends", "📊");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing analytics cards: {ex.Message}");
            }
        }

        private void CreateAnalyticsCard(Panel card, string title, string icon)
        {
            if (card == null) return;

            card.Controls.Clear();

            var headerPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 16F),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(50, 15),
                AutoSize = true
            };

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20),
                Name = $"content_{card.Name}"
            };

            headerPanel.Controls.AddRange(new Control[] { iconLabel, titleLabel });
            card.Controls.AddRange(new Control[] { headerPanel, contentPanel });
        }

        private void LoadAnalyticsData()
        {
            try
            {
                LoadKPIData();
                LoadChartData();

                LoadTotalCounts();
                LoadAnalyticsContent();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading analytics data: {ex.Message}");
            }
        }

        private void LoadKPIData()
        {
            try
            {
                // Calculate total revenue from closed deals
                var closedDeals = _dealViewModel.Deals.Where(d => d.Status.ToLower() == "closed" && d.Value.HasValue).ToList();
                var totalRevenue = closedDeals.Sum(d => d.Value ?? 0);

                // Calculate average deal value
                var avgDealValue = closedDeals.Any() ? closedDeals.Average(d => d.Value ?? 0) : 0;

                // Calculate conversion rate (leads to deals)
                var totalLeads = _leadViewModel.Leads.Count(l => l.IsActive);
                var totalDeals = _dealViewModel.Deals.Count(d => d.IsActive);
                var conversionRate = totalLeads > 0 ? (double)totalDeals / totalLeads * 100 : 0;

                // Calculate average days to close
                var avgDaysToClose = closedDeals.Where(d => d.ClosedAt.HasValue)
                    .Select(d => (d.ClosedAt!.Value - d.CreatedAt).TotalDays)
                    .DefaultIfEmpty(0)
                    .Average();

                // Update KPI values
                UpdateKPIValue(totalRevenueCard, $"₱{totalRevenue:N0}");
                UpdateKPIValue(avgDealValueCard, $"₱{avgDealValue:N0}");
                UpdateKPIValue(conversionRateCard, $"{conversionRate:F1}%");
                UpdateKPIValue(avgDaysToCloseCard, $"{avgDaysToClose:F0} days");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading KPI data: {ex.Message}");
            }
        }

        private void UpdateKPIValue(Panel card, string value)
        {
            var valueLabel = card.Controls.OfType<Label>().FirstOrDefault(l => l.Name.StartsWith("value_"));
            if (valueLabel != null)
            {
                valueLabel.Text = value;
            }
        }

        private void LoadChartData()
        {
            try
            {
                LoadSalesChart();
                LoadPropertyStatusChart();
                LoadLeadConversionChart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading chart data: {ex.Message}");
            }
        }

        private void LoadSalesChart()
        {
            var chartArea = salesChartCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("chartArea_"));
            if (chartArea == null) return;

            chartArea.Controls.Clear();

            // Get monthly sales data
            var monthlySales = _dealViewModel.Deals
                .Where(d => d.Status.ToLower() == "closed" && d.Value.HasValue && d.ClosedAt.HasValue)
                .GroupBy(d => new { d.ClosedAt!.Value.Year, d.ClosedAt.Value.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Total = g.Sum(d => d.Value ?? 0),
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .Take(6)
                .ToList();

            // Create simple bar chart representation
            var maxValue = monthlySales.Any() ? monthlySales.Max(x => x.Total) : 1;
            var barWidth = chartArea.Width / Math.Max(monthlySales.Count, 1) - 10;
            var maxHeight = chartArea.Height - 40;

            for (int i = 0; i < monthlySales.Count; i++)
            {
                var sale = monthlySales[i];
                var barHeight = maxValue > 0 ? (int)(sale.Total / maxValue * maxHeight) : 0;

                var bar = new Panel
                {
                    BackColor = Color.FromArgb(59, 130, 246),
                    Size = new Size(barWidth, barHeight),
                    Location = new Point(i * (barWidth + 10) + 5, maxHeight - barHeight + 20)
                };

                var label = new Label
                {
                    Text = $"₱{sale.Total:N0}",
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    AutoSize = true,
                    Location = new Point(i * (barWidth + 10) + 5, maxHeight + 25)
                };

                chartArea.Controls.AddRange(new Control[] { bar, label });
            }
        }

        private void LoadPropertyStatusChart()
        {
            var chartArea = propertyStatusChartCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("chartArea_"));
            if (chartArea == null) return;

            chartArea.Controls.Clear();

            // Get property status distribution
            var statusCounts = _propertyViewModel.Properties
                .Where(p => p.IsActive)
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            var colors = new[] {
                Color.FromArgb(59, 130, 246),   // Blue
                Color.FromArgb(16, 185, 129),   // Green  
                Color.FromArgb(245, 158, 11),   // Yellow
                Color.FromArgb(239, 68, 68)     // Red
            };

            var total = statusCounts.Sum(x => x.Count);
            var startY = 20;

            for (int i = 0; i < statusCounts.Count; i++)
            {
                var status = statusCounts[i];
                var percentage = total > 0 ? (double)status.Count / total * 100 : 0;

                var colorBox = new Panel
                {
                    BackColor = colors[i % colors.Length],
                    Size = new Size(20, 20),
                    Location = new Point(20, startY + i * 30)
                };

                var statusLabel = new Label
                {
                    Text = $"{status.Status}: {status.Count} ({percentage:F1}%)",
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location = new Point(50, startY + i * 30 + 2),
                    AutoSize = true
                };

                chartArea.Controls.AddRange(new Control[] { colorBox, statusLabel });
            }
        }

        private void LoadLeadConversionChart()
        {
            var chartArea = leadConversionChartCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("chartArea_"));
            if (chartArea == null) return;

            chartArea.Controls.Clear();

            // Calculate conversion funnel
            var totalLeads = _leadViewModel.Leads.Count(l => l.IsActive);
            var qualifiedLeads = _leadViewModel.Leads.Count(l => l.IsActive && l.Status.ToLower() != "new");
            var activeDeals = _dealViewModel.Deals.Count(d => d.IsActive);
            var closedDeals = _dealViewModel.Deals.Count(d => d.Status.ToLower() == "closed");

            var funnelData = new[]
            {
                new { Stage = "Total Leads", Count = totalLeads, Color = Color.FromArgb(59, 130, 246) },
                new { Stage = "Qualified", Count = qualifiedLeads, Color = Color.FromArgb(16, 185, 129) },
                new { Stage = "Active Deals", Count = activeDeals, Color = Color.FromArgb(245, 158, 11) },
                new { Stage = "Closed Deals", Count = closedDeals, Color = Color.FromArgb(139, 92, 246) }
            };

            var maxCount = Math.Max(totalLeads, 1);
            var maxWidth = chartArea.Width - 40;

            for (int i = 0; i < funnelData.Length; i++)
            {
                var stage = funnelData[i];
                var barWidth = (int)((double)stage.Count / maxCount * maxWidth);
                var percentage = totalLeads > 0 ? (double)stage.Count / totalLeads * 100 : 0;

                var bar = new Panel
                {
                    BackColor = stage.Color,
                    Size = new Size(barWidth, 25),
                    Location = new Point(20, 30 + i * 40)
                };

                var label = new Label
                {
                    Text = $"{stage.Stage}: {stage.Count} ({percentage:F1}%)",
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location = new Point(20, 10 + i * 40),
                    AutoSize = true
                };

                chartArea.Controls.AddRange(new Control[] { label, bar });
            }
        }

        private void LoadAnalyticsContent()
        {
            try
            {
                LoadRevenueAnalytics();
                LoadPropertyAnalytics();
                LoadPerformanceAnalytics();
                LoadTrendsAnalytics();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading analytics content: {ex.Message}");
            }
        }

        private void LoadRevenueAnalytics()
        {
            var contentPanel = revenueAnalyticsCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("content_"));
            if (contentPanel == null) return;

            contentPanel.Controls.Clear();

            var deals = _dealViewModel.Deals.Where(d => d.Value.HasValue).ToList();
            var thisMonth = DateTime.Now;
            var lastMonth = thisMonth.AddMonths(-1);

            var thisMonthRevenue = deals.Where(d => d.CreatedAt.Month == thisMonth.Month && d.CreatedAt.Year == thisMonth.Year && d.Status.ToLower() == "closed").Sum(d => d.Value ?? 0);
            var lastMonthRevenue = deals.Where(d => d.CreatedAt.Month == lastMonth.Month && d.CreatedAt.Year == lastMonth.Year && d.Status.ToLower() == "closed").Sum(d => d.Value ?? 0);
            var growth = lastMonthRevenue > 0 ? ((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue * 100) : 0;

            var metrics = new[]
            {
                $"This Month: ₱{thisMonthRevenue:N0}",
                $"Last Month: ₱{lastMonthRevenue:N0}",
                $"Growth: {growth:+0.0;-0.0;0}%",
                $"Pipeline Value: ₱{deals.Where(d => d.Status.ToLower() == "active").Sum(d => d.Value ?? 0):N0}"
            };

            for (int i = 0; i < metrics.Length; i++)
            {
                var label = new Label
                {
                    Text = metrics[i],
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location = new Point(0, i * 25),
                    AutoSize = true
                };
                contentPanel.Controls.Add(label);
            }
        }

        private void LoadPropertyAnalytics()
        {
            var contentPanel = propertyAnalyticsCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("content_"));
            if (contentPanel == null) return;

            contentPanel.Controls.Clear();

            var properties = _propertyViewModel.Properties.Where(p => p.IsActive).ToList();
            var totalValue = properties.Sum(p => p.Price);
            var totalArea = properties.Sum(p => p.SquareMeters);
            var avgPricePerSqm = totalArea > 0 ? totalValue / (decimal)totalArea : 0;
            var avgPrice = properties.Any() ? properties.Average(p => p.Price) : 0;

            var metrics = new[]
            {
                $"Total Properties: {properties.Count}",
                $"Average Price: ₱{avgPrice:N0}",
                $"Total Area: {totalArea:F0} sqm",
                $"Avg Price per sqm: ₱{avgPricePerSqm:N0}"
            };

            for (int i = 0; i < metrics.Length; i++)
            {
                var label = new Label
                {
                    Text = metrics[i],
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location = new Point(0, i * 25),
                    AutoSize = true
                };
                contentPanel.Controls.Add(label);
            }
        }

        private void LoadPerformanceAnalytics()
        {
            var contentPanel = performanceAnalyticsCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("content_"));
            if (contentPanel == null) return;

            contentPanel.Controls.Clear();

            var leads = _leadViewModel.Leads.Where(l => l.IsActive).ToList();
            var deals = _dealViewModel.Deals.Where(d => d.IsActive).ToList();
            var contacts = _contactViewModel.Contacts.Where(c => c.IsActive).ToList();

            var metrics = new[]
            {
                $"Active Leads: {leads.Count}",
                $"Active Deals: {deals.Count}",
                $"Total Contacts: {contacts.Count}",
                $"Deal Success Rate: {(leads.Count > 0 ? (double)deals.Count(d => d.Status.ToLower() == "closed") / leads.Count * 100 : 0):F1}%"
            };

            for (int i = 0; i < metrics.Length; i++)
            {
                var label = new Label
                {
                    Text = metrics[i],
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location = new Point(0, i * 25),
                    AutoSize = true
                };
                contentPanel.Controls.Add(label);
            }
        }



        private void LoadTotalCounts()
        {
            var contentPanel = totalCountsCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("content_"));
            if (contentPanel == null) return;

            contentPanel.Controls.Clear();

            var totalProperties = _propertyViewModel.Properties.Count(p => p.IsActive);
            var totalLeads = _leadViewModel.Leads.Count(l => l.IsActive);
            var totalContacts = _contactViewModel.Contacts.Count(c => c.IsActive);
            var totalDeals = _dealViewModel.Deals.Count(d => d.IsActive);

            var counts = new[]
            {
                new { Icon = "🏠", Label = "Total Properties", Count = totalProperties, Color = Color.FromArgb(59, 130, 246) },
                new { Icon = "🎯", Label = "Total Leads", Count = totalLeads, Color = Color.FromArgb(16, 185, 129) },
                new { Icon = "👥", Label = "Total Contacts", Count = totalContacts, Color = Color.FromArgb(245, 158, 11) },
                new { Icon = "💼", Label = "Total Deals", Count = totalDeals, Color = Color.FromArgb(139, 92, 246) }
            };

            // Create a horizontal layout for the counts
            var itemWidth = (contentPanel.Width - 40) / 4; // Divide available width by 4 items

            for (int i = 0; i < counts.Length; i++)
            {
                var item = counts[i];

                var itemPanel = new Panel
                {
                    Size = new Size(itemWidth - 10, contentPanel.Height - 20),
                    Location = new Point(i * itemWidth + 10, 10),
                    BackColor = Color.Transparent
                };

                var iconLabel = new Label
                {
                    Text = item.Icon,
                    Font = new Font("Segoe UI", 24F),
                    Location = new Point(itemWidth / 2 - 15, 10),
                    AutoSize = true
                };

                var countLabel = new Label
                {
                    Text = item.Count.ToString(),
                    Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                    ForeColor = item.Color,
                    Location = new Point(itemWidth / 2 - 20, 50),
                    AutoSize = true
                };

                var labelText = new Label
                {
                    Text = item.Label,
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location = new Point(itemWidth / 2 - 40, 80),
                    AutoSize = true
                };

                itemPanel.Controls.AddRange(new Control[] { iconLabel, countLabel, labelText });
                contentPanel.Controls.Add(itemPanel);
            }
        }

        private void LoadTrendsAnalytics()
        {
            var contentPanel = trendsAnalyticsCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("content_"));
            if (contentPanel == null) return;

            contentPanel.Controls.Clear();

            var thisMonth = DateTime.Now;
            var lastMonth = thisMonth.AddMonths(-1);

            var thisMonthLeads = _leadViewModel.Leads.Count(l => l.CreatedAt.Month == thisMonth.Month && l.CreatedAt.Year == thisMonth.Year);
            var lastMonthLeads = _leadViewModel.Leads.Count(l => l.CreatedAt.Month == lastMonth.Month && l.CreatedAt.Year == lastMonth.Year);
            var leadGrowth = lastMonthLeads > 0 ? ((double)(thisMonthLeads - lastMonthLeads) / lastMonthLeads * 100) : 0;

            var thisMonthProperties = _propertyViewModel.Properties.Count(p => p.CreatedAt.Month == thisMonth.Month && p.CreatedAt.Year == thisMonth.Year);
            var lastMonthProperties = _propertyViewModel.Properties.Count(p => p.CreatedAt.Month == lastMonth.Month && p.CreatedAt.Year == lastMonth.Year);
            var propertyGrowth = lastMonthProperties > 0 ? ((double)(thisMonthProperties - lastMonthProperties) / lastMonthProperties * 100) : 0;

            var metrics = new[]
            {
                $"Lead Growth: {leadGrowth:+0.0;-0.0;0}%",
                $"Property Growth: {propertyGrowth:+0.0;-0.0;0}%",
                $"New Leads: {thisMonthLeads}",
                $"New Properties: {thisMonthProperties}"
            };

            for (int i = 0; i < metrics.Length; i++)
            {
                var label = new Label
                {
                    Text = metrics[i],
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location = new Point(0, i * 25),
                    AutoSize = true
                };
                contentPanel.Controls.Add(label);
            }
        }

        private void UpdateLastUpdatedTime()
        {
            if (lblLastUpdated != null)
            {
                lblLastUpdated.Text = $"Last updated: {DateTime.Now:MMM dd, yyyy HH:mm}";
            }
        }

        private void totalCountsCard_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}