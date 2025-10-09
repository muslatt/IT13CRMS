// RealEstateCRMWinForms\Views\LogsView.Designer.cs
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    partial class LogsView
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
            this.dataGridViewLogs = new System.Windows.Forms.DataGridView();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.flowHeader = new System.Windows.Forms.FlowLayoutPanel();
            this.panelSpacer = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLogs)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            //
            // dataGridViewLogs
            //
            this.dataGridViewLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewLogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLogs.Location = new System.Drawing.Point(0, 60);
            this.dataGridViewLogs.Name = "dataGridViewLogs";
            this.dataGridViewLogs.Size = new System.Drawing.Size(800, 390);
            this.dataGridViewLogs.TabIndex = 0;
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.flowHeader);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new System.Windows.Forms.Padding(10, 10, 10, 5);
            this.panelHeader.Size = new System.Drawing.Size(800, 60);
            this.panelHeader.TabIndex = 1;
            //
            // flowHeader
            //
            this.flowHeader.AutoSize = false;
            this.flowHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowHeader.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flowHeader.Location = new System.Drawing.Point(10, 10);
            this.flowHeader.Margin = new System.Windows.Forms.Padding(0);
            this.flowHeader.Name = "flowHeader";
            this.flowHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.flowHeader.Size = new System.Drawing.Size(780, 45);
            this.flowHeader.TabIndex = 0;
            this.flowHeader.WrapContents = false;
            //
            // panelSpacer
            //
            this.panelSpacer.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSpacer.Location = new System.Drawing.Point(0, 60);
            this.panelSpacer.Name = "panelSpacer";
            this.panelSpacer.Size = new System.Drawing.Size(800, 8);
            this.panelSpacer.TabIndex = 2;
            //
            // LogsView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewLogs);
            this.Controls.Add(this.panelSpacer);
            this.Controls.Add(this.panelHeader);
            this.Name = "LogsView";
            this.Size = new System.Drawing.Size(800, 450);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLogs)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewLogs;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.FlowLayoutPanel flowHeader;
        private System.Windows.Forms.Panel panelSpacer;
    }
}
