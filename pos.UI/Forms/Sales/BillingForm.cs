using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using pos_system.pos.BLL.Utilities;
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
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class BillingForm : Form
    {
        #region Constants and Fields
        private const string PRINTER_NAME = "XP-80C";
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

        //Mixed Payment Fields
        private decimal _totalAmount;
        private string _cardLastFour;
        private string _bankLastFour;
        private string _firstCardLastFour;
        private string _firstBankLastFour;
        private string _secondCardLastFour;
        private string _secondBankLastFour;
        private string _customerContact;
        private string _customerGender;

        // Barcode scanning fields
        private TextBox _txtBarcodeInput;
        private DateTime _lastScanTime = DateTime.MinValue;
        private bool _isEditingGrid = false;

        // Queue Management
        private List<QueuedBill> _queuedBills = new List<QueuedBill>();

        #endregion

        #region Constructor and Initialization
        public BillingForm(Employee user)
        {
            try
            {
                _currentUser = user;
                InitializeComponent();
                InitializeBarcodeScanner();
                InitializeDataGridView();
                container.Controls.Add(dgvCart, 0, 3);
                dgvCart.Dock = DockStyle.Fill;
                InitializeCartDataTable();
                GenerateBillId();
                InitializeDateTimeTimer();
                AttachEventHandlers();
                this.Load += (s, e) => AttachGridEventHandlers();
                LoadQueuedBills();
                SetupKeyboardShortcuts();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Initialization");
            }

        }

        private void InitializeBarcodeScanner()
        {
            // Create hidden TextBox for barcode scanning
            _txtBarcodeInput = new TextBox
            {
                Location = new Point(-100, -100), // Position off-screen
                Size = new Size(1, 1), // Minimal size
                TabIndex = 0
            };
            this.Controls.Add(_txtBarcodeInput);

            // Set up event handlers for scanner input
            _txtBarcodeInput.KeyDown += TxtBarcodeInput_KeyDown;

            // Set focus to scanner input when form is shown
            this.Shown += (s, e) => FocusBarcodeScanner();
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

        private void SetupKeyboardShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += BillingForm_KeyDown;
        }

        #endregion

        #region Queue Management Methods
        private void LoadQueuedBills()
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_GetQueuedBills", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);

                        using (var reader = cmd.ExecuteReader())
                        {
                            _queuedBills.Clear();
                            while (reader.Read())
                            {
                                try
                                {
                                    var queuedBill = new QueuedBill
                                    {
                                        Queue_ID = reader.GetInt32("Queue_ID"),
                                        Bill_ID = reader.GetInt32("Bill_ID"),
                                        QueuePosition = reader.GetInt32("QueuePosition"),
                                        PausedAt = reader.GetDateTime("PausedAt"),
                                        CartData = reader.GetString("CartData")
                                    };

                                    // Safely handle ItemCount conversion
                                    var itemCountValue = reader["ItemCount"];
                                    if (itemCountValue != DBNull.Value)
                                    {
                                        queuedBill.ItemCount = Convert.ToInt32(itemCountValue);
                                    }
                                    else
                                    {
                                        queuedBill.ItemCount = 0;
                                    }

                                    // Safely handle SubTotal conversion
                                    var subTotalValue = reader["SubTotal"];
                                    if (subTotalValue != DBNull.Value)
                                    {
                                        queuedBill.SubTotal = Convert.ToDecimal(subTotalValue);
                                    }
                                    else
                                    {
                                        queuedBill.SubTotal = 0m;
                                    }

                                    _queuedBills.Add(queuedBill);
                                }
                                catch (Exception rowEx)
                                {
                                    // Log the error but continue processing other rows
                                    Console.WriteLine($"Error processing queued bill: {rowEx.Message}");
                                    // Add a basic bill without the problematic fields
                                    _queuedBills.Add(new QueuedBill
                                    {
                                        Queue_ID = reader.GetInt32("Queue_ID"),
                                        Bill_ID = reader.GetInt32("Bill_ID"),
                                        QueuePosition = reader.GetInt32("QueuePosition"),
                                        PausedAt = reader.GetDateTime("PausedAt"),
                                        CartData = reader.GetString("CartData"),
                                        ItemCount = 0,
                                        SubTotal = 0m
                                    });
                                }
                            }
                        }
                    }
                }

                UpdateQueueBadge();
            }
            catch (SqlException ex) when (ex.Number == 2812) // Procedure not found
            {
                // If stored procedure doesn't exist, just clear the queue
                _queuedBills.Clear();
                UpdateQueueBadge();
                Console.WriteLine("Queue procedure not found - continuing without queue functionality");
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Load Queued Bills");
            }
        }

        private bool CheckStoredProcedureExists(string procedureName)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.ROUTINES 
                WHERE ROUTINE_TYPE = 'PROCEDURE' 
                AND ROUTINE_NAME = @ProcedureName", conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcedureName", procedureName);
                        var result = Convert.ToInt32(cmd.ExecuteScalar());
                        return result > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void UpdateQueueBadge()
        {
            int queuedCount = _queuedBills.Count;
            btnViewQueuedBills.Text = queuedCount > 0 ?
                $"Queued Bills ({queuedCount}) (F3)" : "Queued Bills (F3)";
        }

        //private void BtnPauseBill_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (_cartItems.Rows.Count == 0)
        //        {
        //            ThemedMessageBox.Show("Cannot pause an empty bill", "Warning",
        //                ThemedMessageBoxIcon.Warning);
        //            return;
        //        }

        //        var result = ThemedMessageBoxYesNo.Show(
        //            "Pause current bill and start new one? You can restore it later.",
        //            "Pause Bill");

        //        if (result != DialogResult.Yes) return;

        //        // Serialize cart data
        //        var cartData = new CartData
        //        {
        //            Items = new List<CartItem>(),
        //            Subtotal = _subtotal,
        //            TotalDiscount = _totalDiscount,
        //            BillDiscountPercentage = _billDiscountPercentage,
        //            IsBillDiscountApplied = _isBillDiscountApplied,
        //            AppliedToken = _appliedToken,
        //            TokenApplied = _tokenApplied,
        //            ItemCount = _totalItems
        //        };

        //        foreach (DataRow row in _cartItems.Rows)
        //        {
        //            cartData.Items.Add(new CartItem
        //            {
        //                ProductSize_ID = Convert.ToInt32(row["ProductSize_ID"]),
        //                Product_ID = Convert.ToInt32(row["Product_ID"]),
        //                Barcode = row["Barcode"]?.ToString() ?? string.Empty,
        //                Brand = row["Brand"]?.ToString() ?? string.Empty,
        //                Category = row["Category"]?.ToString() ?? string.Empty,
        //                Description = row["Description"]?.ToString() ?? string.Empty,
        //                Size = row["Size"]?.ToString() ?? string.Empty,
        //                Price = Convert.ToDecimal(row["Price"]),
        //                Quantity = Convert.ToInt32(row["Quantity"]),
        //                DiscountAmountPerItem = Convert.ToDecimal(row["DiscountAmountPerItem"]),
        //                MaxDiscount = Convert.ToDecimal(row["MaxDiscount"]),
        //                AvailableStock = Convert.ToInt32(row["AvailableStock"])
        //            });
        //        }

        //        string serializedCart = JsonConvert.SerializeObject(cartData,
        //            new JsonSerializerSettings
        //            {
        //                NullValueHandling = NullValueHandling.Ignore,
        //                DefaultValueHandling = DefaultValueHandling.Include
        //            });

        //        // Save to database with CURRENT Bill_ID
        //        using (var conn = DbHelper.GetConnection())
        //        {
        //            conn.Open();
        //            using (var cmd = new SqlCommand("sp_PauseCurrentBill", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);
        //                cmd.Parameters.AddWithValue("@BillID", _billId); // Use current Bill_ID
        //                cmd.Parameters.AddWithValue("@CartData", serializedCart);

        //                using (var reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        var queuePosition = Convert.ToInt32(reader["QueuePosition"]);
        //                        var newBillId = Convert.ToInt32(reader["NewBillID"]);

        //                        ThemedMessageBox.Show(
        //                            $"Bill #{_billId} paused and added to queue (Position: {queuePosition}). New Bill ID: {newBillId}",
        //                            "Bill Paused", ThemedMessageBoxIcon.Information);

        //                        // Set the new bill ID for the next bill
        //                        _billId = newBillId;
        //                        lblBillId.Text = $"Bill ID: {_billId}";
        //                    }
        //                }
        //            }
        //        }

        //        // Clear current bill data but keep the new Bill_ID
        //        _cartItems.Rows.Clear();
        //        _billDiscountPercentage = 0;
        //        _isBillDiscountApplied = false;
        //        _discountConflictWarningShown = false;
        //        _tokenApplied = false;
        //        _appliedToken = null;
        //        lblBillDiscount.Text = string.Empty;

        //        // Reset token button
        //        btnApplyToken.Text = "Apply Token";
        //        btnApplyToken.BackColor = Color.MediumPurple;
        //        btnApplyToken.Enabled = true;

        //        UpdateSummary();
        //        FocusBarcodeScanner();
        //        LoadQueuedBills();
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleUnexpectedError(ex, "Pause Bill");
        //    }
        //}


        //private void BtnPauseBill_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (_cartItems.Rows.Count == 0)
        //        {
        //            ThemedMessageBox.Show("Cannot pause an empty bill", "Warning",
        //                ThemedMessageBoxIcon.Warning);
        //            return;
        //        }

        //        var result = ThemedMessageBoxYesNo.Show(
        //            "Pause current bill and start new one? You can restore it later.",
        //            "Pause Bill");

        //        if (result != DialogResult.Yes) return;

        //        // Check if this is a re-pause scenario (previously restored bill)
        //        int? originalQueueId = this.Tag as int?;
        //        bool isRepause = originalQueueId.HasValue;

        //        // Serialize cart data
        //        var cartData = new CartData
        //        {
        //            Items = new List<CartItem>(),
        //            Subtotal = _subtotal,
        //            TotalDiscount = _totalDiscount,
        //            BillDiscountPercentage = _billDiscountPercentage,
        //            IsBillDiscountApplied = _isBillDiscountApplied,
        //            AppliedToken = _appliedToken,
        //            TokenApplied = _tokenApplied,
        //            ItemCount = _totalItems,
        //            IsRepause = isRepause, // Track if this is a re-pause
        //            OriginalQueueId = originalQueueId // Store original queue ID
        //        };

        //        foreach (DataRow row in _cartItems.Rows)
        //        {
        //            cartData.Items.Add(new CartItem
        //            {
        //                ProductSize_ID = Convert.ToInt32(row["ProductSize_ID"]),
        //                Product_ID = Convert.ToInt32(row["Product_ID"]),
        //                Barcode = row["Barcode"]?.ToString() ?? string.Empty,
        //                Brand = row["Brand"]?.ToString() ?? string.Empty,
        //                Category = row["Category"]?.ToString() ?? string.Empty,
        //                Description = row["Description"]?.ToString() ?? string.Empty,
        //                Size = row["Size"]?.ToString() ?? string.Empty,
        //                Price = Convert.ToDecimal(row["Price"]),
        //                Quantity = Convert.ToInt32(row["Quantity"]),
        //                DiscountAmountPerItem = Convert.ToDecimal(row["DiscountAmountPerItem"]),
        //                MaxDiscount = Convert.ToDecimal(row["MaxDiscount"]),
        //                AvailableStock = Convert.ToInt32(row["AvailableStock"])
        //            });
        //        }

        //        string serializedCart = JsonConvert.SerializeObject(cartData,
        //            new JsonSerializerSettings
        //            {
        //                NullValueHandling = NullValueHandling.Ignore,
        //                DefaultValueHandling = DefaultValueHandling.Include
        //            });

        //        // Save to database
        //        using (var conn = DbHelper.GetConnection())
        //        {
        //            conn.Open();

        //            if (isRepause)
        //            {
        //                // Update existing queue entry for re-pause scenario
        //                using (var cmd = new SqlCommand("sp_RepauseBill", conn))
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.Parameters.AddWithValue("@OriginalQueueID", originalQueueId);
        //                    cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);
        //                    cmd.Parameters.AddWithValue("@BillID", _billId);
        //                    cmd.Parameters.AddWithValue("@CartData", serializedCart);

        //                    using (var reader = cmd.ExecuteReader())
        //                    {
        //                        if (reader.Read())
        //                        {
        //                            var queuePosition = Convert.ToInt32(reader["QueuePosition"]);
        //                            var newBillId = Convert.ToInt32(reader["NewBillID"]);

        //                            ThemedMessageBox.Show(
        //                                $"Bill #{_billId} re-paused and updated in queue (Position: {queuePosition}). New Bill ID: {newBillId}",
        //                                "Bill Re-paused", ThemedMessageBoxIcon.Information);

        //                            _billId = newBillId;
        //                            lblBillId.Text = $"Bill ID: {_billId}";

        //                            // Clear the repause tracking
        //                            this.Tag = null;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // Normal pause for new bill
        //                using (var cmd = new SqlCommand("sp_PauseCurrentBill", conn))
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);
        //                    cmd.Parameters.AddWithValue("@BillID", _billId);
        //                    cmd.Parameters.AddWithValue("@CartData", serializedCart);

        //                    using (var reader = cmd.ExecuteReader())
        //                    {
        //                        if (reader.Read())
        //                        {
        //                            var queuePosition = Convert.ToInt32(reader["QueuePosition"]);
        //                            var newBillId = Convert.ToInt32(reader["NewBillID"]);

        //                            ThemedMessageBox.Show(
        //                                $"Bill #{_billId} paused and added to queue (Position: {queuePosition}). New Bill ID: {newBillId}",
        //                                "Bill Paused", ThemedMessageBoxIcon.Information);

        //                            _billId = newBillId;
        //                            lblBillId.Text = $"Bill ID: {_billId}";
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // Clear current bill data but keep the new Bill_ID
        //        ClearCurrentBillData();
        //        LoadQueuedBills();
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleUnexpectedError(ex, "Pause Bill");
        //    }
        //}

        //private void ClearCurrentBillData()
        //{
        //    _cartItems.Rows.Clear();
        //    _billDiscountPercentage = 0;
        //    _isBillDiscountApplied = false;
        //    _discountConflictWarningShown = false;
        //    _tokenApplied = false;
        //    _appliedToken = null;
        //    lblBillDiscount.Text = string.Empty;

        //    // Reset token button
        //    btnApplyToken.Text = "Apply Token";
        //    btnApplyToken.BackColor = Color.MediumPurple;
        //    btnApplyToken.Enabled = true;

        //    UpdateSummary();
        //    FocusBarcodeScanner();
        //}

        private void BtnPauseBill_Click(object sender, EventArgs e)
        {
            try
            {
                if (_cartItems.Rows.Count == 0)
                {
                    ThemedMessageBox.Show("Cannot pause an empty bill", "Warning",
                        ThemedMessageBoxIcon.Warning);
                    return;
                }

                var result = ThemedMessageBoxYesNo.Show(
                    "Pause current bill and start new one? You can restore it later.",
                    "Pause Bill");

                if (result != DialogResult.Yes) return;

                // Check if this is a re-pause scenario (previously restored bill)
                int? originalQueueId = this.Tag as int?;
                bool isRepause = originalQueueId.HasValue;

                // Serialize cart data
                var cartData = new CartData
                {
                    Items = new List<CartItem>(),
                    Subtotal = _subtotal,
                    TotalDiscount = _totalDiscount,
                    BillDiscountPercentage = _billDiscountPercentage,
                    IsBillDiscountApplied = _isBillDiscountApplied,
                    AppliedToken = _appliedToken,
                    TokenApplied = _tokenApplied,
                    ItemCount = _totalItems
                };

                foreach (DataRow row in _cartItems.Rows)
                {
                    cartData.Items.Add(new CartItem
                    {
                        ProductSize_ID = Convert.ToInt32(row["ProductSize_ID"]),
                        Product_ID = Convert.ToInt32(row["Product_ID"]),
                        Barcode = row["Barcode"]?.ToString() ?? string.Empty,
                        Brand = row["Brand"]?.ToString() ?? string.Empty,
                        Category = row["Category"]?.ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString() ?? string.Empty,
                        Size = row["Size"]?.ToString() ?? string.Empty,
                        Price = Convert.ToDecimal(row["Price"]),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        DiscountAmountPerItem = Convert.ToDecimal(row["DiscountAmountPerItem"]),
                        MaxDiscount = Convert.ToDecimal(row["MaxDiscount"]),
                        AvailableStock = Convert.ToInt32(row["AvailableStock"])
                    });
                }

                string serializedCart = JsonConvert.SerializeObject(cartData,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Include
                    });

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    if (isRepause)
                    {
                        // RE-PAUSE SCENARIO: Handle foreign key constraint properly

                        // 1. FIRST: Ensure the Bill record exists with the correct status
                        string checkBillQuery = "SELECT COUNT(*) FROM Bill WHERE Bill_ID = @BillID";
                        using (var cmd = new SqlCommand(checkBillQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@BillID", _billId);
                            int billExists = Convert.ToInt32(cmd.ExecuteScalar());

                            if (billExists > 0)
                            {
                                // Update existing bill status to Paused
                                string updateBillQuery = @"
                                        UPDATE Bill 
                                        SET BillStatus = 'Paused', [date] = GETDATE()
                                        WHERE Bill_ID = @BillID";
                                using (var updateCmd = new SqlCommand(updateBillQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@BillID", _billId);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insert new bill record FIRST
                                string insertBillQuery = @"
                                        INSERT INTO Bill (Bill_ID, Employee_ID, BillStatus, [date])
                                        VALUES (@BillID, @EmployeeID, 'Paused', GETDATE())";
                                using (var insertCmd = new SqlCommand(insertBillQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@BillID", _billId);
                                    insertCmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // 2. SECOND: Now update the BillQueue since the Bill record exists
                        string updateQueueQuery = @"
                                UPDATE BillQueue 
                                SET 
                                    Bill_ID = @BillID,
                                    CartData = @CartData,
                                    PausedAt = GETDATE(),
                                    IsActive = 1
                                WHERE Queue_ID = @OriginalQueueID AND Employee_ID = @EmployeeID";

                        using (var cmd = new SqlCommand(updateQueueQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@BillID", _billId);
                            cmd.Parameters.AddWithValue("@CartData", serializedCart);
                            cmd.Parameters.AddWithValue("@OriginalQueueID", originalQueueId.Value);
                            cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new Exception("Failed to update existing queue entry");
                            }
                        }

                        // Get the queue position for display
                        string getPositionQuery = @"
                                SELECT QueuePosition 
                                FROM BillQueue 
                                WHERE Queue_ID = @QueueID";

                        int queuePosition;
                        using (var cmd = new SqlCommand(getPositionQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@QueueID", originalQueueId.Value);
                            queuePosition = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Generate new Bill_ID for next bill
                        int newBillId = GenerateNewBillId(conn);

                        ThemedMessageBox.Show(
                            $"Bill #{_billId} re-paused and updated in queue (Position: {queuePosition}). New Bill ID: {newBillId}",
                            "Bill Re-paused", ThemedMessageBoxIcon.Information);

                        _billId = newBillId;
                        lblBillId.Text = $"Bill ID: {_billId}";

                        // Clear the repause tracking
                        this.Tag = null;
                    }
                    else
                    {
                        // NORMAL PAUSE SCENARIO: Create new queue entry

                        // 1. FIRST: Insert the bill record
                        string insertBillQuery = @"
                                INSERT INTO Bill (Bill_ID, Employee_ID, BillStatus, [date])
                                VALUES (@BillID, @EmployeeID, 'Paused', GETDATE())";

                        using (var cmd = new SqlCommand(insertBillQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@BillID", _billId);
                            cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. SECOND: Get next queue position
                        string getPositionQuery = @"
                                SELECT ISNULL(MAX(QueuePosition), 0) + 1 
                                FROM BillQueue 
                                WHERE Employee_ID = @EmployeeID AND IsActive = 1";

                        int queuePosition;
                        using (var cmd = new SqlCommand(getPositionQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);
                            queuePosition = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 3. THIRD: Insert into queue (now the Bill_ID exists)
                        string insertQueueQuery = @"
                                INSERT INTO BillQueue (Bill_ID, Employee_ID, QueuePosition, CartData, PausedAt, IsActive)
                                VALUES (@BillID, @EmployeeID, @QueuePosition, @CartData, GETDATE(), 1)";

                        using (var cmd = new SqlCommand(insertQueueQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@BillID", _billId);
                            cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);
                            cmd.Parameters.AddWithValue("@QueuePosition", queuePosition);
                            cmd.Parameters.AddWithValue("@CartData", serializedCart);
                            cmd.ExecuteNonQuery();
                        }

                        // Generate new Bill_ID for next bill
                        int newBillId = GenerateNewBillId(conn);

                        ThemedMessageBox.Show(
                            $"Bill #{_billId} paused and added to queue (Position: {queuePosition}). New Bill ID: {newBillId}",
                            "Bill Paused", ThemedMessageBoxIcon.Information);

                        _billId = newBillId;
                        lblBillId.Text = $"Bill ID: {_billId}";
                    }
                }

                // Clear current bill data but keep the new Bill_ID
                ClearCurrentBillData();
                LoadQueuedBills();
            }
            catch (SqlException sqlEx) when (sqlEx.Message.Contains("FOREIGN KEY constraint"))
            {
                // Handle foreign key constraint violation specifically
                HandleForeignKeyConstraintError(sqlEx);
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Pause Bill");
            }
        }

        private void ClearCurrentBillData()
        {
            _cartItems.Rows.Clear();
            _billDiscountPercentage = 0;
            _isBillDiscountApplied = false;
            _discountConflictWarningShown = false;
            _tokenApplied = false;
            _appliedToken = null;
            lblBillDiscount.Text = string.Empty;

            // Reset token button
            btnApplyToken.Text = "Apply Token";
            btnApplyToken.BackColor = Color.MediumPurple;
            btnApplyToken.Enabled = true;

            UpdateSummary();
            FocusBarcodeScanner();
        }



        private int GenerateNewBillId(SqlConnection conn)
        {
            string query = @"
                DECLARE @NextID INT;
                SELECT @NextID = ISNULL(MAX(Bill_ID), 0) + 1 FROM Bill;
        
                -- Ensure the ID doesn't already exist
                WHILE EXISTS (SELECT 1 FROM Bill WHERE Bill_ID = @NextID)
                BEGIN
                    SET @NextID = @NextID + 1;
                END
        
                SELECT @NextID AS NextBillID;";

            using (var cmd = new SqlCommand(query, conn))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void BtnViewQueuedBills_Click(object sender, EventArgs e)
        {

            try
            {
                using (var queueForm = new QueuedBillsForm(_currentUser.Employee_ID))
                {
                    if (queueForm.ShowDialog() == DialogResult.OK && queueForm.SelectedBill != null)
                    {
                        RestoreQueuedBill(queueForm.SelectedBill);
                    }
                }

                // Refresh the queue list after any operations
                LoadQueuedBills();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "View Queued Bills");
            }
        }
        //private void RestoreQueuedBill(QueuedBill queuedBill)
        //{
        //    try
        //    {
        //        if (_cartItems.Rows.Count > 0)
        //        {
        //            var result = ThemedMessageBoxYesNo.Show(
        //                "Current bill will be paused. Restore selected bill?",
        //                "Confirm Restore");

        //            if (result != DialogResult.Yes) return;

        //            // Pause current bill first
        //            BtnPauseBill_Click(this, EventArgs.Empty);
        //        }

        //        using (var conn = DbHelper.GetConnection())
        //        {
        //            conn.Open();
        //            using (var cmd = new SqlCommand("sp_RestorePausedBill", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@QueueID", queuedBill.Queue_ID);
        //                cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);

        //                using (var reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        int originalBillId = reader.GetInt32("BillID");
        //                        string cartDataJson = reader.GetString("CartData");

        //                        // Generate a NEW Bill_ID for the restored bill
        //                        GenerateBillId();

        //                        // Load the restored bill with the NEW Bill_ID
        //                        LoadBillFromCartData(_billId, cartDataJson);

        //                        ThemedMessageBox.Show("Bill restored successfully with new Bill ID", "Success",
        //                            ThemedMessageBoxIcon.Information);
        //                    }
        //                }
        //            }
        //        }

        //        LoadQueuedBills();
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleUnexpectedError(ex, "Restore Queued Bill");
        //    }
        //}

        private void RestoreQueuedBill(QueuedBill queuedBill)
        {
            try
            {
                if (_cartItems.Rows.Count > 0)
                {
                    var result = ThemedMessageBoxYesNo.Show(
                        "Current bill will be paused. Restore selected bill?",
                        "Confirm Restore");

                    if (result != DialogResult.Yes) return;

                    // Pause current bill first
                    BtnPauseBill_Click(this, EventArgs.Empty);
                }

                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("sp_RestorePausedBill", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@QueueID", queuedBill.Queue_ID);
                        cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int originalBillId = reader.GetInt32("BillID");
                                string cartDataJson = reader.GetString("CartData");

                                // Generate a NEW Bill_ID for the restored bill
                                GenerateBillId();

                                // Load the restored bill with the NEW Bill_ID
                                LoadBillFromCartData(_billId, cartDataJson);

                                // Store the original queue ID for re-pause scenario
                                // We'll use Tag property or a field to track this
                                this.Tag = queuedBill.Queue_ID; // Store original queue ID

                                ThemedMessageBox.Show("Bill restored successfully with new Bill ID", "Success",
                                    ThemedMessageBoxIcon.Information);
                            }
                        }
                    }
                }

                LoadQueuedBills();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Restore Queued Bill");
            }
        }

        private void LoadBillFromCartData(int billId, string cartDataJson)
        {
            try
            {
                // Clear current bill
                ClearBill();

                // Deserialize cart data
                var cartData = JsonConvert.DeserializeObject<CartData>(cartDataJson);

                // Set bill ID
                _billId = billId;
                lblBillId.Text = $"Bill ID: {_billId}";

                // Restore cart items
                foreach (var item in cartData.Items)
                {
                    _cartItems.Rows.Add(
                        item.ProductSize_ID,
                        item.Product_ID,
                        item.Barcode,
                        item.Brand,
                        item.Category,
                        item.Description,
                        item.Size,
                        item.Price,
                        item.Quantity,
                        item.DiscountAmountPerItem,
                        0, // DiscountAmount
                        0, // NetPrice
                        item.MaxDiscount,
                        item.AvailableStock
                    );
                }

                // Restore discounts and tokens
                _subtotal = cartData.Subtotal;
                _totalDiscount = cartData.TotalDiscount;
                _billDiscountPercentage = cartData.BillDiscountPercentage;
                _isBillDiscountApplied = cartData.IsBillDiscountApplied;
                _appliedToken = cartData.AppliedToken;
                _tokenApplied = cartData.TokenApplied;

                // Update UI
                UpdateSummary();

                if (_isBillDiscountApplied)
                {
                    lblBillDiscount.Text = $"-Rs.{(_subtotal * (_billDiscountPercentage / 100)):0.00} ({_billDiscountPercentage}%)";
                }

                if (_tokenApplied)
                {
                    btnApplyToken.Text = $"Token: Rs.{_appliedToken.TotalRefund:N2}";
                    btnApplyToken.ForeColor = Color.White;
                    btnApplyToken.BackColor = Color.MediumSeaGreen;
                    btnApplyToken.Enabled = false;
                }

                FocusBarcodeScanner();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Load Bill from Cart Data");
            }
        }
        #endregion

        #region Barcode Scanning Implementation
        private void TxtBarcodeInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent beep sound

                // Prevent double-processing
                if ((DateTime.Now - _lastScanTime).TotalMilliseconds < 100) return;
                _lastScanTime = DateTime.Now;

                string barcode = _txtBarcodeInput.Text.Trim();
                if (!string.IsNullOrEmpty(barcode))
                {
                    // Show in visible field for user feedback
                    txtBarcode.Text = barcode;
                    AddItemToCart();
                }

                // Reset and prepare for next scan
                _txtBarcodeInput.Clear();
                FocusBarcodeScanner();
            }
        }

        private void FocusBarcodeScanner()
        {
            if (!_isEditingGrid)
            {
                _txtBarcodeInput.Focus();
                _txtBarcodeInput.SelectAll();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            FocusBarcodeScanner();
        }

        private void BillingForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F11) // Add item
                {
                    if (!string.IsNullOrWhiteSpace(txtBarcode.Text))
                    {
                        btnAddItem.PerformClick();
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.F12) // Process payment
                {
                    btnProcessPayment.PerformClick();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.F1) // Clear bill
                {
                    btnClearBill.PerformClick();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.F2) // Pause bill
                {
                    btnPauseBill.PerformClick();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.F3) // View queued bills
                {
                    btnViewQueuedBills.PerformClick();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape) // Refocus scanner
                {
                    FocusBarcodeScanner();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Keyboard Shortcut");
            }
        }

        private void TxtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                // Handle Enter key
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    if (!string.IsNullOrWhiteSpace(txtBarcode.Text))
                    {
                        AddItemToCart();
                    }
                    FocusBarcodeScanner();
                }
                // Allow barcode characters
                else if (char.IsLetterOrDigit(e.KeyChar) ||
                         e.KeyChar == '-' ||
                         e.KeyChar == '.' ||
                         e.KeyChar == '_' ||
                         e.KeyChar == ' ' ||
                         e.KeyChar == '+' ||
                         e.KeyChar == '*')
                {
                    // Allow these characters
                }
                // Handle backspace and delete
                else if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete)
                {
                    // Allow through
                }
                // Handle Ctrl+V for paste
                else if (e.KeyChar == 22 && Control.ModifierKeys == Keys.Control)
                {
                    // Allow paste
                }
                else
                {
                    e.Handled = true;
                    ShowInvalidCharacterTooltip();
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Barcode Input");
            }
        }

        private void ShowInvalidCharacterTooltip()
        {
            var toolTip = new ToolTip
            {
                ToolTipTitle = "Invalid Character",
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Warning
            };

            toolTip.Show("Only letters, numbers, and -_.+* symbols are allowed",
                         txtBarcode,
                         txtBarcode.Width / 2,
                         -40,
                         2000);
        }
        #endregion

        #region Form Components and Data Initialization
        private void InitializeDataGridView()
        {
            dgvCart.ReadOnly = false;
            dgvCart.BackgroundColor = Color.White;
            dgvCart.BorderStyle = BorderStyle.None;
            dgvCart.AutoGenerateColumns = false;
            dgvCart.AllowUserToAddRows = false;
            dgvCart.RowHeadersVisible = false;
            dgvCart.AllowUserToResizeRows = false;
            dgvCart.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.GridColor = Color.FromArgb(240, 240, 240);

            dgvCart.CellBeginEdit += (s, e) => _isEditingGrid = true;
            dgvCart.CellEndEdit += (s, e) => {
                _isEditingGrid = false;
                FocusBarcodeScanner();
            };
            dgvCart.DataError += DgvCart_DataError;

            var colDelete = new DataGridViewButtonColumn
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

            var colQuantity = new DataGridViewTextBoxColumn
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

            var colDiscountAmountPerItem = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DiscountAmountPerItem",
                HeaderText = "Discount (Rs.)",
                Name = "DiscountAmountPerItem",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            };

            DataGridViewTextBoxColumn colSize = new DataGridViewTextBoxColumn
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
                colDiscountAmountPerItem,
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
                _cartItems.Columns.Add("DiscountAmountPerItem", typeof(decimal));
                _cartItems.Columns.Add("DiscountAmount", typeof(decimal), "DiscountAmountPerItem * Quantity");
                _cartItems.Columns.Add("NetPrice", typeof(decimal), "(Price - DiscountAmountPerItem) * Quantity");
                _cartItems.Columns.Add("MaxDiscount", typeof(decimal));
                _cartItems.Columns.Add("AvailableStock", typeof(int));

                dgvCart.DataSource = _cartItems;
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Cart Initialization");
            }
        }

        //private void GenerateBillId()
        //{
        //    try
        //    {
        //        using (var conn = DbHelper.GetConnection())
        //        {
        //            conn.Open();

        //            // Use a more robust method to get the next Bill_ID
        //            var cmd = new SqlCommand(@"
        //        SELECT ISNULL(MAX(Bill_ID), 0) + 1 
        //        FROM Bill 
        //        WHERE BillStatus IN ('Active', 'Completed', 'Paused')", conn);

        //            var result = cmd.ExecuteScalar();
        //            _billId = Convert.ToInt32(result);
        //            lblBillId.Text = $"Bill ID: {_billId}";
        //        }
        //    }
        //    catch (SqlException sqlEx)
        //    {
        //        HandleDatabaseError(sqlEx);
        //        try
        //        {
        //            // Fallback: use timestamp-based ID
        //            _billId = int.Parse(DateTime.Now.ToString("MMddHHmmss"));
        //            lblBillId.Text = $"Bill ID: {_billId}";
        //            MessageBox.Show("Using fallback bill ID", "Warning",
        //                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //        catch (Exception fallbackEx)
        //        {
        //            _billId = 1;
        //            lblBillId.Text = $"Bill ID: {_billId}";
        //            HandleUnexpectedError(fallbackEx, "Bill ID Fallback");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleUnexpectedError(ex, "Bill ID Generation");
        //    }
        //}

        //private void GenerateBillId()
        //{
        //    try
        //    {
        //        using (var conn = DbHelper.GetConnection())
        //        {
        //            conn.Open();

        //            // Get the next available Bill_ID that doesn't exist in the Bill table
        //            var cmd = new SqlCommand(@"
        //        DECLARE @NextID INT;
        //        SELECT @NextID = ISNULL(MAX(Bill_ID), 0) + 1 FROM Bill;

        //        -- Ensure the ID doesn't already exist
        //        WHILE EXISTS (SELECT 1 FROM Bill WHERE Bill_ID = @NextID)
        //        BEGIN
        //            SET @NextID = @NextID + 1;
        //        END

        //        SELECT @NextID AS NextBillID;", conn);

        //            var result = cmd.ExecuteScalar();
        //            _billId = Convert.ToInt32(result);
        //            lblBillId.Text = $"Bill ID: {_billId}";
        //        }
        //    }
        //    catch (SqlException sqlEx)
        //    {
        //        HandleDatabaseError(sqlEx);
        //        // Fallback: use timestamp-based ID
        //        try
        //        {
        //            _billId = int.Parse(DateTime.Now.ToString("MMddHHmmss"));
        //            lblBillId.Text = $"Bill ID: {_billId}";
        //        }
        //        catch (Exception fallbackEx)
        //        {
        //            _billId = new Random().Next(1000, 9999);
        //            lblBillId.Text = $"Bill ID: {_billId}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleUnexpectedError(ex, "Bill ID Generation");
        //    }
        //}


        private void GenerateBillId()
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    _billId = GenerateNewBillId(conn);
                    lblBillId.Text = $"Bill ID: {_billId}";
                }
            }
            catch (SqlException sqlEx)
            {
                HandleDatabaseError(sqlEx);
                // Fallback: use timestamp-based ID
                try
                {
                    _billId = int.Parse(DateTime.Now.ToString("MMddHHmmss"));
                    lblBillId.Text = $"Bill ID: {_billId}";
                }
                catch (Exception fallbackEx)
                {
                    _billId = new Random().Next(1000, 9999);
                    lblBillId.Text = $"Bill ID: {_billId}";
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
                btnClearBill.Click += (s, e) => ClearBillButton();
                btnAddItem.Click += BtnAddItem_Click;
                txtBarcode.KeyPress += TxtBarcode_KeyPress;
                btnProcessPayment.Click += BtnProcessPayment_Click;
                btnApplyBillDiscount.Click += BtnApplyBillDiscount_Click;
                btnClearDiscounts.Click += BtnClearDiscounts_Click;
                btnApplyToken.Click += BtnApplyToken_Click;

                // Queue management events
                btnPauseBill.Click += BtnPauseBill_Click;
                btnViewQueuedBills.Click += BtnViewQueuedBills_Click;
                menuPauseBill.Click += (s, e) => BtnPauseBill_Click(s, e);
                menuViewQueuedBills.Click += (s, e) => BtnViewQueuedBills_Click(s, e);
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

                var discountColumn = dgvCart.Columns["DiscountAmountPerItem"];
                if (discountColumn != null)
                {
                    dgvCart.CellValidating += (s, e) =>
                    {
                        try
                        {
                            if (e.ColumnIndex == discountColumn.Index)
                            {
                                DataRow dataRow = _cartItems.Rows[e.RowIndex];
                                decimal maxDiscount = Convert.ToDecimal(dataRow["MaxDiscount"]);
                                decimal price = Convert.ToDecimal(dataRow["Price"]);

                                if (decimal.TryParse(e.FormattedValue.ToString(), out decimal newValue))
                                {
                                    if (newValue > maxDiscount)
                                    {
                                        e.Cancel = true;
                                        dgvCart.Rows[e.RowIndex].ErrorText =
                                            $"Discount cannot exceed Rs.{maxDiscount:N2}";
                                    }
                                    else if (newValue > price)
                                    {
                                        e.Cancel = true;
                                        dgvCart.Rows[e.RowIndex].ErrorText =
                                            "Discount cannot exceed item price";
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
                            if (e.ColumnIndex == discountColumn.Index)
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

                var deleteColumn = dgvCart.Columns.Cast<DataGridViewColumn>()
                    .FirstOrDefault(col => col is DataGridViewButtonColumn && col.HeaderText == "Delete");
                var quantityColumn = dgvCart.Columns.Cast<DataGridViewColumn>()
                    .FirstOrDefault(col => col.HeaderText == "Qty");
                var discountColumn = dgvCart.Columns.Cast<DataGridViewColumn>()
                    .FirstOrDefault(col => col.HeaderText == "Discount (Rs.)");

                if (deleteColumn != null && e.ColumnIndex == deleteColumn.Index)
                {
                    _cartItems.Rows.RemoveAt(e.RowIndex);
                    FocusBarcodeScanner();
                }
                else if (quantityColumn != null && e.ColumnIndex == quantityColumn.Index)
                {
                    ShowQuantityEditor(e.RowIndex);
                }
                else if (discountColumn != null && e.ColumnIndex == discountColumn.Index)
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
                if (!string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    AddItemToCart();
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Add Item");
            }
        }

        private void DgvCart_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            ShowError($"Data error: {e.Exception.Message}");
            e.Cancel = true;
        }

        private void BtnApplyBillDiscount_Click(object sender, EventArgs e)
        {
            try
            {
                bool hasPerItemDiscount = _cartItems.AsEnumerable()
                    .Any(row => Convert.ToDecimal(row["DiscountAmountPerItem"]) > 0);

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
                        row["DiscountAmountPerItem"] = 0;
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
                    row["DiscountAmountPerItem"] = 0;
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
                using (var tokenForm = new TokenForm())
                {
                    if (tokenForm.ShowDialog() == DialogResult.OK && tokenForm.SelectedToken != null)
                    {
                        _appliedToken = new ReturnToken
                        {
                            ReturnId = tokenForm.SelectedToken.ReturnId,
                            TotalRefund = tokenForm.SelectedToken.TotalRefund
                        };

                        _tokenApplied = true;

                        // Update button appearance
                        btnApplyToken.Text = $"Token: Rs.{_appliedToken.TotalRefund:N2}";
                        btnApplyToken.ForeColor = Color.White;
                        btnApplyToken.BackColor = Color.MediumSeaGreen; // Change color to indicate applied
                        btnApplyToken.Enabled = false; // Disable button after applying

                        MessageBox.Show($"Token applied successfully! Value: Rs.{_appliedToken.TotalRefund:N2}",
                            "Token Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (txtBarcode.Focused && string.IsNullOrWhiteSpace(txtBarcode.Text))
                    return;

                if (_cartItems.Rows.Count == 0)
                {
                    ThemedMessageBox.Show("Cart is empty", "Warning", ThemedMessageBoxIcon.Warning);
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
                            customerContact: paymentForm.CustomerContact,
                            customerGender: paymentForm.CustomerGender,
                            // Mixed payment parameters
                            firstPaymentMethod: paymentForm.FirstPaymentMethod,
                            firstPaymentAmount: paymentForm.FirstPaymentAmount,
                            firstCardLast4: paymentForm.FirstCardLastFour,
                            firstBankLast4: paymentForm.FirstBankLastFour,
                            secondPaymentMethod: paymentForm.SecondPaymentMethod,
                            secondPaymentAmount: paymentForm.SecondPaymentAmount,
                            secondCardLast4: paymentForm.SecondCardLastFour,
                            secondBankLast4: paymentForm.SecondBankLastFour
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Payment Processing");
            }
        }

        private void HandleForeignKeyConstraintError(SqlException sqlEx)
        {
            try
            {
                string errorMessage = "Database constraint error: The bill reference is invalid.\n\n";
                errorMessage += "This usually happens when a bill reference doesn't exist.\n";
                errorMessage += "The system will attempt to recover by creating a new bill.";

                ThemedMessageBox.Show(errorMessage, "Constraint Error",
                    ThemedMessageBoxIcon.Warning);

                // Attempt recovery by creating a fresh pause
                RecoverFromPauseError();
            }
            catch (Exception recoveryEx)
            {
                HandleUnexpectedError(recoveryEx, "Foreign Key Constraint Recovery");
            }
        }

        private void RecoverFromPauseError()
        {
            try
            {
                // Clear any re-pause tracking
                this.Tag = null;

                // Generate a new bill ID
                GenerateBillId();

                // Show message to user
                ThemedMessageBox.Show($"Recovery successful. New Bill ID: {_billId}",
                    "Recovery Complete", ThemedMessageBoxIcon.Information);

                // Reload queued bills to refresh state
                LoadQueuedBills();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Pause Error Recovery");
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
                            ps.ProductSize_ID,
                            ps.Size_ID,
                            ps.unitCost,
                            s.SizeLabel, 
                            ps.quantity AS AvailableStock, 
                            ps.RetailPrice
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

                txtBarcode.Clear();
                FocusBarcodeScanner();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Add Item");
                FocusBarcodeScanner();
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
                string barcode = itemRow.Field<string>("barcode");
                string brand = itemRow.Field<string>("brandName");
                string category = itemRow.Field<string>("categoryName");
                string description = itemRow.Field<string>("description");
                decimal price = itemRow.Field<decimal>("RetailPrice");
                decimal maxDiscount = itemRow.Field<decimal>("maxDiscount");
                decimal unitCost = itemRow.Field<decimal>("unitCost");

                // Check if item already in cart
                var existingRow = _cartItems.AsEnumerable()
                    .FirstOrDefault(row => row.Field<int>("ProductSize_ID") == productSizeId);

                if (existingRow != null)
                {
                    // Item exists - just increment quantity (don't show dialog)
                    int currentQty = Convert.ToInt32(existingRow["Quantity"]);
                    int newQty = currentQty + quantity;

                    // Ensure we don't exceed available stock
                    if (newQty > availableStock)
                    {
                        newQty = availableStock;
                        ShowError($"Cannot exceed available stock of {availableStock}");
                    }

                    existingRow["Quantity"] = newQty;
                }
                else
                {
                    // New item - show edit dialog
                    int currentQty = Math.Min(quantity, availableStock);
                    decimal currentDiscount = 0;

                    if (quantity > availableStock)
                    {
                        ShowError($"Cannot exceed available stock of {availableStock}");
                    }

                    using (var editForm = new ItemSet(
                        description: description,
                        barcode: barcode,
                        brand: brand,
                        unitCost: unitCost,
                        category: category,
                        size: size,
                        availableStock: availableStock,
                        unitPrice: price,
                        maxDiscount: maxDiscount,
                        currentQuantity: currentQty,
                        currentDiscount: currentDiscount))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            _cartItems.Rows.Add(
                                productSizeId,
                                productId,
                                barcode,
                                brand,
                                category,
                                description,
                                size,
                                price,
                                editForm.SelectedQuantity,
                                editForm.SelectedDiscountPerItem,
                                0, // DiscountAmount
                                0, // NetPrice
                                maxDiscount,
                                availableStock
                            );
                        }
                    }
                }
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

                FocusBarcodeScanner();
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
                decimal currentDiscount = Convert.ToDecimal(row["DiscountAmountPerItem"]);
                decimal price = Convert.ToDecimal(row["Price"]);

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

                using (var discountForm = new DiscountAmountForm(maxDiscount, price, currentDiscount))
                {
                    if (discountForm.ShowDialog() == DialogResult.OK)
                    {
                        row["DiscountAmountPerItem"] = discountForm.SelectedDiscountAmount;

                        if (discountForm.SelectedDiscountAmount > 0 && _isBillDiscountApplied)
                        {
                            _billDiscountPercentage = 0;
                            _isBillDiscountApplied = false;
                            lblBillDiscount.Text = string.Empty;
                            UpdateSummary();
                        }
                    }
                }

                FocusBarcodeScanner();
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
                FocusBarcodeScanner();
            }
            catch (Exception ex)
            {
                HandleUnexpectedError(ex, "Refresh Bill");
            }
        }

        //private void ClearBillButton()
        //{
        //    DialogResult result = ThemedMessageBoxYesNo.Show("Are you sure you want to clear the bill?", "Warning");

        //    if (result == DialogResult.Yes)
        //    {
        //        try
        //        {
        //            _cartItems.Rows.Clear();
        //            _billDiscountPercentage = 0;
        //            _isBillDiscountApplied = false;
        //            _discountConflictWarningShown = false;
        //            _tokenApplied = false;
        //            _appliedToken = null;
        //            lblBillDiscount.Text = string.Empty;
        //            txtTokenId.Enabled = true;
        //            btnApplyToken.Enabled = true;
        //            txtTokenId.Clear();
        //            GenerateBillId();
        //            FocusBarcodeScanner();
        //            UpdateSummary();
        //        }
        //        catch (Exception ex)
        //        {
        //            HandleUnexpectedError(ex, "Clear Bill");
        //        }
        //    }
        //}

        private void ClearBillButton()
        {
            DialogResult result = ThemedMessageBoxYesNo.Show("Are you sure you want to clear the bill?", "Warning");

            if (result == DialogResult.Yes)
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

                    // Reset token button
                    btnApplyToken.Text = "Apply Token";
                    btnApplyToken.BackColor = Color.MediumPurple;
                    btnApplyToken.Enabled = true;

                    GenerateBillId();
                    FocusBarcodeScanner();
                    UpdateSummary();
                }
                catch (Exception ex)
                {
                    HandleUnexpectedError(ex, "Clear Bill");
                }
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

                // Reset token button
                btnApplyToken.Text = "Apply Token";
                btnApplyToken.BackColor = Color.MediumPurple;
                btnApplyToken.Enabled = true;

                // Reset payment fields
                _cardLastFour = null;
                _bankLastFour = null;
                _firstCardLastFour = null;
                _firstBankLastFour = null;
                _secondCardLastFour = null;
                _secondBankLastFour = null;
                _customerContact = null;
                _customerGender = null;

                lblBillDiscount.Text = string.Empty;
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
                    lblBillDiscount.Text = $"-Rs.{billDiscountAmount:0.00} ({_billDiscountPercentage}%)";
                }
                else
                {
                    lblBillDiscount.Text = string.Empty;
                }

                _total = _subtotal - _totalDiscount - billDiscountAmount;
                lblTotal.Text = $"Rs.{_total:0.00}";
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
        //private void ProcessConfirmedPayment(
        //    string paymentMethod,
        //    decimal amountTendered,
        //    string cardLast4,
        //    string bankLast4,
        //    decimal change,
        //    string customerContact,
        //    string customerGender,
        //    // Mixed payment parameters
        //    string firstPaymentMethod = null,
        //    decimal firstPaymentAmount = 0,
        //    string firstCardLast4 = null,
        //    string firstBankLast4 = null,
        //    string secondPaymentMethod = null,
        //    decimal secondPaymentAmount = 0,
        //    string secondCardLast4 = null,
        //    string secondBankLast4 = null)
        //{
        //    try
        //    {
        //        // Store payment details in fields for use in other methods
        //        _cardLastFour = cardLast4;
        //        _bankLastFour = bankLast4;
        //        _customerContact = customerContact;
        //        _customerGender = customerGender;
        //        _firstCardLastFour = firstCardLast4;
        //        _firstBankLastFour = firstBankLast4;
        //        _secondCardLastFour = secondCardLast4;
        //        _secondBankLastFour = secondBankLast4;

        //        // Prepare items for stored procedure
        //        var items = new List<BillItem>();
        //        foreach (DataRow row in _cartItems.Rows)
        //        {
        //            items.Add(new BillItem
        //            {
        //                ProductSize_ID = row["ProductSize_ID"] != DBNull.Value ?
        //                    Convert.ToInt32(row["ProductSize_ID"]) : 0,
        //                Quantity = row["Quantity"] != DBNull.Value ?
        //                    Convert.ToInt32(row["Quantity"]) : 0,
        //                ItemSellingPrice = row["Price"] != DBNull.Value ?
        //                    Convert.ToDecimal(row["Price"]) : 0,
        //                Per_item_Discount = row["DiscountAmountPerItem"] != DBNull.Value ?
        //                    Convert.ToDecimal(row["DiscountAmountPerItem"]) : 0
        //            });
        //        }

        //        // Calculate total discount
        //        decimal totalDiscount = CalculateTotalDiscount();

        //        // Determine actual payment method for database
        //        string dbPaymentMethod = paymentMethod;
        //        object sqlCardLast4 = DBNull.Value;
        //        object sqlBankLast4 = DBNull.Value;
        //        object sqlToken = DBNull.Value;

        //        // Handle payments parameter for mixed payment
        //        DataTable paymentsDt = null;
        //        if (paymentMethod == "Mixed")
        //        {
        //            // Create payments data table for mixed payment
        //            paymentsDt = CreatePaymentsDataTable(
        //                _billId,
        //                firstPaymentMethod, firstPaymentAmount, firstCardLast4, firstBankLast4,
        //                secondPaymentMethod, secondPaymentAmount, secondCardLast4, secondBankLast4
        //            );

        //            // For mixed payments, we don't use the single payment parameters
        //            sqlCardLast4 = DBNull.Value;
        //            sqlBankLast4 = DBNull.Value;
        //        }
        //        else
        //        {
        //            // Set single payment details
        //            if (paymentMethod == "Card")
        //            {
        //                sqlCardLast4 = cardLast4;
        //            }
        //            else if (paymentMethod == "Bank Transfer")
        //            {
        //                sqlBankLast4 = bankLast4;
        //            }
        //        }

        //        // Handle token payments
        //        if (_tokenApplied)
        //        {
        //            sqlToken = _appliedToken.ReturnId;

        //            // If token covers entire amount
        //            if (amountTendered == 0 && change == 0 && paymentMethod == "Token")
        //            {
        //                dbPaymentMethod = null;
        //            }
        //        }

        //        using (var conn = DbHelper.GetConnection())
        //        {
        //            conn.Open();
        //            using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
        //            using (var cmd = new SqlCommand("sp_ProcessSale", conn, transaction))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@BillID", _billId);
        //                cmd.Parameters.AddWithValue("@EmployeeID", _currentUser.Employee_ID);
        //                cmd.Parameters.AddWithValue("@PaymentMethod", dbPaymentMethod ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@Discount", totalDiscount);
        //                cmd.Parameters.AddWithValue("@CardLast4", sqlCardLast4);
        //                cmd.Parameters.AddWithValue("@BankAccountLast4", sqlBankLast4);
        //                cmd.Parameters.AddWithValue("@Token_ReturnID", sqlToken);
        //                cmd.Parameters.AddWithValue("@CustomerContact", customerContact ?? (object)DBNull.Value);
        //                cmd.Parameters.AddWithValue("@ContactGender", customerGender ?? (object)DBNull.Value);

        //                // Add the payments parameter for mixed payments
        //                var paymentsParam = cmd.Parameters.AddWithValue("@Payments", paymentsDt ?? (object)DBNull.Value);
        //                paymentsParam.SqlDbType = SqlDbType.Structured;
        //                paymentsParam.TypeName = "BillPaymentType";

        //                // Add items parameter
        //                var dt = CreateItemsDataTable(items);
        //                var param = cmd.Parameters.AddWithValue("@Items", dt);
        //                param.SqlDbType = SqlDbType.Structured;
        //                param.TypeName = "BillItemType";

        //                // Execute with transaction
        //                cmd.ExecuteNonQuery();

        //                // Commit transaction
        //                transaction.Commit();
        //            }
        //        }

        //        // Print the bill
        //        PrintBill(
        //            billId: _billId,
        //            paymentMethod: paymentMethod,
        //            amountTendered: amountTendered,
        //            change: change,
        //            cardLast4: cardLast4,
        //            bankLast4: bankLast4,
        //            // Mixed payment parameters
        //            firstPaymentMethod: firstPaymentMethod,
        //            firstPaymentAmount: firstPaymentAmount,
        //            firstCardLast4: firstCardLast4,
        //            firstBankLast4: firstBankLast4,
        //            secondPaymentMethod: secondPaymentMethod,
        //            secondPaymentAmount: secondPaymentAmount,
        //            secondCardLast4: secondCardLast4,
        //            secondBankLast4: secondBankLast4
        //        );

        //        // Show success message
        //        ShowPaymentSuccess(
        //            paymentMethod: paymentMethod,
        //            amountTendered: amountTendered,
        //            change: change,
        //            totalItems: _totalItems,
        //            totalAmount: _total,
        //            // Mixed payment parameters
        //            firstPaymentMethod: firstPaymentMethod,
        //            firstPaymentAmount: firstPaymentAmount,
        //            secondPaymentMethod: secondPaymentMethod,
        //            secondPaymentAmount: secondPaymentAmount
        //        );

        //        // Reset for next bill
        //        ClearBill();
        //        FocusBarcodeScanner();
        //    }
        //    catch (SqlException ex) when (ex.Number == 50004)
        //    {
        //        ShowError("Invalid cash payment: " + ex.Message.Replace("Cash payment should not have card/bank details.", string.Empty));
        //    }
        //    catch (SqlException ex)
        //    {
        //        HandleDatabaseError(ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleUnexpectedError(ex, "Payment Confirmation");
        //    }
        //}

        private void ProcessConfirmedPayment(
    string paymentMethod,
    decimal amountTendered,
    string cardLast4,
    string bankLast4,
    decimal change,
    string customerContact,
    string customerGender,
    // Mixed payment parameters
    string firstPaymentMethod = null,
    decimal firstPaymentAmount = 0,
    string firstCardLast4 = null,
    string firstBankLast4 = null,
    string secondPaymentMethod = null,
    decimal secondPaymentAmount = 0,
    string secondCardLast4 = null,
    string secondBankLast4 = null)
        {
            try
            {
                // Store payment details in fields for use in other methods
                _cardLastFour = cardLast4;
                _bankLastFour = bankLast4;
                _customerContact = customerContact;
                _customerGender = customerGender;
                _firstCardLastFour = firstCardLast4;
                _firstBankLastFour = firstBankLast4;
                _secondCardLastFour = secondCardLast4;
                _secondBankLastFour = secondBankLast4;

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
                        ItemSellingPrice = row["Price"] != DBNull.Value ?
                            Convert.ToDecimal(row["Price"]) : 0,
                        Per_item_Discount = row["DiscountAmountPerItem"] != DBNull.Value ?
                            Convert.ToDecimal(row["DiscountAmountPerItem"]) : 0
                    });
                }

                // Calculate total discount
                decimal totalDiscount = CalculateTotalDiscount();

                // Determine actual payment method for database
                string dbPaymentMethod = paymentMethod;
                object sqlCardLast4 = DBNull.Value;
                object sqlBankLast4 = DBNull.Value;
                object sqlToken = DBNull.Value;

                // Handle payments parameter for ALL payment methods
                DataTable paymentsDt = CreateEmptyPaymentsDataTable(); // Always create a DataTable, never null

                if (paymentMethod == "Mixed")
                {
                    // Create payments data table for mixed payment
                    paymentsDt = CreatePaymentsDataTable(
                        _billId,
                        firstPaymentMethod, firstPaymentAmount, firstCardLast4, firstBankLast4,
                        secondPaymentMethod, secondPaymentAmount, secondCardLast4, secondBankLast4
                    );

                    // For mixed payments, we don't use the single payment parameters
                    sqlCardLast4 = DBNull.Value;
                    sqlBankLast4 = DBNull.Value;
                }
                else
                {
                    // For single payment methods, create a DataTable with the single payment
                    paymentsDt = CreateSinglePaymentDataTable(_billId, paymentMethod, amountTendered, cardLast4, bankLast4);

                    // Set single payment details
                    if (paymentMethod == "Card")
                    {
                        sqlCardLast4 = cardLast4;
                    }
                    else if (paymentMethod == "Bank Transfer")
                    {
                        sqlBankLast4 = bankLast4;
                    }
                }

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
                        cmd.Parameters.AddWithValue("@ContactGender", customerGender ?? (object)DBNull.Value);

                        // Add the payments parameter for ALL payment methods (never DBNull)
                        var paymentsParam = cmd.Parameters.AddWithValue("@Payments", paymentsDt);
                        paymentsParam.SqlDbType = SqlDbType.Structured;
                        paymentsParam.TypeName = "BillPaymentType";

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
                    bankLast4: bankLast4,
                    // Mixed payment parameters
                    firstPaymentMethod: firstPaymentMethod,
                    firstPaymentAmount: firstPaymentAmount,
                    firstCardLast4: firstCardLast4,
                    firstBankLast4: firstBankLast4,
                    secondPaymentMethod: secondPaymentMethod,
                    secondPaymentAmount: secondPaymentAmount,
                    secondCardLast4: secondCardLast4,
                    secondBankLast4: secondBankLast4
                );

                // Show success message
                ShowPaymentSuccess(
                    paymentMethod: paymentMethod,
                    amountTendered: amountTendered,
                    change: change,
                    totalItems: _totalItems,
                    totalAmount: _total,
                    // Mixed payment parameters
                    firstPaymentMethod: firstPaymentMethod,
                    firstPaymentAmount: firstPaymentAmount,
                    secondPaymentMethod: secondPaymentMethod,
                    secondPaymentAmount: secondPaymentAmount
                );

                // Reset for next bill
                ClearBill();
                FocusBarcodeScanner();
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

        private DataTable CreateEmptyPaymentsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Bill_ID", typeof(int));
            dt.Columns.Add("PaymentMethod", typeof(string));
            dt.Columns.Add("PaymentAmount", typeof(decimal));
            dt.Columns.Add("CardLast4", typeof(string));
            dt.Columns.Add("BankAccountLast4", typeof(string));
            dt.Columns.Add("PaymentOrder", typeof(int));
            return dt;
        }

        private DataTable CreateSinglePaymentDataTable(int billId, string paymentMethod, decimal amount, string cardLast4, string bankLast4)
        {
            DataTable dt = CreateEmptyPaymentsDataTable();

            if (amount > 0) // Only add payment if amount is positive
            {
                dt.Rows.Add(
                    billId,
                    paymentMethod,
                    amount,
                    paymentMethod == "Card" ? cardLast4 ?? (object)DBNull.Value : DBNull.Value,
                    paymentMethod == "Bank Transfer" ? bankLast4 ?? (object)DBNull.Value : DBNull.Value,
                    1
                );
            }

            return dt;
        }

        private DataTable CreatePaymentsDataTable(
            int billId,
            string firstPaymentMethod, decimal firstPaymentAmount, string firstCardLast4, string firstBankLast4,
            string secondPaymentMethod, decimal secondPaymentAmount, string secondCardLast4, string secondBankLast4)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Bill_ID", typeof(int));
            dt.Columns.Add("PaymentMethod", typeof(string));
            dt.Columns.Add("PaymentAmount", typeof(decimal));
            dt.Columns.Add("CardLast4", typeof(string));
            dt.Columns.Add("BankAccountLast4", typeof(string));
            dt.Columns.Add("PaymentOrder", typeof(int));

            // Add first payment
            dt.Rows.Add(
                billId,
                firstPaymentMethod,
                firstPaymentAmount,
                firstCardLast4 ?? (object)DBNull.Value,
                firstBankLast4 ?? (object)DBNull.Value,
                1
            );

            // Add second payment
            dt.Rows.Add(
                billId,
                secondPaymentMethod,
                secondPaymentAmount,
                secondCardLast4 ?? (object)DBNull.Value,
                secondBankLast4 ?? (object)DBNull.Value,
                2
            );

            return dt;
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
                    item.ItemSellingPrice,
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
                decimal discountPerItem = Convert.ToDecimal(row["DiscountAmountPerItem"]);
                int qty = Convert.ToInt32(row["Quantity"]);
                totalDiscount += discountPerItem * qty;
            }

            if (_isBillDiscountApplied && _billDiscountPercentage > 0)
            {
                totalDiscount += _subtotal * (_billDiscountPercentage / 100);
            }

            return totalDiscount;
        }

        private void PrintBill(int billId, string paymentMethod,
            decimal amountTendered, decimal change,
            string cardLast4, string bankLast4,
            string firstPaymentMethod = null, decimal firstPaymentAmount = 0, string firstCardLast4 = null, string firstBankLast4 = null,
            string secondPaymentMethod = null, decimal secondPaymentAmount = 0, string secondCardLast4 = null, string secondBankLast4 = null)
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
                PrintCentered("No.102, Negombo Rd, Narammala.", output);
                PrintCentered("Tel: 0777491913 / 0374545097", output);
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
                    string description = row["Description"].ToString();
                    string brand = row["Brand"].ToString();
                    string category = row["Category"].ToString();
                    string size = row["Size"].ToString();
                    decimal price = Convert.ToDecimal(row["Price"]);
                    int qty = Convert.ToInt32(row["Quantity"]);
                    decimal discountPerItem = Convert.ToDecimal(row["DiscountAmountPerItem"]);
                    decimal netPrice = Convert.ToDecimal(row["NetPrice"]);
                    decimal lineTotal = price * qty;

                    // Item description (brand + category) - LEFT ALIGNED
                    PrintLeft($"{category}", output);

                    // Size and pricing details
                    string sizeInfo = !string.IsNullOrEmpty(size) ? $"{size} " : string.Empty;
                    string priceDetails = $"{qty} x {price:0.00} = {lineTotal:0.00}";
                    PrintLeftRight(sizeInfo, priceDetails, output);

                    // Discount information if applicable
                    if (discountPerItem > 0)
                    {
                        decimal discountAmount = discountPerItem * qty;
                        string discountLine = $"Discount: -{discountPerItem:0.00} per item";
                        PrintLeftRight(string.Empty, discountLine, output);
                        PrintLeftRight(string.Empty, $"Net: {netPrice:0.00}", output);
                    }

                    output.AddRange(Encoding.ASCII.GetBytes("\n"));
                }

                PrintSeparator(output);

                // Discount summary
                if (_totalDiscount > 0)
                {
                    PrintLeftRight("SUB TOTAL:", $"Rs.{_subtotal:0.00}", output);
                    PrintLeftRight("ITEM DISCOUNTS:", $"-Rs.{_totalDiscount:0.00}", output);
                }

                if (_isBillDiscountApplied && _billDiscountPercentage > 0)
                {
                    decimal billDiscount = Math.Round(_subtotal * (_billDiscountPercentage / 100), 2);
                    PrintLeftRight("SUB TOTAL:", $"Rs.{Math.Round(_subtotal, 2)}", output);
                    PrintLeftRight($"BILL DISCOUNT ({_billDiscountPercentage}%):", $"-Rs.{billDiscount:0.00}", output);
                }

                PrintLeftRight("TOTAL:", $"Rs.{Math.Round(_total, 2)}", output);
                PrintSeparator(output);

                // Token information
                if (_tokenApplied)
                {
                    PrintLeftRight("RETURN VALUE:", $"Rs.{Math.Round(_appliedToken.TotalRefund, 2)}", output);
                }

                // Payment details - Handle Mixed Payment
                if (paymentMethod == "Mixed")
                {
                    PrintLeftRight("MIXED PAYMENT", "SPLIT", output);
                    PrintSeparator(output);

                    // First Payment
                    PrintLeftRight($"{firstPaymentMethod.ToUpper()}:", $"Rs.{firstPaymentAmount:0.00}", output);
                    if (firstPaymentMethod == "Card" && !string.IsNullOrEmpty(firstCardLast4))
                        PrintLeftRight("CARD LAST 4:", firstCardLast4, output);
                    else if (firstPaymentMethod == "Bank Transfer" && !string.IsNullOrEmpty(firstBankLast4))
                        PrintLeftRight("BANK LAST 4:", firstBankLast4, output);

                    // Second Payment
                    PrintLeftRight($"{secondPaymentMethod.ToUpper()}:", $"Rs.{secondPaymentAmount:0.00}", output);
                    if (secondPaymentMethod == "Card" && !string.IsNullOrEmpty(secondCardLast4))
                        PrintLeftRight("CARD LAST 4:", secondCardLast4, output);
                    else if (secondPaymentMethod == "Bank Transfer" && !string.IsNullOrEmpty(secondBankLast4))
                        PrintLeftRight("BANK LAST 4:", secondBankLast4, output);

                    PrintSeparator(output);
                    PrintLeftRight("TOTAL PAID:", $"Rs.{(firstPaymentAmount + secondPaymentAmount):0.00}", output);

                    decimal totalPaid = firstPaymentAmount + secondPaymentAmount;
                    if (totalPaid > _total)
                    {
                        PrintLeftRight("CHANGE:", $"Rs.{(totalPaid - _total):0.00}", output);
                    }
                }
                else
                {
                    // Single Payment Method
                    if (paymentMethod == "Cash")
                    {
                        PrintLeftRight("CASH TENDERED:", $"Rs.{amountTendered:0.00}", output);
                        PrintLeftRight("CHANGE:", $"Rs.{change:0.00}", output);
                    }
                    else if (paymentMethod == "Card")
                    {
                        PrintLeftRight("CARD PAYMENT:", $"Rs.{amountTendered:0.00}", output);
                        PrintLeftRight("LAST 4 DIGITS:", cardLast4, output);
                    }
                    else if (paymentMethod == "Bank Transfer")
                    {
                        PrintLeftRight("BANK TRANSFER:", $"Rs.{amountTendered:0.00}", output);
                        PrintLeftRight("LAST 4 DIGITS:", bankLast4, output);
                    }
                    else if (paymentMethod == "Token")
                    {
                        PrintLeftRight("FULLY PAID WITH TOKEN", $"Rs.{_appliedToken.TotalRefund:0.00}", output);
                    }
                }

                output.AddRange(Encoding.ASCII.GetBytes("\n"));

                // Customer contact if available
                if (!string.IsNullOrEmpty(_customerContact))
                {
                    PrintLeftRight("CONTACT:", _customerContact, output);
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


        // Helper methods for printing
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

        private void ShowPaymentSuccess(
            string paymentMethod,
            decimal amountTendered,
            decimal change,
            int totalItems,
            decimal totalAmount,
            string firstPaymentMethod = null,
            decimal firstPaymentAmount = 0,
            string secondPaymentMethod = null,
            decimal secondPaymentAmount = 0)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Payment processed successfully!\n");
            message.AppendLine($"Bill ID: {_billId}");
            message.AppendLine($"Total Items: {totalItems}");
            message.AppendLine($"Total Amount: Rs.{totalAmount:0.00}");

            // Add payment method specific details
            if (paymentMethod == "Mixed")
            {
                message.AppendLine($"\nPayment Method: Mixed Payment");
                message.AppendLine($"First Payment ({firstPaymentMethod}): Rs.{firstPaymentAmount:0.00}");

                if (firstPaymentMethod == "Card")
                    message.AppendLine($"Card Last 4: {_firstCardLastFour}");
                else if (firstPaymentMethod == "Bank Transfer")
                    message.AppendLine($"Bank Last 4: {_firstBankLastFour}");

                message.AppendLine($"Second Payment ({secondPaymentMethod}): Rs.{secondPaymentAmount:0.00}");

                if (secondPaymentMethod == "Card")
                    message.AppendLine($"Card Last 4: {_secondCardLastFour}");
                else if (secondPaymentMethod == "Bank Transfer")
                    message.AppendLine($"Bank Last 4: {_secondBankLastFour}");

                decimal totalPaid = firstPaymentAmount + secondPaymentAmount;
                message.AppendLine($"Total Paid: Rs.{totalPaid:0.00}");

                if (totalPaid > totalAmount)
                {
                    message.AppendLine($"Change Given: Rs.{(totalPaid - totalAmount):0.00}");
                }
            }
            else
            {
                switch (paymentMethod)
                {
                    case "Cash":
                        message.AppendLine($"\nPayment Method: Cash");
                        message.AppendLine($"Amount Tendered: Rs.{amountTendered:0.00}");
                        message.AppendLine($"Change: Rs.{change:0.00}");
                        break;

                    case "Card":
                        message.AppendLine($"\nPayment Method: Card");
                        message.AppendLine($"Amount Paid: Rs.{totalAmount:0.00}");
                        message.AppendLine($"Card Last 4: {_cardLastFour}");
                        break;

                    case "Bank Transfer":
                        message.AppendLine($"\nPayment Method: Bank Transfer");
                        message.AppendLine($"Amount Paid: Rs.{totalAmount:0.00}");
                        message.AppendLine($"Bank Last 4: {_bankLastFour}");
                        break;

                    case "Token":
                        message.AppendLine($"\nPayment Method: Token");
                        message.AppendLine($"Token Value Applied: Rs.{_appliedToken.TotalRefund:0.00}");
                        message.AppendLine($"Remaining Token Balance: Rs.0.00");
                        break;
                }
            }

            // Token information if applied
            if (_tokenApplied && paymentMethod != "Token")
            {
                message.AppendLine($"\nToken Applied: Rs.{_appliedToken.TotalRefund:0.00}");
            }

            // Customer contact information if available
            if (!string.IsNullOrEmpty(_customerContact))
            {
                message.AppendLine($"\nCustomer Contact: {_customerContact}");
                if (!string.IsNullOrEmpty(_customerGender))
                {
                    message.AppendLine($"Customer Gender: {_customerGender}");
                }
            }

            ThemedMessageBoxGreen.Show(message.ToString(), "Payment Success");

            // Optional: Play success sound or other feedback
            System.Media.SystemSounds.Exclamation.Play();
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