using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    public partial class PropertyRejectionDialog : Form
    {
        public string RejectionReason { get; private set; } = string.Empty;

        private TextBox txtRejectionReason;
        private Button btnReject;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblInstructions;

        public PropertyRejectionDialog()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 300);
            this.Text = "Reject Property Request";
            this.BackColor = Color.White;

            // Title label
            lblTitle = new Label
            {
                Text = "Reject Property Request",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(460, 30),
                ForeColor = Color.FromArgb(33, 37, 41)
            };
            this.Controls.Add(lblTitle);

            // Instructions label
            lblInstructions = new Label
            {
                Text = "Please provide a reason for rejecting this property request. This reason will be visible to the client.",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 60),
                Size = new Size(460, 40),
                ForeColor = Color.FromArgb(73, 80, 87)
            };
            this.Controls.Add(lblInstructions);

            // Rejection reason text box
            txtRejectionReason = new TextBox
            {
                Location = new Point(20, 110),
                Size = new Size(460, 80),
                Multiline = true,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41)
            };
            this.Controls.Add(txtRejectionReason);

            // Reject button
            btnReject = new Button
            {
                Text = "Reject Property",
                Location = new Point(280, 210),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnReject.Click += BtnReject_Click;
            this.Controls.Add(btnReject);

            // Cancel button
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(390, 210),
                Size = new Size(80, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            // Set focus to text box
            this.Load += (s, e) => txtRejectionReason.Focus();
        }

        private void BtnReject_Click(object? sender, EventArgs e)
        {
            string reason = txtRejectionReason.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(reason))
            {
                MessageBox.Show("Please provide a reason for rejecting this property request.",
                    "Rejection Reason Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (reason.Length < 10)
            {
                MessageBox.Show("Please provide a more detailed reason for rejection (at least 10 characters).",
                    "Rejection Reason Too Short", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RejectionReason = reason;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}