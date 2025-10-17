using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Services;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class ReturnChecking : Form
    {
        // Theme colors matching Bills form
        private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
        private static readonly Color ForegroundColor = Color.Black;
        private static readonly Color SecondaryColor = Color.Gray;
        private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

        // Form controls
        private DataGridView dgvReturns;
        private DataGridView dgvReturnItems;
        private TextBox txtReturnId;
        private TextBox txtBillId;
        private ComboBox cmbIsUsed;
        private DateTimePicker dtpReturnDate;
        private CheckBox chkUseDate;
        private Button btnSearch;
        private Button btnClear;
        private Button btnReprintToken;
        private Panel container;
        private TableLayoutPanel mainContentLayout;

        public ReturnChecking()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void InitializeComponent()
        {
            // Form setup
            this.Size = new Size(1200, 850);
            this.Text = "Return Checking";
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
                Text = "RETURN CHECKING",
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
                Padding = new Padding(0, 5, 20, 5),
                Margin = new Padding(0, 0, 0, 5)
            };

            // Filter controls in table layout
            var filterTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 8,
                RowCount = 2,
                AutoSize = true
            };

            // Configure columns
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));

            // Row heights
            filterTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            filterTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            // Return ID
            var lblReturnId = new Label
            {
                Text = "Return ID:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ForegroundColor
            };

            txtReturnId = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 10, 10, 0)
            };
            txtReturnId.KeyDown += TextBox_KeyDown;

            // Bill ID
            var lblBillId = new Label
            {
                Text = "Bill ID:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ForegroundColor
            };

            txtBillId = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 10, 10, 0)
            };
            txtBillId.KeyDown += TextBox_KeyDown;

            // Is Used filter
            var lblIsUsed = new Label
            {
                Text = "Token Used:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ForegroundColor
            };

            cmbIsUsed = new ComboBox
            {
                Margin = new Padding(0, 10, 10, 0),
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbIsUsed.Items.AddRange(new object[] { "All", "Used", "Not Used" });
            cmbIsUsed.SelectedIndex = 0;

            // Return Date
            var lblReturnDate = new Label
            {
                Text = "Return Date:",
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

            dtpReturnDate = new DateTimePicker
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

            datePanel.Controls.Add(dtpReturnDate);
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
            filterTable.Controls.Add(lblReturnId, 0, 0);
            filterTable.Controls.Add(txtReturnId, 1, 0);
            filterTable.Controls.Add(lblBillId, 2, 0);
            filterTable.Controls.Add(txtBillId, 3, 0);
            filterTable.Controls.Add(lblIsUsed, 4, 0);
            filterTable.Controls.Add(cmbIsUsed, 5, 0);
            filterTable.Controls.Add(lblReturnDate, 6, 0);
            filterTable.Controls.Add(datePanel, 7, 0);
            filterTable.Controls.Add(buttonPanel, 7, 1);

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
            mainContentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainContentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            mainContentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));

            // Data Grid for Returns
            var returnsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0, 0, 0, 5)
            };

            dgvReturns = new DataGridView
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

            FormatDataGrid(dgvReturns);

            // Configure returns grid columns
            dgvReturns.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Return_ID",
                HeaderText = "RETURN ID",
                Name = "Return_ID",
                Width = 80
            });

            dgvReturns.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OriginalBill_ID",
                HeaderText = "BILL ID",
                Name = "OriginalBill_ID",
                Width = 70
            });

            dgvReturns.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ReturnDate",
                HeaderText = "RETURN DATE",
                Name = "ReturnDate",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd-MMM-yyyy HH:mm"
                },
                Width = 120
            });

            dgvReturns.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalRefund",
                HeaderText = "REFUND AMOUNT",
                Name = "TotalRefund",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                Width = 100
            });

            dgvReturns.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "IsUsed",
                HeaderText = "TOKEN USED",
                Name = "IsUsed",
                Width = 80
            });

            dgvReturns.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UsedInBill_ID",
                HeaderText = "USED IN BILL",
                Name = "UsedInBill_ID",
                Width = 80
            });

            dgvReturns.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "EmployeeName",
                HeaderText = "PROCESSED BY",
                Name = "EmployeeName",
                Width = 150
            });

            returnsPanel.Controls.Add(dgvReturns);
            mainContentLayout.Controls.Add(returnsPanel, 0, 0);

            // Return items grid
            var itemsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0, 0, 0, 5)
            };

            dgvReturnItems = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = BackgroundColor,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            FormatDataGrid(dgvReturnItems);

            // Configure return items columns
            dgvReturnItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ITEM DESCRIPTION",
                DataPropertyName = "Description",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvReturnItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "BRAND",
                DataPropertyName = "BrandName",
                Width = 100
            });

            dgvReturnItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "CATEGORY",
                DataPropertyName = "CategoryName",
                Width = 100
            });

            dgvReturnItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "SIZE",
                DataPropertyName = "SizeLabel",
                Width = 80
            });

            dgvReturnItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "QUANTITY",
                DataPropertyName = "Quantity",
                Width = 70,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvReturnItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "REFUND AMOUNT",
                DataPropertyName = "RefundAmount",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                Width = 100
            });

            dgvReturnItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "REASON",
                DataPropertyName = "ReasonDescription",
                Width = 120
            });

            dgvReturnItems.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "RESTOCKED",
                DataPropertyName = "IsRestocked",
                Width = 80
            });

            itemsPanel.Controls.Add(dgvReturnItems);
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

            btnReprintToken = CreateButton("REPRINT TOKEN", PrimaryColor);
            btnReprintToken.Height = 40;
            btnReprintToken.Width = 180;
            btnReprintToken.Enabled = false;
            btnReprintToken.Click += BtnReprintToken_Click;
            actionLayout.Controls.Add(btnReprintToken);

            actionPanel.Controls.Add(actionLayout);
            mainContentLayout.Controls.Add(actionPanel, 0, 2);

            // Assemble container
            container.Controls.Add(mainContentLayout);
            container.Controls.Add(filterPanel);
            container.Controls.Add(titlePanel);
            this.Controls.Add(container);

            // Handle return selection
            dgvReturns.SelectionChanged += DgvReturns_SelectionChanged;
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
            dtpReturnDate.Enabled = chkUseDate.Checked;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchReturns();
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchReturns();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtReturnId.Clear();
            txtBillId.Clear();
            cmbIsUsed.SelectedIndex = 0;
            chkUseDate.Checked = false;

            // Create empty DataTables with the same structure instead of setting to null
            ClearDataGridViews();
            btnReprintToken.Enabled = false;
        }

        private void ClearDataGridViews()
        {
            // For dgvReturns - create empty DataTable with same structure
            if (dgvReturns != null)
            {
                var emptyReturnsTable = new DataTable();
                // Add columns to match the expected structure
                emptyReturnsTable.Columns.Add("Return_ID", typeof(int));
                emptyReturnsTable.Columns.Add("OriginalBill_ID", typeof(int));
                emptyReturnsTable.Columns.Add("ReturnDate", typeof(DateTime));
                emptyReturnsTable.Columns.Add("TotalRefund", typeof(decimal));
                emptyReturnsTable.Columns.Add("IsUsed", typeof(bool));
                emptyReturnsTable.Columns.Add("UsedInBill_ID", typeof(int));
                emptyReturnsTable.Columns.Add("EmployeeName", typeof(string));

                dgvReturns.DataSource = emptyReturnsTable;
            }

            // For dgvReturnItems - create empty DataTable with same structure
            if (dgvReturnItems != null)
            {
                var emptyItemsTable = new DataTable();
                // Add columns to match the expected structure
                emptyItemsTable.Columns.Add("Description", typeof(string));
                emptyItemsTable.Columns.Add("BrandName", typeof(string));
                emptyItemsTable.Columns.Add("CategoryName", typeof(string));
                emptyItemsTable.Columns.Add("SizeLabel", typeof(string));
                emptyItemsTable.Columns.Add("Quantity", typeof(int));
                emptyItemsTable.Columns.Add("RefundAmount", typeof(decimal));
                emptyItemsTable.Columns.Add("ReasonDescription", typeof(string));
                emptyItemsTable.Columns.Add("IsRestocked", typeof(bool));

                dgvReturnItems.DataSource = emptyItemsTable;
            }
        }


        private void SearchReturns()
        {
            try
            {
                int? returnId = null;
                if (!string.IsNullOrWhiteSpace(txtReturnId.Text))
                {
                    if (int.TryParse(txtReturnId.Text, out int tempReturnId))
                    {
                        returnId = tempReturnId;
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid Return ID", "Invalid Input",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                int? billId = null;
                if (!string.IsNullOrWhiteSpace(txtBillId.Text))
                {
                    if (int.TryParse(txtBillId.Text, out int tempBillId))
                    {
                        billId = tempBillId;
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid Bill ID", "Invalid Input",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                bool? isUsed = null;
                if (cmbIsUsed.SelectedIndex == 1) // Used
                    isUsed = true;
                else if (cmbIsUsed.SelectedIndex == 2) // Not Used
                    isUsed = false;

                DateTime? returnDate = null;
                if (chkUseDate.Checked)
                {
                    returnDate = dtpReturnDate.Value.Date;
                }

                var returnService = new ReturnService();
                var returns = returnService.GetReturns(returnId, billId, isUsed, returnDate);

                dgvReturns.DataSource = returns;

                if (returns.Rows.Count == 0)
                {
                    btnReprintToken.Enabled = false;
                    dgvReturnItems.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching returns: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvReturns_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReturns.SelectedRows.Count > 0)
            {
                int returnId = Convert.ToInt32(dgvReturns.SelectedRows[0].Cells["Return_ID"].Value);
                LoadReturnItems(returnId);
                btnReprintToken.Enabled = true;
            }
            else
            {
                btnReprintToken.Enabled = false;
            }
        }

        private void LoadReturnItems(int returnId)
        {
            try
            {
                var returnService = new ReturnService();
                var returnItems = returnService.GetReturnItems(returnId);
                dgvReturnItems.DataSource = returnItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading return items: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReprintToken_Click(object sender, EventArgs e)
        {
            if (dgvReturns.SelectedRows.Count > 0)
            {
                int returnId = Convert.ToInt32(dgvReturns.SelectedRows[0].Cells["Return_ID"].Value);
                ReprintReturnToken(returnId);
            }
        }

        private void ReprintReturnToken(int returnId)
        {
            try
            {
                var returnService = new ReturnService();
                var receiptData = returnService.GetReturnReceiptData(returnId);

                // Use the same printing logic from ReturnsForm
                PrintReturnReceipt(receiptData);

                MessageBox.Show($"Return token reprinted successfully!\nReturn ID: {receiptData.ReturnId}",
                    "Reprint Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reprinting token: {ex.Message}", "Print Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintReturnReceipt(ReturnReceiptData receiptData)
        {
            try
            {
                const string PRINTER_NAME = "XP-80C";
                if (string.IsNullOrEmpty(PRINTER_NAME))
                {
                    MessageBox.Show("No receipt printer found. Please configure a printer.", "Print Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<byte> output = new List<byte>();
                output.AddRange(new byte[] { 0x1B, 0x40 }); // Initialize printer

                // Print header
                PrintCentered("RETURN TOKEN REPRINT", output, true);
                PrintCentered("STYLE NEWAGE", output);
                PrintCentered("No.102, Negombo Rd, Narammala.", output);
                PrintCentered("Tel: 0777491913 / 0374545097", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Print return details
                PrintCentered($"RETURN ID: {receiptData.ReturnId}", output, true);
                PrintCentered($"ORIGINAL BILL: {receiptData.OriginalBillId}", output);
                PrintCentered($"BILL DATE: {receiptData.BillDate:yyyy-MM-dd}", output);
                PrintCentered($"RETURN DATE: {receiptData.ReturnDate:yyyy-MM-dd}", output);
                PrintCentered($"REPRINT DATE: {DateTime.Now:yyyy-MM-dd HH:mm}", output);
                PrintCentered($"CASHIER: {receiptData.CashierName}", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));
                PrintSeparator(output);

                // Print items header
                PrintLeftRight("ITEM", "QTY  REFUND", output);
                PrintSeparator(output);

                // Print items
                foreach (var item in receiptData.Items)
                {
                    PrintLeft($"{item.Category}", output);
                    if (!string.IsNullOrEmpty(item.Size))
                        PrintLeft($"Size: {item.Size}", output);

                    PrintLeftRight(string.Empty, $"{item.Quantity}   {item.Refund:N2}", output);
                    output.AddRange(Encoding.ASCII.GetBytes("\n"));
                }

                // Print footer
                PrintSeparator(output);
                PrintLeftRight("TOTAL REFUND:", receiptData.TotalRefund.ToString("N2"), output, true);

                // Token usage status
                output.AddRange(Encoding.ASCII.GetBytes("\n"));
                bool isUsed = CheckIfTokenUsed(receiptData.ReturnId);
                if (isUsed)
                {
                    PrintCentered("*** TOKEN ALREADY USED ***", output, true);
                    int usedInBill = GetTokenUsageBill(receiptData.ReturnId);
                    if (usedInBill > 0)
                    {
                        PrintCentered($"Used in Bill: {usedInBill}", output);
                    }
                }
                else
                {
                    PrintCentered("*** TOKEN VALID FOR USE ***", output, true);
                }

                output.AddRange(Encoding.ASCII.GetBytes("\n"));
                PrintCentered("Thank you for your business!", output);
                PrintCentered("Items must be returned with tags", output);
                PrintCentered("within 3 days for exchange only", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n\n\n"));

                // Printer commands
                output.AddRange(new byte[] { 0x1B, 0x64, 0x02 }); // Feed 2 lines
                output.AddRange(new byte[] { 0x1B, 0x69 }); // Cut paper

                // Send to printer using the same RawPrinterHelper from ReturnsForm
                ReturnsForm.RawPrinterHelper.SendBytesToPrinter(PRINTER_NAME, output.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool CheckIfTokenUsed(int returnId)
        {
            string query = "SELECT COUNT(*) FROM Bill WHERE Token_ReturnID = @ReturnId";
            SqlParameter[] parameters = { new SqlParameter("@ReturnId", returnId) };
            var result = DbHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return Convert.ToInt32(result) > 0;
        }

        private int GetTokenUsageBill(int returnId)
        {
            string query = "SELECT Bill_ID FROM Bill WHERE Token_ReturnID = @ReturnId";
            SqlParameter[] parameters = { new SqlParameter("@ReturnId", returnId) };
            var result = DbHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // Printing helpers (same as in ReturnsForm)
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

        private void PrintLeftRight(string left, string right, List<byte> output, bool bold = false)
        {
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
            output.AddRange(Encoding.ASCII.GetBytes(new string('-', MAX_LINE_WIDTH) + "\n"));
        }
    }
}
