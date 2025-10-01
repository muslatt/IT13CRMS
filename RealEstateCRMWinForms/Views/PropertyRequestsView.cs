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
        private Button? btnRefresh;
        private TextBox? searchBox;
        private Label? searchIcon;
        private ComboBox? sortComboBox;
        private Button? btnFilter;
        private Button? btnShowPending;
        private Button? btnShowRejected;
        private Label? lblPageInfo;
        private Button? btnPrevPage;
        private Button? btnNextPage;

        public PropertyRequestsView()
        {
            _viewModel = new PropertyRequestsViewModel();
            InitializeComponent();
            // Enable smoother scrolling/painting in the panel hosting cards
            Utils.ControlExtensions.EnableDoubleBuffering(flowLayoutPanel);

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
            if (btnRefresh != null)
            {
                btnRefresh.Click += (_, __) => LoadProperties();
            }
            if (btnShowPending != null)
            {
                btnShowPending.Click += BtnShowPending_Click;
            }
            if (btnShowRejected != null)
            {
                btnShowRejected.Click += BtnShowRejected_Click;
            }
            if (btnPrevPage != null)
            {
                btnPrevPage.Click += (_, __) => ChangePage(-1);
            }
            if (btnNextPage != null)
            {
                btnNextPage.Click += (_, __) => ChangePage(1);
            }

            LoadProperties();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            // Header panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(16)
            };

            // Refresh button
            btnRefresh = new Button
            {
                Text = "Refresh",
                AutoSize = true,
                Location = new Point(0, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            // Search box
            searchBox = new TextBox
            {
                Location = new Point(100, 8),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Search properties..."
            };

            // Search icon
            searchIcon = new Label
            {
                Location = new Point(355, 12),
                Size = new Size(20, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Sort combo
            sortComboBox = new ComboBox
            {
                Location = new Point(380, 8),
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
                Location = new Point(540, 8),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            // Show Pending Requests button
            btnShowPending = new Button
            {
                Text = "Pending Requests",
                Location = new Point(630, 8),
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
                Location = new Point(780, 8),
                Size = new Size(160, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(233, 236, 239), // Gray when inactive
                ForeColor = Color.FromArgb(73, 80, 87),
                Font = new Font("Segoe UI", 10F)
            };

            // Page controls
            lblPageInfo = new Label
            {
                Text = "Page 1 of 1",
                AutoSize = true,
                Location = new Point(950, 12),
                Font = new Font("Segoe UI", 10F)
            };

            btnPrevPage = new Button
            {
                Text = "Previous",
                Location = new Point(1030, 8),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            btnNextPage = new Button
            {
                Text = "Next",
                Location = new Point(1120, 8),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            headerPanel.Controls.AddRange(new Control[] { btnRefresh, searchBox, searchIcon, sortComboBox, btnFilter, btnShowPending, btnShowRejected, lblPageInfo, btnPrevPage, btnNextPage });

            // Flow layout panel for property cards
            flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(16)
            };

            Controls.Add(flowLayoutPanel);
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

            // Update UI
            lblPageInfo!.Text = $"Page {_currentPage} of {_totalPages}";
            btnPrevPage!.Enabled = _currentPage > 1;
            btnNextPage!.Enabled = _currentPage < _totalPages;

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
                    btnShowRejected.BackColor = Color.FromArgb(220, 53, 69);
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

        private void ChangePage(int delta)
        {
            _currentPage += delta;
            if (_currentPage < 1) _currentPage = 1;
            if (_currentPage > _totalPages) _currentPage = _totalPages;
            ApplyFilterAndSort(resetPage: false);
        }
    }
}