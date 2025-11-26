using FontAwesome.Sharp;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Services;
using pos_system.pos.Core;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using pos_system.pos.UI.Forms.Common;
using pos_system.pos.UI.Forms.Inventory;
using pos_system.pos.UI.Forms.Sales;
using pos_system.pos.UI.Forms.Controls;
using System.Data;
using System.Drawing.Drawing2D;
using System.Text;



namespace pos_system.pos.UI.Forms.Dashboard
{
    public partial class OwnerDashboard : Form
    {
        public Employee _currentUser;
        private bool _dragging = false;
        private Point _startPoint = new Point(0, 0);

        public static Color BackgroundColor => Color.FromArgb(214, 208, 208);
        public static Color ForegroundColor => Color.WhiteSmoke;
        public static Color HeaderColor => Color.FromArgb(170, 170, 170);
        public static Color GridLineColor => Color.FromArgb(70, 70, 70);
        public static Color SelectionColor => Color.FromArgb(0, 120, 215);

        public static void ShowThemedMessage(string message)
        {
            using (var msgBox = new Common.ThemedMessageBox(message))
            {
                msgBox.ShowDialog();
            }
        }

        public OwnerDashboard(Employee user)
        {
            InitializeComponent();
            _currentUser = user;

            lblWelcome.Text = $"Welcome, {_currentUser.firstName} {_currentUser.lastName}";
            btnClose.Click += (s, e) => CloseOwnerDashboard();
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            // Setup form dragging
            headerPanel.MouseDown += (s, e) =>
            {
                _dragging = true;
                _startPoint = new Point(e.X, e.Y);
            };

            headerPanel.MouseMove += (s, e) =>
            {
                if (_dragging)
                {
                    Point p = PointToScreen(new Point(e.X, e.Y));
                    Location = new Point(p.X - _startPoint.X, p.Y - _startPoint.Y);
                }
            };

            headerPanel.MouseUp += (s, e) => { _dragging = false; };

            // Also allow dragging via the welcome label
            lblWelcome.MouseDown += (s, e) =>
            {
                _dragging = true;
                _startPoint = new Point(e.X, e.Y);
            };

            lblWelcome.MouseMove += headerPanel_MouseMove;
            lblWelcome.MouseUp += (s, e) => { _dragging = false; };

            CreateSidebarButton("Dashboard", "🏠", 40);
            CreateSidebarButton("Items", "📦", 100);
            CreateSidebarButton("Employees", "👥", 160);
            CreateSidebarButton("Reports", "📊", 220);
            CreateSidebarButton("Brand && Category", "🏷️", 280);
            CreateSidebarButton("Barcode Print", "🖨️", 340);
            CreateSidebarButton("Sales", "💲", 400);
            CreateSidebarButton("Bills", "📄", 460);
            CreateSidebarButton("Return Checking", "🔄", 520);
            CreateSidebarButton("Logout", "🔒", 580);

            OpenChildForm(new DashboardForm(), _dashboardButton);
        }

        private void headerPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(new Point(e.X, e.Y));
                Location = new Point(p.X - _startPoint.X, p.Y - _startPoint.Y);
            }
        }

        private void CloseOwnerDashboard()
        {
            DialogResult result = ThemedMessageBoxYesNo.Show($"Are you sure you want to \nclose the application {_currentUser.firstName}?", "Warning", MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                System.Windows.Forms.Application.Exit(); // Close form only on "Yes"
            }

        }

        private void CreateSidebarButton(string text, string icon, int yPos)
        {
            Button btn = new Button();
            btn.Text = $"{icon}  {text}";
            btn.Tag = text;
            btn.ForeColor = Color.FromArgb(71, 71, 71);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 11);
            btn.Size = new Size(220, 50);
            btn.Location = new Point(0, yPos);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(20, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

            if (text == "Dashboard") _dashboardButton = btn;

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
                        OpenChildForm(new DashboardForm(), btn);
                        break;
                    case "Items":
                        OpenChildForm(new ItemsManagement(), btn);
                        break;
                    case "Brand && Category":
                        OpenChildForm(new BrandAndCategory(), btn);
                        break;
                    case "Employees":
                        OpenChildForm(new EmployeesManagement(), btn);
                        break;
                    case "Reports":
                        OpenChildForm(new ReportsView(), btn);
                        break;
                    case "Barcode Print":
                        OpenChildForm(new BarcodePrint(), btn);
                        break;
                    case "Sales":
                        OpenChildForm(new SalesForm(), btn);
                        break;
                    case "Bills":
                        OpenChildForm(new Bills(), btn);
                        break;
                    case "Return Checking":
                        OpenChildForm(new ReturnChecking(), btn);
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
            DialogResult result = ThemedMessageBoxYesNo.Show($"Are you sure you want to logout {_currentUser.firstName}?", "Warning", MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                Auth.LoginForm login = new pos_system.pos.UI.Forms.Auth.LoginForm();
                login.Show();
            }

            //this.Hide();
            //Auth.LoginForm login = new pos_system.pos.UI.Forms.Auth.LoginForm();
            //login.Show();
        }

        public partial class DashboardForm : Form
        {
            // Theme colors
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
            private static readonly Color WarningColor = Color.FromArgb(241, 196, 15);
            private static readonly Color SuccessColor = Color.FromArgb(46, 204, 113);
            private static readonly Color MixedPaymentColor = Color.FromArgb(155, 89, 182);
            private static readonly Color CardBackgroundColor = Color.FromArgb(250, 250, 250);
            private static readonly Color ShadowColor = Color.FromArgb(100, 0, 0, 0);

            private readonly DashboardService _dashboardService = new DashboardService();
            private DashboardMetrics _metrics;
            private System.Windows.Forms.Timer _refreshTimer;

            // UI Components
            private DoubleBufferedPanel cardsSection;
            private DoubleBufferedPanel lineChartSection;
            private DoubleBufferedPanel pieChartSection;
            private DoubleBufferedFlowLayoutPanel panelCards;
            private LiveCharts.WinForms.CartesianChart dailyChart;
            private LiveCharts.WinForms.PieChart paymentPieChart;
            private DoubleBufferedPanel titlePanel;
            private DoubleBufferedPanel mainPanel;

            // Card Labels
            private Label lblTotalItems;
            private Label lblActiveEmployees;
            private Label lblTotalBills;
            private Label lblTotalReturns;
            private Label lblTotalCategories;
            private Label lblTotalBrands;
            private Label lblTodaysSales;
            private Label lblTodaysCOGS;
            private Label lblTodaysQuantity;
            private Label lblTodaysProfit;

            // Payment Method Labels
            private Label lblCashPayments;
            private Label lblCardPayments;
            private Label lblBankPayments;
            private Label lblMixedPayments;
            private Label lblTokenPayments;

            public DashboardForm()
            {
                InitializeComponent();
                InitializeAutoRefresh();
                LoadDashboardData();

                // Disable auto-scroll and use manual layout
                this.AutoScroll = false;
                this.Resize += (s, e) => AdjustLayout();
                AdjustLayout();
            }

            // Custom double-buffered panels to prevent flickering
            public class DoubleBufferedPanel : Panel
            {
                public DoubleBufferedPanel()
                {
                    this.DoubleBuffered = true;
                    this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                                 ControlStyles.UserPaint |
                                 ControlStyles.ResizeRedraw |
                                 ControlStyles.OptimizedDoubleBuffer, true);
                    this.UpdateStyles();
                }
            }

            public class DoubleBufferedFlowLayoutPanel : FlowLayoutPanel
            {
                public DoubleBufferedFlowLayoutPanel()
                {
                    this.DoubleBuffered = true;
                    this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                                 ControlStyles.UserPaint |
                                 ControlStyles.ResizeRedraw |
                                 ControlStyles.OptimizedDoubleBuffer, true);
                    this.UpdateStyles();
                }
            }

            private void InitializeAutoRefresh()
            {
                _refreshTimer = new System.Windows.Forms.Timer();
                _refreshTimer.Interval = 30000; // 30 seconds
                _refreshTimer.Tick += (s, e) => LoadDashboardData();
                _refreshTimer.Start();
            }

            private void InitializeComponent()
            {
                // Form Setup with double buffering
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                             ControlStyles.UserPaint |
                             ControlStyles.ResizeRedraw |
                             ControlStyles.OptimizedDoubleBuffer, true);

                Text = "Retail POS Dashboard";
                Size = new Size(1200, 1000);
                BackColor = Color.FromArgb(240, 240, 240);
                Padding = new Padding(0);
                FormBorderStyle = FormBorderStyle.None;
                ShowIcon = false;

                // Main Panel - Manual layout without docking to prevent scroll issues
                mainPanel = new DoubleBufferedPanel
                {
                    BackColor = Color.FromArgb(240, 240, 240),
                    Location = new Point(0, 0),
                    Size = new Size(1200, 1000)
                };

                // Title Panel
                titlePanel = CreateShadowCard("RETAIL POS DASHBOARD", 70, PrimaryColor, true);
                titlePanel.Location = new Point(25, 25);

                var lblTitle = new Label
                {
                    Text = "RETAIL POS DASHBOARD",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = PrimaryColor,
                };

                var lblAutoRefresh = new Label
                {
                    Text = "Auto-refresh: 30s",
                    Dock = DockStyle.Right,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10, FontStyle.Italic),
                    ForeColor = Color.White,
                    Size = new Size(120, 70)
                };

                titlePanel.Controls.Add(lblAutoRefresh);
                titlePanel.Controls.Add(lblTitle);

                // Cards Section
                cardsSection = CreateShadowCard("", 0, CardBackgroundColor, false);
                cardsSection.Location = new Point(25, 110);
                cardsSection.Padding = new Padding(20);

                var cardsTitle = new Label
                {
                    Text = "BUSINESS OVERVIEW",
                    Dock = DockStyle.Top,
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = PrimaryColor,
                    Height = 40,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                panelCards = new DoubleBufferedFlowLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Padding = new Padding(0, 10, 0, 0),
                    WrapContents = true,
                    BackColor = Color.Transparent,
                    Dock = DockStyle.Top
                };

                // Create Enhanced Cards with Icons
                CreateModernCard("Total Items", "0", IconChar.Box, PrimaryColor, out lblTotalItems);
                CreateModernCard("Active Employees", "0", IconChar.UserFriends, PrimaryColor, out lblActiveEmployees);
                CreateModernCard("Total Bills", "0", IconChar.Receipt, PrimaryColor, out lblTotalBills);
                CreateModernCard("Total Returns", "0", IconChar.ExchangeAlt, DeleteColor, out lblTotalReturns);
                CreateModernCard("Categories", "0", IconChar.List, PrimaryColor, out lblTotalCategories);
                CreateModernCard("Brands", "0", IconChar.Tags, PrimaryColor, out lblTotalBrands);
                CreateModernCard("Today's Sales", "0.00", IconChar.DollarSign, SuccessColor, out lblTodaysSales);
                CreateModernCard("Today's COS", "0.00", IconChar.MoneyBillWave, WarningColor, out lblTodaysCOGS);
                CreateModernCard("Today's Profit", "0.00", IconChar.ChartLine, SuccessColor, out lblTodaysProfit);
                CreateModernCard("Today's Quantity", "0", IconChar.ShoppingCart, PrimaryColor, out lblTodaysQuantity);

                // Payment Method Breakdown Card
                CreatePaymentMethodCard();

                cardsSection.Controls.Add(panelCards);
                cardsSection.Controls.Add(cardsTitle);

                // Line Chart Section
                lineChartSection = CreateShadowCard("DAILY SALES TREND", 360, CardBackgroundColor, false);
                lineChartSection.Location = new Point(25, 0); // Will be positioned in AdjustLayout
                lineChartSection.Padding = new Padding(25);

                dailyChart = new LiveCharts.WinForms.CartesianChart
                {
                    Height = 300,
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    LegendLocation = LegendLocation.None,
                };

                lineChartSection.Controls.Add(dailyChart);

                // Pie Chart Section
                pieChartSection = CreateShadowCard("PAYMENT METHOD DISTRIBUTION", 360, CardBackgroundColor, false);
                pieChartSection.Location = new Point(25, 0); // Will be positioned in AdjustLayout
                pieChartSection.Padding = new Padding(25);

                paymentPieChart = new LiveCharts.WinForms.PieChart
                {
                    Height = 300,
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    LegendLocation = LegendLocation.Right
                };

                pieChartSection.Controls.Add(paymentPieChart);

                // Configure charts appearance
                ConfigureCharts();

                // Add controls to main panel
                mainPanel.Controls.Add(pieChartSection);
                mainPanel.Controls.Add(lineChartSection);
                mainPanel.Controls.Add(cardsSection);
                mainPanel.Controls.Add(titlePanel);

                // Add main panel to form
                Controls.Add(mainPanel);

                // Enable manual scrolling
                mainPanel.MouseWheel += MainPanel_MouseWheel;
                mainPanel.Focus();
            }

            private void MainPanel_MouseWheel(object sender, MouseEventArgs e)
            {
                // Manual scrolling implementation
                int scrollAmount = e.Delta > 0 ? 40 : -40;
                int newTop = mainPanel.Top + scrollAmount;

                // Limit scrolling bounds
                int maxScroll = Math.Max(0, mainPanel.Height - this.ClientSize.Height);
                newTop = Math.Min(0, Math.Max(-maxScroll, newTop));

                mainPanel.Top = newTop;
            }

            private DoubleBufferedPanel CreateShadowCard(string title, int height, Color backgroundColor, bool isTitleCard)
            {
                var card = new DoubleBufferedPanel
                {
                    BackColor = Color.Transparent,
                    Height = height
                };

                if (!isTitleCard)
                {
                    card.Paint += (sender, e) =>
                    {
                        // Use a single buffered graphics to prevent flickering
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        e.Graphics.InterpolationMode = InterpolationMode.Low;

                        // Single shadow layer (simplified to reduce flickering)
                        var shadowRect = new Rectangle(2, 3, card.Width - 4, card.Height - 4);
                        using (var shadowPath = GetRoundedRect(shadowRect, 10))
                        using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                        {
                            e.Graphics.FillPath(shadowBrush, shadowPath);
                        }

                        // Main card
                        var mainRect = new Rectangle(0, 0, card.Width - 2, card.Height - 2);
                        using (var mainPath = GetRoundedRect(mainRect, 8))
                        using (var mainBrush = new SolidBrush(backgroundColor))
                        using (var borderPen = new Pen(Color.FromArgb(220, 220, 220), 1))
                        {
                            e.Graphics.FillPath(mainBrush, mainPath);
                            e.Graphics.DrawPath(borderPen, mainPath);
                        }

                        // Title bar for sections
                        if (!string.IsNullOrEmpty(title) && !isTitleCard)
                        {
                            var titleRect = new Rectangle(0, 0, card.Width, 40);
                            using (var titleBrush = new SolidBrush(Color.FromArgb(245, 245, 245)))
                            using (var titlePen = new Pen(Color.FromArgb(230, 230, 230), 1))
                            using (var titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
                            using (var titleFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                            {
                                e.Graphics.FillRectangle(titleBrush, titleRect);
                                e.Graphics.DrawLine(titlePen, 0, 40, card.Width, 40);
                                e.Graphics.DrawString(title, titleFont, new SolidBrush(PrimaryColor),
                                    new Rectangle(20, 0, card.Width - 40, 40), titleFormat);
                            }
                        }
                    };
                }

                return card;
            }

            private void ConfigureCharts()
            {
                // Daily Chart Configuration
                dailyChart.AxisX.Add(new LiveCharts.Wpf.Axis
                {
                    Separator = new LiveCharts.Wpf.Separator { StrokeThickness = 0.5 },
                    LabelsRotation = 30,
                    FontSize = 12,
                    Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 100, 100))
                });

                dailyChart.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    MinValue = 0,
                    LabelFormatter = value => value.ToString("N2"),
                    FontSize = 12,
                    Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 100, 100)),
                    Separator = new LiveCharts.Wpf.Separator
                    {
                        Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 220, 220)),
                        StrokeThickness = 1
                    }
                });

                // Pie Chart Configuration
                paymentPieChart.LegendLocation = LiveCharts.LegendLocation.Right;
            }

            private void AdjustLayout()
            {
                // Calculate positions manually to avoid docking issues
                int currentY = 25;
                int sectionWidth = this.ClientSize.Width - 50;

                // Title Panel
                titlePanel.Location = new Point(25, currentY);
                titlePanel.Width = sectionWidth;
                currentY += titlePanel.Height + 25;

                // Cards Section
                cardsSection.Location = new Point(25, currentY);
                cardsSection.Width = sectionWidth;
                cardsSection.Height = panelCards.Height + 80;
                currentY += cardsSection.Height + 25;

                // Line Chart Section
                lineChartSection.Location = new Point(25, currentY + 20);
                lineChartSection.Width = sectionWidth;
                currentY += lineChartSection.Height + 25;

                // Pie Chart Section
                pieChartSection.Location = new Point(25, currentY + 20);
                pieChartSection.Width = sectionWidth;

                // Adjust main panel size for scrolling
                int totalHeight = currentY + pieChartSection.Height + 25;
                mainPanel.Size = new Size(this.ClientSize.Width, Math.Max(this.ClientSize.Height, totalHeight));

                // Adjust card sizes based on available width
                int cardWidth = CalculateCardWidth();
                foreach (Control card in panelCards.Controls)
                {
                    if (card.Tag?.ToString() != "PaymentMethodCard")
                        card.Width = cardWidth;
                }
            }

            private int CalculateCardWidth()
            {
                const int minCardWidth = 270;
                const int maxCardWidth = 280;
                const int margin = 15;

                int containerWidth = panelCards.ClientSize.Width - margin;

                if (containerWidth < minCardWidth * 2 + margin * 2)
                {
                    return Math.Min(maxCardWidth, (containerWidth / 2) - margin * 2);
                }

                int cardsPerRow = Math.Max(3, containerWidth / minCardWidth);
                int calculatedWidth = (containerWidth / cardsPerRow) - margin * 2;

                return Math.Min(maxCardWidth, Math.Max(minCardWidth, calculatedWidth));
            }

            private void CreateModernCard(string title, string initialValue, IconChar icon, Color accentColor, out Label valueLabel)
            {
                int cardWidth = CalculateCardWidth();

                var card = new DoubleBufferedPanel
                {
                    Size = new Size(cardWidth, 120),
                    Margin = new Padding(10),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Padding = new Padding(0)
                };

                // Simplified painting to reduce flickering
                card.Paint += (sender, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);

                    // Single shadow layer
                    var shadowRect = new Rectangle(1, 2, card.Width - 2, card.Height - 2);
                    using (var shadowPath = GetRoundedRect(shadowRect, 8))
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                    {
                        e.Graphics.FillPath(shadowBrush, shadowPath);
                    }

                    // Main card
                    using (var path = GetRoundedRect(rect, 6))
                    using (var brush = new SolidBrush(Color.White))
                    using (var borderPen = new Pen(Color.FromArgb(240, 240, 240), 1))
                    {
                        e.Graphics.FillPath(brush, path);
                        e.Graphics.DrawPath(borderPen, path);
                    }

                    // Accent bar
                    using (var accentBrush = new SolidBrush(accentColor))
                    {
                        e.Graphics.FillRectangle(accentBrush, 0, 0, card.Width, 4);
                    }
                };

                // Icon with background
                var iconContainer = new DoubleBufferedPanel
                {
                    Size = new Size(50, 50),
                    Location = new Point(15, 25),
                    BackColor = Color.FromArgb(245, 245, 245)
                };

                iconContainer.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var path = GetRoundedRect(new Rectangle(0, 0, iconContainer.Width - 1, iconContainer.Height - 1), 8))
                    using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
                    using (var borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
                    {
                        e.Graphics.FillPath(brush, path);
                        e.Graphics.DrawPath(borderPen, path);
                    }
                };

                var iconControl = new IconPictureBox
                {
                    IconChar = icon,
                    IconColor = accentColor,
                    IconSize = 24,
                    Location = new Point(13, 13),
                    Size = new Size(24, 24),
                    BackColor = Color.Transparent
                };

                iconContainer.Controls.Add(iconControl);

                // Title label
                var titleLabel = new Label
                {
                    Text = title,
                    Location = new Point(75, 25),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(120, 120, 120),
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = true
                };

                // Value label
                valueLabel = new Label
                {
                    Text = initialValue,
                    Location = new Point(75, 50),
                    Font = new Font("Segoe UI", 22, FontStyle.Bold),
                    ForeColor = Color.FromArgb(60, 60, 60),
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = true
                };

                card.Controls.Add(iconContainer);
                card.Controls.Add(titleLabel);
                card.Controls.Add(valueLabel);
                panelCards.Controls.Add(card);
            }

            private void CreatePaymentMethodCard()
            {
                var card = new DoubleBufferedPanel
                {
                    Size = new Size(280, 180),
                    Margin = new Padding(10),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Padding = new Padding(0),
                    Tag = "PaymentMethodCard"
                };

                // Simplified card styling
                card.Paint += (sender, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);

                    // Single shadow
                    var shadowRect = new Rectangle(1, 2, card.Width - 2, card.Height - 2);
                    using (var shadowPath = GetRoundedRect(shadowRect, 8))
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                    {
                        e.Graphics.FillPath(shadowBrush, shadowPath);
                    }

                    // Main card
                    using (var path = GetRoundedRect(rect, 6))
                    using (var brush = new SolidBrush(Color.White))
                    using (var borderPen = new Pen(Color.FromArgb(240, 240, 240), 1))
                    {
                        e.Graphics.FillPath(brush, path);
                        e.Graphics.DrawPath(borderPen, path);
                    }

                    // Accent bar
                    using (var accentBrush = new SolidBrush(MixedPaymentColor))
                    {
                        e.Graphics.FillRectangle(accentBrush, 0, 0, card.Width, 4);
                    }
                };

                // Title
                var titleLabel = new Label
                {
                    Text = "Payment Methods",
                    Location = new Point(15, 15),
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    ForeColor = Color.FromArgb(60, 60, 60),
                    AutoSize = true
                };

                // Payment method labels and values
                int startY = 50;
                int labelWidth = 120;
                int valueWidth = 100;

                // Cash
                CreatePaymentMethodRow("Cash", SuccessColor, startY, labelWidth, valueWidth, out lblCashPayments);
                CreatePaymentMethodRow("Card", PrimaryColor, startY + 30, labelWidth, valueWidth, out lblCardPayments);
                CreatePaymentMethodRow("Bank", PrimaryColor, startY + 60, labelWidth, valueWidth, out lblBankPayments);
                CreatePaymentMethodRow("Mixed", MixedPaymentColor, startY + 90, labelWidth, valueWidth, out lblMixedPayments);

                void CreatePaymentMethodRow(string method, Color color, int y, int lblWidth, int valWidth, out Label valueLabel)
                {
                    var icon = new IconPictureBox
                    {
                        IconChar = GetPaymentIcon(method),
                        IconColor = color,
                        IconSize = 16,
                        Location = new Point(20, y + 2),
                        Size = new Size(16, 16)
                    };

                    var label = new Label
                    {
                        Text = method + ":",
                        Location = new Point(45, y),
                        Size = new Size(lblWidth, 20),
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = Color.FromArgb(100, 100, 100)
                    };

                    valueLabel = new Label
                    {
                        Text = "0.00",
                        Location = new Point(150, y),
                        Size = new Size(valWidth, 20),
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = Color.FromArgb(60, 60, 60),
                        TextAlign = ContentAlignment.MiddleRight
                    };

                    card.Controls.Add(icon);
                    card.Controls.Add(label);
                    card.Controls.Add(valueLabel);
                }

                card.Controls.Add(titleLabel);
                panelCards.Controls.Add(card);
            }

            private IconChar GetPaymentIcon(string method)
            {
                return method switch
                {
                    "Cash" => IconChar.MoneyBillWave,
                    "Card" => IconChar.CreditCard,
                    "Bank" => IconChar.University,
                    "Mixed" => IconChar.Random,
                    _ => IconChar.MoneyBillWave
                };
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

            private void UpdateCharts()
            {
                UpdateDailyChart();
                UpdatePaymentPieChart();
            }

            private void UpdateDailyChart()
            {
                dailyChart.AxisX.Clear();
                dailyChart.AxisY.Clear();
                dailyChart.Series.Clear();

                if (_metrics?.DailySales == null || _metrics.DailySales.Count == 0)
                    return;

                var series = new LiveCharts.Wpf.LineSeries
                {
                    Title = "Daily Sales",
                    Values = new LiveCharts.ChartValues<decimal>(_metrics.DailySales.Select(d => d.TotalSales)),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 8,
                    Stroke = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(PrimaryColor.R, PrimaryColor.G, PrimaryColor.B)),
                    Fill = System.Windows.Media.Brushes.Transparent,
                    StrokeThickness = 3
                };
                dailyChart.Series.Add(series);

                dailyChart.AxisX.Add(new LiveCharts.Wpf.Axis
                {
                    Labels = _metrics.DailySales.Select(d => d.Period).ToArray(),
                    Separator = new LiveCharts.Wpf.Separator { StrokeThickness = 0.5 },
                    LabelsRotation = 30,
                    FontSize = 11
                });

                dailyChart.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    MinValue = 0,
                    LabelFormatter = value => value.ToString("N2"),
                    Separator = new LiveCharts.Wpf.Separator
                    {
                        Stroke = new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(220, 220, 220))
                    }
                });
            }

            private void UpdatePaymentPieChart()
            {
                paymentPieChart.Series.Clear();

                if (_metrics == null) return;

                var seriesCollection = new LiveCharts.SeriesCollection();

                if (_metrics.DailyCashPayments > 0)
                {
                    seriesCollection.Add(new LiveCharts.Wpf.PieSeries
                    {
                        Title = "Cash",
                        Values = new LiveCharts.ChartValues<decimal> { _metrics.DailyCashPayments },
                        DataLabels = true,
                        LabelPoint = point => $"{point.Y:N2}",
                        Fill = new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(SuccessColor.R, SuccessColor.G, SuccessColor.B))
                    });
                }

                if (_metrics.DailyCardPayments > 0)
                {
                    seriesCollection.Add(new LiveCharts.Wpf.PieSeries
                    {
                        Title = "Card",
                        Values = new LiveCharts.ChartValues<decimal> { _metrics.DailyCardPayments },
                        DataLabels = true,
                        LabelPoint = point => $"{point.Y:N2}",
                        Fill = new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(PrimaryColor.R, PrimaryColor.G, PrimaryColor.B))
                    });
                }

                if (_metrics.DailyBankPayments > 0)
                {
                    seriesCollection.Add(new LiveCharts.Wpf.PieSeries
                    {
                        Title = "Bank Transfer",
                        Values = new LiveCharts.ChartValues<decimal> { _metrics.DailyBankPayments },
                        DataLabels = true,
                        LabelPoint = point => $"{point.Y:N2}",
                        Fill = new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(70, 130, 180))
                    });
                }

                if (_metrics.DailyMixedPayments > 0)
                {
                    seriesCollection.Add(new LiveCharts.Wpf.PieSeries
                    {
                        Title = "Mixed",
                        Values = new LiveCharts.ChartValues<decimal> { _metrics.DailyMixedPayments },
                        DataLabels = true,
                        LabelPoint = point => $"{point.Y:N2}",
                        Fill = new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(MixedPaymentColor.R, MixedPaymentColor.G, MixedPaymentColor.B))
                    });
                }

                paymentPieChart.Series = seriesCollection;
            }

            private void LoadDashboardData()
            {
                try
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new Action(LoadDashboardData));
                        return;
                    }

                    _metrics = _dashboardService.GetDashboardMetrics();

                    // Update main cards
                    lblTotalItems.Text = _metrics.TotalItems.ToString("N0");
                    lblActiveEmployees.Text = _metrics.ActiveEmployees.ToString("N0");
                    lblTotalBills.Text = _metrics.TotalBills.ToString("N0");
                    lblTotalReturns.Text = _metrics.TotalReturns.ToString("N0");
                    lblTotalCategories.Text = _metrics.TotalCategories.ToString("N0");
                    lblTotalBrands.Text = _metrics.TotalBrands.ToString("N0");
                    lblTodaysSales.Text = _metrics.TodaysSales.ToString("N2");
                    lblTodaysCOGS.Text = _metrics.TodaysCOGS.ToString("N2");
                    lblTodaysProfit.Text = (_metrics.TodaysSales - _metrics.TodaysCOGS).ToString("N2");
                    lblTodaysQuantity.Text = _metrics.TodaysQuantity.ToString("N0");

                    // Update payment method labels
                    lblCashPayments.Text = _metrics.DailyCashPayments.ToString("N2");
                    lblCardPayments.Text = _metrics.DailyCardPayments.ToString("N2");
                    lblBankPayments.Text = _metrics.DailyBankPayments.ToString("N2");
                    lblMixedPayments.Text = _metrics.DailyMixedPayments.ToString("N2");

                    // Update charts
                    UpdateCharts();

                    // Refresh layout
                    AdjustLayout();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _refreshTimer?.Stop();
                    _refreshTimer?.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }
}
