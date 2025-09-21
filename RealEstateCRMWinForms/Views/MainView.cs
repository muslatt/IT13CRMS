using RealEstateCRMWinForms.ViewModels;
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

        public MainView()
        {
            InitializeComponent();
            _viewModel = new UserViewModel();
            dataGridView1.DataSource = _viewModel.Users;

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
            SetCurrentUser("Ron Vergel Luzon", "Broker");
        }

        private void PanelSidebar_SizeChanged(object? sender, System.EventArgs e)
        {
            // Adjust flow and bottom panel widths and buttons on resize
            UpdateSidebarLayout();
        }

        private void ConfigureSidebarFont()
        {
            const string fontFamily = "Segoe UI";
            var sideButtons = new[] { btnDashboard, btnProperties, btnLeads, btnContacts, btnDeals, btnSettings, btnHelp };

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
                                Math.Max(300, (_sidebarButtonHeight + 8) * 5)), // at least fit 5 items
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Ensure Tag exists for settings/help so we can restore text after collapse
            if (btnSettings != null && btnSettings.Tag == null) btnSettings.Tag = btnSettings.Text;
            if (btnHelp != null && btnHelp.Tag == null) btnHelp.Tag = btnHelp.Text;

            // Ensure consistent button sizing and margins, move existing nav buttons into the flow panel
            var navButtons = new[] { btnDashboard, btnProperties, btnLeads, btnContacts, btnDeals };
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
            if (panelSidebarBottom != null)
            {
                if (panelSidebar.Controls.Contains(panelSidebarBottom))
                    panelSidebar.Controls.Remove(panelSidebarBottom);
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
            SetActiveNavButton(GetButtonForSection(lblSectionTitle.Text));
        }

        private Button? GetButtonForSection(string section)
        {
            return section switch
            {
                "Dashboard" => btnDashboard,
                "Leads" => btnLeads,
                "Contacts" => btnContacts,
                "Deals" => btnDeals,
                "Properties" => btnProperties,
                _ => null
            };
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
                { btnLeads, '\uE77B' }, // Contact / person
                { btnContacts, '\uE8C7' }, // People
                { btnDeals, '\uEAFD' }, // Currency / money
                { btnSettings, '\uE713' }, // Settings / gear
                { btnHelp, '\uE11B' } // Help / question
            };

            foreach (var kv in map)
            {
                var btn = kv.Key;
                var glyph = kv.Value;

                if (btn == null) continue;

                if (glyph.HasValue)
                {
                    btn.Image?.Dispose();
                    btn.Image = CreateIconFromGlyph(glyph.Value, iconSize, iconSize, iconColor);

                    // Ensure uniform layout
                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                    btn.TextImageRelation = TextImageRelation.ImageBeforeText;

                    // Add a left padding scaled for larger icon
                    btn.Padding = new Padding(12, btn.Padding.Top, btn.Padding.Right, btn.Padding.Bottom);
                }
            }
        }

        private Image CreateIconFromGlyph(char glyph, int width, int height, Color color)
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
                    g.DrawString(glyph.ToString(), font, brush, new RectangleF(0, 0, width, height), sf);
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

        private void button1_Click(object sender, EventArgs e)
        {
            _viewModel.LoadUsers();
        }

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
            // Show "Agency Dashboard" in the small top header when Dashboard is active,
            // otherwise show the section name as-is.
            lblSectionTitle.Text = section == "Dashboard" ? "Agency Dashboard" : section;

            // set visual active button and update content
            switch (section)
            {
                case "Dashboard":
                    SetActiveNavButton(btnDashboard);
                    ShowDashboardView();
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
                case "Properties":
                default:
                    SetActiveNavButton(btnProperties);
                    ShowPropertiesView();
                    break;
            }
        }

        private void ShowPropertiesView()
        {
            SwitchContentView(new PropertiesView());
        }

        private void ShowDashboardView()
        {
            SwitchContentView(new DashboardView());
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
                // Hide the old grid and button
                dataGridView1.Visible = false;
                btnLoadUsers.Visible = false;

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
                btnLoadUsers.Visible = true;
            }
        }

        // Public helper to populate the header user area
        public void SetCurrentUser(string displayName, string role)
        {
            lblUserName.Text = displayName ?? string.Empty;
            lblUserRole.Text = role ?? string.Empty;

            // pbAvatar currently blank; you can set pbAvatar.Image = ... when available
            pbAvatar.Image = null;
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