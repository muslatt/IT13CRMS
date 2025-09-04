// RealEstateCRMWinForms\Views\LeadsView.Designer.cs
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    partial class LeadsView
    {
        private System.ComponentModel.IContainer components = null;
        private Panel searchPanel;
        private TextBox searchBox;
        private Panel rightControlsPanel;
        private Button btnFilter;
        private ComboBox sortComboBox;
        private Button btnAddLead;
        private Panel contentPanel;
        private DataGridView dataGridViewLeads;

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
            searchPanel = new Panel();
            searchBox = new TextBox();
            rightControlsPanel = new Panel();
            btnFilter = new Button();
            sortComboBox = new ComboBox();
            btnAddLead = new Button();
            contentPanel = new Panel();
            dataGridViewLeads = new DataGridView();
            
            searchPanel.SuspendLayout();
            rightControlsPanel.SuspendLayout();
            contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewLeads).BeginInit();
            SuspendLayout();

            // searchPanel
            searchPanel.BackColor = Color.Transparent;
            searchPanel.Controls.Add(searchBox);
            searchPanel.Controls.Add(rightControlsPanel);
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Location = new Point(0, 0);
            searchPanel.Name = "searchPanel";
            searchPanel.Padding = new Padding(20, 15, 20, 15);
            searchPanel.Size = new Size(940, 60);
            searchPanel.TabIndex = 0;

            // searchBox
            searchBox.Font = new Font("Segoe UI", 10F);
            searchBox.Location = new Point(20, 15);
            searchBox.Name = "searchBox";
            searchBox.PlaceholderText = "Search";
            searchBox.Size = new Size(350, 25);
            searchBox.TabIndex = 0;
            searchBox.TextChanged += SearchBox_TextChanged;

            // rightControlsPanel
            rightControlsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rightControlsPanel.BackColor = Color.Transparent;
            rightControlsPanel.Controls.Add(btnFilter);
            rightControlsPanel.Controls.Add(sortComboBox);
            rightControlsPanel.Controls.Add(btnAddLead);
            rightControlsPanel.Location = new Point(580, 12);
            rightControlsPanel.Name = "rightControlsPanel";
            rightControlsPanel.Size = new Size(340, 35);
            rightControlsPanel.TabIndex = 1;

            // btnFilter
            btnFilter.BackColor = Color.White;
            btnFilter.FlatAppearance.BorderColor = Color.FromArgb(206, 212, 218);
            btnFilter.FlatAppearance.BorderSize = 1;
            btnFilter.FlatStyle = FlatStyle.Flat;
            btnFilter.Font = new Font("Segoe UI", 9F);
            btnFilter.ForeColor = Color.FromArgb(108, 117, 125);
            btnFilter.Location = new Point(0, 0);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(80, 35);
            btnFilter.TabIndex = 0;
            btnFilter.Text = "📋 Filter";
            btnFilter.UseVisualStyleBackColor = false;

            // sortComboBox
            sortComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sortComboBox.Font = new Font("Segoe UI", 9F);
            sortComboBox.FormattingEnabled = true;
            sortComboBox.Items.AddRange(new object[] { "Newest to Oldest", "Oldest to Newest", "Name A-Z", "Name Z-A" });
            sortComboBox.Location = new Point(90, 0);
            sortComboBox.Name = "sortComboBox";
            sortComboBox.Size = new Size(140, 23);
            sortComboBox.TabIndex = 1;
            sortComboBox.SelectedIndexChanged += SortComboBox_SelectedIndexChanged;

            // btnAddLead
            btnAddLead.BackColor = Color.FromArgb(0, 123, 255);
            btnAddLead.FlatAppearance.BorderSize = 0;
            btnAddLead.FlatStyle = FlatStyle.Flat;
            btnAddLead.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAddLead.ForeColor = Color.White;
            btnAddLead.Location = new Point(240, 0);
            btnAddLead.Name = "btnAddLead";
            btnAddLead.Size = new Size(100, 35);
            btnAddLead.TabIndex = 2;
            btnAddLead.Text = "+ Add Lead";
            btnAddLead.UseVisualStyleBackColor = false;
            btnAddLead.Click += BtnAddLead_Click;

            // contentPanel
            contentPanel.BackColor = Color.FromArgb(248, 249, 250);
            contentPanel.Controls.Add(dataGridViewLeads);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(0, 60);
            contentPanel.Name = "contentPanel";
            contentPanel.Padding = new Padding(20);
            contentPanel.Size = new Size(940, 540);
            contentPanel.TabIndex = 1;

            // dataGridViewLeads
            dataGridViewLeads.AllowUserToAddRows = false;
            dataGridViewLeads.AllowUserToDeleteRows = false;
            dataGridViewLeads.AllowUserToResizeColumns = false;
            dataGridViewLeads.AllowUserToResizeRows = false;
            dataGridViewLeads.AllowUserToOrderColumns = false;
            dataGridViewLeads.AutoGenerateColumns = false;
            dataGridViewLeads.BackgroundColor = Color.White;
            dataGridViewLeads.BorderStyle = BorderStyle.None;
            dataGridViewLeads.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewLeads.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewLeads.ColumnHeadersHeight = 40;
            dataGridViewLeads.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewLeads.Dock = DockStyle.Fill;
            dataGridViewLeads.EnableHeadersVisualStyles = false;
            dataGridViewLeads.GridColor = Color.FromArgb(240, 240, 240);
            dataGridViewLeads.Location = new Point(20, 20);
            dataGridViewLeads.MultiSelect = false;
            dataGridViewLeads.Name = "dataGridViewLeads";
            dataGridViewLeads.ReadOnly = true;
            dataGridViewLeads.RowHeadersVisible = false;
            dataGridViewLeads.RowTemplate.Height = 60;
            dataGridViewLeads.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewLeads.Size = new Size(900, 500);
            dataGridViewLeads.TabIndex = 0;

            // Column styling
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(108, 117, 125);
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.Padding = new Padding(15, 0, 0, 0);
            dataGridViewLeads.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 249, 250);

            // Default cell styling
            dataGridViewLeads.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dataGridViewLeads.DefaultCellStyle.Padding = new Padding(15, 0, 0, 0);
            dataGridViewLeads.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 245, 255);
            dataGridViewLeads.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Add new columns
            var colSelect = new DataGridViewCheckBoxColumn
            {
                Name = "Select",
                HeaderText = "",
                Width = 40,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            var colAvatar = new DataGridViewImageColumn
            {
                Name = "Avatar",
                HeaderText = "",
                Width = 60,
                ImageLayout = DataGridViewImageCellLayout.Normal,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            var colName = new DataGridViewTextBoxColumn
            {
                Name = "Lead",
                HeaderText = "Lead",
                DataPropertyName = "FullName",
                Width = 150,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            var colStatus = new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataPropertyName = "Status",
                Width = 130,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            var colCreateContact = new DataGridViewButtonColumn
            {
                Name = "CreateContact",
                HeaderText = "Create a contact",
                Text = "Move to Contacts",
                UseColumnTextForButtonValue = true,
                Width = 140,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            var colEmail = new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                DataPropertyName = "Email",
                Width = 180,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            var colPhone = new DataGridViewTextBoxColumn
            {
                Name = "Phone",
                HeaderText = "Phone",
                DataPropertyName = "Phone",
                Width = 140,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            var colType = new DataGridViewTextBoxColumn
            {
                Name = "Type",
                HeaderText = "Type",
                DataPropertyName = "Type",
                Width = 100,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            var colAddress = new DataGridViewTextBoxColumn
            {
                Name = "Address",
                HeaderText = "Address",
                DataPropertyName = "Address",
                Width = 120,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, // Fill remaining space
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            dataGridViewLeads.Columns.AddRange(new DataGridViewColumn[] {
                colSelect, colAvatar, colName, colStatus, colCreateContact, colEmail, colPhone, colType, colAddress
            });

            // Events
            dataGridViewLeads.CellPainting += DataGridViewLeads_CellPainting;
            dataGridViewLeads.CellFormatting += DataGridViewLeads_CellFormatting;
            dataGridViewLeads.CellContentClick += DataGridViewLeads_CellContentClick;
            dataGridViewLeads.ColumnHeaderMouseClick += DataGridViewLeads_ColumnHeaderMouseClick;

            // LeadsView
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 249, 250);
            Controls.Add(contentPanel);
            Controls.Add(searchPanel);
            Name = "LeadsView";
            Size = new Size(940, 600);

            searchPanel.ResumeLayout(false);
            searchPanel.PerformLayout();
            rightControlsPanel.ResumeLayout(false);
            contentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewLeads).EndInit();
            ResumeLayout(false);
        }
    }
}