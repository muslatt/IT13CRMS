using RealEstateCRMWinForms.ViewModels;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class MainView : UserControl
    {
        private readonly UserViewModel _viewModel;
        public event EventHandler? LogoutRequested;

        private UserControl? _currentContentView;

        public MainView()
        {
            InitializeComponent();
            _viewModel = new UserViewModel();
            dataGridView1.DataSource = _viewModel.Users;

            // default section
            SwitchSection("Dashboard");

            // default placeholder user (blank avatar)
            SetCurrentUser("Ron Vergel Luzon", "Broker");
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
            // simple visual indicator for active nav
            foreach (Control c in panelSidebar.Controls)
            {
                if (c is Button b && b != btnSettings && b != btnHelp)
                {
                    b.BackColor = Color.Transparent;
                    b.ForeColor = Color.Black;
                    b.Font = new Font(b.Font, FontStyle.Regular);
                }
            }

            if (active != null)
            {
                active.BackColor = Color.FromArgb(235, 243, 255);
                active.ForeColor = Color.FromArgb(0, 102, 204);
                active.Font = new Font(active.Font, FontStyle.Bold);
            }
        }

        private void SwitchSection(string section)
        {
            lblSectionTitle.Text = section;

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
            // For now, keep the existing grid
            SwitchContentView(null);
        }

        private void ShowLeadsView()
        {
            // Instantiate and show the new LeadsView
            SwitchContentView(new LeadsView());
        }

        private void ShowContactsView()
        {
            // TODO: Create ContactsView
            SwitchContentView(null);
        }

        private void ShowDealsView()
        {
            // TODO: Create DealsView
            SwitchContentView(null);
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
    }
}