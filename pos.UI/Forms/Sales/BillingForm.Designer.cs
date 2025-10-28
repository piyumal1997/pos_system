using pos_system.pos.UI.Forms;
using pos_system;
using pos_system.pos;
using pos_system.pos.UI;

namespace pos_system.pos.UI.Forms.Sales
{
    partial class BillingForm
    {
        private System.ComponentModel.IContainer components = null;

        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color LightBlue = Color.FromArgb(189, 224, 254);
        private readonly Color White = Color.White;
        private readonly Color DarkText = Color.FromArgb(64, 64, 64);
        protected Label lblDateTime;
        protected TextBox txtBarcode;
        protected DataGridView dgvCart;
        protected Label lblItemCount;
        protected Label lblSubtotal;
        protected Label lblTotalDiscount;
        protected Label lblTotal;
        protected Button btnProcessPayment;
        protected Button btnAddItem;
        protected Button btnClearBill;
        protected Button btnApplyBillDiscount;
        protected Button btnClearDiscounts;
        protected Label lblBillDiscount;

        protected DataGridViewButtonColumn colDelete;
        protected DataGridViewTextBoxColumn colQuantity;
        protected DataGridViewTextBoxColumn colDiscount;
        protected DataGridViewTextBoxColumn colSize;

        private TableLayoutPanel container;
        private Panel headerPanel;
        private Panel spacer;
        private Button btnRefreshBill;
        private Panel billIdPanel;
        private Label lblBillId;
        private Panel barcodePanel;
        private TableLayoutPanel barcodeContainer;
        private Panel summaryPanel;
        private TableLayoutPanel summaryLayout;
        private Panel itemsTotalPanel;
        private TableLayoutPanel itemsTotalLayout;
        private Label itemsLabel;
        private Label subtotalLabel;
        private Label discountLabel;
        private Label totalLabel;
        private Panel paymentPanel;
        private TableLayoutPanel paymentLayout;
        private TableLayoutPanel tableLayoutHeader;

        // Token UI components
        private TextBox txtTokenId;
        private Button btnApplyToken;

        // Queue Management UI Components
        private Button btnPauseBill;
        private Button btnViewQueuedBills;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem queueMenu;
        private ToolStripMenuItem menuPauseBill;
        private ToolStripMenuItem menuViewQueuedBills;



        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BillingForm));
            this.container = new System.Windows.Forms.TableLayoutPanel();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.tableLayoutHeader = new System.Windows.Forms.TableLayoutPanel();
            this.btnRefreshBill = new System.Windows.Forms.Button();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.billIdPanel = new System.Windows.Forms.Panel();
            this.lblBillId = new System.Windows.Forms.Label();
            this.barcodePanel = new System.Windows.Forms.Panel();
            this.barcodeContainer = new System.Windows.Forms.TableLayoutPanel();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.btnClearBill = new System.Windows.Forms.Button();
            this.btnApplyBillDiscount = new System.Windows.Forms.Button();
            this.btnClearDiscounts = new System.Windows.Forms.Button();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.txtTokenId = new System.Windows.Forms.TextBox();
            this.btnApplyToken = new System.Windows.Forms.Button();
            this.btnPauseBill = new System.Windows.Forms.Button();
            this.btnViewQueuedBills = new System.Windows.Forms.Button();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.summaryLayout = new System.Windows.Forms.TableLayoutPanel();
            this.itemsTotalPanel = new System.Windows.Forms.Panel();
            this.itemsTotalLayout = new System.Windows.Forms.TableLayoutPanel();
            this.itemsLabel = new System.Windows.Forms.Label();
            this.lblItemCount = new System.Windows.Forms.Label();
            this.subtotalLabel = new System.Windows.Forms.Label();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.discountLabel = new System.Windows.Forms.Label();
            this.lblTotalDiscount = new System.Windows.Forms.Label();
            this.lblBillDiscount = new System.Windows.Forms.Label();
            this.totalLabel = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.paymentPanel = new System.Windows.Forms.Panel();
            this.paymentLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnProcessPayment = new System.Windows.Forms.Button();
            this.spacer = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.queueMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPauseBill = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewQueuedBills = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvCart = new System.Windows.Forms.DataGridView();
            this.container.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.tableLayoutHeader.SuspendLayout();
            this.billIdPanel.SuspendLayout();
            this.barcodePanel.SuspendLayout();
            this.barcodeContainer.SuspendLayout();
            this.summaryPanel.SuspendLayout();
            this.summaryLayout.SuspendLayout();
            this.itemsTotalPanel.SuspendLayout();
            this.itemsTotalLayout.SuspendLayout();
            this.paymentPanel.SuspendLayout();
            this.paymentLayout.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
            this.SuspendLayout();
            // 
            // container
            // 
            this.container.ColumnCount = 1;
            this.container.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.container.Controls.Add(this.headerPanel, 0, 0);
            this.container.Controls.Add(this.billIdPanel, 0, 1);
            this.container.Controls.Add(this.barcodePanel, 0, 2);
            this.container.Controls.Add(this.dgvCart, 0, 3);
            this.container.Controls.Add(this.summaryPanel, 0, 4);
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(0, 24);
            this.container.Name = "container";
            this.container.RowCount = 5;
            this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.container.Size = new System.Drawing.Size(1200, 676);
            this.container.TabIndex = 0;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.headerPanel.Controls.Add(this.tableLayoutHeader);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerPanel.Location = new System.Drawing.Point(3, 3);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1194, 34);
            this.headerPanel.TabIndex = 0;
            // 
            // tableLayoutHeader
            // 
            this.tableLayoutHeader.ColumnCount = 2;
            this.tableLayoutHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutHeader.Controls.Add(this.btnRefreshBill, 1, 0);
            this.tableLayoutHeader.Controls.Add(this.lblDateTime, 0, 0);
            this.tableLayoutHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutHeader.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutHeader.Name = "tableLayoutHeader";
            this.tableLayoutHeader.RowCount = 1;
            this.tableLayoutHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutHeader.Size = new System.Drawing.Size(1194, 34);
            this.tableLayoutHeader.TabIndex = 0;
            // 
            // btnRefreshBill
            // 
            this.btnRefreshBill.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRefreshBill.BackColor = System.Drawing.Color.SteelBlue;
            this.btnRefreshBill.FlatAppearance.BorderSize = 0;
            this.btnRefreshBill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshBill.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefreshBill.ForeColor = System.Drawing.Color.White;
            this.btnRefreshBill.Location = new System.Drawing.Point(1114, 3);
            this.btnRefreshBill.Name = "btnRefreshBill";
            this.btnRefreshBill.Size = new System.Drawing.Size(77, 28);
            this.btnRefreshBill.TabIndex = 2;
            this.btnRefreshBill.Text = "Refresh";
            this.btnRefreshBill.UseVisualStyleBackColor = false;
            // 
            // lblDateTime
            // 
            this.lblDateTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateTime.ForeColor = System.Drawing.Color.White;
            this.lblDateTime.Location = new System.Drawing.Point(10, 4);
            this.lblDateTime.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(170, 25);
            this.lblDateTime.TabIndex = 0;
            this.lblDateTime.Text = "2023-01-01 00:00";
            this.lblDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // billIdPanel
            // 
            this.billIdPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(224)))), ((int)(((byte)(254)))));
            this.billIdPanel.Controls.Add(this.lblBillId);
            this.billIdPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.billIdPanel.Location = new System.Drawing.Point(3, 43);
            this.billIdPanel.Name = "billIdPanel";
            this.billIdPanel.Size = new System.Drawing.Size(1194, 34);
            this.billIdPanel.TabIndex = 1;
            // 
            // lblBillId
            // 
            this.lblBillId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBillId.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblBillId.Location = new System.Drawing.Point(0, 0);
            this.lblBillId.Name = "lblBillId";
            this.lblBillId.Size = new System.Drawing.Size(1194, 34);
            this.lblBillId.TabIndex = 0;
            this.lblBillId.Text = "New Bill";
            this.lblBillId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // barcodePanel
            // 
            this.barcodePanel.Controls.Add(this.barcodeContainer);
            this.barcodePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.barcodePanel.Location = new System.Drawing.Point(3, 83);
            this.barcodePanel.Name = "barcodePanel";
            this.barcodePanel.Size = new System.Drawing.Size(1194, 114);
            this.barcodePanel.TabIndex = 2;
            // 
            // barcodeContainer
            // 
            this.barcodeContainer.ColumnCount = 4;
            this.barcodeContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.barcodeContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.barcodeContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.barcodeContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.barcodeContainer.Controls.Add(this.txtBarcode, 0, 0);
            this.barcodeContainer.Controls.Add(this.btnClearBill, 1, 0);
            this.barcodeContainer.Controls.Add(this.btnApplyBillDiscount, 2, 0);
            this.barcodeContainer.Controls.Add(this.btnClearDiscounts, 3, 0);
            this.barcodeContainer.Controls.Add(this.btnAddItem, 0, 1);
            this.barcodeContainer.Controls.Add(this.txtTokenId, 1, 1);
            this.barcodeContainer.Controls.Add(this.btnApplyToken, 2, 1);
            this.barcodeContainer.Controls.Add(this.btnPauseBill, 3, 1);
            this.barcodeContainer.Controls.Add(this.btnViewQueuedBills, 0, 2);
            this.barcodeContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.barcodeContainer.Location = new System.Drawing.Point(0, 0);
            this.barcodeContainer.Name = "barcodeContainer";
            this.barcodeContainer.RowCount = 3;
            this.barcodeContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.barcodeContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.barcodeContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.barcodeContainer.Size = new System.Drawing.Size(1194, 114);
            this.barcodeContainer.TabIndex = 0;
            // 
            // txtBarcode
            // 
            this.txtBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBarcode.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.txtBarcode.Location = new System.Drawing.Point(3, 3);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.PlaceholderText = "Scan Barcode or Enter";
            this.txtBarcode.Size = new System.Drawing.Size(471, 36);
            this.txtBarcode.TabIndex = 0;
            // 
            // btnClearBill
            // 
            this.btnClearBill.BackColor = System.Drawing.Color.IndianRed;
            this.btnClearBill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClearBill.FlatAppearance.BorderSize = 0;
            this.btnClearBill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearBill.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClearBill.ForeColor = System.Drawing.Color.White;
            this.btnClearBill.Location = new System.Drawing.Point(480, 3);
            this.btnClearBill.Name = "btnClearBill";
            this.btnClearBill.Size = new System.Drawing.Size(232, 31);
            this.btnClearBill.TabIndex = 2;
            this.btnClearBill.Text = "Clear Bill (F1)";
            this.btnClearBill.UseVisualStyleBackColor = false;
            // 
            // btnApplyBillDiscount
            // 
            this.btnApplyBillDiscount.BackColor = System.Drawing.Color.Goldenrod;
            this.btnApplyBillDiscount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnApplyBillDiscount.FlatAppearance.BorderSize = 0;
            this.btnApplyBillDiscount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyBillDiscount.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnApplyBillDiscount.ForeColor = System.Drawing.Color.Black;
            this.btnApplyBillDiscount.Location = new System.Drawing.Point(718, 3);
            this.btnApplyBillDiscount.Name = "btnApplyBillDiscount";
            this.btnApplyBillDiscount.Size = new System.Drawing.Size(232, 31);
            this.btnApplyBillDiscount.TabIndex = 3;
            this.btnApplyBillDiscount.Text = "Apply Bill Disc";
            this.btnApplyBillDiscount.UseVisualStyleBackColor = false;
            // 
            // btnClearDiscounts
            // 
            this.btnClearDiscounts.BackColor = System.Drawing.Color.Orange;
            this.btnClearDiscounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClearDiscounts.FlatAppearance.BorderSize = 0;
            this.btnClearDiscounts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearDiscounts.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClearDiscounts.ForeColor = System.Drawing.Color.Black;
            this.btnClearDiscounts.Location = new System.Drawing.Point(956, 3);
            this.btnClearDiscounts.Name = "btnClearDiscounts";
            this.btnClearDiscounts.Size = new System.Drawing.Size(235, 31);
            this.btnClearDiscounts.TabIndex = 4;
            this.btnClearDiscounts.Text = "Clear Disc";
            this.btnClearDiscounts.UseVisualStyleBackColor = false;
            // 
            // btnAddItem
            // 
            this.btnAddItem.BackColor = System.Drawing.Color.SteelBlue;
            this.btnAddItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddItem.FlatAppearance.BorderSize = 0;
            this.btnAddItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddItem.ForeColor = System.Drawing.Color.White;
            this.btnAddItem.Location = new System.Drawing.Point(3, 41);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(471, 31);
            this.btnAddItem.TabIndex = 1;
            this.btnAddItem.Text = "Add Item (F11)";
            this.btnAddItem.UseVisualStyleBackColor = false;
            // 
            // txtTokenId
            // 
            this.txtTokenId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTokenId.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtTokenId.Location = new System.Drawing.Point(480, 41);
            this.txtTokenId.Name = "txtTokenId";
            this.txtTokenId.PlaceholderText = "Enter Token ID";
            this.txtTokenId.Size = new System.Drawing.Size(232, 29);
            this.txtTokenId.TabIndex = 5;
            // 
            // btnApplyToken
            // 
            this.btnApplyToken.BackColor = System.Drawing.Color.MediumPurple;
            this.btnApplyToken.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnApplyToken.FlatAppearance.BorderSize = 0;
            this.btnApplyToken.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyToken.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnApplyToken.ForeColor = System.Drawing.Color.White;
            this.btnApplyToken.Location = new System.Drawing.Point(718, 41);
            this.btnApplyToken.Name = "btnApplyToken";
            this.btnApplyToken.Size = new System.Drawing.Size(232, 31);
            this.btnApplyToken.TabIndex = 6;
            this.btnApplyToken.Text = "Apply Token";
            this.btnApplyToken.UseVisualStyleBackColor = false;
            // 
            // btnPauseBill
            // 
            this.btnPauseBill.BackColor = System.Drawing.Color.Orange;
            this.btnPauseBill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPauseBill.FlatAppearance.BorderSize = 0;
            this.btnPauseBill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPauseBill.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPauseBill.ForeColor = System.Drawing.Color.White;
            this.btnPauseBill.Location = new System.Drawing.Point(956, 41);
            this.btnPauseBill.Name = "btnPauseBill";
            this.btnPauseBill.Size = new System.Drawing.Size(235, 31);
            this.btnPauseBill.TabIndex = 7;
            this.btnPauseBill.Text = "Pause Bill (F2)";
            this.btnPauseBill.UseVisualStyleBackColor = false;
            // 
            // btnViewQueuedBills
            // 
            this.barcodeContainer.SetColumnSpan(this.btnViewQueuedBills, 4);
            this.btnViewQueuedBills.BackColor = System.Drawing.Color.Teal;
            this.btnViewQueuedBills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnViewQueuedBills.FlatAppearance.BorderSize = 0;
            this.btnViewQueuedBills.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewQueuedBills.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnViewQueuedBills.ForeColor = System.Drawing.Color.White;
            this.btnViewQueuedBills.Location = new System.Drawing.Point(3, 79);
            this.btnViewQueuedBills.Name = "btnViewQueuedBills";
            this.btnViewQueuedBills.Size = new System.Drawing.Size(1188, 32);
            this.btnViewQueuedBills.TabIndex = 8;
            this.btnViewQueuedBills.Text = "Queued Bills (F3)";
            this.btnViewQueuedBills.UseVisualStyleBackColor = false;
            // 
            // summaryPanel
            // 
            this.summaryPanel.Controls.Add(this.summaryLayout);
            this.summaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryPanel.Location = new System.Drawing.Point(3, 529);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Size = new System.Drawing.Size(1194, 144);
            this.summaryPanel.TabIndex = 3;
            // 
            // summaryLayout
            // 
            this.summaryLayout.ColumnCount = 2;
            this.summaryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.summaryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.summaryLayout.Controls.Add(this.itemsTotalPanel, 0, 0);
            this.summaryLayout.Controls.Add(this.paymentPanel, 1, 0);
            this.summaryLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryLayout.Location = new System.Drawing.Point(0, 0);
            this.summaryLayout.Name = "summaryLayout";
            this.summaryLayout.RowCount = 1;
            this.summaryLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.summaryLayout.Size = new System.Drawing.Size(1194, 144);
            this.summaryLayout.TabIndex = 0;
            // 
            // itemsTotalPanel
            // 
            this.itemsTotalPanel.Controls.Add(this.itemsTotalLayout);
            this.itemsTotalPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsTotalPanel.Location = new System.Drawing.Point(3, 3);
            this.itemsTotalPanel.Name = "itemsTotalPanel";
            this.itemsTotalPanel.Size = new System.Drawing.Size(770, 138);
            this.itemsTotalPanel.TabIndex = 0;
            // 
            // itemsTotalLayout
            // 
            this.itemsTotalLayout.ColumnCount = 2;
            this.itemsTotalLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.itemsTotalLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.itemsTotalLayout.Controls.Add(this.itemsLabel, 0, 0);
            this.itemsTotalLayout.Controls.Add(this.lblItemCount, 1, 0);
            this.itemsTotalLayout.Controls.Add(this.subtotalLabel, 0, 1);
            this.itemsTotalLayout.Controls.Add(this.lblSubtotal, 1, 1);
            this.itemsTotalLayout.Controls.Add(this.discountLabel, 0, 2);
            this.itemsTotalLayout.Controls.Add(this.lblTotalDiscount, 1, 2);
            this.itemsTotalLayout.Controls.Add(this.lblBillDiscount, 1, 3);
            this.itemsTotalLayout.Controls.Add(this.totalLabel, 0, 4);
            this.itemsTotalLayout.Controls.Add(this.lblTotal, 1, 4);
            this.itemsTotalLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsTotalLayout.Location = new System.Drawing.Point(0, 0);
            this.itemsTotalLayout.Name = "itemsTotalLayout";
            this.itemsTotalLayout.RowCount = 5;
            this.itemsTotalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.itemsTotalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.itemsTotalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.itemsTotalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.itemsTotalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.itemsTotalLayout.Size = new System.Drawing.Size(770, 138);
            this.itemsTotalLayout.TabIndex = 0;
            // 
            // itemsLabel
            // 
            this.itemsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.itemsLabel.Location = new System.Drawing.Point(3, 0);
            this.itemsLabel.Name = "itemsLabel";
            this.itemsLabel.Size = new System.Drawing.Size(379, 27);
            this.itemsLabel.TabIndex = 0;
            this.itemsLabel.Text = "Items:";
            this.itemsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblItemCount
            // 
            this.lblItemCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblItemCount.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblItemCount.Location = new System.Drawing.Point(388, 0);
            this.lblItemCount.Name = "lblItemCount";
            this.lblItemCount.Size = new System.Drawing.Size(379, 27);
            this.lblItemCount.TabIndex = 1;
            this.lblItemCount.Text = "0";
            this.lblItemCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtotalLabel
            // 
            this.subtotalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtotalLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.subtotalLabel.Location = new System.Drawing.Point(3, 27);
            this.subtotalLabel.Name = "subtotalLabel";
            this.subtotalLabel.Size = new System.Drawing.Size(379, 27);
            this.subtotalLabel.TabIndex = 8;
            this.subtotalLabel.Text = "Subtotal:";
            this.subtotalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubtotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblSubtotal.Location = new System.Drawing.Point(388, 27);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(379, 27);
            this.lblSubtotal.TabIndex = 9;
            this.lblSubtotal.Text = "Rs.0.00";
            this.lblSubtotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // discountLabel
            // 
            this.discountLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.discountLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.discountLabel.Location = new System.Drawing.Point(3, 54);
            this.discountLabel.Name = "discountLabel";
            this.discountLabel.Size = new System.Drawing.Size(379, 27);
            this.discountLabel.TabIndex = 6;
            this.discountLabel.Text = "Item Discounts:";
            this.discountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotalDiscount
            // 
            this.lblTotalDiscount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalDiscount.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalDiscount.ForeColor = System.Drawing.Color.Red;
            this.lblTotalDiscount.Location = new System.Drawing.Point(388, 54);
            this.lblTotalDiscount.Name = "lblTotalDiscount";
            this.lblTotalDiscount.Size = new System.Drawing.Size(379, 27);
            this.lblTotalDiscount.TabIndex = 7;
            this.lblTotalDiscount.Text = "Rs.0.00";
            this.lblTotalDiscount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBillDiscount
            // 
            this.lblBillDiscount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBillDiscount.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblBillDiscount.ForeColor = System.Drawing.Color.Red;
            this.lblBillDiscount.Location = new System.Drawing.Point(388, 81);
            this.lblBillDiscount.Name = "lblBillDiscount";
            this.lblBillDiscount.Size = new System.Drawing.Size(379, 27);
            this.lblBillDiscount.TabIndex = 16;
            this.lblBillDiscount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // totalLabel
            // 
            this.totalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalLabel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.totalLabel.Location = new System.Drawing.Point(3, 108);
            this.totalLabel.Name = "totalLabel";
            this.totalLabel.Size = new System.Drawing.Size(379, 30);
            this.totalLabel.TabIndex = 15;
            this.totalLabel.Text = "Total:";
            this.totalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotal
            // 
            this.lblTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(388, 108);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(379, 30);
            this.lblTotal.TabIndex = 14;
            this.lblTotal.Text = "Rs.0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // paymentPanel
            // 
            this.paymentPanel.Controls.Add(this.paymentLayout);
            this.paymentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paymentPanel.Location = new System.Drawing.Point(779, 3);
            this.paymentPanel.Name = "paymentPanel";
            this.paymentPanel.Size = new System.Drawing.Size(412, 138);
            this.paymentPanel.TabIndex = 1;
            // 
            // paymentLayout
            // 
            this.paymentLayout.ColumnCount = 1;
            this.paymentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.paymentLayout.Controls.Add(this.btnProcessPayment, 0, 0);
            this.paymentLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paymentLayout.Location = new System.Drawing.Point(0, 0);
            this.paymentLayout.Name = "paymentLayout";
            this.paymentLayout.RowCount = 1;
            this.paymentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.paymentLayout.Size = new System.Drawing.Size(412, 138);
            this.paymentLayout.TabIndex = 0;
            // 
            // btnProcessPayment
            // 
            this.btnProcessPayment.BackColor = System.Drawing.Color.SeaGreen;
            this.btnProcessPayment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnProcessPayment.FlatAppearance.BorderSize = 0;
            this.btnProcessPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcessPayment.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnProcessPayment.ForeColor = System.Drawing.Color.White;
            this.btnProcessPayment.Location = new System.Drawing.Point(10, 10);
            this.btnProcessPayment.Margin = new System.Windows.Forms.Padding(10);
            this.btnProcessPayment.Name = "btnProcessPayment";
            this.btnProcessPayment.Size = new System.Drawing.Size(392, 118);
            this.btnProcessPayment.TabIndex = 1;
            this.btnProcessPayment.Text = "PROCESS PAYMENT (F12)";
            this.btnProcessPayment.UseVisualStyleBackColor = false;
            // 
            // spacer
            // 
            this.spacer.AutoSize = true;
            this.spacer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spacer.Location = new System.Drawing.Point(170, 0);
            this.spacer.Margin = new System.Windows.Forms.Padding(0);
            this.spacer.Name = "spacer";
            this.spacer.Size = new System.Drawing.Size(0, 35);
            this.spacer.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.queueMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1200, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // queueMenu
            // 
            this.queueMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPauseBill,
            this.menuViewQueuedBills});
            this.queueMenu.Name = "queueMenu";
            this.queueMenu.Size = new System.Drawing.Size(54, 20);
            this.queueMenu.Text = "&Queue";
            // 
            // menuPauseBill
            // 
            this.menuPauseBill.Name = "menuPauseBill";
            this.menuPauseBill.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.menuPauseBill.Size = new System.Drawing.Size(180, 22);
            this.menuPauseBill.Text = "&Pause Bill";
            // 
            // menuViewQueuedBills
            // 
            this.menuViewQueuedBills.Name = "menuViewQueuedBills";
            this.menuViewQueuedBills.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.menuViewQueuedBills.Size = new System.Drawing.Size(180, 22);
            this.menuViewQueuedBills.Text = "&View Queued Bills";
            // 
            // dgvCart
            // 
            this.dgvCart.AllowUserToAddRows = false;
            this.dgvCart.AllowUserToDeleteRows = false;
            this.dgvCart.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCart.BackgroundColor = System.Drawing.Color.White;
            this.dgvCart.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCart.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCart.Location = new System.Drawing.Point(3, 203);
            this.dgvCart.Name = "dgvCart";
            this.dgvCart.ReadOnly = true;
            this.dgvCart.Size = new System.Drawing.Size(1194, 320);
            this.dgvCart.TabIndex = 4;
            // 
            // BillingForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.container);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BillingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "POS System - Billing";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.container.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.tableLayoutHeader.ResumeLayout(false);
            this.tableLayoutHeader.PerformLayout();
            this.billIdPanel.ResumeLayout(false);
            this.barcodePanel.ResumeLayout(false);
            this.barcodeContainer.ResumeLayout(false);
            this.barcodeContainer.PerformLayout();
            this.summaryPanel.ResumeLayout(false);
            this.summaryLayout.ResumeLayout(false);
            this.itemsTotalPanel.ResumeLayout(false);
            this.itemsTotalLayout.ResumeLayout(false);
            this.paymentPanel.ResumeLayout(false);
            this.paymentLayout.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}