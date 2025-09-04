
namespace RealEstateCRMWinForms.Views
{
    partial class MainView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelSidebar = new Panel();
            btnHelp = new Button();
            btnSettings = new Button();
            navSeparator = new Panel();
            btnProperties = new Button();
            btnDeals = new Button();
            btnContacts = new Button();
            btnLeads = new Button();
            btnDashboard = new Button();
            panelContent = new Panel();
            contentLeftBorder = new Panel();
            dataGridView1 = new DataGridView();
            btnLoadUsers = new Button();
            headerPanel = new Panel();
            headerBottomBorder = new Panel();
            lblSectionTitle = new Label();
            panelUser = new Panel();
            btnUserMenu = new Button();
            lblUserRole = new Label();
            lblUserName = new Label();
            pbAvatar = new PictureBox();
            contextMenuStripUser = new ContextMenuStrip(components);
            logoutToolStripMenuItem = new ToolStripMenuItem();
            panelSidebar.SuspendLayout();
            panelContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            headerPanel.SuspendLayout();
            panelUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbAvatar).BeginInit();
            contextMenuStripUser.SuspendLayout();
            SuspendLayout();
            // 
            // panelSidebar
            // 
            panelSidebar.BackColor = Color.White;
            panelSidebar.Controls.Add(btnHelp);
            panelSidebar.Controls.Add(btnSettings);
            panelSidebar.Controls.Add(navSeparator);
            panelSidebar.Controls.Add(btnProperties);
            panelSidebar.Controls.Add(btnDeals);
            panelSidebar.Controls.Add(btnContacts);
            panelSidebar.Controls.Add(btnLeads);
            panelSidebar.Controls.Add(btnDashboard);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Padding = new Padding(12);
            panelSidebar.Size = new Size(220, 800);
            panelSidebar.TabIndex = 0;
            // 
            // btnHelp
            // 
            btnHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnHelp.FlatAppearance.BorderSize = 0;
            btnHelp.FlatStyle = FlatStyle.Flat;
            btnHelp.Location = new Point(12, 754);
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new Size(196, 30);
            btnHelp.TabIndex = 7;
            btnHelp.Text = "Help";
            btnHelp.UseVisualStyleBackColor = true;
            // 
            // btnSettings
            // 
            btnSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSettings.FlatAppearance.BorderSize = 0;
            btnSettings.FlatStyle = FlatStyle.Flat;
            btnSettings.Location = new Point(12, 718);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(196, 30);
            btnSettings.TabIndex = 6;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = true;
            // 
            // navSeparator
            // 
            navSeparator.BackColor = Color.Black;
            navSeparator.Location = new Point(12, 192);
            navSeparator.Name = "navSeparator";
            navSeparator.Size = new Size(196, 1);
            navSeparator.TabIndex = 5;
            // 
            // btnProperties
            // 
            btnProperties.FlatAppearance.BorderSize = 0;
            btnProperties.FlatStyle = FlatStyle.Flat;
            btnProperties.Location = new Point(12, 156);
            btnProperties.Name = "btnProperties";
            btnProperties.Size = new Size(196, 30);
            btnProperties.TabIndex = 4;
            btnProperties.Text = "Properties";
            btnProperties.TextAlign = ContentAlignment.MiddleLeft;
            btnProperties.UseVisualStyleBackColor = true;
            btnProperties.Click += btnProperties_Click;
            // 
            // btnDeals
            // 
            btnDeals.FlatAppearance.BorderSize = 0;
            btnDeals.FlatStyle = FlatStyle.Flat;
            btnDeals.Location = new Point(12, 120);
            btnDeals.Name = "btnDeals";
            btnDeals.Size = new Size(196, 30);
            btnDeals.TabIndex = 3;
            btnDeals.Text = "Deals";
            btnDeals.TextAlign = ContentAlignment.MiddleLeft;
            btnDeals.UseVisualStyleBackColor = true;
            btnDeals.Click += btnDeals_Click;
            // 
            // btnContacts
            // 
            btnContacts.FlatAppearance.BorderSize = 0;
            btnContacts.FlatStyle = FlatStyle.Flat;
            btnContacts.Location = new Point(12, 84);
            btnContacts.Name = "btnContacts";
            btnContacts.Size = new Size(196, 30);
            btnContacts.TabIndex = 2;
            btnContacts.Text = "Contacts";
            btnContacts.TextAlign = ContentAlignment.MiddleLeft;
            btnContacts.UseVisualStyleBackColor = true;
            btnContacts.Click += btnContacts_Click;
            // 
            // btnLeads
            // 
            btnLeads.FlatAppearance.BorderSize = 0;
            btnLeads.FlatStyle = FlatStyle.Flat;
            btnLeads.Location = new Point(12, 48);
            btnLeads.Name = "btnLeads";
            btnLeads.Size = new Size(196, 30);
            btnLeads.TabIndex = 1;
            btnLeads.Text = "Leads";
            btnLeads.TextAlign = ContentAlignment.MiddleLeft;
            btnLeads.UseVisualStyleBackColor = true;
            btnLeads.Click += btnLeads_Click;
            // 
            // btnDashboard
            // 
            btnDashboard.FlatAppearance.BorderSize = 0;
            btnDashboard.FlatStyle = FlatStyle.Flat;
            btnDashboard.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDashboard.Location = new Point(12, 12);
            btnDashboard.Name = "btnDashboard";
            btnDashboard.Size = new Size(196, 30);
            btnDashboard.TabIndex = 0;
            btnDashboard.Text = "Dashboard";
            btnDashboard.TextAlign = ContentAlignment.MiddleLeft;
            btnDashboard.UseVisualStyleBackColor = true;
            btnDashboard.Click += btnDashboard_Click;
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.FromArgb(248, 249, 250);
            // place a thin rule at the left edge of the content area so it's flush with the sidebar
            panelContent.Controls.Add(dataGridView1);
            panelContent.Controls.Add(btnLoadUsers);
            panelContent.Controls.Add(lblSectionTitle);
            panelContent.Controls.Add(headerPanel);
            panelContent.Controls.Add(contentLeftBorder);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(220, 0);
            panelContent.Name = "panelContent";
            // no outer padding so header sits flush to the sidebar seam
            panelContent.Padding = new Padding(0);
            panelContent.Size = new Size(980, 800);
            panelContent.TabIndex = 1;
            // 
            // contentLeftBorder
            // 
            contentLeftBorder.BackColor = Color.Black;
            contentLeftBorder.Dock = DockStyle.Left;
            contentLeftBorder.Location = new Point(0, 0);
            contentLeftBorder.Name = "contentLeftBorder";
            contentLeftBorder.Size = new Size(1, 800);
            contentLeftBorder.TabIndex = 9;
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(20, 108);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(940, 572);
            dataGridView1.TabIndex = 2;
            // 
            // btnLoadUsers
            // 
            btnLoadUsers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLoadUsers.BackColor = Color.FromArgb(0, 123, 255);
            btnLoadUsers.FlatStyle = FlatStyle.Flat;
            btnLoadUsers.ForeColor = Color.White;
            btnLoadUsers.Location = new Point(20, 750);
            btnLoadUsers.Name = "btnLoadUsers";
            btnLoadUsers.Size = new Size(120, 35);
            btnLoadUsers.TabIndex = 3;
            btnLoadUsers.Text = "Load Users";
            btnLoadUsers.UseVisualStyleBackColor = false;
            btnLoadUsers.Click += button1_Click;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.White;
            headerPanel.Controls.Add(panelUser);
            headerPanel.Controls.Add(headerBottomBorder);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            // keep horizontal padding for internal header content
            headerPanel.Padding = new Padding(0, 0, 20, 0);
            headerPanel.Size = new Size(980, 58);
            headerPanel.TabIndex = 0;
            headerPanel.Paint += headerPanel_Paint;
            // 
            // headerBottomBorder
            // 
            headerBottomBorder.BackColor = Color.Black;
            headerBottomBorder.Dock = DockStyle.Bottom;
            headerBottomBorder.Location = new Point(0, 57);
            headerBottomBorder.Name = "headerBottomBorder";
            headerBottomBorder.Size = new Size(980, 1);
            headerBottomBorder.TabIndex = 3;
            // 
            // lblSectionTitle
            // 
            lblSectionTitle.AutoSize = true;
            lblSectionTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblSectionTitle.ForeColor = Color.FromArgb(33, 37, 41);
            // page title under the top bar (above grid)
            lblSectionTitle.Location = new Point(20, 68);
            lblSectionTitle.Name = "lblSectionTitle";
            lblSectionTitle.Size = new Size(198, 32);
            lblSectionTitle.TabIndex = 0;
            lblSectionTitle.Text = "Properties";
            // 
            // panelUser
            // 
            panelUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panelUser.BackColor = Color.Transparent;
            panelUser.Controls.Add(btnUserMenu);
            panelUser.Controls.Add(lblUserRole);
            panelUser.Controls.Add(lblUserName);
            panelUser.Controls.Add(pbAvatar);
            panelUser.Location = new Point(720, 6);
            panelUser.Name = "panelUser";
            panelUser.Size = new Size(224, 48);
            panelUser.TabIndex = 2;
            // 
            // btnUserMenu
            // 
            btnUserMenu.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUserMenu.FlatAppearance.BorderSize = 0;
            btnUserMenu.FlatStyle = FlatStyle.Flat;
            btnUserMenu.Location = new Point(196, 12);
            btnUserMenu.Name = "btnUserMenu";
            btnUserMenu.Size = new Size(24, 24);
            btnUserMenu.TabIndex = 3;
            btnUserMenu.Text = "▾";
            btnUserMenu.UseVisualStyleBackColor = true;
            btnUserMenu.Click += btnUserMenu_Click;
            // 
            // lblUserRole
            // 
            lblUserRole.AutoSize = true;
            lblUserRole.Font = new Font("Segoe UI", 8F);
            lblUserRole.ForeColor = Color.Gray;
            lblUserRole.Location = new Point(64, 26);
            lblUserRole.Name = "lblUserRole";
            lblUserRole.Size = new Size(41, 13);
            lblUserRole.TabIndex = 2;
            lblUserRole.Text = "Broker";
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUserName.ForeColor = Color.FromArgb(33, 37, 41);
            lblUserName.Location = new Point(64, 8);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(104, 15);
            lblUserName.TabIndex = 1;
            lblUserName.Text = "Ron Vergel Luzon";
            // 
            // pbAvatar
            // 
            pbAvatar.BackColor = Color.LightGray;
            pbAvatar.BorderStyle = BorderStyle.FixedSingle;
            pbAvatar.Location = new Point(12, 6);
            pbAvatar.Name = "pbAvatar";
            pbAvatar.Size = new Size(40, 40);
            pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            pbAvatar.TabIndex = 0;
            pbAvatar.TabStop = false;
            pbAvatar.Click += pbAvatar_Click;
            // 
            // contextMenuStripUser
            // 
            contextMenuStripUser.Items.AddRange(new ToolStripItem[] { logoutToolStripMenuItem });
            contextMenuStripUser.Name = "contextMenuStripUser";
            contextMenuStripUser.Size = new Size(113, 26);
            // 
            // logoutToolStripMenuItem
            // 
            logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            logoutToolStripMenuItem.Size = new Size(112, 22);
            logoutToolStripMenuItem.Text = "Logout";
            logoutToolStripMenuItem.Click += logoutToolStripMenuItem_Click;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            Controls.Add(panelContent);
            Controls.Add(panelSidebar);
            Name = "MainView";
            Size = new Size(1200, 800);
            panelSidebar.ResumeLayout(false);
            panelContent.ResumeLayout(false);
            panelContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            panelUser.ResumeLayout(false);
            panelUser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbAvatar).EndInit();
            contextMenuStripUser.ResumeLayout(false);
            ResumeLayout(false);

        }

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Panel navSeparator;
        private System.Windows.Forms.Button btnProperties;
        private System.Windows.Forms.Button btnDeals;
        private System.Windows.Forms.Button btnContacts;
        private System.Windows.Forms.Button btnLeads;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel contentLeftBorder;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel headerBottomBorder;
        private System.Windows.Forms.Label lblSectionTitle;
        private System.Windows.Forms.Panel panelUser;
        private System.Windows.Forms.Button btnUserMenu;
        private System.Windows.Forms.Label lblUserRole;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.PictureBox pbAvatar;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripUser;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnLoadUsers;
    }
}