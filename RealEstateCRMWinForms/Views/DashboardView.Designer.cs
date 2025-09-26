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
            totalRevenueCard = new Panel();
            avgDealValueCard = new Panel();
            conversionRateCard = new Panel();
            avgDaysToCloseCard = new Panel();
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
            lblLastUpdated.Location = new Point(800, 35);
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
            // Order: Top to bottom -> KPI, Charts, Avg Salary (full width), Occupation Preference (full width), System Overview, Analytics
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
            // analyticsPanel
            // 
            analyticsPanel.ColumnCount = 2;
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            analyticsPanel.Controls.Add(revenueAnalyticsCard, 0, 0);
            analyticsPanel.Controls.Add(propertyAnalyticsCard, 1, 0);
            analyticsPanel.Controls.Add(performanceAnalyticsCard, 0, 1);
            analyticsPanel.Controls.Add(trendsAnalyticsCard, 1, 1);
            analyticsPanel.Dock = DockStyle.Top;
            analyticsPanel.Location = new Point(30, 910);
            analyticsPanel.Name = "analyticsPanel";
            analyticsPanel.RowCount = 2;
            analyticsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            analyticsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            analyticsPanel.Size = new Size(1323, 400);
            analyticsPanel.TabIndex = 3;
            // 
            // revenueAnalyticsCard
            // 
            revenueAnalyticsCard.BackColor = Color.White;
            revenueAnalyticsCard.BorderStyle = BorderStyle.FixedSingle;
            revenueAnalyticsCard.Dock = DockStyle.Fill;
            revenueAnalyticsCard.Location = new Point(12, 12);
            revenueAnalyticsCard.Margin = new Padding(12);
            revenueAnalyticsCard.Name = "revenueAnalyticsCard";
            revenueAnalyticsCard.Padding = new Padding(20);
            revenueAnalyticsCard.Size = new Size(637, 176);
            revenueAnalyticsCard.TabIndex = 0;
            // 
            // propertyAnalyticsCard
            // 
            propertyAnalyticsCard.BackColor = Color.White;
            propertyAnalyticsCard.BorderStyle = BorderStyle.FixedSingle;
            propertyAnalyticsCard.Dock = DockStyle.Fill;
            propertyAnalyticsCard.Location = new Point(673, 12);
            propertyAnalyticsCard.Margin = new Padding(12);
            propertyAnalyticsCard.Name = "propertyAnalyticsCard";
            propertyAnalyticsCard.Padding = new Padding(20);
            propertyAnalyticsCard.Size = new Size(638, 176);
            propertyAnalyticsCard.TabIndex = 1;
            // 
            // performanceAnalyticsCard
            // 
            performanceAnalyticsCard.BackColor = Color.White;
            performanceAnalyticsCard.BorderStyle = BorderStyle.FixedSingle;
            performanceAnalyticsCard.Dock = DockStyle.Fill;
            performanceAnalyticsCard.Location = new Point(12, 212);
            performanceAnalyticsCard.Margin = new Padding(12);
            performanceAnalyticsCard.Name = "performanceAnalyticsCard";
            performanceAnalyticsCard.Padding = new Padding(20);
            performanceAnalyticsCard.Size = new Size(637, 176);
            performanceAnalyticsCard.TabIndex = 2;
            // 
            // trendsAnalyticsCard
            // 
            trendsAnalyticsCard.BackColor = Color.White;
            trendsAnalyticsCard.BorderStyle = BorderStyle.FixedSingle;
            trendsAnalyticsCard.Dock = DockStyle.Fill;
            trendsAnalyticsCard.Location = new Point(673, 212);
            trendsAnalyticsCard.Margin = new Padding(12);
            trendsAnalyticsCard.Name = "trendsAnalyticsCard";
            trendsAnalyticsCard.Padding = new Padding(20);
            trendsAnalyticsCard.Size = new Size(638, 176);
            trendsAnalyticsCard.TabIndex = 3;
            // 
            // totalCountsPanel
            // 
            totalCountsPanel.ColumnCount = 1;
            totalCountsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            totalCountsPanel.Controls.Add(totalCountsCard, 0, 0);
            totalCountsPanel.Dock = DockStyle.Top;
            totalCountsPanel.Location = new Point(30, 690);
            totalCountsPanel.Name = "totalCountsPanel";
            totalCountsPanel.RowCount = 1;
            totalCountsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            totalCountsPanel.Size = new Size(1323, 280);
            totalCountsPanel.TabIndex = 2;
            // 
            // totalCountsCard
            // 
            totalCountsCard.BackColor = Color.White;
            totalCountsCard.BorderStyle = BorderStyle.FixedSingle;
            totalCountsCard.Dock = DockStyle.Fill;
            totalCountsCard.Location = new Point(12, 12);
            totalCountsCard.Margin = new Padding(12);
            totalCountsCard.Name = "totalCountsCard";
            totalCountsCard.Padding = new Padding(20);
            totalCountsCard.Size = new Size(1299, 256);
            totalCountsCard.TabIndex = 0;
            totalCountsCard.Paint += totalCountsCard_Paint;
            // 
            // chartsPanel (keeps three charts only)
            // 
            chartsPanel.ColumnCount = 2;
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            chartsPanel.Controls.Add(salesChartCard, 0, 0);
            chartsPanel.Controls.Add(propertyStatusChartCard, 1, 0);
            chartsPanel.Controls.Add(leadConversionChartCard, 0, 1);
            // removed avgSalaryChartCard from chartsPanel
            chartsPanel.Dock = DockStyle.Top;
            chartsPanel.Location = new Point(30, 211);
            chartsPanel.Name = "chartsPanel";
            chartsPanel.RowCount = 2;
            chartsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 260F));
            chartsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 260F));
            chartsPanel.Size = new Size(1323, 544);
            chartsPanel.TabIndex = 1;
            // 
            // salesChartCard
            // 
            salesChartCard.BackColor = Color.White;
            salesChartCard.BorderStyle = BorderStyle.FixedSingle;
            salesChartCard.Dock = DockStyle.Fill;
            salesChartCard.Location = new Point(12, 12);
            salesChartCard.Margin = new Padding(12);
            salesChartCard.Name = "salesChartCard";
            salesChartCard.Padding = new Padding(20);
            salesChartCard.Size = new Size(649, 236);
            salesChartCard.MinimumSize = new Size(0, 236);
            salesChartCard.TabIndex = 0;
            // 
            // propertyStatusChartCard
            // 
            propertyStatusChartCard.BackColor = Color.White;
            propertyStatusChartCard.BorderStyle = BorderStyle.FixedSingle;
            propertyStatusChartCard.Dock = DockStyle.Fill;
            propertyStatusChartCard.Location = new Point(673, 12);
            propertyStatusChartCard.Margin = new Padding(12);
            propertyStatusChartCard.Name = "propertyStatusChartCard";
            propertyStatusChartCard.Padding = new Padding(20);
            propertyStatusChartCard.Size = new Size(638, 236);
            propertyStatusChartCard.MinimumSize = new Size(0, 236);
            propertyStatusChartCard.TabIndex = 1;
            // 
            // leadConversionChartCard
            // 
            leadConversionChartCard.BackColor = Color.White;
            leadConversionChartCard.BorderStyle = BorderStyle.FixedSingle;
            leadConversionChartCard.Dock = DockStyle.Fill;
            leadConversionChartCard.Location = new Point(12, 141);
            leadConversionChartCard.Margin = new Padding(12);
            leadConversionChartCard.Name = "leadConversionChartCard";
            leadConversionChartCard.Padding = new Padding(20);
            leadConversionChartCard.Size = new Size(649, 236);
            leadConversionChartCard.MinimumSize = new Size(0, 236);
            leadConversionChartCard.TabIndex = 2;
            // 
            // avgSalaryChartCard (now full width like System Overview)
            // 
            avgSalaryChartCard.BackColor = Color.White;
            avgSalaryChartCard.BorderStyle = BorderStyle.FixedSingle;
            avgSalaryChartCard.Dock = DockStyle.Top;
            avgSalaryChartCard.Location = new Point(30, 470);
            avgSalaryChartCard.Margin = new Padding(12);
            avgSalaryChartCard.Name = "avgSalaryChartCard";
            avgSalaryChartCard.Padding = new Padding(20);
            avgSalaryChartCard.Size = new Size(1323, 220); // match System Overview height
            avgSalaryChartCard.TabIndex = 4;
            // 
            // occupationPreferenceCard (full width like avg salary)
            // 
            occupationPreferenceCard.BackColor = Color.White;
            occupationPreferenceCard.BorderStyle = BorderStyle.FixedSingle;
            occupationPreferenceCard.Dock = DockStyle.Top;
            occupationPreferenceCard.Location = new Point(30, 690);
            occupationPreferenceCard.Margin = new Padding(12);
            occupationPreferenceCard.Name = "occupationPreferenceCard";
            occupationPreferenceCard.Padding = new Padding(20);
            occupationPreferenceCard.Size = new Size(1323, 220);
            occupationPreferenceCard.TabIndex = 5;
            // 
            // kpiPanel
            // 
            kpiPanel.ColumnCount = 5;
            kpiPanel.ColumnStyles.Clear();
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            kpiPanel.Controls.Add(totalRevenueCard, 0, 0);
            kpiPanel.Controls.Add(avgDealValueCard, 1, 0);
            kpiPanel.Controls.Add(conversionRateCard, 2, 0);
            kpiPanel.Controls.Add(avgDaysToCloseCard, 3, 0);
            kpiPanel.Controls.Add(projectedRevenueCard, 4, 0);
            kpiPanel.Dock = DockStyle.Top;
            kpiPanel.Location = new Point(30, 30);
            kpiPanel.Name = "kpiPanel";
            kpiPanel.RowCount = 1;
            kpiPanel.RowStyles.Clear();
            kpiPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            kpiPanel.Size = new Size(1323, 181);
            kpiPanel.TabIndex = 0;
            // 
            // totalRevenueCard
            // 
            totalRevenueCard.BackColor = Color.White;
            totalRevenueCard.BorderStyle = BorderStyle.FixedSingle;
            totalRevenueCard.Dock = DockStyle.Fill;
            totalRevenueCard.Location = new Point(12, 12);
            totalRevenueCard.Margin = new Padding(12);
            totalRevenueCard.Name = "totalRevenueCard";
            totalRevenueCard.Padding = new Padding(20);
            totalRevenueCard.Size = new Size(306, 157);
            totalRevenueCard.TabIndex = 0;
            // 
            // avgDealValueCard
            // 
            avgDealValueCard.BackColor = Color.White;
            avgDealValueCard.BorderStyle = BorderStyle.FixedSingle;
            avgDealValueCard.Dock = DockStyle.Fill;
            avgDealValueCard.Location = new Point(342, 12);
            avgDealValueCard.Margin = new Padding(12);
            avgDealValueCard.Name = "avgDealValueCard";
            avgDealValueCard.Padding = new Padding(20);
            avgDealValueCard.Size = new Size(306, 157);
            avgDealValueCard.TabIndex = 1;
            // 
            // conversionRateCard
            // 
            conversionRateCard.BackColor = Color.White;
            conversionRateCard.BorderStyle = BorderStyle.FixedSingle;
            conversionRateCard.Dock = DockStyle.Fill;
            conversionRateCard.Location = new Point(672, 12);
            conversionRateCard.Margin = new Padding(12);
            conversionRateCard.Name = "conversionRateCard";
            conversionRateCard.Padding = new Padding(20);
            conversionRateCard.Size = new Size(306, 157);
            conversionRateCard.TabIndex = 2;
            // 
            // avgDaysToCloseCard
            // 
            avgDaysToCloseCard.BackColor = Color.White;
            avgDaysToCloseCard.BorderStyle = BorderStyle.FixedSingle;
            avgDaysToCloseCard.Dock = DockStyle.Fill;
            avgDaysToCloseCard.Location = new Point(1002, 12);
            avgDaysToCloseCard.Margin = new Padding(12);
            avgDaysToCloseCard.Name = "avgDaysToCloseCard";
            avgDaysToCloseCard.Padding = new Padding(20);
            avgDaysToCloseCard.Size = new Size(309, 157);
            avgDaysToCloseCard.TabIndex = 3;

            // projectedRevenueCard
            projectedRevenueCard.BackColor = Color.White;
            projectedRevenueCard.BorderStyle = BorderStyle.FixedSingle;
            projectedRevenueCard.Dock = DockStyle.Fill;
            projectedRevenueCard.Location = new Point(1332, 12);
            projectedRevenueCard.Margin = new Padding(12);
            projectedRevenueCard.Name = "projectedRevenueCard";
            projectedRevenueCard.Padding = new Padding(20);
            projectedRevenueCard.Size = new Size(306, 157);
            projectedRevenueCard.TabIndex = 4;
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
        
        // New Analytics Components
        private System.Windows.Forms.TableLayoutPanel kpiPanel;
        private System.Windows.Forms.Panel totalRevenueCard;
        private System.Windows.Forms.Panel avgDealValueCard;
        private System.Windows.Forms.Panel conversionRateCard;
        private System.Windows.Forms.Panel avgDaysToCloseCard;
        private System.Windows.Forms.Panel projectedRevenueCard;
        
        private System.Windows.Forms.TableLayoutPanel chartsPanel;
        private System.Windows.Forms.Panel salesChartCard;
        private System.Windows.Forms.Panel propertyStatusChartCard;
        private System.Windows.Forms.Panel leadConversionChartCard;
        private System.Windows.Forms.Panel avgSalaryChartCard;
        private System.Windows.Forms.Panel occupationPreferenceCard;
        
        private System.Windows.Forms.TableLayoutPanel totalCountsPanel;
        private System.Windows.Forms.Panel totalCountsCard;
        
        private System.Windows.Forms.TableLayoutPanel analyticsPanel;
        private System.Windows.Forms.Panel revenueAnalyticsCard;
        private System.Windows.Forms.Panel propertyAnalyticsCard;
        private System.Windows.Forms.Panel performanceAnalyticsCard;
        private System.Windows.Forms.Panel trendsAnalyticsCard;
    }
}
