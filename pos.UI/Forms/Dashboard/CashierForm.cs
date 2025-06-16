using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pos_system.pos.Models;
using pos_system.pos.UI.Forms.Sales;

namespace pos_system.pos.UI.Forms
{
    public partial class CashierForm : Form
    {
        public Employee _currentUser;
        private Panel _leftPanel;
        private Panel _mainPanel;
        private Button _currentButton;
        private Form _activeForm;
        private Button _dashboardButton;
        private Panel headerPanel;
        private Label lblWelcome;
        private Button btnClose;

        public CashierForm (Employee user)
        {
            InitializeComponent();
            _currentUser = user;
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new Size(1200, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Cashier Dashboard";
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            lblWelcome = new Label
            {
                Text = $"Welcome, {_currentUser.firstName} {_currentUser.lastName}",
                //Dock = DockStyle.Left,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Padding = new Padding(20, 0, 0, 0),
                Size = new Size(0, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            btnClose = new Button
            {
                Text = "✕",
                Size = new Size(35, 35),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16),
                BackColor = Color.Transparent,
                UseVisualStyleBackColor = false,
                Location = new Point(1151, 12),
            };
            btnClose.Click += (s, e) => Application.Exit();

            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(btnClose);

            // Left Panel
            _leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.WhiteSmoke
            };

            // Main Content Panel
            _mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Add controls to form
            this.Controls.Add(_mainPanel);
            this.Controls.Add(_leftPanel);
            this.Controls.Add(headerPanel);

            // Create sidebar buttons
            CreateSidebarButton("Dashboard", "🏠", 80);
            CreateSidebarButton("Billing", "💰", 140);
            CreateSidebarButton("Returns", "🔄", 200);
            CreateSidebarButton("Logout", "🔒", 500);

            OpenChildForm(new DashboardView(), _dashboardButton);
        }

        private void CreateSidebarButton(string text, string icon, int yPos)
        {
            Button btn = new Button
            {
                Text = $"{icon}  {text}",
                Tag = text,
                ForeColor = Color.FromArgb(71, 71, 71),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 11),
                Size = new Size(220, 50),
                Location = new Point(0, yPos),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };

            if (text == "Dashboard") _dashboardButton = btn;

            // Hover Effects
            btn.MouseEnter += (s, e) =>
            {
                if (btn != _currentButton) btn.BackColor = Color.FromArgb(225, 225, 225);
            };

            btn.MouseLeave += (s, e) =>
            {
                if (btn != _currentButton) btn.BackColor = Color.Transparent;
            };

            btn.Click += (s, e) =>
            {
                ActivateButton(btn);
                switch (text)
                {
                    case "Dashboard":
                        OpenChildForm(new DashboardView(), btn);
                        break;
                    case "Billing":
                        OpenChildForm(new BillingForm(_currentUser), btn);
                        break;
                    case "Returns":
                        OpenChildForm(new ReturnsForm(_currentUser), btn);
                        break;
                    case "Logout":
                        Logout();
                        break;
                }
            };

            _leftPanel.Controls.Add(btn);
        }

        private void ActivateButton(Button btn)
        {
            if (btn == null) return;
            if (_currentButton != null)
            {
                _currentButton.BackColor = Color.Transparent;
                _currentButton.ForeColor = Color.Gray;
            }
            btn.BackColor = Color.FromArgb(41, 128, 185);
            btn.ForeColor = Color.White;
            _currentButton = btn;
        }

        private void OpenChildForm(Form childForm, Button btn)
        {
            if (_activeForm != null) _activeForm.Close();
            ActivateButton(btn);
            _activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            _mainPanel.Controls.Add(childForm);
            _mainPanel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void Logout()
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        // Nested view classes
        public class DashboardView : Form { }

    }
}
