namespace RealEstateCRMWinForms.Views
{
    partial class DashboardView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            headerPanel = new Panel();
            lblLastUpdated = new Label();
            lblSubtitle = new Label();
            lblTitle = new Label();
            mainPanel = new Panel();
            cardsPanel = new TableLayoutPanel();
            leadsCard = new Panel();
            lblLeadsIcon = new Label();
            lblLeadsTitle = new Label();
            lblLeadsValue = new Label();
            lblLeadsDesc = new Label();
            lblViewAllLeads = new Label();
            contactsCard = new Panel();
            lblContactsIcon = new Label();
            lblContactsTitle = new Label();
            lblContactsValue = new Label();
            lblContactsDesc = new Label();
            lblViewAllContacts = new Label();
            dealsCard = new Panel();
            lblDealsIcon = new Label();
            lblDealsTitle = new Label();
            lblDealsValue = new Label();
            lblDealsDesc = new Label();
            lblViewAllDeals = new Label();
            propertiesCard = new Panel();
            lblPropertiesIcon = new Label();
            lblPropertiesTitle = new Label();
            lblPropertiesValue = new Label();
            lblPropertiesDesc = new Label();
            lblViewAllProperties = new Label();

            // New stats row (below the main cards)
            statsPanel = new TableLayoutPanel();
            activeListingsCard = new Panel();
            avgDaysCard = new Panel();
            avgPriceCard = new Panel();

            // Recent sections
            recentSectionsPanel = new TableLayoutPanel();
            recentPropertiesCard = new Panel();
            lblRecentPropertiesTitle = new Label();
            lblViewAllRecentProperties = new Label();
            recentPropertiesList = new Panel();
            bottomSectionPanel = new TableLayoutPanel();
            recentDealsCard = new Panel();
            lblRecentDealsTitle = new Label();
            lblViewAllRecentDeals = new Label();
            recentDealsList = new Panel();
            recentContactsCard = new Panel();
            lblRecentContactsTitle = new Label();
            lblViewAllRecentContacts = new Label();
            recentContactsList = new Panel();
            recentLeadsCard = new Panel();
            lblRecentLeadsTitle = new Label();
            lblViewAllRecentLeads = new Label();
            recentLeadsList = new Panel();

            headerPanel.SuspendLayout();
            mainPanel.SuspendLayout();
            cardsPanel.SuspendLayout();
            leadsCard.SuspendLayout();
            contactsCard.SuspendLayout();
            dealsCard.SuspendLayout();
            propertiesCard.SuspendLayout();

            statsPanel.SuspendLayout();
            activeListingsCard.SuspendLayout();
            avgDaysCard.SuspendLayout();
            avgPriceCard.SuspendLayout();

            recentSectionsPanel.SuspendLayout();
            recentPropertiesCard.SuspendLayout();
            bottomSectionPanel.SuspendLayout();
            recentDealsCard.SuspendLayout();
            recentContactsCard.SuspendLayout();
            recentLeadsCard.SuspendLayout();

            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.White;
            headerPanel.Controls.Add(lblLastUpdated);
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Padding = new Padding(30, 25, 30, 25);
            headerPanel.Size = new Size(1400, 90);
            headerPanel.TabIndex = 0;
            // 
            // lblLastUpdated
            // 
            lblLastUpdated.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblLastUpdated.AutoSize = true;
            lblLastUpdated.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLastUpdated.ForeColor = Color.FromArgb(107, 114, 128);
            lblLastUpdated.Location = new Point(800, 35);
            lblLastUpdated.Name = "lblLastUpdated";
            lblLastUpdated.Size = new Size(180, 17);
            lblLastUpdated.TabIndex = 2;
            lblLastUpdated.Text = "Last updated: Sept 09, 2025 00:05";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSubtitle.ForeColor = Color.FromArgb(107, 114, 128);
            lblSubtitle.Location = new Point(33, 55);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(242, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Overview of your real estate business";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.FromArgb(17, 24, 39);
            lblTitle.Location = new Point(30, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(260, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Agency Dashboard";
            // 
            // mainPanel
            // 
            mainPanel.BackColor = Color.FromArgb(249, 250, 251);
            mainPanel.Controls.Add(recentSectionsPanel);
            mainPanel.Controls.Add(statsPanel);
            mainPanel.Controls.Add(cardsPanel);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 90);
            mainPanel.Name = "mainPanel";
            mainPanel.Padding = new Padding(30);
            mainPanel.Size = new Size(1400, 810);
            mainPanel.TabIndex = 1;
            mainPanel.AutoScroll = true;
            // 
            // cardsPanel (TableLayoutPanel for even sizing)
            // 
            cardsPanel.Controls.Add(propertiesCard, 0, 0);
            cardsPanel.Controls.Add(dealsCard, 1, 0);
            cardsPanel.Controls.Add(contactsCard, 2, 0);
            cardsPanel.Controls.Add(leadsCard, 3, 0);
            cardsPanel.Dock = DockStyle.Top;
            cardsPanel.Location = new Point(30, 30);
            cardsPanel.Name = "cardsPanel";
            cardsPanel.Padding = new Padding(0);
            cardsPanel.Size = new Size(1340, 240);
            cardsPanel.TabIndex = 0;
            cardsPanel.ColumnCount = 4;
            cardsPanel.RowCount = 1;
            cardsPanel.ColumnStyles.Clear();
            cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cardsPanel.RowStyles.Clear();
            cardsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            cardsPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            // 
            // propertiesCard
            // 
            propertiesCard.BackColor = Color.White;
            propertiesCard.BorderStyle = BorderStyle.FixedSingle;
            propertiesCard.Controls.Add(lblPropertiesIcon);
            propertiesCard.Controls.Add(lblPropertiesTitle);
            propertiesCard.Controls.Add(lblPropertiesValue);
            propertiesCard.Controls.Add(lblPropertiesDesc);
            propertiesCard.Controls.Add(lblViewAllProperties);
            propertiesCard.Dock = DockStyle.Fill;
            propertiesCard.Margin = new Padding(12);
            propertiesCard.Name = "propertiesCard";
            propertiesCard.Padding = new Padding(28);
            propertiesCard.TabIndex = 0;
            // 
            // lblPropertiesIcon
            // 
            lblPropertiesIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPropertiesIcon.AutoSize = true;
            lblPropertiesIcon.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPropertiesIcon.ForeColor = Color.FromArgb(59, 130, 246);
            lblPropertiesIcon.Location = new Point(propertiesCard.ClientSize.Width - 48, 18);
            lblPropertiesIcon.Name = "lblPropertiesIcon";
            lblPropertiesIcon.Size = new Size(45, 32);
            lblPropertiesIcon.TabIndex = 4;
            lblPropertiesIcon.Text = "🏠";
            // 
            // lblPropertiesTitle
            // 
            lblPropertiesTitle.AutoSize = true;
            lblPropertiesTitle.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPropertiesTitle.ForeColor = Color.FromArgb(29, 78, 216);
            lblPropertiesTitle.Location = new Point(25, 20);
            lblPropertiesTitle.Name = "lblPropertiesTitle";
            lblPropertiesTitle.Size = new Size(120, 20);
            lblPropertiesTitle.TabIndex = 0;
            lblPropertiesTitle.Text = "Total Properties";
            // 
            // lblPropertiesValue
            // 
            lblPropertiesValue.AutoSize = true;
            lblPropertiesValue.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPropertiesValue.ForeColor = Color.FromArgb(29, 78, 216);
            lblPropertiesValue.Location = new Point(25, 56);
            lblPropertiesValue.Name = "lblPropertiesValue";
            lblPropertiesValue.Size = new Size(66, 65);
            lblPropertiesValue.TabIndex = 1;
            lblPropertiesValue.Text = "0";
            lblPropertiesValue.Click += lblPropertiesValue_Click;
            // 
            // lblPropertiesDesc
            // 
            lblPropertiesDesc.AutoSize = true;
            lblPropertiesDesc.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPropertiesDesc.ForeColor = Color.FromArgb(100, 120, 220);
            lblPropertiesDesc.Location = new Point(110, 72);
            lblPropertiesDesc.Name = "lblPropertiesDesc";
            lblPropertiesDesc.Size = new Size(107, 19);
            lblPropertiesDesc.TabIndex = 2;
            lblPropertiesDesc.Text = "0 Active Listings";
            // 
            // lblViewAllProperties
            // 
            lblViewAllProperties.AutoSize = false;
            lblViewAllProperties.Cursor = Cursors.Hand;
            lblViewAllProperties.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblViewAllProperties.ForeColor = Color.FromArgb(29, 78, 216);
            lblViewAllProperties.Dock = DockStyle.Bottom;
            lblViewAllProperties.Height = 22;
            lblViewAllProperties.TextAlign = ContentAlignment.MiddleLeft;
            lblViewAllProperties.Margin = new Padding(0);
            lblViewAllProperties.Name = "lblViewAllProperties";
            lblViewAllProperties.TabIndex = 3;
            lblViewAllProperties.Text = "View All Properties →";
            lblViewAllProperties.Click += LblViewAllProperties_Click;
            // 
            // dealsCard
            // 
            dealsCard.BackColor = Color.White;
            dealsCard.BorderStyle = BorderStyle.FixedSingle;
            dealsCard.Controls.Add(lblDealsIcon);
            dealsCard.Controls.Add(lblDealsTitle);
            dealsCard.Controls.Add(lblDealsValue);
            dealsCard.Controls.Add(lblDealsDesc);
            dealsCard.Controls.Add(lblViewAllDeals);
            dealsCard.Dock = DockStyle.Fill;
            dealsCard.Margin = new Padding(12);
            dealsCard.Name = "dealsCard";
            dealsCard.Padding = new Padding(28);
            dealsCard.TabIndex = 1;
            // 
            // lblDealsIcon
            // 
            lblDealsIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblDealsIcon.AutoSize = true;
            lblDealsIcon.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDealsIcon.ForeColor = Color.FromArgb(16, 185, 129);
            lblDealsIcon.Location = new Point(dealsCard.ClientSize.Width - 48, 18);
            lblDealsIcon.Name = "lblDealsIcon";
            lblDealsIcon.Size = new Size(45, 32);
            lblDealsIcon.TabIndex = 4;
            lblDealsIcon.Text = "💰";
            // 
            // lblDealsTitle
            // 
            lblDealsTitle.AutoSize = true;
            lblDealsTitle.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDealsTitle.ForeColor = Color.FromArgb(6, 78, 59);
            lblDealsTitle.Location = new Point(25, 20);
            lblDealsTitle.Name = "lblDealsTitle";
            lblDealsTitle.Size = new Size(85, 20);
            lblDealsTitle.TabIndex = 0;
            lblDealsTitle.Text = "Total Deals";
            // 
            // lblDealsValue
            // 
            lblDealsValue.AutoSize = true;
            lblDealsValue.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDealsValue.ForeColor = Color.FromArgb(6, 78, 59);
            lblDealsValue.Location = new Point(25, 56);
            lblDealsValue.Name = "lblDealsValue";
            lblDealsValue.Size = new Size(45, 65);
            lblDealsValue.TabIndex = 1;
            lblDealsValue.Text = "0";
            // 
            // lblDealsDesc
            // 
            lblDealsDesc.AutoSize = true;
            lblDealsDesc.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDealsDesc.ForeColor = Color.FromArgb(50, 100, 70);
            lblDealsDesc.Location = new Point(110, 72);
            lblDealsDesc.Name = "lblDealsDesc";
            lblDealsDesc.Size = new Size(137, 19);
            lblDealsDesc.TabIndex = 2;
            lblDealsDesc.Text = "0 Closed | 0 In Progress";
            // 
            // lblViewAllDeals
            // 
            lblViewAllDeals.AutoSize = false;
            lblViewAllDeals.Cursor = Cursors.Hand;
            lblViewAllDeals.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblViewAllDeals.ForeColor = Color.FromArgb(6, 78, 59);
            lblViewAllDeals.Dock = DockStyle.Bottom;
            lblViewAllDeals.Height = 22;
            lblViewAllDeals.TextAlign = ContentAlignment.MiddleLeft;
            lblViewAllDeals.Margin = new Padding(0);
            lblViewAllDeals.Name = "lblViewAllDeals";
            lblViewAllDeals.TabIndex = 3;
            lblViewAllDeals.Text = "View All Deals →";
            lblViewAllDeals.Click += LblViewAllDeals_Click;
            // 
            // contactsCard
            // 
            contactsCard.BackColor = Color.White;
            contactsCard.BorderStyle = BorderStyle.FixedSingle;
            contactsCard.Controls.Add(lblContactsIcon);
            contactsCard.Controls.Add(lblContactsTitle);
            contactsCard.Controls.Add(lblContactsValue);
            contactsCard.Controls.Add(lblContactsDesc);
            contactsCard.Controls.Add(lblViewAllContacts);
            contactsCard.Dock = DockStyle.Fill;
            contactsCard.Margin = new Padding(12);
            contactsCard.Name = "contactsCard";
            contactsCard.Padding = new Padding(28);
            contactsCard.TabIndex = 2;
            // 
            // lblContactsIcon
            // 
            lblContactsIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblContactsIcon.AutoSize = true;
            lblContactsIcon.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblContactsIcon.ForeColor = Color.FromArgb(139, 92, 246);
            lblContactsIcon.Location = new Point(contactsCard.ClientSize.Width - 48, 18);
            lblContactsIcon.Name = "lblContactsIcon";
            lblContactsIcon.Size = new Size(45, 32);
            lblContactsIcon.TabIndex = 4;
            lblContactsIcon.Text = "👥";
            // 
            // lblContactsTitle
            // 
            lblContactsTitle.AutoSize = true;
            lblContactsTitle.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblContactsTitle.ForeColor = Color.FromArgb(88, 28, 135);
            lblContactsTitle.Location = new Point(25, 20);
            lblContactsTitle.Name = "lblContactsTitle";
            lblContactsTitle.Size = new Size(106, 20);
            lblContactsTitle.TabIndex = 0;
            lblContactsTitle.Text = "Total Contacts";
            // 
            // lblContactsValue
            // 
            lblContactsValue.AutoSize = true;
            lblContactsValue.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblContactsValue.ForeColor = Color.FromArgb(88, 28, 135);
            lblContactsValue.Location = new Point(25, 56);
            lblContactsValue.Name = "lblContactsValue";
            lblContactsValue.Size = new Size(45, 65);
            lblContactsValue.TabIndex = 1;
            lblContactsValue.Text = "0";
            // 
            // lblContactsDesc
            // 
            lblContactsDesc.AutoSize = true;
            lblContactsDesc.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblContactsDesc.ForeColor = Color.FromArgb(90, 30, 140);
            lblContactsDesc.Location = new Point(110, 72);
            lblContactsDesc.Name = "lblContactsDesc";
            lblContactsDesc.Size = new Size(117, 19);
            lblContactsDesc.TabIndex = 2;
            lblContactsDesc.Text = "0 Agents | 0 Clients";
            // 
            // lblViewAllContacts
            // 
            lblViewAllContacts.AutoSize = false;
            lblViewAllContacts.Cursor = Cursors.Hand;
            lblViewAllContacts.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblViewAllContacts.ForeColor = Color.FromArgb(88, 28, 135);
            lblViewAllContacts.Dock = DockStyle.Bottom;
            lblViewAllContacts.Height = 22;
            lblViewAllContacts.TextAlign = ContentAlignment.MiddleLeft;
            lblViewAllContacts.Margin = new Padding(0);
            lblViewAllContacts.Name = "lblViewAllContacts";
            lblViewAllContacts.TabIndex = 3;
            lblViewAllContacts.Text = "View All Contacts →";
            lblViewAllContacts.Click += LblViewAllContacts_Click;
            // 
            // leadsCard
            // 
            leadsCard.BackColor = Color.White;
            leadsCard.BorderStyle = BorderStyle.FixedSingle;
            leadsCard.Controls.Add(lblLeadsIcon);
            leadsCard.Controls.Add(lblLeadsTitle);
            leadsCard.Controls.Add(lblLeadsValue);
            leadsCard.Controls.Add(lblLeadsDesc);
            leadsCard.Controls.Add(lblViewAllLeads);
            leadsCard.Dock = DockStyle.Fill;
            leadsCard.Margin = new Padding(12);
            leadsCard.Name = "leadsCard";
            leadsCard.Padding = new Padding(28);
            leadsCard.TabIndex = 3;
            // 
            // lblLeadsIcon
            // 
            lblLeadsIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblLeadsIcon.AutoSize = true;
            lblLeadsIcon.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLeadsIcon.ForeColor = Color.FromArgb(245, 158, 11);
            lblLeadsIcon.Location = new Point(leadsCard.ClientSize.Width - 48, 18);
            lblLeadsIcon.Name = "lblLeadsIcon";
            lblLeadsIcon.Size = new Size(45, 32);
            lblLeadsIcon.TabIndex = 4;
            lblLeadsIcon.Text = "⚡";
            // 
            // lblLeadsTitle
            // 
            lblLeadsTitle.AutoSize = true;
            lblLeadsTitle.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLeadsTitle.ForeColor = Color.FromArgb(120, 53, 15);
            lblLeadsTitle.Location = new Point(25, 20);
            lblLeadsTitle.Name = "lblLeadsTitle";
            lblLeadsTitle.Size = new Size(87, 20);
            lblLeadsTitle.TabIndex = 0;
            lblLeadsTitle.Text = "Total Leads";
            // 
            // lblLeadsValue
            // 
            lblLeadsValue.AutoSize = true;
            lblLeadsValue.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLeadsValue.ForeColor = Color.FromArgb(120, 53, 15);
            lblLeadsValue.Location = new Point(25, 56);
            lblLeadsValue.Name = "lblLeadsValue";
            lblLeadsValue.Size = new Size(45, 65);
            lblLeadsValue.TabIndex = 1;
            lblLeadsValue.Text = "3";
            // 
            // lblLeadsDesc
            // 
            lblLeadsDesc.AutoSize = true;
            lblLeadsDesc.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLeadsDesc.ForeColor = Color.FromArgb(120, 53, 15);
            lblLeadsDesc.Location = new Point(110, 72);
            lblLeadsDesc.Name = "lblLeadsDesc";
            lblLeadsDesc.Size = new Size(104, 19);
            lblLeadsDesc.TabIndex = 2;
            lblLeadsDesc.Text = "Active Prospects";
            // 
            // lblViewAllLeads
            // 
            lblViewAllLeads.AutoSize = false;
            lblViewAllLeads.Cursor = Cursors.Hand;
            lblViewAllLeads.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblViewAllLeads.ForeColor = Color.FromArgb(120, 53, 15);
            lblViewAllLeads.Dock = DockStyle.Bottom;
            lblViewAllLeads.Height = 22;
            lblViewAllLeads.TextAlign = ContentAlignment.MiddleLeft;
            lblViewAllLeads.Margin = new Padding(0);
            lblViewAllLeads.Name = "lblViewAllLeads";
            lblViewAllLeads.TabIndex = 3;
            lblViewAllLeads.Text = "View All Leads →";
            lblViewAllLeads.Click += LblViewAllLeads_Click;

            // -----------------------------
            // Stats row: statsPanel (3 equal columns)
            // -----------------------------
            statsPanel.Dock = DockStyle.Top;
            statsPanel.Location = new Point(30, 30 + cardsPanel.Height + 12);
            statsPanel.Name = "statsPanel";
            statsPanel.Size = new Size(1340, 120);
            statsPanel.TabIndex = 10;
            statsPanel.ColumnCount = 3;
            statsPanel.RowCount = 1;
            statsPanel.ColumnStyles.Clear();
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            statsPanel.RowStyles.Clear();
            statsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            statsPanel.Padding = new Padding(0);
            statsPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

            // activeListingsCard - only basic setup, labels will be added at runtime
            activeListingsCard.BackColor = Color.White;
            activeListingsCard.BorderStyle = BorderStyle.FixedSingle;
            activeListingsCard.Dock = DockStyle.Fill;
            activeListingsCard.Margin = new Padding(12);
            activeListingsCard.Padding = new Padding(20);
            activeListingsCard.Name = "activeListingsCard";
            activeListingsCard.TabIndex = 11;

            // avgDaysCard - only basic setup, labels will be added at runtime
            avgDaysCard.BackColor = Color.White;
            avgDaysCard.BorderStyle = BorderStyle.FixedSingle;
            avgDaysCard.Dock = DockStyle.Fill;
            avgDaysCard.Margin = new Padding(12);
            avgDaysCard.Padding = new Padding(20);
            avgDaysCard.Name = "avgDaysCard";
            avgDaysCard.TabIndex = 12;

            // avgPriceCard - only basic setup, labels will be added at runtime
            avgPriceCard.BackColor = Color.White;
            avgPriceCard.BorderStyle = BorderStyle.FixedSingle;
            avgPriceCard.Dock = DockStyle.Fill;
            avgPriceCard.Margin = new Padding(12);
            avgPriceCard.Padding = new Padding(20);
            avgPriceCard.Name = "avgPriceCard";
            avgPriceCard.TabIndex = 13;

            statsPanel.Controls.Add(activeListingsCard, 0, 0);
            statsPanel.Controls.Add(avgDaysCard, 1, 0);
            statsPanel.Controls.Add(avgPriceCard, 2, 0);

            // -----------------------------
            // Recent Sections Panel - Modified Layout
            // -----------------------------
            recentSectionsPanel.Dock = DockStyle.Top;
            recentSectionsPanel.Location = new Point(30, 30 + cardsPanel.Height + statsPanel.Height + 24);
            recentSectionsPanel.Name = "recentSectionsPanel";
            recentSectionsPanel.Size = new Size(1340, 600);
            recentSectionsPanel.TabIndex = 20;
            recentSectionsPanel.ColumnCount = 1;
            recentSectionsPanel.RowCount = 2;
            recentSectionsPanel.ColumnStyles.Clear();
            recentSectionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            recentSectionsPanel.RowStyles.Clear();
            recentSectionsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 250F));
            recentSectionsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            recentSectionsPanel.Padding = new Padding(0);
            recentSectionsPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

            // Recent Properties Card (increase top padding to make space for header)
            recentPropertiesCard.BackColor = Color.White;
            recentPropertiesCard.BorderStyle = BorderStyle.FixedSingle;
            recentPropertiesCard.Dock = DockStyle.Fill;
            recentPropertiesCard.Margin = new Padding(12);
            recentPropertiesCard.Padding = new Padding(20, 44, 20, 20);
            recentPropertiesCard.Name = "recentPropertiesCard";
            recentPropertiesCard.TabIndex = 21;

            lblRecentPropertiesTitle.AutoSize = true;
            lblRecentPropertiesTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRecentPropertiesTitle.ForeColor = Color.FromArgb(17, 24, 39);
            lblRecentPropertiesTitle.Text = "Recent Properties";
            lblRecentPropertiesTitle.Location = new Point(0, 0);

            lblViewAllRecentProperties.AutoSize = true;
            lblViewAllRecentProperties.Font = new Font("Segoe UI", 10F);
            lblViewAllRecentProperties.ForeColor = Color.FromArgb(59, 130, 246);
            lblViewAllRecentProperties.Text = "View All";
            lblViewAllRecentProperties.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblViewAllRecentProperties.Cursor = Cursors.Hand;
            lblViewAllRecentProperties.Click += LblViewAllProperties_Click;

            recentPropertiesList.Dock = DockStyle.Fill;
            recentPropertiesList.Margin = new Padding(0);
            recentPropertiesList.AutoScroll = true;
            recentPropertiesList.BackColor = Color.FromArgb(249, 250, 251);

            recentPropertiesCard.Controls.Add(lblRecentPropertiesTitle);
            recentPropertiesCard.Controls.Add(lblViewAllRecentProperties);
            recentPropertiesCard.Controls.Add(recentPropertiesList);

            // bottomSectionPanel
            bottomSectionPanel.Dock = DockStyle.Fill;
            bottomSectionPanel.ColumnCount = 3;
            bottomSectionPanel.RowCount = 1;
            bottomSectionPanel.Name = "bottomSectionPanel";
            bottomSectionPanel.TabIndex = 25;
            bottomSectionPanel.ColumnStyles.Clear();
            bottomSectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            bottomSectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            bottomSectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            bottomSectionPanel.RowStyles.Clear();
            bottomSectionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            bottomSectionPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            bottomSectionPanel.Margin = new Padding(0);

            // Recent Deals Card (padding for header)
            recentDealsCard.BackColor = Color.White;
            recentDealsCard.BorderStyle = BorderStyle.FixedSingle;
            recentDealsCard.Dock = DockStyle.Fill;
            recentDealsCard.Margin = new Padding(12);
            recentDealsCard.Padding = new Padding(20, 44, 20, 20);
            recentDealsCard.Name = "recentDealsCard";
            recentDealsCard.TabIndex = 22;

            lblRecentDealsTitle.AutoSize = true;
            lblRecentDealsTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRecentDealsTitle.ForeColor = Color.FromArgb(17, 24, 39);
            lblRecentDealsTitle.Text = "Recent Deals";
            lblRecentDealsTitle.Location = new Point(0, 0);

            lblViewAllRecentDeals.AutoSize = true;
            lblViewAllRecentDeals.Font = new Font("Segoe UI", 10F);
            lblViewAllRecentDeals.ForeColor = Color.FromArgb(59, 130, 246);
            lblViewAllRecentDeals.Text = "View All";
            lblViewAllRecentDeals.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblViewAllRecentDeals.Cursor = Cursors.Hand;
            lblViewAllRecentDeals.Click += LblViewAllDeals_Click;

            recentDealsList.Dock = DockStyle.Fill;
            recentDealsList.Margin = new Padding(0);
            recentDealsList.AutoScroll = true;
            recentDealsList.BackColor = Color.FromArgb(249, 250, 251);

            recentDealsCard.Controls.Add(lblRecentDealsTitle);
            recentDealsCard.Controls.Add(lblViewAllRecentDeals);
            recentDealsCard.Controls.Add(recentDealsList);

            // Recent Contacts Card (padding for header)
            recentContactsCard.BackColor = Color.White;
            recentContactsCard.BorderStyle = BorderStyle.FixedSingle;
            recentContactsCard.Dock = DockStyle.Fill;
            recentContactsCard.Margin = new Padding(12);
            recentContactsCard.Padding = new Padding(20, 44, 20, 20);
            recentContactsCard.Name = "recentContactsCard";
            recentContactsCard.TabIndex = 23;

            lblRecentContactsTitle.AutoSize = true;
            lblRecentContactsTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRecentContactsTitle.ForeColor = Color.FromArgb(17, 24, 39);
            lblRecentContactsTitle.Text = "Recent Contacts";
            lblRecentContactsTitle.Location = new Point(0, 0);

            lblViewAllRecentContacts.AutoSize = true;
            lblViewAllRecentContacts.Font = new Font("Segoe UI", 10F);
            lblViewAllRecentContacts.ForeColor = Color.FromArgb(59, 130, 246);
            lblViewAllRecentContacts.Text = "View All";
            lblViewAllRecentContacts.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblViewAllRecentContacts.Cursor = Cursors.Hand;
            lblViewAllRecentContacts.Click += LblViewAllContacts_Click;

            recentContactsList.Dock = DockStyle.Fill;
            recentContactsList.Margin = new Padding(0);
            recentContactsList.AutoScroll = true;
            recentContactsList.BackColor = Color.FromArgb(249, 250, 251);

            recentContactsCard.Controls.Add(lblRecentContactsTitle);
            recentContactsCard.Controls.Add(lblViewAllRecentContacts);
            recentContactsCard.Controls.Add(recentContactsList);

            // Recent Leads Card (padding for header)
            recentLeadsCard.BackColor = Color.White;
            recentLeadsCard.BorderStyle = BorderStyle.FixedSingle;
            recentLeadsCard.Dock = DockStyle.Fill;
            recentLeadsCard.Margin = new Padding(12);
            recentLeadsCard.Padding = new Padding(20, 44, 20, 20);
            recentLeadsCard.Name = "recentLeadsCard";
            recentLeadsCard.TabIndex = 24;

            lblRecentLeadsTitle.AutoSize = true;
            lblRecentLeadsTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblRecentLeadsTitle.ForeColor = Color.FromArgb(17, 24, 39);
            lblRecentLeadsTitle.Text = "Recent Leads";
            lblRecentLeadsTitle.Location = new Point(0, 0);

            lblViewAllRecentLeads.AutoSize = true;
            lblViewAllRecentLeads.Font = new Font("Segoe UI", 10F);
            lblViewAllRecentLeads.ForeColor = Color.FromArgb(59, 130, 246);
            lblViewAllRecentLeads.Text = "View All";
            lblViewAllRecentLeads.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblViewAllRecentLeads.Cursor = Cursors.Hand;
            lblViewAllRecentLeads.Click += LblViewAllLeads_Click;

            recentLeadsList.Dock = DockStyle.Fill;
            recentLeadsList.Margin = new Padding(0);
            recentLeadsList.AutoScroll = true;
            recentLeadsList.BackColor = Color.FromArgb(249, 250, 251);

            recentLeadsCard.Controls.Add(lblRecentLeadsTitle);
            recentLeadsCard.Controls.Add(lblViewAllRecentLeads);
            recentLeadsCard.Controls.Add(recentLeadsList);

            // Add other cards to bottom section
            bottomSectionPanel.Controls.Add(recentDealsCard, 0, 0);
            bottomSectionPanel.Controls.Add(recentContactsCard, 1, 0);
            bottomSectionPanel.Controls.Add(recentLeadsCard, 2, 0);

            recentSectionsPanel.Controls.Add(recentPropertiesCard, 0, 0);
            recentSectionsPanel.Controls.Add(bottomSectionPanel, 0, 1);

            // 
            // DashboardView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(249, 250, 251);
            Controls.Add(mainPanel);
            Controls.Add(headerPanel);
            Name = "DashboardView";
            Size = new Size(1400, 900);

            // Resume layout for all panels
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            mainPanel.ResumeLayout(false);
            cardsPanel.ResumeLayout(false);
            recentSectionsPanel.ResumeLayout(false);
            bottomSectionPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblLastUpdated;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.TableLayoutPanel cardsPanel;
        private System.Windows.Forms.Panel propertiesCard;
        private System.Windows.Forms.Label lblPropertiesTitle;
        private System.Windows.Forms.Label lblPropertiesValue;
        private System.Windows.Forms.Label lblPropertiesDesc;
        private System.Windows.Forms.Label lblViewAllProperties;
        private System.Windows.Forms.Label lblPropertiesIcon;
        private System.Windows.Forms.Panel dealsCard;
        private System.Windows.Forms.Label lblDealsIcon;
        private System.Windows.Forms.Label lblDealsTitle;
        private System.Windows.Forms.Label lblDealsValue;
        private System.Windows.Forms.Label lblDealsDesc;
        private System.Windows.Forms.Label lblViewAllDeals;
        private System.Windows.Forms.Panel contactsCard;
        private System.Windows.Forms.Label lblContactsIcon;
        private System.Windows.Forms.Label lblContactsTitle;
        private System.Windows.Forms.Label lblContactsValue;
        private System.Windows.Forms.Label lblContactsDesc;
        private System.Windows.Forms.Label lblViewAllContacts;
        private System.Windows.Forms.Panel leadsCard;
        private System.Windows.Forms.Label lblLeadsIcon;
        private System.Windows.Forms.Label lblLeadsTitle;
        private System.Windows.Forms.Label lblLeadsValue;
        private System.Windows.Forms.Label lblLeadsDesc;
        private System.Windows.Forms.Label lblViewAllLeads;

        // new stats controls - only panels declared, labels created at runtime
        private System.Windows.Forms.TableLayoutPanel statsPanel;
        private System.Windows.Forms.Panel activeListingsCard;
        private System.Windows.Forms.Panel avgDaysCard;
        private System.Windows.Forms.Panel avgPriceCard;

        // New recent sections controls
        private System.Windows.Forms.TableLayoutPanel recentSectionsPanel;
        private System.Windows.Forms.Panel recentPropertiesCard;
        private System.Windows.Forms.Label lblRecentPropertiesTitle;
        private System.Windows.Forms.Label lblViewAllRecentProperties;
        private System.Windows.Forms.Panel recentPropertiesList;
        private System.Windows.Forms.Panel recentDealsCard;
        private System.Windows.Forms.Label lblRecentDealsTitle;
        private System.Windows.Forms.Label lblViewAllRecentDeals;
        private System.Windows.Forms.Panel recentDealsList;
        private System.Windows.Forms.Panel recentContactsCard;
        private System.Windows.Forms.Label lblRecentContactsTitle;
        private System.Windows.Forms.Label lblViewAllRecentContacts;
        private System.Windows.Forms.Panel recentContactsList;
        private System.Windows.Forms.Panel recentLeadsCard;
        private System.Windows.Forms.Label lblRecentLeadsTitle;
        private System.Windows.Forms.Label lblViewAllRecentLeads;
        private System.Windows.Forms.Panel recentLeadsList;
        
        private System.Windows.Forms.TableLayoutPanel bottomSectionPanel;
    }
}