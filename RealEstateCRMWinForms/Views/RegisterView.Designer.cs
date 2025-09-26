using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    partial class RegisterView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlMain = new Panel();
            pnlCenter = new Panel();
            pnlRegister = new Panel();
            lblRegTitle = new Label();
            txtRegFirstName = new TextBox();
            txtRegLastName = new TextBox();
            txtRegEmail = new TextBox();
            txtRegPassword = new TextBox();
            btnRegister = new Button();
            lblBackToLogin = new Label();
            pnlMain.SuspendLayout();
            pnlCenter.SuspendLayout();
            pnlRegister.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.FromArgb(248, 249, 250);
            pnlMain.Controls.Add(pnlCenter);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(1200, 800);
            pnlMain.TabIndex = 0;
            // 
            // pnlCenter
            // 
            pnlCenter.Anchor = AnchorStyles.None;
            pnlCenter.BackColor = Color.White;
            pnlCenter.Controls.Add(pnlRegister);
            pnlCenter.Location = new Point(350, 150);
            pnlCenter.Name = "pnlCenter";
            pnlCenter.Size = new Size(500, 500);
            pnlCenter.TabIndex = 0;
            // 
            // pnlRegister
            // 
            pnlRegister.Controls.Add(lblRegTitle);
            pnlRegister.Controls.Add(txtRegFirstName);
            pnlRegister.Controls.Add(txtRegLastName);
            pnlRegister.Controls.Add(txtRegEmail);
            pnlRegister.Controls.Add(txtRegPassword);
            pnlRegister.Controls.Add(btnRegister);
            pnlRegister.Controls.Add(lblBackToLogin);
            pnlRegister.Dock = DockStyle.Fill;
            pnlRegister.Location = new Point(0, 0);
            pnlRegister.Name = "pnlRegister";
            pnlRegister.Size = new Size(500, 500);
            pnlRegister.TabIndex = 1;
            // 
            // lblRegTitle
            // 
            lblRegTitle.AutoSize = true;
            lblRegTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblRegTitle.ForeColor = Color.FromArgb(33, 37, 41);
            lblRegTitle.Location = new Point(50, 40);
            lblRegTitle.Name = "lblRegTitle";
            lblRegTitle.Size = new Size(248, 45);
            lblRegTitle.TabIndex = 0;
            lblRegTitle.Text = "Create Account";
            // 
            // txtRegFirstName
            // 
            txtRegFirstName.Font = new Font("Segoe UI", 12F);
            txtRegFirstName.Location = new Point(50, 120);
            txtRegFirstName.Name = "txtRegFirstName";
            txtRegFirstName.PlaceholderText = "First Name";
            txtRegFirstName.Size = new Size(190, 29);
            txtRegFirstName.TabIndex = 1;
            // 
            // txtRegLastName
            // 
            txtRegLastName.Font = new Font("Segoe UI", 12F);
            txtRegLastName.Location = new Point(260, 120);
            txtRegLastName.Name = "txtRegLastName";
            txtRegLastName.PlaceholderText = "Last Name";
            txtRegLastName.Size = new Size(190, 29);
            txtRegLastName.TabIndex = 2;
            // 
            // txtRegEmail
            // 
            txtRegEmail.Font = new Font("Segoe UI", 12F);
            txtRegEmail.Location = new Point(50, 170);
            txtRegEmail.Name = "txtRegEmail";
            txtRegEmail.PlaceholderText = "Email address";
            txtRegEmail.Size = new Size(400, 29);
            txtRegEmail.TabIndex = 3;
            // 
            // txtRegPassword
            // 
            txtRegPassword.Font = new Font("Segoe UI", 12F);
            txtRegPassword.Location = new Point(50, 220);
            txtRegPassword.Name = "txtRegPassword";
            txtRegPassword.PlaceholderText = "Password (min. 6 characters)";
            txtRegPassword.Size = new Size(400, 29);
            txtRegPassword.TabIndex = 4;
            txtRegPassword.UseSystemPasswordChar = true;
            // 
            // btnRegister
            // 
            btnRegister.BackColor = Color.FromArgb(40, 167, 69);
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(50, 280);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(400, 45);
            btnRegister.TabIndex = 5;
            btnRegister.Text = "Create Account";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += btnRegister_Click;
            // 
            // lblBackToLogin
            // 
            lblBackToLogin.AutoSize = true;
            lblBackToLogin.Cursor = Cursors.Hand;
            lblBackToLogin.Font = new Font("Segoe UI", 10F);
            lblBackToLogin.ForeColor = Color.FromArgb(0, 123, 255);
            lblBackToLogin.Location = new Point(50, 360);
            lblBackToLogin.Name = "lblBackToLogin";
            lblBackToLogin.Size = new Size(102, 19);
            lblBackToLogin.TabIndex = 6;
            lblBackToLogin.Text = "Back to Sign In";
            lblBackToLogin.Click += lblBackToLogin_Click;
            // 
            // RegisterView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(pnlMain);
            Name = "RegisterView";
            Size = new Size(1200, 800);
            pnlMain.ResumeLayout(false);
            pnlCenter.ResumeLayout(false);
            pnlRegister.ResumeLayout(false);
            pnlRegister.PerformLayout();
            ResumeLayout(false);
        }

        private Panel pnlMain;
        private Panel pnlCenter;
        private Panel pnlRegister;
        private Label lblRegTitle;
        private TextBox txtRegFirstName;
        private TextBox txtRegLastName;
        private TextBox txtRegEmail;
        private TextBox txtRegPassword;
        private Button btnRegister;
        private Label lblBackToLogin;
    }
}
