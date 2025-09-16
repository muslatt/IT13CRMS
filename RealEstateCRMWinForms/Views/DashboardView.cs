using RealEstateCRMWinForms.ViewModels;
using System;
using System.Linq;
using System.Windows.Forms;

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
            _propertyViewModel = new PropertyViewModel();
            _leadViewModel = new LeadViewModel();
            _contactViewModel = new ContactViewModel();
            _dealViewModel = new DealViewModel();

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                // Load data from ViewModels
                var properties = _propertyViewModel.Properties;
                var leads = _leadViewModel.Leads;
                var contacts = _contactViewModel.Contacts;
                var deals = _dealViewModel.Deals;

                // Update Properties Card
                lblPropertiesValue.Text = properties.Count.ToString();
                var activeProperties = properties.Count(p => p.IsActive);
                lblPropertiesDesc.Text = $"{activeProperties} Active Listings";

                // Update Deals Card (use actual Deal data)
                lblDealsValue.Text = deals.Count.ToString();

                // Count deals by status - assuming "Closed" is one status and others are in progress
                var closedDeals = deals.Count(d => d.Status == "Closed" || d.Status == "Contract Signed");
                var inProgressDeals = deals.Count(d => d.Status != "Closed" && d.Status != "Contract Signed" && d.IsActive);
                lblDealsDesc.Text = $"{closedDeals} Closed | {inProgressDeals} In Progress";

                // Update Contacts Card
                lblContactsValue.Text = contacts.Count.ToString();

                // For demonstration, assuming all contacts are agents for now
                // You can modify this logic based on your Contact model structure
                var agents = contacts.Count;
                var clients = 0; // You can add logic to differentiate between agents and clients
                lblContactsDesc.Text = $"{agents} Agents | {clients} Clients";

                // Update Leads Card
                lblLeadsValue.Text = leads.Count.ToString();
                lblLeadsDesc.Text = "Active Prospects";

                // Update timestamp
                lblLastUpdated.Text = $"Last updated: {DateTime.Now:MMM dd, yyyy HH:mm}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Navigation event handlers for the "View All" links
        private void LblViewAllProperties_Click(object sender, EventArgs e)
        {
            NavigateToSection("Properties");
        }

        private void LblViewAllDeals_Click(object sender, EventArgs e)
        {
            NavigateToSection("Deals");
        }

        private void LblViewAllContacts_Click(object sender, EventArgs e)
        {
            NavigateToSection("Contacts");
        }

        private void LblViewAllLeads_Click(object sender, EventArgs e)
        {
            NavigateToSection("Leads");
        }

        private void NavigateToSection(string sectionName)
        {
            try
            {
                // Find the MainView form that contains this UserControl
                var mainForm = this.FindForm();

                // Check if the form is MainView by checking its type name
                if (mainForm != null && mainForm.GetType().Name == "MainView")
                {
                    // Use reflection to call SwitchSection method
                    var switchSectionMethod = mainForm.GetType().GetMethod("SwitchSection");
                    if (switchSectionMethod != null)
                    {
                        switchSectionMethod.Invoke(mainForm, new object[] { sectionName });
                        return;
                    }
                }

                // Alternative approach: try to find MainView through parent controls
                Control parent = this.Parent;
                while (parent != null)
                {
                    if (parent.GetType().Name == "MainView")
                    {
                        var switchSectionMethod = parent.GetType().GetMethod("SwitchSection");
                        if (switchSectionMethod != null)
                        {
                            switchSectionMethod.Invoke(parent, new object[] { sectionName });
                            return;
                        }
                    }
                    parent = parent.Parent;
                }

                // If we can't find MainView, just show a message
                MessageBox.Show($"Navigating to {sectionName} section...", "Navigation",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Public method to refresh the dashboard data
        public void RefreshDashboard()
        {
            LoadDashboardData();
        }

        private void lblContactsIcon_Click(object sender, EventArgs e)
        {

        }

        private void lblPropertiesValue_Click(object sender, EventArgs e)
        {

        }

        private void dealsCard_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}