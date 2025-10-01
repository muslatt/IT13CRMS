using RealEstateCRMWinForms.ViewModels;
using RealEstateCRMWinForms.Services;
using RealEstateCRMWinForms.Models;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace RealEstateCRMWinForms.Views
{
    public partial class MainView : UserControl
    {
        private readonly UserViewModel _viewModel;
        public event EventHandler? LogoutRequested;
        public event EventHandler? RegisterAgentRequested;

        private UserControl? _currentContentView;

        // Modern sidebar helpers
        private FlowLayoutPanel? flSidebarNav;
        private Button? btnCollapseSidebar;
        private bool _sidebarCollapsed = false;

        // bottom panel reference (made a field so we can update on resize)
        private Panel? panelSidebarBottom;

        // configurable appearance (changed to support "much larger" nav)
        private float _sidebarFontPt = 18f;    // larger font in points
        private int _sidebarIconSize = 28;     // larger icon size in px
        private int _sidebarButtonHeight = 48; // taller nav buttons

        // Color palette (brand + complementary accents)
        private readonly Color _brandBlue = Color.FromArgb(0, 102, 204);
        private readonly Color _accentWarm = Color.FromArgb(255, 159, 67);       // complementary warm accent
        private readonly Color _lightAccent = Color.FromArgb(255, 244, 230);     // subtle warm background for active state
        private readonly Color _hoverAccent = Color.FromArgb(255, 247, 236);     // hover background
        private bool _isBroker = false;
        private bool _isClient = false;
        private MailjetInboundListener? _mailjetInbound;
        private Button? btnManageAgents; // runtime-created nav button
        private Button? btnLogs; // runtime-created nav button
        private Button? btnPropertyRequests; // runtime-created nav button for brokers
        private Button? btnInquiries; // runtime-created nav button for brokers
        private Button? btnClientAddProperty; // client-specific nav
        private Button? btnClientBrowseProperty; // client-specific nav
        private Button? btnClientMyDeals; // client-specific nav for deals

        public MainView()
        {
            InitializeComponent();
            _viewModel = new UserViewModel();
            dataGridView1.DataSource = _viewModel.Users;

            // Hide Settings and Help from the sidebar
            if (btnSettings != null) btnSettings.Visible = false;
            if (btnHelp != null) btnHelp.Visible = false;

            // increase sizes for a much larger nav
            _sidebarFontPt = 18f;      // "much larger" — adjust as desired (16-22)
            _sidebarIconSize = 28;     // increase glyph/icon size
            _sidebarButtonHeight = 48; // taller buttons for better visual weight

            // Setup uniform icons for sidebar buttons (uses _sidebarIconSize)
            SetupSidebarIcons();

            // apply larger font to sidebar buttons (uses _sidebarFontPt)
            ConfigureSidebarFont();

            // Apply a modern sidebar layout at runtime (safe - doesn't edit Designer)
            ApplyModernSidebarLayout();

            // listen for sidebar size changes so runtime layout stays consistent
            panelSidebar.SizeChanged -= PanelSidebar_SizeChanged;
            panelSidebar.SizeChanged += PanelSidebar_SizeChanged;

            // Load the sidebar logo from embedded resource
            LoadSidebarLogo();

            // default section
            SwitchSection("Dashboard");

            // default placeholder user (blank avatar)
            SetCurrentUser("", "");

            // Try to start Mailjet inbound listener (for replies)
            try
            {
                _mailjetInbound = new MailjetInboundListener(new EmailLogService(), new ContactViewModel());
                if (_mailjetInbound.CanStart)
                {
                    _mailjetInbound.Start();
                }
            }
            catch { }
        }

        private void PanelSidebar_SizeChanged(object? sender, System.EventArgs e)
        {
            // Adjust flow and bottom panel widths and buttons on resize
            UpdateSidebarLayout();
        }

        private void ConfigureSidebarFont()
        {
            const string fontFamily = "Segoe UI";
            var sideButtons = new[] { btnDashboard, btnProperties, btnPendingAssignments, btnLeads, btnContacts, btnDeals, btnSettings, btnHelp };

            foreach (var b in sideButtons)
            {
                if (b == null) continue;
                // Use points so DPI scaling remains consistent
                b.Font = new Font(fontFamily, _sidebarFontPt, FontStyle.Regular, GraphicsUnit.Point);
                // give a neutral foreground color to start
                b.ForeColor = Color.FromArgb(33, 37, 41);
                b.BackColor = Color.Transparent;
            }

            // make Dashboard bold
            if (btnDashboard != null)
            {
                btnDashboard.Font = new Font("Segoe UI", _sidebarFontPt, FontStyle.Bold, GraphicsUnit.Point);
            }
        }

        private void ApplyModernSidebarLayout()
        {
            // Remove any existing runtime flow if re-applying
            if (flSidebarNav != null)
            {
                panelSidebar.Controls.Remove(flSidebarNav);
                flSidebarNav.Controls.Clear(); // Clear controls to prevent disposing the buttons
                flSidebarNav.Dispose();
                flSidebarNav = null;
            }

            // Create a FlowLayoutPanel to manage nav ordering and spacing automatically
            flSidebarNav = new FlowLayoutPanel
            {
                Name = "flSidebarNav",
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Location = new Point(12, pbLogo?.Bottom + 12 ?? 100),
                Size = new Size(panelSidebar.Width - 24,  // account for padding
                                Math.Max(300, (_sidebarButtonHeight + 8) * 6)), // fit at least 6 items
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Ensure Tag exists for settings/help so we can restore text after collapse
            if (btnSettings != null && btnSettings.Tag == null) btnSettings.Tag = btnSettings.Text;
            if (btnHelp != null && btnHelp.Tag == null) btnHelp.Tag = btnHelp.Text;

            // Ensure consistent button sizing and margins, move existing nav buttons into the flow panel
            EnsureManageAgentsButton();
            EnsureLogsButton();
            EnsurePropertyRequestsButton();
            EnsureInquiriesButton();
            EnsureClientButtons();
            List<Button?> navButtons = new List<Button?>();
            if (_isClient)
            {
                navButtons.Add(btnClientBrowseProperty);
                navButtons.Add(btnClientAddProperty);
                navButtons.Add(btnClientMyDeals);
            }
            else
            {
                navButtons.AddRange(new[] { btnDashboard, btnProperties, btnLeads, btnContacts, btnDeals, btnInquiries, btnPropertyRequests, btnManageAgents, btnPendingAssignments, btnLogs });
            }
            foreach (var b in navButtons)
            {
                if (b == null) continue;

                // preserve original text in Tag so we can collapse/expand safely
                if (b.Tag == null) b.Tag = b.Text;

                // remove from designer parent and add to flow panel (if not already removed)
                if (panelSidebar.Controls.Contains(b)) panelSidebar.Controls.Remove(b);

                b.Width = flSidebarNav.Width - 4;
                b.Height = _sidebarButtonHeight;
                b.Margin = new Padding(0, 8, 0, 0);
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.ImageAlign = ContentAlignment.MiddleLeft;
                b.TextImageRelation = TextImageRelation.ImageBeforeText;

                // reset padding appropriate for larger font/icon
                b.Padding = new Padding(12, 0, 8, 0);

                // hover effects
                b.MouseEnter -= NavButton_MouseEnter;
                b.MouseLeave -= NavButton_MouseLeave;
                b.MouseEnter += NavButton_MouseEnter;
                b.MouseLeave += NavButton_MouseLeave;

                // apply subtle rounded look by changing BackColor on controls (transparent keeps designer look)
                b.FlatAppearance.MouseOverBackColor = Color.Transparent;

                // If the control is already a child of flSidebarNav, don't add again
                if (!flSidebarNav.Controls.Contains(b))
                    flSidebarNav.Controls.Add(b);
            }

            // Insert the nav flow into panelSidebar below the logo
            if (!panelSidebar.Controls.Contains(flSidebarNav))
                panelSidebar.Controls.Add(flSidebarNav);

            // Place flow immediately after pbLogo
            var logoIndex = panelSidebar.Controls.IndexOf(pbLogo);
            if (logoIndex >= 0)
                panelSidebar.Controls.SetChildIndex(flSidebarNav, logoIndex + 1);

            // Create collapse/expand button (top-right inside sidebar)
            if (btnCollapseSidebar == null)
            {
                btnCollapseSidebar = new Button();
                btnCollapseSidebar.Name = "btnCollapseSidebar";
                btnCollapseSidebar.Text = "≡";
                btnCollapseSidebar.FlatStyle = FlatStyle.Flat;
                btnCollapseSidebar.Size = new Size(36, 36);
                btnCollapseSidebar.BackColor = Color.Transparent;
                btnCollapseSidebar.FlatAppearance.BorderSize = 0;
                btnCollapseSidebar.Click += BtnCollapseSidebar_Click;
            }

            btnCollapseSidebar.Location = new Point(panelSidebar.Width - btnCollapseSidebar.Width - 12, 12);

            // Add collapse button on top so it's accessible
            if (!panelSidebar.Controls.Contains(btnCollapseSidebar))
                panelSidebar.Controls.Add(btnCollapseSidebar);
            panelSidebar.Controls.SetChildIndex(btnCollapseSidebar, 0);

            // Tidy bottom items: place Settings and Help anchored to bottom
            // In this app, we are removing Settings and Help from the sidebar,
            // so skip creating the bottom panel entirely.
            bool showBottomItems = false;
            if (!showBottomItems)
            {
                if (panelSidebarBottom != null)
                {
                    if (panelSidebar.Controls.Contains(panelSidebarBottom))
                        panelSidebar.Controls.Remove(panelSidebarBottom);
                    panelSidebarBottom.Controls.Clear(); // Clear controls to prevent disposing the buttons
                    panelSidebarBottom.Dispose();
                    panelSidebarBottom = null;
                }

                // Final layout fixup without bottom items
                UpdateSidebarLayout();
                return;
            }
            if (panelSidebarBottom != null)
            {
                if (panelSidebar.Controls.Contains(panelSidebarBottom))
                    panelSidebar.Controls.Remove(panelSidebarBottom);
                panelSidebarBottom.Controls.Clear(); // Clear controls to prevent disposing the buttons
                panelSidebarBottom.Dispose();
                panelSidebarBottom = null;
            }

            panelSidebarBottom = new Panel
            {
                Name = "panelSidebarBottom",
                BackColor = Color.FromArgb(250, 250, 250), // soft neutral background for bottom area
                Location = new Point(12, panelSidebar.Height - 140),
                Size = new Size(panelSidebar.Width - 24, 100),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Move settings/help into bottom panel
            if (btnSettings != null)
            {
                if (panelSidebar.Controls.Contains(btnSettings)) panelSidebar.Controls.Remove(btnSettings);
                btnSettings.Width = panelSidebarBottom.Width;
                btnSettings.Height = _sidebarButtonHeight - 6;
                btnSettings.Margin = new Padding(0, 8, 0, 0);
                btnSettings.FlatStyle = FlatStyle.Flat;
                btnSettings.FlatAppearance.BorderSize = 0;
                btnSettings.Padding = new Padding(12, 0, 8, 0);
                btnSettings.ForeColor = Color.FromArgb(90, 95, 100);
                if (!panelSidebarBottom.Controls.Contains(btnSettings))
                    panelSidebarBottom.Controls.Add(btnSettings);
            }
            if (btnHelp != null)
            {
                if (panelSidebar.Controls.Contains(btnHelp)) panelSidebar.Controls.Remove(btnHelp);
                btnHelp.Width = panelSidebarBottom.Width;
                btnHelp.Height = _sidebarButtonHeight - 6;
                btnHelp.Margin = new Padding(0, 8, 0, 0);
                btnHelp.FlatStyle = FlatStyle.Flat;
                btnHelp.FlatAppearance.BorderSize = 0;
                btnHelp.Padding = new Padding(12, 0, 8, 0);
                btnHelp.ForeColor = Color.FromArgb(90, 95, 100);
                if (!panelSidebarBottom.Controls.Contains(btnHelp))
                    panelSidebarBottom.Controls.Add(btnHelp);
            }

            if (!panelSidebar.Controls.Contains(panelSidebarBottom))
                panelSidebar.Controls.Add(panelSidebarBottom);
            panelSidebar.Controls.SetChildIndex(panelSidebarBottom, panelSidebar.Controls.Count - 1);

            // Final layout fixup to ensure widths and positions are correct
            UpdateSidebarLayout();
        }

        private void EnsureClientButtons()
        {
            // Recreate client buttons if they were disposed earlier (e.g., when flSidebarNav was disposed)
            if (btnClientAddProperty == null || btnClientAddProperty.IsDisposed)
            {
                btnClientAddProperty = new Button
                {
                    Name = "btnClientAddProperty",
                    Text = "My Listings",
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", _sidebarFontPt, FontStyle.Regular, GraphicsUnit.Point),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                btnClientAddProperty.FlatAppearance.BorderSize = 0;
                // Fixed icon positioning - removed vertical offset for proper centering
                try { btnClientAddProperty.Image = CreateIconFromGlyph('\uE710', _sidebarIconSize, _sidebarIconSize, _brandBlue); } catch { }
                btnClientAddProperty.ImageAlign = ContentAlignment.MiddleLeft;
                btnClientAddProperty.TextImageRelation = TextImageRelation.ImageBeforeText;
                btnClientAddProperty.Padding = new Padding(12, 0, 8, 0);
                btnClientAddProperty.Click -= BtnClientAddProperty_Click;
                btnClientAddProperty.Click += BtnClientAddProperty_Click;
            }

            if (btnClientBrowseProperty == null || btnClientBrowseProperty.IsDisposed)
            {
                btnClientBrowseProperty = new Button
                {
                    Name = "btnClientBrowseProperty",
                    Text = "Browse Properties", // Fixed: Full label instead of "Browse Property"
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", _sidebarFontPt, FontStyle.Regular, GraphicsUnit.Point),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                btnClientBrowseProperty.FlatAppearance.BorderSize = 0;
                // Fixed icon positioning - removed vertical offset for proper centering
                try { btnClientBrowseProperty.Image = CreateIconFromGlyph('\uE8B8', _sidebarIconSize, _sidebarIconSize, _brandBlue); } catch { }
                btnClientBrowseProperty.ImageAlign = ContentAlignment.MiddleLeft;
                btnClientBrowseProperty.TextImageRelation = TextImageRelation.ImageBeforeText;
                btnClientBrowseProperty.Padding = new Padding(12, 0, 8, 0);
                btnClientBrowseProperty.Click -= BtnClientBrowseProperty_Click;
                btnClientBrowseProperty.Click += BtnClientBrowseProperty_Click;
            }

            if (btnClientMyDeals == null || btnClientMyDeals.IsDisposed)
            {
                btnClientMyDeals = new Button
                {
                    Name = "btnClientMyDeals",
                    Text = "My Deals",
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", _sidebarFontPt, FontStyle.Regular, GraphicsUnit.Point),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                btnClientMyDeals.FlatAppearance.BorderSize = 0;
                // Use deals icon
                try { btnClientMyDeals.Image = CreateIconFromGlyph('\uEAFD', _sidebarIconSize, _sidebarIconSize, _brandBlue); } catch { }
                btnClientMyDeals.ImageAlign = ContentAlignment.MiddleLeft;
                btnClientMyDeals.TextImageRelation = TextImageRelation.ImageBeforeText;
                btnClientMyDeals.Padding = new Padding(12, 0, 8, 0);
                btnClientMyDeals.Click -= BtnClientMyDeals_Click;
                btnClientMyDeals.Click += BtnClientMyDeals_Click;
            }
        }

        private void UpdateSidebarLayout()
        {
            if (panelSidebar == null) return;

            // Update collapse button position
            if (btnCollapseSidebar != null)
            {
                btnCollapseSidebar.Location = new Point(panelSidebar.Width - btnCollapseSidebar.Width - 12, btnCollapseSidebar.Location.Y);
            }

            // Update flSidebarNav width and button widths if expanded
            if (flSidebarNav != null)
            {
                flSidebarNav.Width = Math.Max(100, panelSidebar.Width - 24);
                // adjust height to fit available space between logo and bottom panel
                int top = pbLogo?.Bottom + 12 ?? 100;
                int bottomY = panelSidebarBottom != null ? panelSidebarBottom.Top : panelSidebar.Height - 12;
                flSidebarNav.Location = new Point(12, top);
                flSidebarNav.Height = Math.Max(120, bottomY - top - 12);

                foreach (Control c in flSidebarNav.Controls)
                {
                    if (c is Button b && !_sidebarCollapsed)
                    {
                        b.Width = flSidebarNav.Width - 4;
                        b.Height = _sidebarButtonHeight;
                    }
                }
            }

            // Update bottom panel sizing/position and bottom button widths
            if (panelSidebarBottom != null)
            {
                panelSidebarBottom.Location = new Point(12, panelSidebar.Height - panelSidebarBottom.Height - 12);
                panelSidebarBottom.Width = Math.Max(100, panelSidebar.Width - 24);

                foreach (Control c in panelSidebarBottom.Controls)
                {
                    if (c is Button b)
                    {
                        if (!_sidebarCollapsed)
                        {
                            b.Width = panelSidebarBottom.Width;
                            b.Height = _sidebarButtonHeight - 6;
                        }
                        else
                        {
                            b.Width = 56;
                            b.Height = 56;
                        }
                    }
                }
            }

            // When collapsed ensure controls use small square sizes
            if (_sidebarCollapsed)
            {
                if (flSidebarNav != null)
                {
                    foreach (Control c in flSidebarNav.Controls)
                    {
                        if (c is Button b)
                        {
                            b.Width = 56;
                            b.Height = 56;
                        }
                    }
                }
            }
        }

        private void BtnCollapseSidebar_Click(object? sender, EventArgs e)
        {
            ToggleSidebar();
        }

        private void ToggleSidebar()
        {
            if (panelSidebar == null || flSidebarNav == null) return;

            _sidebarCollapsed = !_sidebarCollapsed;

            if (_sidebarCollapsed)
            {
                // Collapse: narrow sidebar and hide texts
                panelSidebar.Width = 72;
                pbLogo.Visible = false;
                btnCollapseSidebar!.Location = new Point(panelSidebar.Width - btnCollapseSidebar.Width - 8, btnCollapseSidebar.Location.Y);

                foreach (Control c in flSidebarNav.Controls)
                {
                    if (c is Button b)
                    {
                        // center icon
                        b.Text = string.Empty;
                        b.ImageAlign = ContentAlignment.MiddleCenter;
                        b.Padding = new Padding(0);
                        b.Width = 56;
                        b.Height = 56; // icon-focused square
                    }
                }

                // bottom items
                if (btnSettings != null) { btnSettings.Text = string.Empty; btnSettings.Width = 56; btnSettings.ImageAlign = ContentAlignment.MiddleCenter; btnSettings.Height = 56; }
                if (btnHelp != null) { btnHelp.Text = string.Empty; btnHelp.Width = 56; btnHelp.ImageAlign = ContentAlignment.MiddleCenter; btnHelp.Height = 56; }
            }
            else
            {
                // Expand: restore sizes and texts from Tag
                panelSidebar.Width = 220;
                pbLogo.Visible = true;
                btnCollapseSidebar!.Location = new Point(panelSidebar.Width - btnCollapseSidebar.Width - 12, btnCollapseSidebar.Location.Y);

                foreach (Control c in flSidebarNav.Controls)
                {
                    if (c is Button b)
                    {
                        b.Text = b.Tag as string ?? b.Text;
                        b.ImageAlign = ContentAlignment.MiddleLeft;
                        b.Padding = new Padding(12, b.Padding.Top, b.Padding.Right, b.Padding.Bottom);
                        b.Width = flSidebarNav.Width - 4;
                        b.Height = _sidebarButtonHeight;
                    }
                }

                if (btnSettings != null) { btnSettings.Text = btnSettings.Tag as string ?? btnSettings.Text; btnSettings.Width = panelSidebarBottom?.Width ?? (panelSidebar.Width - 24); btnSettings.ImageAlign = ContentAlignment.MiddleLeft; btnSettings.Height = _sidebarButtonHeight - 6; }
                if (btnHelp != null) { btnHelp.Text = btnHelp.Tag as string ?? btnHelp.Text; btnHelp.Width = panelSidebarBottom?.Width ?? (panelSidebar.Width - 24); btnHelp.ImageAlign = ContentAlignment.MiddleLeft; btnHelp.Height = _sidebarButtonHeight - 6; }
            }

            // Ensure layout updated after toggle
            UpdateSidebarLayout();

            // Reapply active styling to make sure active button remains highlighted
            // (SetActiveNavButton expects current active button; easiest is to re-call with current lblSectionTitle)
            var activeBtn = GetButtonForSection(lblSectionTitle.Text);
            if (activeBtn != null)
                SetActiveNavButton(activeBtn);
        }

        private Button? GetButtonForSection(string section)
        {
            return section switch
            {
                "Dashboard" => btnDashboard,
                "Manage Agents" => btnManageAgents,
                "Property Requests" => btnPropertyRequests,
                "Inquiries" => btnInquiries,
                "Leads" => btnLeads,
                "Contacts" => btnContacts,
                "Deals" => btnDeals,
                "Properties" => btnProperties,
                "Pending Assignments" => btnPendingAssignments,
                "Add Property" => btnClientAddProperty,
                "My Listings" => btnClientAddProperty,
                "Browse Properties" => btnClientBrowseProperty, // Updated to match the corrected text
                "My Deals" => btnClientMyDeals,
                _ => null
            };
        }

        private void changeCredentialsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var user = Services.UserSession.Instance.CurrentUser;
            if (user == null) return;
            using var dlg = new ChangeCredentialsDialog(new AuthenticationService(), user.Email);
            dlg.ShowDialog(FindForm());
        }

        private void NavButton_MouseEnter(object? sender, System.EventArgs e)
        {
            if (sender is Button b)
            {
                // hover uses subtle warm accent to complement brand blue icons
                b.BackColor = _hoverAccent;
            }
        }

        private void NavButton_MouseLeave(object? sender, System.EventArgs e)
        {
            if (sender is Button b)
            {
                // keep active button highlighted
                var isActive = lblSectionTitle.Text == (b.Tag as string ?? b.Text);
                if (!isActive)
                    b.BackColor = Color.Transparent;
            }
        }

        private void SetupSidebarIcons()
        {
            // Use configurable icon size
            int iconSize = _sidebarIconSize; // px (square)
            Color iconColor = _brandBlue; // brand blue

            // Map buttons to Segoe MDL2 glyphs
            var map = new Dictionary<Button, char?>
            {
                { btnDashboard, '\uE80F' }, // Home
                { btnProperties, '\uE8B8' }, // Building / property-ish
                { btnPendingAssignments, '\uE916' }, // Clock / pending
                { btnLeads, '\uE77B' }, // Contact / person
                { btnContacts, '\uE8C7' }, // People
                { btnDeals, '\uEAFD' }, // Currency 
                { btnSettings, '\uE713' }, // Settings / gear
                { btnHelp, '\uE11B' } // Help / question
            };

            if (btnManageAgents != null)
            {
                map[btnManageAgents] = '\uE716'; // People (multiple)
            }

            foreach (var kv in map)
            {
                var btn = kv.Key;
                var glyph = kv.Value;

                if (btn == null) continue;

                if (glyph.HasValue)
                {
                    btn.Image?.Dispose();
                    // Create icon without any manual vertical offset so glyphs are centered by default
                    btn.Image = CreateIconFromGlyph(glyph.Value, iconSize, iconSize, iconColor);

                    // Ensure uniform layout
                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                    btn.TextImageRelation = TextImageRelation.ImageBeforeText;

                    // Add a left padding scaled for larger icon
                    btn.Padding = new Padding(12, btn.Padding.Top, btn.Padding.Right, btn.Padding.Bottom);
                }
            }

            // Fixed: Pending Assignments icon positioning - removed manual vertical offset
            if (btnPendingAssignments != null)
            {
                try
                {
                    btnPendingAssignments.Image?.Dispose();
                    btnPendingAssignments.Image = CreateIconFromGlyph('\uE916', _sidebarIconSize, _sidebarIconSize, _brandBlue);
                }
                catch { }
            }
        }

        private Image CreateIconFromGlyph(char glyph, int width, int height, Color color, int verticalOffset = 0)
        {
            // Create a bitmap and draw the glyph centered
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Segoe MDL2 Assets contains Windows UI glyphs
                using (var font = new Font("Segoe MDL2 Assets", Math.Max(12, height - 6), FontStyle.Regular, GraphicsUnit.Pixel))
                using (var brush = new SolidBrush(color))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    // apply vertical offset by translating the rectangle used to draw the glyph
                    var rect = new RectangleF(0, verticalOffset, width, height);
                    g.DrawString(glyph.ToString(), font, brush, rect, sf);
                }
            }
            return bmp;
        }

        private void LoadSidebarLogo()
        {
            try
            {
                // Load image from embedded resource
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "RealEstateCRMWinForms.Assets.Homey_transparent.png";

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        pbLogo.Image?.Dispose();
                        pbLogo.Image = new Bitmap(stream);
                        // enlarge logo area slightly for visual balance with bigger nav
                        pbLogo.Size = new Size(196, 100);
                        Debug.WriteLine("✓ Logo loaded successfully from embedded resource");
                    }
                    else
                    {
                        Debug.WriteLine($"✗ Embedded resource not found: {resourceName}");
                        CreateLogoPlaceholder();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadSidebarLogo error: {ex.Message}");
                CreateLogoPlaceholder();
            }
        }

        private void CreateLogoPlaceholder()
        {
            // Create a placeholder to show the logo area is working - updated size to match enlarged logo
            var placeholderBitmap = new Bitmap(196, 100);
            using (var g = Graphics.FromImage(placeholderBitmap))
            {
                // use complementary warm accent for logo background
                g.Clear(_lightAccent);
                using (var brush = new SolidBrush(_accentWarm))
                using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
                {
                    string text = "LOGO HERE";
                    var textSize = g.MeasureString(text, font);
                    var x = (placeholderBitmap.Width - textSize.Width) / 2;
                    var y = (placeholderBitmap.Height - textSize.Height) / 2;
                    g.DrawString(text, font, brush, x, y);
                }
            }
            pbLogo.Image?.Dispose();
            pbLogo.Image = placeholderBitmap;
            pbLogo.Size = new Size(196, 100);
        }

        // Removed legacy button1_Click handler (Load Users)

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            SwitchSection("Dashboard");
        }

        private void btnLeads_Click(object sender, EventArgs e)
        {
            SwitchSection("Leads");
        }

        private void btnContacts_Click(object sender, EventArgs e)
        {
            SwitchSection("Contacts");
        }

        private void btnDeals_Click(object sender, EventArgs e)
        {
            SwitchSection("Deals");
        }

        private void btnProperties_Click(object sender, EventArgs e)
        {
            SwitchSection("Properties");
        }

        private void BtnClientAddProperty_Click(object? sender, EventArgs e)
        {
            // Show properties view in My Listings mode for clients
            SwitchSection("My Listings");
        }

        private void BtnClientBrowseProperty_Click(object? sender, EventArgs e)
        {
            // Show properties view in browse mode (only approved properties, no add button)
            SwitchSection("Browse Properties");
        }

        private void BtnClientMyDeals_Click(object? sender, EventArgs e)
        {
            // Show client deals view with status change requests
            SwitchSection("My Deals");
        }

        private void btnManageAgents_Click(object? sender, EventArgs e)
        {
            SwitchSection("Manage Agents");
        }

        private void btnLogs_Click(object? sender, EventArgs e)
        {
            SwitchSection("Logs");
        }

        private void EnsureManageAgentsButton()
        {
            if (btnManageAgents != null) return;

            btnManageAgents = new Button
            {
                Name = "btnManageAgents",
                Text = "Manage Agents",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", _sidebarFontPt, FontStyle.Regular, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft
            };
            btnManageAgents.FlatAppearance.BorderSize = 0;
            // assign icon similar to others - removed vertical offset for proper centering
            try { btnManageAgents.Image = CreateIconFromGlyph('\uE716', _sidebarIconSize, _sidebarIconSize, _brandBlue); } catch { }
            // make sure alignment/padding matches designer-created nav buttons
            btnManageAgents.ImageAlign = ContentAlignment.MiddleLeft;
            btnManageAgents.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnManageAgents.Padding = new Padding(12, btnManageAgents.Padding.Top, 8, btnManageAgents.Padding.Bottom);
            btnManageAgents.Click += btnManageAgents_Click;
        }

        private void EnsurePropertyRequestsButton()
        {
            if (btnPropertyRequests != null) return;

            btnPropertyRequests = new Button
            {
                Name = "btnPropertyRequests",
                Text = "Property Requests",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", _sidebarFontPt, FontStyle.Regular, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft
            };
            btnPropertyRequests.FlatAppearance.BorderSize = 0;
            // assign icon - using a request icon glyph, e.g., \uE8C1 for mail or \uE8F1 for flag
            try { btnPropertyRequests.Image = CreateIconFromGlyph('\uE8C1', _sidebarIconSize, _sidebarIconSize, _brandBlue); } catch { }
            // make sure alignment/padding matches designer-created nav buttons
            btnPropertyRequests.ImageAlign = ContentAlignment.MiddleLeft;
            btnPropertyRequests.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnPropertyRequests.Padding = new Padding(12, btnPropertyRequests.Padding.Top, 8, btnPropertyRequests.Padding.Bottom);
            btnPropertyRequests.Click += btnPropertyRequests_Click;
        }

        private void btnPropertyRequests_Click(object? sender, EventArgs e)
        {
            SwitchSection("Property Requests");
        }

        private void EnsureInquiriesButton()
        {
            if (btnInquiries != null) return;

            btnInquiries = new Button
            {
                Name = "btnInquiries",
                Text = "Inquiries",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", _sidebarFontPt, FontStyle.Regular, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft
            };
            btnInquiries.FlatAppearance.BorderSize = 0;
            // assign icon - using an inquiry/message icon glyph
            try { btnInquiries.Image = CreateIconFromGlyph('\uE8BD', _sidebarIconSize, _sidebarIconSize, _brandBlue); } catch { }
            // make sure alignment/padding matches designer-created nav buttons
            btnInquiries.ImageAlign = ContentAlignment.MiddleLeft;
            btnInquiries.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnInquiries.Padding = new Padding(12, btnInquiries.Padding.Top, 8, btnInquiries.Padding.Bottom);
            btnInquiries.Click += btnInquiries_Click;
        }

        private void btnInquiries_Click(object? sender, EventArgs e)
        {
            SwitchSection("Inquiries");
        }

        private void EnsureLogsButton()
        {
            if (btnLogs != null) return;

            btnLogs = new Button
            {
                Name = "btnLogs",
                Text = "Logs",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", _sidebarFontPt, FontStyle.Regular, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleLeft
            };
            btnLogs.FlatAppearance.BorderSize = 0;
            // assign icon - using a log icon glyph, e.g., \uE7C4 for history or \uE81C for list - removed vertical offset
            try { btnLogs.Image = CreateIconFromGlyph('\uE81C', _sidebarIconSize, _sidebarIconSize, _brandBlue); } catch { }
            // make sure alignment/padding matches designer-created nav buttons
            btnLogs.ImageAlign = ContentAlignment.MiddleLeft;
            btnLogs.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnLogs.Padding = new Padding(12, btnLogs.Padding.Top, 8, btnLogs.Padding.Bottom);
            btnLogs.Click += btnLogs_Click;
        }

        private void SetActiveNavButton(Button active)
        {
            // clear visuals for all nav buttons (search both flSidebarNav and panelSidebarBottom)
            IEnumerable<Button> allNavButtons()
            {
                if (flSidebarNav != null)
                {
                    foreach (Control c in flSidebarNav.Controls)
                        if (c is Button b) yield return b;
                }

                if (panelSidebarBottom != null)
                {
                    foreach (Control c in panelSidebarBottom.Controls)
                        if (c is Button b) yield return b;
                }

                // fallback: existing direct children
                foreach (Control c in panelSidebar.Controls)
                    if (c is Button b) yield return b;
            }

            foreach (var b in allNavButtons())
            {
                if (b == btnSettings || b == btnHelp) continue;
                b.BackColor = Color.Transparent;
                b.ForeColor = Color.FromArgb(33, 37, 41);
                // preserve font size/unit — reset to regular
                b.Font = new Font(b.Font.FontFamily, b.Font.Size, FontStyle.Regular, b.Font.Unit);
            }

            if (active != null)
            {
                // active uses light warm background and brand blue foreground to complement icons
                active.BackColor = _lightAccent;
                active.ForeColor = _brandBlue;
                active.Font = new Font(active.Font.FontFamily, active.Font.Size, FontStyle.Bold, active.Font.Unit);
            }
        }

        public void SwitchSection(string section)
        {
            // Prevent non-Brokers and non-Clients from accessing Properties section
            if (!_isBroker && !_isClient && string.Equals(section, "Properties", System.StringComparison.OrdinalIgnoreCase))
            {
                section = "Dashboard";
            }
            // Update the small top header. For Dashboard we no longer show "Agency Dashboard";
            // leave it blank instead. Other sections continue to use their section name.
            var effectiveTitle = section;
            lblSectionTitle.Text = effectiveTitle == "Dashboard" ? string.Empty : effectiveTitle;

            // set visual active button and update content
            switch (section)
            {
                case "Dashboard":
                    SetActiveNavButton(btnDashboard);
                    ShowDashboardView();
                    break;
                case "Manage Agents":
                    if (_isBroker)
                    {
                        SetActiveNavButton(btnManageAgents!);
                        SwitchContentView(new ManageAgentsView());
                    }
                    else
                    {
                        SwitchSection("Dashboard");
                    }
                    break;
                case "Property Requests":
                    if (_isBroker)
                    {
                        SetActiveNavButton(btnPropertyRequests!);
                        SwitchContentView(new PropertyRequestsView());
                    }
                    else
                    {
                        SwitchSection("Dashboard");
                    }
                    break;
                case "Inquiries":
                    if (_isBroker)
                    {
                        SetActiveNavButton(btnInquiries!);
                        // TODO: Create InquiriesView
                        SwitchContentView(new InquiriesView());
                    }
                    else
                    {
                        SwitchSection("Dashboard");
                    }
                    break;
                case "Leads":
                    SetActiveNavButton(btnLeads);
                    ShowLeadsView();
                    break;
                case "Contacts":
                    SetActiveNavButton(btnContacts);
                    ShowContactsView();
                    break;
                case "Deals":
                    SetActiveNavButton(btnDeals);
                    ShowDealsView();
                    break;
                case "Pending Assignments":
                    SetActiveNavButton(btnPendingAssignments);
                    SwitchContentView(new PendingAssignmentsView(false));
                    break;
                case "Logs":
                    SetActiveNavButton(btnLogs!);
                    SwitchContentView(new LogsView());
                    break;
                case "Browse Properties":
                    if (btnClientBrowseProperty != null)
                        SetActiveNavButton(btnClientBrowseProperty);
                    ShowBrowsePropertiesView();
                    break;
                case "My Listings":
                    if (btnClientAddProperty != null)
                        SetActiveNavButton(btnClientAddProperty);
                    ShowMyListingsView();
                    break;
                case "My Deals":
                    if (btnClientMyDeals != null)
                        SetActiveNavButton(btnClientMyDeals);
                    ShowClientDealsView();
                    break;
                case "Properties":
                default:
                    SetActiveNavButton(btnProperties);
                    ShowPropertiesView();
                    break;
            }
        }

        private void ShowPropertiesView()
        {
            // Guard at runtime as well
            if (_isBroker || _isClient)
            {
                bool isReadOnly = _isClient;
                SwitchContentView(new PropertiesView(isReadOnly));
            }
            else
            {
                // Fallback for others
                SwitchSection("Dashboard");
            }
        }

        private void ShowBrowsePropertiesView()
        {
            // For browse mode: clients see only approved properties, no add button
            if (_isClient)
            {
                SwitchContentView(new PropertiesView(isReadOnly: true, isBrowseMode: true));
            }
            else
            {
                // Fallback for non-clients
                SwitchSection("Dashboard");
            }
        }

        private void ShowMyListingsView()
        {
            // For My Listings mode: clients see their properties with approved listings toggle and can add new properties
            if (_isClient)
            {
                SwitchContentView(new PropertiesView(isReadOnly: false, isBrowseMode: false, isMyListingsMode: true));
            }
            else
            {
                // Fallback for non-clients
                SwitchSection("Dashboard");
            }
        }

        private void ShowDashboardView()
        {
            SwitchContentView(new DashboardView());
        }

        private void ShowClientDealsView()
        {
            // Show client deals view with pending status change requests
            if (_isClient)
            {
                SwitchContentView(new ClientDealsView());
            }
            else
            {
                // Fallback for non-clients
                SwitchSection("Dashboard");
            }
        }

        private void ShowLeadsView()
        {
            SwitchContentView(new LeadsView());
        }

        private void ShowContactsView()
        {
            SwitchContentView(new ContactsView());
        }

        private void ShowDealsView()
        {
            SwitchContentView(new DealsView());
        }

        private void SwitchContentView(UserControl? newView)
        {
            // Remove current content view
            if (_currentContentView != null)
            {
                panelContent.Controls.Remove(_currentContentView);
                _currentContentView.Dispose();
                _currentContentView = null;
            }

            if (newView != null)
            {
                // Hide the old grid
                dataGridView1.Visible = false;

                // Add new view
                _currentContentView = newView;
                _currentContentView.Location = new Point(1, 108); // After header and title
                _currentContentView.Size = new Size(panelContent.Width - 2, panelContent.Height - 130);
                _currentContentView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                panelContent.Controls.Add(_currentContentView);
            }
            else
            {
                // Show the old grid for other sections
                dataGridView1.Visible = true;
            }
        }

        // Public helper to populate the header user area
        public void SetCurrentUser(string displayName, string role)
        {
            lblUserName.Text = displayName ?? string.Empty;

            // Display friendly role names
            var friendlyRole = string.Empty;
            if (string.Equals(role, UserRole.Broker.ToString(), System.StringComparison.OrdinalIgnoreCase))
                friendlyRole = "Logged in as Broker";
            else if (string.Equals(role, UserRole.Client.ToString(), System.StringComparison.OrdinalIgnoreCase))
                friendlyRole = "Client";
            else if (string.Equals(role, UserRole.Agent.ToString(), System.StringComparison.OrdinalIgnoreCase))
                friendlyRole = "Agent";
            else
                friendlyRole = role ?? string.Empty;

            lblUserRole.Text = string.IsNullOrWhiteSpace(friendlyRole) ? string.Empty : friendlyRole;

            // pbAvatar currently blank; you can set pbAvatar.Image = ... when available
            pbAvatar.Image = null;

            // Apply role-based UI gating
            ApplyRolePermissions(friendlyRole);
        }

        private void ApplyRolePermissions(string role)
        {
            // Role passed in is the friendly name; treat Broker specially
            _isBroker = string.Equals(role, "Logged in as Broker", System.StringComparison.OrdinalIgnoreCase)
                        || string.Equals(role, "Broker", System.StringComparison.OrdinalIgnoreCase);

            // Treat Client specially (friendly name 'Client')
            _isClient = string.Equals(role, "Client", System.StringComparison.OrdinalIgnoreCase);

            // Only Brokers can access certain admin/dev tools
            // Load Users button removed

            // Example: show Settings for Brokers only (if re-enabled elsewhere)
            if (btnSettings != null)
            {
                btnSettings.Visible = _isBroker;
                btnSettings.Enabled = _isBroker;
            }

            if (registerAgentToolStripMenuItem != null)
            {
                registerAgentToolStripMenuItem.Visible = _isBroker;
                registerAgentToolStripMenuItem.Enabled = _isBroker;
            }

            // Properties navigation: visible only for Brokers
            if (btnProperties != null)
            {
                btnProperties.Text = "Properties";
                btnProperties.Visible = _isBroker;
                btnProperties.Enabled = _isBroker;
            }

            // Client-specific nav: Add/Browse properties
            EnsureClientButtons();
            if (btnClientAddProperty != null)
            {
                btnClientAddProperty.Visible = _isClient;
                btnClientAddProperty.Enabled = _isClient;
            }
            if (btnClientBrowseProperty != null)
            {
                btnClientBrowseProperty.Visible = _isClient;
                btnClientBrowseProperty.Enabled = _isClient;
            }
            if (btnClientMyDeals != null)
            {
                btnClientMyDeals.Visible = _isClient;
                btnClientMyDeals.Enabled = _isClient;
            }

            // Rebuild the sidebar layout to include/exclude client buttons as needed
            ApplyModernSidebarLayout();
            // Manage Agents navigation for Brokers only
            EnsureManageAgentsButton();
            if (btnManageAgents != null)
            {
                btnManageAgents.Visible = _isBroker;
                btnManageAgents.Enabled = _isBroker;
            }
            // Property Requests navigation for Brokers only
            EnsurePropertyRequestsButton();
            if (btnPropertyRequests != null)
            {
                btnPropertyRequests.Visible = _isBroker;
                btnPropertyRequests.Enabled = _isBroker;
            }
            EnsureInquiriesButton();
            if (btnInquiries != null)
            {
                btnInquiries.Visible = _isBroker;
                btnInquiries.Enabled = _isBroker;
            }
            // Show Pending Assignments nav for Agents only
            if (btnPendingAssignments != null)
            {
                btnPendingAssignments.Visible = !_isBroker;
                btnPendingAssignments.Enabled = !_isBroker;
            }

            // If an Agent is currently on Properties, redirect to Dashboard
            if (!_isBroker && (_currentContentView is PropertiesView || _currentContentView is ManageAgentsView))
            {
                SwitchSection("Dashboard");
            }
        }

        private void registerAgentToolStripMenuItem_Click(object? sender, System.EventArgs e)
        {
            RegisterAgentRequested?.Invoke(this, System.EventArgs.Empty);
        }

        // show context menu when user clicks avatar or caret button
        private void btnUserMenu_Click(object sender, EventArgs e)
        {
            // show under the button
            contextMenuStripUser.Show(btnUserMenu, new Point(0, btnUserMenu.Height));
        }

        private void pbAvatar_Click(object sender, EventArgs e)
        {
            contextMenuStripUser.Show(pbAvatar, new Point(0, pbAvatar.Height));
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // bubble logout event to container (MainContainerForm will handle it)
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnPendingAssignments_Click(object? sender, EventArgs e)
        {
            SwitchSection("Pending Assignments");
        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelSidebar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelSidebar_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void panelSidebar_Paint_2(object sender, PaintEventArgs e)
        {

        }

        private void pbLogo_Click(object sender, EventArgs e)
        {

        }
    }
}