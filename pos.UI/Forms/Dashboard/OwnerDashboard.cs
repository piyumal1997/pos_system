using pos_system.pos.BLL.Services;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System.Data;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using BarTender;
using BTFormat = BarTender.Format;
using BTApplication = BarTender.Application;
using System.Diagnostics;
using pos_system.pos.Core;
using RetailPOS.BLL.Services;
using LiveCharts;
using LiveCharts.WinForms;
using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using pos_system.pos.DAL;
using static pos_system.pos.UI.Forms.Dashboard.CashierForm;
using pos_system.pos.UI.Forms.Inventory;
using LiveCharts.Wpf.Charts.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
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
            btnClose.Click += (s, e) => System.Windows.Forms.Application.Exit();
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

            CreateSidebarButton("Dashboard", "🏠", 80);
            CreateSidebarButton("Items", "📦", 140);
            CreateSidebarButton("Employees", "👥", 200);
            CreateSidebarButton("Reports", "📊", 260);
            CreateSidebarButton("Brand && Category", "🏷️", 320);
            CreateSidebarButton("Barcode Print", "🖨️", 380);
            CreateSidebarButton("Sales", "💲", 440);
            CreateSidebarButton("Bills", "📄", 500);
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
                        OpenChildForm(new Sales(), btn);
                        break;
                    case "Bills":
                        OpenChildForm(new Bills(), btn);
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
            pos_system.pos.UI.Forms.Auth.LoginForm login = new pos_system.pos.UI.Forms.Auth.LoginForm();
            login.Show();
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
                    LabelFormatter = value => value.ToString("0.00"),
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
                    lblTodaysCOGS.Text = _metrics.TodaysCOGS.ToString("N2");
                    lblTodaysCOGS.Text = _metrics.TodaysCOGS.ToString("N2");
                    lblTodaysQuantity.Text = _metrics.TodaysQuantity.ToString("N2");

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
        public partial class ItemsManagement : Form
        {
            private readonly ItemService _itemService = new ItemService();
            private DataGridView dgvItems;
            private Button btnAdd;
            private Button btnEdit;
            private Button btnDelete;
            private Button btnRefresh;
            private Button btnSearch;

            // Theme colors
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
            private static readonly Color ForegroundColor = Color.Black;
            private static readonly Color SecondaryColor = Color.Gray;
            private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
            private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

            public ItemsManagement()
            {
                InitializeComponent();
                LoadItems();
            }

            private void InitializeComponent()
            {
                // Form setup
                this.Size = new Size(1200, 700);
                this.FormBorderStyle = FormBorderStyle.None;
                this.Dock = DockStyle.Fill;
                this.BackColor = BackgroundColor;

                // Main container
                var container = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor,
                    Padding = new Padding(20)
                };

                // Title panel
                var titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor
                };

                var lblTitle = new Label
                {
                    Text = "PRODUCT MANAGEMENT",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.White,
                    Padding = new Padding(20, 0, 0, 0)
                };

                // Toolbar
                var toolbar = new FlowLayoutPanel
                {
                    Dock = DockStyle.Top,
                    Height = 80,
                    FlowDirection = FlowDirection.LeftToRight,
                    Padding = new Padding(0, 15, 0, 15),
                    BackColor = HeaderColor,
                    WrapContents = false,
                    AutoScroll = true
                };

                // Toolbar buttons
                btnAdd = CreateToolbarButton("ADD NEW", PrimaryColor);
                btnEdit = CreateToolbarButton("EDIT", PrimaryColor);
                btnDelete = CreateToolbarButton("DELETE", DeleteColor);
                btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);
                btnSearch = CreateToolbarButton("SEARCH", PrimaryColor);

                // DataGrid
                dgvItems = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AllowUserToAddRows = false,
                    ReadOnly = true,
                    BackgroundColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    AutoGenerateColumns = false,
                    BorderStyle = BorderStyle.None,
                    EnableHeadersVisualStyles = false,
                    RowHeadersVisible = false,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                };

                // Grid styling
                dgvItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };
                //dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dgvItems.ColumnHeadersHeight = 50;

                dgvItems.RowTemplate.Height = 50;
                dgvItems.RowTemplate.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);

                dgvItems.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 12),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor
                };

                dgvItems.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                // Events
                btnAdd.Click += (s, e) => ShowItemForm();
                btnEdit.Click += (s, e) => EditItem();
                btnDelete.Click += (s, e) => DeleteItem();
                btnRefresh.Click += (s, e) => LoadItems();
                btnSearch.Click += (s, e) => LoadSearchItem();

                // Layout
                titlePanel.Controls.Add(lblTitle);
                toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh, btnSearch });

                container.Controls.Add(dgvItems);
                container.Controls.Add(toolbar);
                container.Controls.Add(titlePanel);

                this.Controls.Add(container);
                ConfigureGridColumns();

                // Handle form closing to clean up images
                this.FormClosing += (s, e) => CleanupImages();
            }

            private Button CreateToolbarButton(string text, Color backColor)
            {
                return new Button
                {
                    Text = text,
                    Size = new Size(120, 45),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = {
                BorderSize = 0,
                MouseOverBackColor = ControlPaint.Light(backColor, 0.2f)
            },
                    BackColor = backColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Margin = new Padding(10, 0, 10, 0),
                    Cursor = Cursors.Hand,
                    TextImageRelation = TextImageRelation.ImageBeforeText
                };
            }

            private void ConfigureGridColumns()
            {
                dgvItems.Columns.Clear();

                dgvItems.Columns.AddRange(
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Product_ID",
                        HeaderText = "ID",
                        Visible = false
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Barcode",
                        HeaderText = "BARCODE",
                        Width = 120
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Description",
                        HeaderText = "DESCRIPTION",
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "BrandName",
                        HeaderText = "BRAND",
                        Width = 120
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "CategoryName",
                        HeaderText = "CATEGORY",
                        Width = 120
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "GenderName",
                        HeaderText = "GENDER",
                        Width = 60
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "SizesSummary",
                        HeaderText = "SIZES/STOCK",
                        Width = 280,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleLeft
                        }
                    },
                    new DataGridViewImageColumn
                    {
                        Name = "ImageColumn",
                        DataPropertyName = "ImageObject",
                        HeaderText = "IMAGE",
                        Width = 100,
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleCenter,
                            NullValue = null,
                            Padding = new Padding(5),
                        }
                    }
                );
            }

            private void LoadItems()
            {
                // Clean up previous images
                CleanupImages();

                dgvItems.DataSource = null;
                var items = _itemService.GetAllItems();

                // Process items
                foreach (var item in items)
                {
                    item.SizesSummary = GetSizesSummary(item.Sizes);

                    // Convert byte[] to Image
                    if (item.ItemImage != null && item.ItemImage.Length > 0)
                    {
                        try
                        {
                            byte[] imageData = item.ItemImage as byte[];
                            if (imageData != null && imageData.Length > 0)
                            {
                                using (var ms = new MemoryStream(imageData))
                                {
                                    item.ImageObject = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                item.ImageObject = null;
                            }
                        }
                        catch
                        {
                            item.ImageObject = null;
                        }
                    }
                    else
                    {
                        item.ImageObject = null;
                    }
                }

                dgvItems.DataSource = items;
            }

            private void CleanupImages()
            {
                if (dgvItems.DataSource is IEnumerable<Item> items)
                {
                    foreach (var item in items)
                    {
                        if (item.ImageObject != null)
                        {
                            item.ImageObject.Dispose();
                            item.ImageObject = null;
                        }
                    }
                }
            }

            private string GetSizesSummary(List<ProductSize> sizes)
            {
                if (sizes == null || sizes.Count == 0)
                    return "No Sizes";

                var summary = new List<string>();
                foreach (var size in sizes)
                {
                    summary.Add($"{size.SizeLabel}: {size.Quantity}");
                }
                return string.Join(", ", summary);
            }

            private void LoadSearchItem()
            {
                using var form = new pos_system.pos.UI.Forms.Inventory.SearchItemForm();
                if (form.ShowDialog() == DialogResult.OK)
                    LoadItems();
            }

            private void ShowItemForm(Item item = null)
            {
                using var form = new ItemForm(item);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadItems();
            }

            private void EditItem()
            {
                if (dgvItems.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select a product to edit");
                    return;
                }

                var item = dgvItems.SelectedRows[0].DataBoundItem as Item;
                if (item == null) return;

                var fullItem = _itemService.GetItemById(item.Product_ID);
                ShowItemForm(fullItem);
            }

            private void DeleteItem()
            {
                if (dgvItems.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select a product to delete");
                    return;
                }

                var item = dgvItems.SelectedRows[0].DataBoundItem as Item;
                if (item == null) return;

                if (ConfirmDelete($"Are you sure you want to delete '{item.Description}'?"))
                {
                    if (_itemService.DeleteItem(item.Product_ID))
                        LoadItems();
                    else
                        ShowMessage("Error deleting product");
                }
            }

            private void ShowMessage(string text)
            {
                MessageBox.Show(text, "Product Management",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            private bool ConfirmDelete(string message)
            {
                return MessageBox.Show(message, "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }
        }
        public class EmployeesManagement : Form
        {
            private readonly EmployeeService _service = new EmployeeService();
            private DataGridView dgvEmployees = null!;
            private Button btnAdd = null!;
            private Button btnEdit = null!;
            private Button btnToggleStatus = null!;
            private Button btnRefresh = null!;

            // Theme colors based on LoginForm
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
            private static readonly Color ForegroundColor = Color.Black;
            private static readonly Color SecondaryColor = Color.Gray;
            private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
            private static readonly Color WarningColor = Color.FromArgb(241, 196, 15);
            private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

            public EmployeesManagement()
            {
                InitializeComponents();
                LoadEmployees();
            }

            private void InitializeComponents()
            {
                // Form setup
                this.Size = new Size(980, 656);
                this.FormBorderStyle = FormBorderStyle.None;
                this.Dock = DockStyle.Fill;
                this.BackColor = BackgroundColor;

                // Main container
                var container = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor,
                    Padding = new Padding(20)
                };

                // Title panel
                var titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor
                };

                var lblTitle = new Label
                {
                    Text = "EMPLOYEE MANAGEMENT",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.White,
                    Padding = new Padding(20, 0, 0, 0)
                };

                // Toolbar
                var toolbar = new FlowLayoutPanel
                {
                    Dock = DockStyle.Top,
                    Height = 80,
                    FlowDirection = FlowDirection.LeftToRight,
                    Padding = new Padding(0, 15, 0, 15),
                    BackColor = HeaderColor,
                    WrapContents = false,
                    AutoScroll = true
                };

                // Toolbar buttons
                btnAdd = CreateToolbarButton("ADD NEW", PrimaryColor);
                btnEdit = CreateToolbarButton("EDIT", PrimaryColor);
                btnToggleStatus = CreateToolbarButton("DELETE", DeleteColor);
                btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);

                // DataGrid
                dgvEmployees = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AllowUserToAddRows = false,
                    ReadOnly = true,
                    BackgroundColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    AutoGenerateColumns = false,
                    BorderStyle = BorderStyle.None,
                    EnableHeadersVisualStyles = false,
                    RowHeadersVisible = false,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                };

                // Grid styling
                dgvEmployees.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };

                dgvEmployees.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dgvEmployees.RowTemplate.Height = 100;
                dgvEmployees.RowTemplate.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);

                dgvEmployees.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 11),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor
                };

                dgvEmployees.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                // Events
                btnAdd.Click += (s, e) => ShowEmployeeForm();
                btnEdit.Click += (s, e) => EditEmployee();
                btnToggleStatus.Click += (s, e) => ToggleStatus();
                btnRefresh.Click += (s, e) => LoadEmployees();

                // Layout
                titlePanel.Controls.Add(lblTitle);
                toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnToggleStatus, btnRefresh });

                container.Controls.Add(dgvEmployees);
                container.Controls.Add(toolbar);
                container.Controls.Add(titlePanel);

                this.Controls.Add(container);
                ConfigureGridColumns();
            }

            private Button CreateToolbarButton(string text, Color backColor)
            {
                return new Button
                {
                    Text = text,
                    Size = new Size(120, 45),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = {
                BorderSize = 0,
                MouseOverBackColor = ControlPaint.Light(backColor, 0.2f)
            },
                    BackColor = backColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Margin = new Padding(10, 0, 10, 0),
                    Cursor = Cursors.Hand,
                    TextImageRelation = TextImageRelation.ImageBeforeText
                };
            }

            private void ConfigureGridColumns()
            {
                dgvEmployees.Columns.Clear();

                dgvEmployees.Columns.AddRange(
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Employee_ID",
                        HeaderText = "ID",
                        Name = "Employee_ID",
                        Visible = false
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "firstName",
                        HeaderText = "FIRST NAME",
                        Name = "firstName",
                        Width = 150
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "lastName",
                        HeaderText = "LAST NAME",
                        Name = "lastName",
                        Width = 150
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "nic",
                        HeaderText = "NIC",
                        Width = 150
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "contactno",
                        HeaderText = "CONTACT",
                        Width = 150
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "RoleName",
                        HeaderText = "ROLE",
                        Width = 150
                    },
                    new DataGridViewImageColumn
                    {
                        DataPropertyName = "picture",
                        HeaderText = "PHOTO",
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        Width = 80,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Padding = new Padding(5),
                            Alignment = DataGridViewContentAlignment.MiddleCenter
                        }
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "status",
                        HeaderText = "STATUS",
                        Name = "status",
                        Width = 120,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleCenter
                        }
                    }
                );
            }

            private void LoadEmployees() => dgvEmployees.DataSource = _service.GetAllEmployees();

            private void ShowEmployeeForm(Employee employee = null)
            {
                using var form = new pos_system.pos.UI.Forms.Controls.EmployeeForm(employee);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadEmployees();
            }

            private void EditEmployee()
            {
                if (dgvEmployees.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select an employee to edit");
                    return;
                }

                var row = (dgvEmployees.SelectedRows[0].DataBoundItem as DataRowView).Row;
                var employee = new Employee
                {
                    Employee_ID = (int)row["Employee_ID"],
                    firstName = row["firstName"].ToString(),
                    lastName = row["lastName"].ToString(),
                    nic = row["nic"].ToString(),
                    userName = row["userName"].ToString(),
                    password = row["password"].ToString(), //Hashing Password is there
                    contactNo = row["contactNo"].ToString(),
                    status = row["status"].ToString(),
                    Role_ID = (int)row["Role_ID"],
                    address = row["address"].ToString(),
                    email = row["email"].ToString(),
                    picture = row["picture"] as byte[]
                };

                ShowEmployeeForm(employee);
            }

            private void ToggleStatus()
            {
                if (dgvEmployees.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select an employee");
                    return;
                }

                var row = dgvEmployees.SelectedRows[0];
                int id = (int)row.Cells["Employee_ID"].Value;

                if (id == 1)
                {
                    ShowMessage("Cannot change status of the Owner");
                    return;
                }

                string name = $"{row.Cells["firstName"].Value} {row.Cells["lastName"].Value}";
                string currentStatus = row.Cells["status"].Value?.ToString() ?? string.Empty;
                string newStatus = currentStatus == "Active" ? "Inactive" : "Active";

                if (ConfirmAction($"Change status of {name} to {newStatus}?"))
                {
                    _service.ToggleStatus(id);
                    LoadEmployees();
                }
            }

            private void ShowMessage(string text)
            {
                MessageBox.Show(text, "Employee Management",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            private bool ConfirmAction(string message)
            {
                return MessageBox.Show(message, "Confirm Action",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }
        }
        public class BrandAndCategory : Form
        {
            private readonly BrandService _brandService = new BrandService();
            private readonly CategoryService _categoryService = new CategoryService();
            private TabControl tabControl;
            private DataGridView dgvBrands;
            private DataGridView dgvCategories;

            // Theme colors based on LoginForm
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
            private static readonly Color ForegroundColor = Color.Black;
            private static readonly Color SecondaryColor = Color.Gray;
            private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
            private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

            private DataGridView dgvSizes;
            private readonly SizeService _sizeService = new SizeService();

            public BrandAndCategory()
            {
                InitializeComponent();
                LoadBrands();
                LoadCategories();
                LoadSizes();
            }

            private Panel CreateSizeManagementLayout()
            {
                var panel = new Panel { Dock = DockStyle.Fill, BackColor = BackgroundColor };

                // Title panel
                var titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor
                };

                var lblTitle = new Label
                {
                    Text = "SIZE MANAGEMENT",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                    Padding = new Padding(20, 0, 0, 0)
                };

                // Toolbar
                var toolbar = new FlowLayoutPanel
                {
                    Dock = DockStyle.Top,
                    Height = 80,
                    FlowDirection = FlowDirection.LeftToRight,
                    Padding = new Padding(0, 15, 0, 15),
                    BackColor = HeaderColor,
                    WrapContents = false
                };

                // Toolbar buttons
                var btnAdd = CreateToolbarButton("ADD", PrimaryColor);
                var btnEdit = CreateToolbarButton("EDIT", PrimaryColor);
                var btnDelete = CreateToolbarButton("DELETE", DeleteColor);
                var btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);

                // DataGrid
                dgvSizes = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AllowUserToAddRows = false,
                    ReadOnly = true,
                    BackgroundColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    AutoGenerateColumns = false,
                    BorderStyle = BorderStyle.None,
                    EnableHeadersVisualStyles = false,
                    RowHeadersVisible = false,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                };

                // Grid styling
                dgvSizes.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };

                dgvSizes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

                dgvSizes.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 11),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor,
                    Padding = new Padding(10, 5, 10, 5),
                };

                dgvSizes.RowTemplate.Height = 32;
                dgvSizes.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                // Events
                btnAdd.Click += (s, e) => ShowSizeForm();
                btnEdit.Click += (s, e) => EditSize();
                btnDelete.Click += (s, e) => DeleteSize();
                btnRefresh.Click += (s, e) => LoadSizes();

                // Layout
                titlePanel.Controls.Add(lblTitle);
                toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });
                panel.Controls.Add(dgvSizes);
                panel.Controls.Add(toolbar);
                panel.Controls.Add(titlePanel);

                // Configure columns
                dgvSizes.Columns.AddRange(
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Size_ID",
                        HeaderText = "ID",
                        Visible = false
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "SizeLabel",
                        HeaderText = "SIZE LABEL",
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "SizeType",
                        HeaderText = "SIZE TYPE",
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    }
                );

                LoadSizes();

                return panel;
            }

            private void LoadSizes()
            {
                dgvSizes.DataSource = null;
                dgvSizes.DataSource = _sizeService.GetAllSize();
                dgvSizes.ClearSelection();
            }

            private void ShowSizeForm(Sizes size = null)
            {
                using var form = new SizeForm(size);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadSizes();
            }

            private void EditSize()
            {
                if (dgvSizes.SelectedRows.Count == 0) return;
                dynamic size = dgvSizes.SelectedRows[0].DataBoundItem;
                ShowSizeForm(new Sizes
                {
                    Size_ID = size.Size_ID,
                    SizeLabel = size.SizeLabel,
                    SizeType = size.SizeType
                });
            }

            private void DeleteSize()
            {
                if (dgvSizes.SelectedRows.Count == 0) return;
                dynamic size = dgvSizes.SelectedRows[0].DataBoundItem;

                if (ConfirmAction($"Delete size '{size.SizeLabel}' ({size.SizeType})?"))
                {
                    if (_sizeService.DeleteSize(size.Size_ID))
                        LoadSizes();
                    else
                        ShowMessage("Error deleting size");
                }
            }

            private void InitializeComponent()
            {
                // Form setup
                this.Size = new Size(980, 656);
                this.FormBorderStyle = FormBorderStyle.None;
                this.Dock = DockStyle.Fill;
                this.BackColor = BackgroundColor;

                // Main container
                var container = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor,
                    Padding = new Padding(20)
                };

                // Tab control with modern styling
                tabControl = new TabControl
                {
                    Dock = DockStyle.Fill,
                    Appearance = TabAppearance.FlatButtons,
                    ItemSize = new Size(180, 40),
                    SizeMode = TabSizeMode.Fixed,
                    DrawMode = TabDrawMode.OwnerDrawFixed
                };

                // Custom tab drawing
                tabControl.DrawItem += (sender, e) =>
                {
                    var tabPage = tabControl.TabPages[e.Index];
                    var tabRect = tabControl.GetTabRect(e.Index);
                    var isSelected = tabControl.SelectedIndex == e.Index;

                    // Background
                    using (var brush = new SolidBrush(isSelected ? PrimaryColor : HeaderColor))
                        e.Graphics.FillRectangle(brush, tabRect);

                    // Text
                    TextRenderer.DrawText(
                        e.Graphics,
                        tabPage.Text,
                        new Font("Segoe UI", 13, isSelected ? FontStyle.Bold : FontStyle.Regular),
                        tabRect,
                        isSelected ? Color.White : Color.Black,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    );
                };

                tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;

                // Brands tab
                var tabBrands = new TabPage { Text = "BRANDS" };
                tabBrands.BackColor = BackgroundColor;
                tabBrands.Controls.Add(CreateBrandManagementLayout());

                // Categories tab
                var tabCategories = new TabPage { Text = "CATEGORIES" };
                tabCategories.BackColor = BackgroundColor;
                tabCategories.Controls.Add(CreateCategoryManagementLayout());

                tabControl.TabPages.Add(tabBrands);
                tabControl.TabPages.Add(tabCategories);
                container.Controls.Add(tabControl);
                this.Controls.Add(container);

                var tabSizes = new TabPage { Text = "SIZES" };
                tabSizes.BackColor = BackgroundColor;
                tabSizes.Controls.Add(CreateSizeManagementLayout());
                tabControl.TabPages.Add(tabSizes);
            }

            private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (tabControl.SelectedTab?.Text == "SIZES")
                {
                    LoadSizes();
                }
            }

            private Panel CreateBrandManagementLayout()
            {
                var panel = new Panel { Dock = DockStyle.Fill, BackColor = BackgroundColor };

                // Title panel
                var titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor
                };

                var lblTitle = new Label
                {
                    Text = "BRAND MANAGEMENT",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                    Padding = new Padding(20, 0, 0, 0)
                };

                // Toolbar
                var toolbar = new FlowLayoutPanel
                {
                    Dock = DockStyle.Top,
                    Height = 80,
                    FlowDirection = FlowDirection.LeftToRight,
                    Padding = new Padding(0, 15, 0, 15),
                    BackColor = HeaderColor,
                    WrapContents = false
                };

                // Toolbar buttons
                var btnAdd = CreateToolbarButton("ADD", PrimaryColor);
                var btnEdit = CreateToolbarButton("EDIT", PrimaryColor);
                var btnDelete = CreateToolbarButton("DELETE", DeleteColor);
                var btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);

                // DataGrid
                dgvBrands = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AllowUserToAddRows = false,
                    ReadOnly = true,
                    BackgroundColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    AutoGenerateColumns = false,
                    BorderStyle = BorderStyle.None,
                    EnableHeadersVisualStyles = false,
                    RowHeadersVisible = false,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                };

                dgvBrands.RowTemplate.Height = 32;
                // Grid styling
                dgvBrands.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };

                dgvBrands.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

                dgvBrands.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 11),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor,
                    Padding = new Padding(10, 5, 10, 5),
                };
             

                dgvBrands.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                // Events
                btnAdd.Click += (s, e) => ShowBrandForm();
                btnEdit.Click += (s, e) => EditBrand();
                btnDelete.Click += (s, e) => DeleteBrand();
                btnRefresh.Click += (s, e) => LoadBrands();

                // Layout
                titlePanel.Controls.Add(lblTitle);
                toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });
                panel.Controls.Add(dgvBrands);
                panel.Controls.Add(toolbar);
                panel.Controls.Add(titlePanel);

                // Configure columns
                dgvBrands.Columns.AddRange(
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Brand_ID",
                        Name = "Brand_ID",
                        HeaderText = "ID",
                        Visible = false
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "brandName",
                        Name = "brandName",
                        HeaderText = "BRAND NAME",
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    }
                );

                return panel;
            }

            private Panel CreateCategoryManagementLayout()
            {
                var panel = new Panel { Dock = DockStyle.Fill, BackColor = BackgroundColor };

                // Title panel
                var titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor
                };

                var lblTitle = new Label
                {
                    Text = "CATEGORY MANAGEMENT",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                    Padding = new Padding(20, 0, 0, 0)
                };

                // Toolbar
                var toolbar = new FlowLayoutPanel
                {
                    Dock = DockStyle.Top,
                    Height = 80,
                    FlowDirection = FlowDirection.LeftToRight,
                    Padding = new Padding(0, 15, 0, 15),
                    BackColor = HeaderColor,
                    WrapContents = false
                };

                // Toolbar buttons
                var btnAdd = CreateToolbarButton("ADD", PrimaryColor);
                var btnEdit = CreateToolbarButton("EDIT", PrimaryColor);
                var btnDelete = CreateToolbarButton("DELETE", DeleteColor);
                var btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);

                // DataGrid
                dgvCategories = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AllowUserToAddRows = false,
                    ReadOnly = true,
                    BackgroundColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    AutoGenerateColumns = false,
                    BorderStyle = BorderStyle.None,
                    EnableHeadersVisualStyles = false,
                    RowHeadersVisible = false,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                };

                // Grid styling
                dgvCategories.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };

                dgvCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

                dgvCategories.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 11),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor,
                    Padding = new Padding(10, 5, 10, 5),

                };

                dgvCategories.RowTemplate.Height = 32;
                dgvCategories.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                // Events
                btnAdd.Click += (s, e) => ShowCategoryForm();
                btnEdit.Click += (s, e) => EditCategory();
                btnDelete.Click += (s, e) => DeleteCategory();
                btnRefresh.Click += (s, e) => LoadCategories();

                // Layout
                titlePanel.Controls.Add(lblTitle);
                toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });
                panel.Controls.Add(dgvCategories);
                panel.Controls.Add(toolbar);
                panel.Controls.Add(titlePanel);

                // Configure columns
                dgvCategories.Columns.AddRange(
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Category_ID",
                        HeaderText = "ID",
                        Visible = false
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "categoryName",
                        HeaderText = "CATEGORY NAME",
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    }
                );

                return panel;
            }

            private Button CreateToolbarButton(string text, Color backColor)
            {
                return new Button
                {
                    Text = text,
                    Size = new Size(120, 45),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = {
                BorderSize = 0,
                MouseOverBackColor = ControlPaint.Light(backColor, 0.2f)
            },
                    BackColor = backColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Margin = new Padding(10, 0, 10, 0),
                    Cursor = Cursors.Hand,
                    TextImageRelation = TextImageRelation.ImageBeforeText
                };
            }

            private void LoadBrands() => dgvBrands.DataSource = _brandService.GetAllBrands();

            private void LoadCategories() => dgvCategories.DataSource = _categoryService.GetAllCategories();

            private void EditBrand()
            {
                if (dgvBrands.SelectedRows.Count == 0) return;

                // Cast directly to your Brand object
                var brand = dgvBrands.SelectedRows[0].DataBoundItem as Brand;
                Debug.WriteLine($"Data {brand}");
                if (brand == null) return;

                ShowBrandForm(brand);
            }

            private void ShowBrandForm(Brand brand = null)
            {
                using var form = new BrandForm(brand);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadBrands();
            }

            private void DeleteBrand()
            {
                if (dgvBrands.SelectedRows.Count == 0) return;

                var brand = dgvBrands.SelectedRows[0].DataBoundItem as Brand;
                if (brand == null) return;

                if (ConfirmAction($"Delete brand '{brand.brandName}'?"))
                {
                    if (_brandService.DeleteBrand(brand.Brand_ID))
                        LoadBrands();
                    else
                        ShowMessage("Error deleting brand");
                }
            }

            private void ShowCategoryForm(Category category = null)
            {
                using var form = new CategoryForm(category);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadCategories();
            }

            private void EditCategory()
            {
                if (dgvCategories.SelectedRows.Count == 0) return;

                var row = (dgvCategories.SelectedRows[0].DataBoundItem as DataRowView)?.Row;
                if (row == null) return;

                var category = new Category
                {
                    Category_ID = (int)row["Category_ID"],
                    categoryName = row["categoryName"].ToString()
                };

                ShowCategoryForm(category);
            }

            private void DeleteCategory()
            {
                if (dgvCategories.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select a category to delete");
                    return;
                }

                var row = dgvCategories.SelectedRows[0];
                int id = (int)row.Cells["Category_ID"].Value;
                string name = row.Cells["categoryName"].Value?.ToString() ?? string.Empty;

                if (ConfirmAction($"Delete category '{name}'?"))
                {
                    if (_categoryService.DeleteCategory(id))
                        LoadCategories();
                    else
                        ShowMessage("Error deleting category");
                }
            }

            private void ShowMessage(string text)
            {
                MessageBox.Show(text, "Brand & Category",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            private bool ConfirmAction(string message)
            {
                return MessageBox.Show(message, "Confirm Action",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }
        }
        public partial class BrandForm : Form
        {
            private readonly Brand _brand;
            private readonly BrandService _service = new BrandService();
            private TextBox txtName;
            private Button btnClose;
            private Button btnSave;
            private Button btnCancel;
            private Label lblTitle;

            private Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private Color ButtonHoverColor = Color.FromArgb(31, 97, 141);
            private Color ButtonGray = Color.FromArgb(120, 120, 120);

            public BrandForm(Brand brand = null)
            {
                _brand = brand ?? new Brand();
                InitializeComponent();
                new DropShadow().ApplyShadows(this);
            }

            private void InitializeComponent()
            {
                this.SuspendLayout();
                this.Size = new Size(400, 220);
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.CenterParent;
                this.BackColor = Color.White;
                this.Font = new Font("Segoe UI", 10);
                this.Text = _brand.Brand_ID > 0 ? "Edit Brand" : "Add Brand";
                this.Padding = new Padding(1);

                // Main container panel - holds everything
                Panel mainContainer = new Panel();
                mainContainer.Dock = DockStyle.Fill;
                mainContainer.BackColor = Color.White;
                this.Controls.Add(mainContainer);

                // TOP PANEL (header)
                Panel topPanel = new Panel();
                topPanel.Dock = DockStyle.Top;
                topPanel.Height = 40;
                topPanel.BackColor = PrimaryColor;
                topPanel.Padding = new Padding(0, 0, 10, 0);
                mainContainer.Controls.Add(topPanel);

                // Title label
                lblTitle = new Label();
                lblTitle.Dock = DockStyle.Left;
                lblTitle.Text = _brand.Brand_ID > 0 ? "Edit Brand" : "Add Brand";
                lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                lblTitle.ForeColor = Color.White;
                lblTitle.Padding = new Padding(10, 10, 0, 0);
                lblTitle.AutoSize = true;
                topPanel.Controls.Add(lblTitle);

                // Close button
                btnClose = new Button();
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.FlatStyle = FlatStyle.Flat;
                btnClose.Font = new Font("Segoe UI", 12F);
                btnClose.ForeColor = Color.White;
                btnClose.Text = "✕";
                btnClose.Dock = DockStyle.Right;
                btnClose.Size = new Size(40, 40);
                btnClose.Cursor = Cursors.Hand;
                btnClose.Click += (s, e) => this.Close();
                topPanel.Controls.Add(btnClose);

                // CONTENT PANEL - holds form elements
                Panel contentPanel = new Panel();
                contentPanel.Dock = DockStyle.Fill;
                contentPanel.BackColor = Color.White;
                contentPanel.Padding = new Padding(20, 40, 20, 20);
                mainContainer.Controls.Add(contentPanel);

                // Create a container for the form elements
                Panel formContainer = new Panel();
                formContainer.Dock = DockStyle.Fill;
                formContainer.BackColor = Color.White;
                contentPanel.Controls.Add(formContainer);

                // Brand Name Label
                Label lblName = new Label();
                lblName.Text = "Brand Name:";
                lblName.Location = new Point(20, 30);
                lblName.AutoSize = true;
                lblName.ForeColor = Color.Black;
                formContainer.Controls.Add(lblName);

                // Brand Name TextBox
                txtName = new TextBox();
                txtName.Location = new Point(150, 30);
                txtName.Size = new Size(200, 30);
                txtName.Text = _brand.brandName;
                txtName.BorderStyle = BorderStyle.FixedSingle;
                formContainer.Controls.Add(txtName);

                // BUTTON PANEL - at bottom of content panel
                Panel buttonPanel = new Panel();
                buttonPanel.Dock = DockStyle.Bottom;
                buttonPanel.Height = 60;
                buttonPanel.BackColor = Color.White;
                contentPanel.Controls.Add(buttonPanel);

                FlowLayoutPanel buttonFlow = new FlowLayoutPanel();
                buttonFlow.FlowDirection = FlowDirection.RightToLeft;
                buttonFlow.Dock = DockStyle.Fill;
                buttonFlow.Padding = new Padding(0, 10, 10, 0);
                buttonPanel.Controls.Add(buttonFlow);

                // Cancel button
                btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.Size = new Size(100, 40);
                btnCancel.BackColor = ButtonGray;
                btnCancel.FlatStyle = FlatStyle.Flat;
                btnCancel.FlatAppearance.BorderSize = 0;
                btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnCancel.ForeColor = Color.White;
                btnCancel.Cursor = Cursors.Hand;
                btnCancel.Margin = new Padding(0);
                btnCancel.Click += (s, e) => this.Close();
                buttonFlow.Controls.Add(btnCancel);

                // Save button
                btnSave = new Button();
                btnSave.Text = "Save";
                btnSave.Size = new Size(100, 40);
                btnSave.BackColor = PrimaryColor;
                btnSave.FlatStyle = FlatStyle.Flat;
                btnSave.FlatAppearance.BorderSize = 0;
                btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnSave.ForeColor = Color.White;
                btnSave.Margin = new Padding(0, 0, 10, 0);
                btnSave.Cursor = Cursors.Hand;
                btnSave.Click += (s, e) =>
                {
                    if (ValidateAndSave())
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                };
                buttonFlow.Controls.Add(btnSave);

                // Add button hover effects
                btnSave.MouseEnter += (s, e) => btnSave.BackColor = ButtonHoverColor;
                btnSave.MouseLeave += (s, e) => btnSave.BackColor = PrimaryColor;
                btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.Gray;
                btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = ButtonGray;
                btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(200, 50, 50);
                btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.Transparent;

                this.ResumeLayout(true);
            }

            private bool ValidateAndSave()
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    ShowThemedMessage("Brand name cannot be empty");
                    return false;
                }

                if (_service.CheckBrandExists(txtName.Text,
                    _brand.Brand_ID > 0 ? _brand.Brand_ID : (int?)null))
                {
                    ShowThemedMessage("Brand name already exists");
                    return false;
                }

                _brand.brandName = txtName.Text.Trim();

                bool success = _brand.Brand_ID > 0
                    ? _service.UpdateBrand(_brand.Brand_ID, _brand.brandName)
                    : _service.AddBrand(_brand.brandName);

                if (!success)
                {
                    ShowThemedMessage("Error saving brand");
                    return false;
                }
                return true;
            }

            private void ShowThemedMessage(string message)
            {
                MessageBox.Show(this, message, "Brand",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Enable form dragging
            private bool _dragging;
            private Point _startPoint = new Point(0, 0);

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                if (e.Button == MouseButtons.Left && e.Y < 40) // Only drag from header area
                {
                    _dragging = true;
                    _startPoint = new Point(e.X, e.Y);
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (_dragging)
                {
                    Point p = PointToScreen(e.Location);
                    Location = new Point(p.X - this._startPoint.X, p.Y - this._startPoint.Y);
                }
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                _dragging = false;
            }
        }
        public partial class CategoryForm : Form
        {
            private readonly Category _category;
            private readonly CategoryService _categoryService = new CategoryService();
            private readonly SizeService _sizeService = new SizeService();
            private List<CategorySize> _allSizes;
            private CheckedListBox clbSizes;
            private TextBox txtName;
            private Button btnClose;
            private Button btnSave;
            private Button btnCancel;
            private Label lblTitle;

            private Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private Color ButtonHoverColor = Color.FromArgb(31, 97, 141);
            private Color ButtonGray = Color.FromArgb(120, 120, 120);

            public CategoryForm(Category category = null)
            {
                _category = category ?? new Category();
                InitializeComponent();
                this.Load += CategoryForm_Load;
                new DropShadow().ApplyShadows(this);
            }

            private void CategoryForm_Load(object sender, EventArgs e)
            {
                LoadSizes();
            }

            private void LoadSizes()
            {
                try
                {
                    _allSizes = _sizeService.GetAllSizes();
                    clbSizes.Items.Clear();

                    foreach (var group in _allSizes.GroupBy(s => s.SizeType))
                    {
                        // Add header with null Size_ID
                        clbSizes.Items.Add(new SizeItem
                        {
                            Text = $"--- {group.Key} ---",
                            Size_ID = null
                        }, false);

                        foreach (var size in group)
                        {
                            // Store both text and Size_ID
                            clbSizes.Items.Add(new SizeItem
                            {
                                Text = size.SizeLabel,
                                Size_ID = size.Size_ID
                            }, false);
                        }
                    }

                    if (_category.Category_ID > 0)
                    {
                        var assignedSizes = _categoryService.GetSizesByCategoryId(_category.Category_ID);
                        var assignedSizeIds = assignedSizes.Select(s => s.Size_ID).ToList();

                        for (int i = 0; i < clbSizes.Items.Count; i++)
                        {
                            var item = (SizeItem)clbSizes.Items[i];
                            // Check if item is a valid size and assigned to category
                            if (item.Size_ID.HasValue && assignedSizeIds.Contains(item.Size_ID.Value))
                            {
                                clbSizes.SetItemChecked(i, true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowThemedMessage($"Error loading sizes: {ex.Message}");
                }
            }

            private class SizeItem
            {
                public string Text { get; set; }
                public int? Size_ID { get; set; }
                public override string ToString() => Text;
            }

            private void InitializeComponent()
            {
                this.SuspendLayout();
                this.Size = new Size(500, 550);
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.CenterParent;
                this.BackColor = Color.White;
                this.Font = new Font("Segoe UI", 10);
                this.Text = _category.Category_ID > 0 ? "Edit Category" : "Add Category";
                this.Padding = new Padding(1);

                // Main container panel - holds everything
                Panel mainContainer = new Panel();
                mainContainer.Dock = DockStyle.Fill;
                mainContainer.BackColor = Color.White;
                this.Controls.Add(mainContainer);

                // TOP PANEL (header)
                Panel topPanel = new Panel();
                topPanel.Dock = DockStyle.Top;
                topPanel.Height = 40;
                topPanel.BackColor = PrimaryColor;
                topPanel.Padding = new Padding(0, 0, 10, 0);
                mainContainer.Controls.Add(topPanel);

                // Title label
                lblTitle = new Label();
                lblTitle.Dock = DockStyle.Left;
                lblTitle.Text = _category.Category_ID > 0 ? "Edit Category" : "Add Category";
                lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                lblTitle.ForeColor = Color.White;
                lblTitle.Padding = new Padding(10, 10, 0, 0);
                lblTitle.AutoSize = true;
                topPanel.Controls.Add(lblTitle);

                // Close button
                btnClose = new Button();
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.FlatStyle = FlatStyle.Flat;
                btnClose.Font = new Font("Segoe UI", 12F);
                btnClose.ForeColor = Color.White;
                btnClose.Text = "✕";
                btnClose.Dock = DockStyle.Right;
                btnClose.Size = new Size(40, 40);
                btnClose.Cursor = Cursors.Hand;
                btnClose.Click += (s, e) => this.Close();
                topPanel.Controls.Add(btnClose);

                // CONTENT CONTAINER - holds form elements and buttons
                Panel contentPanel = new Panel();
                contentPanel.Dock = DockStyle.Fill;
                contentPanel.BackColor = Color.White;
                contentPanel.Padding = new Padding(20, 40, 20, 20);
                mainContainer.Controls.Add(contentPanel);

                // Main table layout for form fields
                TableLayoutPanel tableLayout = new TableLayoutPanel();
                tableLayout.Dock = DockStyle.Fill;
                tableLayout.ColumnCount = 2;
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
                tableLayout.Padding = new Padding(10);
                tableLayout.AutoScroll = true;
                contentPanel.Controls.Add(tableLayout);

                // Row definitions
                tableLayout.RowCount = 3;
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Category name row
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Sizes label row
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Sizes list row

                // Category Name
                tableLayout.Controls.Add(new Label()
                {
                    Text = "Category Name:",
                    Anchor = AnchorStyles.Left,
                    AutoSize = true,
                    ForeColor = Color.Black
                }, 0, 0);

                txtName = new TextBox();
                txtName.Dock = DockStyle.Fill;
                txtName.BorderStyle = BorderStyle.FixedSingle;
                txtName.Text = _category.categoryName;
                tableLayout.Controls.Add(txtName, 1, 0);

                // Sizes Label
                tableLayout.Controls.Add(new Label()
                {
                    Text = "Applicable Sizes:",
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    AutoSize = true,
                    ForeColor = Color.Black,
                    Margin = new Padding(0, 8, 0, 0)
                }, 0, 1);

                // Sizes CheckedListBox - spans both columns
                Panel sizesContainer = new Panel();
                sizesContainer.Dock = DockStyle.Fill;
                sizesContainer.BackColor = Color.White;
                tableLayout.SetColumnSpan(sizesContainer, 2);
                tableLayout.SetRow(sizesContainer, 2);

                clbSizes = new CheckedListBox();
                clbSizes.Dock = DockStyle.Fill;
                clbSizes.BorderStyle = BorderStyle.FixedSingle;
                clbSizes.CheckOnClick = true;
                clbSizes.BackColor = Color.White;
                clbSizes.Margin = new Padding(10);
                sizesContainer.Controls.Add(clbSizes);
                tableLayout.Controls.Add(sizesContainer, 0, 2);

                // BUTTON PANEL (at bottom of content area)
                Panel buttonPanel = new Panel();
                buttonPanel.Dock = DockStyle.Bottom;
                buttonPanel.Height = 60;
                buttonPanel.BackColor = Color.White;
                contentPanel.Controls.Add(buttonPanel);

                FlowLayoutPanel buttonFlow = new FlowLayoutPanel();
                buttonFlow.FlowDirection = FlowDirection.RightToLeft;
                buttonFlow.Dock = DockStyle.Fill;
                buttonFlow.Padding = new Padding(0, 10, 0, 0);
                buttonPanel.Controls.Add(buttonFlow);

                // Cancel button
                btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.Size = new Size(100, 40);
                btnCancel.BackColor = ButtonGray;
                btnCancel.FlatStyle = FlatStyle.Flat;
                btnCancel.FlatAppearance.BorderSize = 0;
                btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnCancel.ForeColor = Color.White;
                btnCancel.Cursor = Cursors.Hand;
                btnCancel.Margin = new Padding(0);
                btnCancel.Click += (s, e) => this.Close();
                buttonFlow.Controls.Add(btnCancel);

                // Save button
                btnSave = new Button();
                btnSave.Text = "Save";
                btnSave.Size = new Size(100, 40);
                btnSave.BackColor = PrimaryColor;
                btnSave.FlatStyle = FlatStyle.Flat;
                btnSave.FlatAppearance.BorderSize = 0;
                btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnSave.ForeColor = Color.White;
                btnSave.Margin = new Padding(0, 0, 10, 0);
                btnSave.Cursor = Cursors.Hand;
                btnSave.Click += (s, e) => SaveCategory();
                buttonFlow.Controls.Add(btnSave);

                // Add button hover effects
                btnSave.MouseEnter += (s, e) => btnSave.BackColor = ButtonHoverColor;
                btnSave.MouseLeave += (s, e) => btnSave.BackColor = PrimaryColor;
                btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.Gray;
                btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = ButtonGray;
                btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(200, 50, 50);
                btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.Transparent;

                this.ResumeLayout(true);
            }

            private void SaveCategory()
            {
                if (ValidateInput())
                {
                    try
                    {
                        bool success;
                        int categoryId = _category.Category_ID;

                        if (categoryId > 0)
                        {
                            success = _categoryService.UpdateCategory(categoryId, txtName.Text);
                        }
                        else
                        {
                            success = _categoryService.AddCategory(txtName.Text);
                            categoryId = _categoryService.GetCategoryIdByName(txtName.Text);
                        }

                        if (success && categoryId > 0)
                        {
                            UpdateCategorySizes(categoryId);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            ShowThemedMessage("Error saving category");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowThemedMessage($"Error: {ex.Message}");
                    }
                }
            }

            private bool ValidateInput()
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    ShowThemedMessage("Category name cannot be empty");
                    return false;
                }

                if (_categoryService.CheckCategoryExists(txtName.Text,
                    _category.Category_ID > 0 ? _category.Category_ID : (int?)null))
                {
                    ShowThemedMessage("Category name already exists");
                    return false;
                }

                return true;
            }

            private void UpdateCategorySizes(int categoryId)
            {
                var selectedSizeIds = new List<int>();

                for (int i = 0; i < clbSizes.Items.Count; i++)
                {
                    var item = (SizeItem)clbSizes.Items[i];
                    // Skip headers and unchecked items
                    if (item.Size_ID == null || !clbSizes.GetItemChecked(i))
                        continue;

                    selectedSizeIds.Add(item.Size_ID.Value);
                }

                if (!_categoryService.UpdateCategorySizes(categoryId, selectedSizeIds))
                {
                    ShowThemedMessage("Error updating sizes for category");
                }
            }

            private void ShowThemedMessage(string message)
            {
                MessageBox.Show(this, message, "Category",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Enable form dragging
            private bool _dragging;
            private Point _startPoint = new Point(0, 0);

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                if (e.Button == MouseButtons.Left)
                {
                    _dragging = true;
                    _startPoint = new Point(e.X, e.Y);
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (_dragging)
                {
                    Point p = PointToScreen(e.Location);
                    Location = new Point(p.X - this._startPoint.X, p.Y - this._startPoint.Y);
                }
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                _dragging = false;
            }
        }
        public partial class BarcodePrint : Form
        {
            // Theme colors
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
            private static readonly Color ForegroundColor = Color.Black;
            private static readonly Color SecondaryColor = Color.Gray;
            private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

            // UI Controls
            private DataGridView dgvItems;
            private Button btnPreview;
            private Button btnPrint;
            private ComboBox cboPrinters;
            private ComboBox cboTemplates;
            private Label lblPrinter;
            private Label lblTemplate;
            private NumericUpDown nudPrintCount;
            private PictureBox picPreview;
            private TextBox txtSearch;
            private ComboBox cboBrand;
            private ComboBox cboCategory;
            private Button btnSearch;
            private Button btnClear;
            private TextBox txtBarcode;
            private TextBox txtPrice;
            private TextBox txtBrand;
            private TextBox txtCategory;
            private TextBox txtSize;
            private TextBox txtQuantity;

            // Data
            private List<Item> items = new List<Item>();
            private Item selectedItem;
            private List<TemplateItem> templateItems = new List<TemplateItem>();

            // State
            private volatile bool _isClosing = false;

            public BarcodePrint()
            {
                InitializeForm();
                LoadTemplates();
                LoadPrinters();
                LoadBrands();
                LoadCategories();
                LoadItems();

                this.FormClosing += (s, e) => _isClosing = true;
            }

            private void InitializeForm()
            {
                // Form setup
                this.Text = "BARCODE LABEL PRINTING";
                this.Size = new Size(980, 700);
                this.FormBorderStyle = FormBorderStyle.None;
                this.Dock = DockStyle.Fill;
                this.BackColor = BackgroundColor;

                // Main container
                var container = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor,
                    Padding = new Padding(20)
                };

                // Title panel
                var titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor
                };

                var lblTitle = new Label
                {
                    Text = "BARCODE LABEL PRINTING",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.White,
                    Padding = new Padding(20, 0, 0, 0)
                };
                titlePanel.Controls.Add(lblTitle);

                // Search panel
                var searchPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 50,
                    BackColor = HeaderColor,
                    Padding = new Padding(10)
                };

                // Build search controls
                txtSearch = new TextBox
                {
                    PlaceholderText = "Search by description or barcode...",
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10),
                    Margin = new Padding(0, 0, 10, 0)
                };
                txtSearch.KeyPress += (s, e) =>
                {
                    if (e.KeyChar == (char)Keys.Enter) PerformSearch();
                };

                cboBrand = new ComboBox
                {
                    Dock = DockStyle.Right,
                    Width = 150,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10),
                    Margin = new Padding(0, 0, 10, 0)
                };

                cboCategory = new ComboBox
                {
                    Dock = DockStyle.Right,
                    Width = 150,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10),
                    Margin = new Padding(0, 0, 10, 0)
                };

                btnSearch = CreateButton("SEARCH", PrimaryColor, 90, 30);
                btnSearch.Click += (s, e) => PerformSearch();

                btnClear = CreateButton("CLEAR", SecondaryColor, 80, 30);
                btnClear.Click += (s, e) => ClearSearch();

                // Search container layout
                var searchContainer = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 6,
                    RowCount = 1,
                    Padding = new Padding(0)
                };
                searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Search box
                searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Category
                searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Brand
                searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));  // Clear
                searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90)); // Search
                searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));  // Spacer

                searchContainer.Controls.Add(txtSearch, 0, 0);
                searchContainer.Controls.Add(cboCategory, 1, 0);
                searchContainer.Controls.Add(cboBrand, 2, 0);
                searchContainer.Controls.Add(btnClear, 3, 0);
                searchContainer.Controls.Add(btnSearch, 4, 0);
                searchPanel.Controls.Add(searchContainer);

                // Main content panel
                var contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor
                };

                // Left panel - DataGridView
                var leftPanel = new Panel
                {
                    Dock = DockStyle.Left,
                    Width = 500,
                    Padding = new Padding(0, 0, 20, 0)
                };

                dgvItems = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = BackgroundColor,
                    BorderStyle = BorderStyle.Fixed3D,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    RowHeadersVisible = false,
                    AllowUserToAddRows = false,
                    ReadOnly = true,
                    EnableHeadersVisualStyles = false
                };

                // Grid styling
                dgvItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(0)
                };

                dgvItems.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor
                };

                dgvItems.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                // Right panel - Details and controls
                var rightPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(20, 0, 0, 0)
                };

                // Details section
                var detailsGroup = new GroupBox
                {
                    Text = "ITEM DETAILS",
                    Dock = DockStyle.Top,
                    Height = 190,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = PrimaryColor
                };

                var detailsLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 6,
                    Padding = new Padding(10, 0, 10, 10)
                };

                // Create detail labels and text boxes
                detailsLayout.Controls.Add(CreateDetailLabel("Barcode:"), 0, 0);
                txtBarcode = CreateDetailTextBox();
                detailsLayout.Controls.Add(txtBarcode, 1, 0);

                detailsLayout.Controls.Add(CreateDetailLabel("Price:"), 0, 1);
                txtPrice = CreateDetailTextBox();
                detailsLayout.Controls.Add(txtPrice, 1, 1);

                detailsLayout.Controls.Add(CreateDetailLabel("Brand:"), 0, 2);
                txtBrand = CreateDetailTextBox();
                detailsLayout.Controls.Add(txtBrand, 1, 2);

                detailsLayout.Controls.Add(CreateDetailLabel("Category:"), 0, 3);
                txtCategory = CreateDetailTextBox();
                detailsLayout.Controls.Add(txtCategory, 1, 3);

                detailsLayout.Controls.Add(CreateDetailLabel("Size:"), 0, 4);
                txtSize = CreateDetailTextBox();
                detailsLayout.Controls.Add(txtSize, 1, 4);

                detailsLayout.Controls.Add(CreateDetailLabel("Quantity:"), 0, 5);
                txtQuantity = CreateDetailTextBox();
                detailsLayout.Controls.Add(txtQuantity, 1, 5);

                detailsGroup.Controls.Add(detailsLayout);

                // Preview section
                var previewGroup = new GroupBox
                {
                    Text = "LABEL PREVIEW",
                    Dock = DockStyle.Top,
                    Height = 160,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = PrimaryColor
                };

                var previewPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10),
                    BackColor = Color.White
                };

                picPreview = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                previewPanel.Controls.Add(picPreview);
                previewGroup.Controls.Add(previewPanel);

                // Controls section
                var controlsPanel = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 4,
                    ColumnCount = 3,
                    AutoSize = true,
                    Padding = new Padding(0, 10, 0, 0)
                };
                controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
                controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32)); // Template
                controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32)); // Printer
                controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32)); // Print Count
                controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Buttons

                // Template selection
                lblTemplate = new Label
                {
                    Text = "Template:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                cboTemplates = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10)
                };
                cboTemplates.SelectedIndexChanged += (s, e) => RegeneratePreview();

                // Printer selection
                lblPrinter = new Label
                {
                    Text = "Printer:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                cboPrinters = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10)
                };

                // Print count
                var lblPrintCount = new Label
                {
                    Text = "Print Count:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                nudPrintCount = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = 1000,
                    Value = 1,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10)
                };

                // Create Preview and Print buttons
                btnPreview = CreateButton("PREVIEW", Color.SteelBlue, 140, 35);
                btnPreview.Enabled = false;
                btnPreview.Click += BtnPreview_Click;

                btnPrint = CreateButton("PRINT LABELS", PrimaryColor, 140, 35);
                btnPrint.Enabled = false;
                btnPrint.Click += BtnPrint_Click;

                // Layout controls
                controlsPanel.Controls.Add(lblTemplate, 0, 0);
                controlsPanel.Controls.Add(cboTemplates, 1, 0);
                controlsPanel.SetColumnSpan(cboTemplates, 2);

                controlsPanel.Controls.Add(lblPrinter, 0, 1);
                controlsPanel.Controls.Add(cboPrinters, 1, 1);
                controlsPanel.SetColumnSpan(cboPrinters, 2);

                controlsPanel.Controls.Add(lblPrintCount, 0, 2);
                controlsPanel.Controls.Add(nudPrintCount, 1, 2);
                controlsPanel.SetColumnSpan(nudPrintCount, 2);

                controlsPanel.Controls.Add(btnPreview, 1, 3);
                controlsPanel.Controls.Add(btnPrint, 2, 3);

                // Events
                dgvItems.SelectionChanged += DgvItems_SelectionChanged;

                // Build panels
                leftPanel.Controls.Add(dgvItems);

                rightPanel.Controls.Add(controlsPanel);
                rightPanel.Controls.Add(previewGroup);
                rightPanel.Controls.Add(detailsGroup);

                titlePanel.Controls.Add(lblTitle);

                contentPanel.Controls.Add(rightPanel);
                contentPanel.Controls.Add(leftPanel);

                container.Controls.Add(contentPanel);
                container.Controls.Add(searchPanel);
                container.Controls.Add(titlePanel);

                this.Controls.Add(container);
                ConfigureGridColumns();
            }

            private Label CreateDetailLabel(string text) => new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10)
            };

            private TextBox CreateDetailTextBox() => new TextBox
            {
                ReadOnly = true,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = HeaderColor,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(3, 5, 3, 5)
            };

            private Button CreateButton(string text, Color backColor, int width, int height)
            {
                return new Button
                {
                    Text = text,
                    Size = new Size(width, height),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    BackColor = backColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Margin = new Padding(5, 0, 5, 0),
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
            }

            private void ConfigureGridColumns()
            {
                dgvItems.Columns.Clear();

                dgvItems.Columns.AddRange(
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "ProductSize_ID",
                        HeaderText = "ID",
                        Width = 40,
                        Name = "Item_ID"
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Barcode",
                        HeaderText = "BARCODE",
                        Width = 120
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Description",
                        HeaderText = "DESCRIPTION",
                        Width = 200
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "RetailPrice",
                        HeaderText = "PRICE",
                        Width = 80,
                        DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "BrandName",
                        HeaderText = "BRAND",
                        Width = 100
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "CategoryName",
                        HeaderText = "CATEGORY",
                        Width = 100
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "SizeLabel",
                        HeaderText = "SIZE",
                        Width = 70
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Quantity",
                        HeaderText = "QTY",
                        Width = 60,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleRight
                        }
                    }
                );
            }

            private void LoadTemplates()
            {
                try
                {
                    string templateDir = Path.Combine(System.Windows.Forms.Application.StartupPath, "Barcode");

                    if (!Directory.Exists(templateDir))
                    {
                        MessageBox.Show($"Template directory not found:\n{templateDir}",
                                        "Missing Templates", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var templateFiles = Directory.GetFiles(templateDir, "*.btw");

                    if (templateFiles.Length == 0)
                    {
                        MessageBox.Show("No template files found in the template directory",
                                        "Missing Templates", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    templateItems.Clear();
                    cboTemplates.Items.Clear();

                    foreach (string file in templateFiles)
                    {
                        string fileName = Path.GetFileName(file);
                        int labelsPerRow = GetLabelsPerRowFromFileName(fileName);

                        templateItems.Add(new TemplateItem
                        {
                            FileName = fileName,
                            FilePath = file,
                            LabelsPerRow = labelsPerRow
                        });
                    }

                    // Sort templates alphabetically
                    templateItems = templateItems.OrderBy(t => t.FileName).ToList();

                    foreach (var template in templateItems)
                    {
                        cboTemplates.Items.Add(template);
                    }

                    if (cboTemplates.Items.Count > 0)
                    {
                        cboTemplates.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading templates: {ex.Message}", "Template Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private int GetLabelsPerRowFromFileName(string fileName)
            {
                // Default to 1 label per row
                int labelsPerRow = 1;

                if (fileName.Contains("2up")) return 2;
                if (fileName.Contains("3up")) return 3;
                if (fileName.Contains("4up")) return 4;

                return labelsPerRow;
            }

            private void RegeneratePreview()
            {
                if (selectedItem != null && btnPreview.Enabled)
                {
                    GenerateBarTenderPreview();
                }
            }

            private void LoadPrinters()
            {
                try
                {
                    var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
                    cboPrinters.DataSource = printers;

                    // Set default printer if available
                    using (PrintDocument doc = new PrintDocument())
                    {
                        if (printers.Contains(doc.PrinterSettings.PrinterName))
                        {
                            cboPrinters.SelectedItem = doc.PrinterSettings.PrinterName;
                        }
                        else if (cboPrinters.Items.Count > 0)
                        {
                            cboPrinters.SelectedIndex = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading printers: {ex.Message}", "Printer Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void LoadBrands()
            {
                try
                {
                    var brandService = new BrandService();
                    var brands = brandService.GetAllBrands();

                    cboBrand.Items.Clear();
                    cboBrand.Items.Add(new ComboBoxItem("All Brands", 0));

                    foreach (var brand in brands)
                    {
                        cboBrand.Items.Add(new ComboBoxItem(brand.brandName, brand.Brand_ID));
                    }
                    cboBrand.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading brands: {ex.Message}", "Data Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void LoadCategories()
            {
                try
                {
                    var categoryService = new CategoryService();
                    var categories = categoryService.GetAllCategorie();

                    cboCategory.Items.Clear();
                    cboCategory.Items.Add(new ComboBoxItem("All Categories", 0));

                    if (categories != null)
                    {
                        foreach (var category in categories)
                        {
                            if (category != null)
                            {
                                cboCategory.Items.Add(new ComboBoxItem(
                                    category.categoryName ?? "Unnamed Category",
                                    category.Category_ID
                                ));
                            }
                        }
                    }
                    cboCategory.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading categories: {ex.Message}", "Data Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void LoadItems(string searchTerm = "", int brandId = 0, int categoryId = 0)
            {
                try
                {
                    var repository = new ItemRepository();
                    items = repository.SearchItemsWithVariants(searchTerm, brandId, categoryId);

                    dgvItems.Rows.Clear();
                    foreach (var item in items)
                    {
                        dgvItems.Rows.Add(
                            item.ProductSize_ID,
                            item.Barcode,
                            item.Description,
                            item.RetailPrice,
                            item.BrandName,
                            item.CategoryName,
                            item.SizeLabel ?? "N/A",
                            item.Quantity
                        );
                    }

                    // Clear selection
                    dgvItems.ClearSelection();
                    selectedItem = null;
                    ClearDetails();
                    btnPrint.Enabled = false;
                    btnPreview.Enabled = false;
                    ClearPreviewImage();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading items: {ex.Message}", "Data Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void ClearDetails()
            {
                txtBarcode.Clear();
                txtPrice.Clear();
                txtBrand.Clear();
                txtCategory.Clear();
                txtSize.Clear();
                txtQuantity.Clear();
            }

            private void ClearPreviewImage()
            {
                if (picPreview.InvokeRequired)
                {
                    picPreview.Invoke((MethodInvoker)delegate
                    {
                        if (!picPreview.IsDisposed)
                        {
                            picPreview.Image?.Dispose();
                            picPreview.Image = null;
                        }
                    });
                }
                else
                {
                    if (!picPreview.IsDisposed)
                    {
                        picPreview.Image?.Dispose();
                        picPreview.Image = null;
                    }
                }
            }

            private void PerformSearch()
            {
                string searchTerm = txtSearch.Text.Trim();
                int brandId = (cboBrand.SelectedItem as ComboBoxItem)?.Value ?? 0;
                int categoryId = (cboCategory.SelectedItem as ComboBoxItem)?.Value ?? 0;

                LoadItems(searchTerm, brandId, categoryId);
            }

            private void ClearSearch()
            {
                txtSearch.Clear();
                cboBrand.SelectedIndex = 0;
                cboCategory.SelectedIndex = 0;
                LoadItems();
            }

            private void DgvItems_SelectionChanged(object sender, EventArgs e)
            {
                if (dgvItems.SelectedRows.Count > 0)
                {
                    int selectedId = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["Item_ID"].Value);
                    selectedItem = items.FirstOrDefault(i => i.ProductSize_ID == selectedId);

                    if (selectedItem != null)
                    {
                        // Populate details
                        txtBarcode.Text = selectedItem.Barcode;
                        txtPrice.Text = selectedItem.RetailPrice.ToString("C2");
                        txtBrand.Text = selectedItem.BrandName;
                        txtCategory.Text = selectedItem.CategoryName;
                        txtSize.Text = selectedItem.SizeLabel ?? "N/A";
                        txtQuantity.Text = selectedItem.Quantity.ToString();

                        // Set reasonable print count
                        nudPrintCount.Value = Math.Max(1, Math.Min(selectedItem.Quantity, 100));

                        // Enable printing and preview
                        btnPrint.Enabled = true;
                        btnPreview.Enabled = true;

                        // Generate preview immediately
                        GenerateBarTenderPreview();
                    }
                }
                else
                {
                    btnPrint.Enabled = false;
                    btnPreview.Enabled = false;
                    ClearDetails();
                    ClearPreviewImage();
                }
            }

            private void GenerateBarTenderPreview()
            {
                // Capture current item to avoid race conditions
                Item currentItem = selectedItem;

                if (cboTemplates.SelectedItem == null)
                {
                    return;
                }

                TemplateItem template = (TemplateItem)cboTemplates.SelectedItem;

                if (currentItem == null || !File.Exists(template.FilePath))
                {
                    ClearPreviewImage();
                    return;
                }

                Thread staThread = new Thread(() =>
                {
                    // Check if form is closing before processing
                    if (_isClosing) return;

                    string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempDir);

                    string outputFile = null;
                    BTApplication btApp = null;
                    BTFormat btFormat = null;

                    try
                    {
                        // Double-check if form is closing
                        if (_isClosing) return;

                        btApp = new BTApplication();
                        btApp.Visible = false;

                        btFormat = btApp.Formats.Open(template.FilePath, false, "");

                        // Use captured currentItem reference
                        SetFormatData(btFormat, currentItem);

                        // Set labels per row
                        btFormat.PrintSetup.NumberSerializedLabels = template.LabelsPerRow;

                        string fileNameTemplate = "preview_" + Guid.NewGuid().ToString("N") + ".png";
                        ExportLabelToImage(btFormat, tempDir, fileNameTemplate, out outputFile);

                        if (!string.IsNullOrEmpty(outputFile) && File.Exists(outputFile))
                        {
                            using (FileStream fs = new FileStream(outputFile, FileMode.Open, FileAccess.Read))
                            {
                                Image previewImage = Image.FromStream(fs);

                                this.Invoke((MethodInvoker)delegate
                                {
                                    // Only update if the current item is still selected
                                    if (!_isClosing && !picPreview.IsDisposed && selectedItem == currentItem)
                                    {
                                        picPreview.Image?.Dispose();
                                        picPreview.Image = new Bitmap(previewImage);
                                    }
                                    else
                                    {
                                        previewImage.Dispose();
                                    }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!_isClosing)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show($"Preview Error: {ex.Message}");
                            });
                        }
                    }
                    finally
                    {
                        CleanupBarTender(ref btFormat, ref btApp);

                        try
                        {
                            if (Directory.Exists(tempDir))
                                Directory.Delete(tempDir, true);
                        }
                        catch { }
                    }
                });

                staThread.SetApartmentState(ApartmentState.STA);
                staThread.IsBackground = true;
                staThread.Start();
            }

            private void BtnPreview_Click(object sender, EventArgs e)
            {
                if (selectedItem == null) return;
                GenerateBarTenderPreview();
            }

            private void BtnPrint_Click(object sender, EventArgs e)
            {
                if (selectedItem == null)
                {
                    MessageBox.Show("Please select an item first", "No Selection",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboTemplates.SelectedItem == null)
                {
                    MessageBox.Show("Please select a template", "Template Required",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string printerName = cboPrinters.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(printerName))
                {
                    MessageBox.Show("Please select a printer", "Printer Required",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Capture UI values on the main thread before starting background thread
                TemplateItem template = (TemplateItem)cboTemplates.SelectedItem;
                int copies = (int)nudPrintCount.Value;
                Item itemToPrint = selectedItem;  // Capture current item

                Thread staThread = new Thread(() => PrintWithBarTender(template, printerName, copies, itemToPrint));
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.IsBackground = true;
                staThread.Start();
            }

            private void PrintWithBarTender(TemplateItem template, string printerName, int copies, Item item)
            {
                if (!File.Exists(template.FilePath))
                {
                    this.Invoke((MethodInvoker)delegate {
                        MessageBox.Show($"Template file not found:\n{template.FilePath}",
                                        "Template Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                    return;
                }

                BTApplication btApp = null;
                BTFormat btFormat = null;

                try
                {
                    btApp = new BTApplication();
                    btApp.Visible = false;
                    btApp.Save(true);  // Prevent save prompts

                    btFormat = btApp.Formats.Open(template.FilePath, true, "");
                    SetFormatData(btFormat, item);  // Use captured item

                    // Configure printer
                    btFormat.PrintSetup.Printer = printerName;
                    btFormat.PrintSetup.IdenticalCopiesOfLabel = copies;
                    btFormat.PrintSetup.NumberSerializedLabels = template.LabelsPerRow;

                    // Print
                    btFormat.PrintOut(false, true);  // (ShowStatusWindow, WaitUntilFinished)

                    this.Invoke((MethodInvoker)delegate {
                        MessageBox.Show($"{copies} label(s) sent to printer", "Print Successful",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        MessageBox.Show($"Print Error: {ex.Message}\n\n{ex.StackTrace}",
                                        "Print Failure",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    });
                }
                finally
                {
                    CleanupBarTender(ref btFormat, ref btApp);
                }
            }

            private void SetFormatData(BTFormat format, Item item)
            {
                if (item == null) return;

                try
                {
                    format.SetNamedSubStringValue("Barcode", item.Barcode ?? "");
                    format.SetNamedSubStringValue("RetailPrice", $"Rs.{item.RetailPrice.ToString("N2")}");
                    format.SetNamedSubStringValue("Category", item.CategoryName ?? "");
                    format.SetNamedSubStringValue("Size", $"({ItemSize(item.SizeLabel ?? "N/A")})");
                    format.SetNamedSubStringValue("Sex", $"({GetGenderCode(item.Gender_ID)})");
                    format.SetNamedSubStringValue("CostCode", $"{ConvertNumberToCode(Convert.ToInt32(item.UnitCost).ToString())}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error setting template data: {ex.Message}");
                }
            }

            public static string ConvertNumberToCode(string number)
            {
                if (string.IsNullOrEmpty(number))
                    return "XXXX";

                StringBuilder result = new StringBuilder();
                foreach (char digit in number)
                {
                    switch (digit)
                    {
                        case '0': result.Append('X'); break;
                        case '1': result.Append('A'); break;
                        case '2': result.Append('B'); break;
                        case '3': result.Append('C'); break;
                        case '4': result.Append('D'); break;
                        case '5': result.Append('E'); break;
                        case '6': result.Append('F'); break;
                        case '7': result.Append('G'); break;
                        case '8': result.Append('H'); break;
                        case '9': result.Append('I'); break;
                        default: result.Append('?'); break;
                    }
                }
                return result.ToString();
            }
            private string ItemSize(string size)
            {
                return size switch
                {
                    "One Size" => "One",  // One Size
                    "XXXL" => "3XL",
                    "XXXXl" => "4XL",
                    _ => size,  // Other Any Sizes
                };
            }

            private string GetGenderCode(int genderId)
            {
                return genderId switch
                {
                    1 => "M",  // Male
                    2 => "F",  // Female
                    3 => "U",  // Unisex
                    4 => "N",  // None
                    _ => "N"   // Default to None
                };
            }

            private void ExportLabelToImage(BTFormat format, string directory,
                                           string fileNameTemplate, out string outputFilePath)
            {
                outputFilePath = null;

                try
                {
                    BarTender.Messages messages;
                    _ = format.ExportPrintPreviewToImage(
                        directory,
                        fileNameTemplate,
                        "PNG",
                        BtColors.btColors24Bit,
                        203,
                        0,
                        BtSaveOptions.btDoNotSaveChanges,
                        false,
                        true,
                        out messages
                    );

                    outputFilePath = FindGeneratedFile(directory, fileNameTemplate);
                }
                catch (Exception ex)
                {
                    throw new Exception("Export failed: " + ex.Message);
                }
            }

            private string FindGeneratedFile(string directory, string fileNameTemplate)
            {
                string searchPattern = Path.GetFileNameWithoutExtension(fileNameTemplate) + "*" +
                                       Path.GetExtension(fileNameTemplate);

                var files = Directory.GetFiles(directory, searchPattern);
                if (files.Length > 0) return files[0];

                string exactPath = Path.Combine(directory, fileNameTemplate);
                return File.Exists(exactPath) ? exactPath : null;
            }

            private void CleanupBarTender(ref BTFormat format, ref BTApplication app)
            {
                try
                {
                    if (format != null)
                    {
                        format.Close(BtSaveOptions.btDoNotSaveChanges);
                        while (Marshal.ReleaseComObject(format) > 0) { }
                        format = null;
                    }

                    if (app != null)
                    {
                        app.Quit(BtSaveOptions.btDoNotSaveChanges);
                        while (Marshal.ReleaseComObject(app) > 0) { }
                        app = null;
                    }
                }
                catch { }
                finally
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            // Helper classes
            private class ComboBoxItem
            {
                public string Text { get; }
                public int Value { get; }

                public ComboBoxItem(string text, int value)
                {
                    Text = text;
                    Value = value;
                }

                public override string ToString() => Text;
            }

            private class TemplateItem
            {
                public string FileName { get; set; }
                public string FilePath { get; set; }
                public int LabelsPerRow { get; set; }

                public override string ToString()
                {
                    return $"{FileName} ({LabelsPerRow} per row)";
                }
            }
        }
        public partial class Sales : Form
        {
            private readonly SalesService _salesService;
            private SalesReport _currentReport;

            // Theme colors
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
            private static readonly Color ForegroundColor = Color.Black;
            private static readonly Color SecondaryColor = Color.Gray;
            private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

            // Controls
            private DateTimePicker dtpStartDate;
            private DateTimePicker dtpEndDate;
            private ComboBox cmbBrand;
            private ComboBox cmbCategory;
            private Button btnFilter;
            private Button btnClear;
            private Button btnExport;
            private Label[] summaryLabels = new Label[10];
            private TabControl tabControl;
            private DataGridView dgvSalesItems;
            private DataGridView dgvReturnItems;
            private System.Windows.Forms.DataVisualization.Charting.Chart chartSalesTrend;

            public Sales()
            {
                InitializeComponent();
                _salesService = new SalesService(new SalesRepository());
                LoadBrands();
                LoadCategories();
                InitializeSummaryControls();
            }

            private void InitializeComponent()
            {
                // Form setup
                this.Text = "Sales Analytics Dashboard";
                this.Size = new Size(1200, 850);  // Increased height
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = BackgroundColor;
                this.Font = new Font("Segoe UI", 9);
                this.Padding = new Padding(20);

                // Header panel
                var headerPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor,
                    Padding = new Padding(20, 0, 20, 0)
                };

                var lblTitle = new Label
                {
                    Text = "SALES ANALYTICS DASHBOARD",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.White
                };
                headerPanel.Controls.Add(lblTitle);

                // Filter panel - increased height
                var filterPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 100,
                    BackColor = HeaderColor,
                    Padding = new Padding(15)
                };

                // Date controls
                var lblStartDate = new Label
                {
                    Text = "Start Date:",
                    Location = new Point(20, 15),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                dtpStartDate = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Location = new Point(105, 12),
                    Width = 120,
                    Value = DateTime.Today.AddMonths(-1)
                };

                var lblEndDate = new Label
                {
                    Text = "End Date:",
                    Location = new Point(240, 15),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                dtpEndDate = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Location = new Point(315, 12),
                    Width = 120,
                    Value = DateTime.Today
                };

                // Brand/Category filters
                var lblBrand = new Label
                {
                    Text = "Brand:",
                    Location = new Point(450, 15),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                cmbBrand = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 150,
                    Location = new Point(505, 12)
                };

                var lblCategory = new Label
                {
                    Text = "Category:",
                    Location = new Point(670, 15),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                cmbCategory = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 150,
                    Location = new Point(745, 12)
                };

                // Buttons - repositioned vertically
                btnFilter = CreateButton("Apply Filters", PrimaryColor, 450, 55);
                btnFilter.Click += BtnFilter_Click;

                btnClear = CreateButton("Clear Filters", SecondaryColor, 600, 55);
                btnClear.Click += BtnClear_Click;

                btnExport = CreateButton("Export Report", Color.DarkGreen, 750, 55);
                btnExport.Click += BtnExport_Click;

                // Add controls to filter panel
                filterPanel.Controls.AddRange(new Control[] {
                    lblStartDate, dtpStartDate, lblEndDate, dtpEndDate,
                    lblBrand, cmbBrand, lblCategory, cmbCategory,
                    btnFilter, btnClear, btnExport
                });

                // Summary panel - increased height for two rows
                var summaryPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 140,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(15),
                    Tag = "summary"
                };

                // Main content panel
                var mainPanel = new Panel { Dock = DockStyle.Fill };

                // Tab control
                tabControl = new TabControl
                {
                    Dock = DockStyle.Fill,
                    Appearance = TabAppearance.FlatButtons,
                    ItemSize = new Size(120, 30),
                    SizeMode = TabSizeMode.Fixed
                };

                // Sales items tab
                var tabSales = new TabPage("Sales Items")
                {
                    BackColor = BackgroundColor,
                    Padding = new Padding(5)
                };
                dgvSalesItems = CreateDataGridView();
                dgvSalesItems.Dock = DockStyle.Fill;
                dgvSalesItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                tabSales.Controls.Add(dgvSalesItems);

                // Return items tab
                var tabReturns = new TabPage("Return Items")
                {
                    BackColor = BackgroundColor,
                    Padding = new Padding(5)
                };
                dgvReturnItems = CreateDataGridView();
                dgvReturnItems.Dock = DockStyle.Fill;
                dgvReturnItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                tabReturns.Controls.Add(dgvReturnItems);

                // Charts tab
                var tabCharts = new TabPage("Visualizations")
                {
                    BackColor = BackgroundColor,
                    Padding = new Padding(5)
                };
                chartSalesTrend = new System.Windows.Forms.DataVisualization.Charting.Chart
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White
                };
                InitializeChart();
                tabCharts.Controls.Add(chartSalesTrend);

                tabControl.TabPages.AddRange(new TabPage[] { tabSales, tabReturns, tabCharts });
                mainPanel.Controls.Add(tabControl);

                // Add panels to form
                this.Controls.Add(mainPanel);
                this.Controls.Add(summaryPanel);
                this.Controls.Add(filterPanel);
                this.Controls.Add(headerPanel);
            }

            private void InitializeSummaryControls()
            {
                string[] labels = {
                    "Total Sales", "Total Cost", "Gross Profit", "Items Sold",
                    "Bills Processed", "Avg. Bill Value", "Cash Sales",
                    "Card Sales", "Bank Transfers", "Returns"
                };

                // Find summary panel
                var summaryPanel = this.Controls.OfType<Panel>()
                    .FirstOrDefault(p => p.Tag?.ToString() == "summary");

                if (summaryPanel == null) return;

                // First row (top 5 items)
                int x = 20;
                for (int i = 0; i < 5; i++)
                {
                    var lblCaption = new Label
                    {
                        Text = labels[i],
                        Location = new Point(x, 15),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = Color.Black
                    };

                    summaryLabels[i] = new Label
                    {
                        Text = "0.00",
                        Location = new Point(x, 40),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        ForeColor = PrimaryColor,
                        Tag = labels[i]
                    };

                    summaryPanel.Controls.Add(lblCaption);
                    summaryPanel.Controls.Add(summaryLabels[i]);
                    x += 190;  // Increased spacing for better visibility
                }

                // Second row (remaining 5 items)
                x = 20;
                for (int i = 5; i < 10; i++)
                {
                    var lblCaption = new Label
                    {
                        Text = labels[i],
                        Location = new Point(x, 80),  // Lower position
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        ForeColor = Color.Black
                    };

                    summaryLabels[i] = new Label
                    {
                        Text = "0.00",
                        Location = new Point(x, 105),  // Lower position
                        AutoSize = true,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        ForeColor = PrimaryColor,
                        Tag = labels[i]
                    };

                    summaryPanel.Controls.Add(lblCaption);
                    summaryPanel.Controls.Add(summaryLabels[i]);
                    x += 190;  // Increased spacing for better visibility
                }
            }

            private void InitializeChart()
            {
                chartSalesTrend.Series.Add("Sales");
                chartSalesTrend.Series["Sales"].ChartType = SeriesChartType.Column;
                chartSalesTrend.Series["Sales"].Color = PrimaryColor;

                chartSalesTrend.Series.Add("Returns");
                chartSalesTrend.Series["Returns"].ChartType = SeriesChartType.Column;
                chartSalesTrend.Series["Returns"].Color = Color.IndianRed;

                var chartArea = new ChartArea("MainArea");
                chartArea.AxisX.MajorGrid.Enabled = false;
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
                chartSalesTrend.ChartAreas.Add(chartArea);
            }

            private Button CreateButton(string text, Color color, int x, int y)
            {
                return new Button
                {
                    Text = text,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = color,
                    ForeColor = Color.White,
                    Size = new Size(120, 34),
                    Location = new Point(x, y),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                };
            }

            private DataGridView CreateDataGridView()
            {
                var dgv = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = BackgroundColor,
                    BorderStyle = BorderStyle.None,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    RowHeadersVisible = false,
                    AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(245, 249, 255)
                    },
                    ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = PrimaryColor,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        Padding = new Padding(5),
                        Alignment = DataGridViewContentAlignment.MiddleLeft
                    }
                };
                return dgv;
            }

            private void LoadBrands()
            {
                try
                {
                    cmbBrand.DataSource = _salesService.GetBrands();
                    cmbBrand.DisplayMember = "brandName";
                    cmbBrand.ValueMember = "Brand_ID";
                    cmbBrand.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading brands: {ex.Message}");
                }
            }

            private void LoadCategories()
            {
                try
                {
                    cmbCategory.DataSource = _salesService.GetCategories();
                    cmbCategory.DisplayMember = "categoryName";
                    cmbCategory.ValueMember = "Category_ID";
                    cmbCategory.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading categories: {ex.Message}");
                }
            }

            private void BtnFilter_Click(object sender, EventArgs e)
            {
                try
                {
                    var filter = new SalesFilter
                    {
                        StartDate = dtpStartDate.Value.Date,
                        EndDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1),
                        BrandId = cmbBrand.SelectedValue as int?,
                        CategoryId = cmbCategory.SelectedValue as int?
                    };

                    _currentReport = _salesService.GetSalesReport(filter);
                    UpdateUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void UpdateUI()
            {
                // Update summary labels
                summaryLabels[0].Text = _currentReport.TotalSales.ToString("N2");
                summaryLabels[1].Text = _currentReport.TotalCost.ToString("N2");
                summaryLabels[2].Text = (_currentReport.TotalSales - _currentReport.TotalCost).ToString("N2");
                summaryLabels[3].Text = _currentReport.TotalItemsSold.ToString("N0");
                summaryLabels[4].Text = _currentReport.BillCount.ToString("N0");

                decimal avgBillValue = _currentReport.BillCount > 0 ?
                    _currentReport.TotalSales / _currentReport.BillCount : 0;
                summaryLabels[5].Text = avgBillValue.ToString("N2");

                summaryLabels[6].Text = _currentReport.CashSales.ToString("N2");
                summaryLabels[7].Text = _currentReport.CardSales.ToString("N2");
                summaryLabels[8].Text = _currentReport.BankTransferSales.ToString("N2");
                summaryLabels[9].Text = _currentReport.ReturnCount.ToString("N0");

                // Bind data grids
                dgvSalesItems.DataSource = _currentReport.SalesItems;
                FormatDataGridColumns(dgvSalesItems, true);

                dgvReturnItems.DataSource = _currentReport.ReturnItems;
                FormatDataGridColumns(dgvReturnItems, false);

                // Update chart
                UpdateSalesChart();
            }

            private void FormatDataGridColumns(DataGridView dgv, bool isSales)
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    // Format currency columns
                    if (col.Name.Contains("Price") || col.Name.Contains("Amount") ||
                        col.Name.Contains("Value") || col.Name.Contains("Discount"))
                    {
                        col.DefaultCellStyle.Format = "N2";
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }

                    // Format date columns
                    if (col.Name.Contains("Date"))
                    {
                        col.DefaultCellStyle.Format = "d";
                    }

                    // Set header text
                    col.HeaderText = col.HeaderText.Replace("_", " ");
                }

                // Auto-size after data bind
                dgv.AutoResizeColumns();
            }

            private void UpdateSalesChart()
            {
                if (_currentReport?.SalesItems == null) return;

                chartSalesTrend.Series["Sales"].Points.Clear();
                chartSalesTrend.Series["Returns"].Points.Clear();

                // Group sales by date
                var salesByDate = _currentReport.SalesItems
                    .GroupBy(s => s.SaleDate.Date)
                    .Select(g => new {
                        Date = g.Key,
                        Sales = g.Sum(i => i.NetAmount),
                        Returns = _currentReport.ReturnItems
                            .Where(r => r.ReturnDate.Date == g.Key)
                            .Sum(r => r.RefundValue)
                    })
                    .OrderBy(d => d.Date)
                    .ToList();

                foreach (var day in salesByDate)
                {
                    chartSalesTrend.Series["Sales"].Points.AddXY(day.Date.ToString("MMM dd"), day.Sales);
                    chartSalesTrend.Series["Returns"].Points.AddXY(day.Date.ToString("MMM dd"), day.Returns);
                }

                chartSalesTrend.ChartAreas["MainArea"].RecalculateAxesScale();
            }

            private void BtnClear_Click(object sender, EventArgs e)
            {
                dtpStartDate.Value = DateTime.Today.AddMonths(-1);
                dtpEndDate.Value = DateTime.Today;
                cmbBrand.SelectedIndex = -1;
                cmbCategory.SelectedIndex = -1;

                foreach (var label in summaryLabels)
                {
                    label.Text = "0.00";
                }

                dgvSalesItems.DataSource = null;
                dgvReturnItems.DataSource = null;
                chartSalesTrend.Series["Sales"].Points.Clear();
                chartSalesTrend.Series["Returns"].Points.Clear();
            }

            private void BtnExport_Click(object sender, EventArgs e)
            {
                if (_currentReport == null)
                {
                    MessageBox.Show("No data to export. Please generate a report first.");
                    return;
                }

                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel Files|*.xlsx|CSV Files|*.csv";
                    sfd.Title = "Export Sales Report";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            ExportReport(sfd.FileName);
                            MessageBox.Show("Report exported successfully!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Export failed: {ex.Message}");
                        }
                    }
                }
            }

            private void ExportReport(string filePath)
            {
                // This would be implemented with EPPlus or similar library
                MessageBox.Show($"Export functionality would save to: {filePath}\n" +
                    "Implementation requires EPPlus or CSV library");
            }
        }
        public partial class Bills : Form
        {
            // Theme colors matching ItemsManagement and Sales
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
            private static readonly Color ForegroundColor = Color.Black;
            private static readonly Color SecondaryColor = Color.Gray;
            private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

            // Form controls
            private DataGridView dgvBills;
            private TextBox txtBillNumber;
            private TextBox txtCustomerContact;
            private DateTimePicker dtpBillDate;
            private CheckBox chkUseDate;
            private Button btnSearch;
            private Button btnClear;

            public Bills()
            {
                InitializeComponent();
                this.Dock = DockStyle.Fill;
                this.FormBorderStyle = FormBorderStyle.None;
            }

            private void InitializeComponent()
            {
                // Form setup
                this.Size = new Size(1200, 750);
                this.Text = "Bill Search";
                this.BackColor = BackgroundColor;
                this.Padding = new Padding(20);

                // Main container
                var container = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor,
                    Padding = new Padding(0)
                };

                // Title panel
                var titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor
                };

                var lblTitle = new Label
                {
                    Text = "BILL SEARCH",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.White
                };
                titlePanel.Controls.Add(lblTitle);

                // Filter panel
                var filterPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 120,
                    BackColor = HeaderColor,
                    Padding = new Padding(20, 15, 20, 15)
                };

                // Filter controls in table layout
                var filterTable = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 6,
                    RowCount = 2,
                    AutoSize = true
                };

                // Configure columns
                filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
                filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
                filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
                filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));   // Date controls
                filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));     // Buttons

                // Row heights
                filterTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                filterTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

                // Bill Number
                var lblBillNumber = new Label
                {
                    Text = "Bill Number:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = ForegroundColor
                };

                txtBillNumber = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10),
                    Margin = new Padding(0, 10, 10, 0)
                };
                txtBillNumber.KeyDown += TextBox_KeyDown;

                // Customer Contact
                var lblCustomerContact = new Label
                {
                    Text = "Customer Contact:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = ForegroundColor
                };

                txtCustomerContact = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10),
                    Margin = new Padding(0, 10, 10, 0)
                };
                txtCustomerContact.KeyDown += TextBox_KeyDown;

                // Bill Date
                var lblBillDate = new Label
                {
                    Text = "Bill Date:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = ForegroundColor
                };

                var datePanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0, 5, 0, 5)
                };

                dtpBillDate = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Width = 120,
                    Location = new Point(0, 5),
                    Enabled = false,
                    Font = new Font("Segoe UI", 10)
                };

                chkUseDate = new CheckBox
                {
                    Text = "Use Date",
                    Location = new Point(130, 5),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                chkUseDate.CheckedChanged += ChkUseDate_CheckedChanged;

                datePanel.Controls.Add(dtpBillDate);
                datePanel.Controls.Add(chkUseDate);

                // Buttons
                var buttonPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.LeftToRight,
                    Padding = new Padding(0, 5, 0, 5)
                };

                btnSearch = CreateButton("SEARCH", PrimaryColor);
                btnSearch.Click += BtnSearch_Click;
                btnSearch.Height = 30;

                btnClear = CreateButton("CLEAR", SecondaryColor);
                btnClear.Click += BtnClear_Click;
                btnClear.Height = 30;
                btnClear.Margin = new Padding(10, 0, 0, 0);

                buttonPanel.Controls.Add(btnSearch);
                buttonPanel.Controls.Add(btnClear);

                // Add controls to filter table
                filterTable.Controls.Add(lblBillNumber, 0, 0);
                filterTable.Controls.Add(txtBillNumber, 1, 0);
                filterTable.Controls.Add(lblCustomerContact, 2, 0);
                filterTable.Controls.Add(txtCustomerContact, 3, 0);
                filterTable.Controls.Add(lblBillDate, 4, 0);
                filterTable.Controls.Add(datePanel, 5, 0);
                filterTable.Controls.Add(buttonPanel, 5, 1);

                filterPanel.Controls.Add(filterTable);

                // Data Grid
                dgvBills = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = BackgroundColor,
                    BorderStyle = BorderStyle.None,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    RowHeadersVisible = false,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect
                };

                // Configure grid style
                FormatDataGrid(dgvBills);

                // Create columns
                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Bill_ID",
                    HeaderText = "BILL NO",
                    Name = "colBillId",
                });

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "BillDate",
                    HeaderText = "DATE",
                    Name = "colBillDate",
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "dd-MMM-yyyy"
                    }
                });

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "PaymentMethod",
                    HeaderText = "PAYMENT",
                    Name = "colPayment",
                });

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "CustomerContact",
                    HeaderText = "CONTACT",
                    Name = "colContact",
                });

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Barcode",
                    HeaderText = "BARCODE",
                    Name = "colBarcode",
                });

                //dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                //{
                //    DataPropertyName = "Description",
                //    HeaderText = "DESCRIPTION",
                //    Name = "colDescription",
                //});

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "BrandName",
                    HeaderText = "BRAND",
                    Name = "colBrand",
                });

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "CategoryName",
                    HeaderText = "CATEGORY",
                    Name = "colCategory",
                });

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "SizeLabel",
                    HeaderText = "SIZE",
                    Name = "colSize",
                });

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Quantity",
                    HeaderText = "QTY",
                    Name = "colQuantity",
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                });

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "RetailPrice",
                    HeaderText = "RETAIL PRICE",
                    Name = "colRetailPrice",
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "N2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                });

                dgvBills.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ItemSellingPrice",
                    HeaderText = "SOLD PRICE",
                    Name = "colSoldPrice",
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "N2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                });

                // Image column
                //var imgCol = new DataGridViewImageColumn
                //{
                //    DataPropertyName = "ItemImage",
                //    HeaderText = "IMAGE",
                //    Name = "colImage",
                //    ImageLayout = DataGridViewImageCellLayout.Zoom,
                //    FillWeight = 80
                //};
                //dgvBills.Columns.Add(imgCol);
                //dgvBills.AutoGenerateColumns = false;

                // Handle image formatting
                //dgvBills.CellFormatting += DgvBills_CellFormatting;

                // Assemble container
                container.Controls.Add(dgvBills);
                container.Controls.Add(filterPanel);
                container.Controls.Add(titlePanel);
                this.Controls.Add(container);
            }

            private void FormatDataGrid(DataGridView dgv)
            {
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.RowHeadersVisible = false;
                dgv.ReadOnly = true;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.BackgroundColor = BackgroundColor;
                dgv.ForeColor = ForegroundColor;
                dgv.BorderStyle = BorderStyle.None;
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgv.RowTemplate.Height = 50;


                // Header styling
                dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(0)
                };
                dgv.ColumnHeadersHeight = 40;
                dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                // Cell styling
                dgv.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor,
                    Padding = new Padding(10, 5, 10, 5)
                };

                // Alternating rows
                dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                dgv.EnableHeadersVisualStyles = false;
            }

            private Button CreateButton(string text, Color backColor)
            {
                return new Button
                {
                    Text = text,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = {
                BorderSize = 0,
                MouseOverBackColor = ControlPaint.Light(backColor, 0.2f)
            },
                    BackColor = backColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Height = 35,
                    Margin = new Padding(0, 0, 10, 0)
                };
            }

            //private void DgvBills_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
            //{
            //    if (e.RowIndex < 0 || e.ColumnIndex != dgvBills.Columns["colImage"].Index)
            //        return;

            //    if (dgvBills.Rows[e.RowIndex].Cells["colImage"].Value != null &&
            //        dgvBills.Rows[e.RowIndex].Cells["colImage"].Value != DBNull.Value)
            //    {
            //        try
            //        {
            //            byte[] imageData = (byte[])dgvBills.Rows[e.RowIndex].Cells["colImage"].Value;
            //            using (var ms = new System.IO.MemoryStream(imageData))
            //            {
            //                e.Value = Image.FromStream(ms);
            //            }
            //        }
            //        catch
            //        {
            //            SetDefaultImage(e);
            //        }
            //    }
            //    else
            //    {
            //        SetDefaultImage(e);
            //    }
            //}

            //private void SetDefaultImage(DataGridViewCellFormattingEventArgs e)
            //{
            //    // Create a default placeholder image
            //    var defaultImage = new Bitmap(60, 60);
            //    using (var g = Graphics.FromImage(defaultImage))
            //    {
            //        g.Clear(Color.LightGray);
            //        using (var pen = new Pen(Color.Gray, 1))
            //        {
            //            g.DrawRectangle(pen, 0, 0, defaultImage.Width - 1, defaultImage.Height - 1);
            //        }
            //        using (var font = new Font("Arial", 8))
            //        {
            //            g.DrawString("No Image", font, Brushes.DarkGray,
            //                new PointF(10, defaultImage.Height / 2 - 10));
            //        }
            //    }
            //    e.Value = defaultImage;
            //}

            private void ChkUseDate_CheckedChanged(object sender, EventArgs e)
            {
                dtpBillDate.Enabled = chkUseDate.Checked;
            }

            private void TextBox_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SearchBills();
                }
            }

            private void BtnSearch_Click(object sender, EventArgs e)
            {
                SearchBills();
            }

            private void BtnClear_Click(object sender, EventArgs e)
            {
                txtBillNumber.Clear();
                txtCustomerContact.Clear();
                chkUseDate.Checked = false;
                if (dgvBills.DataSource is DataTable dt)
                {
                    dt.Clear();
                }
                else
                {
                    dgvBills.DataSource = new DataTable();
                }
            }

            private void SearchBills()
            {
                int? billId = null;
                if (!string.IsNullOrWhiteSpace(txtBillNumber.Text))
                {
                    if (int.TryParse(txtBillNumber.Text, out int tempBillId))
                    {
                        billId = tempBillId;
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid Bill Number", "Invalid Input",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string contact = txtCustomerContact.Text.Trim();
                if (contact == "") contact = null;

                DateTime? billDate = null;
                if (chkUseDate.Checked)
                {
                    billDate = dtpBillDate.Value.Date;
                }

                try
                {
                    using (SqlConnection conn = DbHelper.GetConnection())
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_SearchBills", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@BillID", billId ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@CustomerContact", contact ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@BillDate", billDate ?? (object)DBNull.Value);

                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            if (!dt.Columns.Contains("ItemImage"))
                            {
                                dt.Columns.Add("ItemImage", typeof(byte[]));
                            }

                            dgvBills.DataSource = dt;

                            if (dgvBills.Columns.Contains("ItemImage"))
                            {
                                dgvBills.Columns["ItemImage"].Visible = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error searching bills: {ex.Message}\n\n{ex.InnerException?.Message}",
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public class SizeForm : Form
        {
            private readonly Sizes _size;
            private TextBox txtLabel;
            private TextBox txtType;
            private Button btnSave;
            private Button btnCancel;

            public SizeForm(Sizes size = null)
            {
                _size = size ?? new Sizes();
                InitializeComponent();
                Text = _size.Size_ID == 0 ? "Add New Size" : "Edit Size";
            }

            private void InitializeComponent()
            {
                // Form setup
                Size = new Size(400, 250);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                StartPosition = FormStartPosition.CenterParent;
                MaximizeBox = false;
                MinimizeBox = false;
                BackColor = Color.White;

                // Main layout
                var mainPanel = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 3,
                    Padding = new Padding(20),
                    ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 30F),
                new ColumnStyle(SizeType.Percent, 70F)
            },
                    RowStyles =
            {
                new RowStyle(SizeType.Absolute, 40F),
                new RowStyle(SizeType.Absolute, 40F),
                new RowStyle(SizeType.Percent, 100F)
            }
                };

                // Labels
                mainPanel.Controls.Add(new Label
                {
                    Text = "Size Label:",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                }, 0, 0);

                mainPanel.Controls.Add(new Label
                {
                    Text = "Size Type:",
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill
                }, 0, 1);

                // TextBoxes
                txtLabel = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };
                txtType = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };

                mainPanel.Controls.Add(txtLabel, 1, 0);
                mainPanel.Controls.Add(txtType, 1, 1);

                // Button panel
                var buttonPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 60,
                    Padding = new Padding(10)
                };

                btnSave = new Button
                {
                    Text = "Save",
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(41, 128, 185),
                    ForeColor = Color.White,
                    Size = new Size(100, 40),
                    Anchor = AnchorStyles.Right
                };

                btnCancel = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(231, 76, 60),
                    ForeColor = Color.White,
                    Size = new Size(100, 40),
                    Anchor = AnchorStyles.Right
                };

                btnSave.Left = buttonPanel.Width - btnSave.Width - btnCancel.Width - 20;
                btnCancel.Left = buttonPanel.Width - btnCancel.Width - 10;

                buttonPanel.Controls.Add(btnSave);
                buttonPanel.Controls.Add(btnCancel);

                // Event handlers
                btnSave.Click += BtnSave_Click;
                Load += SizeForm_Load;

                // Add controls
                Controls.Add(mainPanel);
                Controls.Add(buttonPanel);
            }

            //private void SizeForm_Load(object sender, EventArgs e)
            //{
            //    txtLabel.Text = _size.SizeLabel;
            //    txtType.Text = _size.SizeType;
            //}

            //private void BtnSave_Click(object sender, EventArgs e)
            //{
            //    if (string.IsNullOrWhiteSpace(txtLabel.Text))
            //    {
            //        MessageBox.Show("Size label is required", "Validation Error",
            //                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return;
            //    }

            //    _size.SizeLabel = txtLabel.Text.Trim();
            //    _size.SizeType = txtType.Text.Trim();

            //    var service = new SizeService();
            //    if (service.SaveSize(_size))
            //    {
            //        DialogResult = DialogResult.OK;
            //        Close();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Failed to save size", "Database Error",
            //                       MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}

            private void SizeForm_Load(object sender, EventArgs e)
            {
                // Populate form fields
                txtLabel.Text = _size.SizeLabel;
                txtType.Text = _size.SizeType;
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtLabel.Text))
                {
                    MessageBox.Show("Size label is required", "Validation Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLabel.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtType.Text))
                {
                    MessageBox.Show("Size type is required", "Validation Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtType.Focus();
                    return;
                }

                // Update size object
                _size.SizeLabel = txtLabel.Text.Trim();
                _size.SizeType = txtType.Text.Trim();

                // Save through service
                var service = new SizeService();
                if (service.SaveSize(_size))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to save size", "Database Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public class ReportsView : Form { }
    }
}
