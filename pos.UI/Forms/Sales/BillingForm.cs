using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Utilities;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using pos_system.pos.UI.Forms.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class BillingForm : Form
    {
        #region Constants and Fields
        private const string PRINTER_NAME = "Xprinter XP-80";
        private const int MAX_LINE_WIDTH = 32;
        private System.Timers.Timer _dateTimer;

        private Employee _currentUser;
        private DataTable _cartItems;
        private int _totalItems = 0;
        private decimal _total = 0;
        private int _billId = 0;
        private decimal _totalDiscount = 0;
        private decimal _subtotal = 0;
        private decimal _billDiscountPercentage = 0;
        private bool _isBillDiscountApplied = false;
        private bool _isPerItemDiscountApplied = false;
        private bool _discountConflictWarningShown = false;
        private ReturnToken _appliedToken;
        private bool _tokenApplied;

        #endregion

        #region Helper Classes
        public class ReturnToken
        {
            public int ReturnId { get; set; }
            public decimal TotalRefund { get; set; }
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
            _dateTimer = new System.Timers.Timer(1000);
            _dateTimer.Elapsed += UpdateDateTime;
            _dateTimer.AutoReset = true;
            _dateTimer.Enabled = true;
            UpdateDateTime(null, null);
        }

        private void UpdateDateTime(object sender, ElapsedEventArgs e)
        {
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

            DataGridViewTextBoxColumn colProductSizeId = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductSize_ID",
                HeaderText = "ID",
                Visible = false
            };

            DataGridViewTextBoxColumn colProductId = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Product_ID",
                HeaderText = "ProdID",
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
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            DataGridViewTextBoxColumn colDiscountAmount = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DiscountAmount",
                HeaderText = "Discount Amt",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
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
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
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
                colProductSizeId,
                colProductId,
                colBarcode,
                colBrand,
                colCategory,
                colDescription,
                colSize,
                colPrice,
                colQuantity,
                colDiscount,
                colDiscountAmount,
                colNetPrice,
                colMaxDiscount,
                colAvailableStock,
                colDelete
            });
        }

        private void InitializeCartDataTable()
        {
            try
            {
                _cartItems = new DataTable();
                _cartItems.Columns.Add("ProductSize_ID", typeof(int));
                _cartItems.Columns.Add("Product_ID", typeof(int));
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
                                if (reader.GetBoolean(2))
                                {
                                    ShowError("Token has already been used");
                                    return;
                                }

                                _appliedToken = new ReturnToken
                                {
                                    ReturnId = reader.GetInt32(0),
                                    TotalRefund = reader.GetDecimal(1)
                                };

                                _tokenApplied = true;
                                txtTokenId.Enabled = false;
                                btnApplyToken.Enabled = false;

                                MessageBox.Show($"Token applied successfully! Value: Rs.{_appliedToken.TotalRefund:N2}",
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

                decimal totalAmount = Convert.ToDecimal(_total);
                decimal tokenValue = _tokenApplied ? _appliedToken.TotalRefund : 0;

                if (_tokenApplied && totalAmount < tokenValue)
                {
                    ShowError($"Purchase amount must be >= token value (RS.{tokenValue})");
                    return;
                }

                using (var paymentForm = new PaymentForm(totalAmount, tokenValue))
                {
                    if (paymentForm.ShowDialog() == DialogResult.OK && paymentForm.IsConfirmed)
                    {
                        ProcessConfirmedPayment(
                            paymentMethod: paymentForm.PaymentMethod,
                            amountTendered: paymentForm.AmountTendered,
                            cardLast4: paymentForm.CardLastFour,
                            bankLast4: paymentForm.BankLastFour,
                            change: paymentForm.Change,
                            customerContact: paymentForm.CustomerContact
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

                string barcode = txtBarcode.Text.Trim();
                int quantity = 1;

                if (barcode.Contains("x"))
                {
                    var parts = barcode.Split('x');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int qty) && qty > 0)
                    {
                        quantity = qty;
                        barcode = parts[1].Trim();
                    }
                }

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            p.Product_ID, 
                            p.description, 
                            p.barcode, 
                            p.maxDiscount,
                            b.brandName, 
                            c.categoryName,
                            ps.ProductSize_ID,  -- Explicitly select column
                            ps.Size_ID, 
                            s.SizeLabel, 
                            ps.quantity AS AvailableStock, 
                            ps.RetailPrice,
                            ps.unitCost
                        FROM Product p
                        INNER JOIN Brand b ON p.Brand_ID = b.Brand_ID
                        INNER JOIN Category c ON p.Category_ID = c.Category_ID
                        INNER JOIN ProductSize ps ON p.Product_ID = ps.Product_ID
                        LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
                        WHERE p.barcode = @Barcode AND p.IsDeleted = 0 AND ps.quantity > 0";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Barcode", barcode);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count == 0)
                            {
                                ShowError("Item not found or out of stock");
                                return;
                            }

                            // DEBUG: Check columns
                            Console.WriteLine("\nColumns in result:");
                            foreach (DataColumn col in dt.Columns)
                            {
                                Console.WriteLine($"{col.ColumnName}");
                            }

                            // Single size handling
                            if (dt.Rows.Count == 1)
                            {
                                AddItemToCartInternal(dt.Rows[0], quantity);
                            }
                            // Multiple sizes handling
                            else
                            {
                                using (var sizeForm = new SizeSelectionForm(dt))
                                {
                                    if (sizeForm.ShowDialog() == DialogResult.OK)
                                    {
                                        // Case-insensitive access
                                        var selectedRow = dt.AsEnumerable()
                                            .FirstOrDefault(r =>
                                                Convert.ToInt32(r["ProductSize_ID"]) == sizeForm.SelectedProductSizeId);

                                        if (selectedRow != null)
                                        {
                                            AddItemToCartInternal(selectedRow, quantity);
                                        }
                                        else
                                        {
                                            ShowError("Selected size not found in database");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Add Item");
            }
        }

        private void AddItemToCartInternal(DataRow itemRow, int quantity)
        {
            try
            {
                int productSizeId = itemRow.Field<int>("ProductSize_ID");
                int productId = itemRow.Field<int>("Product_ID");
                int availableStock = itemRow.Field<int>("AvailableStock");
                string size = itemRow["SizeLabel"] != DBNull.Value ?
                    itemRow.Field<string>("SizeLabel") : "N/A";

                // Check if item already in cart
                var existingRow = _cartItems.AsEnumerable()
                    .FirstOrDefault(row => row.Field<int>("ProductSize_ID") == productSizeId);

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
                        productSizeId,
                        productId,
                        itemRow.Field<string>("barcode"),
                        itemRow.Field<string>("brandName"),
                        itemRow.Field<string>("categoryName"),
                        itemRow.Field<string>("description"),
                        size,
                        itemRow.Field<decimal>("RetailPrice"),
                        quantity,
                        0, // Discount %
                        0, // DiscountAmount
                        0, // NetPrice
                        itemRow.Field<decimal>("maxDiscount"),
                        availableStock
                    );
                }

                txtBarcode.Clear();
                txtBarcode.Focus();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Add Item to Cart");
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
                lblSubtotal.Text = $"Rs.{_subtotal:0.00}";
                lblTotalDiscount.Text = $"Rs.{_totalDiscount:0.00}";

                decimal billDiscountAmount = 0;
                if (_isBillDiscountApplied && _billDiscountPercentage > 0)
                {
                    billDiscountAmount = Math.Round((_subtotal * (_billDiscountPercentage / 100)), 2);
                    lblBillDiscount.Text = $"-{Convert.ToDecimal(billDiscountAmount)} ({_billDiscountPercentage}%)";
                }
                else
                {
                    lblBillDiscount.Text = string.Empty;
                }

                lblTotal.Text = $"Rs.{Convert.ToDecimal(_subtotal - _totalDiscount - billDiscountAmount)}";
                _total = _subtotal - _totalDiscount - billDiscountAmount;
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
        #endregion

        #region Payment Processing
        private void ProcessConfirmedPayment(string paymentMethod, decimal amountTendered,
            string cardLast4, string bankLast4, decimal change, string customerContact)
        {
            try
            {
                // Prepare items for stored procedure
                var items = new List<BillItem>();
                foreach (DataRow row in _cartItems.Rows)
                {
                    items.Add(new BillItem
                    {
                        ProductSize_ID = row["ProductSize_ID"] != DBNull.Value ?
                            Convert.ToInt32(row["ProductSize_ID"]) : 0,
                        Quantity = row["Quantity"] != DBNull.Value ?
                            Convert.ToInt32(row["Quantity"]) : 0,
                        SellingPrice = row["Price"] != DBNull.Value ?
                            Convert.ToDecimal(row["Price"]) : 0,
                        Per_item_Discount = row["Discount"] != DBNull.Value ?
                            Convert.ToDecimal(row["Discount"]) : 0
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
                        cmd.Parameters.AddWithValue("@CustomerContact", customerContact ?? (object)DBNull.Value);

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

        private DataTable CreateItemsDataTable(List<BillItem> items)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductSize_ID", typeof(int));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("SellingPrice", typeof(decimal));
            dt.Columns.Add("Per_item_Discount", typeof(decimal));

            foreach (var item in items)
            {
                dt.Rows.Add(
                    item.ProductSize_ID,
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
                PrintCentered("Tel: 0777491913 / 0372249139", output);
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
                    int productSizeId = row.Field<int>("ProductSize_ID");

                    using (var conn = DbHelper.GetConnection())
                    {
                        conn.Open();
                        string query = @"
                            SELECT 
                                p.description, p.barcode, 
                                b.brandName, c.categoryName,
                                s.SizeLabel, ps.RetailPrice
                            FROM ProductSize ps
                            INNER JOIN Product p ON ps.Product_ID = p.Product_ID
                            INNER JOIN Brand b ON p.Brand_ID = b.Brand_ID
                            INNER JOIN Category c ON p.Category_ID = c.Category_ID
                            LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
                            WHERE ps.ProductSize_ID = @ProductSizeId";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProductSizeId", productSizeId);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string brand = reader["brandName"].ToString();
                                    string category = reader["categoryName"].ToString();
                                    string size = reader["SizeLabel"] != DBNull.Value ?
                                        reader["SizeLabel"].ToString() : string.Empty;
                                    string itemDesc = $"{category}".Trim();

                                    decimal retailPrice = Convert.ToDecimal(row["Price"]);
                                    int qty = Convert.ToInt32(row["Quantity"]);
                                    decimal discount = Convert.ToDecimal(row["Discount"]);
                                    decimal netPrice = Convert.ToDecimal(row["NetPrice"]);
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
                                        PrintLeftRight(string.Empty, $"Total: Rs.{lineTotal:0.00}", output);
                                    }

                                    output.AddRange(Encoding.ASCII.GetBytes("\n"));
                                }
                            }
                        }
                    }
                }

                PrintSeparator(output);
                // Discount summary
                if (_totalDiscount > 0)
                {
                    PrintLeftRight("SUB TOTAL:", $"Rs.{_subtotal:0.00}", output);
                    PrintLeftRight("Discount:", $"Rs.-{Math.Round(_totalDiscount, 2)}", output);
                }

                if (_isBillDiscountApplied && _billDiscountPercentage > 0)
                {
                    PrintLeftRight("SUB TOTAL:", $"Rs.{Math.Round(_subtotal, 2)}", output);
                    decimal billDiscount = Math.Round(_subtotal * (_billDiscountPercentage / 100), 2);
                    PrintLeftRight("Discount:", $"Rs.-{Math.Round(billDiscount, 2)} ({_billDiscountPercentage}%)", output);
                }
                PrintLeftRight("TOTAL:", $"Rs.{Math.Round(_total, 2)}", output);
                PrintSeparator(output);
                // Token information
                if (_tokenApplied)
                {
                    PrintLeftRight("Return Value:", $"Rs.{Math.Round(_appliedToken.TotalRefund, 2)}", output);
                }

                // Payment details
                if (paymentMethod == "Cash")
                {
                    PrintLeftRight("Cash Tendered:", $"Rs.{amountTendered:0.00}", output);
                    PrintLeftRight("Change:", $"Rs.{change:0.00}", output);
                }
                else if (paymentMethod == "Card")
                {
                    PrintLeftRight("Card Payment:", $"Rs.{amountTendered.ToString()}", output);
                    PrintLeftRight("Last 4 Digits:", cardLast4, output);
                }
                else if (paymentMethod == "Bank Transfer")
                {
                    PrintLeftRight("Bank Transfer:", $"Rs.{amountTendered.ToString()}", output);
                    PrintLeftRight("Last 4 Digits:", bankLast4, output);
                }
                else if (paymentMethod == "Token")
                {
                    PrintLeftRight("Fully Paid with Token", $"Rs.{_appliedToken.TotalRefund.ToString()}", output);
                }

                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Thank you message - CENTERED
                PrintCentered("Thank you for your purchase!", output);
                PrintCentered("Come Again!", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                PrintCentered("-------IMPORTANT NOTICE-------", output);
                PrintCentered("Returns accepted in 3 days with", output);
                PrintCentered("tag & receipt.", output);
                PrintCentered("No cash refunds on returns.", output);
                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Printer commands
                output.AddRange(Encoding.ASCII.GetBytes("\n\n\n")); // Feed paper
                output.AddRange(new byte[] { 0x1B, 0x70, 0x00, 0x14, 0x50 });// Open cash drawer
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
                             $"Total: Rs.{_total}";

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dateTimer != null)
                {
                    _dateTimer.Stop();
                    _dateTimer.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}