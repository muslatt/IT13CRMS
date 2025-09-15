// RealEstateCRMWinForms\Views\ContactsView.Designer.cs
using System.Windows.Forms;
using System.Drawing;

namespace RealEstateCRMWinForms.Views
{
    partial class ContactsView
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
            this.rightControlsPanel = new System.Windows.Forms.Panel();
            this.btnAddContact = new System.Windows.Forms.Button();
            this.sortComboBox = new System.Windows.Forms.ComboBox();
            this.filterButton = new System.Windows.Forms.Button();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.dataGridViewContacts = new System.Windows.Forms.DataGridView();
            this.searchPanel.SuspendLayout();
            this.rightControlsPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContacts)).BeginInit();
            this.SuspendLayout();
            // 
            // searchPanel
            // 
            this.searchPanel.BackColor = System.Drawing.Color.White;
            this.searchPanel.Controls.Add(this.rightControlsPanel);
            this.searchPanel.Controls.Add(this.searchBox);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(0, 0);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Padding = new System.Windows.Forms.Padding(10);
            this.searchPanel.Size = new System.Drawing.Size(940, 60);
            this.searchPanel.TabIndex = 0;
            // 
            // rightControlsPanel
            // 
            this.rightControlsPanel.Controls.Add(this.btnAddContact);
            this.rightControlsPanel.Controls.Add(this.sortComboBox);
            this.rightControlsPanel.Controls.Add(this.filterButton);
            this.rightControlsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightControlsPanel.Location = new System.Drawing.Point(520, 10);
            this.rightControlsPanel.Name = "rightControlsPanel";
            this.rightControlsPanel.Size = new System.Drawing.Size(410, 40);
            this.rightControlsPanel.TabIndex = 1;
            // 
            // btnAddContact
            // 
            this.btnAddContact.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnAddContact.FlatAppearance.BorderSize = 0;
            this.btnAddContact.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddContact.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAddContact.ForeColor = System.Drawing.Color.White;
            this.btnAddContact.Location = new System.Drawing.Point(290, 5);
            this.btnAddContact.Name = "btnAddContact";
            this.btnAddContact.Size = new System.Drawing.Size(110, 30);
            this.btnAddContact.TabIndex = 2;
            this.btnAddContact.Text = "+ Add Contact";
            this.btnAddContact.UseVisualStyleBackColor = false;
            this.btnAddContact.Click += new System.EventHandler(this.BtnAddContact_Click);
            // 
            // sortComboBox
            // 
            this.sortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sortComboBox.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.sortComboBox.FormattingEnabled = true;
            this.sortComboBox.Items.AddRange(new object[] {
            "Newest to Oldest",
            "Oldest to Newest",
            "Name A-Z",
            "Name Z-A"});
            this.sortComboBox.Location = new System.Drawing.Point(130, 9);
            this.sortComboBox.Name = "sortComboBox";
            this.sortComboBox.Size = new System.Drawing.Size(150, 23);
            this.sortComboBox.TabIndex = 1;
            this.sortComboBox.SelectedIndexChanged += new System.EventHandler(this.SortComboBox_SelectedIndexChanged);
            // 
            // filterButton
            // 
            this.filterButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.filterButton.Location = new System.Drawing.Point(10, 7);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(110, 27);
            this.filterButton.TabIndex = 0;
            this.filterButton.Text = "Filter";
            this.filterButton.UseVisualStyleBackColor = true;
            // 
            // searchBox
            // 
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.searchBox.Location = new System.Drawing.Point(15, 19);
            this.searchBox.Name = "searchBox";
            this.searchBox.PlaceholderText = "Search Contacts...";
            this.searchBox.Size = new System.Drawing.Size(300, 23);
            this.searchBox.TabIndex = 0;
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.dataGridViewContacts);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 60);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(10);
            this.contentPanel.Size = new System.Drawing.Size(940, 540);
            this.contentPanel.TabIndex = 1;
            // 
            // dataGridViewContacts
            // 
            this.dataGridViewContacts.AllowUserToAddRows = false;
            this.dataGridViewContacts.AllowUserToDeleteRows = false;
            this.dataGridViewContacts.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewContacts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewContacts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewContacts.Location = new System.Drawing.Point(10, 10);
            this.dataGridViewContacts.Name = "dataGridViewContacts";
            this.dataGridViewContacts.ReadOnly = true;
            this.dataGridViewContacts.RowTemplate.Height = 25;
            this.dataGridViewContacts.Size = new System.Drawing.Size(920, 520);
            this.dataGridViewContacts.TabIndex = 0;
            // 
            // ContactsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.searchPanel);
            this.Name = "ContactsView";
            this.Size = new System.Drawing.Size(940, 600);
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.rightControlsPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContacts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Panel rightControlsPanel;
        private System.Windows.Forms.Button filterButton;
        private System.Windows.Forms.ComboBox sortComboBox;
        private System.Windows.Forms.Button btnAddContact;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.DataGridView dataGridViewContacts;
    }
}