namespace RealEstateCRMWinForms.Views
{
    partial class EmailVerificationView
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblEmailAddress;
        private Label lblInstructions;
        private TextBox txtVerificationCode;
        private Button btnVerify;
        private Button btnResendCode;
        private Label lblBackToLogin;

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
            this.lblTitle = new Label();
            this.lblEmailAddress = new Label();
            this.lblInstructions = new Label();
            this.txtVerificationCode = new TextBox();
            this.btnVerify = new Button();
            this.btnResendCode = new Button();
            this.lblBackToLogin = new Label();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(41, 128, 185);
            this.lblTitle.Location = new Point(50, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(200, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Verify Your Email";

            // lblEmailAddress
            this.lblEmailAddress.AutoSize = true;
            this.lblEmailAddress.Font = new Font("Segoe UI", 10F);
            this.lblEmailAddress.Location = new Point(50, 80);
            this.lblEmailAddress.Name = "lblEmailAddress";
            this.lblEmailAddress.Size = new Size(300, 19);
            this.lblEmailAddress.TabIndex = 1;
            this.lblEmailAddress.Text = "We've sent a verification code to: [email]";

            // lblInstructions
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Font = new Font("Segoe UI", 9F);
            this.lblInstructions.ForeColor = Color.Gray;
            this.lblInstructions.Location = new Point(50, 110);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new Size(350, 15);
            this.lblInstructions.TabIndex = 2;
            this.lblInstructions.Text = "Please enter the 6-digit verification code to activate your account.";

            // txtVerificationCode
            this.txtVerificationCode.Font = new Font("Segoe UI", 12F);
            this.txtVerificationCode.Location = new Point(50, 150);
            this.txtVerificationCode.MaxLength = 6;
            this.txtVerificationCode.Name = "txtVerificationCode";
            this.txtVerificationCode.Size = new Size(150, 29);
            this.txtVerificationCode.TabIndex = 3;
            this.txtVerificationCode.TextAlign = HorizontalAlignment.Center;
            this.txtVerificationCode.CharacterCasing = CharacterCasing.Upper;

            // btnVerify
            this.btnVerify.BackColor = Color.FromArgb(41, 128, 185);
            this.btnVerify.FlatStyle = FlatStyle.Flat;
            this.btnVerify.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnVerify.ForeColor = Color.White;
            this.btnVerify.Location = new Point(50, 200);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new Size(120, 35);
            this.btnVerify.TabIndex = 4;
            this.btnVerify.Text = "Verify Email";
            this.btnVerify.UseVisualStyleBackColor = false;
            this.btnVerify.Click += new EventHandler(this.btnVerify_Click);

            // btnResendCode
            this.btnResendCode.BackColor = Color.FromArgb(149, 165, 166);
            this.btnResendCode.FlatStyle = FlatStyle.Flat;
            this.btnResendCode.Font = new Font("Segoe UI", 9F);
            this.btnResendCode.ForeColor = Color.White;
            this.btnResendCode.Location = new Point(180, 200);
            this.btnResendCode.Name = "btnResendCode";
            this.btnResendCode.Size = new Size(100, 35);
            this.btnResendCode.TabIndex = 5;
            this.btnResendCode.Text = "Resend Code";
            this.btnResendCode.UseVisualStyleBackColor = false;
            this.btnResendCode.Click += new EventHandler(this.btnResendCode_Click);

            // lblBackToLogin
            this.lblBackToLogin.AutoSize = true;
            this.lblBackToLogin.Cursor = Cursors.Hand;
            this.lblBackToLogin.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            this.lblBackToLogin.ForeColor = Color.FromArgb(41, 128, 185);
            this.lblBackToLogin.Location = new Point(50, 260);
            this.lblBackToLogin.Name = "lblBackToLogin";
            this.lblBackToLogin.Size = new Size(88, 15);
            this.lblBackToLogin.TabIndex = 6;
            this.lblBackToLogin.Text = "Back to Sign In";
            this.lblBackToLogin.Click += new EventHandler(this.lblBackToLogin_Click);

            // EmailVerificationView
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblEmailAddress);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.txtVerificationCode);
            this.Controls.Add(this.btnVerify);
            this.Controls.Add(this.btnResendCode);
            this.Controls.Add(this.lblBackToLogin);
            this.Name = "EmailVerificationView";
            this.Size = new Size(450, 320);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}