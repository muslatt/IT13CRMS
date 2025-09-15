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
            searchBox = new TextBox();
            rightControlsPanel = new Panel();
            btnFilter = new Button();
            sortComboBox = new ComboBox();
            btnAddProperty = new Button();
            searchPanel.SuspendLayout();
            rightControlsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel.Location = new Point(0, 60);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Padding = new Padding(20);
            flowLayoutPanel.Size = new Size(940, 540);
            flowLayoutPanel.TabIndex = 0;
            flowLayoutPanel.WrapContents = true;
            // 
            // searchPanel
            // 
            searchPanel.BackColor = Color.Transparent;
            searchPanel.Controls.Add(searchBox);
            searchPanel.Controls.Add(rightControlsPanel);
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Location = new Point(0, 0);
            searchPanel.Name = "searchPanel";
            searchPanel.Padding = new Padding(20, 15, 20, 15);
            searchPanel.Size = new Size(940, 60);
            searchPanel.TabIndex = 1;
            // 
            // searchBox
            // 
            searchBox.Font = new Font("Segoe UI", 10F);
            searchBox.Location = new Point(20, 15);
            searchBox.Name = "searchBox";
            searchBox.PlaceholderText = "Search";
            searchBox.Size = new Size(350, 25);
            searchBox.TabIndex = 0;
            // 
            // rightControlsPanel
            // 
            rightControlsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rightControlsPanel.BackColor = Color.Transparent;
            rightControlsPanel.Controls.Add(btnFilter);
            rightControlsPanel.Controls.Add(sortComboBox);
            rightControlsPanel.Controls.Add(btnAddProperty);
            rightControlsPanel.Location = new Point(520, 12);
            // Slightly larger panel to accommodate wider Add button
            rightControlsPanel.Size = new Size(420, 40);
            rightControlsPanel.TabIndex = 1;
            // 
            // btnFilter
            // 
            btnFilter.BackColor = Color.White;
            btnFilter.FlatAppearance.BorderColor = Color.FromArgb(206, 212, 218);
            btnFilter.FlatAppearance.BorderSize = 1;
            btnFilter.FlatStyle = FlatStyle.Flat;
            btnFilter.Font = new Font("Segoe UI", 12F);
            btnFilter.ForeColor = Color.FromArgb(108, 117, 125);
            btnFilter.Location = new Point(0, 0);
            btnFilter.Name = "btnFilter";
            // Make button wider so its content isn't clipped
            btnFilter.Size = new Size(100, 35);
            btnFilter.TabIndex = 0;
            btnFilter.Text = "📋 Filter";
            btnFilter.UseVisualStyleBackColor = false;
            // 
            // sortComboBox
            // 
            sortComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sortComboBox.Font = new Font("Segoe UI", 12F);
            sortComboBox.FormattingEnabled = true;
            sortComboBox.Items.AddRange(new object[] { "Newest to Oldest", "Price: Low to High", "Price: High to Low" });
            // Shift to the right of the filter button and give more height for 12pt font
            sortComboBox.Location = new Point(110, 0);
            sortComboBox.Name = "sortComboBox";
            sortComboBox.Size = new Size(160, 35);
            sortComboBox.TabIndex = 1;
            // 
            // btnAddProperty
            // 
            btnAddProperty.BackColor = Color.FromArgb(0, 123, 255);
            btnAddProperty.FlatAppearance.BorderSize = 0;
            btnAddProperty.FlatStyle = FlatStyle.Flat;
            btnAddProperty.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnAddProperty.ForeColor = Color.White;
            // Position to the right of the combo box and enlarge so full text is visible
            btnAddProperty.Location = new Point(280, 0);
            btnAddProperty.Name = "btnAddProperty";
            btnAddProperty.Size = new Size(130, 35);
            btnAddProperty.TabIndex = 2;
            btnAddProperty.Text = "+ Add Property";
            btnAddProperty.UseVisualStyleBackColor = false;
            btnAddProperty.Click += BtnAddProperty_Click;
            // 
            // PropertiesView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 249, 250);
            Controls.Add(flowLayoutPanel);
            Controls.Add(searchPanel);
            Name = "PropertiesView";
            Size = new Size(940, 600);
            searchPanel.ResumeLayout(false);
            searchPanel.PerformLayout();
            rightControlsPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel;
        private Panel searchPanel;
        private TextBox searchBox;
        private Panel rightControlsPanel;
        private Button btnFilter;
        private ComboBox sortComboBox;
        private Button btnAddProperty;
    }
}