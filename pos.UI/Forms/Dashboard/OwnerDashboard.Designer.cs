using pos_system.pos.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using pos_system.pos.UI.Forms;
using pos_system;
using pos_system.pos;
using pos_system.pos.UI;

namespace pos_system.pos.UI.Forms.Dashboard
{
    public partial class OwnerDashboard : Form
    {
        // Control Declarations
        private Panel _leftPanel;
        private Panel _mainPanel;
        private Button _currentButton;
        private Form _activeForm;
        private Button _dashboardButton;
        private Panel headerPanel;
        private Label lblWelcome;
        private Button btnClose;

        // Colors
        private Color primaryColor;
        private Color secondaryColor;
        private Color activeButtonColor;

        private void InitializeComponent()
        {
            _leftPanel = new Panel();
            _mainPanel = new Panel();
            headerPanel = new Panel();
            lblWelcome = new Label();
            btnClose = new Button();
            headerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _leftPanel
            // 
            _leftPanel.BackColor = Color.WhiteSmoke;
            _leftPanel.Dock = DockStyle.Left;
            _leftPanel.Location = new Point(0, 60);
            _leftPanel.Name = "_leftPanel";
            _leftPanel.Size = new Size(220, 656);
            _leftPanel.TabIndex = 0;
            // 
            // _mainPanel
            // 
            _mainPanel.BackColor = Color.White;
            _mainPanel.Dock = DockStyle.Right;
            _mainPanel.Location = new Point(218, 60);
            _mainPanel.Name = "_mainPanel";
            _mainPanel.Size = new Size(980, 656);
            _mainPanel.TabIndex = 1;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(41, 128, 185);
            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(btnClose);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(1198, 60);
            headerPanel.TabIndex = 2;
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.Location = new Point(20, 15);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(0, 25);
            lblWelcome.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.Transparent;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(1151, 12);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(35, 35);
            btnClose.TabIndex = 1;
            btnClose.Text = "✕";
            btnClose.UseVisualStyleBackColor = false;
            // 
            // OwnerDashboard
            // 
            ClientSize = new Size(1198, 716);
            ControlBox = false;
            Controls.Add(_leftPanel);
            Controls.Add(_mainPanel);
            Controls.Add(headerPanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximumSize = new Size(1200, 800);
            MinimumSize = new Size(1200, 718);
            Name = "OwnerDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            ResumeLayout(false);
        }
    }
}