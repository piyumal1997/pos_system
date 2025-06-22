using pos_system;

namespace pos_system.pos.UI.Forms.Auth
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Panel leftPanel;
        private Panel rightPanel;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private LinkLabel linkForgotPassword;
        private Label lblWelcome;
        private Button btnClose;
        private CheckBox chkRememberMe;

        /// <summary>
        ///  Clean up any resources being used.
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
        private void InitializeComponent()
        {
            leftPanel = new Panel();
            lblWelcome = new Label();
            rightPanel = new Panel();
            btnClose = new Button();
            chkRememberMe = new CheckBox();
            linkForgotPassword = new LinkLabel();
            btnLogin = new Button();
            txtPassword = new TextBox();
            txtUsername = new TextBox();
            lblTitle = new Label();
            leftPanel.SuspendLayout();
            rightPanel.SuspendLayout();
            SuspendLayout();
            // 
            // leftPanel
            // 
            leftPanel.BackColor = Color.FromArgb(41, 128, 185);
            leftPanel.Controls.Add(lblWelcome);
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Location = new Point(0, 0);
            leftPanel.Margin = new Padding(4, 3, 4, 3);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new Size(350, 519);
            leftPanel.TabIndex = 0;
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Location = new Point(70, 233);
            lblWelcome.Margin = new Padding(4, 0, 4, 0);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(187, 64);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "Style NewAge\r\nWelcome Back!";
            lblWelcome.TextAlign = ContentAlignment.TopCenter;
            // 
            // rightPanel
            // 
            rightPanel.BackColor = Color.White;
            rightPanel.Controls.Add(btnClose);
            rightPanel.Controls.Add(chkRememberMe);
            rightPanel.Controls.Add(linkForgotPassword);
            rightPanel.Controls.Add(btnLogin);
            rightPanel.Controls.Add(txtPassword);
            rightPanel.Controls.Add(txtUsername);
            rightPanel.Controls.Add(lblTitle);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(350, 0);
            rightPanel.Margin = new Padding(4, 3, 4, 3);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new Size(583, 519);
            rightPanel.TabIndex = 1;
            // 
            // btnClose
            // 
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F);
            btnClose.ForeColor = Color.Gray;
            btnClose.Location = new Point(525, 12);
            btnClose.Margin = new Padding(4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(45, 46);
            btnClose.TabIndex = 6;
            btnClose.Text = "✕";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // chkRememberMe
            // 
            chkRememberMe.AutoSize = true;
            chkRememberMe.Font = new Font("Segoe UI", 9F);
            chkRememberMe.ForeColor = Color.Gray;
            chkRememberMe.Location = new Point(117, 312);
            chkRememberMe.Margin = new Padding(4, 3, 4, 3);
            chkRememberMe.Name = "chkRememberMe";
            chkRememberMe.Size = new Size(104, 19);
            chkRememberMe.TabIndex = 5;
            chkRememberMe.Text = "Remember me";
            chkRememberMe.UseVisualStyleBackColor = true;
            // 
            // linkForgotPassword
            // 
            linkForgotPassword.AutoSize = true;
            linkForgotPassword.Font = new Font("Segoe UI", 9F);
            linkForgotPassword.LinkColor = Color.Gray;
            linkForgotPassword.Location = new Point(219, 411);
            linkForgotPassword.Margin = new Padding(4, 0, 4, 0);
            linkForgotPassword.Name = "linkForgotPassword";
            linkForgotPassword.Size = new Size(127, 15);
            linkForgotPassword.TabIndex = 4;
            linkForgotPassword.TabStop = true;
            linkForgotPassword.Text = "Forgot your password?";
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(41, 128, 185);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 12F);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(117, 346);
            btnLogin.Margin = new Padding(4, 3, 4, 3);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(350, 46);
            btnLogin.TabIndex = 3;
            btnLogin.Text = "LOGIN";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.BorderStyle = BorderStyle.None;
            txtPassword.Font = new Font("Segoe UI", 12F);
            txtPassword.ForeColor = Color.Gray;
            txtPassword.Location = new Point(117, 254);
            txtPassword.Margin = new Padding(4, 3, 4, 3);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(350, 22);
            txtPassword.TabIndex = 2;
            txtPassword.Text = "Password";
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUsername
            // 
            txtUsername.BorderStyle = BorderStyle.None;
            txtUsername.Font = new Font("Segoe UI", 12F);
            txtUsername.ForeColor = Color.Gray;
            txtUsername.Location = new Point(117, 196);
            txtUsername.Margin = new Padding(4, 3, 4, 3);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(350, 22);
            txtUsername.TabIndex = 1;
            txtUsername.Text = "Username";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(41, 128, 185);
            lblTitle.Location = new Point(117, 115);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(290, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "POS System Login";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 519);
            Controls.Add(rightPanel);
            Controls.Add(leftPanel);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            leftPanel.ResumeLayout(false);
            leftPanel.PerformLayout();
            rightPanel.ResumeLayout(false);
            rightPanel.PerformLayout();
            ResumeLayout(false);

        }

    }
}
