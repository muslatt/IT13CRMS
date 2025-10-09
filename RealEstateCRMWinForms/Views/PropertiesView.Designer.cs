namespace RealEstateCRMWinForms.Views
{
    partial class PropertiesView
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
            flowLayoutPanel = new FlowLayoutPanel();
            searchPanel = new Panel();
            searchBoxContainer = new Panel();
            searchBox = new TextBox();
            searchIcon = new Label();
            rightControlsPanel = new Panel();
            btnFilter = new Button();
            sortComboBox = new ComboBox();
            btnAddProperty = new Button();
            btnShowAll = new Button();
            btnShowApproved = new Button();
            btnShowPending = new Button();
            btnShowRejected = new Button();
            paginationPanel = new Panel();
            propertiesPaginationLayout = new TableLayoutPanel();
            pageNumbersPanel = new FlowLayoutPanel();
            searchPanel.SuspendLayout();
            searchBoxContainer.SuspendLayout();
            rightControlsPanel.SuspendLayout();
            paginationPanel.SuspendLayout();
            propertiesPaginationLayout.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.BackColor = Color.FromArgb(249, 250, 251);
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.Location = new Point(0, 80);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Padding = new Padding(30);
            flowLayoutPanel.Size = new Size(1000, 590);
            flowLayoutPanel.TabIndex = 0;
            flowLayoutPanel.Paint += flowLayoutPanel_Paint;
            flowLayoutPanel.WrapContents = true;
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.AutoScrollMargin = new Size(0, 20);
            // 
            // searchPanel
            // 
            searchPanel.BackColor = Color.White;
            searchPanel.Controls.Add(searchBoxContainer);
            searchPanel.Controls.Add(rightControlsPanel);
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Location = new Point(0, 0);
            searchPanel.Name = "searchPanel";
            searchPanel.Padding = new Padding(30, 20, 30, 20);
            searchPanel.Size = new Size(1000, 80);
            searchPanel.TabIndex = 1;
            // 
            // searchBoxContainer
            // 
            searchBoxContainer.BackColor = Color.FromArgb(243, 244, 246);
            searchBoxContainer.BorderStyle = BorderStyle.FixedSingle;
            searchBoxContainer.Controls.Add(searchBox);
            searchBoxContainer.Controls.Add(searchIcon);
            searchBoxContainer.Location = new Point(30, 20);
            searchBoxContainer.Name = "searchBoxContainer";
            searchBoxContainer.Size = new Size(420, 40);
            searchBoxContainer.TabIndex = 0;
            searchBoxContainer.Paint += searchBoxContainer_Paint;
            // 
            // searchBox
            // 
            searchBox.BackColor = Color.FromArgb(243, 244, 246);
            searchBox.BorderStyle = BorderStyle.None;
            searchBox.Font = new Font("Segoe UI", 11F);
            searchBox.ForeColor = Color.FromArgb(107, 114, 128);
            searchBox.Location = new Point(40, 10);
            searchBox.Name = "searchBox";
            searchBox.PlaceholderText = "Search properties...";
            searchBox.Size = new Size(340, 20);
            searchBox.TabIndex = 0;
            // 
            // searchIcon
            // 
            searchIcon.AutoSize = true;
            searchIcon.Font = new Font("Segoe UI", 12F);
            searchIcon.ForeColor = Color.FromArgb(156, 163, 175);
            searchIcon.Location = new Point(12, 9);
            searchIcon.Name = "searchIcon";
            searchIcon.Size = new Size(32, 21);
            searchIcon.TabIndex = 1;
            searchIcon.Text = "🔍";
            // 
            // rightControlsPanel
            // 
            rightControlsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rightControlsPanel.BackColor = Color.Transparent;
            rightControlsPanel.Controls.Add(btnAddProperty);
            rightControlsPanel.Controls.Add(btnShowAll);
            rightControlsPanel.Controls.Add(btnShowApproved);
            rightControlsPanel.Controls.Add(btnShowPending);
            rightControlsPanel.Controls.Add(btnShowRejected);
            rightControlsPanel.Location = new Point(460, 20);
            rightControlsPanel.Name = "rightControlsPanel";
            rightControlsPanel.Size = new Size(510, 40);
            rightControlsPanel.TabIndex = 1;
            // 
            // btnFilter
            // 
            btnFilter.Anchor = AnchorStyles.None;
            btnFilter.BackColor = Color.White;
            btnFilter.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnFilter.FlatStyle = FlatStyle.Flat;
            btnFilter.Font = new Font("Segoe UI", 10F);
            btnFilter.ForeColor = Color.FromArgb(55, 65, 81);
            btnFilter.Location = new Point(0, 0);
            btnFilter.Margin = new Padding(0, 0, 10, 0);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(100, 30);
            btnFilter.TabIndex = 0;
            btnFilter.Text = "Filter";
            btnFilter.UseVisualStyleBackColor = false;
            // 
            // sortComboBox
            // 
            sortComboBox.Anchor = AnchorStyles.None;
            sortComboBox.BackColor = Color.White;
            sortComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sortComboBox.FlatStyle = FlatStyle.Flat;
            sortComboBox.Font = new Font("Segoe UI", 10F);
            sortComboBox.ForeColor = Color.FromArgb(55, 65, 81);
            sortComboBox.FormattingEnabled = true;
            sortComboBox.Items.AddRange(new object[] { "Newest to Oldest", "Price: Low to High", "Price: High to Low" });
            sortComboBox.Location = new Point(110, 3);
            sortComboBox.Margin = new Padding(0, 0, 20, 0);
            sortComboBox.Name = "sortComboBox";
            sortComboBox.Size = new Size(170, 25);
            sortComboBox.TabIndex = 1;
            // 
            // btnAddProperty
            // 
            btnAddProperty.BackColor = Color.FromArgb(37, 99, 235);
            btnAddProperty.FlatAppearance.BorderSize = 0;
            btnAddProperty.FlatStyle = FlatStyle.Flat;
            btnAddProperty.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddProperty.ForeColor = Color.White;
            btnAddProperty.Location = new Point(0, 0);
            btnAddProperty.Name = "btnAddProperty";
            btnAddProperty.Size = new Size(150, 36);
            btnAddProperty.TabIndex = 2;
            btnAddProperty.Text = "+ Add Property";
            btnAddProperty.UseVisualStyleBackColor = false;
            btnAddProperty.Click += BtnAddProperty_Click;
            // 
            // btnShowAll
            // 
            btnShowAll.BackColor = Color.White;
            btnShowAll.FlatAppearance.BorderColor = Color.FromArgb(0, 123, 255);
            btnShowAll.FlatAppearance.BorderSize = 1;
            btnShowAll.FlatStyle = FlatStyle.Flat;
            btnShowAll.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnShowAll.ForeColor = Color.FromArgb(0, 123, 255);
            btnShowAll.Location = new Point(160, 6);
            btnShowAll.Name = "btnShowAll";
            btnShowAll.Size = new Size(70, 28);
            btnShowAll.TabIndex = 3;
            btnShowAll.Text = "All";
            btnShowAll.UseVisualStyleBackColor = false;
            btnShowAll.Click += BtnShowAll_Click;
            // 
            // btnShowApproved
            // 
            btnShowApproved.BackColor = Color.White;
            btnShowApproved.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnShowApproved.FlatAppearance.BorderSize = 1;
            btnShowApproved.FlatStyle = FlatStyle.Flat;
            btnShowApproved.Font = new Font("Segoe UI", 9F);
            btnShowApproved.ForeColor = Color.FromArgb(107, 114, 128);
            btnShowApproved.Location = new Point(235, 6);
            btnShowApproved.Name = "btnShowApproved";
            btnShowApproved.Size = new Size(90, 28);
            btnShowApproved.TabIndex = 4;
            btnShowApproved.Text = "Approved";
            btnShowApproved.UseVisualStyleBackColor = false;
            btnShowApproved.Click += BtnShowApproved_Click;
            // 
            // btnShowPending
            // 
            btnShowPending.BackColor = Color.White;
            btnShowPending.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnShowPending.FlatAppearance.BorderSize = 1;
            btnShowPending.FlatStyle = FlatStyle.Flat;
            btnShowPending.Font = new Font("Segoe UI", 9F);
            btnShowPending.ForeColor = Color.FromArgb(107, 114, 128);
            btnShowPending.Location = new Point(330, 6);
            btnShowPending.Name = "btnShowPending";
            btnShowPending.Size = new Size(80, 28);
            btnShowPending.TabIndex = 5;
            btnShowPending.Text = "Pending";
            btnShowPending.UseVisualStyleBackColor = false;
            btnShowPending.Click += BtnShowPending_Click;
            // 
            // btnShowRejected
            // 
            btnShowRejected.BackColor = Color.White;
            btnShowRejected.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnShowRejected.FlatAppearance.BorderSize = 1;
            btnShowRejected.FlatStyle = FlatStyle.Flat;
            btnShowRejected.Font = new Font("Segoe UI", 9F);
            btnShowRejected.ForeColor = Color.FromArgb(107, 114, 128);
            btnShowRejected.Location = new Point(415, 6);
            btnShowRejected.Name = "btnShowRejected";
            btnShowRejected.Size = new Size(85, 28);
            btnShowRejected.TabIndex = 6;
            btnShowRejected.Text = "Rejected";
            btnShowRejected.UseVisualStyleBackColor = false;
            btnShowRejected.Click += BtnShowRejected_Click;
            // 
            // paginationPanel
            // 
            paginationPanel.BackColor = Color.White;
            paginationPanel.Controls.Add(propertiesPaginationLayout);
            paginationPanel.Dock = DockStyle.Bottom;
            paginationPanel.Location = new Point(0, 620);
            paginationPanel.Name = "paginationPanel";
            paginationPanel.Padding = new Padding(20, 10, 20, 10);
            paginationPanel.Size = new Size(1000, 50);
            paginationPanel.TabIndex = 2;
            // 
            // propertiesPaginationLayout
            // 
            propertiesPaginationLayout.ColumnCount = 5;
            propertiesPaginationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Filter button
            propertiesPaginationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Sort combo
            propertiesPaginationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // left filler
            propertiesPaginationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // page numbers (centered)
            propertiesPaginationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // right filler
            propertiesPaginationLayout.Controls.Add(btnFilter, 0, 0);
            propertiesPaginationLayout.Controls.Add(sortComboBox, 1, 0);
            propertiesPaginationLayout.Controls.Add(pageNumbersPanel, 3, 0);
            propertiesPaginationLayout.Dock = DockStyle.Fill;
            propertiesPaginationLayout.Location = new Point(20, 10);
            propertiesPaginationLayout.Name = "propertiesPaginationLayout";
            propertiesPaginationLayout.RowCount = 1;
            propertiesPaginationLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            propertiesPaginationLayout.Size = new Size(960, 30);
            propertiesPaginationLayout.TabIndex = 0;

            // 
            // pageNumbersPanel
            // 
            pageNumbersPanel.Anchor = AnchorStyles.None;
            pageNumbersPanel.AutoSize = true;
            pageNumbersPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pageNumbersPanel.FlowDirection = FlowDirection.LeftToRight;
            pageNumbersPanel.WrapContents = false;
            pageNumbersPanel.Margin = new Padding(0);
            pageNumbersPanel.Padding = new Padding(0);
            // 
            // (numeric page buttons are created dynamically in code-behind)
            // 
            // PropertiesView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(249, 250, 251);
            Controls.Add(flowLayoutPanel);
            Controls.Add(paginationPanel);
            Controls.Add(searchPanel);
            Name = "PropertiesView";
            Size = new Size(1000, 670);
            searchPanel.ResumeLayout(false);
            searchBoxContainer.ResumeLayout(false);
            searchBoxContainer.PerformLayout();
            rightControlsPanel.ResumeLayout(false);
            paginationPanel.ResumeLayout(false);
            propertiesPaginationLayout.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Panel rightControlsPanel;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.ComboBox sortComboBox;
        private System.Windows.Forms.Button btnAddProperty;
        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.Button btnShowApproved;
        private System.Windows.Forms.Button btnShowPending;
        private System.Windows.Forms.Button btnShowRejected;
        private System.Windows.Forms.Panel searchBoxContainer;
        private System.Windows.Forms.Label searchIcon;
        private System.Windows.Forms.Panel paginationPanel;
        private System.Windows.Forms.TableLayoutPanel propertiesPaginationLayout;
        private System.Windows.Forms.FlowLayoutPanel pageNumbersPanel;
    }
}