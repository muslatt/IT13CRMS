// RealEstateCRMWinForms\Views\ContactsView.Designer.cs
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

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
            this.searchBoxContainer = new System.Windows.Forms.Panel();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchIcon = new System.Windows.Forms.Label();
            this.rightControlsPanel = new System.Windows.Forms.Panel();
            this.sortComboBox = new System.Windows.Forms.ComboBox();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.paginationPanel = new System.Windows.Forms.Panel();
            this.contactsPaginationLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnPrevContactPage = new System.Windows.Forms.Button();
            this.lblContactPageInfo = new System.Windows.Forms.Label();
            this.btnNextContactPage = new System.Windows.Forms.Button();
            this.dataGridViewContacts = new System.Windows.Forms.DataGridView();
            this.searchPanel.SuspendLayout();
            this.searchBoxContainer.SuspendLayout();
            this.rightControlsPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContacts)).BeginInit();
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
            this.searchPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.SearchPanel_Paint);
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
            this.searchBoxContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.SearchBoxContainer_Paint);
            // 
            // searchBox
            // 
            this.searchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.searchBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.searchBox.Location = new System.Drawing.Point(40, 10);
            this.searchBox.Name = "searchBox";
            this.searchBox.PlaceholderText = "Search contacts...";
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
            this.rightControlsPanel.Controls.Add(this.sortComboBox);
            this.rightControlsPanel.Location = new System.Drawing.Point(530, 20);
            this.rightControlsPanel.Name = "rightControlsPanel";
            this.rightControlsPanel.Size = new System.Drawing.Size(440, 40);
            this.rightControlsPanel.TabIndex = 1;
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
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.paginationPanel);
            this.contentPanel.Controls.Add(this.dataGridViewContacts);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 80);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(30);
            this.contentPanel.Size = new System.Drawing.Size(1000, 590);
            this.contentPanel.TabIndex = 1;
            // 
            // paginationPanel
            // 
            this.paginationPanel.BackColor = System.Drawing.Color.White;
            this.paginationPanel.Controls.Add(this.contactsPaginationLayout);
            this.paginationPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.paginationPanel.Location = new System.Drawing.Point(30, 530);
            this.paginationPanel.Name = "paginationPanel";
            this.paginationPanel.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.paginationPanel.Size = new System.Drawing.Size(940, 40);
            this.paginationPanel.TabIndex = 2;
            // 
            // contactsPaginationLayout
            // 
            this.contactsPaginationLayout.ColumnCount = 4;
            this.contactsPaginationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contactsPaginationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.contactsPaginationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.contactsPaginationLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.contactsPaginationLayout.Controls.Add(this.lblContactPageInfo, 1, 0);
            this.contactsPaginationLayout.Controls.Add(this.btnPrevContactPage, 2, 0);
            this.contactsPaginationLayout.Controls.Add(this.btnNextContactPage, 3, 0);
            this.contactsPaginationLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contactsPaginationLayout.Location = new System.Drawing.Point(10, 8);
            this.contactsPaginationLayout.Name = "contactsPaginationLayout";
            this.contactsPaginationLayout.RowCount = 1;
            this.contactsPaginationLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contactsPaginationLayout.Size = new System.Drawing.Size(920, 24);
            this.contactsPaginationLayout.TabIndex = 0;
            // 
            // btnPrevContactPage
            // 
            this.btnPrevContactPage.AutoSize = true;
            this.btnPrevContactPage.BackColor = System.Drawing.Color.White;
            this.btnPrevContactPage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnPrevContactPage.FlatAppearance.BorderSize = 1;
            this.btnPrevContactPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrevContactPage.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnPrevContactPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnPrevContactPage.Location = new System.Drawing.Point(816, 0);
            this.btnPrevContactPage.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnPrevContactPage.Name = "btnPrevContactPage";
            this.btnPrevContactPage.Size = new System.Drawing.Size(90, 28);
            this.btnPrevContactPage.TabIndex = 0;
            this.btnPrevContactPage.Text = "Previous";
            this.btnPrevContactPage.UseVisualStyleBackColor = false;
            this.btnPrevContactPage.Click += new System.EventHandler(this.BtnPrevContactPage_Click);
            // 
            // lblContactPageInfo
            // 
            this.lblContactPageInfo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblContactPageInfo.AutoSize = true;
            this.lblContactPageInfo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblContactPageInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.lblContactPageInfo.Location = new System.Drawing.Point(720, 2);
            this.lblContactPageInfo.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.lblContactPageInfo.Name = "lblContactPageInfo";
            this.lblContactPageInfo.Size = new System.Drawing.Size(90, 19);
            this.lblContactPageInfo.TabIndex = 1;
            this.lblContactPageInfo.Text = "Page 1 of 1";
            this.lblContactPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNextContactPage
            // 
            this.btnNextContactPage.AutoSize = true;
            this.btnNextContactPage.BackColor = System.Drawing.Color.White;
            this.btnNextContactPage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(213)))), ((int)(((byte)(219)))));
            this.btnNextContactPage.FlatAppearance.BorderSize = 1;
            this.btnNextContactPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextContactPage.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnNextContactPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnNextContactPage.Location = new System.Drawing.Point(912, 0);
            this.btnNextContactPage.Margin = new System.Windows.Forms.Padding(0);
            this.btnNextContactPage.Name = "btnNextContactPage";
            this.btnNextContactPage.Size = new System.Drawing.Size(90, 28);
            this.btnNextContactPage.TabIndex = 2;
            this.btnNextContactPage.Text = "Next";
            this.btnNextContactPage.UseVisualStyleBackColor = false;
            this.btnNextContactPage.Click += new System.EventHandler(this.BtnNextContactPage_Click);
            // 
            // dataGridViewContacts
            // 
            this.dataGridViewContacts.AllowUserToAddRows = false;
            this.dataGridViewContacts.AllowUserToDeleteRows = false;
            this.dataGridViewContacts.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewContacts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewContacts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewContacts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewContacts.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.dataGridViewContacts.Location = new System.Drawing.Point(30, 30);
            this.dataGridViewContacts.Name = "dataGridViewContacts";
            this.dataGridViewContacts.ReadOnly = true;
            this.dataGridViewContacts.Size = new System.Drawing.Size(940, 530);
            this.dataGridViewContacts.TabIndex = 0;
            // 
            // ContactsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.searchPanel);
            this.Name = "ContactsView";
            this.Size = new System.Drawing.Size(1000, 670);
            this.Load += new System.EventHandler(this.ContactsView_Load);
            this.searchPanel.ResumeLayout(false);
            this.searchBoxContainer.ResumeLayout(false);
            this.searchBoxContainer.PerformLayout();
            this.rightControlsPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContacts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Panel rightControlsPanel;
        private System.Windows.Forms.ComboBox sortComboBox;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel paginationPanel;
        private System.Windows.Forms.TableLayoutPanel contactsPaginationLayout;
        private System.Windows.Forms.Button btnPrevContactPage;
        private System.Windows.Forms.Button btnNextContactPage;
        private System.Windows.Forms.Label lblContactPageInfo;
        private System.Windows.Forms.DataGridView dataGridViewContacts;
        private System.Windows.Forms.Panel searchBoxContainer;
        private System.Windows.Forms.Label searchIcon;

        private void ContactsView_Load(object sender, System.EventArgs e)
        {
            // Apply rounded corners to the search box container
            searchBoxContainer.Paint += SearchBoxContainer_Paint;
        }

        private void SearchBoxContainer_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                GraphicsPath path = new GraphicsPath();
                int radius = 20;
                Rectangle rect = new Rectangle(0, 0, panel.Width, panel.Height);

                // Create rounded rectangle
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                panel.Region = new Region(path);
            }
        }

        private void SearchPanel_Paint(object sender, PaintEventArgs e)
        {
            // Empty method to prevent designer errors
        }
    }
}
