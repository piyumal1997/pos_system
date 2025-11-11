using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Newtonsoft.Json;
using pos_system.pos.BLL.Services;
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
    public partial class QueuedBillsForm : Form
    {
        private int _employeeId;
        private SalesService _salesService;

        private List<QueuedBill> _queuedBills;
        public QueuedBill SelectedBill { get; private set; }
        private DataGridView dgvQueuedBills;
        private Button btnRestore;
        private Button btnCancel;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;
        private DataGridView dgvBillItems;
        private SplitContainer splitContainer1;
        private Label lblBillDetails;
        private Button btnDelete;
        

        // Theme colors matching ItemsManagement
        private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
        private static readonly Color ForegroundColor = Color.Black;
        private static readonly Color SecondaryColor = Color.Gray;
        private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
        private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);
        private static readonly Color WarningColor = Color.FromArgb(255, 193, 7);

        public QueuedBillsForm(int employeeId)
        {
            _employeeId = employeeId;
            _salesService = new SalesService();
            InitializeComponent();
            InitializeDataGridView();
            LoadQueuedBills();
            SetupKeyboardShortcuts();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dgvQueuedBills = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.dgvBillItems = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblBillDetails = new System.Windows.Forms.Label();

            // Form setup
            this.Size = new Size(1000, 550);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = BackgroundColor;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Queued Bills - Select Bill to Restore";
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main container
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor,
                Padding = new Padding(5)
            };

            // Split Container
            this.splitContainer1 = new SplitContainer();
            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.Orientation = Orientation.Horizontal;
            this.splitContainer1.Panel1.BackColor = BackgroundColor;
            this.splitContainer1.Panel2.BackColor = BackgroundColor;
            this.splitContainer1.SplitterDistance = 50; // Adjust as needed\

            // Bill Details Label
            this.lblBillDetails = new Label
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = HeaderColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Text = "Select a bill to view details"
            };

            // Queued Bills Grid (Top)
            this.dgvQueuedBills = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = BackgroundColor,
                ForeColor = ForegroundColor,
                AutoGenerateColumns = false,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
            };

            // Bill Items Grid (Bottom)
            this.dgvBillItems = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = BackgroundColor,
                ForeColor = ForegroundColor,
                AutoGenerateColumns = false,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
            };

            // Apply grid styling (same as ItemsManagement)
            ApplyGridStyling(dgvQueuedBills);
            ApplyGridStyling(dgvBillItems);

            // Button Panel
            this.panel1 = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = HeaderColor,
                Padding = new Padding(20)
            };

            this.tableLayoutPanel1 = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));

            // Buttons with ItemsManagement styling
            this.btnDelete = CreateToolbarButton("DELETE", DeleteColor); // Red color for delete
            this.btnRestore = CreateToolbarButton("RESTORE", PrimaryColor);
            this.btnCancel = CreateToolbarButton("CANCEL", WarningColor);

            this.btnDelete.Click += btnDelete_Click;
            this.btnRestore.Click += btnRestore_Click;
            this.btnCancel.Click += btnCancel_Click;

            this.tableLayoutPanel1.Controls.Add(this.btnDelete, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRestore, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 3, 0);

            // Layout
            this.splitContainer1.Panel1.Controls.Add(this.dgvQueuedBills);
            this.splitContainer1.Panel1.Controls.Add(this.lblBillDetails);
            this.splitContainer1.Panel2.Controls.Add(this.dgvBillItems);

            this.panel1.Controls.Add(this.tableLayoutPanel1);

            container.Controls.Add(this.splitContainer1);
            container.Controls.Add(this.panel1);

            this.Controls.Add(container);

            // Events
            this.dgvQueuedBills.CellDoubleClick += DgvQueuedBills_CellDoubleClick;
            this.dgvQueuedBills.KeyDown += DgvQueuedBills_KeyDown;
            this.dgvQueuedBills.SelectionChanged += DgvQueuedBills_SelectionChanged;
        }

        private void ApplyGridStyling(DataGridView grid)
        {
            // Grid styling 
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 5, 0)
            };
            grid.ColumnHeadersHeight = 35;

            grid.RowTemplate.Height = 30;
            grid.RowTemplate.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);

            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BackgroundColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 12),
                SelectionBackColor = SelectionColor,
                SelectionForeColor = ForegroundColor
            };

            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 249, 255)
            };
        }

        private Button CreateToolbarButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {
                    BorderSize = 0,
                    MouseOverBackColor = ControlPaint.Light(backColor, 0.2f)
                },
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Margin = new Padding(10, 0, 10, 0),
                Cursor = Cursors.Hand,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
        }

        private void InitializeDataGridView()
        {
            // Configure Queued Bills grid (top) - Same styling as ItemsManagement
            dgvQueuedBills.Columns.Clear();

            // Add columns with sorting disabled and ItemsManagement styling
            dgvQueuedBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QueuePosition",
                HeaderText = "#",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvQueuedBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Bill_ID",
                HeaderText = "BILL ID",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvQueuedBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemCount",
                HeaderText = "ITEMS",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvQueuedBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SubTotal",
                HeaderText = "AMOUNT",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvQueuedBills.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PausedAt",
                HeaderText = "PAUSED TIME",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            // CartData column is not added, so it's hidden

            // Configure Bill Items grid (bottom) - Same styling as ItemsManagement
            dgvBillItems.Columns.Clear();

            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Brand",
                HeaderText = "BRAND",
                Width = 120,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Category",
                HeaderText = "CATEGORY",
                Width = 120,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Description",
                HeaderText = "DESCRIPTION",
                Width = 200,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Size",
                HeaderText = "SIZE",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                HeaderText = "PRICE",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DiscountAmountPerItem",
                HeaderText = "DISCOUNT",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity",
                HeaderText = "QTY",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            dgvBillItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NetPrice",
                HeaderText = "NET PRICE",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            dgvQueuedBills.DataSource = _queuedBills;
        }

        private void SetupKeyboardShortcuts()
        {
            // Set AcceptButton and CancelButton for Enter and Esc keys
            this.AcceptButton = btnRestore;
            this.CancelButton = btnCancel;

            // Also handle key events manually for additional control
            this.KeyPreview = true;
            this.KeyDown += QueuedBillsForm_KeyDown;
            dgvQueuedBills.KeyDown += DgvQueuedBills_KeyDown;
        }

        private void QueuedBillsForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (dgvQueuedBills.Focused)
                    {
                        RestoreSelected();
                        e.Handled = true;
                    }
                    break;
                case Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    e.Handled = true;
                    break;
            }
        }

        private void DgvQueuedBills_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    RestoreSelected();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    e.Handled = true;
                    break;
            }
        }

        //private void LoadQueuedBills()
        //{
        //    dgvQueuedBills.DataSource = null;
        //    dgvQueuedBills.DataSource = _queuedBills;

        //    // Auto-select the first row if available
        //    if (dgvQueuedBills.Rows.Count > 0)
        //    {
        //        dgvQueuedBills.Rows[0].Selected = true;
        //        dgvQueuedBills.CurrentCell = dgvQueuedBills.Rows[0].Cells[0];
        //        UpdateBillDetails();
        //    }
        //}

        private void LoadQueuedBills()
        {
            try
            {
                _queuedBills = _salesService.GetQueuedBills(_employeeId);
                dgvQueuedBills.DataSource = _queuedBills;

                // Auto-select the first row if available
                if (dgvQueuedBills.Rows.Count > 0)
                {
                    dgvQueuedBills.Rows[0].Selected = true;
                    dgvQueuedBills.CurrentCell = dgvQueuedBills.Rows[0].Cells[0];
                    UpdateBillDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading queued bills: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvQueuedBills_SelectionChanged(object sender, EventArgs e)
        {
            UpdateBillDetails();
        }

        private void UpdateBillDetails()
        {
            if (dgvQueuedBills.SelectedRows.Count > 0)
            {
                var selectedBill = dgvQueuedBills.SelectedRows[0].DataBoundItem as QueuedBill;
                if (selectedBill != null)
                {
                    try
                    {
                        // Update header label
                        lblBillDetails.Text = $"Bill #{selectedBill.Bill_ID} - {selectedBill.ItemCount} items - Total: Rs.{selectedBill.SubTotal:N2} - Paused at: {selectedBill.PausedAt:yyyy-MM-dd HH:mm:ss}";

                        // Deserialize cart data and display items
                        var cartData = JsonConvert.DeserializeObject<CartData>(selectedBill.CartData);
                        if (cartData?.Items != null)
                        {
                            // Create a DataTable for the items
                            var itemsTable = new DataTable();
                            itemsTable.Columns.Add("Brand", typeof(string));
                            itemsTable.Columns.Add("Category", typeof(string));
                            itemsTable.Columns.Add("Description", typeof(string));
                            itemsTable.Columns.Add("Size", typeof(string));
                            itemsTable.Columns.Add("Price", typeof(decimal));
                            itemsTable.Columns.Add("DiscountAmountPerItem", typeof(decimal));
                            itemsTable.Columns.Add("Quantity", typeof(int));
                            itemsTable.Columns.Add("NetPrice", typeof(decimal));

                            foreach (var item in cartData.Items)
                            {
                                decimal netPrice = (item.Price - item.DiscountAmountPerItem) * item.Quantity;
                                itemsTable.Rows.Add(
                                    item.Brand,
                                    item.Category,
                                    item.Description,
                                    item.Size,
                                    item.Price,
                                    item.DiscountAmountPerItem,
                                    item.Quantity,
                                    netPrice
                                );
                            }

                            dgvBillItems.DataSource = itemsTable;
                        }
                        else
                        {
                            dgvBillItems.DataSource = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        lblBillDetails.Text = $"Bill #{selectedBill.Bill_ID} - Error loading details";
                        dgvBillItems.DataSource = null;
                        Console.WriteLine($"Error loading bill details: {ex.Message}");
                    }
                }
            }
            else
            {
                lblBillDetails.Text = "Select a bill to view details";
                dgvBillItems.DataSource = null;
            }
        }

        private void DgvQueuedBills_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                RestoreSelected();
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            RestoreSelected();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void RestoreSelected()
        {
            if (dgvQueuedBills.SelectedRows.Count > 0)
            {
                SelectedBill = dgvQueuedBills.SelectedRows[0].DataBoundItem as QueuedBill;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a bill to restore", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvQueuedBills.SelectedRows.Count > 0)
            {
                var selectedBill = dgvQueuedBills.SelectedRows[0].DataBoundItem as QueuedBill;
                if (selectedBill != null)
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to delete Bill #{selectedBill.Bill_ID} from the queue?",
                        "Confirm Deletion",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            var salesService = new SalesService();
                            bool success = salesService.DeleteQueuedBill(selectedBill.Queue_ID);

                            if (success)
                            {
                                MessageBox.Show("Queued bill deleted successfully", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Reload the queued bills
                                _queuedBills = salesService.GetQueuedBills(_employeeId);
                                dgvQueuedBills.DataSource = _queuedBills;

                                // Clear bill details
                                lblBillDetails.Text = "Select a bill to view details";
                                dgvBillItems.DataSource = null;
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete queued bill", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting queued bill: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a bill to delete", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Prevent F3 from causing sorting errors in DataGridView
            if (keyData == Keys.F3)
            {
                return true; // Handled, don't process further
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
