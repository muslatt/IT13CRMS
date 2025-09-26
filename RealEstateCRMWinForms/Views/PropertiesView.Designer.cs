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
            paginationPanel = new Panel();
            propertiesPaginationLayout = new TableLayoutPanel();
            lblPropertyPageInfo = new Label();
            btnPrevPropertyPage = new Button();
            btnNextPropertyPage = new Button();
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
            rightControlsPanel.Controls.Add(btnFilter);
            rightControlsPanel.Controls.Add(sortComboBox);
            rightControlsPanel.Controls.Add(btnAddProperty);
            rightControlsPanel.Location = new Point(530, 20);
            rightControlsPanel.Name = "rightControlsPanel";
            rightControlsPanel.Size = new Size(440, 40);
            rightControlsPanel.TabIndex = 1;
            // 
            // btnFilter
            // 
            btnFilter.BackColor = Color.White;
            btnFilter.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnFilter.FlatStyle = FlatStyle.Flat;
            btnFilter.Font = new Font("Segoe UI", 10F);
            btnFilter.ForeColor = Color.FromArgb(55, 65, 81);
            btnFilter.Location = new Point(0, 0);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(100, 36);
            btnFilter.TabIndex = 0;
            btnFilter.Text = "Filter";
            btnFilter.UseVisualStyleBackColor = false;
            // 
            // sortComboBox
            // 
            sortComboBox.BackColor = Color.White;
            sortComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sortComboBox.FlatStyle = FlatStyle.Flat;
            sortComboBox.Font = new Font("Segoe UI", 10F);
            sortComboBox.ForeColor = Color.FromArgb(55, 65, 81);
            sortComboBox.FormattingEnabled = true;
            sortComboBox.Items.AddRange(new object[] { "Newest to Oldest", "Price: Low to High", "Price: High to Low" });
            sortComboBox.Location = new Point(110, 3);
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
            btnAddProperty.Location = new Point(290, 0);
            btnAddProperty.Name = "btnAddProperty";
            btnAddProperty.Size = new Size(150, 36);
            btnAddProperty.TabIndex = 2;
            btnAddProperty.Text = "+ Add Property";
            btnAddProperty.UseVisualStyleBackColor = false;
            btnAddProperty.Click += BtnAddProperty_Click;
            // 
            // paginationPanel
            // 
            paginationPanel.BackColor = Color.White;
            paginationPanel.Controls.Add(propertiesPaginationLayout);
            paginationPanel.Dock = DockStyle.Bottom;
            paginationPanel.Location = new Point(0, 630);
            paginationPanel.Name = "paginationPanel";
            // Give a bit more vertical room so text isn't clipped
            // 40 (panel height) - 5 - 5 = 30 client height
            paginationPanel.Padding = new Padding(10, 5, 10, 5);
            paginationPanel.Size = new Size(1000, 40);
            paginationPanel.TabIndex = 2;
            // 
            // propertiesPaginationLayout
            // 
            propertiesPaginationLayout.ColumnCount = 4;
            propertiesPaginationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            propertiesPaginationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            propertiesPaginationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            propertiesPaginationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            propertiesPaginationLayout.Controls.Add(lblPropertyPageInfo, 1, 0);
            propertiesPaginationLayout.Controls.Add(btnPrevPropertyPage, 2, 0);
            propertiesPaginationLayout.Controls.Add(btnNextPropertyPage, 3, 0);
            propertiesPaginationLayout.Dock = DockStyle.Fill;
            propertiesPaginationLayout.Location = new Point(10, 8);
            propertiesPaginationLayout.Name = "propertiesPaginationLayout";
            propertiesPaginationLayout.RowCount = 1;
            propertiesPaginationLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            // Match inner client height to buttons so text isn't clipped
            propertiesPaginationLayout.Size = new Size(980, 30);
            propertiesPaginationLayout.TabIndex = 0;
            // 
            // lblPropertyPageInfo
            // 
            lblPropertyPageInfo.Anchor = AnchorStyles.Right;
            lblPropertyPageInfo.AutoSize = true;
            lblPropertyPageInfo.Font = new Font("Segoe UI", 10F);
            lblPropertyPageInfo.ForeColor = Color.FromArgb(55, 65, 81);
            lblPropertyPageInfo.Location = new Point(800, 3);
            lblPropertyPageInfo.Margin = new Padding(0, 0, 10, 0);
            lblPropertyPageInfo.Name = "lblPropertyPageInfo";
            lblPropertyPageInfo.Size = new Size(86, 19);
            lblPropertyPageInfo.TabIndex = 0;
            lblPropertyPageInfo.Text = "Page 1 of 1";
            // 
            // btnPrevPropertyPage
            // 
            btnPrevPropertyPage.AutoSize = false;
            btnPrevPropertyPage.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnPrevPropertyPage.BackColor = Color.White;
            btnPrevPropertyPage.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnPrevPropertyPage.FlatAppearance.BorderSize = 1;
            btnPrevPropertyPage.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 244, 246);
            btnPrevPropertyPage.FlatAppearance.MouseDownBackColor = Color.FromArgb(229, 231, 235);
            btnPrevPropertyPage.FlatStyle = FlatStyle.Flat;
            btnPrevPropertyPage.Font = new Font("Segoe UI", 10F);
            btnPrevPropertyPage.ForeColor = Color.FromArgb(55, 65, 81);
            btnPrevPropertyPage.Location = new Point(896, 0);
            btnPrevPropertyPage.Margin = new Padding(0, 0, 6, 0);
            btnPrevPropertyPage.Name = "btnPrevPropertyPage";
            btnPrevPropertyPage.Padding = new Padding(12, 6, 12, 6);
            btnPrevPropertyPage.MinimumSize = new Size(100, 30);
            btnPrevPropertyPage.Size = new Size(100, 30);
            btnPrevPropertyPage.TextAlign = ContentAlignment.MiddleCenter;
            btnPrevPropertyPage.TabIndex = 1;
            btnPrevPropertyPage.Text = "Previous";
            btnPrevPropertyPage.UseVisualStyleBackColor = false;
            btnPrevPropertyPage.Click += BtnPrevPropertyPage_Click;
            // 
            // btnNextPropertyPage
            // 
            btnNextPropertyPage.AutoSize = false;
            btnNextPropertyPage.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnNextPropertyPage.BackColor = Color.White;
            btnNextPropertyPage.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnNextPropertyPage.FlatAppearance.BorderSize = 1;
            btnNextPropertyPage.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 244, 246);
            btnNextPropertyPage.FlatAppearance.MouseDownBackColor = Color.FromArgb(229, 231, 235);
            btnNextPropertyPage.FlatStyle = FlatStyle.Flat;
            btnNextPropertyPage.Font = new Font("Segoe UI", 10F);
            btnNextPropertyPage.ForeColor = Color.FromArgb(55, 65, 81);
            btnNextPropertyPage.Location = new Point(992, 0);
            btnNextPropertyPage.Margin = new Padding(0);
            btnNextPropertyPage.Name = "btnNextPropertyPage";
            btnNextPropertyPage.Padding = new Padding(12, 6, 12, 6);
            btnNextPropertyPage.MinimumSize = new Size(72, 30);
            btnNextPropertyPage.Size = new Size(72, 30);
            btnNextPropertyPage.TextAlign = ContentAlignment.MiddleCenter;
            btnNextPropertyPage.TabIndex = 2;
            btnNextPropertyPage.Text = "Next";
            btnNextPropertyPage.UseVisualStyleBackColor = false;
            btnNextPropertyPage.Click += BtnNextPropertyPage_Click;
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
            propertiesPaginationLayout.PerformLayout();
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
        private System.Windows.Forms.Panel searchBoxContainer;
        private System.Windows.Forms.Label searchIcon;
        private System.Windows.Forms.Panel paginationPanel;
        private System.Windows.Forms.TableLayoutPanel propertiesPaginationLayout;
        private System.Windows.Forms.Button btnPrevPropertyPage;
        private System.Windows.Forms.Button btnNextPropertyPage;
        private System.Windows.Forms.Label lblPropertyPageInfo;
    }
}
