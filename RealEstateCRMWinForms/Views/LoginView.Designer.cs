using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Views
{
    partial class LoginView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginView));
            pnlMain = new Panel();
            pnlCenter = new Panel();
            pnlLogin = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnLogin = new Button();
            lblRegisterQuestion = new Label();
            linkRegister = new LinkLabel();
            pnlMain.SuspendLayout();
            pnlCenter.SuspendLayout();
            pnlLogin.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.FromArgb(248, 249, 250);
            pnlMain.BackgroundImage = (Image)resources.GetObject("pnlMain.BackgroundImage");
            pnlMain.BackgroundImageLayout = ImageLayout.Stretch;
            pnlMain.Controls.Add(pnlCenter);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(1200, 800);
            pnlMain.TabIndex = 0;
            pnlMain.Paint += pnlMain_Paint;
            // 
            // pnlCenter
            // 
            pnlCenter.Anchor = AnchorStyles.None;
            pnlCenter.BackColor = Color.White;
            pnlCenter.Controls.Add(pnlLogin);
            pnlCenter.Location = new Point(350, 150);
            pnlCenter.Name = "pnlCenter";
            pnlCenter.Size = new Size(500, 500);
            pnlCenter.TabIndex = 0;
            // 
            // pnlLogin
            // 
            pnlLogin.Controls.Add(lblTitle);
            pnlLogin.Controls.Add(lblSubtitle);
            pnlLogin.Controls.Add(lblEmail);
            pnlLogin.Controls.Add(txtEmail);
            pnlLogin.Controls.Add(lblPassword);
            pnlLogin.Controls.Add(txtPassword);
            pnlLogin.Controls.Add(btnLogin);
            pnlLogin.Controls.Add(lblRegisterQuestion);
            pnlLogin.Controls.Add(linkRegister);
            pnlLogin.Dock = DockStyle.Fill;
            pnlLogin.Location = new Point(0, 0);
            pnlLogin.Name = "pnlLogin";
            pnlLogin.Size = new Size(500, 500);
            pnlLogin.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(33, 37, 41);
            lblTitle.Location = new Point(50, 60);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(124, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Sign In";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 12F);
            lblSubtitle.ForeColor = Color.FromArgb(108, 117, 125);
            lblSubtitle.Location = new Point(50, 110);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(245, 21);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Welcome back to Real Estate CRM";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblEmail.ForeColor = Color.FromArgb(33, 37, 41);
            lblEmail.Location = new Point(50, 156);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(48, 21);
            lblEmail.TabIndex = 2;
            lblEmail.Text = "Email";
            // 
            // txtEmail
            // 
            txtEmail.Font = new Font("Segoe UI", 12F);
            txtEmail.Location = new Point(50, 181);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "example@email.com";
            txtEmail.Size = new Size(400, 29);
            txtEmail.TabIndex = 3;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(33, 37, 41);
            lblPassword.Location = new Point(50, 229);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(79, 21);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Password";
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 12F);
            txtPassword.Location = new Point(50, 254);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Enter your password";
            txtPassword.Size = new Size(400, 29);
            txtPassword.TabIndex = 5;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(0, 123, 255);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(50, 313);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(400, 45);
            btnLogin.TabIndex = 6;
            btnLogin.Text = "Sign In";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // lblRegisterQuestion
            // 
            lblRegisterQuestion.AutoSize = true;
            lblRegisterQuestion.Font = new Font("Segoe UI", 10F);
            lblRegisterQuestion.ForeColor = Color.FromArgb(108, 117, 125);
            lblRegisterQuestion.Location = new Point(142, 380);
            lblRegisterQuestion.Name = "lblRegisterQuestion";
            lblRegisterQuestion.Size = new Size(153, 19);
            lblRegisterQuestion.TabIndex = 7;
            lblRegisterQuestion.Text = "Don't have an account?";
            // 
            // linkRegister
            // 
            linkRegister.AutoSize = true;
            linkRegister.Font = new Font("Segoe UI", 10F);
            linkRegister.LinkColor = Color.FromArgb(0, 123, 255);
            linkRegister.Location = new Point(295, 380);
            linkRegister.Name = "linkRegister";
            linkRegister.Size = new Size(57, 19);
            linkRegister.TabIndex = 8;
            linkRegister.TabStop = true;
            linkRegister.Text = "Sign Up";
            linkRegister.LinkClicked += linkRegister_LinkClicked;
            // 
            // LoginView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlMain);
            Name = "LoginView";
            Size = new Size(1200, 800);
            pnlMain.ResumeLayout(false);
            pnlCenter.ResumeLayout(false);
            pnlLogin.ResumeLayout(false);
            pnlLogin.PerformLayout();
            ResumeLayout(false);
        }

        private Panel pnlMain;
        private Panel pnlCenter;
        private Panel pnlLogin;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblRegisterQuestion;
        private LinkLabel linkRegister;
    }
}