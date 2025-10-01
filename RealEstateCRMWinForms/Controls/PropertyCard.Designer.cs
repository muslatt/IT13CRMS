using System;
using System.Drawing;
using System.Windows.Forms;

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
            if (disposing)
            {
                // Properly dispose of the image to avoid file locks
                if (pictureBox?.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }

                // dispose the feature icons if any
                if (pbBedIcon?.Image != null)
                {
                    pbBedIcon.Image.Dispose();
                    pbBedIcon.Image = null;
                }
                if (pbBathIcon?.Image != null)
                {
                    pbBathIcon.Image.Dispose();
                    pbBathIcon.Image = null;
                }
                if (pbSqmIcon?.Image != null)
                {
                    pbSqmIcon.Image.Dispose();
                    pbSqmIcon.Image = null;
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        // Designer-created controls
        private PictureBox pictureBox;
        private Panel titlePanel;
        private Label lblTitle;
        private Panel statusPanel;
        private Label lblStatus;
        private Label lblAddress;
        private Panel detailsPanel;

        // New icon + value controls that PropertyCard.cs expects
        private PictureBox pbBedIcon;
        private Label lblBedValue;
        private PictureBox pbBathIcon;
        private Label lblBathValue;
        private PictureBox pbSqmIcon;
        private Label lblSqmValue;

        private Label lblPrice;

        // InitializeComponent to create and assign all controls
        private void InitializeComponent()
        {
            // container for potential designer components
            this.components = new System.ComponentModel.Container();

            // Basic UserControl sizing (wider card)
            this.SuspendLayout();
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Size = new Size(360, 320); // increased width from 280 to 360
            this.Margin = new Padding(8);

            // pictureBox (top)
            pictureBox = new PictureBox
            {
                Name = "pictureBox",
                Location = new Point(8, 8),
                Size = new Size(344, 180), // widened to match new card width with 8px margins
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 242, 245)
            };

            // titlePanel contains title and status badge
            titlePanel = new Panel
            {
                Name = "titlePanel",
                Location = new Point(8, 196),
                Size = new Size(344, 36), // widened
                BackColor = Color.Transparent
            };

            lblTitle = new Label
            {
                Name = "lblTitle",
                Location = new Point(0, 0),
                Size = new Size(260, 36), // give more width to the title
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoEllipsis = true,
                Text = "Title"
            };

            // status panel on the right within titlePanel
            statusPanel = new Panel
            {
                Name = "statusPanel",
                Size = new Size(68, 26),
                Location = new Point(titlePanel.Width - 78, 6),
                BackColor = Color.FromArgb(0, 123, 255),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // status label will autosize to its text and be padded so the badge fits longer values
            lblStatus = new Label
            {
                Name = "lblStatus",
                AutoSize = true,
                Padding = new Padding(8, 3, 8, 3),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Sell",
                AutoEllipsis = false
            };

            statusPanel.Controls.Add(lblStatus);
            titlePanel.Controls.Add(lblTitle);
            titlePanel.Controls.Add(statusPanel);

            // address label under title
            lblAddress = new Label
            {
                Name = "lblAddress",
                Location = new Point(8, 236),
                Size = new Size(344, 18),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(108, 117, 125),
                Text = "Address"
            };

            // detailsPanel holds icon + number pairs for bedrooms, bathrooms, sqm
            detailsPanel = new Panel
            {
                Name = "detailsPanel",
                Location = new Point(8, 258),
                Size = new Size(344, 28),
                BackColor = Color.Transparent
            };

            // Calculate even spacing for 3 features across 264px width
            // Each feature gets about 88px (264/3), with small margins
            int featureWidth = 110;  // wider features to space across the larger card
            int spaceBetween = 8;   // small gap between features

            // Bed feature: icon + value (left third)
            pbBedIcon = new PictureBox
            {
                Name = "pbBedIcon",
                Location = new Point(0, 0),
                Size = new Size(20, 28),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent
            };

            lblBedValue = new Label
            {
                Name = "lblBedValue",
                Location = new Point(22, 0),
                Size = new Size(featureWidth - 22, 28),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 54, 59),
                Text = "0",
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Bath feature: icon + value (middle third)
            int bathStartX = featureWidth + spaceBetween;
            pbBathIcon = new PictureBox
            {
                Name = "pbBathIcon",
                Location = new Point(bathStartX, 0),
                Size = new Size(20, 28),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent
            };

            lblBathValue = new Label
            {
                Name = "lblBathValue",
                Location = new Point(bathStartX + 22, 0),
                Size = new Size(featureWidth - 22, 28),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 54, 59),
                Text = "0",
                TextAlign = ContentAlignment.MiddleLeft
            };

            // SQM feature: icon + value + unit (right third)
            int sqmStartX = (featureWidth + spaceBetween) * 2;
            pbSqmIcon = new PictureBox
            {
                Name = "pbSqmIcon",
                Location = new Point(sqmStartX, 0),
                Size = new Size(20, 28),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent
            };

            lblSqmValue = new Label
            {
                Name = "lblSqmValue",
                Location = new Point(sqmStartX + 22, 0),
                Size = new Size(344 - sqmStartX - 22, 28), // use remaining width to fit "sqm"
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 54, 59),
                Text = "0 sqm",
                TextAlign = ContentAlignment.MiddleLeft
            };

            detailsPanel.Controls.Add(pbBedIcon);
            detailsPanel.Controls.Add(lblBedValue);
            detailsPanel.Controls.Add(pbBathIcon);
            detailsPanel.Controls.Add(lblBathValue);
            detailsPanel.Controls.Add(pbSqmIcon);
            detailsPanel.Controls.Add(lblSqmValue);

            // price label at bottom
            lblPrice = new Label
            {
                Name = "lblPrice",
                Location = new Point(8, 288),
                Size = new Size(344, 24),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "₱ 0"
            };

            // Add controls to the UserControl
            this.Controls.Add(pictureBox);
            this.Controls.Add(titlePanel);
            this.Controls.Add(lblAddress);
            this.Controls.Add(detailsPanel);
            this.Controls.Add(lblPrice);

            this.ResumeLayout(false);
        }
    }
}