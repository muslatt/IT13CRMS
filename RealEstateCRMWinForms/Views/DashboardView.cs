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

        // Filter control for the occupation card
        private ComboBox? cmbOccupation;

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

        // Helper: compute a nice rounded axis maximum given a target value and tick count
        private static decimal GetNiceAxisMax(decimal targetMax, int ticks)
        {
            if (ticks <= 0) ticks = 5;
            if (targetMax <= 0) return 1; // fallback

            double x = (double)targetMax;
            // step size is a nice number of (1, 2, 5, 10) * 10^n
            double step = NiceNumber(x / ticks, roundUp: true);
            double axisMax = step * ticks; // ensures top gridline >= targetMax
            return (decimal)axisMax;
        }

        private static double NiceNumber(double x, bool roundUp)
        {
            if (x <= 0) return 1;
            double exp = Math.Floor(Math.Log10(x));
            double f = x / Math.Pow(10, exp); // fractional part 1..10
            double nf;
            if (roundUp)
            {
                if (f <= 1) nf = 1;
                else if (f <= 2) nf = 2;
                else if (f <= 5) nf = 5;
                else nf = 10;
            }
            else
            {
                if (f < 1.5) nf = 1;
                else if (f < 3) nf = 2;
                else if (f < 7) nf = 5;
                else nf = 10;
            }
            return nf * Math.Pow(10, exp);
        }

        // Helper: format axis values with compact currency units
        private static string FormatCurrencyCompact(decimal value)
        {
            if (value >= 1_000_000m)
                return $"₱{value / 1_000_000m:0.#}M";
            if (value >= 1_000m)
                return $"₱{value / 1_000m:0.#}k";
            return $"₱{value:0}";
        }

        private void DashboardView_Load(object? sender, EventArgs e)
        {
            try
            {
                // Initialize all analytics components
                InitializeKPICards();
                // Ensure Projected Revenue KPI exists
                CreateKPICard(projectedRevenueCard, "₱", "Projected Revenue", "₱0", "Sum of closed deal commissions", Color.FromArgb(99, 102, 241));
                InitializeCharts();

                InitializeTotalCounts();
                // keep System Overview layout tidy
                CenterTotalCountsLayout();
                var contentPanel = totalCountsCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("content_"));
                if (contentPanel != null)
                {
                    contentPanel.Resize -= (s, args) => CenterTotalCountsLayout();
                    contentPanel.Resize += (s, args) => CenterTotalCountsLayout();
                }
                InitializeAnalyticsCards();

                // Load analytics data
                LoadAnalyticsData();
                UpdateProjectedRevenueCard();

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

        private void UpdateProjectedRevenueCard()
        {
            try
            {
                _dealViewModel.LoadDeals();
                decimal total = 0m;
                var closed = _dealViewModel.Deals.Where(d => string.Equals(d.Status, BoardViewModel.ClosedBoardName, StringComparison.OrdinalIgnoreCase)).ToList();
                if (closed.Count > 0)
                {
                    var userVm = new UserViewModel();
                    userVm.LoadUsers();
                    foreach (var d in closed)
                    {
                        if (d.Value.HasValue && d.Value.Value > 0)
                        {
                            total += d.Value.Value;
                            continue;
                        }
                        decimal price = d.Property?.Price ?? 0m;
                        decimal percent = 0.05m;
                        if (!string.IsNullOrWhiteSpace(d.CreatedBy))
                        {
                            var u = userVm.Users.FirstOrDefault(u => string.Equals(u.FullName, d.CreatedBy, StringComparison.OrdinalIgnoreCase));
                            if (u != null && u.Role == Models.UserRole.Broker) percent = 0.10m;
                        }
                        total += Math.Round(price * percent, 2);
                    }
                }
                var label = projectedRevenueCard?.Controls.Find($"value_{projectedRevenueCard.Name}", true).FirstOrDefault() as Label;
                if (label != null)
                {
                    label.Text = $"₱{total:N0}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating Projected Revenue: {ex.Message}");
            }
        }

        private void InitializeCharts()
        {
            try
            {
                // Sales Chart Card
                CreateChartCard(salesChartCard, "Monthly Sales", "Track sales performance over time");
                if (salesChartCard != null)
                {
                    salesChartCard.Resize -= (s, e) => LoadSalesChart();
                    salesChartCard.Resize += (s, e) => LoadSalesChart();
                }

                // Property Status Chart Card  
                CreateChartCard(propertyStatusChartCard, "Property Status", "Distribution of property statuses");
                if (propertyStatusChartCard != null)
                {
                    propertyStatusChartCard.Resize -= (s, e) => LoadPropertyStatusChart();
                    propertyStatusChartCard.Resize += (s, e) => LoadPropertyStatusChart();
                }

                // Lead Conversion Chart Card
                CreateChartCard(leadConversionChartCard, "Lead Conversion", "Lead conversion funnel");
                if (leadConversionChartCard != null)
                {
                    leadConversionChartCard.Resize -= (s, e) => LoadLeadConversionChart();
                    leadConversionChartCard.Resize += (s, e) => LoadLeadConversionChart();
                }

                // Average Client Salary Chart Card
                CreateChartCard(avgSalaryChartCard, "Average Client Salary (Last 12 months)", "Monthly average - clients only");
                if (avgSalaryChartCard != null)
                {
                    avgSalaryChartCard.Resize -= (s, e) => LoadAverageSalaryChart();
                    avgSalaryChartCard.Resize += (s, e) => LoadAverageSalaryChart();
                }

                // Preferred Property Type by Occupation Card
                CreateOccupationPreferenceCard();
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

            // Header uses Dock=Top, content fills remaining space to avoid clipping
            var headerPanel = new Panel
            {
                Height = 64,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };

            var headerFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = false,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 12, 20, 0),
                Margin = new Padding(0)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 0)
            };

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = true,
                Margin = new Padding(0, 2, 0, 8)
            };

            headerFlow.Controls.Add(titleLabel);
            headerFlow.Controls.Add(descLabel);
            headerPanel.Controls.Add(headerFlow);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 8, 20, 16)
            };

            // Chart area fills content panel ensuring full visibility
            var chartArea = new Panel
            {
                BackColor = Color.FromArgb(249, 250, 251),
                Dock = DockStyle.Fill,
                Name = $"chartArea_{card.Name}"
            };
            contentPanel.Controls.Add(chartArea);

            // Add Fill first, then Top so Fill sits below header
            card.Controls.Add(contentPanel);
            card.Controls.Add(headerPanel);
        }

        private void CreateOccupationPreferenceCard()
        {
            if (occupationPreferenceCard == null) return;
            occupationPreferenceCard.Controls.Clear();

            // Left information panel (title, filter, hint) - Fixed width with proper separation
            var infoPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300, // Reduced width to provide more space for chart
                BackColor = Color.White,
                Padding = new Padding(20, 12, 10, 12) // Reduced right padding
            };

            var titleLabel = new Label
            {
                Text = "Property Type by Occupation",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblSelect = new Label
            {
                Text = "Select Occupation",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(75, 85, 99),
                AutoSize = true,
                Location = new Point(0, 34)
            };

            cmbOccupation = new ComboBox
            {
                Location = new Point(0, 56),
                Size = new Size(260, 24), // Adjusted width to fit within the panel
                Font = new Font("Segoe UI", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbOccupation.SelectedIndexChanged += (s, e) => RenderOccupationPreferenceChart();

            var hintLabel = new Label
            {
                Text = "Counts derived from Deals with matched Contact and Property, grouped by type.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = true,
                MaximumSize = new Size(260, 0), // Adjusted to fit within panel
                Location = new Point(0, 90)
            };

            // Populate occupations
            var occupations = _contactViewModel.Contacts
                .Where(c => c.IsActive && !string.IsNullOrWhiteSpace(c.Occupation))
                .Select(c => c.Occupation!.Trim())
                .Distinct()
                .OrderBy(o => o)
                .ToList();

            if (occupations.Any())
            {
                cmbOccupation.Items.Add("All");
                foreach (var o in occupations) cmbOccupation.Items.Add(o);
                cmbOccupation.SelectedIndex = 0;
            }
            else
            {
                cmbOccupation.Items.Add("No data available");
                cmbOccupation.SelectedIndex = 0;
                cmbOccupation.Enabled = false;
            }

            // Add a separator panel between info and chart
            var separatorPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 20, // 20px separation
                BackColor = Color.White
            };

            // Right chart host panel - now properly separated
            var chartHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 8, 20, 8) // Adjusted padding
            };

            var chartArea = new Panel
            {
                Name = "chartArea_occupationPreference",
                BackColor = Color.FromArgb(249, 250, 251),
                Dock = DockStyle.Fill
            };

            chartHost.Controls.Add(chartArea);

            // Add controls to info panel
            infoPanel.Controls.Add(hintLabel);
            infoPanel.Controls.Add(cmbOccupation);
            infoPanel.Controls.Add(lblSelect);
            infoPanel.Controls.Add(titleLabel);

            // Add panels in correct order: Fill first, then Left panels
            occupationPreferenceCard.Controls.Add(chartHost);      // Fill - added first
            occupationPreferenceCard.Controls.Add(separatorPanel); // Left separator
            occupationPreferenceCard.Controls.Add(infoPanel);      // Left info panel

            // Handle resize events
            occupationPreferenceCard.Resize -= OccupationPreferenceCard_Resize;
            occupationPreferenceCard.Resize += OccupationPreferenceCard_Resize;

            RenderOccupationPreferenceChart();
        }

        private void OccupationPreferenceCard_Resize(object? sender, EventArgs e)
        {
            // With docking, only need to re-render to adjust bar sizes
            RenderOccupationPreferenceChart();
        }

        private void RenderOccupationPreferenceChart()
        {
            var chartArea = occupationPreferenceCard?
                .Controls
                .Find("chartArea_occupationPreference", true)
                .OfType<Panel>()
                .FirstOrDefault();
            if (chartArea == null) return;

            chartArea.Controls.Clear();

            string? occ = (cmbOccupation != null && cmbOccupation.Enabled && cmbOccupation.SelectedItem != null)
                ? cmbOccupation.SelectedItem.ToString()
                : null;

            // Build data sets: allDeals (for consistent axis) and deals (filtered for bars)
            var allDeals = _dealViewModel.Deals
                .Where(d => d.IsActive && d.Contact != null && d.Property != null && d.Contact!.IsActive)
                .ToList();

            var deals = allDeals;
            if (!string.IsNullOrWhiteSpace(occ) && occ != "All")
            {
                deals = allDeals
                    .Where(d => string.Equals(d.Contact!.Occupation, occ, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var categories = new[] { "Residential", "Commercial", "Raw Land", "Other" };
            var counts = categories.ToDictionary(c => c, c => 0);
            foreach (var d in deals)
            {
                var type = d.Property!.PropertyType?.Trim();
                if (string.IsNullOrWhiteSpace(type)) type = "Other";
                if (!categories.Contains(type)) type = "Other";
                counts[type]++;
            }

            // Compute global counts (across all occupations) to keep a consistent Y-axis
            var globalCounts = categories.ToDictionary(c => c, c => 0);
            foreach (var d in allDeals)
            {
                var type = d.Property!.PropertyType?.Trim();
                if (string.IsNullOrWhiteSpace(type)) type = "Other";
                if (!categories.Contains(type)) type = "Other";
                globalCounts[type]++;
            }

            // Chart layout - adjusted for better spacing
            int leftPad = 60; // Reduced left padding since we have proper separation now
            int bottomPad = 40;
            int topPad = 20;
            int width = Math.Max(10, chartArea.Width - leftPad - 20);
            int height = Math.Max(10, chartArea.Height - topPad - bottomPad);
            int gap = 18;
            int barW = Math.Max(10, (width - gap * (categories.Length + 1)) / categories.Length);
            // Use global maximum for consistent axis across occupation selections
            int max = Math.Max(1, globalCounts.Values.Max());

            // Grid lines
            var gridColor = Color.FromArgb(229, 231, 235);
            for (int i = 0; i <= 5; i++)
            {
                int y = topPad + height * i / 5;
                var grid = new Panel { BackColor = gridColor, Location = new Point(leftPad, y), Size = new Size(width, 1) };
                int gv = (int)Math.Round(max * (1 - i / 5.0));
                var yLabel = new Label { Text = gv.ToString(), Font = new Font("Segoe UI", 8F), ForeColor = Color.FromArgb(107, 114, 128), AutoSize = true, Location = new Point(10, y - 8) };
                chartArea.Controls.AddRange(new Control[] { grid, yLabel });
            }

            // Draw bars
            for (int i = 0; i < categories.Length; i++)
            {
                string cat = categories[i];
                int val = counts[cat];
                int barH = (int)(height * (val / (double)max));
                int x = leftPad + gap + i * (barW + gap);
                int y = topPad + (height - barH);

                var bar = new Panel { BackColor = Color.FromArgb(59, 130, 246), Location = new Point(x, y), Size = new Size(barW, barH) };
                var xtxt = new Label { Text = cat, Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(75, 85, 99), AutoSize = true, Location = new Point(x + Math.Max(0, (barW - TextRenderer.MeasureText(cat, new Font("Segoe UI", 9F)).Width) / 2), topPad + height + 6) };
                var vtxt = new Label { Text = val.ToString(), Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(33, 37, 41), AutoSize = true, Location = new Point(x + barW / 2 - 6, y - 16) };
                chartArea.Controls.AddRange(new Control[] { bar, xtxt, vtxt });
            }
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

            // Header layout: icon + title, no absolute positions (prevents clipping)
            var headerPanel = new Panel
            {
                Height = 56,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };

            var headerFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 12, 20, 12),
                Margin = new Padding(0)
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 18F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                Margin = new Padding(0, 0, 8, 0)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize = true,
                Margin = new Padding(0)
            };

            headerFlow.Controls.Add(iconLabel);
            headerFlow.Controls.Add(titleLabel);
            headerPanel.Controls.Add(headerFlow);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 28, 20, 20),
                Name = $"content_{card.Name}"
            };

            // Important: add Fill first, then Top so the Fill area
            // is measured below the header and won't render underneath it.
            card.Controls.Add(contentPanel);
            card.Controls.Add(headerPanel);
        }

        private void LoadAnalyticsData()
        {
            try
            {
                LoadKPIData();
                LoadChartData();

                LoadTotalCounts();
                RebuildTotalCountsUniform();
                CenterTotalCountsLayout();
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
                LoadAverageSalaryChart();
                RenderOccupationPreferenceChart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading chart data: {ex.Message}");
            }
        }

        private void LoadSalesChart()
        {
            var chartArea = salesChartCard?
                .Controls
                .Find($"chartArea_{salesChartCard.Name}", true)
                .OfType<Panel>()
                .FirstOrDefault();
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
            var chartArea = propertyStatusChartCard?
                .Controls
                .Find($"chartArea_{propertyStatusChartCard.Name}", true)
                .OfType<Panel>()
                .FirstOrDefault();
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
            var chartArea = leadConversionChartCard?
                .Controls
                .Find($"chartArea_{leadConversionChartCard.Name}", true)
                .OfType<Panel>()
                .FirstOrDefault();
            if (chartArea == null) return;

            chartArea.Controls.Clear();

            // Calculate conversion funnel
            var totalLeads = _leadViewModel.Leads.Count(l => l.IsActive);
            // Since leads don't have a status, consider leads with high scores as qualified
            var qualifiedLeads = _leadViewModel.Leads.Count(l => l.IsActive && l.Score >= 40);
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

        private void LoadAverageSalaryChart()
        {
            var chartArea = avgSalaryChartCard?
                .Controls
                .Find($"chartArea_{avgSalaryChartCard.Name}", true)
                .OfType<Panel>()
                .FirstOrDefault();
            if (chartArea == null) return;

            chartArea.Controls.Clear();

            // Fixed range: September 2024 through September 2025 (inclusive)
            var startDate = new DateTime(2024, 9, 1);
            var endDateInclusive = new DateTime(2025, 9, 30);
            var monthsCount = ((endDateInclusive.Year - startDate.Year) * 12 + endDateInclusive.Month - startDate.Month) + 1;

            // Get monthly average salary data for the fixed range
            var monthlySalaryData = new List<(DateTime Month, decimal AverageSalary)>();

            for (int i = 0; i < monthsCount; i++)
            {
                var monthDate = startDate.AddMonths(i);
                var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                // Get contacts created in this month that have salary data
                var contactsInMonth = _contactViewModel.Contacts
                    .Where(c => c.IsActive &&
                               c.CreatedAt >= monthStart &&
                               c.CreatedAt <= monthEnd &&
                               c.Salary.HasValue &&
                               c.Salary > 0)
                    .ToList();

                var avgSalary = contactsInMonth.Any() ? contactsInMonth.Average(c => c.Salary!.Value) : 0;
                monthlySalaryData.Add((monthDate, avgSalary));
            }

            // Determine axis max dynamically from Contacts table salaries and data
            var maxContactSalary = _contactViewModel.Contacts
                .Where(c => c.IsActive && c.Salary.HasValue && c.Salary > 0)
                .Select(c => c.Salary!.Value)
                .DefaultIfEmpty(0)
                .Max();

            var dataMax = monthlySalaryData.Any() ? monthlySalaryData.Max(x => x.AverageSalary) : 0;
            var targetMax = Math.Max(maxContactSalary, dataMax);
            if (targetMax <= 0) targetMax = 1; // fallback

            int ticks = 5; // number of grid segments
            var axisMax = GetNiceAxisMax(targetMax, ticks);

            // Create line chart representation
            var chartWidth = chartArea.Width - 80;
            var chartHeight = chartArea.Height - 60;
            var pointWidth = chartWidth / Math.Max(monthlySalaryData.Count - 1, 1);

            // Draw grid lines and Y-axis labels based on dynamic axisMax
            var gridColor = Color.FromArgb(229, 231, 235);
            for (int i = 0; i <= ticks; i++)
            {
                var y = 30 + (chartHeight * i / ticks);
                var gridLine = new Panel
                {
                    BackColor = gridColor,
                    Size = new Size(chartWidth, 1),
                    Location = new Point(40, y)
                };

                var yValue = axisMax * (ticks - i) / ticks;
                var yLabel = new Label
                {
                    Text = FormatCurrencyCompact(yValue),
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = Color.FromArgb(107, 114, 128),
                    AutoSize = true,
                    Location = new Point(5, y - 8)
                };

                chartArea.Controls.AddRange(new Control[] { gridLine, yLabel });
            }

            // Draw data points and connecting lines scaled to axisMax
            var chartColor = Color.FromArgb(59, 130, 246);
            Panel? previousPoint = null;

            for (int i = 0; i < monthlySalaryData.Count; i++)
            {
                var data = monthlySalaryData[i];
                var x = 40 + (i * pointWidth);
                var y = axisMax > 0 ? 30 + (int)((axisMax - (data.AverageSalary)) / axisMax * chartHeight) : 30 + chartHeight / 2;

                // Create data point
                var point = new Panel
                {
                    BackColor = chartColor,
                    Size = new Size(6, 6),
                    Location = new Point(x - 3, y - 3)
                };

                // Create connecting line to previous point (simple approximation)
                if (previousPoint != null && i > 0)
                {
                    var prevX = previousPoint.Location.X + 3;
                    var prevY = previousPoint.Location.Y + 3;
                    var steps = Math.Max(Math.Abs(x - prevX), Math.Abs(y - prevY));
                    for (int step = 0; step < steps; step += 2)
                    {
                        var lineX = prevX + ((x - prevX) * step / steps);
                        var lineY = prevY + ((y - prevY) * step / steps);
                        var linePoint = new Panel
                        {
                            BackColor = chartColor,
                            Size = new Size(2, 2),
                            Location = new Point(lineX, lineY)
                        };
                        chartArea.Controls.Add(linePoint);
                    }
                }

                // X-axis labels (month names)
                if (i % 2 == 0) // Show every other month to avoid crowding
                {
                    var monthLabel = new Label
                    {
                        Text = data.Month.ToString("MMM yyyy"),
                        Font = new Font("Segoe UI", 7F),
                        ForeColor = Color.FromArgb(107, 114, 128),
                        AutoSize = true,
                        Location = new Point(x - 20, chartHeight + 35)
                    };
                    chartArea.Controls.Add(monthLabel);
                }

                chartArea.Controls.Add(point);
                previousPoint = point;
            }

            // Add legend
            var legendPanel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(140, 25),
                Location = new Point(chartArea.Width - 160, 10)
            };

            var legendColor = new Panel
            {
                BackColor = chartColor,
                Size = new Size(15, 3),
                Location = new Point(8, 11)
            };

            var legendLabel = new Label
            {
                Text = "Avg Salary (₱)",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(28, 6),
                AutoSize = true
            };

            legendPanel.Controls.AddRange(new Control[] { legendColor, legendLabel });
            chartArea.Controls.Add(legendPanel);
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

            // Create a horizontal layout for the counts with better spacing
            var itemWidth = (contentPanel.Width - 60) / 4; // More padding for better spacing
            var startY = 15; // Starting Y position
            var contentHeight = contentPanel.Height - 30; // Available content height

            for (int i = 0; i < counts.Length; i++)
            {
                var item = counts[i];

                var itemPanel = new Panel
                {
                    Size = new Size(itemWidth - 15, contentHeight),
                    Location = new Point(i * itemWidth + 30, startY),
                    BackColor = Color.Transparent
                };

                // Center the content within each item panel
                var centerX = itemWidth / 2 - 7;

                var iconLabel = new Label
                {
                    Text = item.Icon,
                    Font = new Font("Segoe UI", 28F), // Slightly larger icon
                    Location = new Point(centerX - 20, 15),
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var countLabel = new Label
                {
                    Text = item.Count.ToString(),
                    Font = new Font("Segoe UI", 24F, FontStyle.Bold), // Larger count text
                    ForeColor = item.Color,
                    Location = new Point(centerX - 25, 65),
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var labelText = new Label
                {
                    Text = item.Label,
                    Font = new Font("Segoe UI", 12F), // Slightly larger label text
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location = new Point(centerX - 50, 110),
                    Size = new Size(100, 40),
                    TextAlign = ContentAlignment.MiddleCenter
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

        // Rebuild System Overview counts using a uniform grid and stacked layout
        private void RebuildTotalCountsUniform()
        {
            var contentPanel = totalCountsCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("content_"));
            if (contentPanel == null) return;

            contentPanel.Controls.Clear();
            // Slightly tighter top padding so labels don't get clipped in fixed-height cards
            contentPanel.Padding = new Padding(20, 22, 20, 20);

            // Use Segoe MDL2 Assets for consistent glyphs across Windows
            var mdl2 = "Segoe MDL2 Assets";
            var totals = new (string Icon, string Label, int Count, Color Color, string FontFamily)[]
            {
                ("\uE80F", "Total Properties", _propertyViewModel.Properties.Count(p => p.IsActive), Color.FromArgb(59,130,246), mdl2), // Home
                ("\uE77B", "Total Leads", _leadViewModel.Leads.Count(l => l.IsActive), Color.FromArgb(16,185,129), mdl2),             // Contact
                ("\uE716", "Total Contacts", _contactViewModel.Contacts.Count(c => c.IsActive), Color.FromArgb(245,158,11), mdl2),     // People
                ("\uE821", "Total Deals", _dealViewModel.Deals.Count(d => d.IsActive), Color.FromArgb(139,92,246), mdl2)               // Briefcase
            };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = totals.Length,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            for (int i = 0; i < totals.Length; i++)
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / totals.Length));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            for (int i = 0; i < totals.Length; i++)
            {
                var (icon, label, count, color, fontFamily) = totals[i];

                // Inner vertical stack (auto-sized)
                var stack = new TableLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ColumnCount = 1,
                    RowCount = 3,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0),
                    Padding = new Padding(0)
                };
                stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
                stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                stack.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                var iconLabel = new Label
                {
                    Text = icon,
                    // Slightly smaller to fit within card height
                    Font = new Font(fontFamily, 22f, FontStyle.Regular, GraphicsUnit.Point),
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(0, 8, 0, 4)
                };
                var countLabel = new Label
                {
                    Text = count.ToString(),
                    // Slightly smaller to avoid cut-off
                    Font = new Font("Segoe UI", 22f, FontStyle.Bold, GraphicsUnit.Point),
                    ForeColor = color,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(0, 0, 0, 2)
                };
                var textLabel = new Label
                {
                    Text = label,
                    Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    // Reduce bottom margin to keep text inside card
                    Margin = new Padding(0, 0, 0, 6)
                };

                stack.Controls.Add(iconLabel, 0, 0);
                stack.Controls.Add(countLabel, 0, 1);
                stack.Controls.Add(textLabel, 0, 2);

                // Host panel that fills the cell and centers the stack both horizontally and vertically
                var cellHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Margin = new Padding(0), Padding = new Padding(0) };
                cellHost.Controls.Add(stack);

                void CenterInCell()
                {
                    var pref = stack.PreferredSize;
                    var x = Math.Max(0, (cellHost.Width - pref.Width) / 2);
                    var y = Math.Max(0, (cellHost.Height - pref.Height) / 2);
                    stack.Location = new Point(x, y);
                }
                cellHost.Resize += (s, e) => CenterInCell();
                // Initial center
                cellHost.HandleCreated += (s, e) => CenterInCell();

                grid.Controls.Add(cellHost, i, 0);
            }

            contentPanel.Controls.Add(grid);
        }
        // Keeps the counts in "System Overview" vertically stacked and centered to avoid overlap
        private void CenterTotalCountsLayout()
        {
            var contentPanel = totalCountsCard?.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.StartsWith("content_"));
            if (contentPanel == null) return;

            foreach (var itemPanel in contentPanel.Controls.OfType<Panel>())
            {
                var labels = itemPanel.Controls.OfType<Label>().ToList();
                if (labels.Count == 0)
                {
                    // Also support a nested layout container
                    var inner = itemPanel.Controls.OfType<Control>().FirstOrDefault();
                    if (inner != null)
                    {
                        labels = inner.Controls.OfType<Label>().ToList();
                    }
                }
                if (labels.Count == 0) continue;

                int y = 12;
                foreach (var lbl in labels)
                {
                    if (lbl == labels.First())
                        lbl.Font = new Font("Segoe UI Emoji", lbl.Font.Size, lbl.Font.Style);

                    var pref = lbl.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
                    lbl.AutoSize = true;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Location = new Point(Math.Max(0, (itemPanel.Width - pref.Width) / 2), y);
                    y += pref.Height + 8;
                }
            }
        }
    }
}
