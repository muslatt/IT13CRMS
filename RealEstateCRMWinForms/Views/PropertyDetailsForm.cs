using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
using System.Globalization;
using RealEstateCRMWinForms.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace RealEstateCRMWinForms.Views
{
    public partial class PropertyDetailsForm : Form
    {
        private Property _property;
        private PictureBox pictureBoxMain = null!;
        private PictureBox pictureBoxProof = null!;
        private FlowLayoutPanel proofFilesPanel = null!;
        private Label lblTitle = null!;
        private Label lblAddress = null!;
        private Label lblPrice = null!;
        private Panel statusPanel = null!;
        private Label lblStatus = null!;
        private Panel detailsPanel = null!;
        private Panel descriptionPanel = null!;
        private Label lblRejectionReason = null!;
        private Panel proofPanel = null!;
        private Panel logsPanel = null!;
        private Label lblDescription = null!;
        private Button btnClose = null!;
        private Button btnEdit = null!;
        private Button btnResubmit = null!;
        private Button btnViewProof = null!;
        private Button btnDownloadProof = null!;
        private Button btnRequestInfo = null!; // Inquire button for clients
        private Panel headerButtonPanel = null!; // header button container (for layout)
        private bool _propertyWasModified = false;
        private bool _isClientBrowseMode = false;

        // Logs UI
        private DataGridView dgvLogs = null!;
        private CheckBox chkShowRejectedOnly = null!;
        private BindingSource logsBindingSource = new BindingSource();
        private List<ViewModels.LogEntry> _allLogs = new();
        private List<ViewModels.LogEntry> _filteredLogs = new();

        public event EventHandler<PropertyEventArgs>? PropertyUpdated;

        public PropertyDetailsForm(Property property)
        {
            _property = property;
            _isClientBrowseMode = false;
            InitializeComponent();
            LoadPropertyDetails();
        }

        public PropertyDetailsForm(Property property, bool isClientBrowseMode)
        {
            _property = property;
            _isClientBrowseMode = isClientBrowseMode;
            InitializeComponent();
            LoadPropertyDetails();
        }

        private bool ShouldHideActivityLog()
        {
            var currentUser = UserSession.Instance.CurrentUser;
            return _isClientBrowseMode || currentUser?.Role == UserRole.Client;
        }

        private void InitializeComponent()
        {
            Text = "Property Details";
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(248, 249, 250);

            // Compact form size to ensure buttons are visible
            ClientSize = new Size(1100, 900);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Create main container with padding
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(248, 249, 250),
                AutoScroll = true
            };

            // Header section with image and basic info - more compact
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1060, 340),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Add subtle shadow effect
            headerPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, headerPanel.Width, headerPanel.Height);
                using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }
                e.Graphics.FillRectangle(Brushes.White, rect);
            };

            // Property image - more compact
            pictureBoxMain = new PictureBox
            {
                Location = new Point(20, 20),
                Size = new Size(360, 250),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(233, 236, 239)
            };

            // Property info section
            var infoPanel = new Panel
            {
                Location = new Point(390, 20),
                Size = new Size(650, 290),
                BackColor = Color.Transparent
            };

            // Title with modern typography
            lblTitle = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(650, 40),
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Property Title",
                AutoSize = false
            };

            // Address with icon
            lblAddress = new Label
            {
                Location = new Point(0, 48),
                Size = new Size(650, 26),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(108, 117, 125),
                Text = string.Empty,
                AutoSize = false
            };

            // Price with emphasis
            lblPrice = new Label
            {
                Location = new Point(0, 86),
                Size = new Size(380, 44),
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                Text = "₱ 0",
                AutoSize = true
            };

            // Status badge
            statusPanel = new Panel
            {
                Location = new Point(0, 140),
                Size = new Size(140, 36),
                BackColor = Color.FromArgb(0, 123, 255)
            };
            statusPanel.Paint += (s, e) =>
            {
                var rect = statusPanel.ClientRectangle;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 14;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(rect.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, rect.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    statusPanel.Region = new Region(path);
                }
            };

            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "For Sale"
            };
            statusPanel.Controls.Add(lblStatus);

            // Action buttons in header for better visibility
            headerButtonPanel = new Panel
            {
                Location = new Point(0, 186),
                Size = new Size(650, 120), // Increased height to accommodate rejection reason
                BackColor = Color.Transparent
            };

            // Primary action button (changes based on mode)
            btnRequestInfo = new Button
            {
                Text = "Inquire Now",
                Size = new Size(200, 45),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = ColorTranslator.FromHtml("#2563eb"), // Blue for inquire
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, 0),
                Visible = false
            };
            btnRequestInfo.FlatAppearance.BorderSize = 0;
            btnRequestInfo.Click += BtnRequestInfo_Click;

            btnEdit = new Button
            {
                Text = "Edit Property",
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, 0),
                Visible = false
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;

            btnResubmit = new Button
            {
                Text = "Resubmit",
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(160, 0),
                Visible = false
            };
            btnResubmit.FlatAppearance.BorderSize = 0;
            btnResubmit.Click += BtnResubmit_Click;

            // Rejection reason label - shown below buttons for rejected properties
            lblRejectionReason = new Label
            {
                Location = new Point(0, 50),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69), // Red color
                Text = string.Empty,
                AutoSize = true,
                MaximumSize = new Size(650, 0), // Allow height to grow, constrain width
                Visible = false,
                UseMnemonic = false // Prevents & from being interpreted as mnemonic
            };

            headerButtonPanel.Controls.AddRange(new Control[] { btnRequestInfo, btnEdit, btnResubmit, lblRejectionReason });
            infoPanel.Controls.AddRange(new Control[] { lblTitle, lblAddress, lblPrice, statusPanel, headerButtonPanel });

            // Property details card - more compact
            detailsPanel = new Panel
            {
                Location = new Point(0, 330),
                Size = new Size(1060, 200),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            detailsPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, detailsPanel.Width, detailsPanel.Height);
                using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }
                e.Graphics.FillRectangle(Brushes.White, rect);
            };

            var detailsHeader = new Label
            {
                Location = new Point(20, 12),
                Size = new Size(200, 24),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Property Details",
                AutoSize = true
            };

            CreateDetailsGrid();

            // Description card - more compact
            descriptionPanel = new Panel
            {
                Location = new Point(0, 540),
                Size = new Size(1060, 160),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Remove drop shadow to avoid gray bars at the edges of the Description section
            descriptionPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, descriptionPanel.Width, descriptionPanel.Height);
                using (var brush = new SolidBrush(descriptionPanel.BackColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };

            var descriptionHeader = new Label
            {
                Location = new Point(20, 8),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Description",
                AutoSize = true
            };

            lblDescription = new Label
            {
                Location = new Point(20, 32),
                Size = new Size(1020, 86),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(73, 80, 87),
                Text = "No description available.",
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Activity Log card - shows journey logs (including rejections)
            logsPanel = new Panel
            {
                Location = new Point(0, 720),
                Size = new Size(1060, 220),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            logsPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, logsPanel.Width, logsPanel.Height);
                using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }
                e.Graphics.FillRectangle(Brushes.White, rect);
            };

            // Header row (label + filter) to keep filter visible
            var logsHeaderRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Margin = new Padding(0),
                Padding = new Padding(20, 10, 20, 8)
            };

            var logsHeader = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Activity Log",
                Margin = new Padding(0, 0, 16, 0)
            };

            chkShowRejectedOnly = new CheckBox
            {
                Text = "Show rejected only",
                AutoSize = true,
                ForeColor = Color.FromArgb(73, 80, 87),
                Margin = new Padding(0, 6, 0, 0)
            };
            chkShowRejectedOnly.CheckedChanged += (s, e) => ApplyLogsFilter();
            logsHeaderRow.Controls.Add(logsHeader);
            logsHeaderRow.Controls.Add(chkShowRejectedOnly);

            dgvLogs = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            };
            dgvLogs.DataSource = logsBindingSource;
            dgvLogs.RowPrePaint += DgvLogs_RowPrePaint;
            dgvLogs.CellDoubleClick += DgvLogs_CellDoubleClick;
            dgvLogs.KeyDown += DgvLogs_KeyDown;

            // Columns: Timestamp, User, Action, Details
            var tsCol = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ViewModels.LogEntry.Timestamp),
                HeaderText = "Timestamp",
                Width = 150,
                Name = "Timestamp"
            };
            tsCol.DefaultCellStyle.Format = "g";
            var userCol = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ViewModels.LogEntry.UserFullName),
                HeaderText = "User",
                Width = 160,
                Name = "User"
            };
            var actionCol = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ViewModels.LogEntry.Action),
                HeaderText = "Action",
                Width = 220,
                Name = "Action"
            };
            var detailsCol = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ViewModels.LogEntry.Details),
                HeaderText = "Details",
                Width = 440,
                Name = "Details"
            };
            dgvLogs.Columns.AddRange(new DataGridViewColumn[] { tsCol, userCol, actionCol, detailsCol });

            var logsContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 4, 20, 10)
            };
            logsContentPanel.Controls.Add(dgvLogs);

            logsPanel.Controls.AddRange(new Control[] { logsContentPanel, logsHeaderRow });

            bool hideActivityLog = ShouldHideActivityLog();
            logsPanel.Visible = !hideActivityLog;

            // Proof of ownership card - more compact (moved below logs)
            proofPanel = new Panel
            {
                Location = new Point(0, 950),
                Size = new Size(1060, 200),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            proofPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, proofPanel.Width, proofPanel.Height);
                using (var brush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }
                e.Graphics.FillRectangle(Brushes.White, rect);
            };

            var proofHeader = new Label
            {
                Location = new Point(20, 8),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Text = "Proof of Ownership",
                AutoSize = true
            };

            pictureBoxProof = new PictureBox
            {
                Location = new Point(20, 40),
                Size = new Size(220, 120),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(233, 236, 239),
                BorderStyle = BorderStyle.FixedSingle
            };

            proofFilesPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 40),
                Size = new Size(1020, 120),
                BackColor = Color.FromArgb(248, 249, 250),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Bottom button panel - always visible
            var bottomButtonPanel = new Panel
            {
                Location = new Point(0, 1160),
                Size = new Size(1060, 60),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            btnClose = new Button
            {
                Text = "Close",
                Size = new Size(100, 36),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(850, 7),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += BtnClose_Click;

            btnViewProof = new Button
            {
                Text = "View Proof",
                Size = new Size(110, 36),
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(730, 7),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            btnViewProof.FlatAppearance.BorderSize = 0;

            btnDownloadProof = new Button
            {
                Text = "Download",
                Size = new Size(110, 36),
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(23, 162, 184),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(610, 7),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            btnDownloadProof.FlatAppearance.BorderSize = 0;

            bottomButtonPanel.Controls.AddRange(new Control[] { btnClose, btnViewProof, btnDownloadProof });

            // Add all controls
            headerPanel.Controls.AddRange(new Control[] { pictureBoxMain, infoPanel });
            detailsPanel.Controls.Add(detailsHeader);
            descriptionPanel.Controls.AddRange(new Control[] { descriptionHeader, lblDescription });
            proofPanel.Controls.AddRange(new Control[] { proofHeader, pictureBoxProof, proofFilesPanel });

            var controlsToAdd = new List<Control>
            {
                headerPanel,
                detailsPanel,
                descriptionPanel
            };

            if (!hideActivityLog)
            {
                controlsToAdd.Add(logsPanel);
            }

            controlsToAdd.Add(proofPanel);
            controlsToAdd.Add(bottomButtonPanel);

            mainContainer.Controls.AddRange(controlsToAdd.ToArray());

            // Reflow vertical positions to avoid overlap if details panel grew
            int spacer = 10;
            descriptionPanel.Top = detailsPanel.Bottom + spacer;
            var nextTop = descriptionPanel.Bottom + 20;

            if (!hideActivityLog)
            {
                logsPanel.Top = nextTop;
                nextTop = logsPanel.Bottom + 20;
            }

            proofPanel.Top = nextTop;
            nextTop = proofPanel.Bottom + 10;
            bottomButtonPanel.Top = nextTop;

            Controls.Add(mainContainer);
            CancelButton = btnClose;
        }

        private void CreateDetailsGrid()
        {
            var detailsContainer = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(1020, 180),
                BackColor = Color.Transparent
            };

            // Removed emoji/icon characters that were previously causing garbled text ("Ã" showing up)
            // Keep clean labels and values only to ensure no chopped text for client view
            var details = new List<(string Label, string Value)>
            {
                ("Bedrooms", "0"),
                ("Bathrooms", "0"),
                ("Lot Area", "0 sqm"),
                ("Floor Area", "0 sqm"),
                ("Listed", "-"),
                ("Type", "-"),
                ("Transaction", "-")
            };

            // Do not pad with an empty placeholder; this removes the blank tile to the right of "Transaction"

            // Card layout metrics with spacing between columns and rows
            int cardWidth = 240;
            int cardHeight = 80; // increased for better vertical spacing
            int hSpacing = 16; // space between columns
            int vSpacing = 16; // space between rows

            for (int i = 0; i < details.Count; i++)
            {
                var detail = details[i];
                int col = i % 4;
                int row = i / 4;
                var position = new Point(col * (cardWidth + hSpacing), row * (cardHeight + vSpacing));

                // Skip rendering if label is empty (no placeholder card)
                if (!string.IsNullOrEmpty(detail.Label))
                {
                    var detailCard = new Panel
                    {
                        Location = position,
                        Size = new Size(cardWidth, cardHeight),
                        BackColor = Color.FromArgb(248, 249, 250),
                        Tag = detail.Label,
                        Padding = new Padding(8, 6, 8, 6)
                    };

                    detailCard.Paint += (s, e) =>
                    {
                        var rect = detailCard.ClientRectangle;
                        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            int radius = 6;
                            path.AddArc(0, 0, radius, radius, 180, 90);
                            path.AddArc(rect.Width - radius, 0, radius, radius, 270, 90);
                            path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
                            path.AddArc(0, rect.Height - radius, radius, radius, 90, 90);
                            path.CloseAllFigures();
                            detailCard.Region = new Region(path);
                        }
                    };

                    var titleLabel = new SmoothLabel
                    {
                        Text = detail.Label,
                        Location = new Point(12, 8),
                        Size = new Size(150, 18),
                        Font = new Font("Segoe UI", 8.5F),
                        ForeColor = Color.FromArgb(108, 117, 125),
                        BackColor = Color.Transparent
                    };

                    var valueLabel = new SmoothLabel
                    {
                        Text = detail.Value,
                        Location = new Point(12, 30),
                        Size = new Size(216, 40), // taller to avoid glyph clipping
                        Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(33, 37, 41),
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Tag = detail.Label + "_Value",
                        AutoEllipsis = false
                    };
                    // Keep full text visible and avoid chopping
                    valueLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    valueLabel.AutoSize = true; // let height adjust to font metrics
                    valueLabel.MinimumSize = new Size(216, 40); // ensure enough baseline clearance
                    valueLabel.MaximumSize = new Size(216, 0); // restrict width; allow height to grow if needed

                    detailCard.Controls.AddRange(new Control[] { titleLabel, valueLabel });
                    detailsContainer.Controls.Add(detailCard);
                }
            }

            detailsPanel.Controls.Add(detailsContainer);

            // Ensure detailsPanel is tall enough to contain the grid + padding
            int rows = (details.Count + 3) / 4; // 4 cards per row
            int gridHeight = (cardHeight * rows) + (vSpacing * (rows - 1));
            int requiredPanelHeight = 60 + gridHeight + 20; // top offset + grid + bottom padding
            if (detailsPanel.Height < requiredPanelHeight)
            {
                detailsPanel.Height = requiredPanelHeight;
            }
        }

        private void LoadPropertyDetails()
        {
            if (_property == null) return;

            // Basic info
            lblTitle.Text = _property.Title;
            lblAddress.Text = $"Address: {_property.Address}";
            lblPrice.Text = _property.Price.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-PH"));

            // Status badge
            var typeText = string.IsNullOrWhiteSpace(_property.PropertyType) ? "N/A" : _property.PropertyType;
            lblStatus.Text = typeText;

            statusPanel.BackColor = typeText switch
            {
                "Residential" => Color.FromArgb(40, 167, 69),
                "Commercial" => Color.FromArgb(0, 123, 255),
                "Raw Land" => Color.FromArgb(108, 117, 125),
                _ => Color.FromArgb(108, 117, 125),
            };

            // Ensure header layout uses measured sizes (avoid chopped title)
            EnsureHeaderDoesNotClip();

            // Update detail values
            UpdateDetailValue("Bedrooms_Value", _property.Bedrooms.ToString());
            UpdateDetailValue("Bathrooms_Value", _property.Bathrooms.ToString());
            UpdateDetailValue("Lot Area_Value", $"{_property.LotAreaSqm:N0} sqm");
            UpdateDetailValue("Floor Area_Value", $"{_property.FloorAreaSqm:N0} sqm");
            UpdateDetailValue("Listed_Value", _property.CreatedAt.ToString("MMM dd, yyyy"));
            UpdateDetailValue("Type_Value", GetPropertyValue("PropertyType") ?? "Not specified");
            UpdateDetailValue("Transaction_Value", GetPropertyValue("TransactionType") ?? "Not specified");

            lblDescription.Text = GetPropertyValue("Description") ?? GetPropertyValue("Notes") ?? "No description available.";

            // Check if property is rejected and show rejection reason
            bool isRejected = !_property.IsApproved && !string.IsNullOrEmpty(_property.RejectionReason);

            if (isRejected)
            {
                lblRejectionReason.Text = $"⚠ Rejection Reason: {_property.RejectionReason}";
                lblRejectionReason.Visible = true;
            }
            else
            {
                lblRejectionReason.Visible = false;
            }

            LoadPropertyImage();
            LoadProofImage();

            if (!ShouldHideActivityLog())
            {
                LoadPropertyLogs();
            }
            else
            {
                logsPanel.Visible = false;
            }

            // Configure button visibility based on mode
            if (_isClientBrowseMode)
            {
                // Client browse mode - show prominent Inquire button
                btnRequestInfo.Visible = true;
                btnRequestInfo.BringToFront();
                btnEdit.Visible = false;
                btnResubmit.Visible = false;
                AcceptButton = btnRequestInfo;

                System.Diagnostics.Debug.WriteLine("PropertyDetailsForm: Client Browse Mode - Inquire button should be visible");
            }
            else
            {
                var currentUser = UserSession.Instance.CurrentUser;
                bool isBroker = currentUser?.Role == UserRole.Broker;

                btnRequestInfo.Visible = false;
                btnEdit.Visible = !isBroker;

                bool propertyIsRejected = !_property.IsApproved && !string.IsNullOrEmpty(_property.RejectionReason);
                btnResubmit.Visible = !isBroker && propertyIsRejected;

                AcceptButton = btnEdit.Visible ? btnEdit : btnClose;

                System.Diagnostics.Debug.WriteLine($"PropertyDetailsForm: Owner Mode - Edit buttons visible={btnEdit.Visible}, Resubmit={btnResubmit.Visible} (IsApproved={_property.IsApproved})");
            }
        }

        private void UpdateDetailValue(string tag, string value)
        {
            foreach (Control panel in detailsPanel.Controls)
            {
                if (panel is Panel detailsContainer && detailsContainer.Location.X == 20)
                {
                    foreach (Control card in detailsContainer.Controls)
                    {
                        if (card is Panel)
                        {
                            foreach (Control control in card.Controls)
                            {
                                if (control is Label label && label.Tag?.ToString() == tag)
                                {
                                    label.Text = value;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Reflow header elements so the title can wrap and never be cut
        private void EnsureHeaderDoesNotClip()
        {
            if (lblTitle == null || lblAddress == null || lblPrice == null || statusPanel == null || headerButtonPanel == null)
                return;

            // Allow the title to wrap within the info panel width
            int infoWidth = 650; // matches infoPanel.Width set in InitializeComponent
            lblTitle.MaximumSize = new Size(infoWidth, 0); // constrain width, allow height to grow
            lblTitle.AutoSize = true; // grow vertically to fit
            lblTitle.AutoEllipsis = false;

            // Address should also avoid clipping (single or multi-line)
            lblAddress.MaximumSize = new Size(infoWidth, 0);
            lblAddress.AutoSize = true;

            // Price can stay AutoSize=true
            lblPrice.AutoSize = true;

            // Reposition controls based on their measured heights
            int y = 0;
            lblTitle.Location = new Point(0, y);
            y = lblTitle.Bottom + 8;

            lblAddress.Location = new Point(0, y);
            y = lblAddress.Bottom + 12;

            lblPrice.Location = new Point(0, y);
            y = lblPrice.Bottom + 10;

            statusPanel.Location = new Point(0, y);
            y = statusPanel.Bottom + 12;

            // Place buttons panel beneath status badge
            headerButtonPanel.Location = new Point(0, y);
        }

        private void AdjustPanelLayout()
        {
            // Dynamically adjust panel positions based on visibility
            bool hideActivityLog = ShouldHideActivityLog();

            // Start positioning from after the description panel
            var nextTop = descriptionPanel.Bottom + 20;

            // Position activity log if visible
            if (!hideActivityLog && logsPanel.Visible)
            {
                logsPanel.Top = nextTop;
                nextTop = logsPanel.Bottom + 20;
            }
            else
            {
                // Move it out of view when not visible
                logsPanel.Top = -1000;
            }

            // Position proof panel
            proofPanel.Top = nextTop;
            nextTop = proofPanel.Bottom + 10;

            // Position bottom button panel - search in mainContainer's controls
            Panel? mainContainer = Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Fill);
            if (mainContainer != null)
            {
                var bottomButtonPanel = mainContainer.Controls.OfType<Panel>()
                    .FirstOrDefault(p => p.Controls.Contains(btnClose));
                if (bottomButtonPanel != null)
                {
                    bottomButtonPanel.Top = nextTop;
                }

                // Force refresh of the container
                mainContainer.PerformLayout();
                mainContainer.Refresh();
            }
        }

        private string? GetPropertyValue(string propertyName)
        {
            try
            {
                var property = _property.GetType().GetProperty(propertyName);
                return property?.GetValue(_property)?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private void LoadPropertyImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_property?.ImagePath))
                {
                    string imagePath = GetPropertyImagePath(_property.ImagePath);
                    if (File.Exists(imagePath))
                    {
                        using (var img = Image.FromFile(imagePath))
                        {
                            pictureBoxMain.Image?.Dispose();
                            pictureBoxMain.Image = new Bitmap(img);
                        }
                        return;
                    }
                }

                SetDefaultImage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading property image: {ex.Message}");
                SetDefaultImage();
            }
        }

        private void SetDefaultImage()
        {
            pictureBoxMain.Image?.Dispose();

            var defaultBitmap = new Bitmap(360, 250);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.FromArgb(233, 236, 239));
                using (var brush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
                {
                    var text = "No Image Available";
                    var textSize = g.MeasureString(text, font);
                    var x = (defaultBitmap.Width - textSize.Width) / 2;
                    var y = (defaultBitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }

            pictureBoxMain.Image = defaultBitmap;
        }

        private void LoadProofImage()
        {
            try
            {
                proofFilesPanel.Controls.Clear();

                if (_property.ProofFiles != null && _property.ProofFiles.Any())
                {
                    foreach (var proofFile in _property.ProofFiles.OrderByDescending(f => f.UploadDate))
                    {
                        var fileCard = CreateProofFileCard(proofFile);
                        proofFilesPanel.Controls.Add(fileCard);
                    }

                    pictureBoxProof.Visible = false;
                    proofFilesPanel.Visible = true;
                }
                else
                {
                    SetDefaultProofImage();
                    pictureBoxProof.Visible = true;
                    proofFilesPanel.Visible = false;
                }

                btnViewProof.Visible = false;
                btnDownloadProof.Visible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading proof files: {ex.Message}");
                SetDefaultProofImage();
                pictureBoxProof.Visible = true;
                proofFilesPanel.Visible = false;
                btnViewProof.Visible = false;
                btnDownloadProof.Visible = false;
            }
        }

        private Control CreateProofFileCard(PropertyProofFile proofFile)
        {
            var cardPanel = new Panel
            {
                Size = new Size(180, 96),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(8)
            };

            var iconLabel = new Label
            {
                Text = "FILE",
                Font = new Font("Segoe UI", 18F),
                Location = new Point(80, 6),
                Size = new Size(22, 22),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var fileName = proofFile.FileName.Length > 18
                ? proofFile.FileName.Substring(0, 16) + "..."
                : proofFile.FileName;
            var nameLabel = new Label
            {
                Text = fileName,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Location = new Point(6, 30),
                Size = new Size(168, 16),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            var sizeLabel = new Label
            {
                Text = FormatFileSize(proofFile.FileSize),
                Font = new Font("Segoe UI", 7.5F),
                Location = new Point(6, 48),
                Size = new Size(168, 14),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            var btnView = new Button
            {
                Text = "View",
                Size = new Size(78, 24),
                Font = new Font("Segoe UI", 8F),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(12, 66)
            };
            btnView.FlatAppearance.BorderSize = 0;
            btnView.Tag = proofFile;
            btnView.Click += (s, e) => ViewProofFile(proofFile);

            var btnDownload = new Button
            {
                Text = "DL",
                Size = new Size(42, 24),
                Font = new Font("Segoe UI", 8F),
                BackColor = Color.FromArgb(23, 162, 184),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(98, 66)
            };
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.Tag = proofFile;
            btnDownload.Click += (s, e) => DownloadProofFile(proofFile);

            cardPanel.Controls.AddRange(new Control[] { iconLabel, nameLabel, sizeLabel, btnView, btnDownload });
            return cardPanel;
        }

        private string GetPropertyImagePath(string imagePath)
        {
            string imagesDir = Path.Combine(Application.StartupPath, "PropertyImages");
            return Path.Combine(imagesDir, imagePath);
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            var editForm = new EditPropertyForm(_property);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                RefreshPropertyFromDatabase();
                LoadPropertyDetails();
                _propertyWasModified = true;
                PropertyUpdated?.Invoke(this, new PropertyEventArgs(_property));
            }
        }

        private void BtnResubmit_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            bool isRejected = !_property.IsApproved && !string.IsNullOrEmpty(_property.RejectionReason);

            string message;
            if (isRejected)
            {
                message = $"You are about to edit and resubmit this rejected property:\n\n" +
                         $"Property: {_property.Title}\n" +
                         $"Rejection Reason: {_property.RejectionReason}\n\n" +
                         $"After making changes, the property will be resubmitted for broker approval.";
            }
            else if (_property.IsApproved)
            {
                message = $"You are about to resubmit this approved property:\n\n" +
                         $"Property: {_property.Title}\n\n" +
                         $"The property will be marked as pending and require broker approval again.";
            }
            else
            {
                message = $"You are about to resubmit this property:\n\n" +
                         $"Property: {_property.Title}\n\n" +
                         $"After making changes, the property will be resubmitted for broker approval.";
            }

            var confirmResult = MessageBox.Show(
                message,
                "Resubmit Property",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (confirmResult != DialogResult.OK)
                return;

            var editForm = new EditPropertyForm(_property);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var db = Data.DbContextHelper.CreateDbContext();
                    var propertyToUpdate = db.Properties.FirstOrDefault(p => p.Id == _property.Id);

                    if (propertyToUpdate != null)
                    {
                        propertyToUpdate.IsResubmitted = true;
                        propertyToUpdate.IsApproved = false;
                        propertyToUpdate.RejectionReason = null;

                        if (!propertyToUpdate.SubmittedByUserId.HasValue)
                        {
                            var currentUser = Services.UserSession.Instance.CurrentUser;
                            if (currentUser != null)
                            {
                                propertyToUpdate.SubmittedByUserId = currentUser.Id;
                            }
                        }

                        db.SaveChanges();

                        // Log client resubmission scoped to this property
                        try
                        {
                            Services.LoggingService.LogAction(
                                "Client Resubmitted Property",
                                $"Resubmitted by {RealEstateCRMWinForms.Services.UserSession.Instance.CurrentUser?.FullName ?? "Client"}",
                                propertyId: propertyToUpdate.Id);
                        }
                        catch { /* logging safety */ }

                        MessageBox.Show(
                            "Property has been successfully resubmitted for broker approval!",
                            "Resubmitted Successfully",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        RefreshPropertyFromDatabase();
                        LoadPropertyDetails();
                        _propertyWasModified = true;
                        PropertyUpdated?.Invoke(this, new PropertyEventArgs(_property));

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error resubmitting property: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void SetDefaultProofImage()
        {
            var defaultBitmap = new Bitmap(200, 80);
            using (var g = Graphics.FromImage(defaultBitmap))
            {
                g.Clear(Color.FromArgb(233, 236, 239));
                using (var brush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                {
                    string text = "No Proof Available";
                    var textSize = g.MeasureString(text, font);
                    var x = (defaultBitmap.Width - textSize.Width) / 2;
                    var y = (defaultBitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            pictureBoxProof.Image = defaultBitmap;
        }

        private void ViewProofFile(PropertyProofFile proofFile)
        {
            try
            {
                if (File.Exists(proofFile.FilePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = proofFile.FilePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show($"File not found: {proofFile.FileName}", "File Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DownloadProofFile(PropertyProofFile proofFile)
        {
            try
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.FileName = proofFile.FileName;
                    saveDialog.Filter = "All files (*.*)|*.*";
                    saveDialog.Title = "Save Proof File";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(proofFile.FilePath))
                        {
                            File.Copy(proofFile.FilePath, saveDialog.FileName, true);
                            MessageBox.Show("File downloaded successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Source file not found: {proofFile.FileName}", "File Not Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshPropertyFromDatabase()
        {
            if (_property == null) return;

            try
            {
                var viewModel = new ViewModels.PropertyViewModel();
                var refreshedProperty = viewModel.GetPropertyById(_property.Id);

                if (refreshedProperty != null)
                {
                    _property = refreshedProperty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing property from database: {ex.Message}");
            }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            DialogResult = _propertyWasModified ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        private void BtnRequestInfo_Click(object? sender, EventArgs e)
        {
            if (_property == null) return;

            // Show a dialog to get the inquiry message from the client
            var requestForm = new RequestInfoForm(_property);
            if (requestForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(
                    "Your inquiry has been sent successfully! The broker will contact you soon.",
                    "Inquiry Sent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void PictureBoxProof_Click(object? sender, EventArgs e)
        {
            // This method is no longer used
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pictureBoxMain?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void LoadPropertyLogs()
        {
            try
            {
                if (_property == null || _property.Id <= 0)
                {
                    logsBindingSource.DataSource = null;
                    return;
                }

                var vm = new ViewModels.LogViewModel();
                var logs = vm.GetLogsForProperty(_property.Id) ?? new List<ViewModels.LogEntry>();

                _allLogs = logs.OrderByDescending(l => l.Timestamp).ToList();
                ApplyLogsFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load property logs: {ex.Message}");
            }
        }

        private void ApplyLogsFilter()
        {
            IEnumerable<ViewModels.LogEntry> source = _allLogs ?? new List<ViewModels.LogEntry>();
            if (chkShowRejectedOnly != null && chkShowRejectedOnly.Checked)
            {
                source = source.Where(IsRejectedLog);
            }
            _filteredLogs = source.ToList();
            logsBindingSource.DataSource = null;
            logsBindingSource.DataSource = _filteredLogs;
            logsBindingSource.ResetBindings(false);
        }

        private void DgvLogs_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvLogs.Rows[e.RowIndex];
            if (row?.DataBoundItem is ViewModels.LogEntry log)
            {
                row.DefaultCellStyle.ForeColor = IsRejectedLog(log)
                    ? Color.FromArgb(220, 53, 69) // red for rejected
                    : SystemColors.ControlText;
            }
        }

        private static bool IsRejectedLog(ViewModels.LogEntry log)
        {
            if (log == null) return false;
            if (!string.IsNullOrEmpty(log.Action) && log.Action.IndexOf("Rejected", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (!string.IsNullOrEmpty(log.Details) && log.Details.IndexOf("Rejected", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            return false;
        }

        private void DgvLogs_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            ShowSelectedLogDetails();
        }

        private void DgvLogs_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ShowSelectedLogDetails();
            }
        }

        private void ShowSelectedLogDetails()
        {
            if (logsBindingSource?.Current is not ViewModels.LogEntry log)
                return;

            using var dlg = new Form
            {
                Text = "Log Details",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(560, 360),
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                BackColor = Color.White
            };

            var lblTs = new Label { Text = "Timestamp:", Location = new Point(16, 16), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var valTs = new TextBox { ReadOnly = true, BorderStyle = BorderStyle.None, Location = new Point(110, 16), Width = 420, Text = log.Timestamp.ToString("F") };

            var lblUser = new Label { Text = "User:", Location = new Point(16, 44), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var valUser = new TextBox { ReadOnly = true, BorderStyle = BorderStyle.None, Location = new Point(110, 44), Width = 420, Text = log.UserFullName };

            var lblAction = new Label { Text = "Action:", Location = new Point(16, 72), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var valAction = new TextBox { ReadOnly = true, BorderStyle = BorderStyle.None, Location = new Point(110, 72), Width = 420, Text = log.Action };

            var lblDetails = new Label { Text = "Details:", Location = new Point(16, 100), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var valDetails = new TextBox { ReadOnly = true, Multiline = true, ScrollBars = ScrollBars.Vertical, Location = new Point(110, 100), Size = new Size(420, 160), Text = log.Details ?? string.Empty };

            var btnClose = new Button { Text = "Close", Size = new Size(90, 30), Location = new Point(440, 280), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => dlg.Close();

            dlg.Controls.AddRange(new Control[] { lblTs, valTs, lblUser, valUser, lblAction, valAction, lblDetails, valDetails, btnClose });
            dlg.AcceptButton = btnClose;
            dlg.CancelButton = btnClose;
            dlg.ShowDialog(this);
        }
    }
}











