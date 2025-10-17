using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using pos_system.pos.DAL;
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
using System.Configuration;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class Bills : Form
    {
        // Theme colors
        private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
        private static readonly Color ForegroundColor = Color.Black;
        private static readonly Color SecondaryColor = Color.Gray;
        private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

        // Form controls
        private DataGridView dgvBills;
        private DataGridView dgvBillItems;
        private TextBox txtBillNumber;
        private TextBox txtCustomerContact;
        private DateTimePicker dtpBillDate;
        private CheckBox chkUseDate;
        private Button btnSearch;
        private Button btnClear;
        private Button btnPrintBill;
        private Button btnExportContacts;
        private int _selectedBillId;
        private Panel container;
        private TableLayoutPanel mainContentLayout; // For layout management

        public Bills()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void InitializeComponent()
        {
            // Form setup
            this.Size = new Size(1200, 850);
            this.Text = "Bill Search";
            this.BackColor = BackgroundColor;
            this.Font = new Font("Segoe UI", 9);
            this.Padding = new Padding(20);

            // Main container
            container = new Panel
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
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Padding = new Padding(20, 0, 0, 0)
            };
            titlePanel.Controls.Add(lblTitle);

            // Filter panel
            var filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = HeaderColor,
                Padding = new Padding(20, 5, 20, 5),
                Margin = new Padding(0, 0, 0, 5)
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
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

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

            // Create main content layout
            mainContentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = BackgroundColor,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            // Configure rows:
            // - Bills grid: 60% of space
            // - Bill items grid: 30% of space
            // - Action panel: Fixed 60px
            mainContentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainContentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            mainContentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));

            // Data Grid for Bills
            var billsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0, 0, 0, 5)
            };

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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            };

            // Configure grid style
            FormatDataGrid(dgvBills);

            // Configure grid columns
            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Bill_ID",
                HeaderText = "BILL NO",
                Name = "Bill_ID",
                Width = 70
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BillDate",
                HeaderText = "DATE",
                Name = "BillDate",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd-MMM-yyyy HH:mm"
                }
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PaymentMethod",
                HeaderText = "PAYMENT",
                Name = "PaymentMethod",
                Width = 80
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CustomerContact",
                HeaderText = "CONTACT",
                Name = "CustomerContact",
                Width = 150
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ContactGender",
                HeaderText = "GENDER",
                Name = "ContactGender",
                Width = 80
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CashierName",
                HeaderText = "CASHIER",
                Name = "CashierName",
                Width = 150
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Subtotal",
                HeaderText = "SUBTOTAL",
                Name = "Subtotal",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                Width = 70
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NetTotal",
                HeaderText = "AMOUNT",
                Name = "NetTotal",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                Width = 70
            });

            dgvBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalPerItemDiscount",
                HeaderText = "TOTALDISCOUNT",
                Name = "TotalPerItemDiscount",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                Width = 70
            });

            billsPanel.Controls.Add(dgvBills);
            mainContentLayout.Controls.Add(billsPanel, 0, 0);

            // Bill items grid
            var itemsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0, 0, 0, 5)
            };

            dgvBillItems = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = BackgroundColor,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            FormatDataGrid(dgvBillItems);

            // Configure bill items columns
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "DESCRIPTION",
                DataPropertyName = "description",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "PRICE",
                DataPropertyName = "ItemSellingPrice",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                Width = 70
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "QTY",
                DataPropertyName = "quantity",
                Width = 50,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "DISCOUNT",
                DataPropertyName = "Per_item_Discount",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                Width = 70
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "SOLD PRICE",
                DataPropertyName = "ActualSellingPrice",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                Width = 80
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "SIZE",
                DataPropertyName = "SizeLabel",
                Width = 80
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "CATEGORY",
                DataPropertyName = "categoryName",
                Width = 80
            });

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "BRAND",
                DataPropertyName = "brandName",
                Width = 80
            });
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "BARCODE",
                DataPropertyName = "barcode",
                Width = 80
            });

            itemsPanel.Controls.Add(dgvBillItems);
            mainContentLayout.Controls.Add(itemsPanel, 0, 1);

            // Action buttons panel
            var actionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = HeaderColor,
                Padding = new Padding(10)
            };

            var actionLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            btnPrintBill = CreateButton("PRINT BILL", PrimaryColor);
            btnPrintBill.Height = 40;
            btnPrintBill.Width = 150;
            btnPrintBill.Enabled = false;
            btnPrintBill.Click += BtnPrintBill_Click;
            actionLayout.Controls.Add(btnPrintBill);

            btnExportContacts = CreateButton("EXPORT CONTACTS", Color.DarkGreen);
            btnExportContacts.Height = 40;
            btnExportContacts.Width = 180;
            btnExportContacts.Margin = new Padding(20, 0, 0, 0);
            btnExportContacts.Click += BtnExportContacts_Click;
            actionLayout.Controls.Add(btnExportContacts);

            actionPanel.Controls.Add(actionLayout);
            mainContentLayout.Controls.Add(actionPanel, 0, 2);

            // Assemble container
            container.Controls.Add(mainContentLayout);
            container.Controls.Add(filterPanel);
            container.Controls.Add(titlePanel);
            this.Controls.Add(container);

            // Handle bill selection
            dgvBills.SelectionChanged += DgvBills_SelectionChanged;
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
            dgv.RowTemplate.Height = 30;

            // Header styling
            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };
            dgv.ColumnHeadersHeight = 30;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Cell styling
            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BackgroundColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 10),
                SelectionBackColor = SelectionColor,
                SelectionForeColor = ForegroundColor,
                Padding = new Padding(2)
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

            // Clear data without affecting the grid structure
            ClearDataGridData();
            btnPrintBill.Enabled = false;
        }

        private void ClearDataGridData()
        {
            // For dgvBills - create empty DataTable with same structure
            if (dgvBills != null)
            {
                var emptyBillsTable = new DataTable();
                // Add columns to match the expected structure
                emptyBillsTable.Columns.Add("Bill_ID", typeof(int));
                emptyBillsTable.Columns.Add("BillDate", typeof(DateTime));
                emptyBillsTable.Columns.Add("PaymentMethod", typeof(string));
                emptyBillsTable.Columns.Add("CustomerContact", typeof(string));
                emptyBillsTable.Columns.Add("ContactGender", typeof(string));
                emptyBillsTable.Columns.Add("CashierName", typeof(string));
                emptyBillsTable.Columns.Add("Subtotal", typeof(decimal));
                emptyBillsTable.Columns.Add("NetTotal", typeof(decimal));
                emptyBillsTable.Columns.Add("TotalPerItemDiscount", typeof(decimal));

                dgvBills.DataSource = emptyBillsTable;
            }

            // For dgvBillItems - create empty DataTable with same structure
            if (dgvBillItems != null)
            {
                var emptyItemsTable = new DataTable();
                // Add columns to match the expected structure
                emptyItemsTable.Columns.Add("description", typeof(string));
                emptyItemsTable.Columns.Add("ItemSellingPrice", typeof(decimal));
                emptyItemsTable.Columns.Add("quantity", typeof(int));
                emptyItemsTable.Columns.Add("Per_item_Discount", typeof(decimal));
                emptyItemsTable.Columns.Add("ActualSellingPrice", typeof(decimal));
                emptyItemsTable.Columns.Add("SizeLabel", typeof(string));
                emptyItemsTable.Columns.Add("categoryName", typeof(string));
                emptyItemsTable.Columns.Add("brandName", typeof(string));
                emptyItemsTable.Columns.Add("barcode", typeof(string));

                dgvBillItems.DataSource = emptyItemsTable;
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

                        dgvBills.DataSource = dt;

                        if (dt.Rows.Count == 0)
                        {
                            btnPrintBill.Enabled = false;
                            dgvBillItems.DataSource = null;
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

        private void DgvBills_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBills.SelectedRows.Count > 0)
            {
                _selectedBillId = Convert.ToInt32(dgvBills.SelectedRows[0].Cells["Bill_ID"].Value);
                LoadBillItems();
                btnPrintBill.Enabled = true;
            }
            else
            {
                btnPrintBill.Enabled = false;
            }
        }

        private void LoadBillItems()
        {
            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetBillItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BillID", _selectedBillId);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvBillItems.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bill items: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrintBill_Click(object sender, EventArgs e)
        {
            PrintBill(_selectedBillId);
        }

        private void PrintBill(int billId)
        {
            try
            {
                string printerName = ConfigurationManager.AppSettings["ReceiptPrinter"];
                if (string.IsNullOrEmpty(printerName))
                {
                    MessageBox.Show("Receipt printer not configured", "Printer Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get bill header using the new stored procedure
                DataTable header = GetBillHeader(billId);
                if (header.Rows.Count == 0) return;
                DataRow bill = header.Rows[0];

                // Get bill items
                DataTable items = GetBillItems(billId);

                // Build receipt
                List<byte> output = new List<byte> { 0x1B, 0x40 }; // Init printer

                // Shop header
                PrintCentered("STYLE NEWAGE", output);
                PrintCentered("No.102, Negombo Rd, Narammala.", output);
                PrintCentered("Tel: 0777491913 / 0374545097", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Reprint notice
                PrintCentered("** REPRINT **", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Bill info
                PrintCentered($"BILL #: {billId}", output);
                PrintCentered($"ORIGINAL DATE: {Convert.ToDateTime(bill["BillDate"]):yyyy-MM-dd HH:mm}", output);
                PrintCentered($"REPRINT DATE: {DateTime.Now:yyyy-MM-dd HH:mm}", output);
                PrintCentered($"Cashier: {bill["CashierName"]}", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));
                PrintSeparator(output);

                // Customer info
                if (!string.IsNullOrEmpty(bill["CustomerContact"].ToString()))
                {
                    PrintCentered($"Customer: {bill["CustomerContact"]}", output);
                    PrintSeparator(output);
                }

                // Items header
                PrintLeftRight("ITEM", "QTY  PRICE  TOTAL", output);
                PrintSeparator(output);

                // Items
                decimal subtotal = 0;
                foreach (DataRow item in items.Rows)
                {
                    string description = item["description"].ToString();
                    decimal price = Convert.ToDecimal(item["ItemSellingPrice"]);
                    int qty = Convert.ToInt32(item["quantity"]);
                    decimal discount = Convert.ToDecimal(item["Per_item_Discount"]);
                    decimal netPrice = price - discount;
                    decimal lineTotal = netPrice * qty;
                    subtotal += lineTotal;

                    PrintLeft($"{description}", output);
                    PrintLeftRight("", $"{qty} x {price:0.00} = {lineTotal:0.00}", output);

                    if (discount > 0)
                    {
                        PrintLeftRight("", $"Discount: -{discount:0.00} per item", output);
                        PrintLeftRight("", $"Net: {netPrice:0.00}", output);
                    }
                    output.AddRange(Encoding.ASCII.GetBytes("\n"));
                }

                PrintSeparator(output);

                // Totals
                PrintLeftRight("SUB TOTAL:", $"{subtotal:0.00}", output);

                decimal totalDiscount = 0;
                string discountMethod = bill["Discount_Method"].ToString();
                if (discountMethod == "TotalBill")
                {
                    totalDiscount = Convert.ToDecimal(bill["BillDiscount"]);
                    PrintLeftRight("BILL DISCOUNT:", $"-{totalDiscount:0.00}", output);
                }

                decimal netTotal = subtotal - totalDiscount;
                PrintLeftRight("TOTAL:", $"{netTotal:0.00}", output);
                PrintSeparator(output);

                // Token information
                if (bill["Token_ReturnID"] != DBNull.Value && bill["TokenValue"] != DBNull.Value)
                {
                    PrintLeftRight("RETURN VALUE:", $"{Convert.ToDecimal(bill["TokenValue"]):0.00}", output);
                }

                // Payment details
                string paymentMethod = bill["PaymentMethod"].ToString();
                if (paymentMethod == "Cash")
                {
                    PrintLeftRight("PAYMENT METHOD:", "CASH", output);
                    PrintLeftRight("AMOUNT PAID:", $"{netTotal:0.00}", output);
                }
                else if (paymentMethod == "Card")
                {
                    PrintLeftRight("PAYMENT METHOD:", "CARD", output);
                    PrintLeftRight("AMOUNT PAID:", $"{netTotal:0.00}", output);
                    if (bill["CardLast4"] != DBNull.Value)
                    {
                        PrintLeftRight("LAST 4 DIGITS:", bill["CardLast4"].ToString(), output);
                    }
                }
                else if (paymentMethod == "Bank Transfer")
                {
                    PrintLeftRight("PAYMENT METHOD:", "BANK TRANSFER", output);
                    PrintLeftRight("AMOUNT PAID:", $"{netTotal:0.00}", output);
                    if (bill["BankAccountLast4"] != DBNull.Value)
                    {
                        PrintLeftRight("LAST 4 DIGITS:", bill["BankAccountLast4"].ToString(), output);
                    }
                }
                else if (paymentMethod == "Token")
                {
                    PrintLeftRight("PAYMENT METHOD:", "TOKEN", output);
                    PrintLeftRight("TOKEN VALUE:", $"{Convert.ToDecimal(bill["TokenValue"]):0.00}", output);
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

        private DataTable GetBillHeader(int billId)
        {
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetBillForPrinting", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BillID", billId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private DataTable GetBillItems(int billId)
        {
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetBillItems", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BillID", billId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        //private void BtnExportContacts_Click(object sender, EventArgs e)
        //{
        //    if (dgvBills.DataSource == null || dgvBills.Rows.Count == 0)
        //    {
        //        MessageBox.Show("No bills to export", "Export Error",
        //            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    using (SaveFileDialog sfd = new SaveFileDialog())
        //    {
        //        sfd.Filter = "CSV files (*.csv)|*.csv";
        //        sfd.Title = "Export Customer Contacts";
        //        sfd.FileName = $"CustomerContacts_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        //        if (sfd.ShowDialog() == DialogResult.OK)
        //        {
        //            try
        //            {
        //                DataTable dt = (DataTable)dgvBills.DataSource;
        //                var contacts = dt.AsEnumerable()
        //                    .Select(row => row.Field<string>("CustomerContact"))
        //                    .Where(contact => !string.IsNullOrWhiteSpace(contact))
        //                    .Distinct()
        //                    .ToList();

        //                using (StreamWriter sw = new StreamWriter(sfd.FileName))
        //                {
        //                    sw.WriteLine("CustomerContact");
        //                    foreach (string contact in contacts)
        //                    {
        //                        sw.WriteLine(contact);
        //                    }
        //                }

        //                MessageBox.Show($"{contacts.Count} contacts exported", "Export Complete",
        //                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Export failed: {ex.Message}", "Export Error",
        //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //        }
        //    }
        //}

        private void BtnExportContacts_Click(object sender, EventArgs e)
        {
            if (dgvBills.DataSource == null || dgvBills.Rows.Count == 0)
            {
                MessageBox.Show("No bills to export", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                sfd.Title = "Export Customer Contacts";
                sfd.FileName = $"CustomerContacts_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DataTable dt = (DataTable)dgvBills.DataSource;

                        // FILTER CONTACTS BY GENDER - PLACE THIS CODE HERE
                        var maleContacts = dt.AsEnumerable()
                            .Where(row => row.Field<string>("ContactGender") == "Male")
                            .Select(row => row.Field<string>("CustomerContact"))
                            .Where(contact => !string.IsNullOrWhiteSpace(contact))
                            .Distinct()
                            .ToList();

                        var femaleContacts = dt.AsEnumerable()
                            .Where(row => row.Field<string>("ContactGender") == "Female")
                            .Select(row => row.Field<string>("CustomerContact"))
                            .Where(contact => !string.IsNullOrWhiteSpace(contact))
                            .Distinct()
                            .ToList();

                        var unknownContacts = dt.AsEnumerable()
                            .Where(row => string.IsNullOrEmpty(row.Field<string>("ContactGender")))
                            .Select(row => row.Field<string>("CustomerContact"))
                            .Where(contact => !string.IsNullOrWhiteSpace(contact))
                            .Distinct()
                            .ToList();
                        // END OF FILTERING CODE

                        // Create a new Excel workbook
                        using (var workbook = new XLWorkbook())
                        {
                            // Create sheets for each gender
                            var maleSheet = workbook.Worksheets.Add("Male");
                            var femaleSheet = workbook.Worksheets.Add("Female");
                            var unknownSheet = workbook.Worksheets.Add("Unknown");

                            // Add headers to each sheet
                            maleSheet.Cell(1, 1).Value = "CustomerContact";
                            femaleSheet.Cell(1, 1).Value = "CustomerContact";
                            unknownSheet.Cell(1, 1).Value = "CustomerContact";

                            // Style headers
                            foreach (var worksheet in workbook.Worksheets)
                            {
                                worksheet.Row(1).Style.Font.Bold = true;
                                worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;
                            }

                            // ADD DATA TO SHEETS - PLACE THIS CODE HERE
                            for (int i = 0; i < maleContacts.Count; i++)
                            {
                                maleSheet.Cell(i + 2, 1).Value = maleContacts[i];
                            }

                            for (int i = 0; i < femaleContacts.Count; i++)
                            {
                                femaleSheet.Cell(i + 2, 1).Value = femaleContacts[i];
                            }

                            for (int i = 0; i < unknownContacts.Count; i++)
                            {
                                unknownSheet.Cell(i + 2, 1).Value = unknownContacts[i];
                            }
                            // END OF DATA ADDITION CODE

                            // Auto-fit columns
                            maleSheet.Columns().AdjustToContents();
                            femaleSheet.Columns().AdjustToContents();
                            unknownSheet.Columns().AdjustToContents();

                            // Save the workbook
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show($"Exported {maleContacts.Count} male, {femaleContacts.Count} female, and {unknownContacts.Count} unknown contacts.", "Export Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed: {ex.Message}", "Export Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Printing helpers
        private const int MAX_LINE_WIDTH = 32;
        private void PrintCentered(string text, List<byte> output, bool bold = false)
        {
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
            text = text.Length > MAX_LINE_WIDTH ? text.Substring(0, MAX_LINE_WIDTH) : text;
            output.AddRange(Encoding.ASCII.GetBytes(text + "\n"));
        }

        private void PrintLeftRight(string left, string right, List<byte> output)
        {
            int availableSpace = MAX_LINE_WIDTH - right.Length - 1;
            if (availableSpace < 1) availableSpace = 1;

            left = left.Length > availableSpace ? left.Substring(0, availableSpace) : left;
            string line = left.PadRight(availableSpace) + " " + right;
            output.AddRange(Encoding.ASCII.GetBytes(line + "\n"));
        }

        private void PrintSeparator(List<byte> output)
        {
            output.AddRange(Encoding.ASCII.GetBytes(new string('-', MAX_LINE_WIDTH) + "\n"));
        }

        // Raw Printer Helper Class
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
                di.pDocName = "POS Receipt";
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
    }
}
