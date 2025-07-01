using pos_system.pos.Models;
using pos_system.pos.UI.Forms.Sales;
using pos_system.pos.UI.Forms.Common;
using System;
using System.Linq;
using pos_system.pos.UI.Forms;
using pos_system;
using pos_system.pos;
using pos_system.pos.UI;
using pos_system.pos.UI.Forms.Inventory;
using FontAwesome.Sharp;
using pos_system.pos.BLL.Services;
using System.Drawing.Drawing2D;

namespace pos_system.pos.UI.Forms.Dashboard
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
        private Button btnMinimize;

        // Variables for form dragging
        private bool _dragging = false;
        private Point _dragStartPosition;

        public CashierForm(Employee user)
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
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Padding = new Padding(20, 0, 0, 0),
                Size = new Size(0, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            // Minimize Button
            btnMinimize = new Button
            {
                Text = "─",
                Size = new Size(35, 35),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 14),
                Location = new Point(1151 - 45, 12) // Position left of close button
            };
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            // Close Button
            btnClose = new Button
            {
                Text = "✕",
                Size = new Size(35, 35),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                UseVisualStyleBackColor = false,
                Location = new Point(1151, 12)
            };
            btnClose.Click += (s, e) => Application.Exit();

            // Add controls to header
            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(btnMinimize);
            headerPanel.Controls.Add(btnClose);

            // Enable dragging on header panel
            headerPanel.MouseDown += HeaderPanel_MouseDown;
            headerPanel.MouseMove += HeaderPanel_MouseMove;
            headerPanel.MouseUp += HeaderPanel_MouseUp;

            // Enable dragging on welcome label too
            lblWelcome.MouseDown += HeaderPanel_MouseDown;
            lblWelcome.MouseMove += HeaderPanel_MouseMove;
            lblWelcome.MouseUp += HeaderPanel_MouseUp;

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
            CreateSidebarButton("Search Items", "🔍", 260);
            CreateSidebarButton("Bills", "📄", 320);
            CreateSidebarButton("Logout", "🔒", 540);

            OpenChildForm(new DashboardView(_currentUser), _dashboardButton);
        }

        // Form dragging implementation
        private void HeaderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _dragStartPosition = new Point(e.X, e.Y);
        }

        private void HeaderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point newPoint = PointToScreen(new Point(e.X, e.Y));
                Location = new Point(newPoint.X - _dragStartPosition.X, newPoint.Y - _dragStartPosition.Y);
            }
        }

        private void HeaderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
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
                        OpenChildForm(new DashboardView(_currentUser), btn);
                        break;
                    case "Billing":
                        OpenChildForm(new BillingForm(_currentUser), btn);
                        break;
                    case "Returns":
                        OpenChildForm(new ReturnsForm(_currentUser), btn);
                        break;
                    case "Search Items":
                        OpenChildForm(new ItemSearchCashier(_currentUser), btn);
                        break;
                    case "Bills":
                        OpenChildForm(new BillSearchCashier(_currentUser), btn);
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
            Auth.LoginForm login = new Auth.LoginForm();
            login.Show();
        }

        // Nested DashboardView class for completeness
        public class DashboardView : Form
        {
            private readonly DashboardService _dashboardService;
            private DashboardMetrics _metrics;

            // Card Labels
            private Label lblDailySales;
            private Label lblDailyCashIncome;
            private Label lblDailyQuantitySold;
            private Label lblDailyItemsSold;
            private Label lblDailyBankPayments;
            private Label lblDailyCashPayments;
            private Label lblDailyCardPayments;
            private Label lblDailyReturnAmount;
            private Label lblDailyReturnQuantity;

            private FlowLayoutPanel panelCards;
            private Button btnRefresh;
            private Employee _currentUser;
            private Panel titlePanel;
            private TableLayoutPanel mainLayout;

            // Theme colors
            private readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private readonly Color SuccessColor = Color.FromArgb(46, 204, 113);
            private readonly Color WarningColor = Color.FromArgb(241, 196, 15);
            private readonly Color DeleteColor = Color.FromArgb(231, 76, 60);

            public DashboardView(Employee currentUser)
            {
                _currentUser = currentUser;
                _dashboardService = new DashboardService();
                InitializeComponent();
                LoadDashboardData();

                // Handle initial layout
                this.Shown += (s, e) => {
                    PositionRefreshButton();
                    AdjustCardSizes();
                };
            }

            private void InitializeComponent()
            {
                this.SuspendLayout();
                this.BackColor = Color.White;
                this.Text = "Cashier Dashboard";
                this.Padding = new Padding(0);
                this.AutoScroll = true;

                // Main table layout
                mainLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2,
                    BackColor = Color.White,
                    Padding = new Padding(10),
                    Margin = new Padding(0),
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.None
                };
                mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                this.Controls.Add(mainLayout);

                // Title Panel
                titlePanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = PrimaryColor,
                    Padding = new Padding(10, 5, 20, 0),
                    Height = 70
                };
                mainLayout.Controls.Add(titlePanel, 0, 0);

                // Title label
                var lblTitle = new Label
                {
                    Text = "CASHIER DASHBOARD",
                    Dock = DockStyle.Left,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true
                };
                titlePanel.Controls.Add(lblTitle);

                // Refresh Button
                btnRefresh = new Button
                {
                    Text = "REFRESH",
                    Size = new Size(120, 40),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = {
                BorderSize = 0,
                MouseOverBackColor = ControlPaint.Light(PrimaryColor, 0.2f)
            },
                    BackColor = PrimaryColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Anchor = AnchorStyles.Right
                };
                btnRefresh.Click += (s, e) => LoadDashboardData();
                titlePanel.Controls.Add(btnRefresh);

                // Card container - updated to fill available space
                panelCards = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = true,
                    BackColor = Color.White,
                    Padding = new Padding(20)
                };
                mainLayout.Controls.Add(panelCards, 0, 1);

                // Add resize handlers
                panelCards.Resize += (s, e) => AdjustCardSizes();
                mainLayout.Resize += (s, e) => AdjustCardSizes();
                this.Resize += (s, e) => AdjustCardSizes();

                // Create cards
                CreateModernCard("Daily Sales", "0.00", IconChar.DollarSign, SuccessColor, out lblDailySales);
                CreateModernCard("Cash Income", "0.00", IconChar.MoneyBillWave, SuccessColor, out lblDailyCashIncome);
                CreateModernCard("Quantity Sold", "0", IconChar.ShoppingCart, PrimaryColor, out lblDailyQuantitySold);
                CreateModernCard("Items Sold", "0", IconChar.Box, PrimaryColor, out lblDailyItemsSold);
                CreateModernCard("Bank Payments", "0.00", IconChar.University, PrimaryColor, out lblDailyBankPayments);
                CreateModernCard("Cash Payments", "0.00", IconChar.MoneyBill, WarningColor, out lblDailyCashPayments);
                CreateModernCard("Card Payments", "0.00", IconChar.CreditCard, WarningColor, out lblDailyCardPayments);
                CreateModernCard("Return Amount", "0.00", IconChar.ExchangeAlt, DeleteColor, out lblDailyReturnAmount);
                CreateModernCard("Return Quantity", "0", IconChar.Retweet, DeleteColor, out lblDailyReturnQuantity);

                this.ResumeLayout(false);
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                PositionRefreshButton();
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                PositionRefreshButton();
            }

            private void PositionRefreshButton()
            {
                if (btnRefresh != null && titlePanel != null)
                {
                    btnRefresh.Location = new Point(
                        titlePanel.Width - btnRefresh.Width - 20,
                        (titlePanel.Height - btnRefresh.Height) / 2
                    );
                }
            }

            private void CreateModernCard(string title, string initialValue, IconChar icon, Color accentColor, out Label valueLabel)
            {
                var card = new Panel
                {
                    Size = new Size(300, 100),
                    Margin = new Padding(15),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Padding = new Padding(0)
                };

                // Add rounded corners and shadow
                card.Paint += (sender, e) => PaintCard(sender, e, accentColor);

                // Icon
                var iconControl = new IconPictureBox
                {
                    IconChar = icon,
                    IconColor = accentColor,
                    IconSize = 32,
                    Location = new Point(20, 25),
                    Size = new Size(32, 32),
                    BackColor = Color.Transparent
                };

                // Title label
                var titleLabel = new Label
                {
                    Text = title,
                    Location = new Point(60, 25),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(120, 120, 120),
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = true
                };

                // Value label
                valueLabel = new Label
                {
                    Text = initialValue,
                    Location = new Point(60, 50),
                    Font = new Font("Segoe UI", 20, FontStyle.Bold),
                    ForeColor = Color.FromArgb(60, 60, 60),
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = true
                };

                card.Controls.Add(iconControl);
                card.Controls.Add(titleLabel);
                card.Controls.Add(valueLabel);
                panelCards.Controls.Add(card);
            }

            private void PaintCard(object sender, PaintEventArgs e, Color accentColor)
            {
                var card = sender as Panel;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);

                using (var path = GetRoundedRect(rect, 10))
                {
                    // Shadow
                    for (int i = 0; i < 3; i++)
                    {
                        var shadowRect = new Rectangle(i, i + 2, card.Width - 1, card.Height - 1);
                        using (var shadowPath = GetRoundedRect(shadowRect, 10))
                        {
                            using (var shadow = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                            {
                                e.Graphics.FillPath(shadow, shadowPath);
                            }
                        }
                    }

                    // Card background
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    // Accent bar
                    using (var accentBrush = new SolidBrush(accentColor))
                    {
                        e.Graphics.FillRectangle(accentBrush, 0, 0, card.Width, 4);
                    }

                    // Border
                    using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }

            private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
            {
                GraphicsPath path = new GraphicsPath();
                path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
                path.AddArc(bounds.X + bounds.Width - radius, bounds.Y, radius, radius, 270, 90);
                path.AddArc(bounds.X + bounds.Width - radius, bounds.Y + bounds.Height - radius, radius, radius, 0, 90);
                path.AddArc(bounds.X, bounds.Y + bounds.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();
                return path;
            }

            public void LoadDashboardData()
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    _metrics = _dashboardService.GetCashierMetrics(_currentUser.Employee_ID);

                    // Update cards
                    lblDailySales.Text = _metrics.DailySalesAll.ToString("N2");
                    lblDailyCashIncome.Text = _metrics.DailyCashIncome.ToString("N2");
                    lblDailyQuantitySold.Text = _metrics.DailyQuantitySold.ToString("N0");
                    lblDailyItemsSold.Text = _metrics.DailyItemsSold.ToString("N0");
                    lblDailyBankPayments.Text = _metrics.DailyBankPayments.ToString("N2");
                    lblDailyCashPayments.Text = _metrics.DailyCashPayments.ToString("N2");
                    lblDailyCardPayments.Text = _metrics.DailyCardPayments.ToString("N2");
                    lblDailyReturnAmount.Text = _metrics.DailyReturnAmount.ToString("N2");
                    lblDailyReturnQuantity.Text = _metrics.DailyReturnQuantity.ToString("N0");
                }
                catch (Exception ex)
                {
                    ThemedMessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", ThemedMessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }

            private void AdjustCardSizes()
            {
                if (panelCards.Width <= 0 || panelCards.Controls.Count == 0)
                    return;

                const int minCardWidth = 250;
                const int cardMargin = 15;
                const int cardSpacing = cardMargin * 2;

                // Calculate available width (accounting for panel padding)
                int availableWidth = panelCards.ClientSize.Width - panelCards.Padding.Horizontal;

                // Prevent negative width
                if (availableWidth < minCardWidth)
                    availableWidth = minCardWidth;

                // Calculate how many cards fit per row
                int cardsPerRow = Math.Max(1, (availableWidth + cardSpacing) / (minCardWidth + cardSpacing));

                // Calculate card width (distributing available space evenly)
                int cardWidth = (availableWidth - (cardsPerRow * cardSpacing)) / cardsPerRow;

                // Ensure card doesn't become too narrow
                cardWidth = Math.Max(minCardWidth, cardWidth);

                // Apply to all cards
                foreach (Control card in panelCards.Controls)
                {
                    card.Width = cardWidth;
                }
            }
        }
    }
}
