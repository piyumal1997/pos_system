using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Utilities;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using pos_system.pos.UI.Forms.Sales;
using System.Data;
using System.Drawing.Printing;
using System.Text;
using System.Timers;
using pos_system.pos.UI.Forms;
using pos_system;
using pos_system.pos;
using pos_system.pos.UI;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class BillingForm : Form
    {
        #region Constants and Fields
        private const string PRINTER_NAME = "POS-58";
        private const int MAX_LINE_WIDTH = 32;
        private System.Timers.Timer _dateTimer;

        private Employee _currentUser;
        private DataTable _cartItems;
        private int _totalItems = 0;
        private int _billId = 0;
        private decimal _totalDiscount = 0;
        private decimal _subtotal = 0;
        private decimal _billDiscountPercentage = 0;
        private bool _isBillDiscountApplied = false;
        private bool _discountConflictWarningShown = false;
        private pos_system.pos.UI.Forms.Sales.BillingForm.ReturnToken _appliedToken;
        private bool _tokenApplied;

        #endregion

        #region Helper Classes
        public class ReturnToken
        {
            public int ReturnId { get; set; }
            public decimal TotalRefund { get; set; }
        }

        public class BillItem
        {
            public int Item_ID { get; set; }
            public object Size_ID { get; set; }
            public int Quantity { get; set; }
            public decimal SellingPrice { get; set; }
            public decimal Per_item_Discount { get; set; }
        }
        #endregion

        #region Constructor and Initialization
        public BillingForm(Employee user)
        {
            try
            {
                _currentUser = user;
                InitializeComponent();
                InitializeDataGridView();
                container.Controls.Add(dgvCart, 0, 3);
                dgvCart.Dock = DockStyle.Fill;
                InitializeCartDataTable();
                GenerateBillId();
                InitializeDateTimeTimer();
                AttachEventHandlers();
                this.Load += (s, e) => AttachGridEventHandlers();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Initialization");
            }
        }

        private void InitializeDateTimeTimer()
        {
            _dateTimer = new System.Timers.Timer(1000); // Update every second
            _dateTimer.Elapsed += UpdateDateTime;
            _dateTimer.AutoReset = true;
            _dateTimer.Enabled = true;
            UpdateDateTime(null, null); // Initial update
        }

        private void UpdateDateTime(object sender, ElapsedEventArgs e)
        {
            // Ensure thread-safe update of UI control
            if (lblDateTime.InvokeRequired)
            {
                lblDateTime.Invoke(new Action(() => UpdateDateTime(sender, e)));
                return;
            }

            lblDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void InitializeDataGridView()
        {
            dgvCart = new DataGridView
            {
                ReadOnly = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                GridColor = Color.FromArgb(240, 240, 240),
                Dock = DockStyle.Fill
            };

            // Initialize columns
            colDelete = new DataGridViewButtonColumn
            {
                HeaderText = "Delete",
                Text = "🗑️",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.LightCoral,
                    ForeColor = Color.White,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            colQuantity = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity",
                HeaderText = "Qty",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            colDiscount = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Discount",
                HeaderText = "Discount %",
                Name = "Discount",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            // Size column
            colSize = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Size",
                HeaderText = "Size",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };

            // Configure other columns
            DataGridViewTextBoxColumn colItemId = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Item_ID",
                HeaderText = "ID",
                Visible = false
            };

            DataGridViewTextBoxColumn colBarcode = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Barcode",
                HeaderText = "Barcode",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Visible = false
            };

            DataGridViewTextBoxColumn colBrand = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Brand",
                HeaderText = "Brand",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            DataGridViewTextBoxColumn colCategory = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Category",
                HeaderText = "Category",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            DataGridViewTextBoxColumn colDescription = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Description",
                HeaderText = "Description",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                HeaderText = "Price",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            DataGridViewTextBoxColumn colDiscountAmount = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DiscountAmount",
                HeaderText = "Discount Amt",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true
            };

            DataGridViewTextBoxColumn colNetPrice = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NetPrice",
                HeaderText = "Net Price",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true
            };

            DataGridViewTextBoxColumn colMaxDiscount = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MaxDiscount",
                HeaderText = "Max Discount",
                Visible = false
            };

            DataGridViewTextBoxColumn colAvailableStock = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AvailableStock",
                HeaderText = "Stock",
                Visible = false
            };

            // Style DataGridView
            dgvCart.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            dgvCart.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvCart.EnableHeadersVisualStyles = false;
            dgvCart.RowHeadersVisible = false;
            dgvCart.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvCart.DefaultCellStyle.SelectionBackColor = Color.FromArgb(189, 224, 254);
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.FromArgb(64, 64, 64);
            dgvCart.RowTemplate.Height = 30;

            dgvCart.Columns.AddRange(new DataGridViewColumn[] {
                colItemId, colBarcode, colBrand, colCategory, colDescription,
                colSize,
                colPrice, colQuantity, colDiscount, colDiscountAmount, colNetPrice,
                colMaxDiscount, colAvailableStock, colDelete
            });
        }

        private void InitializeCartDataTable()
        {
            try
            {
                _cartItems = new DataTable();
                _cartItems.Columns.Add("Item_ID", typeof(int));
                _cartItems.Columns.Add("Barcode", typeof(string));
                _cartItems.Columns.Add("Brand", typeof(string));
                _cartItems.Columns.Add("Category", typeof(string));
                _cartItems.Columns.Add("Description", typeof(string));
                _cartItems.Columns.Add("Size", typeof(string));
                _cartItems.Columns.Add("Price", typeof(decimal));
                _cartItems.Columns.Add("Quantity", typeof(int));
                _cartItems.Columns.Add("Discount", typeof(decimal));
                _cartItems.Columns.Add("DiscountAmount", typeof(decimal), "Price * Quantity * Discount / 100");
                _cartItems.Columns.Add("NetPrice", typeof(decimal), "Price * Quantity - (Price * Quantity * Discount / 100)");
                _cartItems.Columns.Add("MaxDiscount", typeof(decimal));
                _cartItems.Columns.Add("AvailableStock", typeof(int));

                dgvCart.DataSource = _cartItems;
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Cart Initialization");
            }
        }

        private void GenerateBillId()
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT ISNULL(MAX(Bill_ID), 0) + 1 FROM Bill", conn);
                    _billId = Convert.ToInt32(cmd.ExecuteScalar());
                    lblBillId.Text = $"Bill ID: {_billId}";
                }
            }
            catch (SqlException sqlEx)
            {
                HandleDatabaseError(sqlEx);
                try
                {
                    _billId = int.Parse(DateTime.Now.ToString("MMddHHmmss"));
                    lblBillId.Text = $"Bill ID: {_billId}";
                    MessageBox.Show("Using fallback bill ID", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception fallbackEx)
                {
                    _billId = 1;
                    lblBillId.Text = $"Bill ID: {_billId}";
                    HandleUnexpectedError(fallbackEx, "Bill ID Fallback");
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Bill ID Generation");
            }
        }
        #endregion

        #region Event Handlers
        private void AttachEventHandlers()
        {
            try
            {
                btnRefreshBill.Click += (s, e) => RefreshBill();
                btnClearBill.Click += (s, e) => ClearBill();
                btnAddItem.Click += BtnAddItem_Click;
                txtBarcode.KeyPress += TxtBarcode_KeyPress;
                btnProcessPayment.Click += BtnProcessPayment_Click;
                btnApplyBillDiscount.Click += BtnApplyBillDiscount_Click;
                btnClearDiscounts.Click += BtnClearDiscounts_Click;
                btnApplyToken.Click += BtnApplyToken_Click;
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Event Attachment");
            }
        }

        private void AttachGridEventHandlers()
        {
            try
            {
                dgvCart.CellClick += DgvCart_CellClick;
                _cartItems.RowChanged += (s, e) => UpdateSummary();
                _cartItems.RowDeleted += (s, e) => UpdateSummary();
                _cartItems.TableNewRow += (s, e) => UpdateSummary();

                dgvCart.CellValidating += (s, e) =>
                {
                    try
                    {
                        if (e.ColumnIndex == colDiscount.Index)
                        {
                            DataRow dataRow = _cartItems.Rows[e.RowIndex];
                            decimal maxDiscount = Convert.ToDecimal(dataRow["MaxDiscount"]);

                            if (decimal.TryParse(e.FormattedValue.ToString(), out decimal newValue))
                            {
                                if (newValue > maxDiscount)
                                {
                                    e.Cancel = true;
                                    dgvCart.Rows[e.RowIndex].ErrorText = $"Discount cannot exceed {maxDiscount}%";
                                }

                                if (_isBillDiscountApplied && newValue > 0 && !_discountConflictWarningShown)
                                {
                                    MessageBox.Show("A bill discount is already applied. Per-item discounts will override it.",
                                        "Discount Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    _discountConflictWarningShown = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleUnexpectedError(ex, "Grid Validation");
                    }
                };

                dgvCart.CellEndEdit += (s, e) =>
                {
                    try
                    {
                        if (e.ColumnIndex == colDiscount.Index)
                        {
                            dgvCart.Rows[e.RowIndex].ErrorText = string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleUnexpectedError(ex, "Grid Edit");
                    }
                };
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Grid Event Attachment");
            }
        }

        private void DgvCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgvCart == null) return;

                if (e.ColumnIndex == colDelete.Index)
                {
                    _cartItems.Rows.RemoveAt(e.RowIndex);
                }
                else if (e.ColumnIndex == colQuantity.Index)
                {
                    ShowQuantityEditor(e.RowIndex);
                }
                else if (e.ColumnIndex == colDiscount.Index)
                {
                    ShowDiscountEditor(e.RowIndex);
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Grid Cell Click");
            }
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                AddItemToCart();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Add Item");
            }
        }

        private void TxtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    AddItemToCart();
                    e.Handled = true;

                    BeginInvoke((MethodInvoker)delegate
                    {
                        txtBarcode.SelectAll();
                    });
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Barcode Input");
            }
        }

        private void BtnApplyBillDiscount_Click(object sender, EventArgs e)
        {
            try
            {
                bool hasPerItemDiscount = _cartItems.AsEnumerable()
                    .Any(row => Convert.ToDecimal(row["Discount"]) > 0);

                if (hasPerItemDiscount)
                {
                    var result = MessageBox.Show(
                        "Per-item discounts are already applied. Applying a bill discount will override them. Continue?",
                        "Discount Conflict",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result != DialogResult.Yes) return;

                    // Clear all per-item discounts
                    foreach (DataRow row in _cartItems.Rows)
                    {
                        row["Discount"] = 0;
                    }
                }

                using (var discountForm = new BillDiscountForm())
                {
                    if (discountForm.ShowDialog() == DialogResult.OK)
                    {
                        _billDiscountPercentage = discountForm.SelectedDiscount;
                        _isBillDiscountApplied = true;
                        UpdateSummary();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Apply Discount");
            }
        }

        private void BtnClearDiscounts_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isBillDiscountApplied)
                {
                    _billDiscountPercentage = 0;
                    _isBillDiscountApplied = false;
                    lblBillDiscount.Text = string.Empty;
                }

                foreach (DataRow row in _cartItems.Rows)
                {
                    row["Discount"] = 0;
                }

                _discountConflictWarningShown = false;
                UpdateSummary();
                MessageBox.Show("All discounts have been cleared", "Discounts Cleared",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Clear Discounts");
            }
        }

        private void BtnApplyToken_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTokenId.Text))
                {
                    ShowError("Please enter a token ID");
                    return;
                }

                if (!int.TryParse(txtTokenId.Text, out int tokenId))
                {
                    ShowError("Invalid token format");
                    return;
                }

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT Return_ID, TotalRefund, IsUsed 
                        FROM [Return] 
                        WHERE Return_ID = @TokenId";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TokenId", tokenId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.GetBoolean(2)) // IsUsed
                                {
                                    ShowError("Token has already been used");
                                    return;
                                }

                                _appliedToken = new pos_system.pos.UI.Forms.Sales.BillingForm.ReturnToken
                                {
                                    ReturnId = reader.GetInt32(0),
                                    TotalRefund = reader.GetDecimal(1)
                                };

                                _tokenApplied = true;
                                txtTokenId.Enabled = false;
                                btnApplyToken.Enabled = false;

                                MessageBox.Show($"Token applied successfully! Value: {_appliedToken.TotalRefund:C2}",
                                    "Token Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                ShowError("Token not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Apply Token");
            }
        }

        private void BtnProcessPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (_cartItems.Rows.Count == 0)
                {
                    ShowError("Cart is empty");
                    return;
                }

                decimal totalAmount = Convert.ToDecimal(lblTotal.Text.Replace("$", string.Empty).Trim());
                decimal tokenValue = _tokenApplied ? _appliedToken.TotalRefund : 0;

                // Validate token coverage
                if (_tokenApplied && totalAmount < tokenValue)
                {
                    ShowError($"Purchase amount must be >= token value ({tokenValue:C2})");
                    return;
                }

                using (var paymentForm = new pos_system.pos.UI.Forms.Sales.PaymentForm(totalAmount, tokenValue))
                {
                    if (paymentForm.ShowDialog() == DialogResult.OK && paymentForm.IsConfirmed)
                    {
                        ProcessConfirmedPayment(
                            paymentMethod: paymentForm.PaymentMethod,
                            amountTendered: paymentForm.AmountTendered,
                            cardLast4: paymentForm.CardLastFour,
                            bankLast4: paymentForm.BankLastFour,
                            change: paymentForm.Change
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Payment Processing");
            }
        }
        #endregion

        #region Business Logic
        private void AddItemToCart()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    ShowError("Please enter a barcode");
                    return;
                }

                string barcode = txtBarcode.Text;
                int quantity = 1;

                if (barcode.Contains("x"))
                {
                    var parts = barcode.Split('x');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int qty) && qty > 0)
                    {
                        quantity = qty;
                        barcode = parts[1];
                    }
                }

                try
                {
                    using (var conn = DbHelper.GetConnection())
                    {
                        conn.Open();
                        string query = @"
                            SELECT i.Item_ID, i.barcode, i.description, i.RetailPrice, 
                                   b.brandName AS Brand, c.categoryName AS Category, 
                                   s.SizeLabel AS Size,
                                   i.quantity AS AvailableStock, i.maxDiscount
                            FROM Item i
                            INNER JOIN Brand b ON i.Brand_ID = b.Brand_ID
                            INNER JOIN Category c ON i.Category_ID = c.Category_ID
                            LEFT JOIN Size s ON i.Size_ID = s.Size_ID
                            WHERE i.barcode = @Barcode AND i.IsDeleted = 0 AND i.quantity > 0";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Barcode", barcode);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int availableStock = reader.GetInt32(7);
                                    decimal maxDiscount = reader.GetDecimal(8);
                                    string size = reader["Size"] != DBNull.Value ? reader.GetString(6) : "N/A";

                                    var existingRow = _cartItems.AsEnumerable()
                                        .FirstOrDefault(row => row.Field<string>("Barcode") == barcode);

                                    if (existingRow != null)
                                    {
                                        int currentQty = Convert.ToInt32(existingRow["Quantity"]);
                                        int newQty = currentQty + quantity;

                                        if (newQty > availableStock)
                                        {
                                            newQty = availableStock;
                                            ShowError($"Cannot exceed available stock of {availableStock}");
                                        }

                                        existingRow["Quantity"] = newQty;
                                    }
                                    else
                                    {
                                        if (quantity > availableStock)
                                        {
                                            quantity = availableStock;
                                            ShowError($"Cannot exceed available stock of {availableStock}");
                                        }

                                        _cartItems.Rows.Add(
                                            reader.GetInt32(0),
                                            reader.GetString(1),
                                            reader.GetString(4),
                                            reader.GetString(5),
                                            reader.GetString(2),
                                            size,
                                            reader.GetDecimal(3),
                                            quantity,
                                            0,
                                            0,
                                            0,
                                            maxDiscount,
                                            availableStock
                                        );
                                    }
                                    txtBarcode.Clear();
                                    txtBarcode.Focus();
                                }
                                else
                                {
                                    ShowError("Item not found or out of stock");
                                }
                            }
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    HandleDatabaseError(sqlEx);
                }
                catch (InvalidOperationException ioEx)
                {
                    ShowError($"Operation failed: {ioEx.Message}");
                }
                catch (Exception ex)
                {
                    HandleUnexpectedError(ex, "Add Item to Cart");
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Add Item");
            }
        }

        private void ShowQuantityEditor(int rowIndex)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= _cartItems.Rows.Count) return;

                int currentQty = Convert.ToInt32(_cartItems.Rows[rowIndex]["Quantity"]);
                int availableStock = Convert.ToInt32(_cartItems.Rows[rowIndex]["AvailableStock"]);

                using (var qtyForm = new QuantityForm(availableStock, currentQty))
                {
                    if (qtyForm.ShowDialog() == DialogResult.OK)
                    {
                        _cartItems.Rows[rowIndex]["Quantity"] = qtyForm.SelectedQuantity;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                ShowError("Invalid item selection");
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Quantity Editor");
            }
        }

        private void ShowDiscountEditor(int rowIndex)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= _cartItems.Rows.Count) return;

                DataRow row = _cartItems.Rows[rowIndex];
                decimal maxDiscount = Convert.ToDecimal(row["MaxDiscount"]);
                decimal currentDiscount = Convert.ToDecimal(row["Discount"]);

                if (_isBillDiscountApplied && currentDiscount == 0 && !_discountConflictWarningShown)
                {
                    var result = MessageBox.Show(
                        "A bill discount is already applied. Adding per-item discounts will override it. Continue?",
                        "Discount Conflict",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result != DialogResult.Yes) return;
                    _discountConflictWarningShown = true;
                }

                using (var discountForm = new DiscountForm(maxDiscount, currentDiscount))
                {
                    if (discountForm.ShowDialog() == DialogResult.OK)
                    {
                        row["Discount"] = discountForm.SelectedDiscount;

                        if (discountForm.SelectedDiscount > 0 && _isBillDiscountApplied)
                        {
                            _billDiscountPercentage = 0;
                            _isBillDiscountApplied = false;
                            lblBillDiscount.Text = string.Empty;
                            UpdateSummary();
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                ShowError("Invalid item selection");
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Discount Editor");
            }
        }

        private void RefreshBill()
        {
            try
            {
                if (_cartItems.Rows.Count > 0)
                {
                    var result = MessageBox.Show("This will clear the current bill. Continue?",
                        "Refresh Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result != DialogResult.Yes) return;
                }

                ClearBill();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Refresh Bill");
            }
        }

        private void ClearBill()
        {
            try
            {
                _cartItems.Rows.Clear();
                _billDiscountPercentage = 0;
                _isBillDiscountApplied = false;
                _discountConflictWarningShown = false;
                _tokenApplied = false;
                _appliedToken = null;
                lblBillDiscount.Text = string.Empty;
                txtTokenId.Enabled = true;
                btnApplyToken.Enabled = true;
                txtTokenId.Clear();
                GenerateBillId();
                txtBarcode.Focus();
                UpdateSummary();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Clear Bill");
            }
        }

        private void UpdateSummary()
        {
            try
            {
                decimal total = 0;
                _totalItems = 0;
                _subtotal = 0;
                _totalDiscount = 0;

                foreach (DataRow row in _cartItems.Rows)
                {
                    if (row["Quantity"] != DBNull.Value)
                        _totalItems += Convert.ToInt32(row["Quantity"]);

                    if (row["NetPrice"] != DBNull.Value)
                        total += Convert.ToDecimal(row["NetPrice"]);

                    if (row["DiscountAmount"] != DBNull.Value)
                        _totalDiscount += Convert.ToDecimal(row["DiscountAmount"]);

                    if (row["Price"] != DBNull.Value && row["Quantity"] != DBNull.Value)
                    {
                        _subtotal += Convert.ToDecimal(row["Price"]) * Convert.ToInt32(row["Quantity"]);
                    }
                }

                lblItemCount.Text = _totalItems.ToString();
                lblSubtotal.Text = _subtotal.ToString("C2");
                lblTotalDiscount.Text = _totalDiscount.ToString("C2");

                decimal billDiscountAmount = 0;
                if (_isBillDiscountApplied && _billDiscountPercentage > 0)
                {
                    billDiscountAmount = _subtotal * (_billDiscountPercentage / 100);
                    lblBillDiscount.Text = $"-{billDiscountAmount.ToString("C2")} ({_billDiscountPercentage}%)";
                }
                else
                {
                    lblBillDiscount.Text = string.Empty;
                }

                lblTotal.Text = (_subtotal - _totalDiscount - billDiscountAmount).ToString("C2");
            }
            catch (InvalidCastException castEx)
            {
                ShowError($"Data conversion error: {castEx.Message}\n\nPlease check item data.");
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Update Summary");
            }
        }

        private object GetSizeIdFromLabel(string sizeLabel)
        {
            try
            {
                if (string.IsNullOrEmpty(sizeLabel) || sizeLabel == "N/A")
                    return DBNull.Value;

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT Size_ID FROM Size WHERE SizeLabel = @Label", conn);
                    cmd.Parameters.AddWithValue("@Label", sizeLabel);
                    var result = cmd.ExecuteScalar();
                    return result ?? DBNull.Value;
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Get Size ID");
                return DBNull.Value;
            }
        }
        #endregion

        #region Payment Processing
        private void ProcessConfirmedPayment(string paymentMethod, decimal amountTendered,
            string cardLast4, string bankLast4, decimal change)
        {
            try
            {
                // Prepare items for stored procedure
                var items = new List<pos_system.pos.UI.Forms.Sales.BillingForm.BillItem>();
                foreach (DataRow row in _cartItems.Rows)
                {
                    items.Add(new pos_system.pos.UI.Forms.Sales.BillingForm.BillItem
                    {
                        Item_ID = row["Item_ID"] != DBNull.Value ? Convert.ToInt32(row["Item_ID"]) : 0,
                        Size_ID = GetSizeIdFromLabel(row["Size"]?.ToString()),
                        Quantity = row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0,
                        SellingPrice = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0,
                        Per_item_Discount = row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : 0
                    });
                }

                // Calculate total discount
                decimal totalDiscount = CalculateTotalDiscount();

                // Determine actual payment method for database
                string dbPaymentMethod = paymentMethod;
                object sqlCardLast4 = DBNull.Value;
                object sqlBankLast4 = DBNull.Value;
                object sqlToken = DBNull.Value;

                // Handle token payments
                if (_tokenApplied)
                {
                    sqlToken = _appliedToken.ReturnId;

                    // If token covers entire amount
                    if (amountTendered == 0 && change == 0 && paymentMethod == "Token")
                    {
                        dbPaymentMethod = null;
                    }
                }

                // Set card/bank details only for relevant payment methods
                if (paymentMethod == "Card")
                {
                    sqlCardLast4 = cardLast4;
                }
                else if (paymentMethod == "Bank Transfer")
                {
                    sqlBankLast4 = bankLast4;
                }

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                    using (var cmd = new SqlCommand("sp_ProcessSale", conn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BillID", _billId);
                        cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);
                        cmd.Parameters.AddWithValue("@PaymentMethod", dbPaymentMethod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Discount", totalDiscount);
                        cmd.Parameters.AddWithValue("@CardLast4", sqlCardLast4);
                        cmd.Parameters.AddWithValue("@BankAccountLast4", sqlBankLast4);
                        cmd.Parameters.AddWithValue("@Token_ReturnID", sqlToken);


                        // Add items parameter
                        var dt = CreateItemsDataTable(items);
                        var param = cmd.Parameters.AddWithValue("@Items", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "BillItemType";

                        // Execute with transaction
                        cmd.ExecuteNonQuery();

                        // Commit transaction
                        transaction.Commit();
                    }
                }

                // Print the bill
                PrintBill(
                    billId: _billId,
                    paymentMethod: paymentMethod,
                    amountTendered: amountTendered,
                    change: change,
                    cardLast4: cardLast4,
                    bankLast4: bankLast4
                );

                // Show success message
                ShowPaymentSuccess();

                // Reset for next bill
                ClearBill();
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                ShowError("Invalid cash payment: " + ex.Message.Replace("Cash payment should not have card/bank details.", string.Empty));
            }
            catch (SqlException ex)
            {
                HandleDatabaseError(ex);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Payment Confirmation");
            }
        }

        private DataTable CreateItemsDataTable(List<pos_system.pos.UI.Forms.Sales.BillingForm.BillItem> items)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Item_ID", typeof(int));
            dt.Columns.Add("Size_ID", typeof(int));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("SellingPrice", typeof(decimal));
            dt.Columns.Add("Per_item_Discount", typeof(decimal));

            foreach (var item in items)
            {
                dt.Rows.Add(
                    item.Item_ID,
                    item.Size_ID ?? (object)DBNull.Value,
                    item.Quantity,
                    item.SellingPrice,
                    item.Per_item_Discount
                );
            }

            return dt;
        }

        private decimal CalculateTotalDiscount()
        {
            decimal totalDiscount = 0;
            foreach (DataRow row in _cartItems.Rows)
            {
                decimal price = Convert.ToDecimal(row["Price"]);
                int qty = Convert.ToInt32(row["Quantity"]);
                decimal disc = Convert.ToDecimal(row["Discount"]);
                totalDiscount += price * qty * (disc / 100);
            }

            if (_isBillDiscountApplied && _billDiscountPercentage > 0)
            {
                totalDiscount += _subtotal * (_billDiscountPercentage / 100);
            }

            return totalDiscount;
        }

        private void PrintBill(int billId, string paymentMethod,
            decimal amountTendered, decimal change,
            string cardLast4, string bankLast4)
        {
            try
            {
                var printerName = GetPrinterName();
                if (string.IsNullOrEmpty(printerName))
                {
                    throw new Exception("Receipt printer not found. Please check printer setup.");
                }

                List<byte> output = new List<byte>();
                output.AddRange(new byte[] { 0x1B, 0x40 }); // Initialize printer

                // Shop details - CENTERED
                PrintCentered("STYLE NEWAGE", output);
                PrintCentered("No.16, Negombo Rd, Narammala.", output);
                PrintCentered("Tel: 077491913 / 0372249139", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Bill header - CENTERED
                PrintCentered($"BILL #: {billId}", output);
                PrintCentered(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), output);
                PrintCentered($"Cashier: {_currentUser.firstName} {_currentUser.lastName}", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));
                PrintSeparator(output);

                // Column headers
                PrintLeftRight("ITEM", "QTY  PRICE  TOTAL", output);
                PrintSeparator(output);

                // Print items with proper formatting
                foreach (DataRow row in _cartItems.Rows)
                {
                    string brand = row["Brand"]?.ToString() ?? string.Empty;
                    string category = row["Category"]?.ToString() ?? string.Empty;
                    string size = row["Size"]?.ToString() ?? string.Empty;
                    string itemDesc = $"{brand} {category}".Trim();

                    decimal retailPrice = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0;
                    int qty = row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0;
                    decimal discount = row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : 0;
                    decimal netPrice = row["NetPrice"] != DBNull.Value ? Convert.ToDecimal(row["NetPrice"]) : 0;
                    decimal lineTotal = retailPrice * qty;

                    // Item description (brand + category) - LEFT ALIGNED
                    PrintLeft(itemDesc, output);

                    // Size and pricing details
                    string sizeInfo = !string.IsNullOrEmpty(size) ? $"{size} " : string.Empty;
                    string priceDetails = $"{qty} x {retailPrice:0.00} {lineTotal:0.00}";
                    PrintLeftRight(sizeInfo, priceDetails, output);

                    // Discount information if applicable
                    if (discount > 0)
                    {
                        decimal discountAmount = lineTotal * (discount / 100);
                        string discountLine = $"Discount: {discount}% (-{discountAmount:0.00})";
                        PrintLeftRight(string.Empty, discountLine, output);
                        PrintLeftRight(string.Empty, $"Net: {netPrice:0.00}", output);
                    }
                    else
                    {
                        PrintLeftRight(string.Empty, $"Total: {lineTotal:0.00}", output);
                    }

                    output.AddRange(Encoding.ASCII.GetBytes("\n"));
                }

                PrintSeparator(output);
                PrintLeftRight("BILL TOTAL:", _subtotal.ToString("C2"), output);
                PrintSeparator(output);
                // Print total amount
                PrintLeftRight("TOTAL:", lblTotal.Text, output);
                PrintSeparator(output);

                // Discount summary
                if (_totalDiscount > 0)
                {
                    PrintLeftRight("Per-Item Discount:", $"-{_totalDiscount:C2}", output);
                }

                if (_isBillDiscountApplied && _billDiscountPercentage > 0)
                {
                    decimal billDiscount = _subtotal * (_billDiscountPercentage / 100);
                    PrintLeftRight("Bill Discount:", $"-{billDiscount:C2} ({_billDiscountPercentage}%)", output);
                }

                // Token information
                if (_tokenApplied)
                {
                    PrintCentered("PAYMENT WITH RETURN TOKEN", output);
                    PrintLeftRight("Token Value:", _appliedToken.TotalRefund.ToString("C2"), output);
                }

                // Payment details
                if (paymentMethod == "Cash")
                {
                    PrintLeftRight("Cash Tendered:", amountTendered.ToString("C2"), output);
                    PrintLeftRight("Change:", change.ToString("C2"), output);
                }
                else if (paymentMethod == "Card")
                {
                    PrintLeftRight("Card Payment:", amountTendered.ToString("C2"), output);
                    PrintLeftRight("Last 4 Digits:", cardLast4, output);
                }
                else if (paymentMethod == "Bank Transfer")
                {
                    PrintLeftRight("Bank Transfer:", amountTendered.ToString("C2"), output);
                    PrintLeftRight("Last 4 Digits:", bankLast4, output);
                }
                else if (paymentMethod == "Token")
                {
                    PrintLeftRight("Fully Paid with Token", _appliedToken.TotalRefund.ToString("C2"), output);
                }

                output.AddRange(Encoding.ASCII.GetBytes("\n"));



                // Thank you message - CENTERED
                PrintCentered("Thank you for your purchase!", output);
                PrintCentered("Come again!", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Printer commands
                output.AddRange(Encoding.ASCII.GetBytes("\n\n\n")); // Feed paper
                output.AddRange(new byte[] { 0x1B, 0x64, 0x02 }); // Cut paper
                output.AddRange(new byte[] { 0x1B, 0x69 }); // Open cash drawer

                // Send to printer
                RawPrinterHelper.SendBytesToPrinter(printerName, output.ToArray());
            }
            catch (Exception ex)
            {
                ShowError($"Printing failed: {ex.Message}");
            }
        }

        private void PrintCentered(string text, List<byte> output)
        {
            try
            {
                if (text.Length > MAX_LINE_WIDTH)
                    text = text.Substring(0, MAX_LINE_WIDTH);

                int spaces = (MAX_LINE_WIDTH - text.Length) / 2;
                output.AddRange(Encoding.ASCII.GetBytes(new string(' ', spaces) + text + "\n"));
            }
            catch (Exception ex)
            {
                output.AddRange(Encoding.ASCII.GetBytes(text + "\n"));
                Console.WriteLine($"Centered print error: {ex.Message}");
            }
        }

        private void PrintLeft(string text, List<byte> output)
        {
            try
            {
                text = TruncateString(text, MAX_LINE_WIDTH);
                output.AddRange(Encoding.ASCII.GetBytes(text + "\n"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Left print error: {ex.Message}");
            }
        }

        private void PrintLeftRight(string left, string right, List<byte> output)
        {
            try
            {
                int availableSpace = MAX_LINE_WIDTH - right.Length - 1;
                if (availableSpace < 1) availableSpace = 1;

                left = TruncateString(left, availableSpace);
                string line = left.PadRight(availableSpace) + " " + right;
                output.AddRange(Encoding.ASCII.GetBytes(line + "\n"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Left-right print error: {ex.Message}");
            }
        }

        private void PrintSeparator(List<byte> output)
        {
            try
            {
                output.AddRange(Encoding.ASCII.GetBytes(new string('-', MAX_LINE_WIDTH) + "\n"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Separator print error: {ex.Message}");
            }
        }

        private string TruncateString(string input, int maxLength)
        {
            try
            {
                if (string.IsNullOrEmpty(input)) return string.Empty;
                return input.Length <= maxLength ? input : input.Substring(0, maxLength - 3) + "...";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Truncate error: {ex.Message}");
                return input.Length <= maxLength ? input : input.Substring(0, maxLength);
            }
        }

        private string GetPrinterName()
        {
            try
            {
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    if (printer.Contains(PRINTER_NAME))
                    {
                        return printer;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Printer Detection");
                return null;
            }
        }

        private void ShowPaymentSuccess()
        {
            string message = "Payment processed successfully!\n\n" +
                             $"Bill ID: {_billId}\n" +
                             $"Total: {lblTotal.Text}";

            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Error Handling
        private void HandleUnexpectedError(Exception ex, string context)
        {
            try
            {
                string errorMessage = $"An unexpected error occurred in {context}:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
                MessageBox.Show(errorMessage, "Unexpected Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (context == "Initialization")
                {
                    MessageBox.Show("Application may not function properly. Please restart.",
                        "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch
            {
                MessageBox.Show("A critical error occurred. Please restart the application.",
                    "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void HandleDatabaseError(SqlException ex)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Database Error:");
                sb.AppendLine($"Error Number: {ex.Number}");
                sb.AppendLine($"Message: {ex.Message}");

                if (ex.Number == 547)
                {
                    sb.AppendLine("\nPossible reasons:");
                    sb.AppendLine("- Insufficient stock for some items");
                    sb.AppendLine("- Invalid item references");
                }

                MessageBox.Show(sb.ToString(), "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception handlerEx)
            {
                HandleUnexpectedError(handlerEx, "Database Error Handling");
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Processing Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_dateTimer != null)
        //        {
        //            _dateTimer.Stop();
        //            _dateTimer.Dispose();
        //        }
        //    }
        //    base.Dispose(disposing);
        //}
        #endregion
    }
}