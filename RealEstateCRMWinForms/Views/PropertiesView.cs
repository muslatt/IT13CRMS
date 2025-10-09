using RealEstateCRMWinForms.Controls;
using RealEstateCRMWinForms.ViewModels;
using System.Drawing;
using System.Windows.Forms;
using RealEstateCRMWinForms.Utils;
using System.Linq;
using System;
using System.Collections.Generic;
using RealEstateCRMWinForms.Models;
using System.ComponentModel;

namespace RealEstateCRMWinForms.Views
{
    public enum ApprovalFilterMode
    {
        All,
        ApprovedOnly,
        PendingOnly,
        RejectedOnly
    }

    public partial class PropertiesView : UserControl
    {
        private readonly PropertyViewModel _viewModel;
        private const int PageSize = 10;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private List<Property> _currentProperties = new();
        private string _searchQuery = string.Empty;
        private PropertyFilterOptions _filters = new();
        private bool _isReadOnly;
        private bool _isBrowseMode;
        private bool _isMyListingsMode;
        private bool _showApprovedOnly;
        private bool _showPendingApproval;
        private ApprovalFilterMode _currentFilterMode = ApprovalFilterMode.All;

        public PropertiesView()
        {
            _isReadOnly = false;
            _isBrowseMode = false;
            _viewModel = new PropertyViewModel();
            InitializeComponent();
            // Enable smoother scrolling/painting in the panel hosting cards
            Utils.ControlExtensions.EnableDoubleBuffering(flowLayoutPanel);

            // Make the header area resolution-adaptive
            SetupSearchHeaderLayout();

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

            LoadProperties();
        }

        public PropertiesView(bool isReadOnly) : this()
        {
            _isReadOnly = isReadOnly;
            _isBrowseMode = false;

            // For Broker side only: hide the Add Property and filter buttons
            if (!_isReadOnly) // Broker side
            {
                if (btnAddProperty != null) btnAddProperty.Visible = false;
                if (btnShowAll != null) btnShowAll.Visible = false;
                if (btnShowApproved != null) btnShowApproved.Visible = false;
                if (btnShowPending != null) btnShowPending.Visible = false;
                if (btnShowRejected != null) btnShowRejected.Visible = false;
            }

            // Load properties with filtering
            LoadProperties();
        }

        public PropertiesView(bool isReadOnly, bool isBrowseMode) : this()
        {
            _isReadOnly = isReadOnly;
            _isBrowseMode = isBrowseMode;
            _isMyListingsMode = false;

            // In browse mode, automatically show only approved properties
            if (isBrowseMode)
            {
                _showApprovedOnly = true;
                _currentFilterMode = ApprovalFilterMode.ApprovedOnly;
            }

            // In browse mode, hide the Add Property button
            if (btnAddProperty != null)
            {
                btnAddProperty.Visible = false;
            }
            // In browse mode, hide the approval filter buttons
            if (btnShowAll != null) btnShowAll.Visible = false;
            if (btnShowApproved != null) btnShowApproved.Visible = false;
            if (btnShowPending != null) btnShowPending.Visible = false;
            if (btnShowRejected != null) btnShowRejected.Visible = false;
            // Load properties with filtering
            LoadProperties();
        }

        public PropertiesView(bool isReadOnly, bool isBrowseMode, bool isMyListingsMode) : this()
        {
            _isReadOnly = isReadOnly;
            _isBrowseMode = isBrowseMode;
            _isMyListingsMode = isMyListingsMode;

            // In browse mode, hide the Add Property button
            // In My Listings mode, show the Add Property button for clients
            if (btnAddProperty != null)
            {
                btnAddProperty.Visible = isMyListingsMode && !_isReadOnly;
            }

            // Show the approval filter buttons only in My Listings mode
            if (btnShowAll != null)
            {
                btnShowAll.Visible = isMyListingsMode;
            }
            if (btnShowApproved != null)
            {
                btnShowApproved.Visible = isMyListingsMode;
            }
            if (btnShowPending != null)
            {
                btnShowPending.Visible = isMyListingsMode;
            }
            if (btnShowRejected != null)
            {
                btnShowRejected.Visible = isMyListingsMode;
            }

            if (isMyListingsMode)
            {
                UpdateButtonStyles();
                ApplyMyListingsLayout();
            }

            // Load properties with filtering
            LoadProperties();
        }

        private void ApplyMyListingsLayout()
        {
            try
            {
                // Reduce paddings to help fit 4x2 cards without vertical scroll bars on common screens
                if (flowLayoutPanel != null)
                {
                    AdjustBottomPaddingForFooter();
                }
                if (searchPanel != null)
                {
                    searchPanel.Padding = new Padding(20, 10, 20, 10);
                }
                if (paginationPanel != null)
                {
                    paginationPanel.Padding = new Padding(20, 8, 20, 8);
                    // Recalculate padding whenever footer size changes
                    paginationPanel.SizeChanged -= PaginationPanel_SizeChanged;
                    paginationPanel.SizeChanged += PaginationPanel_SizeChanged;
                }
            }
            catch { /* layout-only safety */ }
        }

        private void PaginationPanel_SizeChanged(object? sender, EventArgs e)
        {
            AdjustBottomPaddingForFooter();
        }

        private void AdjustBottomPaddingForFooter()
        {
            if (flowLayoutPanel == null) return;
            int footer = paginationPanel?.Height ?? 0;
            int bottomPad = Math.Max(footer + 24, 72);
            flowLayoutPanel.Padding = new Padding(16, 16, 16, bottomPad);
            flowLayoutPanel.AutoScrollMargin = new Size(0, bottomPad);
            flowLayoutPanel.PerformLayout();
            flowLayoutPanel.Invalidate();
            flowLayoutPanel.Update();
        }

        private void SetupSearchHeaderLayout()
        {
            try
            {
                // DPI-aware autoscale for this control
                AutoScaleMode = AutoScaleMode.Dpi;

                // Stretch the search box with available width; keep controls panel on the right
                if (searchBoxContainer != null)
                {
                    searchBoxContainer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                }
                if (rightControlsPanel != null)
                {
                    rightControlsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    rightControlsPanel.AutoSize = true;
                    rightControlsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    SetupRightControlsFlow(rightControlsPanel);
                }

                // Recompute sizes on resize to prevent overlap and adapt to width changes
                if (searchPanel != null)
                {
                    searchPanel.Resize -= SearchPanel_Resize;
                    searchPanel.Resize += SearchPanel_Resize;
                    SearchPanel_Resize(searchPanel, EventArgs.Empty);
                }
            }
            catch { /* layout-only safety */ }
        }

        private static void SetupRightControlsFlow(Panel host)
        {
            try
            {
                if (host.Controls.OfType<FlowLayoutPanel>().Any()) return; // already converted
                var children = host.Controls.Cast<Control>().OrderBy(c => c.Left).ToList();
                host.Controls.Clear();
                var flow = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    BackColor = Color.Transparent,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };
                foreach (var c in children)
                {
                    c.Margin = new Padding(c.Margin.Left, 0, 8, 0);
                    flow.Controls.Add(c);
                }
                host.Controls.Add(flow);
            }
            catch { }
        }

        private void SearchPanel_Resize(object? sender, EventArgs e)
        {
            if (searchPanel == null || searchBoxContainer == null || rightControlsPanel == null)
                return;

            var pad = searchPanel.Padding;
            var available = searchPanel.ClientSize.Width - pad.Horizontal;
            var rightWidth = rightControlsPanel.Width;
            var spacing = 12; // gap between left search and right controls
            var targetWidth = Math.Max(220, available - rightWidth - spacing);

            // Position left search box
            searchBoxContainer.Location = new Point(pad.Left, searchBoxContainer.Location.Y);
            searchBoxContainer.Size = new Size(targetWidth, searchBoxContainer.Height);

            // Ensure right controls stay within the panel bounds (anchored to right)
            rightControlsPanel.Location = new Point(searchPanel.ClientSize.Width - pad.Right - rightWidth, rightControlsPanel.Location.Y);
        }

        private void BtnAddProperty_Click(object? sender, EventArgs e)
        {
            var addPropertyForm = new AddPropertyForm();
            if (addPropertyForm.ShowDialog() == DialogResult.OK)
            {
                // Refresh the properties list
                LoadProperties();
            }
        }

        private void LoadProperties()
        {
            bool showRejected = _currentFilterMode == ApprovalFilterMode.RejectedOnly;
            _viewModel.LoadProperties(_isReadOnly, _isBrowseMode, _isMyListingsMode, _showApprovedOnly, _showPendingApproval, showRejected);
            _currentProperties = _viewModel.Properties?.ToList() ?? new List<Property>();
            ApplyFilterAndSort(resetPage: false);
        }

        private void Card_PropertyUpdated(object? sender, PropertyEventArgs e)
        {
            // Refresh the properties list to reflect any changes
            LoadProperties();
        }

        private void Card_PropertyDeleted(object? sender, PropertyEventArgs e)
        {
            if (sender is PropertyCard card)
            {
                // Remove the card from the UI
                flowLayoutPanel.Controls.Remove(card);

                // Dispose of the card to free resources
                card.Dispose();

                // Optionally refresh the entire list to ensure consistency
                LoadProperties();
            }
        }

        private void ChkApprovedOnly_CheckedChanged(object? sender, EventArgs e)
        {
            // This method is no longer used - replaced with toggle button
        }

        private void ChkPendingApproval_CheckedChanged(object? sender, EventArgs e)
        {
            // This method is no longer used - replaced with toggle button
        }

        private void BtnShowAll_Click(object? sender, EventArgs e)
        {
            _currentFilterMode = ApprovalFilterMode.All;
            _showApprovedOnly = false;
            _showPendingApproval = false;
            UpdateButtonStyles();
            LoadProperties();
        }

        private void BtnShowApproved_Click(object? sender, EventArgs e)
        {
            _currentFilterMode = ApprovalFilterMode.ApprovedOnly;
            _showApprovedOnly = true;
            _showPendingApproval = false;
            UpdateButtonStyles();
            LoadProperties();
        }

        private void BtnShowPending_Click(object? sender, EventArgs e)
        {
            _currentFilterMode = ApprovalFilterMode.PendingOnly;
            _showApprovedOnly = false;
            _showPendingApproval = true;
            UpdateButtonStyles();
            LoadProperties();
        }

        private void BtnShowRejected_Click(object? sender, EventArgs e)
        {
            _currentFilterMode = ApprovalFilterMode.RejectedOnly;
            _showApprovedOnly = false;
            _showPendingApproval = false;
            UpdateButtonStyles();
            LoadProperties();
        }

        private void UpdateButtonStyles()
        {
            if (btnShowAll == null || btnShowApproved == null || btnShowPending == null || btnShowRejected == null)
                return;

            // Define colors
            var activeBlue = Color.FromArgb(0, 123, 255);
            var inactiveGray = Color.FromArgb(107, 114, 128);
            var activeBorderBlue = Color.FromArgb(0, 123, 255);
            var inactiveBorderGray = Color.FromArgb(209, 213, 219);

            // Reset all buttons to inactive state
            btnShowAll.ForeColor = inactiveGray;
            btnShowAll.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btnShowAll.FlatAppearance.BorderColor = inactiveBorderGray;

            btnShowApproved.ForeColor = inactiveGray;
            btnShowApproved.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btnShowApproved.FlatAppearance.BorderColor = inactiveBorderGray;

            btnShowPending.ForeColor = inactiveGray;
            btnShowPending.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btnShowPending.FlatAppearance.BorderColor = inactiveBorderGray;

            btnShowRejected.ForeColor = inactiveGray;
            btnShowRejected.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            btnShowRejected.FlatAppearance.BorderColor = inactiveBorderGray;

            // Highlight the active button based on current filter mode
            switch (_currentFilterMode)
            {
                case ApprovalFilterMode.All:
                    btnShowAll.ForeColor = activeBlue;
                    btnShowAll.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    btnShowAll.FlatAppearance.BorderColor = activeBorderBlue;
                    break;
                case ApprovalFilterMode.ApprovedOnly:
                    btnShowApproved.ForeColor = activeBlue;
                    btnShowApproved.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    btnShowApproved.FlatAppearance.BorderColor = activeBorderBlue;
                    break;
                case ApprovalFilterMode.PendingOnly:
                    btnShowPending.ForeColor = activeBlue;
                    btnShowPending.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    btnShowPending.FlatAppearance.BorderColor = activeBorderBlue;
                    break;
                case ApprovalFilterMode.RejectedOnly:
                    btnShowRejected.ForeColor = activeBlue;
                    btnShowRejected.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    btnShowRejected.FlatAppearance.BorderColor = activeBorderBlue;
                    break;
            }
        }

        private void UpdateToggleButtonText()
        {
            // This method is no longer used - replaced with separate buttons
        }

        private void flowLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void searchBoxContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ApplyPagination()
        {
            // Suspend drawing and layout to avoid flicker while we rebuild the list
            flowLayoutPanel.SuspendLayout();
            try
            {
                // Clear existing cards and dispose them properly
                var cardsToRemove = flowLayoutPanel.Controls.OfType<PropertyCard>().ToList();
                foreach (var card in cardsToRemove)
                {
                    // Unsubscribe from events to prevent memory leaks
                    card.PropertyUpdated -= Card_PropertyUpdated;
                    card.PropertyDeleted -= Card_PropertyDeleted;
                    flowLayoutPanel.Controls.Remove(card);
                    card.Dispose();
                }

                // Remove any existing bottom spacer to avoid duplicates
                var oldSpacer = flowLayoutPanel.Controls
                    .Cast<Control>()
                    .FirstOrDefault(c => string.Equals(c.Name, "bottomSpacer", StringComparison.Ordinal));
                if (oldSpacer != null)
                {
                    flowLayoutPanel.Controls.Remove(oldSpacer);
                    oldSpacer.Dispose();
                }

                // Reset scroll position to the top for each page change
                try
                {
                    flowLayoutPanel.AutoScrollPosition = new Point(0, 0);
                }
                catch { }

                if (_currentProperties.Count == 0)
                {
                    _totalPages = 1;
                    UpdatePaginationControls();
                    return;
                }

                var pageSize = _isMyListingsMode ? 8 : PageSize;
                _totalPages = (int)Math.Ceiling(_currentProperties.Count / (double)pageSize);
                if (_currentPage > _totalPages) _currentPage = 1;

                var pageItems = _currentProperties
                    .Skip((_currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Create cards one by one and ensure they're properly initialized
                foreach (var property in pageItems)
                {
                    // Create a completely fresh card
                    var card = new PropertyCard(_isReadOnly, _isBrowseMode)
                    {
                        Margin = _isMyListingsMode ? new Padding(8) : new Padding(10)
                    };

                    // CRITICAL: Set property IMMEDIATELY after creation, before any other operations
                    card.SetProperty(property);

                    // Force immediate visual update
                    card.Refresh();
                    Application.DoEvents(); // Process any pending UI updates

                    // Subscribe to events AFTER the card is fully set up
                    card.PropertyUpdated += Card_PropertyUpdated;
                    card.PropertyDeleted += Card_PropertyDeleted;

                    // Add to the flow panel
                    flowLayoutPanel.Controls.Add(card);

                    // Final refresh after adding to container
                    card.Invalidate(true);
                    card.Update();
                }

                // Force complete refresh of the container
                flowLayoutPanel.PerformLayout();
                flowLayoutPanel.Refresh();
            }
            finally
            {
                flowLayoutPanel.ResumeLayout(true);
            }

            // Add an invisible spacer at the bottom in My Listings to prevent the last row from being clipped
            if (_isMyListingsMode && flowLayoutPanel != null)
            {
                int footer = paginationPanel?.Height ?? 0;
                int spacerHeight = Math.Max(footer + 24, 72);
                var spacer = new Panel
                {
                    Name = "bottomSpacer",
                    Width = Math.Max(1, flowLayoutPanel.ClientSize.Width),
                    Height = spacerHeight, // breathing room below last row
                    Margin = new Padding(0),
                    BackColor = Color.Transparent
                };
                flowLayoutPanel.Controls.Add(spacer);
                try { flowLayoutPanel.SetFlowBreak(spacer, true); } catch { }
                flowLayoutPanel.PerformLayout();
                flowLayoutPanel.Invalidate();
                flowLayoutPanel.Update();
            }

            // Re-apply bottom padding based on footer size after adding content
            if (_isMyListingsMode)
            {
                AdjustBottomPaddingForFooter();
            }

            UpdatePaginationControls();
        }

        private void UpdatePaginationControls()
        {
            BuildNumericPagination();
        }

        private void BuildNumericPagination()
        {
            // Find the page numbers container added in the designer
            var pnl = pageNumbersPanel; // direct field from designer
            if (pnl == null) return;

            pnl.SuspendLayout();
            try
            {
                // Clear existing buttons
                foreach (Control c in pnl.Controls)
                {
                    c.Click -= PageNumber_Click; // best-effort detach
                    c.Dispose();
                }
                pnl.Controls.Clear();

                int total = Math.Max(1, _totalPages);
                int current = Math.Max(1, Math.Min(_currentPage, total));

                void AddEllipsis()
                {
                    var lbl = new Label
                    {
                        Text = "…",
                        AutoSize = false,
                        Width = 20,
                        Height = 28,
                        Margin = new Padding(4, 0, 4, 0),
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = Color.FromArgb(107, 114, 128),
                        BackColor = Color.Transparent,
                        Font = new Font("Segoe UI", 9F, FontStyle.Regular)
                    };
                    pnl.Controls.Add(lbl);
                }

                void AddPageButton(int i)
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

                    bool isCurrent = (i == current);
                    if (isCurrent)
                    {
                        btn.BackColor = Color.FromArgb(37, 99, 235);
                        btn.ForeColor = Color.White;
                        btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    }

                    btn.Click += PageNumber_Click;
                    pnl.Controls.Add(btn);
                }

                if (total <= 7)
                {
                    for (int i = 1; i <= total; i++)
                        AddPageButton(i);
                }
                else
                {
                    // Always show first page
                    AddPageButton(1);

                    if (current > 4)
                        AddEllipsis();

                    if (current <= 4)
                    {
                        // Show first block
                        for (int i = 2; i <= Math.Min(5, total - 1); i++)
                            AddPageButton(i);
                    }
                    else if (current >= total - 3)
                    {
                        // Show last block
                        for (int i = Math.Max(2, total - 4); i <= total - 1; i++)
                            AddPageButton(i);
                    }
                    else
                    {
                        // Middle window around current
                        for (int i = current - 1; i <= current + 1; i++)
                            AddPageButton(i);
                    }

                    if (current < total - 3)
                        AddEllipsis();

                    // Always show last page
                    AddPageButton(total);
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
                    ApplyPagination();
                }
            }
        }

        private void SearchBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                var newQuery = searchBox?.Text?.Trim() ?? string.Empty;
                if (!string.Equals(newQuery, _searchQuery, StringComparison.Ordinal))
                {
                    _searchQuery = newQuery;
                }
                ApplyFilterAndSort(resetPage: true);
            }
        }

        private void SortComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ApplyFilterAndSort(resetPage: false);
        }

        private void BtnFilter_Click(object? sender, EventArgs e)
        {
            using var dlg = new PropertyFilterDialog(_filters, GetDistinctPropertyTypes());
            var result = dlg.ShowDialog();
            if (dlg.Cleared)
            {
                _filters = new PropertyFilterOptions();
                ApplyFilterAndSort(resetPage: true);
                return;
            }
            if (result == DialogResult.OK)
            {
                _filters = dlg.Result ?? new PropertyFilterOptions();
                ApplyFilterAndSort(resetPage: true);
            }
        }

        private List<string> GetDistinctPropertyTypes()
        {
            try
            {
                return _viewModel.Properties
                    .Select(p => p.PropertyType)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s)
                    .ToList();
            }
            catch { return new List<string>(); }
        }

        private void ApplyFilterAndSort(bool resetPage)
        {
            IEnumerable<Property> data = _viewModel.Properties ?? new BindingList<Property>();

            // Search
            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                var q = _searchQuery.Trim();
                data = data.Where(p => MatchesSearch(p, q));
            }

            // Filters (OR logic across provided fields; min/max combine as a range)
            if (_filters != null)
            {
                bool statusOn = !string.IsNullOrWhiteSpace(_filters.Status);
                bool typeOn = !string.IsNullOrWhiteSpace(_filters.PropertyType);
                bool transOn = !string.IsNullOrWhiteSpace(_filters.TransactionType);
                bool minPriceOn = _filters.MinPrice.HasValue;
                bool maxPriceOn = _filters.MaxPrice.HasValue;
                bool priceOn = minPriceOn || maxPriceOn;
                bool minBedsOn = _filters.MinBedrooms.HasValue;
                bool minBathsOn = _filters.MinBathrooms.HasValue;

                bool anyOn = statusOn || typeOn || transOn || priceOn || minBedsOn || minBathsOn;

                if (anyOn)
                {
                    data = data.Where(p =>
                    {
                        bool match = false;

                        if (statusOn)
                            match = match || false; // Status field removed, always false
                        if (typeOn)
                            match = match || string.Equals(p.PropertyType, _filters.PropertyType, StringComparison.OrdinalIgnoreCase);
                        if (transOn)
                            match = match || string.Equals(p.TransactionType, _filters.TransactionType, StringComparison.OrdinalIgnoreCase);
                        if (priceOn)
                        {
                            bool priceMatch;
                            if (minPriceOn && maxPriceOn)
                                priceMatch = p.Price >= _filters.MinPrice!.Value && p.Price <= _filters.MaxPrice!.Value;
                            else if (minPriceOn)
                                priceMatch = p.Price >= _filters.MinPrice!.Value;
                            else
                                priceMatch = p.Price <= _filters.MaxPrice!.Value;
                            match = match || priceMatch;
                        }
                        if (minBedsOn)
                            match = match || p.Bedrooms >= _filters.MinBedrooms!.Value;
                        if (minBathsOn)
                            match = match || p.Bathrooms >= _filters.MinBathrooms!.Value;

                        return match;
                    });
                }
            }

            var list = data.ToList();
            list = SortProperties(list, sortComboBox?.SelectedItem as string);

            _currentProperties = list;
            if (resetPage) _currentPage = 1;
            ApplyPagination();
        }

        private static bool MatchesSearch(Property p, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return true;

            // Split into tokens by whitespace
            var tokens = (query ?? string.Empty)
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToList();

            // Prepare numeric strings without separators for price and area
            string priceDigits = new string(p.Price.ToString("F0").Where(char.IsDigit).ToArray());
            string sqmDigits = new string(p.LotAreaSqm.ToString("F0").Where(char.IsDigit).ToArray());
            string bedDigits = new string(p.Bedrooms.ToString().Where(char.IsDigit).ToArray());
            string bathDigits = new string(p.Bathrooms.ToString().Where(char.IsDigit).ToArray());

            // Build a normalized haystack of common text fields
            string Hay(string? s) => (s ?? string.Empty).ToLowerInvariant();
            var haystack = string.Join(" ", new[]
            {
                Hay(p.Title), Hay(p.Address), Hay(p.Description),
                Hay(p.PropertyType), priceDigits, sqmDigits, bedDigits, bathDigits
            });

            foreach (var raw in tokens)
            {
                var t = raw.ToLowerInvariant();
                bool matched = false;

                // Support patterns like "3br", "3bed", "2ba", "2bath"
                if (!matched && (t.EndsWith("br") || t.EndsWith("bed") || t.EndsWith("beds")))
                {
                    var num = new string(t.Where(char.IsDigit).ToArray());
                    if (int.TryParse(num, out var n) && p.Bedrooms == n)
                        matched = true;
                }
                if (!matched && (t.EndsWith("ba") || t.EndsWith("bath") || t.EndsWith("baths")))
                {
                    var num = new string(t.Where(char.IsDigit).ToArray());
                    if (int.TryParse(num, out var n) && p.Bathrooms == n)
                        matched = true;
                }

                // Digits-only token ? try match on price/area/bed/bath numbers
                if (!matched)
                {
                    var tokenDigits = new string(t.Where(char.IsDigit).ToArray());
                    if (!string.IsNullOrEmpty(tokenDigits))
                    {
                        if ((priceDigits?.Contains(tokenDigits) ?? false) ||
                            (sqmDigits?.Contains(tokenDigits) ?? false) ||
                            (bedDigits == tokenDigits) ||
                            (bathDigits == tokenDigits))
                        {
                            matched = true;
                        }
                    }
                }

                // Fallback: substring in text fields
                if (!matched && haystack.Contains(t))
                {
                    matched = true;
                }

                if (!matched)
                {
                    return false; // all tokens must match something
                }
            }

            return true;
        }

        private static List<Property> SortProperties(List<Property> props, string? sort)
        {
            return (sort ?? "Newest to Oldest") switch
            {
                "Price: Low to High" => props.OrderBy(p => p.Price).ToList(),
                "Price: High to Low" => props.OrderByDescending(p => p.Price).ToList(),
                _ => props.OrderByDescending(p => p.CreatedAt).ToList(),
            };
        }

        // Prev/Next handlers removed in favor of numeric page buttons
    }
}
