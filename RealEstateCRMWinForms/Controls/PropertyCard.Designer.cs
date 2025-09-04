namespace RealEstateCRMWinForms.Controls
{
    partial class PropertyCard
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
            pictureBox = new PictureBox();
            statusPanel = new Panel();
            lblStatus = new Label();
            lblTitle = new Label();
            lblAddress = new Label();
            detailsPanel = new Panel();
            lblBedrooms = new Label();
            lblBathrooms = new Label();
            lblSquareMeters = new Label();
            lblPrice = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            statusPanel.SuspendLayout();
            detailsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.BackColor = Color.LightGray;
            pictureBox.Controls.Add(statusPanel);
            pictureBox.Location = new Point(8, 8);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(264, 180);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            // 
            // statusPanel
            // 
            statusPanel.BackColor = Color.FromArgb(0, 123, 255);
            statusPanel.Controls.Add(lblStatus);
            statusPanel.Location = new Point(16, 16);
            statusPanel.Name = "statusPanel";
            statusPanel.Size = new Size(60, 24);
            statusPanel.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblStatus.ForeColor = Color.White;
            lblStatus.Location = new Point(0, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(60, 24);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Sell";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(33, 37, 41);
            lblTitle.Location = new Point(8, 196);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(264, 24);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Property Title";
            // 
            // lblAddress
            // 
            lblAddress.Font = new Font("Segoe UI", 9F);
            lblAddress.ForeColor = Color.FromArgb(108, 117, 125);
            lblAddress.Location = new Point(8, 220);
            lblAddress.Name = "lblAddress";
            lblAddress.Size = new Size(264, 20);
            lblAddress.TabIndex = 2;
            lblAddress.Text = "Property Address";
            // 
            // detailsPanel
            // 
            detailsPanel.BackColor = Color.Transparent;
            detailsPanel.Controls.Add(lblBedrooms);
            detailsPanel.Controls.Add(lblBathrooms);
            detailsPanel.Controls.Add(lblSquareMeters);
            detailsPanel.Location = new Point(8, 248);
            detailsPanel.Name = "detailsPanel";
            detailsPanel.Size = new Size(264, 32);
            detailsPanel.TabIndex = 3;
            // 
            // lblBedrooms
            // 
            lblBedrooms.Font = new Font("Segoe UI", 9F);
            lblBedrooms.ForeColor = Color.FromArgb(108, 117, 125);
            lblBedrooms.Location = new Point(0, 8);
            lblBedrooms.Name = "lblBedrooms";
            lblBedrooms.Size = new Size(60, 16);
            lblBedrooms.TabIndex = 0;
            lblBedrooms.Text = "🛏️ 4";
            // 
            // lblBathrooms
            // 
            lblBathrooms.Font = new Font("Segoe UI", 9F);
            lblBathrooms.ForeColor = Color.FromArgb(108, 117, 125);
            lblBathrooms.Location = new Point(70, 8);
            lblBathrooms.Name = "lblBathrooms";
            lblBathrooms.Size = new Size(60, 16);
            lblBathrooms.TabIndex = 1;
            lblBathrooms.Text = "🚿 2";
            // 
            // lblSquareMeters
            // 
            lblSquareMeters.Font = new Font("Segoe UI", 9F);
            lblSquareMeters.ForeColor = Color.FromArgb(108, 117, 125);
            lblSquareMeters.Location = new Point(140, 8);
            lblSquareMeters.Name = "lblSquareMeters";
            lblSquareMeters.Size = new Size(80, 16);
            lblSquareMeters.TabIndex = 2;
            lblSquareMeters.Text = "📐 99 sqm";
            // 
            // lblPrice
            // 
            lblPrice.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblPrice.ForeColor = Color.FromArgb(33, 37, 41);
            lblPrice.Location = new Point(8, 288);
            lblPrice.Name = "lblPrice";
            lblPrice.Size = new Size(264, 24);
            lblPrice.TabIndex = 4;
            lblPrice.Text = "₱ 9,999,999";
            // 
            // PropertyCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(lblPrice);
            Controls.Add(detailsPanel);
            Controls.Add(lblAddress);
            Controls.Add(lblTitle);
            Controls.Add(pictureBox);
            Name = "PropertyCard";
            Padding = new Padding(8);
            Size = new Size(280, 360);
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            statusPanel.ResumeLayout(false);
            detailsPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox;
        private Panel statusPanel;
        private Label lblStatus;
        private Label lblTitle;
        private Label lblAddress;
        private Panel detailsPanel;
        private Label lblBedrooms;
        private Label lblBathrooms;
        private Label lblSquareMeters;
        private Label lblPrice;
    }
}