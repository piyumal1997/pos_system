using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Services;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class ReturnsForm : Form
    {
        private Employee _currentUser;
        private DataTable _billItems;
        private DataTable _returnReasons;
        private int _lastReturnId;
        private decimal _lastTotalRefund;
        private DataTable _lastReturnItems;
        private DateTime _billDate;
        private bool _billExpired = false;

        public ReturnsForm(Employee currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            InitializeReturnReasons();
            lblBillDate.Text = string.Empty;
            lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void InitializeReturnReasons()
        {
            try
            {
                string query = "SELECT Reason_ID, Description FROM ReturnReason";
                _returnReasons = DbHelper.GetDataTable(query, CommandType.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading return reasons: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearchBill_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtBillId.Text, out int billId))
                {
                    MessageBox.Show("Please enter a valid Bill ID", "Input Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string billDateQuery = @"SELECT [date] FROM Bill WHERE Bill_ID = @BillId";
                SqlParameter[] billDateParams = { new SqlParameter("@BillId", billId) };
                DataTable billDateTable = DbHelper.GetDataTable(billDateQuery, CommandType.Text, billDateParams);

                if (billDateTable.Rows.Count == 0)
                {
                    MessageBox.Show("Bill not found", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _billDate = Convert.ToDateTime(billDateTable.Rows[0]["date"]);
                lblBillDate.Text = _billDate.ToString("yyyy-MM-dd");

                TimeSpan difference = DateTime.Now - _billDate;
                _billExpired = difference.TotalDays > 3;

                if (_billExpired)
                {
                    MessageBox.Show($"This bill is from {_billDate:yyyy-MM-dd} and is more than 3 days old. Returns are not allowed.",
                        "Bill Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnProcessReturn.Enabled = false;
                    return;
                }

                string query = @"SELECT 
                    bi.Item_ID, 
                    i.description AS ItemName,
                    b.brandName AS Brand,
                    c.categoryName AS Category,
                    s.SizeLabel,
                    bi.quantity AS OriginalQty,
                    ISNULL(bi.ReturnedQuantity, 0) AS ReturnedQty,
                    (bi.quantity - ISNULL(bi.ReturnedQuantity, 0)) AS AvailableQty,
                    bi.ItemSellingPrice AS Price
                FROM Bill_Item bi
                JOIN Bill bl ON bi.Bill_ID = bl.Bill_ID
                JOIN Item i ON bi.Item_ID = i.Item_ID
                LEFT JOIN Size s ON bi.Size_ID = s.Size_ID
                LEFT JOIN Brand b ON i.Brand_ID = b.Brand_ID
                LEFT JOIN Category c ON i.Category_ID = c.Category_ID
                WHERE bi.Bill_ID = @BillId
                AND (bi.quantity - ISNULL(bi.ReturnedQuantity, 0)) > 0";

                SqlParameter[] parameters = {
                    new SqlParameter("@BillId", billId)
                };

                _billItems = DbHelper.GetDataTable(query, CommandType.Text, parameters);

                if (_billItems.Rows.Count == 0)
                {
                    MessageBox.Show("No returnable items found for this bill", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _billItems.Columns.Add("ReturnQty", typeof(int));
                _billItems.Columns.Add("ReasonId", typeof(int));
                _billItems.Columns.Add("IsRestocked", typeof(bool));

                dgvBillItems.DataSource = null;
                dgvBillItems.Columns.Clear();

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Item_ID",
                    Visible = false
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ItemName",
                    HeaderText = "Item Name",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Brand",
                    HeaderText = "Brand",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Category",
                    HeaderText = "Category",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "SizeLabel",
                    HeaderText = "Size",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "OriginalQty",
                    HeaderText = "Original Qty",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ReturnedQty",
                    HeaderText = "Already Returned",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "AvailableQty",
                    HeaderText = "Available",
                    ReadOnly = true,
                    Name = "AvailableQtyCol",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Price",
                    HeaderText = "Price",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" },
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                DataGridViewColumn returnQtyCol = CreateNumericUpDownColumn();
                dgvBillItems.Columns.Add(returnQtyCol);

                DataGridViewComboBoxColumn reasonColumn = new DataGridViewComboBoxColumn
                {
                    HeaderText = "Return Reason",
                    Name = "ReasonCol",
                    DataPropertyName = "ReasonId",
                    DisplayMember = "Description",
                    ValueMember = "Reason_ID",
                    DataSource = _returnReasons.Copy(),
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                    FlatStyle = FlatStyle.Flat,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };
                dgvBillItems.Columns.Add(reasonColumn);

                DataGridViewCheckBoxColumn restockColumn = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "Restock?",
                    Name = "RestockCol",
                    DataPropertyName = "IsRestocked",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };
                dgvBillItems.Columns.Add(restockColumn);

                dgvBillItems.DataSource = _billItems;

                foreach (DataGridViewRow row in dgvBillItems.Rows)
                {
                    if (row.Cells["ReturnQtyCol"] != null)
                    {
                        row.Cells["ReturnQtyCol"].Value = 0;
                    }

                    if (row.Cells["ReasonCol"] != null && _returnReasons.Rows.Count > 0)
                    {
                        row.Cells["ReasonCol"].Value = _returnReasons.Rows[0]["Reason_ID"];
                    }

                    if (row.Cells["RestockCol"] != null)
                    {
                        row.Cells["RestockCol"].Value = true;
                    }
                }

                btnProcessReturn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bill items: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataGridViewColumn CreateNumericUpDownColumn()
        {
            DataGridViewColumn column = new DataGridViewColumn();
            column.CellTemplate = new DataGridViewNumericUpDownCell();
            column.Name = "ReturnQtyCol";
            column.HeaderText = "Return Qty";
            column.DataPropertyName = "ReturnQty";
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            return column;
        }

        private void btnProcessReturn_Click(object sender, EventArgs e)
        {
            try
            {
                if (_billExpired)
                {
                    MessageBox.Show("This bill is more than 3 days old. Returns are not allowed.",
                        "Bill Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_billItems == null || _billItems.Rows.Count == 0)
                {
                    MessageBox.Show("No items to return", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool allValid = true;
                StringBuilder validationErrors = new StringBuilder();
                _lastReturnItems = new DataTable();
                _lastReturnItems.Columns.Add("ItemName", typeof(string));
                _lastReturnItems.Columns.Add("Brand", typeof(string));
                _lastReturnItems.Columns.Add("Category", typeof(string));
                _lastReturnItems.Columns.Add("Size", typeof(string));
                _lastReturnItems.Columns.Add("Quantity", typeof(int));
                _lastReturnItems.Columns.Add("Price", typeof(decimal));
                _lastReturnItems.Columns.Add("Refund", typeof(decimal));

                foreach (DataRow row in _billItems.Rows)
                {
                    try
                    {
                        // Skip validation if no return requested
                        if (row["ReturnQty"] == DBNull.Value || (int)row["ReturnQty"] <= 0)
                        {
                            continue;
                        }

                        int returnQty = (int)row["ReturnQty"];
                        int availableQty = (int)row["AvailableQty"];

                        if (returnQty > availableQty)
                        {
                            validationErrors.AppendLine($"Return quantity cannot exceed available quantity for {row["Brand"] + " " + row["Category"]}  (Available: {availableQty})");
                            allValid = false;
                        }

                        if (row["ReasonId"] == DBNull.Value)
                        {
                            validationErrors.AppendLine($"Please select a reason for {row["ItemName"]}");
                            allValid = false;
                        }

                        // Add to receipt table
                        _lastReturnItems.Rows.Add(
                            row["ItemName"],
                            row["Brand"],
                            row["Category"],
                            row["SizeLabel"],
                            returnQty,
                            row["Price"],
                            (decimal)row["Price"] * returnQty
                        );
                    }
                    catch (Exception ex)
                    {
                        validationErrors.AppendLine($"Error validating {row["ItemName"]}: {ex.Message}");
                        allValid = false;
                    }
                }

                if (!allValid)
                {
                    MessageBox.Show(validationErrors.ToString(), "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_lastReturnItems.Rows.Count == 0)
                {
                    MessageBox.Show("No items selected for return", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var returnItems = new List<ReturnItem>();
                foreach (DataRow row in _billItems.Rows)
                {
                    try
                    {
                        if (row["ReturnQty"] != DBNull.Value && (int)row["ReturnQty"] > 0)
                        {
                            returnItems.Add(new ReturnItem
                            {
                                BillId = int.Parse(txtBillId.Text),
                                ItemId = (int)row["Item_ID"],
                                Quantity = (int)row["ReturnQty"],
                                ReasonId = (int)row["ReasonId"],
                                IsRestocked = (bool)row["IsRestocked"]
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error processing item {row["ItemName"]}: {ex.Message}", "Processing Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                ReturnService returnService = new ReturnService();
                ReturnResult result = returnService.ProcessReturn(
                    _currentUser.Employee_ID,
                    int.Parse(txtBillId.Text),
                    returnItems
                );

                _lastReturnId = result.ReturnId;
                _lastTotalRefund = result.TotalRefund;

                MessageBox.Show($"Return processed successfully!\nReturn ID: {_lastReturnId}",
                    "Return Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                PrintReturnReceipt();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing return: {ex.Message}", "Processing Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintReturnReceipt()
        {
            try
            {
                string printerName = GetPrinterName();
                if (string.IsNullOrEmpty(printerName))
                {
                    MessageBox.Show("No receipt printer found. Please configure a printer.", "Print Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<byte> output = new List<byte>();
                output.AddRange(new byte[] { 0x1B, 0x40 });

                PrintCentered("RETURN RECEIPT", output, true);
                PrintCentered("STYLE NEWAGE", output);
                PrintCentered("No.16, Negombo Rd, Narammala", output);
                PrintCentered("Tel: 077491913 / 0372249139", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                PrintCentered($"RETURN ID: {_lastReturnId}", output, true);
                PrintCentered($"ORIGINAL BILL: {txtBillId.Text}", output);
                PrintCentered($"BILL DATE: {_billDate:yyyy-MM-dd}", output);
                PrintCentered($"RETURN DATE: {DateTime.Now:yyyy-MM-dd}", output);
                PrintCentered($"CASHIER: {_currentUser.firstName} {_currentUser.lastName}", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));
                PrintSeparator(output);

                PrintLeftRight("ITEM", "QTY  REFUND", output);
                PrintSeparator(output);

                foreach (DataRow row in _lastReturnItems.Rows)
                {
                    string itemName = row["ItemName"].ToString();
                    string brand = row["Brand"].ToString();
                    string category = row["Category"].ToString();
                    string size = row["Size"].ToString();
                    int qty = (int)row["Quantity"];
                    decimal refund = (decimal)row["Refund"];

                    PrintLeft($"{brand} {category}", output);
                    if (!string.IsNullOrEmpty(size))
                    {
                        PrintLeft($"Size: {size}", output);
                    }
                    PrintLeft(itemName, output);

                    PrintLeftRight(string.Empty, $"{qty}   {refund:C}", output);
                    output.AddRange(Encoding.ASCII.GetBytes("\n"));
                }

                PrintSeparator(output);
                PrintLeftRight("TOTAL REFUND:", _lastTotalRefund.ToString("C"), output, true);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                PrintCentered("Thank you for your business!", output);
                PrintCentered("Items must be returned with tags", output);
                PrintCentered("within 3 days for exchange only", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n\n\n"));

                output.AddRange(new byte[] { 0x1B, 0x64, 0x02 });
                output.AddRange(new byte[] { 0x1B, 0x69 });

                RawPrinterHelper.SendBytesToPrinter(printerName, output.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string GetPrinterName()
        {
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                if (printer.Contains("POS") || printer.Contains("Receipt"))
                {
                    return printer;
                }
            }
            return null;
        }

        private void PrintCentered(string text, List<byte> output, bool bold = false)
        {
            const int MAX_LINE_WIDTH = 32;

            if (bold)
            {
                output.AddRange(new byte[] { 0x1B, 0x45, 0x01 });
            }

            if (text.Length > MAX_LINE_WIDTH)
                text = text.Substring(0, MAX_LINE_WIDTH);

            int spaces = (MAX_LINE_WIDTH - text.Length) / 2;
            output.AddRange(Encoding.ASCII.GetBytes(new string(' ', spaces) + text + "\n"));

            if (bold)
            {
                output.AddRange(new byte[] { 0x1B, 0x45, 0x00 });
            }
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
            {
                output.AddRange(new byte[] { 0x1B, 0x45, 0x01 });
            }

            int availableSpace = MAX_LINE_WIDTH - right.Length - 1;
            if (availableSpace < 1) availableSpace = 1;

            left = left.Length > availableSpace ? left.Substring(0, availableSpace) : left;
            string line = left.PadRight(availableSpace) + " " + right;
            output.AddRange(Encoding.ASCII.GetBytes(line + "\n"));

            if (bold)
            {
                output.AddRange(new byte[] { 0x1B, 0x45, 0x00 });
            }
        }

        private void PrintSeparator(List<byte> output)
        {
            output.AddRange(Encoding.ASCII.GetBytes(new string('-', 32) + "\n"));
        }

        private void ResetForm()
        {
            txtBillId.Clear();
            dgvBillItems.DataSource = null;
            dgvBillItems.Columns.Clear();
            btnProcessReturn.Enabled = false;
            _billItems = null;
            lblBillDate.Text = string.Empty;
            _billExpired = false;
        }

        private void txtBillId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dgvBillItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                if (e.Exception != null)
                {
                    string columnName = dgvBillItems.Columns[e.ColumnIndex].HeaderText;

                    if (e.Exception is FormatException)
                    {
                        MessageBox.Show($"Invalid format in {columnName} column", "Input Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Data error in {columnName}: {e.Exception.Message}", "Input Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    e.ThrowException = false;
                }
            }
            catch
            {
                // Suppress any secondary errors
            }
        }

        private void dgvBillItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvBillItems != null)
            {
                dgvBillItems.Rows[e.RowIndex].ErrorText = string.Empty;
            }
        }

        private void dgvBillItems_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (dgvBillItems.Columns[e.ColumnIndex].Name == "ReturnQtyCol" &&
                    dgvBillItems.Rows[e.RowIndex].IsNewRow == false)
                {
                    dgvBillItems.Rows[e.RowIndex].ErrorText = null;

                    if (!int.TryParse(e.FormattedValue.ToString(), out int returnQty) || returnQty < 0)
                    {
                        e.Cancel = true;
                        dgvBillItems.Rows[e.RowIndex].ErrorText = "Please enter a valid positive number";
                        return;
                    }

                    if (_billItems != null && e.RowIndex < _billItems.Rows.Count)
                    {
                        int availableQty = Convert.ToInt32(_billItems.Rows[e.RowIndex]["AvailableQty"]);

                        if (returnQty > availableQty)
                        {
                            e.Cancel = true;
                            dgvBillItems.Rows[e.RowIndex].ErrorText = $"Cannot exceed available quantity ({availableQty})";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Validation error: {ex.Message}", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtBillId_Enter(object sender, EventArgs e)
        {
            if (txtBillId.Text == "Enter Bill ID")
            {
                txtBillId.Text = string.Empty;
                txtBillId.ForeColor = DarkText;
            }
        }

        private void txtBillId_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBillId.Text))
            {
                txtBillId.Text = "Enter Bill ID";
                txtBillId.ForeColor = Color.Gray;
            }
        }

        public class DataGridViewNumericUpDownCell : DataGridViewTextBoxCell
        {
            public override void InitializeEditingControl(int rowIndex, object initialFormattedValue,
                DataGridViewCellStyle dataGridViewCellStyle)
            {
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
                NumericUpDownEditingControl ctl =
                    DataGridView.EditingControl as NumericUpDownEditingControl;

                if (this.Value == null || this.Value == DBNull.Value)
                {
                    ctl.Value = 0;
                }
                else
                {
                    ctl.Value = Convert.ToDecimal(this.Value);
                }
            }

            public override Type EditType => typeof(NumericUpDownEditingControl);
            public override Type ValueType => typeof(int);
            public override object DefaultNewRowValue => 0;
        }

        public class NumericUpDownEditingControl : NumericUpDown, IDataGridViewEditingControl
        {
            private DataGridView dataGridView;
            private bool valueChanged = false;
            private int rowIndex;

            public NumericUpDownEditingControl()
            {
                this.Minimum = 0;
                this.Maximum = 1000;
                this.DecimalPlaces = 0;
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public object EditingControlFormattedValue
            {
                get { return this.Value.ToString(); }
                set
                {
                    if (value is string)
                    {
                        try
                        {
                            this.Value = decimal.Parse((string)value);
                        }
                        catch
                        {
                            this.Value = 0;
                        }
                    }
                }
            }

            public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
            {
                return EditingControlFormattedValue;
            }

            public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
            {
                this.Font = dataGridViewCellStyle.Font;
                this.ForeColor = dataGridViewCellStyle.ForeColor;
                this.BackColor = dataGridViewCellStyle.BackColor;
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int EditingControlRowIndex
            {
                get { return rowIndex; }
                set { rowIndex = value; }
            }

            public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
            {
                switch (key & Keys.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.PageDown:
                    case Keys.PageUp:
                        return true;
                    default:
                        return !dataGridViewWantsInputKey;
                }
            }

            public void PrepareEditingControlForEdit(bool selectAll)
            {
            }

            public bool RepositionEditingControlOnValueChange => false;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public DataGridView EditingControlDataGridView
            {
                get { return dataGridView; }
                set { dataGridView = value; }
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public bool EditingControlValueChanged
            {
                get { return valueChanged; }
                set { valueChanged = value; }
            }

            public Cursor EditingPanelCursor => base.Cursor;

            protected override void OnValueChanged(EventArgs e)
            {
                valueChanged = true;
                this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
                base.OnValueChanged(e);
            }
        }

        public static class RawPrinterHelper
        {
            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA",
                SetLastError = true, CharSet = CharSet.Ansi,
                ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter",
                SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA",
                SetLastError = true, CharSet = CharSet.Ansi,
                ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            private static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

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
            private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

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
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();
                di.pDocName = "POS Return Receipt";
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

                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }

                if (dwWritten != pBytes.Length)
                {
                    return false;
                }
                return true;
            }
        }

    }
}