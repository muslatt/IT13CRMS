using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class LogsView : UserControl
    {
        private readonly LogViewModel _viewModel;
        private BindingSource _bindingSource;
        private Button btnDownloadCsv;

        public LogsView()
        {
            _viewModel = new LogViewModel();
            _bindingSource = new BindingSource();

            InitializeComponent();

            ConfigureGridAppearance();
            CreateDownloadButton();
            InitializeData();
        }

        private void InitializeData()
        {
            try
            {
                var logs = _viewModel.GetLogs();
                _bindingSource.DataSource = logs;
                dataGridViewLogs.DataSource = _bindingSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridAppearance()
        {
            dataGridViewLogs.AutoGenerateColumns = false;
            dataGridViewLogs.Columns.Clear();

            // Timestamp column
            var timestampCol = new DataGridViewTextBoxColumn
            {
                Name = "Timestamp",
                HeaderText = "Timestamp",
                DataPropertyName = "Timestamp",
                Width = 150
            };
            timestampCol.DefaultCellStyle.Format = "g"; // General date/time format
            dataGridViewLogs.Columns.Add(timestampCol);

            // User column
            var userCol = new DataGridViewTextBoxColumn
            {
                Name = "User",
                HeaderText = "User",
                DataPropertyName = "UserFullName",
                Width = 150
            };
            dataGridViewLogs.Columns.Add(userCol);

            // Action column
            var actionCol = new DataGridViewTextBoxColumn
            {
                Name = "Action",
                HeaderText = "Action",
                DataPropertyName = "Action",
                Width = 200
            };
            dataGridViewLogs.Columns.Add(actionCol);

            // Details column
            var detailsCol = new DataGridViewTextBoxColumn
            {
                Name = "Details",
                HeaderText = "Details",
                DataPropertyName = "Details",
                Width = 300
            };
            dataGridViewLogs.Columns.Add(detailsCol);

            // Grid settings
            dataGridViewLogs.AllowUserToAddRows = false;
            dataGridViewLogs.AllowUserToDeleteRows = false;
            dataGridViewLogs.ReadOnly = true;
            dataGridViewLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewLogs.MultiSelect = false;
            dataGridViewLogs.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewLogs.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void CreateDownloadButton()
        {
            btnDownloadCsv = new Button
            {
                Text = "📥 Download Logs (CSV)",
                Size = new Size(180, 35),
                Location = new Point(10, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnDownloadCsv.FlatAppearance.BorderSize = 0;
            btnDownloadCsv.Click += BtnDownloadCsv_Click;

            // Add button to the form
            this.Controls.Add(btnDownloadCsv);
            btnDownloadCsv.BringToFront();
        }

        private void BtnDownloadCsv_Click(object? sender, EventArgs e)
        {
            try
            {
                var logs = _viewModel.GetLogs();
                if (logs == null || logs.Count == 0)
                {
                    MessageBox.Show("No logs available to export.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    DefaultExt = "csv",
                    FileName = $"Logs_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    Title = "Save Logs as CSV"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCsv(logs, saveFileDialog.FileName);
                    MessageBox.Show($"Logs successfully exported to:\n{saveFileDialog.FileName}",
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Ask if user wants to open the file
                    var result = MessageBox.Show("Do you want to open the exported file?",
                        "Open File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveFileDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting logs: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCsv(List<LogEntry> logs, string filePath)
        {
            var csv = new StringBuilder();

            // Add header
            csv.AppendLine("Timestamp,User,Action,Details");

            // Add data rows
            foreach (var log in logs)
            {
                var timestamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                var user = EscapeCsvField(log.UserFullName);
                var action = EscapeCsvField(log.Action);
                var details = EscapeCsvField(log.Details ?? string.Empty);

                csv.AppendLine($"{timestamp},{user},{action},{details}");
            }

            File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            // If field contains comma, newline, or quote, wrap it in quotes and escape existing quotes
            if (field.Contains(',') || field.Contains('\n') || field.Contains('\r') || field.Contains('"'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}