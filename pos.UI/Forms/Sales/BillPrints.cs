using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Services;
using pos_system.pos.BLL.Utilities;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using pos_system.pos.UI.Forms.Common;
using RetailPOS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class BillPrints : Form
    {
        // Theme colors
        private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
        private static readonly Color ForegroundColor = Color.Black;
        private static readonly Color SecondaryColor = Color.Gray;
        private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

        // Form controls
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private TextBox txtBillId;
        private TextBox txtCustomerContact;
        private DataGridView dgvBills;
        private Button btnSearch;
        private Button btnClear;
        private Button btnReprint;
        private Button btnDummyPrint;
        private ComboBox cboPrinters;
        private DataGridView dgvBillItems;
        private TableLayoutPanel searchLayout;

        // Data
        private DataTable _billItems = new DataTable();
        private decimal _subtotal;
        private decimal _totalPerItemDiscount;
        private decimal _billDiscount;
        private decimal _total;
        private string _paymentMethod;
        private string _cardLast4;
        private string _bankLast4;
        private decimal? _tokenValue;
        private string _customerContact;
        private DateTime _billDate;
        private string _cashierName;
        private int _selectedBillId;

        // Services
        private BillService _billService;
        private BillSummary _currentBillSummary;

        public BillPrints(Employee currentUser)
        {
            InitializeForm();
            _billService = new BillService();
        }

        private void InitializeForm()
        {
            // Form setup
            this.Text = "BILL SEARCH AND REPRINT";
            this.Size = new Size(1100, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = BackgroundColor;

            // Main container
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor,
                Padding = new Padding(15)
            };

            // Title panel
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "BILL SEARCH AND REPRINT",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Padding = new Padding(10, 0, 0, 0)
            };
            titlePanel.Controls.Add(lblTitle);

            // Search panel
            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = HeaderColor,
                Padding = new Padding(15)
            };

            // Create search layout with 2 rows
            searchLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 8,
                RowCount = 2,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            searchLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            searchLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            // First row: date pickers and bill ID
            var lblStartDate = new Label
            {
                Text = "Start Date:",
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };

            dtpStartDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };

            var lblEndDate = new Label
            {
                Text = "End Date:",
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };

            dtpEndDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };

            var lblBillId = new Label
            {
                Text = "Bill ID:",
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };

            txtBillId = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };

            // Second row: customer contact and buttons
            var lblCustomer = new Label
            {
                Text = "Customer Contact:",
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };

            txtCustomerContact = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };

            // Buttons will be added to the second row
            btnSearch = CreateButton("SEARCH", PrimaryColor, 100, 30);
            btnSearch.Click += BtnSearch_Click;

            btnClear = CreateButton("CLEAR", SecondaryColor, 100, 30);
            btnClear.Click += (s, e) => ClearSearch();

            // Add controls to search layout
            searchLayout.Controls.Add(lblStartDate, 0, 0);
            searchLayout.Controls.Add(dtpStartDate, 1, 0);
            searchLayout.Controls.Add(lblEndDate, 2, 0);
            searchLayout.Controls.Add(dtpEndDate, 3, 0);
            searchLayout.Controls.Add(lblBillId, 4, 0);
            searchLayout.Controls.Add(txtBillId, 5, 0);
            searchLayout.Controls.Add(lblCustomer, 6, 0);
            searchLayout.Controls.Add(txtCustomerContact, 7, 0);
            searchLayout.Controls.Add(btnSearch, 6, 1);
            searchLayout.Controls.Add(btnClear, 7, 1);

            searchPanel.Controls.Add(searchLayout);

            // Main content panel
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor
            };

            // Left panel - Bills list
            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 500,
                Padding = new Padding(0, 0, 20, 0)
            };

            // Create bills DataGridView
            dgvBills = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = BackgroundColor,
                BorderStyle = BorderStyle.Fixed3D,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                EnableHeadersVisualStyles = false,
                AutoGenerateColumns = false
            };

            // Grid styling
            dgvBills.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(0)
            };

            dgvBills.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BackgroundColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 10),
                SelectionBackColor = SelectionColor,
                SelectionForeColor = ForegroundColor
            };

            dgvBills.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 249, 255)
            };

            // Configure grid columns
            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Bill_ID",
                HeaderText = "Bill ID",
                Name = "Bill_ID",
                Width = 70
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BillDate",
                HeaderText = "Date",
                Width = 120
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PaymentMethod",
                HeaderText = "Payment",
                Width = 80
            });

            var amountColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NetTotal",
                HeaderText = "Amount",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                Width = 80
            };
            dgvBills.Columns.Add(amountColumn);

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CustomerContact",
                HeaderText = "Customer",
                Width = 150
            });

            // Right panel - Bill items
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Bill items section
            var itemsGroup = new GroupBox
            {
                Text = "BILL ITEMS",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                Padding = new Padding(10)
            };

            dgvBillItems = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = BackgroundColor,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Style bill items grid
            dgvBillItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            dgvBillItems.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BackgroundColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 9),
                SelectionBackColor = SelectionColor,
                SelectionForeColor = ForegroundColor
            };

            // Configure bill items columns
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Description",
                DataPropertyName = "description",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                DataPropertyName = "ItemSellingPrice",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                Width = 70
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                DataPropertyName = "quantity",
                Width = 50
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Discount",
                DataPropertyName = "Per_item_Discount",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                Width = 70
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Total",
                DataPropertyName = "NetPrice",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                Width = 80
            });

            itemsGroup.Controls.Add(dgvBillItems);

            // Controls panel
            var controlsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                Padding = new Padding(10)
            };

            // Create a table layout with 2 rows and 4 columns
            var controlTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            // Configure column widths
            controlTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            controlTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            controlTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            controlTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

            // Configure row heights
            controlTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            controlTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            // Printer label (first row)
            var lblPrinter = new Label
            {
                Text = "Printer:",
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Right
            };
            controlTable.Controls.Add(lblPrinter, 0, 0);

            // Printer combo box (first row, spans 3 columns)
            cboPrinters = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 10, 5)
            };
            controlTable.Controls.Add(cboPrinters, 1, 0);
            controlTable.SetColumnSpan(cboPrinters, 3);

            // Reprint button (second row, column 1)
            btnReprint = CreateButton("REPRINT BILL", PrimaryColor, 0, 0);
            btnReprint.Dock = DockStyle.Fill;
            btnReprint.Click += BtnReprint_Click;
            controlTable.Controls.Add(btnReprint, 1, 1);

            // Test print button (second row, column 2)
            btnDummyPrint = CreateButton("TEST", Color.Teal, 0, 0);
            btnDummyPrint.Dock = DockStyle.Fill;
            btnDummyPrint.Click += BtnDummyPrint_Click;
            controlTable.Controls.Add(btnDummyPrint, 2, 1);

            // Add the table to controls panel
            controlsPanel.Controls.Add(controlTable);

            // Build panels
            leftPanel.Controls.Add(dgvBills);
            rightPanel.Controls.Add(itemsGroup);
            rightPanel.Controls.Add(controlsPanel);
            titlePanel.Controls.Add(lblTitle);
            contentPanel.Controls.Add(rightPanel);
            contentPanel.Controls.Add(leftPanel);
            container.Controls.Add(contentPanel);
            container.Controls.Add(searchPanel);
            container.Controls.Add(titlePanel);
            this.Controls.Add(container);

            // Initialize data
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Today;
            LoadPrinters();

            // Add selection changed event
            dgvBills.SelectionChanged += DgvBills_SelectionChanged;
        }

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
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5, 0, 5, 0),
                Cursor = Cursors.Hand
            };
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

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                // Parse parameters
                int? billId = null;
                if (int.TryParse(txtBillId.Text, out int parsedBillId) && parsedBillId > 0)
                {
                    billId = parsedBillId;
                }

                string customerContact = txtCustomerContact.Text.Trim();
                if (string.IsNullOrWhiteSpace(customerContact))
                {
                    customerContact = null;
                }

                // Use service layer to search bills
                var result = _billService.SearchBills(billId, dtpStartDate.Value.Date, dtpEndDate.Value.Date, customerContact);
                dgvBills.DataSource = result;

                if (result.Rows.Count == 0)
                {
                    ThemedMessageBox.Show("No bills found matching the criteria", "Search Results",
                        ThemedMessageBoxIcon.Warning);
                    btnReprint.Enabled = false;
                    dgvBillItems.DataSource = null;
                }
                else
                {
                    // Automatically select first row if only one bill found
                    if (result.Rows.Count == 1)
                    {
                        dgvBills.Rows[0].Selected = true;
                        DgvBills_SelectionChanged(null, null);
                    }
                    btnReprint.Enabled = result.Rows.Count > 0;
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show($"Error searching bills: {ex.Message}", "Database Error",
                    ThemedMessageBoxIcon.Warning);
            }
        }

        private void ClearSearch()
        {
            txtBillId.Clear();
            txtCustomerContact.Clear();
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Today;
            dgvBills.DataSource = null;
            dgvBillItems.DataSource = null;
            btnReprint.Enabled = false;
        }

        private void DgvBills_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBills.SelectedRows.Count > 0)
            {
                _selectedBillId = Convert.ToInt32(dgvBills.SelectedRows[0].Cells["Bill_ID"].Value);
                LoadBillData();
                btnReprint.Enabled = true;
            }
            else
            {
                btnReprint.Enabled = false;
                dgvBillItems.DataSource = null;
            }
        }

        private void LoadBillData()
        {
            try
            {
                // Get bill summary from service layer
                _currentBillSummary = _billService.GetBillSummary(_selectedBillId);

                if (_currentBillSummary?.Header == null)
                {
                    MessageBox.Show("Bill not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Set header properties
                var header = _currentBillSummary.Header;
                _billDate = header.BillDate;
                _paymentMethod = header.PaymentMethod;
                _billDiscount = header.DiscountMethod == "TotalBill" ? header.BillDiscount : 0;
                _customerContact = header.CustomerContact;
                _cardLast4 = header.CardLast4;
                _bankLast4 = header.BankLast4;
                _tokenValue = header.TokenValue;
                _cashierName = header.CashierName;

                // Set calculated totals
                _subtotal = _currentBillSummary.Subtotal;
                _totalPerItemDiscount = _currentBillSummary.TotalPerItemDiscount;
                _total = _currentBillSummary.Total;

                // Update UI - get DataTable for grid binding
                _billItems = _billService.GetBillItemsForDisplay(_selectedBillId);
                dgvBillItems.DataSource = _billItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bill: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReprint_Click(object sender, EventArgs e)
        {
            if (dgvBills.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a bill to reprint", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintBill();
        }

        private void PrintBill()
        {
            try
            {
                var printerName = cboPrinters.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(printerName))
                {
                    MessageBox.Show("Please select a printer", "Printer Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create receipt content
                List<byte> output = new List<byte>();
                output.AddRange(new byte[] { 0x1B, 0x40 }); // Initialize printer

                // Shop details - CENTERED
                PrintCentered("STYLE NEWAGE", output);
                PrintCentered("No.102, Negombo Rd, Narammala.", output);
                PrintCentered("Tel: 0777491913 / 0374545097", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Reprint notice
                PrintCentered("** REPRINT **", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Bill header - CENTERED
                PrintCentered($"BILL #: {_selectedBillId}", output);
                PrintCentered($"ORIGINAL DATE: {_billDate:yyyy-MM-dd HH:mm}", output);
                PrintCentered($"REPRINT DATE: {DateTime.Now:yyyy-MM-dd HH:mm}", output);
                PrintCentered($"Cashier: {_cashierName}", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));
                PrintSeparator(output);

                // Customer contact if available
                if (!string.IsNullOrEmpty(_customerContact))
                {
                    PrintCentered($"Customer: {_customerContact}", output);
                    PrintSeparator(output);
                }

                // Column headers
                PrintLeftRight("ITEM", "QTY  PRICE  TOTAL", output);
                PrintSeparator(output);

                // Print items
                foreach (DataRow row in _billItems.Rows)
                {
                    string description = row["description"].ToString();
                    decimal price = Convert.ToDecimal(row["ItemSellingPrice"]);
                    int qty = Convert.ToInt32(row["quantity"]);
                    decimal discountPerItem = Convert.ToDecimal(row["Per_item_Discount"]);
                    decimal netPrice = Convert.ToDecimal(row["NetPrice"]);
                    decimal lineTotal = price * qty;

                    // Item description
                    PrintLeft($"{description}", output);

                    // Pricing details
                    string priceDetails = $"{qty} x {price:0.00} = {lineTotal:0.00}";
                    PrintLeftRight("", priceDetails, output);

                    // Discount information if applicable
                    if (discountPerItem > 0)
                    {
                        string discountLine = $"Discount: -{discountPerItem:0.00} per item";
                        PrintLeftRight("", discountLine, output);
                        PrintLeftRight("", $"Net: {netPrice:0.00}", output);
                    }

                    output.AddRange(Encoding.ASCII.GetBytes("\n"));
                }

                PrintSeparator(output);

                // Discount summary
                PrintLeftRight("SUB TOTAL:", $"{_subtotal:0.00}", output);

                if (_totalPerItemDiscount > 0)
                {
                    PrintLeftRight("ITEM DISCOUNTS:", $"-{_totalPerItemDiscount:0.00}", output);
                }

                if (_billDiscount > 0)
                {
                    PrintLeftRight($"BILL DISCOUNT:", $"-{_billDiscount:0.00}", output);
                }

                PrintLeftRight("TOTAL:", $"{_total:0.00}", output);
                PrintSeparator(output);

                // Token information
                if (_tokenValue.HasValue)
                {
                    PrintLeftRight("RETURN VALUE:", $"{_tokenValue.Value:0.00}", output);
                }

                // Payment details
                if (_paymentMethod == "Cash")
                {
                    PrintLeftRight("PAYMENT METHOD:", "CASH", output);
                    PrintLeftRight("AMOUNT PAID:", $"{_total:0.00}", output);
                }
                else if (_paymentMethod == "Card")
                {
                    PrintLeftRight("PAYMENT METHOD:", "CARD", output);
                    PrintLeftRight("AMOUNT PAID:", $"{_total:0.00}", output);
                    PrintLeftRight("LAST 4 DIGITS:", _cardLast4, output);
                }
                else if (_paymentMethod == "Bank Transfer")
                {
                    PrintLeftRight("PAYMENT METHOD:", "BANK TRANSFER", output);
                    PrintLeftRight("AMOUNT PAID:", $"{_total:0.00}", output);
                    PrintLeftRight("LAST 4 DIGITS:", _bankLast4, output);
                }
                else if (_paymentMethod == "Token")
                {
                    PrintLeftRight("PAYMENT METHOD:", "TOKEN", output);
                    PrintLeftRight("TOKEN VALUE:", $"{_tokenValue:0.00}", output);
                }

                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Return policy
                PrintCentered("-------RETURN POLICY-------", output);
                PrintCentered("Returns accepted in 3 days with", output);
                PrintCentered("original tag & receipt.", output);
                PrintCentered("No cash refunds on returns.", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n\n\n"));

                // Printer commands
                output.AddRange(new byte[] { 0x1B, 0x64, 0x02 }); // Feed 2 lines
                output.AddRange(new byte[] { 0x1B, 0x69 }); // Cut paper

                // Send to printer
                RawPrinterHelper.SendBytesToPrinter(printerName, output.ToArray());

                MessageBox.Show("Bill reprinted successfully!", "Reprint Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Printing failed: {ex.Message}", "Print Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDummyPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var printerName = cboPrinters.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(printerName))
                {
                    MessageBox.Show("Please select a printer", "Printer Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create a test receipt
                List<byte> output = new List<byte>();
                output.AddRange(new byte[] { 0x1B, 0x40 }); // Initialize printer

                // Shop details
                PrintCentered("STYLE NEWAGE", output);
                PrintCentered("TEST PRINT", output);
                PrintCentered(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));
                PrintSeparator(output);

                // Test content
                PrintLeft("This is a test print", output);
                PrintLeft("from the BillPrints module", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                PrintLeft("Printer:", output);
                PrintLeft(printerName, output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                PrintLeft("Printing successful!", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n\n\n"));

                // Printer commands
                output.AddRange(new byte[] { 0x1B, 0x64, 0x02 }); // Feed 2 lines
                output.AddRange(new byte[] { 0x1B, 0x69 }); // Cut paper

                // Send to printer
                RawPrinterHelper.SendBytesToPrinter(printerName, output.ToArray());

                MessageBox.Show("Test print sent successfully!", "Test Print",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Test print failed: {ex.Message}", "Print Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private const int MAX_LINE_WIDTH = 32;

        private void PrintCentered(string text, List<byte> output)
        {
            if (text.Length > MAX_LINE_WIDTH)
                text = text.Substring(0, MAX_LINE_WIDTH);

            int spaces = (MAX_LINE_WIDTH - text.Length) / 2;
            output.AddRange(Encoding.ASCII.GetBytes(new string(' ', spaces) + text + "\n"));
        }

        private void PrintLeft(string text, List<byte> output)
        {
            text = text.Length > MAX_LINE_WIDTH ? text.Substring(0, MAX_LINE_WIDTH) : text;
            output.AddRange(Encoding.ASCII.GetBytes(text + "\n"));
        }

        private void PrintLeftRight(string left, string right, List<byte> output)
        {
            int availableSpace = MAX_LINE_WIDTH - right.Length - 1;
            left = left.Length > availableSpace ? left.Substring(0, availableSpace) : left;
            string line = left.PadRight(availableSpace) + " " + right;
            output.AddRange(Encoding.ASCII.GetBytes(line + "\n"));
        }

        private void PrintSeparator(List<byte> output)
        {
            output.AddRange(Encoding.ASCII.GetBytes(new string('-', MAX_LINE_WIDTH) + "\n"));
        }
    }
}