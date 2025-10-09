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
            leadConversionContentPanel = new TableLayoutPanel();
            avgSalaryChartCard = new Panel();
            avgSalaryDatePicker = new DateTimePicker();
            avgSalaryControlsPanel = new Panel();
            lblAvgSalaryStart = new Label();
            occupationPreferenceCard = new Panel();
            kpiPanel = new TableLayoutPanel();
            conversionRateCard = new Panel();
            projectedRevenueCard = new Panel();
            userRevenueCard = new Panel();
            headerPanel.SuspendLayout();
            mainPanel.SuspendLayout();
            analyticsPanel.SuspendLayout();
            totalCountsPanel.SuspendLayout();
            chartsPanel.SuspendLayout();
            leadConversionChartCard.SuspendLayout();
            avgSalaryChartCard.SuspendLayout();
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
            kpiPanel.AutoSize = true;
            kpiPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            kpiPanel.ColumnCount = 3;
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33334F));
            kpiPanel.Controls.Add(conversionRateCard, 0, 0);
            kpiPanel.Controls.Add(projectedRevenueCard, 1, 0);
            kpiPanel.Controls.Add(userRevenueCard, 2, 0);
            kpiPanel.Dock = DockStyle.Top;
            kpiPanel.Location = new Point(30, 30);
            kpiPanel.Name = "kpiPanel";
            kpiPanel.Padding = new Padding(0, 0, 0, 10);
            kpiPanel.RowCount = 1;
            kpiPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            kpiPanel.Size = new Size(1340, 180);
            kpiPanel.TabIndex = 0;
            // 
            // conversionRateCard
            // 
            conversionRateCard.BackColor = Color.White;
            conversionRateCard.BorderStyle = BorderStyle.FixedSingle;
            conversionRateCard.Dock = DockStyle.Fill;
            conversionRateCard.Location = new Point(0, 0);
            conversionRateCard.Margin = new Padding(0, 0, 10, 0);
            conversionRateCard.MinimumSize = new Size(0, 180);
            conversionRateCard.Name = "conversionRateCard";
            conversionRateCard.Padding = new Padding(20);
            conversionRateCard.Size = new Size(442, 180);
            conversionRateCard.TabIndex = 0;
            // 
            // projectedRevenueCard
            // 
            projectedRevenueCard.BackColor = Color.White;
            projectedRevenueCard.BorderStyle = BorderStyle.FixedSingle;
            projectedRevenueCard.Dock = DockStyle.Fill;
            projectedRevenueCard.Location = new Point(448, 0);
            projectedRevenueCard.Margin = new Padding(10, 0, 10, 0);
            projectedRevenueCard.MinimumSize = new Size(0, 180);
            projectedRevenueCard.Name = "projectedRevenueCard";
            projectedRevenueCard.Padding = new Padding(20);
            projectedRevenueCard.Size = new Size(442, 180);
            projectedRevenueCard.TabIndex = 1;
            // 
            // userRevenueCard
            // 
            userRevenueCard.BackColor = Color.White;
            userRevenueCard.BorderStyle = BorderStyle.FixedSingle;
            userRevenueCard.Dock = DockStyle.Fill;
            userRevenueCard.Location = new Point(896, 0);
            userRevenueCard.Margin = new Padding(10, 0, 0, 0);
            userRevenueCard.MinimumSize = new Size(0, 180);
            userRevenueCard.Name = "userRevenueCard";
            userRevenueCard.Padding = new Padding(20);
            userRevenueCard.Size = new Size(442, 180);
            userRevenueCard.TabIndex = 2;
            // 
            // chartsPanel
            // 
            chartsPanel.ColumnCount = 2;
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            chartsPanel.Controls.Add(leadConversionChartCard, 0, 0);
            chartsPanel.Controls.Add(propertyStatusChartCard, 1, 0);
            chartsPanel.Dock = DockStyle.Top;
            chartsPanel.Location = new Point(30, 210);
            chartsPanel.Name = "chartsPanel";
            chartsPanel.RowCount = 1;
            chartsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 280F));
            chartsPanel.Size = new Size(1340, 280);
            chartsPanel.TabIndex = 1;
            // 
            // leadConversionChartCard - UPDATED with better content spacing
            // 
            leadConversionChartCard.BackColor = Color.White;
            leadConversionChartCard.BorderStyle = BorderStyle.FixedSingle;
            leadConversionChartCard.Controls.Add(leadConversionContentPanel);
            leadConversionChartCard.Dock = DockStyle.Fill;
            leadConversionChartCard.Location = new Point(0, 0);
            leadConversionChartCard.Margin = new Padding(0, 0, 10, 0);
            leadConversionChartCard.Name = "leadConversionChartCard";
            leadConversionChartCard.Padding = new Padding(25, 20, 25, 20);
            leadConversionChartCard.Size = new Size(665, 280);
            leadConversionChartCard.TabIndex = 0;
            // 
            // leadConversionContentPanel - NEW: TableLayoutPanel for proper spacing
            // 
            leadConversionContentPanel.ColumnCount = 1;
            leadConversionContentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            leadConversionContentPanel.Dock = DockStyle.Fill;
            leadConversionContentPanel.Location = new Point(25, 20);
            leadConversionContentPanel.Name = "leadConversionContentPanel";
            leadConversionContentPanel.RowCount = 4;
            leadConversionContentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            leadConversionContentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            leadConversionContentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            leadConversionContentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            leadConversionContentPanel.Size = new Size(615, 240);
            leadConversionContentPanel.TabIndex = 0;
            // 
            // propertyStatusChartCard
            // 
            propertyStatusChartCard.BackColor = Color.White;
            propertyStatusChartCard.BorderStyle = BorderStyle.FixedSingle;
            propertyStatusChartCard.Dock = DockStyle.Fill;
            propertyStatusChartCard.Location = new Point(675, 0);
            propertyStatusChartCard.Margin = new Padding(10, 0, 0, 0);
            propertyStatusChartCard.Name = "propertyStatusChartCard";
            propertyStatusChartCard.Padding = new Padding(20);
            propertyStatusChartCard.Size = new Size(665, 280);
            propertyStatusChartCard.TabIndex = 1;
            // 
            // avgSalaryChartCard - UPDATED with better date picker positioning
            // 
            avgSalaryChartCard.BackColor = Color.White;
            avgSalaryChartCard.BorderStyle = BorderStyle.FixedSingle;
            avgSalaryChartCard.Controls.Add(avgSalaryControlsPanel);
            avgSalaryChartCard.Dock = DockStyle.Top;
            avgSalaryChartCard.Location = new Point(30, 490);
            avgSalaryChartCard.Name = "avgSalaryChartCard";
            avgSalaryChartCard.Padding = new Padding(20);
            avgSalaryChartCard.Size = new Size(1340, 220);
            avgSalaryChartCard.TabIndex = 2;
            // 
            // avgSalaryControlsPanel - UPDATED: Panel instead of FlowLayoutPanel for better control
            // 
            avgSalaryControlsPanel.BackColor = Color.Transparent;
            avgSalaryControlsPanel.Controls.Add(avgSalaryDatePicker);
            avgSalaryControlsPanel.Controls.Add(lblAvgSalaryStart);
            avgSalaryControlsPanel.Dock = DockStyle.Top;
            avgSalaryControlsPanel.Height = 35;
            avgSalaryControlsPanel.Location = new Point(20, 20);
            avgSalaryControlsPanel.Name = "avgSalaryControlsPanel";
            avgSalaryControlsPanel.Size = new Size(1300, 35);
            avgSalaryControlsPanel.TabIndex = 0;
            // 
            // avgSalaryDatePicker - UPDATED: Better positioning
            // 
            avgSalaryDatePicker.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            avgSalaryDatePicker.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            avgSalaryDatePicker.Format = DateTimePickerFormat.Short;
            avgSalaryDatePicker.Location = new Point(1130, 6);
            avgSalaryDatePicker.Name = "avgSalaryDatePicker";
            avgSalaryDatePicker.Size = new Size(150, 23);
            avgSalaryDatePicker.TabIndex = 1;
            // 
            // lblAvgSalaryStart - UPDATED: Better positioning
            // 
            lblAvgSalaryStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAvgSalaryStart.AutoSize = true;
            lblAvgSalaryStart.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAvgSalaryStart.ForeColor = Color.FromArgb(107, 114, 128);
            lblAvgSalaryStart.Location = new Point(1090, 9);
            lblAvgSalaryStart.Name = "lblAvgSalaryStart";
            lblAvgSalaryStart.Size = new Size(34, 15);
            lblAvgSalaryStart.TabIndex = 0;
            lblAvgSalaryStart.Text = "Start:";
            // 
            // occupationPreferenceCard
            // 
            occupationPreferenceCard.BackColor = Color.White;
            occupationPreferenceCard.BorderStyle = BorderStyle.FixedSingle;
            occupationPreferenceCard.Dock = DockStyle.Top;
            occupationPreferenceCard.Location = new Point(30, 710);
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
            totalCountsPanel.Location = new Point(30, 930);
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
            analyticsPanel.ColumnCount = 4;
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            analyticsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            analyticsPanel.Controls.Add(propertyAnalyticsCard, 0, 0);
            analyticsPanel.Controls.Add(performanceAnalyticsCard, 1, 0);
            analyticsPanel.Controls.Add(trendsAnalyticsCard, 2, 0);
            analyticsPanel.Controls.Add(revenueAnalyticsCard, 3, 0);
            analyticsPanel.Dock = DockStyle.Top;
            analyticsPanel.Location = new Point(30, 1210);
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
            leadConversionChartCard.ResumeLayout(false);
            avgSalaryChartCard.ResumeLayout(false);
            kpiPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblLastUpdated;
        private System.Windows.Forms.Panel mainPanel;

        // KPI Components
        private System.Windows.Forms.TableLayoutPanel kpiPanel;
        private System.Windows.Forms.Panel conversionRateCard;
        private System.Windows.Forms.Panel projectedRevenueCard;
        private System.Windows.Forms.Panel userRevenueCard;

        // Charts Components
        private System.Windows.Forms.TableLayoutPanel chartsPanel;
        private System.Windows.Forms.Panel salesChartCard;
        private System.Windows.Forms.Panel propertyStatusChartCard;
        private System.Windows.Forms.Panel leadConversionChartCard;
        private System.Windows.Forms.TableLayoutPanel leadConversionContentPanel; // NEW
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

        // Average Client Salary controls - UPDATED
        private System.Windows.Forms.DateTimePicker avgSalaryDatePicker;
        private System.Windows.Forms.Panel avgSalaryControlsPanel; // Changed from FlowLayoutPanel to Panel
        private System.Windows.Forms.Label lblAvgSalaryStart;
    }
}