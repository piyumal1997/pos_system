using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Services;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using pos_system.pos.UI.Forms.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using pos_system.pos.UI.Forms.Inventory;
using System.Text.RegularExpressions;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class ReturnsForm : Form
    {
        private Employee _currentUser;
        private DataTable _billItems;
        private DataTable _returnReasons;
        private bool _billExpired = false;
        private DateTime _billDate;
        private const string PRINTER_NAME = "XP-80C"; 
        private ContextMenuStrip contextMenuStripImage;
        private ToolStripMenuItem viewImageToolStripMenuItem;
        private int currentSelectedRowIndex = -1;

        public ReturnsForm(Employee currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            // Initialize context menu
            InitializeContextMenu();

            InitializeReturnReasons();
            lblBillDate.Text = string.Empty;
            lblCurrentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtBillId.ForeColor = Color.Gray;
            txtBillId.Text = "Enter Bill ID";

            // Enable form-level key event capture
            this.KeyPreview = true;
            this.KeyDown += ReturnsForm_KeyDown; // Handle F12
            txtBillId.KeyDown += txtBillId_KeyDown; // Handle Enter in textbox

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

        private void InitializeContextMenu()
        {
            // Create context menu
            contextMenuStripImage = new ContextMenuStrip();

            // Create menu item
            viewImageToolStripMenuItem = new ToolStripMenuItem();
            viewImageToolStripMenuItem.Text = "View Image";
            viewImageToolStripMenuItem.Click += new EventHandler(viewImageToolStripMenuItem_Click);

            // Add menu item to context menu
            contextMenuStripImage.Items.Add(viewImageToolStripMenuItem);

            // Assign context menu to DataGridView
            dgvBillItems.ContextMenuStrip = contextMenuStripImage;

            // Wire up the CellMouseClick event
            dgvBillItems.CellMouseClick += dgvBillItems_CellMouseClick;

            // Also wire up the SelectionChanged event to track the current row
            dgvBillItems.SelectionChanged += dgvBillItems_SelectionChanged;

            // Wire up the CellClick event
            dgvBillItems.CellClick += dgvBillItems_CellClick;
        }

        private void dgvBillItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBillItems.SelectedRows.Count > 0)
            {
                currentSelectedRowIndex = dgvBillItems.SelectedRows[0].Index;
            }
            else if (dgvBillItems.CurrentRow != null)
            {
                currentSelectedRowIndex = dgvBillItems.CurrentRow.Index;
            }
        }

        private void ReturnsForm_Load(object sender, EventArgs e)
        {
            dgvBillItems.ContextMenuStrip = contextMenuStripImage;
        }

        private void dgvBillItems_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Console.WriteLine($"CellMouseClick - Row: {e.RowIndex}, Column: {e.ColumnIndex}, Button: {e.Button}");

            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                Console.WriteLine($"Right-click detected on row {e.RowIndex}");

                // Ensure the row is selected
                if (!dgvBillItems.Rows[e.RowIndex].Selected)
                {
                    Console.WriteLine($"Selecting row {e.RowIndex}");
                    dgvBillItems.ClearSelection();
                    dgvBillItems.Rows[e.RowIndex].Selected = true;
                }

                currentSelectedRowIndex = e.RowIndex;
                Console.WriteLine($"Set currentSelectedRowIndex to {currentSelectedRowIndex}");

                // Show the context menu at the mouse position
                var relativeMousePosition = dgvBillItems.PointToClient(Cursor.Position);
                contextMenuStripImage.Show(dgvBillItems, relativeMousePosition);
            }
        }

        private void viewImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If currentSelectedRowIndex is -1, try to get the selected row
            if (currentSelectedRowIndex == -1 && dgvBillItems.SelectedRows.Count > 0)
            {
                currentSelectedRowIndex = dgvBillItems.SelectedRows[0].Index;
            }

            if (currentSelectedRowIndex >= 0 && _billItems != null && _billItems.Rows.Count > currentSelectedRowIndex)
            {
                DataRow selectedRow = _billItems.Rows[currentSelectedRowIndex];
                int productSizeId = Convert.ToInt32(selectedRow["ProductSize_ID"]);

                // Debug info
                string debugInfo = $"ProductSize_ID: {productSizeId}\n";
                debugInfo += $"Product: {selectedRow["ItemName"]}\n";
                debugInfo += $"Brand: {selectedRow["Brand"]}\n";
                debugInfo += $"Category: {selectedRow["Category"]}\n";

                // Get the product image
                Image productImage = GetProductImageByProductSizeId(productSizeId);

                if (productImage != null)
                {
                    debugInfo += $"Image size: {productImage.Width}x{productImage.Height}\n";

                    // Show image in a dialog
                    using (ImagePreviewDialog previewDialog = new ImagePreviewDialog(productImage))
                    {
                        previewDialog.ShowDialog();
                    }
                }
                else
                {
                    debugInfo += "No image found\n";

                    // Show debug info in a message box
                    //MessageBox.Show(debugInfo, "Image Debug Info",
                    //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a row first.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvBillItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                currentSelectedRowIndex = e.RowIndex;
            }
        }

        private void CheckImageBasePath()
        {
            string imageBasePath = ConfigurationManager.AppSettings["ImageBasePath"];

            if (string.IsNullOrEmpty(imageBasePath))
            {
                MessageBox.Show("ImageBasePath is not configured in app.config", "Configuration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(imageBasePath))
            {
                MessageBox.Show($"Image base path does not exist: {imageBasePath}", "Path Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show some info about the path
            string[] imageFiles = Directory.GetFiles(imageBasePath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            MessageBox.Show($"Image base path: {imageBasePath}\nFound {imageFiles.Length} image files",
                "Image Path Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //private Image GetProductImageByProductSizeId(int productSizeId)
        //{
        //    try
        //    {
        //        string query = @"
        //    SELECT p.ItemImage, p.Product_ID, p.description, p.barcode
        //    FROM Product p
        //    INNER JOIN ProductSize ps ON p.Product_ID = ps.Product_ID
        //    WHERE ps.ProductSize_ID = @ProductSizeId";

        //        SqlParameter[] parameters = { new SqlParameter("@ProductSizeId", productSizeId) };
        //        DataTable dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);

        //        if (dt.Rows.Count > 0)
        //        {
        //            DataRow row = dt.Rows[0];
        //            object imageData = row["ItemImage"];

        //            if (imageData != DBNull.Value)
        //            {
        //                // Check if image is stored as byte array
        //                if (imageData is byte[] byteArray)
        //                {
        //                    if (byteArray.Length > 0)
        //                    {
        //                        using (MemoryStream ms = new MemoryStream(byteArray))
        //                        {
        //                            return Image.FromStream(ms);
        //                        }
        //                    }
        //                }
        //                // Check if image is stored as base64 string
        //                else if (imageData is string base64String)
        //                {
        //                    byte[] imageBytes = Convert.FromBase64String(base64String);
        //                    using (MemoryStream ms = new MemoryStream(imageBytes))
        //                    {
        //                        return Image.FromStream(ms);
        //                    }
        //                }
        //                // Check if image is stored as file path
        //                else if (imageData is string filePath && File.Exists(filePath))
        //                {
        //                    return Image.FromFile(filePath);
        //                }
        //            }

        //            // Fallback to file system loading (existing code)
        //            string imageBasePath = ConfigurationManager.AppSettings["ImageBasePath"];
        //            int productId = Convert.ToInt32(row["Product_ID"]);
        //            string description = row["description"].ToString();
        //            string barcode = row["barcode"].ToString();

        //            if (!string.IsNullOrEmpty(imageBasePath) && Directory.Exists(imageBasePath))
        //            {
        //                string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
        //                foreach (string ext in extensions)
        //                {
        //                    string imagePath = Path.Combine(imageBasePath, $"{productId}{ext}");
        //                    if (File.Exists(imagePath)) return Image.FromFile(imagePath);

        //                    imagePath = Path.Combine(imageBasePath, $"{barcode}{ext}");
        //                    if (File.Exists(imagePath)) return Image.FromFile(imagePath);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error loading image: {ex.Message}", "Error",
        //                        MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    return null;
        //}

        private Image GetProductImageByProductSizeId(int productSizeId)
        {
            try
            {
                string query = @"
            SELECT p.ItemImage, p.Product_ID, p.description, p.barcode
            FROM Product p
            INNER JOIN ProductSize ps ON p.Product_ID = ps.Product_ID
            WHERE ps.ProductSize_ID = @ProductSizeId";

                SqlParameter[] parameters = { new SqlParameter("@ProductSizeId", productSizeId) };
                DataTable dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    object imageData = row["ItemImage"];

                    // Debug: Check the actual type of the data
                    Console.WriteLine($"Image data type: {imageData?.GetType().Name}");

                    if (imageData != null && imageData != DBNull.Value)
                    {
                        // Handle different types of image data storage
                        if (imageData is byte[] byteData)
                        {
                            // Binary data directly stored in database
                            if (byteData.Length > 0)
                            {
                                using (MemoryStream ms = new MemoryStream(byteData))
                                {
                                    return Image.FromStream(ms);
                                }
                            }
                        }
                        else if (imageData is string stringData)
                        {
                            // String data - could be a file path or Base-64 encoded string

                            // First, check if it's a valid file path
                            if (File.Exists(stringData))
                            {
                                return Image.FromFile(stringData);
                            }

                            // If not a file path, try to parse as Base-64
                            try
                            {
                                // Check if it looks like Base-64 (optional but helpful)
                                if (stringData.Length % 4 == 0 &&
                                    System.Text.RegularExpressions.Regex.IsMatch(stringData, @"^[a-zA-Z0-9\+/]*={0,3}$"))
                                {
                                    byte[] imageBytes = Convert.FromBase64String(stringData);
                                    using (MemoryStream ms = new MemoryStream(imageBytes))
                                    {
                                        return Image.FromStream(ms);
                                    }
                                }
                            }
                            catch (FormatException)
                            {
                                // Not a valid Base-64 string, continue to other options
                                Console.WriteLine("Data is not a valid Base-64 string");
                            }

                            // If we reach here, the string is neither a file path nor Base-64
                            // Check if it's a relative path in the image directory
                            string imageBasePath = ConfigurationManager.AppSettings["ImageBasePath"];
                            if (!string.IsNullOrEmpty(imageBasePath))
                            {
                                string fullPath = Path.Combine(imageBasePath, stringData);
                                if (File.Exists(fullPath))
                                {
                                    return Image.FromFile(fullPath);
                                }
                            }
                        }
                    }

                    // If no image found in the database, try to load from file system
                    string imageBasePathFallback = ConfigurationManager.AppSettings["ImageBasePath"];
                    int productId = Convert.ToInt32(row["Product_ID"]);
                    string description = row["description"].ToString();
                    string barcode = row["barcode"].ToString();

                    if (!string.IsNullOrEmpty(imageBasePathFallback) && Directory.Exists(imageBasePathFallback))
                    {
                        string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                        foreach (string ext in extensions)
                        {
                            string imagePath = Path.Combine(imageBasePathFallback, $"{productId}{ext}");
                            if (File.Exists(imagePath)) return Image.FromFile(imagePath);

                            imagePath = Path.Combine(imageBasePathFallback, $"{barcode}{ext}");
                            if (File.Exists(imagePath)) return Image.FromFile(imagePath);

                            string safeDescription = RemoveInvalidFileNameChars(description);
                            imagePath = Path.Combine(imageBasePathFallback, $"{safeDescription}{ext}");
                            if (File.Exists(imagePath)) return Image.FromFile(imagePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        private string RemoveInvalidFileNameChars(string filename)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalidChars)
            {
                filename = filename.Replace(c.ToString(), "");
            }
            return filename;
        }

        private void btnSearchBill_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtBillId.Text == "Enter Bill ID" || !int.TryParse(txtBillId.Text, out int billId))
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
                _billExpired = difference.TotalDays > 30;

                if (_billExpired)
                {
                    ThemedMessageBox.Show($"This bill is from {_billDate:yyyy-MM-dd} and is more than 30 days old. Returns are not allowed.",
                        "Bill Expired", ThemedMessageBoxIcon.Warning);
                    btnProcessReturn.Enabled = false;
                    return;
                }

                // UPDATED QUERY FOR NEW SCHEMA
                string query = @"SELECT 
                    bi.ProductSize_ID,
                    p.description AS ItemName,
                    br.brandName AS Brand,
                    cat.categoryName AS Category,
                    s.SizeLabel,
                    bi.quantity AS OriginalQty,
                    ISNULL(bi.ReturnedQuantity, 0) AS ReturnedQty,
                    (bi.quantity - ISNULL(bi.ReturnedQuantity, 0)) AS AvailableQty,
                    (bi.ItemSellingPrice - bi.Per_item_Discount) AS Price  -- Actual price after discount
                FROM Bill_Item bi
                JOIN Bill bl ON bi.Bill_ID = bl.Bill_ID
                JOIN ProductSize ps ON bi.ProductSize_ID = ps.ProductSize_ID
                JOIN Product p ON ps.Product_ID = p.Product_ID
                LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
                LEFT JOIN Brand br ON p.Brand_ID = br.Brand_ID
                LEFT JOIN Category cat ON p.Category_ID = cat.Category_ID
                WHERE bi.Bill_ID = @BillId
                AND (bi.quantity - ISNULL(bi.ReturnedQuantity, 0)) > 0";

                SqlParameter[] parameters = { new SqlParameter("@BillId", billId) };

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

                // Hidden columns
                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ProductSize_ID",
                    HeaderText = "ProductSize_ID",
                    Name = "ProductSize_ID",
                    Visible = false
                });

                // Visible columns
                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ItemName",
                    HeaderText = "Item Name",
                    Name = "ItemName",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Brand",
                    HeaderText = "Brand",
                    Name = "Brand",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Category",
                    HeaderText = "Category",
                    Name = "Category",
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
                    HeaderText = "Returned Qty",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "AvailableQty",
                    HeaderText = "Available",
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Price",
                    HeaderText = "Price",
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                // Editable columns
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
                        row.Cells["ReturnQtyCol"].Value = 0;

                    if (row.Cells["ReasonCol"] != null && _returnReasons.Rows.Count > 0)
                        row.Cells["ReasonCol"].Value = _returnReasons.Rows[0]["Reason_ID"];

                    if (row.Cells["RestockCol"] != null)
                        row.Cells["RestockCol"].Value = true;
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
                if (txtBillId.Text == "Enter Bill ID" || !int.TryParse(txtBillId.Text, out int billId))
                {
                    MessageBox.Show("Please enter a valid Bill ID", "Input Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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
                List<ReturnItem> returnItems = new List<ReturnItem>();

                foreach (DataRow row in _billItems.Rows)
                {
                    try
                    {
                        if (row["ReturnQty"] == DBNull.Value || (int)row["ReturnQty"] <= 0)
                            continue;

                        int returnQty = (int)row["ReturnQty"];
                        int availableQty = (int)row["AvailableQty"];

                        if (returnQty > availableQty)
                        {
                            validationErrors.AppendLine(
                                $"Return quantity cannot exceed available quantity for {row["Brand"]} {row["Category"]} (Available: {availableQty})");
                            allValid = false;
                        }

                        if (row["ReasonId"] == DBNull.Value)
                        {
                            validationErrors.AppendLine($"Please select a reason for {row["ItemName"]}");
                            allValid = false;
                        }

                        // UPDATED FOR NEW SCHEMA - Using ProductSize_ID
                        returnItems.Add(new ReturnItem
                        {
                            BillId = billId,
                            ProductSizeId = (int)row["ProductSize_ID"],
                            Quantity = returnQty,
                            ReasonId = (int)row["ReasonId"],
                            IsRestocked = (bool)row["IsRestocked"]
                        });
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

                if (returnItems.Count == 0)
                {
                    MessageBox.Show("No items selected for return", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ReturnService returnService = new ReturnService();
                var result = returnService.ProcessReturn(
                    _currentUser.Employee_ID,
                    billId,
                    returnItems
                );

                var receiptData = returnService.GetReturnReceiptData(result.ReturnId);

                PrintReturnReceipt(receiptData);
                ResetForm();

                MessageBox.Show($"Return processed successfully!\nReturn ID: {receiptData.ReturnId}",
                    "Return Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing return: {ex}", "Processing Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintReturnReceipt(ReturnReceiptData receiptData)
        {
            try
            {
                //string printerName = GetPrinterName();
                if (string.IsNullOrEmpty(PRINTER_NAME))
                {
                    MessageBox.Show("No receipt printer found. Please configure a printer.", "Print Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<byte> output = new List<byte>();
                output.AddRange(new byte[] { 0x1B, 0x40 }); // Initialize printer

                // Print header
                PrintCentered("RETURN RECEIPT", output, true);
                PrintCentered("STYLE NEWAGE", output);
                PrintCentered("No.102, Negombo Rd, Narammala.", output);
                PrintCentered("Tel: 0777491913 / 0374545097", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Print return details
                PrintCentered($"RETURN ID: {receiptData.ReturnId}", output, true);
                PrintCentered($"ORIGINAL BILL: {receiptData.OriginalBillId}", output);
                PrintCentered($"BILL DATE: {receiptData.BillDate:yyyy-MM-dd}", output);
                PrintCentered($"RETURN DATE: {receiptData.ReturnDate:yyyy-MM-dd}", output);
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

                    //PrintLeft(item.ItemName, output); //Description not needed
                    PrintLeftRight(string.Empty, $"{item.Quantity}   {item.Refund:N2}", output);
                    output.AddRange(Encoding.ASCII.GetBytes("\n"));
                }

                // Print footer
                PrintSeparator(output);
                PrintLeftRight("TOTAL REFUND:", receiptData.TotalRefund.ToString("N2"), output, true);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                PrintCentered("Thank you for your business!", output);
                PrintCentered("Items must be returned with tags", output);
                PrintCentered("within 3 days for exchange only", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n\n\n"));

                // Printer commands
                output.AddRange(new byte[] { 0x1B, 0x64, 0x02 }); // Feed 2 lines
                output.AddRange(new byte[] { 0x1B, 0x69 }); // Cut paper

                // Send to printer
                RawPrinterHelper.SendBytesToPrinter(PRINTER_NAME, output.ToArray());
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
                    return printer;
            }
            return null;
        }

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

        private void ResetForm()
        {
            txtBillId.Text = "Enter Bill ID";
            txtBillId.ForeColor = Color.Gray;
            dgvBillItems.DataSource = null;
            dgvBillItems.Columns.Clear();
            btnProcessReturn.Enabled = false;
            _billItems = null;
            lblBillDate.Text = string.Empty;
            _billExpired = false;
        }

        private void txtBillId_Enter(object sender, EventArgs e)
        {
            if (txtBillId.Text == "Enter Bill ID")
            {
                txtBillId.Text = "";
                txtBillId.ForeColor = SystemColors.WindowText;
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

        private void dgvBillItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show($"Data error: {e.Exception.Message}", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.ThrowException = false;
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

        // Trigger Search when Enter is pressed in Bill ID textbox
        private void txtBillId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearchBill.PerformClick();
                e.Handled = true;
            }
        }

        // Trigger Process Return when F12 is pressed
        private void ReturnsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12 && btnProcessReturn.Enabled)
            {
                btnProcessReturn.PerformClick();
                e.Handled = true;
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

                return dwWritten == pBytes.Length;
            }
        }
    }
}