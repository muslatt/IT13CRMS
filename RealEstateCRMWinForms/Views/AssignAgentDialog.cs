using RealEstateCRMWinForms.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public class AssignAgentDialog : Form
    {
        private ComboBox cmbAgent;
        private Button btnAssign;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblPropertyInfo;
        private Label lblClientInfo;
        private Label lblInquiryMessage;
        private TextBox txtNotes;
        private Inquiry _inquiry;

        public int? SelectedAgentId { get; private set; }
        public string? DealNotes { get; private set; }

        public AssignAgentDialog(Inquiry inquiry)
        {
            _inquiry = inquiry;
            InitializeComponent();
            LoadAgents();
        }

        private void InitializeComponent()
        {
            Text = "Assign Agent to Inquiry";
            Size = new Size(600, 550);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(248, 249, 250);

            // Title
            lblTitle = new Label
            {
                Text = "Assign Agent & Create Deal",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(30, 20),
                Size = new Size(540, 30),
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            // Property info
            lblPropertyInfo = new Label
            {
                Text = $"Property: {_inquiry.Property?.Title ?? "N/A"}\nLocation: {_inquiry.Property?.Address ?? "N/A"}\nPrice: {_inquiry.Property?.Price.ToString("C0") ?? "N/A"}",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(30, 70),
                Size = new Size(540, 60),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            // Client info
            lblClientInfo = new Label
            {
                Text = $"Client: {_inquiry.Client?.FullName ?? "N/A"}\nEmail: {_inquiry.Client?.Email ?? "N/A"}\nPhone: {_inquiry.ContactPhone ?? "N/A"}",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(30, 140),
                Size = new Size(540, 60),
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            // Inquiry message label
            var lblMessageHeader = new Label
            {
                Text = "Client Message:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(30, 210),
                Size = new Size(540, 20),
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            // Inquiry message
            lblInquiryMessage = new Label
            {
                Text = _inquiry.Message,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(30, 235),
                Size = new Size(540, 50),
                ForeColor = Color.FromArgb(73, 80, 87),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5),
                BackColor = Color.White
            };

            // Agent selection label
            var lblAgent = new Label
            {
                Text = "Select Agent:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(30, 300),
                Size = new Size(120, 25),
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            // Agent combo box
            cmbAgent = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(160, 297),
                Width = 410,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Notes label
            var lblNotes = new Label
            {
                Text = "Deal Notes (Optional):",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(30, 340),
                Size = new Size(200, 25),
                ForeColor = Color.FromArgb(33, 37, 41)
            };

            // Notes text box
            txtNotes = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(30, 370),
                Size = new Size(540, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                PlaceholderText = "Add any notes about this deal..."
            };

            // Assign button
            btnAssign = new Button
            {
                Text = "✓ Assign & Create Deal",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(350, 460),
                Size = new Size(220, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAssign.FlatAppearance.BorderSize = 0;
            btnAssign.Click += BtnAssign_Click;

            // Cancel button
            btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 11F),
                Location = new Point(230, 460),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            Controls.AddRange(new Control[] {
                lblTitle, lblPropertyInfo, lblClientInfo,
                lblMessageHeader, lblInquiryMessage,
                lblAgent, cmbAgent,
                lblNotes, txtNotes,
                btnAssign, btnCancel
            });

            AcceptButton = btnAssign;
            CancelButton = btnCancel;
        }

        private void LoadAgents()
        {
            try
            {
                using var db = Data.DbContextHelper.CreateDbContext();

                // Load agents (users with Agent or Broker role)
                var agents = db.Users
                    .Where(u => u.IsActive && (u.RoleInt == (int)UserRole.Agent || u.RoleInt == (int)UserRole.Broker))
                    .OrderBy(u => u.FirstName)
                    .Select(u => new
                    {
                        u.Id,
                        DisplayName = u.FirstName + " " + u.LastName + " (" + u.Email + ")"
                    })
                    .ToList();

                cmbAgent.DataSource = agents;
                cmbAgent.DisplayMember = "DisplayName";
                cmbAgent.ValueMember = "Id";

                if (agents.Count == 0)
                {
                    MessageBox.Show(
                        "No agents available. Please create agent accounts first.",
                        "No Agents",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    btnAssign.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading agents: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                btnAssign.Enabled = false;
            }
        }

        private void BtnAssign_Click(object? sender, EventArgs e)
        {
            if (cmbAgent.SelectedValue == null)
            {
                MessageBox.Show(
                    "Please select an agent.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show(
                "This will:\n" +
                "1. Assign the selected agent to this inquiry\n" +
                "2. Create a contact from the client (if not exists)\n" +
                "3. Create a new deal in the NEW pipeline\n" +
                "4. Mark the inquiry as responded\n\n" +
                "Do you want to continue?",
                "Confirm Assignment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                SelectedAgentId = Convert.ToInt32(cmbAgent.SelectedValue);
                DealNotes = txtNotes.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
