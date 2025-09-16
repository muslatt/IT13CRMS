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
            cardsPanel = new Panel();
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
            headerPanel.SuspendLayout();
            mainPanel.SuspendLayout();
            cardsPanel.SuspendLayout();
            leadsCard.SuspendLayout();
            contactsCard.SuspendLayout();
            dealsCard.SuspendLayout();
            propertiesCard.SuspendLayout();
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
            headerPanel.Size = new Size(1000, 90);
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
            mainPanel.Controls.Add(cardsPanel);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 90);
            mainPanel.Name = "mainPanel";
            mainPanel.Padding = new Padding(30);
            mainPanel.Size = new Size(1000, 580);
            mainPanel.TabIndex = 1;
            // 
            // cardsPanel
            // 
            cardsPanel.Controls.Add(leadsCard);
            cardsPanel.Controls.Add(contactsCard);
            cardsPanel.Controls.Add(dealsCard);
            cardsPanel.Controls.Add(propertiesCard);
            cardsPanel.Dock = DockStyle.Top;
            cardsPanel.Location = new Point(30, 30);
            cardsPanel.Name = "cardsPanel";
            cardsPanel.Size = new Size(940, 260);
            cardsPanel.TabIndex = 0;
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
            leadsCard.Location = new Point(705, 0);
            leadsCard.Name = "leadsCard";
            leadsCard.Padding = new Padding(25);
            leadsCard.Size = new Size(220, 240);
            leadsCard.TabIndex = 3;
            // 
            // lblLeadsIcon
            // 
            lblLeadsIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblLeadsIcon.AutoSize = true;
            lblLeadsIcon.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLeadsIcon.ForeColor = Color.FromArgb(245, 158, 11);
            lblLeadsIcon.Location = new Point(165, 25);
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
            lblLeadsTitle.Location = new Point(25, 25);
            lblLeadsTitle.Name = "lblLeadsTitle";
            lblLeadsTitle.Size = new Size(87, 20);
            lblLeadsTitle.TabIndex = 0;
            lblLeadsTitle.Text = "Total Leads";
            // 
            // lblLeadsValue
            // 
            lblLeadsValue.AutoSize = true;
            lblLeadsValue.Font = new Font("Segoe UI", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLeadsValue.ForeColor = Color.FromArgb(120, 53, 15);
            lblLeadsValue.Location = new Point(25, 60);
            lblLeadsValue.Name = "lblLeadsValue";
            lblLeadsValue.Size = new Size(45, 50);
            lblLeadsValue.TabIndex = 1;
            lblLeadsValue.Text = "3";
            // 
            // lblLeadsDesc
            // 
            lblLeadsDesc.AutoSize = true;
            lblLeadsDesc.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLeadsDesc.ForeColor = Color.FromArgb(120, 53, 15);
            lblLeadsDesc.Location = new Point(70, 80);
            lblLeadsDesc.Name = "lblLeadsDesc";
            lblLeadsDesc.Size = new Size(104, 17);
            lblLeadsDesc.TabIndex = 2;
            lblLeadsDesc.Text = "Active Prospects";
            // 
            // lblViewAllLeads
            // 
            lblViewAllLeads.AutoSize = true;
            lblViewAllLeads.Cursor = Cursors.Hand;
            lblViewAllLeads.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblViewAllLeads.ForeColor = Color.FromArgb(120, 53, 15);
            lblViewAllLeads.Location = new Point(25, 200);
            lblViewAllLeads.Name = "lblViewAllLeads";
            lblViewAllLeads.Size = new Size(102, 17);
            lblViewAllLeads.TabIndex = 3;
            lblViewAllLeads.Text = "View All Leads →";
            lblViewAllLeads.Click += LblViewAllLeads_Click;
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
            contactsCard.Location = new Point(470, 0);
            contactsCard.Name = "contactsCard";
            contactsCard.Padding = new Padding(25);
            contactsCard.Size = new Size(220, 240);
            contactsCard.TabIndex = 2;
            // 
            // lblContactsIcon
            // 
            lblContactsIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblContactsIcon.AutoSize = true;
            lblContactsIcon.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblContactsIcon.ForeColor = Color.FromArgb(139, 92, 246);
            lblContactsIcon.Location = new Point(165, 25);
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
            lblContactsTitle.Location = new Point(25, 25);
            lblContactsTitle.Name = "lblContactsTitle";
            lblContactsTitle.Size = new Size(106, 20);
            lblContactsTitle.TabIndex = 0;
            lblContactsTitle.Text = "Total Contacts";
            // 
            // lblContactsValue
            // 
            lblContactsValue.AutoSize = true;
            lblContactsValue.Font = new Font("Segoe UI", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblContactsValue.ForeColor = Color.FromArgb(88, 28, 135);
            lblContactsValue.Location = new Point(25, 60);
            lblContactsValue.Name = "lblContactsValue";
            lblContactsValue.Size = new Size(45, 50);
            lblContactsValue.TabIndex = 1;
            lblContactsValue.Text = "4";
            // 
            // lblContactsDesc
            // 
            lblContactsDesc.AutoSize = true;
            lblContactsDesc.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblContactsDesc.ForeColor = Color.FromArgb(88, 28, 135);
            lblContactsDesc.Location = new Point(70, 80);
            lblContactsDesc.Name = "lblContactsDesc";
            lblContactsDesc.Size = new Size(117, 17);
            lblContactsDesc.TabIndex = 2;
            lblContactsDesc.Text = "4 Agents | 0 Clients";
            // 
            // lblViewAllContacts
            // 
            lblViewAllContacts.AutoSize = true;
            lblViewAllContacts.Cursor = Cursors.Hand;
            lblViewAllContacts.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblViewAllContacts.ForeColor = Color.FromArgb(88, 28, 135);
            lblViewAllContacts.Location = new Point(25, 200);
            lblViewAllContacts.Name = "lblViewAllContacts";
            lblViewAllContacts.Size = new Size(119, 17);
            lblViewAllContacts.TabIndex = 3;
            lblViewAllContacts.Text = "View All Contacts →";
            lblViewAllContacts.Click += LblViewAllContacts_Click;
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
            dealsCard.Location = new Point(235, 0);
            dealsCard.Name = "dealsCard";
            dealsCard.Padding = new Padding(25);
            dealsCard.Size = new Size(220, 240);
            dealsCard.TabIndex = 1;
            // 
            // lblDealsIcon
            // 
            lblDealsIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblDealsIcon.AutoSize = true;
            lblDealsIcon.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDealsIcon.ForeColor = Color.FromArgb(16, 185, 129);
            lblDealsIcon.Location = new Point(165, 25);
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
            lblDealsTitle.Location = new Point(25, 25);
            lblDealsTitle.Name = "lblDealsTitle";
            lblDealsTitle.Size = new Size(85, 20);
            lblDealsTitle.TabIndex = 0;
            lblDealsTitle.Text = "Total Deals";
            // 
            // lblDealsValue
            // 
            lblDealsValue.AutoSize = true;
            lblDealsValue.Font = new Font("Segoe UI", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDealsValue.ForeColor = Color.FromArgb(6, 78, 59);
            lblDealsValue.Location = new Point(25, 60);
            lblDealsValue.Name = "lblDealsValue";
            lblDealsValue.Size = new Size(45, 50);
            lblDealsValue.TabIndex = 1;
            lblDealsValue.Text = "4";
            // 
            // lblDealsDesc
            // 
            lblDealsDesc.AutoSize = true;
            lblDealsDesc.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDealsDesc.ForeColor = Color.FromArgb(6, 78, 59);
            lblDealsDesc.Location = new Point(70, 80);
            lblDealsDesc.Name = "lblDealsDesc";
            lblDealsDesc.Size = new Size(137, 17);
            lblDealsDesc.TabIndex = 2;
            lblDealsDesc.Text = "0 Closed | 0 In Progress";
            // 
            // lblViewAllDeals
            // 
            lblViewAllDeals.AutoSize = true;
            lblViewAllDeals.Cursor = Cursors.Hand;
            lblViewAllDeals.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblViewAllDeals.ForeColor = Color.FromArgb(6, 78, 59);
            lblViewAllDeals.Location = new Point(25, 200);
            lblViewAllDeals.Name = "lblViewAllDeals";
            lblViewAllDeals.Size = new Size(100, 17);
            lblViewAllDeals.TabIndex = 3;
            lblViewAllDeals.Text = "View All Deals →";
            lblViewAllDeals.Click += LblViewAllDeals_Click;
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
            propertiesCard.Location = new Point(0, 0);
            propertiesCard.Name = "propertiesCard";
            propertiesCard.Padding = new Padding(25);
            propertiesCard.Size = new Size(220, 240);
            propertiesCard.TabIndex = 0;
            // 
            // lblPropertiesIcon
            // 
            lblPropertiesIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPropertiesIcon.AutoSize = true;
            lblPropertiesIcon.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPropertiesIcon.ForeColor = Color.FromArgb(59, 130, 246);
            lblPropertiesIcon.Location = new Point(165, 25);
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
            lblPropertiesTitle.Location = new Point(25, 25);
            lblPropertiesTitle.Name = "lblPropertiesTitle";
            lblPropertiesTitle.Size = new Size(120, 20);
            lblPropertiesTitle.TabIndex = 0;
            lblPropertiesTitle.Text = "Total Properties";
            // 
            // lblPropertiesValue
            // 
            lblPropertiesValue.AutoSize = true;
            lblPropertiesValue.Font = new Font("Segoe UI", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPropertiesValue.ForeColor = Color.FromArgb(29, 78, 216);
            lblPropertiesValue.Location = new Point(25, 60);
            lblPropertiesValue.Name = "lblPropertiesValue";
            lblPropertiesValue.Size = new Size(66, 50);
            lblPropertiesValue.TabIndex = 1;
            lblPropertiesValue.Text = "10";
            lblPropertiesValue.Click += lblPropertiesValue_Click;
            // 
            // lblPropertiesDesc
            // 
            lblPropertiesDesc.AutoSize = true;
            lblPropertiesDesc.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPropertiesDesc.ForeColor = Color.FromArgb(29, 78, 216);
            lblPropertiesDesc.Location = new Point(90, 80);
            lblPropertiesDesc.Name = "lblPropertiesDesc";
            lblPropertiesDesc.Size = new Size(107, 17);
            lblPropertiesDesc.TabIndex = 2;
            lblPropertiesDesc.Text = "10 Active Listings";
            // 
            // lblViewAllProperties
            // 
            lblViewAllProperties.AutoSize = true;
            lblViewAllProperties.Cursor = Cursors.Hand;
            lblViewAllProperties.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblViewAllProperties.ForeColor = Color.FromArgb(29, 78, 216);
            lblViewAllProperties.Location = new Point(25, 200);
            lblViewAllProperties.Name = "lblViewAllProperties";
            lblViewAllProperties.Size = new Size(125, 17);
            lblViewAllProperties.TabIndex = 3;
            lblViewAllProperties.Text = "View All Properties →";
            lblViewAllProperties.Click += LblViewAllProperties_Click;
            // 
            // DashboardView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(249, 250, 251);
            Controls.Add(mainPanel);
            Controls.Add(headerPanel);
            Name = "DashboardView";
            Size = new Size(1000, 670);
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            mainPanel.ResumeLayout(false);
            cardsPanel.ResumeLayout(false);
            leadsCard.ResumeLayout(false);
            leadsCard.PerformLayout();
            contactsCard.ResumeLayout(false);
            contactsCard.PerformLayout();
            dealsCard.ResumeLayout(false);
            dealsCard.PerformLayout();
            propertiesCard.ResumeLayout(false);
            propertiesCard.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblLastUpdated;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel cardsPanel;
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
    }
}