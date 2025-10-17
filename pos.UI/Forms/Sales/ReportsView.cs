using LiveCharts;
using LiveCharts.Wpf;
using pos_system.pos.BLL.Repositories;
using System.Data;
using System.Diagnostics;
using CartesianChart = LiveCharts.WinForms.CartesianChart;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class ReportsView : Form
    {
        // Theme colors
        private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
        private static readonly Color ForegroundColor = Color.Black;
        private static readonly Color SecondaryColor = Color.Gray;
        private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
        private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

        private readonly ReportService _reportService = new ReportService();
        private TabControl tabControl;
        private CartesianChart billTimeChart;
        private CartesianChart categorySalesChart;

        // Main chart controls
        private ComboBox cmbDateRange;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnGenerate;
        private Label lblTo;

        // Category chart controls
        private ComboBox cmbCategoryDateRange;
        private DateTimePicker dtpCategoryStartDate;
        private DateTimePicker dtpCategoryEndDate;
        private Button btnCategoryGenerate;
        private Label lblCategoryTo;

        private bool isInitializing = true;
        private SplitContainer mainHorizontalSplitContainer;

        public ReportsView()
        {
            InitializeComponent();
            this.Load += ReportsView_Load; // Use Load event instead of Shown

            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(0, 1800);
            isInitializing = false;
        }

        private void ReportsView_Load(object sender, EventArgs e)
        {
            LoadDefaultCharts();
            // Splitter distance will be set after controls are fully initialized
            System.Windows.Forms.Application.Idle += Application_Idle; // Wait for idle to ensure layout is complete
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Idle -= Application_Idle; // Only run once
            SafeSetSplitterDistance();
        }

        private void SafeSetSplitterDistance()
        {
            if (mainHorizontalSplitContainer == null || mainHorizontalSplitContainer.Height <= 0)
                return;

            try
            {
                // Calculate the available height for panels (total height minus splitter width)
                int totalHeight = mainHorizontalSplitContainer.Height;
                int splitterWidth = mainHorizontalSplitContainer.SplitterWidth;
                int availableHeight = totalHeight - splitterWidth;

                // Calculate equal split
                int equalSplit = availableHeight / 2;

                // Get the minimum and maximum allowed values
                int minAllowed = mainHorizontalSplitContainer.Panel1MinSize;
                int maxAllowed = totalHeight - mainHorizontalSplitContainer.Panel2MinSize - splitterWidth;

                // Ensure the splitter distance is within valid bounds
                int safeSplitterDistance = equalSplit;

                if (safeSplitterDistance < minAllowed)
                {
                    safeSplitterDistance = minAllowed;
                }
                else if (safeSplitterDistance > maxAllowed)
                {
                    safeSplitterDistance = maxAllowed;
                }

                // Only set if it's different from current value and within valid range
                if (safeSplitterDistance >= minAllowed && safeSplitterDistance <= maxAllowed)
                {
                    if (mainHorizontalSplitContainer.SplitterDistance != safeSplitterDistance)
                    {
                        mainHorizontalSplitContainer.SplitterDistance = safeSplitterDistance;
                    }
                }
                else
                {
                    // Fallback: Use a percentage-based approach that's guaranteed to be valid
                    int fallbackDistance = (int)(totalHeight * 0.45); // 45% for first panel
                    fallbackDistance = Math.Max(minAllowed, Math.Min(fallbackDistance, maxAllowed));

                    if (fallbackDistance >= minAllowed && fallbackDistance <= maxAllowed)
                    {
                        mainHorizontalSplitContainer.SplitterDistance = fallbackDistance;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting splitter distance: {ex.Message}");
                // Ignore the error - the splitter will use its default position
            }
        }

        private void InitializeComponent()
        {
            // Form setup
            this.Size = new Size(1200, 900);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = BackgroundColor;
            this.Text = "Reports";

            // Main container
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor,
                Padding = new Padding(20)
            };

            // Tab control
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

            // Sales Graphs tab
            var tabSalesGraphs = new TabPage { Text = "SALES GRAPHS" };
            tabSalesGraphs.BackColor = BackgroundColor;
            tabSalesGraphs.Controls.Add(CreateSalesGraphsLayout());

            // Reports tab (for future implementation)
            var tabReports = new TabPage { Text = "REPORTS" };
            tabReports.BackColor = BackgroundColor;
            tabReports.Controls.Add(CreateReportsLayout());

            tabControl.TabPages.Add(tabSalesGraphs);
            tabControl.TabPages.Add(tabReports);
            container.Controls.Add(tabControl);
            this.Controls.Add(container);
        }

        private Panel CreateSalesGraphsLayout()
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
                Text = "SALES ANALYTICS",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Padding = new Padding(20, 0, 0, 0)
            };

            // Main horizontal split container for top-bottom charts (two rows)
            mainHorizontalSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterWidth = 8,
                BackColor = Color.LightGray,
                Panel1MinSize = 150,  // Correct property on the SplitContainer
                Panel2MinSize = 150   // Correct property on the SplitContainer
                                      //Panel1 = { MinSize = 150 },  // Increased minimum sizes
                                      //Panel2 = { MinSize = 150 }   // Increased minimum sizes
            };

            // Handle resize to maintain reasonable sizes (but don't force equal)
            mainHorizontalSplitContainer.Resize += (sender, e) =>
            {
                // Don't automatically adjust on resize - let user have control
                // SafeSetSplitterDistance();
            };

            // --- TOP PANEL: Bill Time Chart ---
            var topPanel = new Panel { Dock = DockStyle.Fill };

            // Control panel for bill chart
            var billControlPanel = CreateBillControlPanel();
            billControlPanel.Dock = DockStyle.Top;

            // Chart container for bill time chart
            var billChartContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor,
                Padding = new Padding(10)
            };

            var lblBillChart = new Label
            {
                Text = "BILLS OVER TIME",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Height = 30
            };

            billTimeChart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor
            };

            billChartContainer.Controls.Add(billTimeChart);
            billChartContainer.Controls.Add(lblBillChart);

            topPanel.Controls.Add(billChartContainer);
            topPanel.Controls.Add(billControlPanel);

            // --- BOTTOM PANEL: Category Sales Chart ---
            var bottomPanel = new Panel { Dock = DockStyle.Fill };

            // Control panel for category chart
            var categoryControlPanel = CreateCategoryControlPanel();
            categoryControlPanel.Dock = DockStyle.Top;

            // Chart container for category sales chart
            var categoryChartContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor,
                Padding = new Padding(10)
            };

            var lblCategoryChart = new Label
            {
                Text = "CATEGORY SALES",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Height = 30
            };

            categorySalesChart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor
            };

            categoryChartContainer.Controls.Add(categorySalesChart);
            categoryChartContainer.Controls.Add(lblCategoryChart);

            bottomPanel.Controls.Add(categoryChartContainer);
            bottomPanel.Controls.Add(categoryControlPanel);

            // Assemble the main horizontal split container
            mainHorizontalSplitContainer.Panel1.Controls.Add(topPanel);
            mainHorizontalSplitContainer.Panel2.Controls.Add(bottomPanel);

            // Layout
            titlePanel.Controls.Add(lblTitle);
            panel.Controls.Add(mainHorizontalSplitContainer);
            panel.Controls.Add(titlePanel);

            return panel;
        }

        private Panel CreateBillControlPanel()
        {
            var controlPanel = new Panel
            {
                Height = 60,
                BackColor = HeaderColor,
                Padding = new Padding(20, 15, 20, 15),
                Margin = new Padding(0, 0, 0, 5)
            };

            // Date range combo box
            var lblDateRange = new Label
            {
                Text = "Bill Chart Date Range:",
                Location = new Point(0, 15),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ForegroundColor
            };

            cmbDateRange = new ComboBox
            {
                Location = new Point(160, 12),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            cmbDateRange.Items.AddRange(new object[] {
            "Today", "Yesterday", "This Week", "Last Week",
            "This Month", "Last Month", "Custom"
        });

            // Date pickers (initially hidden)
            dtpStartDate = new DateTimePicker
            {
                Location = new Point(320, 12),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Visible = false
            };

            dtpEndDate = new DateTimePicker
            {
                Location = new Point(450, 12),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Visible = false
            };

            // lblTo initialization
            lblTo = new Label
            {
                Text = "to",
                Location = new Point(445, 15),
                Size = new Size(20, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = ForegroundColor,
                Visible = false
            };

            // Generate button
            btnGenerate = new Button
            {
                Text = "GENERATE BILL CHART",
                Size = new Size(200, 35),
                Location = new Point(580, 10),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGenerate.Click += BtnGenerate_Click;

            // Set selected index and attach event handler
            cmbDateRange.SelectedIndex = 0;
            cmbDateRange.SelectedIndexChanged += CmbDateRange_SelectedIndexChanged;

            controlPanel.Controls.AddRange(new Control[] {
            lblDateRange, cmbDateRange, dtpStartDate, dtpEndDate, lblTo, btnGenerate
        });

            return controlPanel;
        }

        private Panel CreateCategoryControlPanel()
        {
            var categoryControlPanel = new Panel
            {
                Height = 60,
                BackColor = HeaderColor,
                Padding = new Padding(20, 15, 20, 15),
                Margin = new Padding(0, 0, 0, 5)
            };

            // Label for the dropdown
            var lblCategoryDateRange = new Label
            {
                Text = "Category Chart Date Range:",
                Location = new Point(0, 15),
                Size = new Size(190, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ForegroundColor
            };

            // ComboBox for category chart date range
            cmbCategoryDateRange = new ComboBox
            {
                Location = new Point(200, 12),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cmbCategoryDateRange.Items.AddRange(new object[] {
            "Today", "Yesterday", "This Week", "Last Week",
            "This Month", "Last Month", "Custom"
        });
            cmbCategoryDateRange.SelectedIndex = 0;

            // Date pickers for category chart (initially hidden)
            dtpCategoryStartDate = new DateTimePicker
            {
                Location = new Point(360, 12),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Visible = false
            };

            dtpCategoryEndDate = new DateTimePicker
            {
                Location = new Point(490, 12),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Visible = false
            };

            // Label for "to"
            lblCategoryTo = new Label
            {
                Text = "to",
                Location = new Point(485, 15),
                Size = new Size(20, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = ForegroundColor,
                Visible = false
            };

            // Generate button for category chart
            btnCategoryGenerate = new Button
            {
                Text = "GENERATE CATEGORY CHART",
                Size = new Size(240, 35),
                Location = new Point(620, 10),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCategoryGenerate.Click += BtnCategoryGenerate_Click;

            // Set event handler for category date range
            cmbCategoryDateRange.SelectedIndexChanged += CmbCategoryDateRange_SelectedIndexChanged;

            categoryControlPanel.Controls.AddRange(new Control[] {
            lblCategoryDateRange, cmbCategoryDateRange, dtpCategoryStartDate,
            dtpCategoryEndDate, lblCategoryTo, btnCategoryGenerate
        });

            return categoryControlPanel;
        }

        private Panel CreateReportsLayout()
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
                Text = "REPORTS",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Padding = new Padding(20, 0, 0, 0)
            };

            // Content panel
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor
            };

            var lblMessage = new Label
            {
                Text = "Reports functionality will be implemented in a future version.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14),
                ForeColor = SecondaryColor
            };

            contentPanel.Controls.Add(lblMessage);
            titlePanel.Controls.Add(lblTitle);
            panel.Controls.Add(contentPanel);
            panel.Controls.Add(titlePanel);

            return panel;
        }

        private void CmbDateRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing || lblTo == null || dtpStartDate == null || dtpEndDate == null)
                return;

            bool isCustom = cmbDateRange.SelectedItem.ToString() == "Custom";
            dtpStartDate.Visible = isCustom;
            dtpEndDate.Visible = isCustom;
            lblTo.Visible = isCustom;
        }

        private void CmbCategoryDateRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing || lblCategoryTo == null || dtpCategoryStartDate == null || dtpCategoryEndDate == null)
                return;

            bool isCustom = cmbCategoryDateRange.SelectedItem.ToString() == "Custom";
            dtpCategoryStartDate.Visible = isCustom;
            dtpCategoryEndDate.Visible = isCustom;
            lblCategoryTo.Visible = isCustom;
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            DateTime startDate, endDate;
            CalculateDateRange(cmbDateRange.SelectedItem.ToString(), dtpStartDate, dtpEndDate, out startDate, out endDate);
            LoadBillTimeChart(startDate, endDate);
        }

        private void BtnCategoryGenerate_Click(object sender, EventArgs e)
        {
            DateTime startDate, endDate;
            CalculateDateRange(cmbCategoryDateRange.SelectedItem.ToString(), dtpCategoryStartDate, dtpCategoryEndDate, out startDate, out endDate);
            LoadCategorySalesChart(startDate, endDate);
        }

        private void CalculateDateRange(string selectedRange, DateTimePicker startPicker, DateTimePicker endPicker, out DateTime startDate, out DateTime endDate)
        {
            switch (selectedRange)
            {
                case "Today":
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                    break;
                case "Yesterday":
                    startDate = DateTime.Today.AddDays(-1);
                    endDate = DateTime.Today.AddSeconds(-1);
                    break;
                case "This Week":
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                    break;
                case "Last Week":
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7);
                    endDate = startDate.AddDays(7).AddSeconds(-1);
                    break;
                case "This Month":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                    break;
                case "Last Month":
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                    endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddSeconds(-1);
                    break;
                case "Custom":
                    startDate = startPicker.Value;
                    endDate = endPicker.Value.AddDays(1).AddSeconds(-1);
                    break;
                default:
                    startDate = DateTime.Today.AddDays(-7);
                    endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                    break;
            }
        }

        private void LoadDefaultCharts()
        {
            // Load default data (last 7 days) for both charts
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate.AddDays(-7);

            LoadBillTimeChart(startDate, endDate);
            LoadCategorySalesChart(startDate, endDate);
        }

        private void LoadBillTimeChart(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Get data from stored procedure
                DataTable data = _reportService.GetSalesTimeSeries(startDate, endDate);

                // Prepare data for chart
                var values = new ChartValues<decimal>();
                var labels = new List<string>();

                bool isSingleDay = (endDate - startDate).TotalDays <= 1;

                foreach (DataRow row in data.Rows)
                {
                    values.Add(Convert.ToDecimal(row["TotalSales"]));

                    if (isSingleDay)
                    {
                        // Format as time for single day
                        DateTime time = Convert.ToDateTime(row["Period"]);
                        labels.Add(time.ToString("HH:mm"));
                    }
                    else
                    {
                        // Format as date for multiple days
                        labels.Add(Convert.ToDateTime(row["Period"]).ToString("MMM dd"));
                    }
                }

                // Clear existing series
                billTimeChart.Series.Clear();

                // Create new series
                var series = new LineSeries
                {
                    Title = "Sales",
                    Values = values,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(PrimaryColor.R, PrimaryColor.G, PrimaryColor.B)),
                    Fill = System.Windows.Media.Brushes.Transparent,
                    StrokeThickness = 3
                };

                billTimeChart.Series.Add(series);

                // Configure X-axis
                billTimeChart.AxisX.Clear();
                billTimeChart.AxisX.Add(new LiveCharts.Wpf.Axis
                {
                    Title = isSingleDay ? "Time" : "Date",
                    Labels = labels.ToArray(),
                    Separator = new LiveCharts.Wpf.Separator { StrokeThickness = 0.5 },
                    LabelsRotation = 30
                });

                // Configure Y-axis
                billTimeChart.AxisY.Clear();
                billTimeChart.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    MinValue = 0,
                    Title = "Sales Amount",
                    LabelFormatter = value => value.ToString("N2")
                });

                billTimeChart.LegendLocation = LegendLocation.None;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bill time chart: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategorySalesChart(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Clear any existing data
                categorySalesChart.Series.Clear();
                categorySalesChart.AxisX.Clear();
                categorySalesChart.AxisY.Clear();

                // Get data from stored procedure
                DataTable data = _reportService.GetCategorySales(startDate, endDate);

                // Debug: Check what data we're getting
                Debug.WriteLine($"Data table has {data?.Rows.Count ?? 0} rows");
                if (data != null)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        Debug.WriteLine($"Category: {row["CategoryName"]}, Period: {row["Period"]}, Sales: {row["TotalSales"]}");
                    }
                }

                // Check if we have valid data
                if (data == null || data.Rows.Count == 0)
                {
                    // Create a simple message on the chart
                    var series = new ColumnSeries
                    {
                        Title = "No Data",
                        Values = new ChartValues<decimal> { 1 },
                        DataLabels = true,
                        LabelPoint = point => "No data available"
                    };

                    categorySalesChart.Series.Add(series);

                    categorySalesChart.AxisX.Add(new LiveCharts.Wpf.Axis
                    {
                        Title = "No Data",
                        Labels = new[] { "Select a different date range" }
                    });

                    categorySalesChart.AxisY.Add(new LiveCharts.Wpf.Axis
                    {
                        MinValue = 0,
                        Title = "Sales Amount",
                        LabelFormatter = value => value.ToString("N2")
                    });

                    return;
                }

                // Process the data for the chart
                bool isSingleDay = (endDate - startDate).TotalDays <= 1;
                var allPeriods = new List<string>();
                var categorySales = new Dictionary<string, Dictionary<string, decimal>>();

                // First, collect all unique periods and category data
                foreach (DataRow row in data.Rows)
                {
                    string category = row["CategoryName"].ToString();
                    DateTime periodDate = Convert.ToDateTime(row["Period"]);
                    string period = isSingleDay ? periodDate.ToString("HH:mm") : periodDate.ToString("MMM dd");
                    decimal sales = Convert.ToDecimal(row["TotalSales"]);

                    // Add period to our list if not already there
                    if (!allPeriods.Contains(period))
                        allPeriods.Add(period);

                    // Initialize category dictionary if needed
                    if (!categorySales.ContainsKey(category))
                        categorySales[category] = new Dictionary<string, decimal>();

                    // Add sales data for this category and period
                    categorySales[category][period] = sales;
                }

                // Sort periods chronologically
                if (isSingleDay)
                {
                    allPeriods = allPeriods.OrderBy(p => DateTime.ParseExact(p, "HH:mm", null)).ToList();
                }
                else
                {
                    allPeriods = allPeriods.OrderBy(p => DateTime.ParseExact(p, "MMM dd", null)).ToList();
                }

                // Create series for each category
                var colors = new[] {
                System.Windows.Media.Color.FromRgb(41, 128, 185),   // Primary blue
                System.Windows.Media.Color.FromRgb(231, 76, 60),    // Red
                System.Windows.Media.Color.FromRgb(39, 174, 96),    // Green
                System.Windows.Media.Color.FromRgb(241, 196, 15),   // Yellow
                System.Windows.Media.Color.FromRgb(142, 68, 173),   // Purple
                System.Windows.Media.Color.FromRgb(44, 62, 80),     // Dark blue
                System.Windows.Media.Color.FromRgb(243, 156, 18)    // Orange
            };

                int colorIndex = 0;
                foreach (var category in categorySales.Keys)
                {
                    var values = new ChartValues<decimal>();

                    // For each period, get the sales value for this category
                    foreach (string period in allPeriods)
                    {
                        decimal value = categorySales[category].ContainsKey(period) ?
                            categorySales[category][period] : 0;
                        values.Add(value);
                    }

                    var series = new ColumnSeries
                    {
                        Title = category,
                        Values = values,
                        Fill = new System.Windows.Media.SolidColorBrush(colors[colorIndex % colors.Length]),
                        DataLabels = true,
                        LabelPoint = point => point.Y > 0 ? point.Y.ToString("N2") : ""
                    };

                    categorySalesChart.Series.Add(series);
                    colorIndex++;
                }

                // Configure X-axis
                categorySalesChart.AxisX.Add(new LiveCharts.Wpf.Axis
                {
                    Title = isSingleDay ? "Time" : "Date",
                    Labels = allPeriods.ToArray(),
                    Separator = new LiveCharts.Wpf.Separator { StrokeThickness = 0.5 }
                });

                // Configure Y-axis
                categorySalesChart.AxisY.Add(new LiveCharts.Wpf.Axis
                {
                    MinValue = 0,
                    Title = "Sales Amount",
                    LabelFormatter = value => value.ToString("N2")
                });

                // Set legend location
                categorySalesChart.LegendLocation = LegendLocation.Right;

                // Force chart update
                categorySalesChart.Update(true, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadCategorySalesChart: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                MessageBox.Show($"Error loading category sales chart: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
