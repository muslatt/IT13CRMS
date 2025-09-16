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
            searchPanel.SuspendLayout();
            searchBoxContainer.SuspendLayout();
            rightControlsPanel.SuspendLayout();
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
            // PropertiesView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(249, 250, 251);
            Controls.Add(flowLayoutPanel);
            Controls.Add(searchPanel);
            Name = "PropertiesView";
            Size = new Size(1000, 670);
            searchPanel.ResumeLayout(false);
            searchBoxContainer.ResumeLayout(false);
            searchBoxContainer.PerformLayout();
            rightControlsPanel.ResumeLayout(false);
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
    }
}