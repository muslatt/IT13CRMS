namespace RealEstateCRMWinForms.Views
{
    partial class DashboardView
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
            headerPanel = new Panel();
            lblLastUpdated = new Label();
            lblSubtitle = new Label();
            lblTitle = new Label();
            mainPanel = new Panel();
            analyticsPanel = new TableLayoutPanel();
            revenueAnalyticsCard = new Panel();
            propertyAnalyticsCard = new Panel();
            performanceAnalyticsCard = new Panel();
            trendsAnalyticsCard = new Panel();
            totalCountsPanel = new TableLayoutPanel();
            totalCountsCard = new Panel();
            chartsPanel = new TableLayoutPanel();
            salesChartCard = new Panel();
            propertyStatusChartCard = new Panel();
            leadConversionChartCard = new Panel();
            avgSalaryChartCard = new Panel();
            occupationPreferenceCard = new Panel();
            kpiPanel = new TableLayoutPanel();
            conversionRateCard = new Panel();
            projectedRevenueCard = new Panel();
            headerPanel.SuspendLayout();
            mainPanel.SuspendLayout();
            analyticsPanel.SuspendLayout();
            totalCountsPanel.SuspendLayout();
            chartsPanel.SuspendLayout();
            kpiPanel.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.White;
            headerPanel.Controls.Add(lblLastUpdated);
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Padding = new Padding(30, 25, 30, 25);
            headerPanel.Size = new Size(1400, 90);
            headerPanel.TabIndex = 0;
            // 
            // lblLastUpdated
            // 
            lblLastUpdated.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblLastUpdated.AutoSize = true;
            lblLastUpdated.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLastUpdated.ForeColor = Color.FromArgb(107, 114, 128);
            lblLastUpdated.Location = new Point(1180, 35);
            lblLastUpdated.Name = "lblLastUpdated";
            lblLastUpdated.Size = new Size(205, 17);
            lblLastUpdated.TabIndex = 2;
            lblLastUpdated.Text = "Last updated: Sept 20, 2025 00:05";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSubtitle.ForeColor = Color.FromArgb(107, 114, 128);
            lblSubtitle.Location = new Point(33, 55);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(298, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Real-time insights and performance metrics";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.FromArgb(17, 24, 39);
            lblTitle.Location = new Point(30, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(281, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Analytics Dashboard";
            // 
            // mainPanel
            // 
            mainPanel.AutoScroll = true;
            mainPanel.BackColor = Color.FromArgb(249, 250, 251);
            mainPanel.Controls.Add(analyticsPanel);
            mainPanel.Controls.Add(totalCountsPanel);
            mainPanel.Controls.Add(occupationPreferenceCard);
            mainPanel.Controls.Add(avgSalaryChartCard);
            mainPanel.Controls.Add(chartsPanel);
            mainPanel.Controls.Add(kpiPanel);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 90);
            mainPanel.Name = "mainPanel";
            mainPanel.Padding = new Padding(30);
            mainPanel.Size = new Size(1400, 810);
            mainPanel.TabIndex = 1;
            // 
            // kpiPanel
            // 
            kpiPanel.ColumnCount = 2;
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            kpiPanel.Controls.Add(conversionRateCard, 0, 0);
            kpiPanel.Controls.Add(projectedRevenueCard, 1, 0);
            kpiPanel.Dock = DockStyle.Top;
            kpiPanel.Location = new Point(30, 30);
            kpiPanel.Name = "kpiPanel";
            kpiPanel.RowCount = 1;
            kpiPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            kpiPanel.Size = new Size(1340, 180);
            kpiPanel.TabIndex = 0;
            // 
            // conversionRateCard
            // 
            conversionRateCard.BackColor = Color.White;
            conversionRateCard.BorderStyle = BorderStyle.FixedSingle;
            conversionRateCard.Dock = DockStyle.Fill;
            conversionRateCard.Location = new Point(0, 0);
            conversionRateCard.Margin = new Padding(0, 0, 3, 0);
            conversionRateCard.Name = "conversionRateCard";
            conversionRateCard.Padding = new Padding(20);
            conversionRateCard.Size = new Size(667, 180);
            conversionRateCard.TabIndex = 0;
            // 
            // projectedRevenueCard
            // 
            projectedRevenueCard.BackColor = Color.White;
            projectedRevenueCard.BorderStyle = BorderStyle.FixedSingle;
            projectedRevenueCard.Dock = DockStyle.Fill;
            projectedRevenueCard.Location = new Point(673, 0);
            projectedRevenueCard.Margin = new Padding(3, 0, 0, 0);
            projectedRevenueCard.Name = "projectedRevenueCard";
            projectedRevenueCard.Padding = new Padding(20);
            projectedRevenueCard.Size = new Size(667, 180);
            projectedRevenueCard.TabIndex = 1;
            // 
            // chartsPanel
            // 
            chartsPanel.ColumnCount = 2;
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            // Updated: Only one row (Lead Conversion left, Property Status right) to remove vertical gap.
            chartsPanel.Controls.Add(leadConversionChartCard, 0, 0);
            chartsPanel.Controls.Add(propertyStatusChartCard, 1, 0);
            chartsPanel.Dock = DockStyle.Top;
            chartsPanel.Location = new Point(30, 210);
            chartsPanel.Name = "chartsPanel";
            chartsPanel.RowCount = 1;
            chartsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 240F));
            chartsPanel.Size = new Size(1340, 240);
            chartsPanel.TabIndex = 1;
            // 
            // leadConversionChartCard (moved to top-left)
            // 
            leadConversionChartCard.BackColor = Color.White;
            leadConversionChartCard.BorderStyle = BorderStyle.FixedSingle;
            leadConversionChartCard.Dock = DockStyle.Fill;
            leadConversionChartCard.Location = new Point(0, 0);
            leadConversionChartCard.Margin = new Padding(0, 0, 3, 3);
            leadConversionChartCard.Name = "leadConversionChartCard";
            leadConversionChartCard.Padding = new Padding(20);
            leadConversionChartCard.Size = new Size(667, 237);
            leadConversionChartCard.TabIndex = 0;
            // 
            // propertyStatusChartCard (top-right)
            // 
            propertyStatusChartCard.BackColor = Color.White;
            propertyStatusChartCard.BorderStyle = BorderStyle.FixedSingle;
            propertyStatusChartCard.Dock = DockStyle.Fill;
            propertyStatusChartCard.Location = new Point(673, 0);
            propertyStatusChartCard.Margin = new Padding(3, 0, 0, 3);
            propertyStatusChartCard.Name = "propertyStatusChartCard";
            propertyStatusChartCard.Padding = new Padding(20);
            propertyStatusChartCard.Size = new Size(667, 237);
            propertyStatusChartCard.TabIndex = 1;
            // 
            // salesChartCard retained (not displayed) - removed from panel to reclaim space. Leave variable for potential future use.
            // 
            // avgSalaryChartCard
            // 
            avgSalaryChartCard.BackColor = Color.White;
            avgSalaryChartCard.BorderStyle = BorderStyle.FixedSingle;
            avgSalaryChartCard.Dock = DockStyle.Top;
            // Move up to immediately follow chartsPanel (210 + 240 = 450)
            avgSalaryChartCard.Location = new Point(30, 450);
            avgSalaryChartCard.Name = "avgSalaryChartCard";
            avgSalaryChartCard.Padding = new Padding(20);
            avgSalaryChartCard.Size = new Size(1340, 220);
            avgSalaryChartCard.TabIndex = 2;
            // 
            // occupationPreferenceCard
            // 
            occupationPreferenceCard.BackColor = Color.White;
            occupationPreferenceCard.BorderStyle = BorderStyle.FixedSingle;
            occupationPreferenceCard.Dock = DockStyle.Top;
            occupationPreferenceCard.Location = new Point(30, 670);
            occupationPreferenceCard.Name = "occupationPreferenceCard";
            occupationPreferenceCard.Padding = new Padding(20);
            occupationPreferenceCard.Size = new Size(1340, 220);
            occupationPreferenceCard.TabIndex = 3;
            // 
            // totalCountsPanel
            // 
            totalCountsPanel.ColumnCount = 1;
            totalCountsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            totalCountsPanel.Controls.Add(totalCountsCard, 0, 0);
            totalCountsPanel.Dock = DockStyle.Top;
            totalCountsPanel.Location = new Point(30, 890);
            totalCountsPanel.Name = "totalCountsPanel";
            totalCountsPanel.RowCount = 1;
            totalCountsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            totalCountsPanel.Size = new Size(1340, 280);
            totalCountsPanel.TabIndex = 4;
            // 
            // totalCountsCard
            // 
            totalCountsCard.BackColor = Color.White;
            totalCountsCard.BorderStyle = BorderStyle.FixedSingle;
            totalCountsCard.Dock = DockStyle.Fill;
            totalCountsCard.Location = new Point(0, 0);
            totalCountsCard.Margin = new Padding(0);
            totalCountsCard.Name = "totalCountsCard";
            totalCountsCard.Padding = new Padding(20);
            totalCountsCard.Size = new Size(1340, 280);
            totalCountsCard.TabIndex = 0;
            // 
            // analyticsPanel
            // 
            // Adjusted layout: shift Property, Performance, Market Trends to the left.
            // Keep revenueAnalyticsCard (first slot) hidden/reserved in case it's reintroduced.
            analyticsPanel.ColumnCount = 4;
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            analyticsPanel.Controls.Add(propertyAnalyticsCard, 0, 0);
            analyticsPanel.Controls.Add(performanceAnalyticsCard, 1, 0);
            analyticsPanel.Controls.Add(trendsAnalyticsCard, 2, 0);
            analyticsPanel.Controls.Add(revenueAnalyticsCard, 3, 0); // moved to the far right (hidden later in code)
            analyticsPanel.Dock = DockStyle.Top;
            analyticsPanel.Location = new Point(30, 1170);
            analyticsPanel.Name = "analyticsPanel";
            analyticsPanel.RowCount = 1;
            analyticsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            analyticsPanel.Size = new Size(1340, 200);
            analyticsPanel.TabIndex = 5;
            // 
            // revenueAnalyticsCard
            // 
            revenueAnalyticsCard.BackColor = Color.White;
            revenueAnalyticsCard.BorderStyle = BorderStyle.FixedSingle;
            revenueAnalyticsCard.Dock = DockStyle.Fill;
            // Moved to last column (index 3)
            revenueAnalyticsCard.Location = new Point(1008, 0);
            revenueAnalyticsCard.Margin = new Padding(3, 0, 0, 0);
            revenueAnalyticsCard.Name = "revenueAnalyticsCard";
            revenueAnalyticsCard.Padding = new Padding(20);
            revenueAnalyticsCard.Size = new Size(332, 200);
            revenueAnalyticsCard.TabIndex = 3;
            // 
            // propertyAnalyticsCard
            // 
            propertyAnalyticsCard.BackColor = Color.White;
            propertyAnalyticsCard.BorderStyle = BorderStyle.FixedSingle;
            propertyAnalyticsCard.Dock = DockStyle.Fill;
            propertyAnalyticsCard.Location = new Point(0, 0);
            propertyAnalyticsCard.Margin = new Padding(0, 0, 3, 0);
            propertyAnalyticsCard.Name = "propertyAnalyticsCard";
            propertyAnalyticsCard.Padding = new Padding(20);
            propertyAnalyticsCard.Size = new Size(329, 200);
            propertyAnalyticsCard.TabIndex = 1;
            // 
            // performanceAnalyticsCard
            // 
            performanceAnalyticsCard.BackColor = Color.White;
            performanceAnalyticsCard.BorderStyle = BorderStyle.FixedSingle;
            performanceAnalyticsCard.Dock = DockStyle.Fill;
            performanceAnalyticsCard.Location = new Point(338, 0);
            performanceAnalyticsCard.Margin = new Padding(3, 0, 3, 0);
            performanceAnalyticsCard.Name = "performanceAnalyticsCard";
            performanceAnalyticsCard.Padding = new Padding(20);
            performanceAnalyticsCard.Size = new Size(329, 200);
            performanceAnalyticsCard.TabIndex = 2;
            // 
            // trendsAnalyticsCard
            // 
            trendsAnalyticsCard.BackColor = Color.White;
            trendsAnalyticsCard.BorderStyle = BorderStyle.FixedSingle;
            trendsAnalyticsCard.Dock = DockStyle.Fill;
            trendsAnalyticsCard.Location = new Point(673, 0);
            trendsAnalyticsCard.Margin = new Padding(3, 0, 3, 0);
            trendsAnalyticsCard.Name = "trendsAnalyticsCard";
            trendsAnalyticsCard.Padding = new Padding(20);
            trendsAnalyticsCard.Size = new Size(332, 200);
            trendsAnalyticsCard.TabIndex = 3;
            // 
            // DashboardView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(249, 250, 251);
            Controls.Add(mainPanel);
            Controls.Add(headerPanel);
            Name = "DashboardView";
            Size = new Size(1400, 900);
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            mainPanel.ResumeLayout(false);
            analyticsPanel.ResumeLayout(false);
            totalCountsPanel.ResumeLayout(false);
            chartsPanel.ResumeLayout(false);
            kpiPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblLastUpdated;
        private System.Windows.Forms.Panel mainPanel;

        // KPI Components (only remaining 2 cards)
        private System.Windows.Forms.TableLayoutPanel kpiPanel;
        private System.Windows.Forms.Panel conversionRateCard;
        private System.Windows.Forms.Panel projectedRevenueCard;

        // Charts Components
        private System.Windows.Forms.TableLayoutPanel chartsPanel;
        private System.Windows.Forms.Panel salesChartCard;
        private System.Windows.Forms.Panel propertyStatusChartCard;
        private System.Windows.Forms.Panel leadConversionChartCard;
        private System.Windows.Forms.Panel avgSalaryChartCard;
        private System.Windows.Forms.Panel occupationPreferenceCard;

        // System Overview
        private System.Windows.Forms.TableLayoutPanel totalCountsPanel;
        private System.Windows.Forms.Panel totalCountsCard;

        // Analytics Components
        private System.Windows.Forms.TableLayoutPanel analyticsPanel;
        private System.Windows.Forms.Panel revenueAnalyticsCard;
        private System.Windows.Forms.Panel propertyAnalyticsCard;
        private System.Windows.Forms.Panel performanceAnalyticsCard;
        private System.Windows.Forms.Panel trendsAnalyticsCard;
    }
}