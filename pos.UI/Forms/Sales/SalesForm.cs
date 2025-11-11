using ClosedXML.Excel;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using RetailPOS.BLL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class SalesForm : Form
    {
        private readonly SalesReportService _salesService;
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
        private Button btnPrintSummary;
        private Label[] summaryLabels = new Label[10];
        private TabControl tabControl;
        private DataGridView dgvSalesItems;
        private DataGridView dgvReturnItems;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSalesTrend;

        public SalesForm()
        {
            InitializeComponent();
            _salesService = new SalesReportService(new SalesReportRepository());
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
            btnFilter = CreateButton("Apply Filters", PrimaryColor, 300, 55);
            btnFilter.Click += BtnFilter_Click;

            btnClear = CreateButton("Clear Filters", SecondaryColor, 450, 55);
            btnClear.Click += BtnClear_Click;

            btnExport = CreateButton("Export Report", Color.DarkGreen, 600, 55);
            btnExport.Click += BtnExport_Click;

            btnPrintSummary = CreateButton("Print Summary", Color.DarkOrange, 750, 55);
            btnPrintSummary.Click += BtnPrintSummary_Click;
            btnPrintSummary.Enabled = false;

            // Add controls to filter panel
            filterPanel.Controls.AddRange(new Control[] {
                    lblStartDate, dtpStartDate, lblEndDate, dtpEndDate,
                    lblBrand, cmbBrand, lblCategory, cmbCategory,
                    btnFilter, btnClear, btnExport, btnPrintSummary
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
            dgvSalesItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
                    "Bills Processed", "Return Value", "Cash Sales",  // Changed index 5
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
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
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
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
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
                Size = new Size(130, 35),
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = {
                        BorderSize = 0
                    },
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
            if (_currentReport == null) return;

            // Get all report sections
            var accounting = _currentReport.AccountingSummary;
            var cashFlow = _currentReport.CashFlowSummary;
            var tokenActivity = _currentReport.TokenActivity;

            // Update summary labels with new mappings
            //ActualCost, ActualProfit & ActualSales
            summaryLabels[0].Text = accounting.ActualSales.ToString("N2");  // Total Sales → CashInflow
            summaryLabels[1].Text = accounting.ActualCost.ToString("N2");    // Total Cost
            summaryLabels[2].Text = accounting.ActualProfit.ToString("N2");  // Gross Profit

            summaryLabels[3].Text = accounting.NetItemsSold.ToString("N0");  // Items Sold
            summaryLabels[4].Text = accounting.BillCount.ToString("N0");  // Bills Processed
            summaryLabels[5].Text = tokenActivity.TokenValueIssued.ToString("N2");  // Return Value
            summaryLabels[6].Text = cashFlow.CashSales.ToString("N2");    // Cash Sales
            summaryLabels[7].Text = cashFlow.CardSales.ToString("N2");    // Card Sales
            summaryLabels[8].Text = cashFlow.BankSales.ToString("N2");    // Bank Transfers
            summaryLabels[9].Text = tokenActivity.TokensUsed.ToString("N0");  // Returns

            // Bind data grids
            dgvSalesItems.DataSource = _currentReport.SalesItems;
            FormatDataGridColumns(dgvSalesItems, true);

            dgvReturnItems.DataSource = _currentReport.ReturnItems;
            FormatDataGridColumns(dgvReturnItems, false);

            // Enable print button
            btnPrintSummary.Enabled = _currentReport != null;

            // Update chart
            UpdateSalesChart();
        }

        // Print functionality
        private void BtnPrintSummary_Click(object sender, EventArgs e)
        {
            try
            {
                PrintSalesSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing summary: {ex.Message}", "Print Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintSalesSummary()
        {
            const string PRINTER_NAME = "XP-80C";
            if (string.IsNullOrEmpty(PRINTER_NAME))
            {
                MessageBox.Show("No receipt printer found.", "Print Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<byte> output = new List<byte>();
            output.AddRange(new byte[] { 0x1B, 0x40 }); // Initialize printer

            // Print header
            PrintCentered("SALES SUMMARY REPORT", output, true);
            PrintCentered("STYLE NEWAGE", output);
            PrintCentered("No.102, Negombo Rd, Narammala.", output);
            PrintCentered("Tel: 0777491913 / 0374545097", output);
            output.AddRange(Encoding.ASCII.GetBytes("\n"));

            // Print date range
            PrintLeftRight("Start Date:", dtpStartDate.Value.ToString("yyyy-MM-dd"), output);
            PrintLeftRight("End Date:", dtpEndDate.Value.ToString("yyyy-MM-dd"), output);
            PrintLeftRight("Generated:", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), output);
            output.AddRange(Encoding.ASCII.GetBytes("\n"));
            PrintSeparator(output);

            // Print summary data
            for (int i = 0; i < summaryLabels.Length; i++)
            {
                string label = summaryLabels[i].Tag.ToString();
                string value = summaryLabels[i].Text;

                // Format currency values
                if (i < 3 || i == 5 || i == 6 || i == 7 || i == 8)
                {
                    value = "Rs." + value;
                }

                PrintLeftRight(label, value, output);
            }

            // Print footer
            PrintSeparator(output);
            PrintCentered("End of Report", output);
            output.AddRange(Encoding.ASCII.GetBytes("\n\n\n"));

            // Printer commands
            output.AddRange(new byte[] { 0x1B, 0x64, 0x02 }); // Feed 2 lines
            output.AddRange(new byte[] { 0x1B, 0x69 });        // Cut paper

            // Send to printer
            RawPrinterHelper.SendBytesToPrinter(PRINTER_NAME, output.ToArray());
        }

        // Helper methods (same as in ReturnsForm)
        private void PrintCentered(string text, List<byte> output, bool bold = false)
        {
            const int MAX_LINE_WIDTH = 32;

            if (bold)
                output.AddRange(new byte[] { 0x1B, 0x45, 0x01 }); // Bold on

            if (text.Length > MAX_LINE_WIDTH)
                text = text.Substring(0, MAX_LINE_WIDTH);

            int spaces = (MAX_LINE_WIDTH - text.Length) / 2;
            output.AddRange(Encoding.ASCII.GetBytes(new string(' ', spaces) + text + "\n"));

            if (bold)
                output.AddRange(new byte[] { 0x1B, 0x45, 0x00 }); // Bold off
        }

        private void PrintLeft(string text, List<byte> output)
        {
            const int MAX_LINE_WIDTH = 32;
            text = text.Length > MAX_LINE_WIDTH ? text.Substring(0, MAX_LINE_WIDTH) : text;
            output.AddRange(Encoding.ASCII.GetBytes(text + "\n"));
        }

        private void PrintLeftRight(string left, string right, List<byte> output, bool bold = false)
        {
            const int MAX_LINE_WIDTH = 32;

            if (bold)
                output.AddRange(new byte[] { 0x1B, 0x45, 0x01 }); // Bold on

            int availableSpace = MAX_LINE_WIDTH - right.Length - 1;
            if (availableSpace < 1) availableSpace = 1;

            left = left.Length > availableSpace ? left.Substring(0, availableSpace) : left;
            string line = left.PadRight(availableSpace) + " " + right;
            output.AddRange(Encoding.ASCII.GetBytes(line + "\n"));

            if (bold)
                output.AddRange(new byte[] { 0x1B, 0x45, 0x00 }); // Bold off
        }

        private void PrintSeparator(List<byte> output)
        {
            output.AddRange(Encoding.ASCII.GetBytes(new string('-', 32) + "\n"));
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

        public static class RawPrinterHelper
        {
            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA",
                SetLastError = true, CharSet = CharSet.Ansi,
                ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter,
                out IntPtr hPrinter, IntPtr pd);

            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter",
                SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA",
                SetLastError = true, CharSet = CharSet.Ansi,
                ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level,
                [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

            [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter",
                SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool EndDocPrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter",
                SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool StartPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter",
                SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool EndPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "WritePrinter",
                SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes,
                Int32 dwCount, out Int32 dwWritten);

            [StructLayout(LayoutKind.Sequential)]
            private class DOCINFOA
            {
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDocName;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pOutputFile;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDataType;
            }

            public static bool SendBytesToPrinter(string szPrinterName, byte[] pBytes)
            {
                Int32 dwError = 0, dwWritten = 0;
                IntPtr hPrinter = IntPtr.Zero;
                DOCINFOA di = new DOCINFOA();
                di.pDocName = "POS Sales Summary";
                di.pDataType = "RAW";

                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        if (StartPagePrinter(hPrinter))
                        {
                            IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(pBytes.Length);
                            Marshal.Copy(pBytes, 0, pUnmanagedBytes, pBytes.Length);
                            bool success = WritePrinter(hPrinter, pUnmanagedBytes, pBytes.Length, out dwWritten);
                            Marshal.FreeCoTaskMem(pUnmanagedBytes);

                            if (!success || dwWritten != pBytes.Length)
                            {
                                dwError = Marshal.GetLastWin32Error();
                            }
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
                else
                {
                    dwError = Marshal.GetLastWin32Error();
                }

                if (dwError != 0)
                {
                    throw new Win32Exception(dwError, $"Printer error (Code: {dwError})");
                }

                return dwWritten == pBytes.Length;
            }
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

        //private void BtnExport_Click(object sender, EventArgs e)
        //{
        //    if (_currentReport == null)
        //    {
        //        MessageBox.Show("No data to export. Please generate a report first.");
        //        return;
        //    }

        //    using (var sfd = new SaveFileDialog())
        //    {
        //        sfd.Filter = "Excel Files|*.xlsx|CSV Files|*.csv";
        //        sfd.Title = "Export Sales Report";
        //        if (sfd.ShowDialog() == DialogResult.OK)
        //        {
        //            try
        //            {
        //                ExportReport(sfd.FileName);
        //                MessageBox.Show("Report exported successfully!");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Export failed: {ex.Message}");
        //            }
        //        }
        //    }
        //}

        // Add to SalesForm.cs
        private void ExportReport(string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    // 1. Accounting Summary
                    var accSheet = workbook.Worksheets.Add("Accounting Summary");
                    AddAccountingSummary(accSheet, _currentReport.AccountingSummary);

                    // 2. Cash Flow
                    var cashSheet = workbook.Worksheets.Add("Cash Flow");
                    AddCashFlowSummary(cashSheet, _currentReport.CashFlowSummary);

                    // 3. Token Activity
                    var tokenSheet = workbook.Worksheets.Add("Token Activity");
                    AddTokenActivity(tokenSheet, _currentReport.TokenActivity);

                    // 4. Sales Items
                    var salesSheet = workbook.Worksheets.Add("Sales Items");
                    AddDataTable(salesSheet, _currentReport.SalesItems);

                    // 5. Return Items
                    var returnSheet = workbook.Worksheets.Add("Return Items");
                    AddDataTable(returnSheet, _currentReport.ReturnItems);

                    // 6. Bill Summaries
                    var billsSheet = workbook.Worksheets.Add("Bills");
                    AddDataTable(billsSheet, _currentReport.BillSummaries);

                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Export failed: " + ex.Message);
            }
        }

        private void AddAccountingSummary(IXLWorksheet sheet, AccountingSummary summary)
        {
            sheet.Cell(1, 1).Value = "Accounting Summary";
            sheet.Cell(1, 1).Style.Font.Bold = true;

            int row = 2;
            AddSheetRow(sheet, ref row, "Gross Sales", summary.GrossSales);
            AddSheetRow(sheet, ref row, "Discounts", summary.Discounts);
            AddSheetRow(sheet, ref row, "Returns", summary.Returns);
            AddSheetRow(sheet, ref row, "Net Sales", summary.NetSales);
            AddSheetRow(sheet, ref row, "Gross COGS", summary.GrossCOGS);
            AddSheetRow(sheet, ref row, "Returns COGS", summary.ReturnsCOGS);
            AddSheetRow(sheet, ref row, "Net COGS", summary.NetCOGS);
            AddSheetRow(sheet, ref row, "Gross Profit", summary.GrossProfit);
            AddSheetRow(sheet, ref row, "Net Profit", summary.NetProfit);
            AddSheetRow(sheet, ref row, "Gross Items Sold", summary.GrossItemsSold);
            AddSheetRow(sheet, ref row, "Returned Items", summary.ReturnedItems);
            AddSheetRow(sheet, ref row, "Net Items Sold", summary.NetItemsSold);
            AddSheetRow(sheet, ref row, "Bill Count", summary.BillCount);
        }

        private void AddCashFlowSummary(IXLWorksheet sheet, CashFlowSummary summary)
        {
            sheet.Cell(1, 1).Value = "Cash Flow Summary";
            sheet.Cell(1, 1).Style.Font.Bold = true;

            int row = 2;
            AddSheetRow(sheet, ref row, "Cash Inflow", summary.CashInflow);
            AddSheetRow(sheet, ref row, "Cash Outflow", summary.CashOutflow);
            AddSheetRow(sheet, ref row, "Net Cash Flow", summary.NetCashFlow);
            AddSheetRow(sheet, ref row, "Cash Sales", summary.CashSales);
            AddSheetRow(sheet, ref row, "Card Sales", summary.CardSales);
            AddSheetRow(sheet, ref row, "Bank Sales", summary.BankSales);
        }

        private void AddTokenActivity(IXLWorksheet sheet, TokenActivity activity)
        {
            sheet.Cell(1, 1).Value = "Token Activity";
            sheet.Cell(1, 1).Style.Font.Bold = true;

            int row = 2;
            AddSheetRow(sheet, ref row, "Tokens Issued", activity.TokensIssued);
            AddSheetRow(sheet, ref row, "Token Value Issued", activity.TokenValueIssued);
            AddSheetRow(sheet, ref row, "Tokens Used", activity.TokensUsed);
            AddSheetRow(sheet, ref row, "Token Value Used", activity.TokenValueUsed);
            AddSheetRow(sheet, ref row, "Tokens Outstanding", activity.TokensOutstanding);
            AddSheetRow(sheet, ref row, "Token Value Outstanding", activity.TokenValueOutstanding);
        }

        private void AddSheetRow(IXLWorksheet sheet, ref int row, string label, object value)
        {
            sheet.Cell(row, 1).Value = label;

            // Handle different types explicitly
            switch (value)
            {
                case decimal dec:
                    sheet.Cell(row, 2).Value = dec;
                    sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                    break;
                case int i:
                    sheet.Cell(row, 2).Value = i;
                    break;
                case double d:
                    sheet.Cell(row, 2).Value = d;
                    break;
                case DateTime dt:
                    sheet.Cell(row, 2).Value = dt;
                    sheet.Cell(row, 2).Style.DateFormat.Format = "yyyy-MM-dd";
                    break;
                default:
                    sheet.Cell(row, 2).Value = value?.ToString() ?? string.Empty;
                    break;
            }
            row++;
        }

        private void AddDataTable<T>(IXLWorksheet sheet, List<T> data)
        {
            if (data == null || data.Count == 0) return;

            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = properties[i].Name;
                sheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                for (int j = 0; j < properties.Length; j++)
                {
                    var value = properties[j].GetValue(item);
                    var cell = sheet.Cell(i + 2, j + 1);

                    // Handle different types
                    switch (value)
                    {
                        case decimal dec:
                            cell.Value = dec;
                            cell.Style.NumberFormat.Format = "#,##0.00";
                            break;
                        case int intVal:
                            cell.Value = intVal;
                            break;
                        case double dbl:
                            cell.Value = dbl;
                            break;
                        case DateTime dt:
                            cell.Value = dt;
                            cell.Style.DateFormat.Format = "yyyy-MM-dd";
                            break;
                        case bool b:
                            cell.Value = b;
                            break;
                        default:
                            cell.Value = value?.ToString() ?? string.Empty;
                            break;
                    }
                }
            }

            sheet.Columns().AdjustToContents();
        }

        // Update BtnExport_Click method
        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (_currentReport == null)
            {
                MessageBox.Show("No data to export. Please generate a report first.");
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                // Generate filename with date range
                string dateRange = $"{dtpStartDate.Value:yyyyMMdd}-{dtpEndDate.Value:yyyyMMdd}";
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string defaultName = $"SalesReport_{dateRange}_{timestamp}.xlsx";

                sfd.FileName = defaultName;
                sfd.Filter = "Excel Files|*.xlsx";
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


    }
}
