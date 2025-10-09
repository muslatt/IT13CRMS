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

            // Layout metrics with balanced padding
            int leftPadding = 40;  // balanced left padding
            int rightPadding = 40; // balanced right padding
            int topPadding = 20;
            int contentWidth = this.ClientSize.Width - (leftPadding + rightPadding);

            // Title label
            lblTitle = new Label
            {
                Text = "Reject Property Request",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(leftPadding, topPadding),
                AutoSize = true,
                MaximumSize = new Size(contentWidth, 0), // allow wrapping if needed
                ForeColor = Color.FromArgb(33, 37, 41)
            };
            this.Controls.Add(lblTitle);

            // Instructions label
            lblInstructions = new Label
            {
                Text = "Please provide a reason for rejecting this property request. This reason will be visible to the client.",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(leftPadding, 60), // temporary; final position set in LayoutControls
                AutoSize = true,
                MaximumSize = new Size(contentWidth, 0),
                ForeColor = Color.FromArgb(73, 80, 87)
            };
            this.Controls.Add(lblInstructions);

            // Rejection reason text box
            txtRejectionReason = new TextBox
            {
                Location = new Point(leftPadding, 110), // final set in LayoutControls
                Size = new Size(contentWidth, 80),
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
                Size = new Size(120, 35),
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
                Size = new Size(90, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            // Set focus to text box and layout
            this.Load += (s, e) => { txtRejectionReason.Focus(); LayoutControls(); };
            this.SizeChanged += (s, e) => LayoutControls();
        }

        private void LayoutControls()
        {
            int leftPadding = 40;
            int rightPadding = 40;
            int topPadding = 20;
            int contentWidth = this.ClientSize.Width - (leftPadding + rightPadding);

            // Title – allow full height without chopping
            lblTitle.MaximumSize = new Size(contentWidth, 0);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(leftPadding, topPadding);

            // Instructions below title
            lblInstructions.MaximumSize = new Size(contentWidth, 0);
            lblInstructions.AutoSize = true;
            lblInstructions.Location = new Point(leftPadding, lblTitle.Bottom + 10);

            // Multiline textbox below instructions
            txtRejectionReason.Location = new Point(leftPadding, lblInstructions.Bottom + 10);
            txtRejectionReason.Size = new Size(contentWidth, 100);

            // Buttons aligned to right with padding
            int buttonsY = txtRejectionReason.Bottom + 15;
            int spacing = 10;
            btnCancel.Location = new Point(this.ClientSize.Width - rightPadding - btnCancel.Width, buttonsY);
            btnReject.Location = new Point(btnCancel.Left - spacing - btnReject.Width, buttonsY);

            // Ensure the form is tall enough to show buttons
            int bottomPadding = 20;
            int desiredClientHeight = buttonsY + btnCancel.Height + bottomPadding;
            int minClientHeight = 300; // original intended height
            int newHeight = Math.Max(minClientHeight, desiredClientHeight);
            if (this.ClientSize.Height < newHeight)
            {
                this.ClientSize = new Size(this.ClientSize.Width, newHeight);
            }
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
