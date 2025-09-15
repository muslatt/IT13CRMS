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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblLastUpdated = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.cardsPanel = new System.Windows.Forms.Panel();
            this.leadsCard = new System.Windows.Forms.Panel();
            this.lblLeadsIcon = new System.Windows.Forms.Label();
            this.lblLeadsTitle = new System.Windows.Forms.Label();
            this.lblLeadsValue = new System.Windows.Forms.Label();
            this.lblLeadsDesc = new System.Windows.Forms.Label();
            this.lblViewAllLeads = new System.Windows.Forms.Label();
            this.contactsCard = new System.Windows.Forms.Panel();
            this.lblContactsIcon = new System.Windows.Forms.Label();
            this.lblContactsTitle = new System.Windows.Forms.Label();
            this.lblContactsValue = new System.Windows.Forms.Label();
            this.lblContactsDesc = new System.Windows.Forms.Label();
            this.lblViewAllContacts = new System.Windows.Forms.Label();
            this.dealsCard = new System.Windows.Forms.Panel();
            this.lblDealsIcon = new System.Windows.Forms.Label();
            this.lblDealsTitle = new System.Windows.Forms.Label();
            this.lblDealsValue = new System.Windows.Forms.Label();
            this.lblDealsDesc = new System.Windows.Forms.Label();
            this.lblViewAllDeals = new System.Windows.Forms.Label();
            this.propertiesCard = new System.Windows.Forms.Panel();
            this.lblPropertiesIcon = new System.Windows.Forms.Label();
            this.lblPropertiesTitle = new System.Windows.Forms.Label();
            this.lblPropertiesValue = new System.Windows.Forms.Label();
            this.lblPropertiesDesc = new System.Windows.Forms.Label();
            this.lblViewAllProperties = new System.Windows.Forms.Label();
            this.headerPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.cardsPanel.SuspendLayout();
            this.leadsCard.SuspendLayout();
            this.contactsCard.SuspendLayout();
            this.dealsCard.SuspendLayout();
            this.propertiesCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.lblLastUpdated);
            this.headerPanel.Controls.Add(this.lblSubtitle);
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(20);
            this.headerPanel.Size = new System.Drawing.Size(940, 80);
            this.headerPanel.TabIndex = 0;
            // 
            // lblLastUpdated
            // 
            this.lblLastUpdated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastUpdated.AutoSize = true;
            this.lblLastUpdated.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLastUpdated.ForeColor = System.Drawing.Color.Gray;
            this.lblLastUpdated.Location = new System.Drawing.Point(750, 25);
            this.lblLastUpdated.Name = "lblLastUpdated";
            this.lblLastUpdated.Size = new System.Drawing.Size(179, 15);
            this.lblLastUpdated.TabIndex = 2;
            this.lblLastUpdated.Text = "Last updated: Sept 09, 2025 00:05";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.Gray;
            this.lblSubtitle.Location = new System.Drawing.Point(23, 50);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(237, 19);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Overview of your real estate business";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(230, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Agency Dashboard";
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.mainPanel.Controls.Add(this.cardsPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 80);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(20);
            this.mainPanel.Size = new System.Drawing.Size(940, 520);
            this.mainPanel.TabIndex = 1;
            // 
            // cardsPanel
            // 
            this.cardsPanel.Controls.Add(this.leadsCard);
            this.cardsPanel.Controls.Add(this.contactsCard);
            this.cardsPanel.Controls.Add(this.dealsCard);
            this.cardsPanel.Controls.Add(this.propertiesCard);
            this.cardsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardsPanel.Location = new System.Drawing.Point(20, 20);
            this.cardsPanel.Name = "cardsPanel";
            this.cardsPanel.Size = new System.Drawing.Size(900, 240);
            // 
            // leadsCard
            // 
            this.leadsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            this.leadsCard.Controls.Add(this.lblLeadsIcon);
            this.leadsCard.Controls.Add(this.lblLeadsTitle);
            this.leadsCard.Controls.Add(this.lblLeadsValue);
            this.leadsCard.Controls.Add(this.lblLeadsDesc);
            this.leadsCard.Controls.Add(this.lblViewAllLeads);
            // Position as 4th card - calculate to fill remaining space
            this.leadsCard.Location = new System.Drawing.Point(675, 10);
            this.leadsCard.Margin = new System.Windows.Forms.Padding(3);
            this.leadsCard.Name = "leadsCard";
            this.leadsCard.Padding = new System.Windows.Forms.Padding(20);
            // Make card larger to span to the right edge
            this.leadsCard.Size = new System.Drawing.Size(215, 220);
            this.leadsCard.TabIndex = 3;
            // 
            // lblLeadsIcon
            // 
            this.lblLeadsIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLeadsIcon.AutoSize = true;
            this.lblLeadsIcon.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.lblLeadsIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.lblLeadsIcon.Location = new System.Drawing.Point(157, 20);
            this.lblLeadsIcon.Name = "lblLeadsIcon";
            this.lblLeadsIcon.Size = new System.Drawing.Size(30, 30);
            this.lblLeadsIcon.TabIndex = 4;
            this.lblLeadsIcon.Text = "⚡";
            // 
            // lblLeadsTitle
            // 
            this.lblLeadsTitle.AutoSize = true;
            this.lblLeadsTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblLeadsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(77)))), ((int)(((byte)(14)))));
            this.lblLeadsTitle.Location = new System.Drawing.Point(20, 20);
            this.lblLeadsTitle.Name = "lblLeadsTitle";
            this.lblLeadsTitle.Size = new System.Drawing.Size(84, 19);
            this.lblLeadsTitle.TabIndex = 0;
            this.lblLeadsTitle.Text = "Total Leads";
            // 
            // lblLeadsValue
            // 
            this.lblLeadsValue.AutoSize = true;
            this.lblLeadsValue.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblLeadsValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(77)))), ((int)(((byte)(14)))));
            this.lblLeadsValue.Location = new System.Drawing.Point(20, 50);
            this.lblLeadsValue.Name = "lblLeadsValue";
            this.lblLeadsValue.Size = new System.Drawing.Size(38, 45);
            this.lblLeadsValue.TabIndex = 1;
            this.lblLeadsValue.Text = "3";
            // 
            // lblLeadsDesc
            // 
            this.lblLeadsDesc.AutoSize = true;
            this.lblLeadsDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLeadsDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(77)))), ((int)(((byte)(14)))));
            this.lblLeadsDesc.Location = new System.Drawing.Point(60, 70);
            this.lblLeadsDesc.Name = "lblLeadsDesc";
            this.lblLeadsDesc.Size = new System.Drawing.Size(94, 15);
            this.lblLeadsDesc.TabIndex = 2;
            this.lblLeadsDesc.Text = "Active Prospects";
            // 
            // lblViewAllLeads
            // 
            this.lblViewAllLeads.AutoSize = true;
            this.lblViewAllLeads.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblViewAllLeads.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblViewAllLeads.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(133)))), ((int)(((byte)(77)))), ((int)(((byte)(14)))));
            this.lblViewAllLeads.Location = new System.Drawing.Point(20, 185);
            this.lblViewAllLeads.Name = "lblViewAllLeads";
            this.lblViewAllLeads.Size = new System.Drawing.Size(215, 220);
            this.lblViewAllLeads.TabIndex = 3;
            this.lblViewAllLeads.Text = "View All Leads →";
            this.lblViewAllLeads.Click += new System.EventHandler(this.LblViewAllLeads_Click);
            // 
            // contactsCard
            // 
            this.contactsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(240)))), ((int)(((byte)(252)))));
            this.contactsCard.Controls.Add(this.lblContactsIcon);
            this.contactsCard.Controls.Add(this.lblContactsTitle);
            this.contactsCard.Controls.Add(this.lblContactsValue);
            this.contactsCard.Controls.Add(this.lblContactsDesc);
            this.contactsCard.Controls.Add(this.lblViewAllContacts);
            // Position as 3rd card with better spacing
            this.contactsCard.Location = new System.Drawing.Point(450, 10);
            this.contactsCard.Margin = new System.Windows.Forms.Padding(3);
            this.contactsCard.Name = "contactsCard";
            this.contactsCard.Padding = new System.Windows.Forms.Padding(20);
            this.contactsCard.Size = new System.Drawing.Size(215, 220);
            this.contactsCard.TabIndex = 2;
            // 
            // lblContactsIcon
            // 
            this.lblContactsIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblContactsIcon.AutoSize = true;
            this.lblContactsIcon.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.lblContactsIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(43)))), ((int)(((byte)(226)))));
            this.lblContactsIcon.Location = new System.Drawing.Point(171, 20);
            this.lblContactsIcon.Name = "lblContactsIcon";
            this.lblContactsIcon.Size = new System.Drawing.Size(30, 30);
            this.lblContactsIcon.TabIndex = 4;
            this.lblContactsIcon.Text = "👥";
            // 
            // lblContactsTitle
            // 
            this.lblContactsTitle.AutoSize = true;
            this.lblContactsTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblContactsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(33)))), ((int)(((byte)(168)))));
            this.lblContactsTitle.Location = new System.Drawing.Point(20, 20);
            this.lblContactsTitle.Name = "lblContactsTitle";
            this.lblContactsTitle.Size = new System.Drawing.Size(103, 19);
            this.lblContactsTitle.TabIndex = 0;
            this.lblContactsTitle.Text = "Total Contacts";
            // 
            // lblContactsValue
            // 
            this.lblContactsValue.AutoSize = true;
            this.lblContactsValue.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblContactsValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(33)))), ((int)(((byte)(168)))));
            this.lblContactsValue.Location = new System.Drawing.Point(20, 50);
            this.lblContactsValue.Name = "lblContactsValue";
            this.lblContactsValue.Size = new System.Drawing.Size(38, 45);
            this.lblContactsValue.TabIndex = 1;
            this.lblContactsValue.Text = "4";
            // 
            // lblContactsDesc
            // 
            this.lblContactsDesc.AutoSize = true;
            this.lblContactsDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblContactsDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(33)))), ((int)(((byte)(168)))));
            this.lblContactsDesc.Location = new System.Drawing.Point(60, 70);
            this.lblContactsDesc.Name = "lblContactsDesc";
            this.lblContactsDesc.Size = new System.Drawing.Size(107, 15);
            this.lblContactsDesc.TabIndex = 2;
            this.lblContactsDesc.Text = "4 Agents | 0 Clients";
            // 
            // lblViewAllContacts
            // 
            this.lblViewAllContacts.AutoSize = true;
            this.lblViewAllContacts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblViewAllContacts.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblViewAllContacts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(33)))), ((int)(((byte)(168)))));
            this.lblViewAllContacts.Location = new System.Drawing.Point(20, 185);
            this.lblViewAllContacts.Name = "lblViewAllContacts";
            this.lblViewAllContacts.Size = new System.Drawing.Size(102, 15);
            this.lblViewAllContacts.TabIndex = 3;
            this.lblViewAllContacts.Text = "View All Contacts →";
            this.lblViewAllContacts.Click += new System.EventHandler(this.LblViewAllContacts_Click);
            // 
            // dealsCard
            // 
            this.dealsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(252)))), ((int)(((byte)(231)))));
            this.dealsCard.Controls.Add(this.lblDealsIcon);
            this.dealsCard.Controls.Add(this.lblDealsTitle);
            this.dealsCard.Controls.Add(this.lblDealsValue);
            this.dealsCard.Controls.Add(this.lblDealsDesc);
            this.dealsCard.Controls.Add(this.lblViewAllDeals);
            // Position as 2nd card with better spacing
            this.dealsCard.Location = new System.Drawing.Point(225, 10);
            this.dealsCard.Margin = new System.Windows.Forms.Padding(3);
            this.dealsCard.Name = "dealsCard";
            this.dealsCard.Padding = new System.Windows.Forms.Padding(20);
            this.dealsCard.Size = new System.Drawing.Size(215, 220);
            this.dealsCard.TabIndex = 1;
            // 
            // lblDealsIcon
            // 
            this.lblDealsIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDealsIcon.AutoSize = true;
            this.lblDealsIcon.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.lblDealsIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblDealsIcon.Location = new System.Drawing.Point(172, 20);
            this.lblDealsIcon.Name = "lblDealsIcon";
            this.lblDealsIcon.Size = new System.Drawing.Size(30, 30);
            this.lblDealsIcon.TabIndex = 4;
            this.lblDealsIcon.Text = "💰";
            // 
            // lblDealsTitle
            // 
            this.lblDealsTitle.AutoSize = true;
            this.lblDealsTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblDealsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.lblDealsTitle.Location = new System.Drawing.Point(20, 20);
            this.lblDealsTitle.Name = "lblDealsTitle";
            this.lblDealsTitle.Size = new System.Drawing.Size(82, 19);
            this.lblDealsTitle.TabIndex = 0;
            this.lblDealsTitle.Text = "Total Deals";
            // 
            // lblDealsValue
            // 
            this.lblDealsValue.AutoSize = true;
            this.lblDealsValue.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblDealsValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.lblDealsValue.Location = new System.Drawing.Point(20, 50);
            this.lblDealsValue.Name = "lblDealsValue";
            this.lblDealsValue.Size = new System.Drawing.Size(38, 45);
            this.lblDealsValue.TabIndex = 1;
            this.lblDealsValue.Text = "4";
            // 
            // lblDealsDesc
            // 
            this.lblDealsDesc.AutoSize = true;
            this.lblDealsDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDealsDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.lblDealsDesc.Location = new System.Drawing.Point(60, 70);
            this.lblDealsDesc.Name = "lblDealsDesc";
            this.lblDealsDesc.Size = new System.Drawing.Size(128, 15);
            this.lblDealsDesc.TabIndex = 2;
            this.lblDealsDesc.Text = "0 Closed | 0 In Progress";
            // 
            // lblViewAllDeals
            // 
            this.lblViewAllDeals.AutoSize = true;
            this.lblViewAllDeals.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblViewAllDeals.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblViewAllDeals.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.lblViewAllDeals.Location = new System.Drawing.Point(20, 185);
            this.lblViewAllDeals.Name = "lblViewAllDeals";
            this.lblViewAllDeals.Size = new System.Drawing.Size(83, 15);
            this.lblViewAllDeals.TabIndex = 3;
            this.lblViewAllDeals.Text = "View All Deals →";
            this.lblViewAllDeals.Click += new System.EventHandler(this.LblViewAllDeals_Click);
            // 
            // propertiesCard
            // 
            this.propertiesCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            this.propertiesCard.Controls.Add(this.lblPropertiesIcon);
            this.propertiesCard.Controls.Add(this.lblPropertiesTitle);
            this.propertiesCard.Controls.Add(this.lblPropertiesValue);
            this.propertiesCard.Controls.Add(this.lblPropertiesDesc);
            this.propertiesCard.Controls.Add(this.lblViewAllProperties);
            // Position as 1st card - start from left
            this.propertiesCard.Location = new System.Drawing.Point(0, 10);
            this.propertiesCard.Margin = new System.Windows.Forms.Padding(3);
            this.propertiesCard.Name = "propertiesCard";
            this.propertiesCard.Padding = new System.Windows.Forms.Padding(20);
            this.propertiesCard.Size = new System.Drawing.Size(215, 220);
            this.propertiesCard.TabIndex = 0;
            // 
            // lblPropertiesIcon
            // 
            this.lblPropertiesIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPropertiesIcon.AutoSize = true;
            this.lblPropertiesIcon.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.lblPropertiesIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.lblPropertiesIcon.Location = new System.Drawing.Point(172, 20);
            this.lblPropertiesIcon.Name = "lblPropertiesIcon";
            this.lblPropertiesIcon.Size = new System.Drawing.Size(30, 30);
            this.lblPropertiesIcon.TabIndex = 4;
            this.lblPropertiesIcon.Text = "🏠";
            // 
            // lblPropertiesTitle
            // 
            this.lblPropertiesTitle.AutoSize = true;
            this.lblPropertiesTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPropertiesTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.lblPropertiesTitle.Location = new System.Drawing.Point(20, 20);
            this.lblPropertiesTitle.Name = "lblPropertiesTitle";
            this.lblPropertiesTitle.Size = new System.Drawing.Size(116, 19);
            this.lblPropertiesTitle.TabIndex = 0;
            this.lblPropertiesTitle.Text = "Total Properties";
            // 
            // lblPropertiesValue
            // 
            this.lblPropertiesValue.AutoSize = true;
            this.lblPropertiesValue.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblPropertiesValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.lblPropertiesValue.Location = new System.Drawing.Point(20, 50);
            this.lblPropertiesValue.Name = "lblPropertiesValue";
            this.lblPropertiesValue.Size = new System.Drawing.Size(56, 45);
            this.lblPropertiesValue.TabIndex = 1;
            this.lblPropertiesValue.Text = "10";
            // 
            // lblPropertiesDesc
            // 
            this.lblPropertiesDesc.AutoSize = true;
            this.lblPropertiesDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPropertiesDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.lblPropertiesDesc.Location = new System.Drawing.Point(80, 70);
            this.lblPropertiesDesc.Name = "lblPropertiesDesc";
            this.lblPropertiesDesc.Size = new System.Drawing.Size(98, 15);
            this.lblPropertiesDesc.TabIndex = 2;
            this.lblPropertiesDesc.Text = "10 Active Listings";
            // 
            // lblViewAllProperties
            // 
            this.lblViewAllProperties.AutoSize = true;
            this.lblViewAllProperties.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblViewAllProperties.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblViewAllProperties.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.lblViewAllProperties.Location = new System.Drawing.Point(20, 185);
            this.lblViewAllProperties.Name = "lblViewAllProperties";
            this.lblViewAllProperties.Size = new System.Drawing.Size(108, 15);
            this.lblViewAllProperties.TabIndex = 3;
            this.lblViewAllProperties.Text = "View All Properties →";
            this.lblViewAllProperties.Click += new System.EventHandler(this.LblViewAllProperties_Click);
            // 
            // DashboardView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "DashboardView";
            this.Size = new System.Drawing.Size(940, 600);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.cardsPanel.ResumeLayout(false);
            this.leadsCard.ResumeLayout(false);
            this.leadsCard.PerformLayout();
            this.contactsCard.ResumeLayout(false);
            this.contactsCard.PerformLayout();
            this.dealsCard.ResumeLayout(false);
            this.dealsCard.PerformLayout();
            this.propertiesCard.ResumeLayout(false);
            this.propertiesCard.PerformLayout();
            this.ResumeLayout(false);

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