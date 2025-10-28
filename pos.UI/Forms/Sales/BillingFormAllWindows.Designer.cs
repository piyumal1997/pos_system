using System.Windows.Forms;
using System.Drawing;

namespace pos_system.pos.UI.Forms.Sales
{
    partial class BillingFormAllWindows
    {
        private System.ComponentModel.IContainer components = null;

        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color LightBlue = Color.FromArgb(189, 224, 254);
        private readonly Color White = Color.White;
        private readonly Color DarkText = Color.FromArgb(64, 64, 64);
        private readonly Color SuccessGreen = Color.FromArgb(46, 204, 113);
        private readonly Color WarningOrange = Color.FromArgb(230, 126, 34);
        private readonly Color DangerRed = Color.FromArgb(231, 76, 60);

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

        private TableLayoutPanel mainContainer;
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;
        private Panel barcodePanel;
        private Panel summaryPanel;
        private Panel paymentPanel;
        private Label lblBillId;
        private Button btnRefreshBill;

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
            this.mainContainer = new System.Windows.Forms.TableLayoutPanel();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblBillId = new System.Windows.Forms.Label();
            this.btnRefreshBill = new System.Windows.Forms.Button();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.dgvCart = new System.Windows.Forms.DataGridView();
            this.barcodePanel = new System.Windows.Forms.Panel();
            this.btnViewQueuedBills = new System.Windows.Forms.Button();
            this.btnPauseBill = new System.Windows.Forms.Button();
            this.btnApplyToken = new System.Windows.Forms.Button();
            this.txtTokenId = new System.Windows.Forms.TextBox();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnClearDiscounts = new System.Windows.Forms.Button();
            this.btnApplyBillDiscount = new System.Windows.Forms.Button();
            this.btnClearBill = new System.Windows.Forms.Button();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblBillDiscount = new System.Windows.Forms.Label();
            this.lblTotalDiscount = new System.Windows.Forms.Label();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblItemCount = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.paymentPanel = new System.Windows.Forms.Panel();
            this.btnProcessPayment = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.queueMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPauseBill = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewQueuedBills = new System.Windows.Forms.ToolStripMenuItem();

            this.mainContainer.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).BeginInit();
            this.barcodePanel.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.summaryPanel.SuspendLayout();
            this.paymentPanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();

            // 
            // mainContainer
            // 
            this.mainContainer.ColumnCount = 1;
            this.mainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainContainer.Controls.Add(this.headerPanel, 0, 0);
            this.mainContainer.Controls.Add(this.contentPanel, 0, 1);
            this.mainContainer.Controls.Add(this.footerPanel, 0, 2);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 24);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.RowCount = 3;
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.mainContainer.Size = new System.Drawing.Size(1200, 676);
            this.mainContainer.TabIndex = 0;

            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = this.PrimaryBlue;
            this.headerPanel.Controls.Add(this.lblBillId);
            this.headerPanel.Controls.Add(this.btnRefreshBill);
            this.headerPanel.Controls.Add(this.lblDateTime);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerPanel.Location = new System.Drawing.Point(3, 3);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(15, 8, 15, 8);
            this.headerPanel.Size = new System.Drawing.Size(1194, 54);
            this.headerPanel.TabIndex = 0;

            // 
            // lblBillId
            // 
            this.lblBillId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBillId.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblBillId.ForeColor = System.Drawing.Color.White;
            this.lblBillId.Location = new System.Drawing.Point(15, 8);
            this.lblBillId.Name = "lblBillId";
            this.lblBillId.Size = new System.Drawing.Size(1104, 38);
            this.lblBillId.TabIndex = 2;
            this.lblBillId.Text = "New Bill";
            this.lblBillId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // btnRefreshBill
            // 
            this.btnRefreshBill.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRefreshBill.BackColor = Color.FromArgb(52, 152, 219);
            this.btnRefreshBill.FlatAppearance.BorderSize = 0;
            this.btnRefreshBill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshBill.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefreshBill.ForeColor = System.Drawing.Color.White;
            this.btnRefreshBill.Location = new System.Drawing.Point(1124, 12);
            this.btnRefreshBill.Name = "btnRefreshBill";
            this.btnRefreshBill.Size = new System.Drawing.Size(55, 30);
            this.btnRefreshBill.TabIndex = 1;
            this.btnRefreshBill.Text = "Refresh";
            this.btnRefreshBill.UseVisualStyleBackColor = false;

            // 
            // lblDateTime
            // 
            this.lblDateTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDateTime.ForeColor = System.Drawing.Color.White;
            this.lblDateTime.Location = new System.Drawing.Point(12, 18);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(122, 19);
            this.lblDateTime.TabIndex = 0;
            this.lblDateTime.Text = "2023-01-01 00:00";

            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.dgvCart);
            this.contentPanel.Controls.Add(this.barcodePanel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(3, 63);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(1194, 430);
            this.contentPanel.TabIndex = 1;

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
            this.dgvCart.GridColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.dgvCart.Location = new System.Drawing.Point(0, 100);
            this.dgvCart.Name = "dgvCart";
            this.dgvCart.ReadOnly = true;
            this.dgvCart.RowHeadersVisible = false;
            this.dgvCart.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCart.Size = new System.Drawing.Size(1194, 330);
            this.dgvCart.TabIndex = 1;

            // 
            // barcodePanel
            // 
            this.barcodePanel.BackColor = this.LightBlue;
            this.barcodePanel.Controls.Add(this.btnViewQueuedBills);
            this.barcodePanel.Controls.Add(this.btnPauseBill);
            this.barcodePanel.Controls.Add(this.btnApplyToken);
            this.barcodePanel.Controls.Add(this.txtTokenId);
            this.barcodePanel.Controls.Add(this.btnAddItem);
            this.barcodePanel.Controls.Add(this.btnClearDiscounts);
            this.barcodePanel.Controls.Add(this.btnApplyBillDiscount);
            this.barcodePanel.Controls.Add(this.btnClearBill);
            this.barcodePanel.Controls.Add(this.txtBarcode);
            this.barcodePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.barcodePanel.Location = new System.Drawing.Point(0, 0);
            this.barcodePanel.Name = "barcodePanel";
            this.barcodePanel.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.barcodePanel.Size = new System.Drawing.Size(1194, 100);
            this.barcodePanel.TabIndex = 0;

            // 
            // btnViewQueuedBills
            // 
            this.btnViewQueuedBills.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnViewQueuedBills.BackColor = Color.FromArgb(149, 165, 166);
            this.btnViewQueuedBills.FlatAppearance.BorderSize = 0;
            this.btnViewQueuedBills.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewQueuedBills.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnViewQueuedBills.ForeColor = System.Drawing.Color.White;
            this.btnViewQueuedBills.Location = new System.Drawing.Point(547, 55);
            this.btnViewQueuedBills.Name = "btnViewQueuedBills";
            this.btnViewQueuedBills.Size = new System.Drawing.Size(100, 30);
            this.btnViewQueuedBills.TabIndex = 8;
            this.btnViewQueuedBills.Text = "Queued Bills";
            this.btnViewQueuedBills.UseVisualStyleBackColor = false;

            // 
            // btnPauseBill
            // 
            this.btnPauseBill.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnPauseBill.BackColor = this.WarningOrange;
            this.btnPauseBill.FlatAppearance.BorderSize = 0;
            this.btnPauseBill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPauseBill.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPauseBill.ForeColor = System.Drawing.Color.White;
            this.btnPauseBill.Location = new System.Drawing.Point(441, 55);
            this.btnPauseBill.Name = "btnPauseBill";
            this.btnPauseBill.Size = new System.Drawing.Size(100, 30);
            this.btnPauseBill.TabIndex = 7;
            this.btnPauseBill.Text = "Pause Bill";
            this.btnPauseBill.UseVisualStyleBackColor = false;

            // 
            // btnApplyToken
            // 
            this.btnApplyToken.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnApplyToken.BackColor = Color.FromArgb(155, 89, 182);
            this.btnApplyToken.FlatAppearance.BorderSize = 0;
            this.btnApplyToken.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyToken.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApplyToken.ForeColor = System.Drawing.Color.White;
            this.btnApplyToken.Location = new System.Drawing.Point(653, 55);
            this.btnApplyToken.Name = "btnApplyToken";
            this.btnApplyToken.Size = new System.Drawing.Size(100, 30);
            this.btnApplyToken.TabIndex = 6;
            this.btnApplyToken.Text = "Apply Token";
            this.btnApplyToken.UseVisualStyleBackColor = false;

            // 
            // txtTokenId
            // 
            this.txtTokenId.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtTokenId.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtTokenId.Location = new System.Drawing.Point(441, 20);
            this.txtTokenId.Name = "txtTokenId";
            this.txtTokenId.PlaceholderText = "Enter Token ID";
            this.txtTokenId.Size = new System.Drawing.Size(312, 25);
            this.txtTokenId.TabIndex = 5;

            // 
            // btnAddItem
            // 
            this.btnAddItem.BackColor = this.PrimaryBlue;
            this.btnAddItem.FlatAppearance.BorderSize = 0;
            this.btnAddItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddItem.ForeColor = System.Drawing.Color.White;
            this.btnAddItem.Location = new System.Drawing.Point(18, 55);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(120, 30);
            this.btnAddItem.TabIndex = 1;
            this.btnAddItem.Text = "Add Item";
            this.btnAddItem.UseVisualStyleBackColor = false;

            // 
            // btnClearDiscounts
            // 
            this.btnClearDiscounts.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnClearDiscounts.BackColor = Color.FromArgb(243, 156, 18);
            this.btnClearDiscounts.FlatAppearance.BorderSize = 0;
            this.btnClearDiscounts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearDiscounts.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClearDiscounts.ForeColor = System.Drawing.Color.White;
            this.btnClearDiscounts.Location = new System.Drawing.Point(759, 55);
            this.btnClearDiscounts.Name = "btnClearDiscounts";
            this.btnClearDiscounts.Size = new System.Drawing.Size(100, 30);
            this.btnClearDiscounts.TabIndex = 4;
            this.btnClearDiscounts.Text = "Clear Disc";
            this.btnClearDiscounts.UseVisualStyleBackColor = false;

            // 
            // btnApplyBillDiscount
            // 
            this.btnApplyBillDiscount.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnApplyBillDiscount.BackColor = Color.FromArgb(241, 196, 15);
            this.btnApplyBillDiscount.FlatAppearance.BorderSize = 0;
            this.btnApplyBillDiscount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyBillDiscount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApplyBillDiscount.ForeColor = System.Drawing.Color.Black;
            this.btnApplyBillDiscount.Location = new System.Drawing.Point(335, 55);
            this.btnApplyBillDiscount.Name = "btnApplyBillDiscount";
            this.btnApplyBillDiscount.Size = new System.Drawing.Size(100, 30);
            this.btnApplyBillDiscount.TabIndex = 3;
            this.btnApplyBillDiscount.Text = "Bill Discount";
            this.btnApplyBillDiscount.UseVisualStyleBackColor = false;

            // 
            // btnClearBill
            // 
            this.btnClearBill.BackColor = this.DangerRed;
            this.btnClearBill.FlatAppearance.BorderSize = 0;
            this.btnClearBill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearBill.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnClearBill.ForeColor = System.Drawing.Color.White;
            this.btnClearBill.Location = new System.Drawing.Point(144, 55);
            this.btnClearBill.Name = "btnClearBill";
            this.btnClearBill.Size = new System.Drawing.Size(100, 30);
            this.btnClearBill.TabIndex = 2;
            this.btnClearBill.Text = "Clear Bill";
            this.btnClearBill.UseVisualStyleBackColor = false;

            // 
            // txtBarcode
            // 
            this.txtBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBarcode.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtBarcode.Location = new System.Drawing.Point(18, 17);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.PlaceholderText = "Scan Barcode or Enter Item Code";
            this.txtBarcode.Size = new System.Drawing.Size(1158, 29);
            this.txtBarcode.TabIndex = 0;

            // 
            // footerPanel
            // 
            this.footerPanel.Controls.Add(this.summaryPanel);
            this.footerPanel.Controls.Add(this.paymentPanel);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerPanel.Location = new System.Drawing.Point(3, 499);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(1194, 174);
            this.footerPanel.TabIndex = 2;

            // 
            // summaryPanel
            // 
            this.summaryPanel.BackColor = Color.FromArgb(250, 250, 250);
            this.summaryPanel.Controls.Add(this.lblTotal);
            this.summaryPanel.Controls.Add(this.lblBillDiscount);
            this.summaryPanel.Controls.Add(this.lblTotalDiscount);
            this.summaryPanel.Controls.Add(this.lblSubtotal);
            this.summaryPanel.Controls.Add(this.lblItemCount);
            this.summaryPanel.Controls.Add(this.label5);
            this.summaryPanel.Controls.Add(this.label4);
            this.summaryPanel.Controls.Add(this.label3);
            this.summaryPanel.Controls.Add(this.label2);
            this.summaryPanel.Controls.Add(this.label1);
            this.summaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryPanel.Location = new System.Drawing.Point(0, 0);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Padding = new System.Windows.Forms.Padding(20);
            this.summaryPanel.Size = new System.Drawing.Size(794, 174);
            this.summaryPanel.TabIndex = 0;

            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = this.PrimaryBlue;
            this.lblTotal.Location = new System.Drawing.Point(550, 115);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(200, 35);
            this.lblTotal.TabIndex = 9;
            this.lblTotal.Text = "Rs.0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // lblBillDiscount
            // 
            this.lblBillDiscount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBillDiscount.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblBillDiscount.ForeColor = this.DangerRed;
            this.lblBillDiscount.Location = new System.Drawing.Point(550, 75);
            this.lblBillDiscount.Name = "lblBillDiscount";
            this.lblBillDiscount.Size = new System.Drawing.Size(200, 25);
            this.lblBillDiscount.TabIndex = 8;
            this.lblBillDiscount.Text = "Rs.0.00";
            this.lblBillDiscount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // lblTotalDiscount
            // 
            this.lblTotalDiscount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalDiscount.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTotalDiscount.ForeColor = this.DangerRed;
            this.lblTotalDiscount.Location = new System.Drawing.Point(550, 50);
            this.lblTotalDiscount.Name = "lblTotalDiscount";
            this.lblTotalDiscount.Size = new System.Drawing.Size(200, 25);
            this.lblTotalDiscount.TabIndex = 7;
            this.lblTotalDiscount.Text = "Rs.0.00";
            this.lblTotalDiscount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // lblSubtotal
            // 
            this.lblSubtotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubtotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblSubtotal.Location = new System.Drawing.Point(550, 25);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(200, 25);
            this.lblSubtotal.TabIndex = 6;
            this.lblSubtotal.Text = "Rs.0.00";
            this.lblSubtotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // lblItemCount
            // 
            this.lblItemCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblItemCount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblItemCount.Location = new System.Drawing.Point(550, 0);
            this.lblItemCount.Name = "lblItemCount";
            this.lblItemCount.Size = new System.Drawing.Size(200, 25);
            this.lblItemCount.TabIndex = 5;
            this.lblItemCount.Text = "0";
            this.lblItemCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(23, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(500, 35);
            this.label5.TabIndex = 4;
            this.label5.Text = "TOTAL:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label4.Location = new System.Drawing.Point(23, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(500, 25);
            this.label4.TabIndex = 3;
            this.label4.Text = "Bill Discount:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label3.Location = new System.Drawing.Point(23, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(500, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "Item Discounts:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(23, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(500, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Subtotal:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(23, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(500, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Items:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // paymentPanel
            // 
            this.paymentPanel.BackColor = Color.FromArgb(250, 250, 250);
            this.paymentPanel.Controls.Add(this.btnProcessPayment);
            this.paymentPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.paymentPanel.Location = new System.Drawing.Point(794, 0);
            this.paymentPanel.Name = "paymentPanel";
            this.paymentPanel.Padding = new System.Windows.Forms.Padding(10);
            this.paymentPanel.Size = new System.Drawing.Size(400, 174);
            this.paymentPanel.TabIndex = 1;

            // 
            // btnProcessPayment
            // 
            this.btnProcessPayment.BackColor = this.SuccessGreen;
            this.btnProcessPayment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnProcessPayment.FlatAppearance.BorderSize = 0;
            this.btnProcessPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcessPayment.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnProcessPayment.ForeColor = System.Drawing.Color.White;
            this.btnProcessPayment.Location = new System.Drawing.Point(10, 10);
            this.btnProcessPayment.Name = "btnProcessPayment";
            this.btnProcessPayment.Size = new System.Drawing.Size(380, 154);
            this.btnProcessPayment.TabIndex = 0;
            this.btnProcessPayment.Text = "PROCESS PAYMENT";
            this.btnProcessPayment.UseVisualStyleBackColor = false;

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
            // BillingForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BillingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "POS System - Billing";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            this.mainContainer.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.contentPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCart)).EndInit();
            this.barcodePanel.ResumeLayout(false);
            this.barcodePanel.PerformLayout();
            this.footerPanel.ResumeLayout(false);
            this.summaryPanel.ResumeLayout(false);
            this.paymentPanel.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}