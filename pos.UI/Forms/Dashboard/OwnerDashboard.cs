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
        // Theme colors based on EmployeeManagement
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
            private static readonly Color WarningColor = Color.FromArgb(241, 196, 15);
            private static readonly Color SuccessColor = Color.FromArgb(46, 204, 113);

            private readonly DashboardService _dashboardService = new DashboardService();
            private DashboardMetrics _metrics;

            // UI Components
            private FlowLayoutPanel panelCards;
            private LiveCharts.WinForms.CartesianChart dailyChart;
            private Button btnRefresh;
            private Panel titlePanel;
            private TableLayoutPanel mainContainer;

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

            public DashboardForm()
            {
                    InitializeComponent();
                    LoadDashboardData();
                    this.Resize += (s, e) => AdjustLayout();
                    AdjustLayout(); // Initial layout adjustment
                }

            private void InitializeComponent()
            {
                // Form Setup
                Text = "Retail POS Dashboard";
                Size = new Size(950, 700);
                BackColor = BackgroundColor;
                AutoScroll = true;  // Fixed: Enable scrolling
                Padding = new Padding(0);
                FormBorderStyle = FormBorderStyle.None;
                ShowIcon = false;   // Remove form icon

                // Main container using TableLayoutPanel for responsive design
                mainContainer = new TableLayoutPanel
                {
                    //Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3,
                    BackColor = BackgroundColor,
                    Padding = new Padding(16),
                    AutoSize = true,
                    //AutoSizeMode = AutoSizeMode.GrowAndShrink
                };
                mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90F));

                // Title Panel
                titlePanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Height = 70,
                    BackColor = PrimaryColor,
                    Padding = new Padding(10, 0, 10, 0)
                };

                var lblTitle = new Label
                {
                    Text = "RETAIL POS DASHBOARD",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                };

                // Refresh Button
                btnRefresh = new Button
                {
                    Text = "REFRESH",
                    Size = new Size(120, 40),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = {
                    BorderSize = 0,
                    MouseOverBackColor = ControlPaint.Light(PrimaryColor, 0.2f)
                },
                    BackColor = PrimaryColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnRefresh.Click += btnRefresh_Click;

                titlePanel.Controls.Add(btnRefresh);
                titlePanel.Controls.Add(lblTitle);

                // Card Panel - Responsive flow layout
                panelCards = new FlowLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Padding = new Padding(0, 10, 0, 20),
                    WrapContents = true,
                    BackColor = BackgroundColor,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
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
                CreateModernCard("Today's Profit", "0.00", IconChar.DollarSign, WarningColor, out lblTodaysProfit);
                CreateModernCard("Today's Quantity", "0", IconChar.ShoppingCart, PrimaryColor, out lblTodaysQuantity);

                // LiveCharts CartesianChart
                dailyChart = new LiveCharts.WinForms.CartesianChart
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor,
                    Margin = new Padding(0, 20, 0, 0),
                    MinimumSize = new Size(950, 300),
                    MaximumSize = new Size(950, 500),
                    Location = new Point(0, 0),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    LegendLocation = LegendLocation.None,
                };
                    dailyChart.Update(true, true);
                    // Configure chart appearance
                    dailyChart.AxisX.Add(new LiveCharts.Wpf.Axis
                {
                    Labels = new string[0],
                    Separator = new Separator { StrokeThickness = 0 },
                    LabelsRotation = 30,
                    FontSize = 11
                });

                dailyChart.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    MinValue = 0,
                    LabelFormatter = value => value.ToString("N2"),
                    Separator = new Separator
                    {
                        Stroke = new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(200, 200, 200))
                    }
                });

                // Add rows to main container
                mainContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F)); // Title
                mainContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Cards
                mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Chart

                // Add controls to container
                mainContainer.Controls.Add(titlePanel, 0, 0);
                mainContainer.Controls.Add(panelCards, 0, 1);
                mainContainer.Controls.Add(dailyChart, 0, 2);

                // Add main container to form
                AutoScroll = true;
                Controls.Add(mainContainer);
             }

            private void AdjustLayout()
            {
                mainContainer.Width = 1000;

                    // Position refresh button dynamically
                btnRefresh.Location = new Point(
                    titlePanel.Width - btnRefresh.Width - 20,
                    (titlePanel.Height - btnRefresh.Height) / 2
                );

                    // Adjust card sizes based on available width
                int cardWidth = CalculateCardWidth();
                foreach (Control card in panelCards.Controls)
                {
                    card.Width = cardWidth;
                }
                //UpdateChart();
            }

            private int CalculateCardWidth()
            {
                const int minCardWidth = 200;
                const int maxCardWidth = 210;
                const int margin = 15;
                const int minCardsPerRow = 3;

                int containerWidth = panelCards.ClientSize.Width - margin;

                if (containerWidth < minCardWidth * minCardsPerRow + margin * minCardsPerRow)
                {
                    // For narrow screens, use 2 columns
                    return Math.Min(maxCardWidth, (containerWidth / 2) - margin * 2);
                }

                // Calculate optimal cards per row
                int cardsPerRow = Math.Max(minCardsPerRow, containerWidth / minCardWidth);

                // Calculate card width
                int calculatedWidth = (containerWidth / cardsPerRow) - margin * 2;

                // Apply constraints
                return Math.Min(maxCardWidth, Math.Max(minCardWidth, calculatedWidth));
            }

            private void CreateModernCard(string title, string initialValue, IconChar icon, Color accentColor, out Label valueLabel)
            {
                int cardWidth = CalculateCardWidth();

                var card = new Panel
                {
                    Size = new Size(cardWidth, 110),
                    Margin = new Padding(10),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    Padding = new Padding(0),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left
                };

                // Add rounded corners and shadow
                card.Paint += (sender, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                    using (var path = GetRoundedRect(rect, 8))
                    {
                        // Shadow
                        using (var shadow = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                var shadowRect = new Rectangle(i, i + 2, card.Width - 1, card.Height - 1);
                                using (var shadowPath = GetRoundedRect(shadowRect, 8))
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
                };

                // Icon
                var iconControl = new IconPictureBox
                {
                    IconChar = icon,
                    IconColor = accentColor,
                    IconSize = 32,
                    Location = new Point(15, 20),
                    Size = new Size(32, 32),
                    BackColor = Color.Transparent
                };

                // Title label
                var titleLabel = new Label
                {
                    Text = title,
                    Location = new Point(55, 20),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(120, 120, 120),
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = true
                };

                // Value label
                valueLabel = new Label
                {
                    Text = initialValue,
                    Location = new Point(55, 45),
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

            private void UpdateChart()
            {
                // Clear existing axes and series
                dailyChart.AxisX.Clear();
                dailyChart.AxisY.Clear();
                dailyChart.Series.Clear();

                if (_metrics?.DailySales == null || _metrics.DailySales.Count == 0)
                    return;

                // Create series with data binding
                var series = new LineSeries
                {
                    Title = "Daily Sales",
                    Values = new ChartValues<decimal>(_metrics.DailySales.Select(d => d.TotalSales)),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    Stroke = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(PrimaryColor.R, PrimaryColor.G, PrimaryColor.B)),
                    Fill = System.Windows.Media.Brushes.Transparent,
                    StrokeThickness = 3
                };
                dailyChart.Series.Add(series);

                // X-Axis Configuration
                dailyChart.AxisX.Add(new LiveCharts.Wpf.Axis
                {
                    Labels = _metrics.DailySales.Select(d => d.Period).ToArray(),
                    Separator = new Separator { StrokeThickness = 0 },
                    LabelsRotation = 30,
                    FontSize = 11
                });

                // Y-Axis Configuration (Fixed)
                dailyChart.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    MinValue = 0,
                    // Remove hardcoded min/max to auto-scale
                    LabelFormatter = value => value.ToString("N2"),
                    Separator = new Separator
                    {
                        Stroke = new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(200, 200, 200))
                    }
                });
            }

            private void LoadDashboardData()
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    _metrics = _dashboardService.GetDashboardMetrics();

                    // Update cards - fixed to ensure values are set
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

                    // Update chart with LiveCharts
                    UpdateChart();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }

            private void btnRefresh_Click(object sender, EventArgs e)
            {
                LoadDashboardData();
            }
        }
    }
}
