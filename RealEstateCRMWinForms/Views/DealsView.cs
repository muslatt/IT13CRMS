// RealEstateCRMWinForms\Views\DealsView.cs
using RealEstateCRMWinForms.Controls;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RealEstateCRMWinForms.Utils;
using RealEstateCRMWinForms.Services;

namespace RealEstateCRMWinForms.Views
{
    public partial class DealsView : UserControl
    {
        private readonly DealViewModel _dealViewModel = new DealViewModel();
        private readonly BoardViewModel _boardViewModel = new BoardViewModel();
        private Dictionary<string, FlowLayoutPanel> _statusPanels;
        private Dictionary<string, Label> _countLabels;
        private Dictionary<string, Color> _statusColors;
        private FlowLayoutPanel _boardPanel;
        private Panel _scrollableContainer;
        private Button btnAddDeal;
        private Button btnAddBoard;
        private ToolTip _toolTip;
        private PendingAssignmentsView _pendingView;
        private Panel _pendingHost;
        private PendingAssignmentsView _pendingViewBroker;
        private Panel _pendingHostBroker;
        private Button btnTogglePending;
        private List<Board> _boards;
        private const int BOARD_WIDTH = 300;
        private const int BOARD_MARGIN = 10;
        private bool _isInitialized = false;

        public DealsView()
        {
            InitializeComponent();
            
            try
            {
                _toolTip = new ToolTip
                {
                    ShowAlways = true,
                    InitialDelay = 200,
                    ReshowDelay = 100,
                    AutoPopDelay = 4000
                };
                InitializeDefaultBoards();
                LoadBoards();
                InitializeDealBoard();
                _isInitialized = true;
                
                // Load deals after everything is set up
                LoadDeals();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DealsView constructor: {ex.Message}");
                MessageBox.Show($"Error initializing deals view: {ex.Message}", 
                    "Initialization Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
            }
        }

        private void InitializeDefaultBoards()
        {
            _boardViewModel.InitializeDefaultBoards();
        }

        private void LoadBoards()
        {
            _boardViewModel.LoadBoards();
            _boards = _boardViewModel.Boards
                .OrderBy(b => b.Order)
                .ThenBy(b => b.CreatedAt)
                .ToList();
            _statusColors = new Dictionary<string, Color>();
            
            foreach (var board in _boards)
            {
                _statusColors[board.Name] = ColorTranslator.FromHtml(board.Color);
            }
        }

        private void InitializeDealBoard()
        {
            // Main container
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            // Header panel with buttons
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            // Add Board button
            btnAddBoard = new Button
            {
                Text = "+ Add Board",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(headerPanel.Width - 270, 12)
            };
            btnAddBoard.FlatAppearance.BorderSize = 0;
            btnAddBoard.Click += BtnAddBoard_Click;

            // Add Deal button
            btnAddDeal = new Button
            {
                Text = "+ Add Deal",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(headerPanel.Width - 140, 12)
            };
            btnAddDeal.FlatAppearance.BorderSize = 0;
            btnAddDeal.Click += BtnAddDeal_Click;

            headerPanel.Controls.AddRange(new Control[] { btnAddBoard, btnAddDeal });

            // Toggle Pending Assignments (Broker)
            btnTogglePending = new Button
            {
                Text = "Pending Assignments",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 123, 255),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Location = new Point(20, 12)
            };
            btnTogglePending.FlatAppearance.BorderSize = 1;
            btnTogglePending.FlatAppearance.BorderColor = Color.FromArgb(0, 123, 255);
            btnTogglePending.Click += (s, e) => ToggleBrokerPendingView();

            // Scrollable container for horizontal scrolling
            _scrollableContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 250),
                AutoScroll = true,
                Padding = new Padding(15, 15, 15, 15)
            };
            // Reduce flicker in scrollable host
            Utils.ControlExtensions.EnableDoubleBuffering(_scrollableContainer);

            // Board panel using FlowLayoutPanel for dynamic layout
            _boardPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(0),
                Margin = new Padding(0),
                Location = new Point(0, 0)
            };
            Utils.ControlExtensions.EnableDoubleBuffering(_boardPanel);

            _statusPanels = new Dictionary<string, FlowLayoutPanel>();
            _countLabels = new Dictionary<string, Label>();

            RefreshBoardLayout();

            _scrollableContainer.Controls.Add(_boardPanel);

            // If current user is an Agent, show Pending Assignments area and limit actions
            var user = UserSession.Instance.CurrentUser;
            if (user != null && user.Role == UserRole.Agent)
            {
                // Hide manual add actions for agents
                btnAddDeal.Visible = false;
                btnAddBoard.Visible = false;
                // hide broker-only toggle
                btnTogglePending.Visible = false;

                // Agents: show only the pipeline (no pending section here)
                mainContainer.Controls.Add(_scrollableContainer);
                mainContainer.Controls.Add(headerPanel);
            }
            else
            {
                // Default ordering without pending view
                mainContainer.Controls.Add(_scrollableContainer);
                mainContainer.Controls.Add(headerPanel);
                // Add broker toggle into header
                headerPanel.Controls.Add(btnTogglePending);
                // Prepare broker pending host (hidden by default)
                _pendingHostBroker = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Visible = false
                };
                _pendingViewBroker = new PendingAssignmentsView(true) { Dock = DockStyle.Fill };
                _pendingViewBroker.AssignmentAccepted += (s, d) => { LoadDeals(); };
                _pendingViewBroker.AssignmentDeclined += (s, d) => { LoadDeals(); };
                _pendingHostBroker.Controls.Add(_pendingViewBroker);
                mainContainer.Controls.Add(_pendingHostBroker);
            }
            this.Controls.Add(mainContainer);

            // Handle resize events
            _scrollableContainer.Resize += ScrollableContainer_Resize;
            this.HandleCreated += DealsView_HandleCreated;
        }

        private void DealsView_HandleCreated(object? sender, EventArgs e)
        {
            // Update heights after the handle is created
            UpdateBoardHeights();
        }

        private void ScrollableContainer_Resize(object? sender, EventArgs e)
        {
            if (_isInitialized)
            {
                UpdateBoardHeights();
            }
        }

        private void ToggleBrokerPendingView()
        {
            if (_pendingHostBroker == null) return;
            bool showingPending = _pendingHostBroker.Visible;
            _pendingHostBroker.Visible = !showingPending;
            _scrollableContainer.Visible = showingPending; // toggle
            btnAddDeal.Visible = showingPending; // hide add while viewing pending
            btnAddBoard.Visible = showingPending;
            btnTogglePending.Text = showingPending ? "Pending Assignments" : "Back to Pipeline";
            if (!showingPending)
            {
                // refresh pending list
                _pendingViewBroker?.GetType().GetMethod("LoadAssignments", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(_pendingViewBroker, null);
            }
        }

        private void UpdateBoardHeights()
        {
            if (_scrollableContainer == null || _statusPanels == null || !_isInitialized) 
                return;

            try
            {
                // Calculate available height
                int availableHeight = Math.Max(_scrollableContainer.ClientSize.Height - _scrollableContainer.Padding.Vertical, 400);
                
                foreach (var panel in _statusPanels.Values)
                {
                    if (panel.Parent is Panel container)
                    {
                        container.Height = availableHeight;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating board heights: {ex.Message}");
            }
        }

        private void RefreshBoardLayout()
        {
            _boardPanel.Controls.Clear();
            _statusPanels.Clear();
            _countLabels.Clear();

            if (_boards.Count == 0) return;

            for (int i = 0; i < _boards.Count; i++)
            {
                var board = _boards[i];
                var color = _statusColors.ContainsKey(board.Name) 
                    ? _statusColors[board.Name] 
                    : Color.FromArgb(173, 216, 230);

                var columnContainer = CreateStatusColumn(board, color);
                _boardPanel.Controls.Add(columnContainer);
            }

            UpdateScrollableContainerSize();
        }

        private void UpdateScrollableContainerSize()
        {
            if (_boardPanel == null) return;
            
            int totalWidth = (_boards.Count * BOARD_WIDTH) + ((_boards.Count + 1) * BOARD_MARGIN);
            _boardPanel.MinimumSize = new Size(totalWidth, 0);
        }

        private Panel CreateStatusColumn(Board board, Color headerColor)
        {
            // Container for the entire column with fixed width
            var container = new Panel 
            { 
                Width = BOARD_WIDTH,
                Height = 600, // Will be updated by UpdateBoardHeights
                Margin = new Padding(BOARD_MARGIN, 0, 0, 0),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Header with count and buttons
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = headerColor,
                Padding = new Padding(15, 10, 15, 10)
            };

            var lblTitle = new Label
            {
                Text = board.Name,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 5)
            };

            // Inline editor for the board title
            var txtTitleEdit = new TextBox
            {
                Visible = false,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(0, 3),
                Width = 160,
                Text = board.Name,
                Tag = board.Id,
                Name = $"txtTitleEdit_{board.Id}"
            };

            // Allow inline editing by double-clicking the title
            lblTitle.DoubleClick += (s, e) =>
            {
                txtTitleEdit.Text = lblTitle.Text;
                txtTitleEdit.Visible = true;
                lblTitle.Visible = false;
                txtTitleEdit.Focus();
                txtTitleEdit.SelectAll();
            };

            // Commit/cancel inline editing from the editor textbox
            txtTitleEdit.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var newName = (txtTitleEdit.Text ?? string.Empty).Trim();
                    var oldName = board.Name;
                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        MessageBox.Show("Board name cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!string.Equals(newName, oldName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (_boards.Any(b => b.Id != board.Id && b.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
                        {
                            MessageBox.Show($"A board named '{newName}' already exists.", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        var success = _boardViewModel.RenameBoard(board, newName);
                        if (!success)
                        {
                            MessageBox.Show("Failed to rename board. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        // Reload entire layout so internal dictionaries (by name) update correctly
                        LoadBoards();
                        RefreshBoardLayout();
                        LoadDeals();
                    }
                    txtTitleEdit.Visible = false;
                    lblTitle.Visible = true;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    txtTitleEdit.Text = lblTitle.Text;
                    txtTitleEdit.Visible = false;
                    lblTitle.Visible = true;
                    e.Handled = true;
                }
            };
            txtTitleEdit.Leave += (s, e) =>
            {
                // Cancel edit on focus loss if still visible
                if (txtTitleEdit.Visible)
                {
                    txtTitleEdit.Visible = false;
                    lblTitle.Visible = true;
                }
            };

            var lblCount = new Label
            {
                Text = "/ 0",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(64, 64, 64),
                AutoSize = true,
                Location = new Point(0, 25)
            };

            _countLabels[board.Name] = lblCount;

            Button? btnDeleteBoard = null;
            Button? btnDeleteClosed = null;
            if (!_boardViewModel.IsSystemBoard(board))
            {
                btnDeleteBoard = new Button
                {
                    // MDL2 Delete glyph
                    Text = "\uE74D",
                    Font = new Font("Segoe MDL2 Assets", 12F, FontStyle.Regular),
                    BackColor = Color.FromArgb(220, 53, 69),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(28, 28),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Tag = board.Id
                };
                btnDeleteBoard.FlatAppearance.BorderSize = 0;
                btnDeleteBoard.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 40, 55);
                btnDeleteBoard.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 35, 50);
                btnDeleteBoard.Click += BtnDeleteBoard_Click;
                if (_toolTip != null)
                {
                    _toolTip.SetToolTip(btnDeleteBoard, "Delete board");
                }
                headerPanel.Controls.Add(btnDeleteBoard);
            }

            // Add a bulk delete button for the Closed/Done system board
            if (string.Equals(board.Name, BoardViewModel.ClosedBoardName, StringComparison.OrdinalIgnoreCase))
            {
                btnDeleteClosed = new Button
                {
                    Text = "\uE74D", // MDL2 delete glyph
                    Font = new Font("Segoe MDL2 Assets", 12F, FontStyle.Regular),
                    BackColor = Color.FromArgb(220, 53, 69),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(28, 28),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Tag = board.Name,
                    Name = "btnDeleteClosedBulk"
                };
                btnDeleteClosed.FlatAppearance.BorderSize = 0;
                btnDeleteClosed.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 40, 55);
                btnDeleteClosed.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 35, 50);
                btnDeleteClosed.Click += BtnDeleteClosed_Click;
                if (_toolTip != null)
                {
                    _toolTip.SetToolTip(btnDeleteClosed, "Delete all closed deals and properties");
                }
                headerPanel.Controls.Add(btnDeleteClosed);
            }

            var btnRenameBoard = new Button
            {
                // MDL2 Edit (pencil) glyph
                Text = "\uE70F",
                Font = new Font("Segoe MDL2 Assets", 12F, FontStyle.Regular),
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(28, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Tag = board.Id
            };
            btnRenameBoard.FlatAppearance.BorderSize = 0;
            btnRenameBoard.FlatAppearance.MouseOverBackColor = Color.FromArgb(90, 97, 112);
            btnRenameBoard.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 77, 92);
            btnRenameBoard.Click += BtnRenameBoard_Click;
            if (_toolTip != null)
            {
                _toolTip.SetToolTip(btnRenameBoard, "Rename board");
            }
            headerPanel.Controls.Add(btnRenameBoard);

            void PositionHeaderButtons()
            {
                const int rightPadding = 10;
                int topPadding = headerPanel.Padding.Top; // align with header content

                int clientRight = headerPanel.ClientSize.Width - headerPanel.Padding.Right;

                if (btnDeleteClosed != null)
                {
                    int deleteClosedX = clientRight - btnDeleteClosed.Width - rightPadding;
                    btnDeleteClosed.Location = new Point(deleteClosedX, topPadding);
                }

                int nextLeft = (btnDeleteClosed != null) ? btnDeleteClosed.Left : clientRight;

                if (btnDeleteBoard != null)
                {
                    int deleteX = nextLeft - btnDeleteBoard.Width - (btnDeleteClosed != null ? 6 : rightPadding);
                    btnDeleteBoard.Location = new Point(deleteX, topPadding);
                }

                int spacing = 6;
                if (btnDeleteBoard != null)
                {
                    int editX = btnDeleteBoard.Left - spacing - btnRenameBoard.Width;
                    btnRenameBoard.Location = new Point(editX, topPadding);
                }
                else
                {
                    int editX = nextLeft - btnRenameBoard.Width - rightPadding;
                    btnRenameBoard.Location = new Point(editX, topPadding);
                }

                btnRenameBoard.BringToFront();
                btnDeleteBoard?.BringToFront();
                btnDeleteClosed?.BringToFront();
            }

            headerPanel.Resize += (s, e) => PositionHeaderButtons();
            PositionHeaderButtons();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, lblCount });

            headerPanel.Controls.Add(txtTitleEdit);

            // Content panel with internal padding
            var columnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(15, 15, 15, 15),
                AllowDrop = true,
                Tag = board.Name
            };
            Utils.ControlExtensions.EnableDoubleBuffering(columnPanel);

            _statusPanels[board.Name] = columnPanel;

            // Wire up drag-and-drop events
            columnPanel.DragEnter += StatusPanel_DragEnter;
            columnPanel.DragDrop += StatusPanel_DragDrop;
            columnPanel.DragOver += StatusPanel_DragOver;

            container.Controls.Add(columnPanel);
            container.Controls.Add(headerPanel);

            // Allow header to receive drops too
            headerPanel.AllowDrop = true;
            headerPanel.DragEnter += (s, e) => StatusPanel_DragEnter(columnPanel, e);
            headerPanel.DragDrop += (s, e) => StatusPanel_DragDrop(columnPanel, e);
            headerPanel.DragOver += (s, e) => StatusPanel_DragOver(columnPanel, e);

            return container;
        }

        private void BtnAddBoard_Click(object? sender, EventArgs e)
        {
            var boardNameDialog = new AddBoardDialog();
            if (boardNameDialog.ShowDialog() == DialogResult.OK)
            {
                string newBoardName = boardNameDialog.BoardName;
                
                if (!string.IsNullOrWhiteSpace(newBoardName) && !_boards.Any(b => b.Name == newBoardName))
                {
                    var newBoard = new Board
                    {
                        Name = newBoardName,
                        Color = "#ADD8E6",
                        IsDefault = false,
                        CreatedBy = "User",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    if (_boardViewModel.AddBoard(newBoard))
                    {
                        LoadBoards();
                        RefreshBoardLayout();
                        LoadDeals();
                        
                        MessageBox.Show($"Board '{newBoardName}' has been successfully created.", 
                            "Board Added", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
                    }
                }
                else if (_boards.Any(b => b.Name == newBoardName))
                {
                    MessageBox.Show("A board with this name already exists!", 
                        "Duplicate Board Name", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnRenameBoard_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Parent is Panel headerPanel)
            {
                var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault(l => l.Font.Bold);
                var editor = headerPanel.Controls.OfType<TextBox>().FirstOrDefault();
                if (titleLabel != null && editor != null)
                {
                    editor.Text = titleLabel.Text;
                    editor.Visible = true;
                    titleLabel.Visible = false;
                    editor.Focus();
                    editor.SelectAll();
                }
            }
        }

        private void BtnDeleteBoard_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int boardId)
            {
                var board = _boards.FirstOrDefault(b => b.Id == boardId);
                if (board != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete the '{board.Name}' board?\n\nAll deals in this board will be moved to '{BoardViewModel.NewBoardName}'.", 
                        "Delete Board", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        if (_boardViewModel.DeleteBoard(board))
                        {
                            LoadBoards();
                            RefreshBoardLayout();
                            LoadDeals();

                            MessageBox.Show($"Board '{board.Name}' has been successfully deleted!", 
                                "Board Deleted", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"System boards '{BoardViewModel.NewBoardName}' and '{BoardViewModel.ClosedBoardName}' cannot be deleted.",
                                "Cannot Delete Board",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void LoadDeals()
        {
            try
            {
                if (_statusPanels == null || _statusPanels.Count == 0)
                {
                    Console.WriteLine("Status panels not initialized yet, skipping deal loading");
                    return;
                }

                // Suspend layout/drawing and clear all panels first
                _boardPanel.SuspendLayout();
                foreach (var panel in _statusPanels.Values)
                {
                    panel.SuspendLayout();
                    panel.SuspendDrawing();
                    panel.Controls.Clear();
                }

                // Force reload from database to ensure we have fresh data
                _dealViewModel.LoadDeals();
                var deals = _dealViewModel.Deals;
                // For agents: only show deals they accepted/created
                var user = UserSession.Instance.CurrentUser;
                if (user != null && user.Role == UserRole.Agent)
                {
                    var agentName = ($"{user.FirstName} {user.LastName}".Trim());
                    deals = new System.ComponentModel.BindingList<Deal>(
                        deals.Where(d => string.Equals(d.CreatedBy ?? string.Empty, agentName, StringComparison.OrdinalIgnoreCase)).ToList());
                }
                // Exclude pending-assignment deals from the pipeline boards
                var filteredDeals = deals.Where(d => string.IsNullOrWhiteSpace(d.Notes) || !d.Notes.Contains("[ASSIGN:"))
                                         .ToList();
                Console.WriteLine($"Loading {filteredDeals.Count} deals");

                foreach (var deal in filteredDeals)
                {
                    var card = new DealCard();
                    card.SetDeal(deal);
                    card.MouseDown += DealCard_MouseDown;

                    // Subscribe to deal events
                    card.DealUpdated += Card_DealUpdated;
                    card.DealDeleted += Card_DealDeleted;

                    // Place card in the correct column based on its status
                    if (_statusPanels.ContainsKey(deal.Status))
                    {
                        _statusPanels[deal.Status].Controls.Add(card);
                        Console.WriteLine($"Added deal '{deal.Title}' to '{deal.Status}' board");
                    }
                    else
                    {
                        // Default to "New" if status not found
                        if (_statusPanels.ContainsKey(BoardViewModel.NewBoardName))
                        {
                            _statusPanels[BoardViewModel.NewBoardName].Controls.Add(card);
                            Console.WriteLine($"Added deal '{deal.Title}' to '{BoardViewModel.NewBoardName}' board (status '{deal.Status}' not found)");
                        }
                    }
                }

                UpdateColumnCounts();

                // Resume drawing/layout
                foreach (var panel in _statusPanels.Values)
                {
                    panel.ResumeLayout();
                    panel.ResumeDrawing();
                }
                _boardPanel.ResumeLayout();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading deals: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Error loading deals: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Card_DealUpdated(object? sender, DealEventArgs e)
        {
            // Reload deals to refresh the entire view with updated data
            LoadDeals();
        }

        private void Card_DealDeleted(object? sender, DealEventArgs e)
        {
            if (sender is DealCard card)
            {
                // Remove the card from its current panel
                if (card.Parent is FlowLayoutPanel panel)
                {
                    panel.Controls.Remove(card);
                    UpdateColumnCounts();
                }
                
                // Dispose of the card to free resources
                card.Dispose();
                
                // Force refresh of the view
                LoadDeals();
            }
        }

        private void UpdateColumnCounts()
        {
            if (_countLabels == null) return;

            foreach (var kvp in _statusPanels)
            {
                var status = kvp.Key;
                var panel = kvp.Value;
                var count = panel.Controls.Count;
                
                if (_countLabels.ContainsKey(status))
                {
                    _countLabels[status].Text = $"/ {count}";
                }
            }
        }

        private async void BtnAddDeal_Click(object? sender, EventArgs e)
        {
            try
            {
                var boardNames = _boards.Select(b => b.Name).ToList();
                var defaultBoard = _boardViewModel.GetDefaultBoard();
                
                var addDealForm = new AddDealForm(boardNames, defaultBoard?.Name);
                if (addDealForm.ShowDialog() == DialogResult.OK && addDealForm.CreatedDeal != null)
                {
                    // Disable the button to prevent multiple clicks
                    if (sender is Button btn)
                    {
                        btn.Enabled = false;
                        btn.Text = "Creating...";
                    }

                    try
                    {
                        if (await _dealViewModel.AddDealAsync(addDealForm.CreatedDeal))
                        {
                            LoadDeals();
                            
                            MessageBox.Show($"Deal '{addDealForm.CreatedDeal.Title}' has been successfully created and the contact has been notified!", 
                                "Deal Added", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to add deal. Please check the console for error details.", "Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    finally
                    {
                        // Re-enable the button
                        if (sender is Button btnSender)
                        {
                            btnSender.Enabled = true;
                            btnSender.Text = "+ Add Deal";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding deal: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Re-enable the button in case of error
                if (sender is Button btnSender)
                {
                    btnSender.Enabled = true;
                    btnSender.Text = "+ Add Deal";
                }
            }
        }

        private void DealCard_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is DealCard card)
            {
                card.DoDragDrop(card, DragDropEffects.Move);
            }
        }

        private void StatusPanel_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DealCard)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void StatusPanel_DragOver(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DealCard)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private async void StatusPanel_DragDrop(object? sender, DragEventArgs e)
        {
            var targetPanel = sender as FlowLayoutPanel;
            var card = e.Data.GetData(typeof(DealCard)) as DealCard;

            if (targetPanel != null && card != null)
            {
                if (targetPanel == card.Parent)
                {
                    return;
                }

                string newStatus = targetPanel.Tag?.ToString() ?? BoardViewModel.NewBoardName;
                var dealToUpdate = card.GetDeal();
                string oldStatus = dealToUpdate.Status;
                
                Console.WriteLine($"Moving deal '{dealToUpdate.Title}' from '{oldStatus}' to '{newStatus}'");
                
                // Temporarily disable drag and drop to prevent multiple operations
                foreach (var panel in _statusPanels.Values)
                {
                    panel.AllowDrop = false;
                }

                try
                {
                    // Brokers cannot move deals whose property is assigned to an Agent
                    var currentUser = UserSession.Instance.CurrentUser;
                    if (currentUser != null && currentUser.Role == UserRole.Broker)
                    {
                        var assignedAgent = dealToUpdate?.Property?.Agent;
                        if (!string.IsNullOrWhiteSpace(assignedAgent))
                        {
                            MessageBox.Show("This property is assigned to an Agent. Brokers cannot move it between pipelines.",
                                "Move Restricted",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // If moving to Closed/Done, confirm and compute commission
                    if (string.Equals(newStatus, BoardViewModel.ClosedBoardName, StringComparison.OrdinalIgnoreCase))
                    {
                        var user = UserSession.Instance.CurrentUser;
                        decimal percent = (user != null && user.Role == UserRole.Broker) ? 0.10m : 0.05m; // Broker 10%, Agent 5%
                        decimal price = dealToUpdate?.Property?.Price ?? 0m;
                        decimal commission = Math.Round(price * percent, 2);

                        var confirm = MessageBox.Show(
                            $"Close this deal? This will record a {percent * 100:0}% commission of ₱{commission:N2}.",
                            "Confirm Close",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        if (confirm != DialogResult.Yes)
                        {
                            return;
                        }

                        dealToUpdate.Value = commission; // store commission in Value
                        dealToUpdate.ClosedAt = DateTime.UtcNow;

                        try
                        {
                            var who = user?.FullName ?? string.Empty;
                            var note = $"Closed by {who}, commission {percent * 100:0}% = ₱{commission:N2} on {DateTime.Now:g}";
                            dealToUpdate.Notes = string.IsNullOrWhiteSpace(dealToUpdate.Notes) ? note : dealToUpdate.Notes + Environment.NewLine + note;
                        }
                        catch { }
                    }

                    if (await _dealViewModel.MoveDealToStatusAsync(dealToUpdate, newStatus))
                    {
                        targetPanel.Controls.Add(card);
                        UpdateColumnCounts();
                        card.SetDeal(dealToUpdate);
                        Console.WriteLine($"Successfully moved deal to '{newStatus}' board and sent notification");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to update deal status in database");
                        MessageBox.Show("Failed to move deal. Please try again.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                finally
                {
                    // Re-enable drag and drop
                    foreach (var panel in _statusPanels.Values)
                    {
                        panel.AllowDrop = true;
                    }
                }
            }
        }

        private void BtnDeleteClosed_Click(object? sender, EventArgs e)
        {
            try
            {
                var confirm = MessageBox.Show(
                    "Delete ALL deals in Closed/Done and their properties?\n\nThis will remove closed deals and mark their linked properties as deleted.",
                    "Delete Closed Deals",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (confirm != DialogResult.Yes) return;

                // Ensure latest data
                _dealViewModel.LoadDeals();
                var propertyVm = new PropertyViewModel();
                propertyVm.LoadProperties();

                var closed = _dealViewModel.Deals
                    .Where(d => d.IsActive && string.Equals(d.Status, BoardViewModel.ClosedBoardName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                int dealsDeleted = 0, propsDeleted = 0;
                foreach (var deal in closed)
                {
                    // Delete linked property (soft delete via ViewModel)
                    if (deal.PropertyId.HasValue)
                    {
                        var prop = propertyVm.GetPropertyById(deal.PropertyId.Value);
                        if (prop != null)
                        {
                            if (propertyVm.DeleteProperty(prop)) propsDeleted++;
                        }
                    }

                    if (_dealViewModel.DeleteDeal(deal)) dealsDeleted++;
                }

                // Refresh UI
                LoadDeals();
                MessageBox.Show($"Deleted {dealsDeleted} closed deals and {propsDeleted} properties.",
                    "Deleted",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting closed items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_isInitialized)
            {
                UpdateBoardHeights();
            }
        }

        // Method to refresh the view when returning from other sections
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && _isInitialized)
            {
                Console.WriteLine("DealsView became visible - refreshing data");
                LoadDeals();
            }
        }
    }

    public class AddBoardDialog : Form
    {
        private TextBox txtBoardName;
        public string BoardName { get; private set; } = string.Empty;

        public AddBoardDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Add New Board";
            // Set client size so positions are relative to the client area (avoids being cut off by window chrome / DPI)
            // Reduced client height so dialog isn't too tall
            this.ClientSize = new Size(560, 260);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // Use 12pt font for the dialog and controls
            Font = new Font("Segoe UI", 12F);

            var lblPrompt = new Label
            {
                Text = "Enter the name for the new board:",
                Location = new Point(20, 20),
                // Make label span the new dialog width with some padding
                Size = new Size(520, 24),
                Font = new Font("Segoe UI", 12F)
            };
            lblPrompt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            txtBoardName = new TextBox
            {
                Location = new Point(20, 50),
                // Wider textbox to fit within the expanded dialog
                Size = new Size(520, 30),
                Font = new Font("Segoe UI", 12F)
            };
            txtBoardName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var btnCancel = new Button
            {
                Text = "Cancel",
                // Anchor buttons to bottom-right so they stay visible if dialog is resized
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(110, 40),
                Font = new Font("Segoe UI", 12F),
                DialogResult = DialogResult.Cancel
            };
            // Position the buttons relative to the client area so they are fully visible
            var paddingBottom = 20;
            btnCancel.Location = new Point(this.ClientSize.Width - btnCancel.Width - 10, this.ClientSize.Height - btnCancel.Height - paddingBottom);

            var btnSave = new Button
            {
                Text = "Add Board",
                // Anchor buttons to bottom-right so they stay visible if dialog is resized
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            btnSave.Location = new Point(this.ClientSize.Width - btnSave.Width - 120 - 20, this.ClientSize.Height - btnSave.Height - paddingBottom);

            Controls.AddRange(new Control[] { lblPrompt, txtBoardName, btnCancel, btnSave });

            // Position the buttons to the bottom-right with padding so they align to the right edge
            var paddingRight = 20;
            paddingBottom = 20;
            btnSave.Location = new Point(this.ClientSize.Width - btnSave.Width - paddingRight, this.ClientSize.Height - btnSave.Height - paddingBottom);
            btnCancel.Location = new Point(btnSave.Location.X - btnCancel.Width - 10, btnSave.Location.Y);
            // Anchor buttons to bottom-right so they remain in place
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

             CancelButton = btnCancel;
             AcceptButton = btnSave;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBoardName.Text))
            {
                MessageBox.Show("Please enter a board name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBoardName.Focus();
                return;
            }

            BoardName = txtBoardName.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

