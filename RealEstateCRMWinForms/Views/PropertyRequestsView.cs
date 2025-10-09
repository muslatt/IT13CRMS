using RealEstateCRMWinForms.Controls;
using RealEstateCRMWinForms.ViewModels;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System;
using System.Collections.Generic;
using RealEstateCRMWinForms.Models;
using System.ComponentModel;

namespace RealEstateCRMWinForms.Views
{
    public class PropertyRequestsView : UserControl
    {
        private readonly PropertyRequestsViewModel _viewModel;
        private const int PageSize = 10;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private List<Property> _currentProperties = new();
        private string _searchQuery = string.Empty;
        private PropertyFilterOptions _filters = new();
        private bool _showRejectedProperties = false;

        // UI Controls
        private FlowLayoutPanel? flowLayoutPanel;
        private Panel? headerPanel;
        private Panel? footerPanel;
        private TextBox? searchBox;
        private Label? searchIcon;
        private ComboBox? sortComboBox;
        private Button? btnFilter;
        private Button? btnShowPending;
        private Button? btnShowRejected;
        private FlowLayoutPanel? pageNumbersPanel;

        public PropertyRequestsView()
        {
            _viewModel = new PropertyRequestsViewModel();
            InitializeComponent();
            // Enable smoother scrolling/painting in the panel hosting cards
            if (flowLayoutPanel != null)
            {
                Utils.ControlExtensions.EnableDoubleBuffering(flowLayoutPanel);
            }

            // Wire up search/sort/filter events
            if (searchBox != null)
            {
                _searchQuery = searchBox.Text?.Trim() ?? string.Empty;
                searchBox.KeyDown += SearchBox_KeyDown;
            }
            if (searchIcon != null)
            {
                try { searchIcon.Font = new Font("Segoe MDL2 Assets", 12F, FontStyle.Regular); searchIcon.Text = "\uE721"; } catch { }
            }
            if (sortComboBox != null)
            {
                sortComboBox.SelectedIndexChanged += SortComboBox_SelectedIndexChanged;
                if (sortComboBox.SelectedIndex < 0 && sortComboBox.Items.Count > 0)
                    sortComboBox.SelectedIndex = 0;
            }
            if (btnFilter != null)
            {
                btnFilter.Click += BtnFilter_Click;
            }
            // Refresh button removed
            if (btnShowPending != null)
            {
                btnShowPending.Click += BtnShowPending_Click;
            }
            if (btnShowRejected != null)
            {
                btnShowRejected.Click += BtnShowRejected_Click;
            }
            // Numeric page buttons are created dynamically; no Prev/Next handlers needed

            LoadProperties();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            // Header panel - now only contains search and basic controls
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(16)
            };

            // Search box (aligned with cards on X; Y unchanged)
            searchBox = new TextBox
            {
                Location = new Point(28, 15),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Search properties..."
            };

            // Search icon (keep same Y, shift X with search)
            searchIcon = new Label
            {
                Location = new Point(286, 19),
                Size = new Size(20, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Show Pending Requests button
            btnShowPending = new Button
            {
                Text = "Pending Requests",
                Location = new Point(400, 15),
                Size = new Size(140, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 123, 255), // Blue when active
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            // Show Rejected Properties button
            btnShowRejected = new Button
            {
                Text = "Rejected Properties",
                Location = new Point(550, 15),
                Size = new Size(160, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(233, 236, 239), // Gray when inactive
                ForeColor = Color.FromArgb(73, 80, 87),
                Font = new Font("Segoe UI", 10F)
            };

            headerPanel.Controls.AddRange(new Control[] { searchBox, searchIcon, btnShowPending, btnShowRejected });

            // Footer panel - contains pagination and filter controls
            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(16),
                BorderStyle = BorderStyle.None
            };

            // Add a subtle top border to the footer
            var footerBorder = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(222, 226, 230)
            };
            footerPanel.Controls.Add(footerBorder);

            // Sort combo
            sortComboBox = new ComboBox
            {
                Location = new Point(16, 20),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            sortComboBox.Items.AddRange(new object[] {
                "Newest First",
                "Oldest First",
                "Price: High to Low",
                "Price: Low to High",
                "Title A-Z",
                "Title Z-A"
            });

            // Filter button
            btnFilter = new Button
            {
                Text = "Filter",
                Location = new Point(180, 20),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            // Numeric page numbers panel (centered)
            pageNumbersPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            // Reposition numeric pagination to be centered within the footer
            footerPanel.Resize += (sender, e) =>
            {
                if (footerPanel != null && pageNumbersPanel != null)
                {
                    int yPosition = 20;
                    int xCentered = Math.Max(16, (footerPanel.Width - pageNumbersPanel.Width) / 2);
                    pageNumbersPanel.Location = new Point(xCentered, yPosition);
                }
            };

            footerPanel.Controls.AddRange(new Control[] { sortComboBox, btnFilter, pageNumbersPanel });

            // Flow layout panel for property cards - now fills the space between header and footer
            flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(16)
            };

            // Add controls in the correct order for proper docking
            Controls.Add(flowLayoutPanel);
            Controls.Add(footerPanel);
            Controls.Add(headerPanel);
        }

        private void LoadProperties()
        {
            _viewModel.LoadPropertyRequests(_showRejectedProperties);
            _currentProperties = _viewModel.Properties?.ToList() ?? new List<Property>();
            ApplyFilterAndSort(resetPage: true);
        }

        private void ApplyFilterAndSort(bool resetPage)
        {
            if (resetPage)
                _currentPage = 1;

            var filtered = _currentProperties.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                filtered = filtered.Where(p =>
                    p.Title.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Address.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            // Apply additional filters
            if (_filters.MinPrice.HasValue)
                filtered = filtered.Where(p => p.Price >= _filters.MinPrice.Value);
            if (_filters.MaxPrice.HasValue)
                filtered = filtered.Where(p => p.Price <= _filters.MaxPrice.Value);
            if (_filters.MinBedrooms.HasValue)
                filtered = filtered.Where(p => p.Bedrooms >= _filters.MinBedrooms.Value);
            if (_filters.PropertyType != null)
                filtered = filtered.Where(p => p.PropertyType == _filters.PropertyType);
            if (_filters.TransactionType != null)
                filtered = filtered.Where(p => p.TransactionType == _filters.TransactionType);

            // Apply sorting
            filtered = sortComboBox?.SelectedItem?.ToString() switch
            {
                "Oldest First" => filtered.OrderBy(p => p.CreatedAt),
                "Price: High to Low" => filtered.OrderByDescending(p => p.Price),
                "Price: Low to High" => filtered.OrderBy(p => p.Price),
                "Title A-Z" => filtered.OrderBy(p => p.Title),
                "Title Z-A" => filtered.OrderByDescending(p => p.Title),
                _ => filtered.OrderByDescending(p => p.CreatedAt) // Newest First
            };

            var sortedList = filtered.ToList();
            _totalPages = (int)Math.Ceiling(sortedList.Count / (double)PageSize);
            if (_totalPages == 0) _totalPages = 1;
            if (_currentPage > _totalPages) _currentPage = _totalPages;

            var pagedProperties = sortedList.Skip((_currentPage - 1) * PageSize).Take(PageSize).ToList();

            // Update numeric pagination buttons
            BuildNumericPagination();

            // Clear existing cards
            flowLayoutPanel!.Controls.Clear();

            // Add property cards
            foreach (var property in pagedProperties)
            {
                var card = new PropertyRequestCard(property);
                card.PropertyApproved += Card_PropertyApproved;
                card.PropertyRejected += Card_PropertyRejected;
                flowLayoutPanel.Controls.Add(card);
            }
        }

        private void Card_PropertyApproved(object? sender, PropertyEventArgs e)
        {
            // Confirm before approving the property
            var result = MessageBox.Show(
                $"Are you sure you want to approve the property '{e.Property.Title}'?\n\nThis will make it visible to all users.",
                "Confirm Property Approval",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes)
            {
                // Approve the property
                _viewModel.ApproveProperty(e.Property.Id);
                try
                {
                    Services.LoggingService.LogAction(
                        "Broker Approved Property",
                        $"Property '{e.Property.Title}' (property {e.Property.Id}) approved",
                        propertyId: e.Property.Id);
                }
                catch { }
                LoadProperties();

                MessageBox.Show(
                    $"Property '{e.Property.Title}' has been successfully approved!",
                    "Property Approved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void Card_PropertyRejected(object? sender, PropertyEventArgs e)
        {
            // Confirm before rejecting the property
            var result = MessageBox.Show(
                $"Are you sure you want to reject the property '{e.Property.Title}'?\n\nReason: {e.RejectionReason}",
                "Confirm Property Rejection",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                // Reject the property with reason (mark as rejected instead of deleting)
                _viewModel.RejectProperty(e.Property.Id, e.RejectionReason);
                try
                {
                    Services.LoggingService.LogAction(
                        "Broker Rejected Property",
                        $"Property '{e.Property.Title}' (property {e.Property.Id}) - Reason: {e.RejectionReason}",
                        propertyId: e.Property.Id);
                }
                catch { }
                LoadProperties();

                MessageBox.Show(
                    $"Property '{e.Property.Title}' has been rejected.",
                    "Property Rejected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void BtnShowPending_Click(object? sender, EventArgs e)
        {
            _showRejectedProperties = false;
            UpdateButtonStyles();
            LoadProperties();
        }

        private void BtnShowRejected_Click(object? sender, EventArgs e)
        {
            _showRejectedProperties = true;
            UpdateButtonStyles();
            LoadProperties();
        }

        private void UpdateButtonStyles()
        {
            if (btnShowPending != null)
            {
                if (_showRejectedProperties)
                {
                    // Pending is inactive
                    btnShowPending.BackColor = Color.FromArgb(233, 236, 239);
                    btnShowPending.ForeColor = Color.FromArgb(73, 80, 87);
                    btnShowPending.Font = new Font("Segoe UI", 10F);
                }
                else
                {
                    // Pending is active
                    btnShowPending.BackColor = Color.FromArgb(0, 123, 255);
                    btnShowPending.ForeColor = Color.White;
                    btnShowPending.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                }
            }

            if (btnShowRejected != null)
            {
                if (_showRejectedProperties)
                {
                    // Rejected is active
                    btnShowRejected.BackColor = Color.FromArgb(0, 123, 255);
                    btnShowRejected.ForeColor = Color.White;
                    btnShowRejected.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                }
                else
                {
                    // Rejected is inactive
                    btnShowRejected.BackColor = Color.FromArgb(233, 236, 239);
                    btnShowRejected.ForeColor = Color.FromArgb(73, 80, 87);
                    btnShowRejected.Font = new Font("Segoe UI", 10F);
                }
            }
        }

        private void SearchBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _searchQuery = searchBox!.Text?.Trim() ?? string.Empty;
                ApplyFilterAndSort(resetPage: true);
            }
        }

        private void SortComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ApplyFilterAndSort(resetPage: true);
        }

        private void BtnFilter_Click(object? sender, EventArgs e)
        {
            using var filterDialog = new PropertyFilterDialog(_filters, new List<string> { "Residential", "Commercial", "Raw Land" });
            if (filterDialog.ShowDialog() == DialogResult.OK)
            {
                _filters = filterDialog.Result ?? new PropertyFilterOptions();
                ApplyFilterAndSort(resetPage: true);
            }
        }

        private void BuildNumericPagination()
        {
            var pnl = pageNumbersPanel;
            if (pnl == null) return;

            pnl.SuspendLayout();
            try
            {
                foreach (Control c in pnl.Controls)
                {
                    c.Click -= PageNumber_Click;
                    c.Dispose();
                }
                pnl.Controls.Clear();

                int total = Math.Max(1, _totalPages);
                for (int i = 1; i <= total; i++)
                {
                    var btn = new Button
                    {
                        Text = i.ToString(),
                        Tag = i,
                        AutoSize = false,
                        Width = 36,
                        Height = 28,
                        Margin = new Padding(4, 0, 4, 0),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(55, 65, 81),
                        Font = new Font("Segoe UI", 9F, FontStyle.Regular)
                    };
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 244, 246);
                    btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(229, 231, 235);

                    bool isCurrent = (i == _currentPage);
                    if (isCurrent)
                    {
                        btn.BackColor = Color.FromArgb(37, 99, 235);
                        btn.ForeColor = Color.White;
                        btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }

                    btn.Click += PageNumber_Click;
                    pnl.Controls.Add(btn);
                }

                // Center the panel after rebuild (width may have changed)
                if (footerPanel != null && pageNumbersPanel != null)
                {
                    int yPosition = 20;
                    int xCentered = Math.Max(16, (footerPanel.Width - pageNumbersPanel.Width) / 2);
                    pageNumbersPanel.Location = new Point(xCentered, yPosition);
                }
            }
            finally
            {
                pnl.ResumeLayout(true);
            }
        }

        private void PageNumber_Click(object? sender, EventArgs e)
        {
            if (sender is Button b && b.Tag is int page)
            {
                if (page != _currentPage)
                {
                    _currentPage = page;
                    ApplyFilterAndSort(resetPage: false);
                }
            }
        }
    }
}
