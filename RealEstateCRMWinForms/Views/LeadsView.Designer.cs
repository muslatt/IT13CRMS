// RealEstateCRMWinForms\Views\LeadsView.Designer.cs
using System.Windows.Forms;
using System.Drawing;

namespace RealEstateCRMWinForms.Views
{
    partial class LeadsView
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
            this.searchPanel = new System.Windows.Forms.Panel();
            this.searchBoxContainer = new System.Windows.Forms.Panel();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchIcon = new System.Windows.Forms.Label();
            this.rightControlsPanel = new System.Windows.Forms.Panel();
            this.btnAddLead = new System.Windows.Forms.Button();
            this.sortComboBox = new System.Windows.Forms.ComboBox();
            this.filterButton = new System.Windows.Forms.Button();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.dataGridViewLeads = new System.Windows.Forms.DataGridView();
            this.searchPanel.SuspendLayout();
            this.searchBoxContainer.SuspendLayout();
            this.rightControlsPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLeads)).BeginInit();
            this.SuspendLayout();
            // 
            // searchPanel
            // 
            this.searchPanel.BackColor = System.Drawing.Color.White;
            this.searchPanel.Controls.Add(this.searchBoxContainer);
            this.searchPanel.Controls.Add(this.rightControlsPanel);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(0, 0);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Padding = new System.Windows.Forms.Padding(30, 20, 30, 20);
            this.searchPanel.Size = new System.Drawing.Size(1000, 80);
            this.searchPanel.TabIndex = 0;
            // 
            // searchBoxContainer
            // 
            this.searchBoxContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.searchBoxContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBoxContainer.Controls.Add(this.searchBox);
            this.searchBoxContainer.Controls.Add(this.searchIcon);
            this.searchBoxContainer.Location = new System.Drawing.Point(30, 20);
            this.searchBoxContainer.Name = "searchBoxContainer";
            this.searchBoxContainer.Size = new System.Drawing.Size(420, 40);
            this.searchBoxContainer.TabIndex = 0;
            // 
            // searchBox
            // 
            this.searchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.searchBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.searchBox.Location = new System.Drawing.Point(40, 10);
            this.searchBox.Name = "searchBox";
            this.searchBox.PlaceholderText = "Search leads...";
            this.searchBox.Size = new System.Drawing.Size(340, 20);
            this.searchBox.TabIndex = 0;
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // searchIcon
            // 
            this.searchIcon.AutoSize = true;
            this.searchIcon.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.searchIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.searchIcon.Location = new System.Drawing.Point(12, 9);
            this.searchIcon.Name = "searchIcon";
            this.searchIcon.Size = new System.Drawing.Size(21, 21);
            this.searchIcon.TabIndex = 1;
            this.searchIcon.Text = "🔍";
            // 
            // rightControlsPanel
            // 
            this.rightControlsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rightControlsPanel.BackColor = System.Drawing.Color.Transparent;
            this.rightControlsPanel.Controls.Add(this.btnAddLead);
            this.rightControlsPanel.Controls.Add(this.sortComboBox);
            this.rightControlsPanel.Controls.Add(this.filterButton);
            this.rightControlsPanel.Location = new System.Drawing.Point(530, 20);
            this.rightControlsPanel.Name = "rightControlsPanel";
            this.rightControlsPanel.Size = new System.Drawing.Size(440, 40);
            this.rightControlsPanel.TabIndex = 1;
            // 
            // btnAddLead
            // 
            this.btnAddLead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnAddLead.FlatAppearance.BorderSize = 0;
            this.btnAddLead.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLead.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAddLead.ForeColor = System.Drawing.Color.White;
            this.btnAddLead.Location = new System.Drawing.Point(290, 0);
            this.btnAddLead.Name = "btnAddLead";
            this.btnAddLead.Size = new System.Drawing.Size(150, 36);
            this.btnAddLead.TabIndex = 2;
            this.btnAddLead.Text = "+ Add Lead";
            this.btnAddLead.UseVisualStyleBackColor = false;
            this.btnAddLead.Click += new System.EventHandler(this.BtnAddLead_Click);
            // 
            // sortComboBox
            // 
            this.sortComboBox.BackColor = System.Drawing.Color.White;
            this.sortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sortComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sortComboBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.sortComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.sortComboBox.FormattingEnabled = true;
            this.sortComboBox.Items.AddRange(new object[] {
            "Newest to Oldest",
            "Oldest to Newest",
            "Name A-Z",
            "Name Z-A"});
            this.sortComboBox.Location = new System.Drawing.Point(110, 3);
            this.sortComboBox.Name = "sortComboBox";
            this.sortComboBox.Size = new System.Drawing.Size(170, 27);
            this.sortComboBox.TabIndex = 1;
            this.sortComboBox.SelectedIndexChanged += new System.EventHandler(this.SortComboBox_SelectedIndexChanged);
            // 
            // filterButton
            // 
            this.filterButton.BackColor = System.Drawing.Color.White;
            this.filterButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.filterButton.FlatAppearance.BorderSize = 1;
            this.filterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.filterButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.filterButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.filterButton.Location = new System.Drawing.Point(0, 0);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(100, 36);
            this.filterButton.TabIndex = 0;
            this.filterButton.Text = "Filter";
            this.filterButton.UseVisualStyleBackColor = false;
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.dataGridViewLeads);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 80);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(30);
            this.contentPanel.Size = new System.Drawing.Size(1000, 590);
            this.contentPanel.TabIndex = 1;
            // 
            // dataGridViewLeads
            // 
            this.dataGridViewLeads.AllowUserToAddRows = false;
            this.dataGridViewLeads.AllowUserToDeleteRows = false;
            this.dataGridViewLeads.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewLeads.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewLeads.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLeads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewLeads.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.dataGridViewLeads.Location = new System.Drawing.Point(30, 30);
            this.dataGridViewLeads.Name = "dataGridViewLeads";
            this.dataGridViewLeads.ReadOnly = true;
            this.dataGridViewLeads.Size = new System.Drawing.Size(940, 530);
            this.dataGridViewLeads.TabIndex = 0;
            this.dataGridViewLeads.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewLeads_CellContentClick_1);
            // 
            // LeadsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.searchPanel);
            this.Name = "LeadsView";
            this.Size = new System.Drawing.Size(1000, 670);
            this.searchPanel.ResumeLayout(false);
            this.searchBoxContainer.ResumeLayout(false);
            this.searchBoxContainer.PerformLayout();
            this.rightControlsPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLeads)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Panel rightControlsPanel;
        private System.Windows.Forms.Button filterButton;
        private System.Windows.Forms.ComboBox sortComboBox;
        private System.Windows.Forms.Button btnAddLead;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.DataGridView dataGridViewLeads;
        private System.Windows.Forms.Panel searchBoxContainer;
        private System.Windows.Forms.Label searchIcon;
    }
}