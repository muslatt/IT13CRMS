using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.Services;
using RealEstateCRMWinForms.ViewModels;

namespace RealEstateCRMWinForms.Views
{
    public class ManageAgentsView : UserControl
    {
        private readonly AgentsViewModel _vm = new AgentsViewModel();
        private readonly AuthenticationService _auth = new AuthenticationService();

        private DataGridView dgv = null!;
        private Button btnRefresh = null!;
        private Button btnEdit = null!;
        private ComboBox cmbActive = null!;      // All/Active/Inactive
        private ComboBox cmbVerified = null!;    // All/Verified/Unverified
        private ComboBox cmbSort = null!;        // Sort options
        private DataGridViewButtonColumn colToggle = null!;

        public ManageAgentsView()
        {
            InitializeComponent();
            Load += (_, __) => RefreshAgents();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            var header = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = Color.White };
            btnRefresh = new Button { Text = "Refresh", AutoSize = true, Location = new Point(8, 8) };
            btnEdit = new Button { Text = "Edit Credentials...", AutoSize = true, Location = new Point(100, 8) };
            btnRefresh.Click += (_, __) => RefreshAgents();
            btnEdit.Click += (_, __) => EditSelected();
            header.Controls.Add(btnRefresh);
            header.Controls.Add(btnEdit);

            // Filters
            header.Controls.Add(new Label { Text = "Active:", AutoSize = true, Location = new Point(8, 44) });
            cmbActive = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(60, 40), Width = 110 };
            cmbActive.Items.AddRange(new object[] { "All", "Active", "Inactive" });
            cmbActive.SelectedIndex = 0;
            cmbActive.SelectedIndexChanged += (_, __) => RefreshAgents();
            header.Controls.Add(cmbActive);

            header.Controls.Add(new Label { Text = "Verified:", AutoSize = true, Location = new Point(180, 44) });
            cmbVerified = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(246, 40), Width = 120 };
            cmbVerified.Items.AddRange(new object[] { "All", "Verified", "Unverified" });
            cmbVerified.SelectedIndex = 0;
            cmbVerified.SelectedIndexChanged += (_, __) => RefreshAgents();
            header.Controls.Add(cmbVerified);

            header.Controls.Add(new Label { Text = "Sort:", AutoSize = true, Location = new Point(380, 44) });
            cmbSort = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(420, 40), Width = 200 };
            cmbSort.Items.AddRange(new object[] {
                "Name (A-Z)",
                "Name (Z-A)",
                "Created (Newest)",
                "Created (Oldest)",
                "Email (A-Z)",
                "Email (Z-A)",
                "Verified first",
                "Unverified first",
                "Active first",
                "Inactive first"
            });
            cmbSort.SelectedIndex = 0;
            cmbSort.SelectedIndexChanged += (_, __) => RefreshAgents();
            header.Controls.Add(cmbSort);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgv.DoubleClick += (_, __) => EditSelected();
            dgv.CellFormatting += Dgv_CellFormatting;
            dgv.CellContentClick += Dgv_CellContentClick;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "First Name", DataPropertyName = nameof(User.FirstName), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Last Name", DataPropertyName = nameof(User.LastName), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = nameof(User.Email), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 200 });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Active", DataPropertyName = nameof(User.IsActive), Width = 70 });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Verified", DataPropertyName = nameof(User.IsEmailVerified), Width = 80 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Created", DataPropertyName = nameof(User.CreatedAt), Width = 130, DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } });
            colToggle = new DataGridViewButtonColumn { HeaderText = "Action", Name = "colToggle", UseColumnTextForButtonValue = false, Width = 90 };
            dgv.Columns.Add(colToggle);

            Controls.Add(dgv);
            Controls.Add(header);
        }

        private void RefreshAgents()
        {
            bool? fActive = cmbActive.SelectedIndex switch { 1 => true, 2 => false, _ => (bool?)null };
            bool? fVerified = cmbVerified.SelectedIndex switch { 1 => true, 2 => false, _ => (bool?)null };
            var (sortBy, desc) = ParseSort();

            _vm.LoadAgents(fActive, fVerified, sortBy, desc);
            dgv.DataSource = _vm.Agents;
        }

        private (string sortBy, bool desc) ParseSort()
        {
            return (cmbSort.SelectedItem as string ?? "Name (A-Z)") switch
            {
                "Name (Z-A)" => ("Name", true),
                "Created (Newest)" => ("Created", true),
                "Created (Oldest)" => ("Created", false),
                "Email (A-Z)" => ("Email", false),
                "Email (Z-A)" => ("Email", true),
                "Verified first" => ("Verified", true),
                "Unverified first" => ("Verified", false),
                "Active first" => ("Active", true),
                "Inactive first" => ("Active", false),
                _ => ("Name", false)
            };
        }

        private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgv.Columns[e.ColumnIndex].Name == "colToggle")
            {
                if (dgv.Rows[e.RowIndex].DataBoundItem is User u)
                {
                    e.Value = u.IsActive ? "Deactivate" : "Activate";
                    e.FormattingApplied = true;
                }
            }
        }

        private void Dgv_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgv.Columns[e.ColumnIndex].Name == "colToggle")
            {
                if (dgv.Rows[e.RowIndex].DataBoundItem is User u)
                {
                    ToggleActive(u);
                }
            }
        }

        private void ToggleActive(User agent)
        {
            try
            {
                var action = agent.IsActive ? "deactivate" : "activate";
                var actionCapitalized = agent.IsActive ? "Deactivate" : "Activate";

                // Confirm before changing agent status
                var result = MessageBox.Show(
                    $"Are you sure you want to {action} agent '{agent.FullName}'?\n\n" +
                    (agent.IsActive ? "Deactivating will prevent this agent from accessing the system." : "Activating will allow this agent to access the system."),
                    $"Confirm {actionCapitalized} Agent",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                    return;

                using var db = RealEstateCRMWinForms.Data.DbContextHelper.CreateDbContext();
                var entity = db.Users.FirstOrDefault(x => x.Id == agent.Id);
                if (entity == null)
                {
                    MessageBox.Show("Agent not found.");
                    return;
                }
                entity.IsActive = !entity.IsActive;
                db.SaveChanges();

                MessageBox.Show(
                    $"Agent '{agent.FullName}' has been successfully {action}d!",
                    $"Agent {actionCapitalized}d",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                RefreshAgents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update agent: {ex.Message}");
            }
        }

        private void EditSelected()
        {
            if (dgv.CurrentRow?.DataBoundItem is not User agent)
            {
                MessageBox.Show("Select an agent to edit.");
                return;
            }

            using var dlg = new EditAgentCredentialsDialog(_auth, agent);
            if (dlg.ShowDialog(FindForm()) == DialogResult.OK && dlg.Updated)
            {
                MessageBox.Show(
                    $"Agent '{agent.FullName}' credentials have been successfully updated!",
                    "Agent Updated",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                RefreshAgents();
            }
        }
    }
}
