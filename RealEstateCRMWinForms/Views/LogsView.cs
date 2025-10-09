using RealEstateCRMWinForms.Models;
using RealEstateCRMWinForms.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class LogsView : UserControl
    {
        private static readonly Color RejectedColor = Color.FromArgb(220, 53, 69);

        private readonly LogViewModel _viewModel;
        private readonly BindingSource _bindingSource;
        private readonly List<LogEntry> _allLogs = new();
        private readonly List<LogEntry> _filteredLogs = new();

        private Button btnDownloadCsv = null!;

        public LogsView()
        {
            _viewModel = new LogViewModel();
            _bindingSource = new BindingSource();

            InitializeComponent();

            dataGridViewLogs.DataSource = _bindingSource;

            ConfigureGridAppearance();
            BuildHeaderControls();
            InitializeData();
        }

        private void InitializeData()
        {
            try
            {
                LoadLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLogs()
        {
            var logs = _viewModel.GetLogs();

            _allLogs.Clear();
            if (logs != null)
            {
                _allLogs.AddRange(logs);
            }

            ApplyLogFilter();
        }

        private void ApplyLogFilter()
        {
            IEnumerable<LogEntry> source = _allLogs;

            _filteredLogs.Clear();
            _filteredLogs.AddRange(source);

            _bindingSource.DataSource = null;
            _bindingSource.DataSource = _filteredLogs;
            _bindingSource.ResetBindings(false);
        }

        private void ConfigureGridAppearance()
        {
            dataGridViewLogs.AutoGenerateColumns = false;
            dataGridViewLogs.Columns.Clear();
            dataGridViewLogs.ReadOnly = true;
            dataGridViewLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewLogs.MultiSelect = false;
            dataGridViewLogs.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewLogs.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridViewLogs.RowHeadersVisible = false;
            dataGridViewLogs.BackgroundColor = Color.White;
            dataGridViewLogs.BorderStyle = BorderStyle.None;
            dataGridViewLogs.CellDoubleClick += DataGridViewLogs_CellDoubleClick;
            dataGridViewLogs.KeyDown += DataGridViewLogs_KeyDown;

            var timestampCol = new DataGridViewTextBoxColumn
            {
                Name = "Timestamp",
                HeaderText = "Timestamp",
                DataPropertyName = "Timestamp",
                Width = 150
            };
            timestampCol.DefaultCellStyle.Format = "g";
            dataGridViewLogs.Columns.Add(timestampCol);

            var userCol = new DataGridViewTextBoxColumn
            {
                Name = "User",
                HeaderText = "User",
                DataPropertyName = "UserFullName",
                Width = 150
            };
            dataGridViewLogs.Columns.Add(userCol);

            var actionCol = new DataGridViewTextBoxColumn
            {
                Name = "Action",
                HeaderText = "Action",
                DataPropertyName = "Action",
                Width = 200
            };
            dataGridViewLogs.Columns.Add(actionCol);

            var detailsCol = new DataGridViewTextBoxColumn
            {
                Name = "Details",
                HeaderText = "Details",
                DataPropertyName = "Details",
                Width = 320
            };
            dataGridViewLogs.Columns.Add(detailsCol);

            // Do not color rows differently; keep neutral styling in broker logs

            // Column sizing for readability
            dataGridViewLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            timestampCol.FillWeight = 18;
            userCol.FillWeight = 18;
            actionCol.FillWeight = 24;
            detailsCol.FillWeight = 40;
        }

        private void DataGridViewLogs_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ShowSelectedLogDetails();
            }
        }

        private void DataGridViewLogs_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            ShowSelectedLogDetails();
        }

        private void ShowSelectedLogDetails()
        {
            if (_bindingSource?.Current is not LogEntry log) return;

            using var dlg = new Form
            {
                Text = "Log Details",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(560, 360),
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                BackColor = Color.White
            };

            var lblTs = new Label { Text = "Timestamp:", Location = new Point(16, 16), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var valTs = new TextBox { ReadOnly = true, BorderStyle = BorderStyle.None, Location = new Point(110, 16), Width = 420, Text = log.Timestamp.ToString("F") };

            var lblUser = new Label { Text = "User:", Location = new Point(16, 44), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var valUser = new TextBox { ReadOnly = true, BorderStyle = BorderStyle.None, Location = new Point(110, 44), Width = 420, Text = log.UserFullName };

            var lblAction = new Label { Text = "Action:", Location = new Point(16, 72), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var valAction = new TextBox { ReadOnly = true, BorderStyle = BorderStyle.None, Location = new Point(110, 72), Width = 420, Text = log.Action };

            var lblDetails = new Label { Text = "Details:", Location = new Point(16, 100), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var valDetails = new TextBox { ReadOnly = true, Multiline = true, ScrollBars = ScrollBars.Vertical, Location = new Point(110, 100), Size = new Size(420, 160), Text = log.Details ?? string.Empty };

            var btnClose = new Button { Text = "Close", Size = new Size(90, 30), Location = new Point(440, 280), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => dlg.Close();

            dlg.Controls.AddRange(new Control[] { lblTs, valTs, lblUser, valUser, lblAction, valAction, lblDetails, valDetails, btnClose });
            dlg.AcceptButton = btnClose;
            dlg.CancelButton = btnClose;
            dlg.ShowDialog(this);
        }

        private void BuildHeaderControls()
        {
            flowHeader.SuspendLayout();
            flowHeader.Controls.Clear();

            btnDownloadCsv = new Button
            {
                Text = "Download Logs (CSV)",
                AutoSize = true,
                Height = 32,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                Margin = new Padding(0, 0, 12, 0)
            };
            btnDownloadCsv.FlatAppearance.BorderSize = 0;
            btnDownloadCsv.Click += BtnDownloadCsv_Click;

            flowHeader.Controls.Add(btnDownloadCsv);
            flowHeader.ResumeLayout();
        }

        private void BtnDownloadCsv_Click(object? sender, EventArgs e)
        {
            try
            {
                LoadLogs();
                var logsToExport = _allLogs;

                if (logsToExport == null || logsToExport.Count == 0)
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
                    ExportToCsv(logsToExport, saveFileDialog.FileName);
                    MessageBox.Show($"Logs successfully exported to:\n{saveFileDialog.FileName}",
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void ExportToCsv(IReadOnlyCollection<LogEntry> logs, string filePath)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Timestamp,User,Action,Details");

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

        // Intentionally keeping broker logs neutral; no row colorization for rejected entries.

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return string.Empty;
            }

            if (field.Contains(',') || field.Contains('\n') || field.Contains('\r') || field.Contains('"'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}
