namespace pos_system.pos.UI.Forms
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (_dateTimer != null)
                {
                    _dateTimer.Stop();
                    _dateTimer.Dispose();
                }

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            container = new TableLayoutPanel();
            headerPanel = new Panel();
            tableLayoutHeader = new TableLayoutPanel();
            btnRefreshBill = new Button();
            lblDateTime = new Label();
            billIdPanel = new Panel();
            lblBillId = new Label();
            barcodePanel = new Panel();
            barcodeContainer = new TableLayoutPanel();
            txtBarcode = new TextBox();
            btnClearBill = new Button();
            btnApplyBillDiscount = new Button();
            btnClearDiscounts = new Button();
            btnAddItem = new Button();
            txtTokenId = new TextBox();
            btnApplyToken = new Button();
            summaryPanel = new Panel();
            summaryLayout = new TableLayoutPanel();
            itemsTotalPanel = new Panel();
            itemsTotalLayout = new TableLayoutPanel();
            itemsLabel = new Label();
            lblItemCount = new Label();
            subtotalLabel = new Label();
            lblSubtotal = new Label();
            discountLabel = new Label();
            lblTotalDiscount = new Label();
            lblBillDiscount = new Label();
            totalLabel = new Label();
            lblTotal = new Label();
            paymentPanel = new Panel();
            paymentLayout = new TableLayoutPanel();
            btnProcessPayment = new Button();
            spacer = new Panel();
            container.SuspendLayout();
            headerPanel.SuspendLayout();
            tableLayoutHeader.SuspendLayout();
            billIdPanel.SuspendLayout();
            barcodePanel.SuspendLayout();
            barcodeContainer.SuspendLayout();
            summaryPanel.SuspendLayout();
            summaryLayout.SuspendLayout();
            itemsTotalPanel.SuspendLayout();
            itemsTotalLayout.SuspendLayout();
            paymentPanel.SuspendLayout();
            paymentLayout.SuspendLayout();
            SuspendLayout();
            // 
            // container
            // 
            container.ColumnCount = 1;
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            container.Controls.Add(headerPanel, 0, 0);
            container.Controls.Add(billIdPanel, 0, 1);
            container.Controls.Add(barcodePanel, 0, 2);
            container.Controls.Add(summaryPanel, 0, 4);
            container.Dock = DockStyle.Fill;
            container.Location = new Point(10, 10);
            container.Name = "container";
            container.RowCount = 5;
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 125F));
            container.Size = new Size(1046, 325);
            container.TabIndex = 0;
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(41, 128, 185);
            headerPanel.Controls.Add(tableLayoutHeader);
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Location = new Point(3, 3);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(1040, 34);
            headerPanel.TabIndex = 0;
            // 
            // tableLayoutHeader
            // 
            tableLayoutHeader.ColumnCount = 2;
            tableLayoutHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutHeader.ColumnStyles.Add(new ColumnStyle());
            tableLayoutHeader.Controls.Add(btnRefreshBill, 1, 0);
            tableLayoutHeader.Controls.Add(lblDateTime, 0, 0);
            tableLayoutHeader.Dock = DockStyle.Fill;
            tableLayoutHeader.Location = new Point(0, 0);
            tableLayoutHeader.Name = "tableLayoutHeader";
            tableLayoutHeader.RowCount = 1;
            tableLayoutHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutHeader.Size = new Size(1040, 34);
            tableLayoutHeader.TabIndex = 0;
            // 
            // btnRefreshBill
            // 
            btnRefreshBill.Anchor = AnchorStyles.Right;
            btnRefreshBill.BackColor = Color.SteelBlue;
            btnRefreshBill.FlatAppearance.BorderSize = 0;
            btnRefreshBill.FlatStyle = FlatStyle.Flat;
            btnRefreshBill.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRefreshBill.ForeColor = Color.White;
            btnRefreshBill.Location = new Point(955, 1);
            btnRefreshBill.Margin = new Padding(0, 0, 10, 0);
            btnRefreshBill.Name = "btnRefreshBill";
            btnRefreshBill.Size = new Size(75, 32);
            btnRefreshBill.TabIndex = 2;
            btnRefreshBill.Text = "Refresh";
            btnRefreshBill.UseVisualStyleBackColor = false;
            // 
            // lblDateTime
            // 
            lblDateTime.Anchor = AnchorStyles.Left;
            lblDateTime.AutoSize = true;
            lblDateTime.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDateTime.ForeColor = Color.White;
            lblDateTime.Location = new Point(5, 4);
            lblDateTime.Margin = new Padding(5, 0, 0, 0);
            lblDateTime.Name = "lblDateTime";
            lblDateTime.Size = new Size(170, 25);
            lblDateTime.TabIndex = 0;
            lblDateTime.Text = "2023-01-01 00:00";
            lblDateTime.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // billIdPanel
            // 
            billIdPanel.BackColor = Color.FromArgb(189, 224, 254);
            billIdPanel.Controls.Add(lblBillId);
            billIdPanel.Dock = DockStyle.Fill;
            billIdPanel.Location = new Point(3, 43);
            billIdPanel.Name = "billIdPanel";
            billIdPanel.Size = new Size(1040, 34);
            billIdPanel.TabIndex = 1;
            // 
            // lblBillId
            // 
            lblBillId.Dock = DockStyle.Fill;
            lblBillId.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblBillId.Location = new Point(0, 0);
            lblBillId.Name = "lblBillId";
            lblBillId.Size = new Size(1040, 34);
            lblBillId.TabIndex = 0;
            lblBillId.Text = "New Bill";
            lblBillId.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // barcodePanel
            // 
            barcodePanel.Controls.Add(barcodeContainer);
            barcodePanel.Dock = DockStyle.Fill;
            barcodePanel.Location = new Point(3, 83);
            barcodePanel.Name = "barcodePanel";
            barcodePanel.Size = new Size(1040, 94);
            barcodePanel.TabIndex = 2;
            // 
            // barcodeContainer
            // 
            barcodeContainer.ColumnCount = 4;
            barcodeContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            barcodeContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            barcodeContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            barcodeContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            barcodeContainer.Controls.Add(txtBarcode, 0, 0);
            barcodeContainer.Controls.Add(btnClearBill, 1, 0);
            barcodeContainer.Controls.Add(btnApplyBillDiscount, 2, 0);
            barcodeContainer.Controls.Add(btnClearDiscounts, 3, 0);
            barcodeContainer.Controls.Add(btnAddItem, 0, 1);
            barcodeContainer.Controls.Add(txtTokenId, 1, 1);
            barcodeContainer.Controls.Add(btnApplyToken, 3, 1);
            barcodeContainer.Dock = DockStyle.Fill;
            barcodeContainer.Location = new Point(0, 0);
            barcodeContainer.Name = "barcodeContainer";
            barcodeContainer.RowCount = 2;
            barcodeContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            barcodeContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            barcodeContainer.Size = new Size(1040, 94);
            barcodeContainer.TabIndex = 0;
            // 
            // txtBarcode
            // 
            txtBarcode.Dock = DockStyle.Fill;
            txtBarcode.Font = new Font("Segoe UI", 16F);
            txtBarcode.Location = new Point(3, 3);
            txtBarcode.Name = "txtBarcode";
            txtBarcode.PlaceholderText = "Scan Barcode or Enter";
            txtBarcode.Size = new Size(466, 36);
            txtBarcode.TabIndex = 0;
            // 
            // btnClearBill
            // 
            btnClearBill.BackColor = Color.IndianRed;
            btnClearBill.Dock = DockStyle.Fill;
            btnClearBill.FlatAppearance.BorderSize = 0;
            btnClearBill.FlatStyle = FlatStyle.Flat;
            btnClearBill.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnClearBill.ForeColor = Color.White;
            btnClearBill.Location = new Point(475, 3);
            btnClearBill.Name = "btnClearBill";
            btnClearBill.Size = new Size(183, 41);
            btnClearBill.TabIndex = 2;
            btnClearBill.Text = "Clear Bill";
            btnClearBill.UseVisualStyleBackColor = false;
            // 
            // btnApplyBillDiscount
            // 
            btnApplyBillDiscount.BackColor = Color.Goldenrod;
            btnApplyBillDiscount.Dock = DockStyle.Fill;
            btnApplyBillDiscount.FlatAppearance.BorderSize = 0;
            btnApplyBillDiscount.FlatStyle = FlatStyle.Flat;
            btnApplyBillDiscount.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnApplyBillDiscount.ForeColor = Color.Black;
            btnApplyBillDiscount.Location = new Point(664, 3);
            btnApplyBillDiscount.Name = "btnApplyBillDiscount";
            btnApplyBillDiscount.Size = new Size(183, 41);
            btnApplyBillDiscount.TabIndex = 3;
            btnApplyBillDiscount.Text = "Apply Bill Disc";
            btnApplyBillDiscount.UseVisualStyleBackColor = false;
            // 
            // btnClearDiscounts
            // 
            btnClearDiscounts.BackColor = Color.Orange;
            btnClearDiscounts.Dock = DockStyle.Fill;
            btnClearDiscounts.FlatAppearance.BorderSize = 0;
            btnClearDiscounts.FlatStyle = FlatStyle.Flat;
            btnClearDiscounts.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnClearDiscounts.ForeColor = Color.Black;
            btnClearDiscounts.Location = new Point(853, 3);
            btnClearDiscounts.Name = "btnClearDiscounts";
            btnClearDiscounts.Size = new Size(184, 41);
            btnClearDiscounts.TabIndex = 4;
            btnClearDiscounts.Text = "Clear Disc";
            btnClearDiscounts.UseVisualStyleBackColor = false;
            // 
            // btnAddItem
            // 
            btnAddItem.BackColor = Color.SteelBlue;
            btnAddItem.Dock = DockStyle.Fill;
            btnAddItem.FlatAppearance.BorderSize = 0;
            btnAddItem.FlatStyle = FlatStyle.Flat;
            btnAddItem.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddItem.ForeColor = Color.White;
            btnAddItem.Location = new Point(3, 50);
            btnAddItem.Name = "btnAddItem";
            btnAddItem.Size = new Size(466, 41);
            btnAddItem.TabIndex = 1;
            btnAddItem.Text = "Add Item";
            btnAddItem.UseVisualStyleBackColor = false;
            // 
            // txtTokenId
            // 
            barcodeContainer.SetColumnSpan(txtTokenId, 2);
            txtTokenId.Dock = DockStyle.Fill;
            txtTokenId.Font = new Font("Segoe UI", 16F);
            txtTokenId.Location = new Point(475, 50);
            txtTokenId.Name = "txtTokenId";
            txtTokenId.PlaceholderText = "Enter Token";
            txtTokenId.Size = new Size(372, 36);
            txtTokenId.TabIndex = 5;
            // 
            // btnApplyToken
            // 
            btnApplyToken.BackColor = Color.MediumPurple;
            btnApplyToken.Dock = DockStyle.Fill;
            btnApplyToken.FlatAppearance.BorderSize = 0;
            btnApplyToken.FlatStyle = FlatStyle.Flat;
            btnApplyToken.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnApplyToken.ForeColor = Color.White;
            btnApplyToken.Location = new Point(853, 50);
            btnApplyToken.Name = "btnApplyToken";
            btnApplyToken.Size = new Size(184, 41);
            btnApplyToken.TabIndex = 6;
            btnApplyToken.Text = "Apply Token";
            btnApplyToken.UseVisualStyleBackColor = false;
            // 
            // summaryPanel
            // 
            summaryPanel.Controls.Add(summaryLayout);
            summaryPanel.Dock = DockStyle.Fill;
            summaryPanel.Location = new Point(3, 203);
            summaryPanel.Name = "summaryPanel";
            summaryPanel.Size = new Size(1040, 119);
            summaryPanel.TabIndex = 3;
            // 
            // summaryLayout
            // 
            summaryLayout.ColumnCount = 2;
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.76923F));
            summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.23077F));
            summaryLayout.Controls.Add(itemsTotalPanel, 0, 0);
            summaryLayout.Controls.Add(paymentPanel, 1, 0);
            summaryLayout.Dock = DockStyle.Fill;
            summaryLayout.Location = new Point(0, 0);
            summaryLayout.Name = "summaryLayout";
            summaryLayout.RowCount = 1;
            summaryLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            summaryLayout.Size = new Size(1040, 119);
            summaryLayout.TabIndex = 0;
            // 
            // itemsTotalPanel
            // 
            itemsTotalPanel.Controls.Add(itemsTotalLayout);
            itemsTotalPanel.Dock = DockStyle.Fill;
            itemsTotalPanel.Location = new Point(3, 3);
            itemsTotalPanel.Name = "itemsTotalPanel";
            itemsTotalPanel.Size = new Size(678, 113);
            itemsTotalPanel.TabIndex = 0;
            // 
            // itemsTotalLayout
            // 
            itemsTotalLayout.ColumnCount = 2;
            itemsTotalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            itemsTotalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            itemsTotalLayout.Controls.Add(itemsLabel, 0, 0);
            itemsTotalLayout.Controls.Add(lblItemCount, 1, 0);
            itemsTotalLayout.Controls.Add(subtotalLabel, 0, 1);
            itemsTotalLayout.Controls.Add(lblSubtotal, 1, 1);
            itemsTotalLayout.Controls.Add(discountLabel, 0, 2);
            itemsTotalLayout.Controls.Add(lblTotalDiscount, 1, 2);
            itemsTotalLayout.Controls.Add(lblBillDiscount, 1, 3);
            itemsTotalLayout.Controls.Add(totalLabel, 0, 4);
            itemsTotalLayout.Controls.Add(lblTotal, 1, 4);
            itemsTotalLayout.Dock = DockStyle.Fill;
            itemsTotalLayout.Location = new Point(0, 0);
            itemsTotalLayout.Name = "itemsTotalLayout";
            itemsTotalLayout.RowCount = 5;
            itemsTotalLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            itemsTotalLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            itemsTotalLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            itemsTotalLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            itemsTotalLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            itemsTotalLayout.Size = new Size(678, 113);
            itemsTotalLayout.TabIndex = 0;
            // 
            // itemsLabel
            // 
            itemsLabel.Dock = DockStyle.Fill;
            itemsLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            itemsLabel.Location = new Point(3, 0);
            itemsLabel.Name = "itemsLabel";
            itemsLabel.Size = new Size(333, 22);
            itemsLabel.TabIndex = 0;
            itemsLabel.Text = "Items:";
            itemsLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblItemCount
            // 
            lblItemCount.Dock = DockStyle.Fill;
            lblItemCount.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblItemCount.Location = new Point(342, 0);
            lblItemCount.Name = "lblItemCount";
            lblItemCount.Size = new Size(333, 22);
            lblItemCount.TabIndex = 1;
            lblItemCount.Text = "0";
            lblItemCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // subtotalLabel
            // 
            subtotalLabel.Dock = DockStyle.Fill;
            subtotalLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            subtotalLabel.Location = new Point(3, 22);
            subtotalLabel.Name = "subtotalLabel";
            subtotalLabel.Size = new Size(333, 22);
            subtotalLabel.TabIndex = 8;
            subtotalLabel.Text = "Subtotal:";
            subtotalLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblSubtotal
            // 
            lblSubtotal.Dock = DockStyle.Fill;
            lblSubtotal.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSubtotal.Location = new Point(342, 22);
            lblSubtotal.Name = "lblSubtotal";
            lblSubtotal.Size = new Size(333, 22);
            lblSubtotal.TabIndex = 9;
            lblSubtotal.Text = "$0.00";
            lblSubtotal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // discountLabel
            // 
            discountLabel.Dock = DockStyle.Fill;
            discountLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            discountLabel.Location = new Point(3, 44);
            discountLabel.Name = "discountLabel";
            discountLabel.Size = new Size(333, 22);
            discountLabel.TabIndex = 6;
            discountLabel.Text = "Item Discounts:";
            discountLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblTotalDiscount
            // 
            lblTotalDiscount.Dock = DockStyle.Fill;
            lblTotalDiscount.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalDiscount.ForeColor = Color.Red;
            lblTotalDiscount.Location = new Point(342, 44);
            lblTotalDiscount.Name = "lblTotalDiscount";
            lblTotalDiscount.Size = new Size(333, 22);
            lblTotalDiscount.TabIndex = 7;
            lblTotalDiscount.Text = "$0.00";
            lblTotalDiscount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblBillDiscount
            // 
            lblBillDiscount.Dock = DockStyle.Fill;
            lblBillDiscount.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBillDiscount.ForeColor = Color.Red;
            lblBillDiscount.Location = new Point(342, 66);
            lblBillDiscount.Name = "lblBillDiscount";
            lblBillDiscount.Size = new Size(333, 22);
            lblBillDiscount.TabIndex = 16;
            lblBillDiscount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // totalLabel
            // 
            totalLabel.Dock = DockStyle.Fill;
            totalLabel.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            totalLabel.Location = new Point(3, 88);
            totalLabel.Name = "totalLabel";
            totalLabel.Size = new Size(333, 25);
            totalLabel.TabIndex = 15;
            totalLabel.Text = "Total:";
            totalLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblTotal
            // 
            lblTotal.Dock = DockStyle.Fill;
            lblTotal.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.Location = new Point(342, 88);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(333, 25);
            lblTotal.TabIndex = 14;
            lblTotal.Text = "$0.00";
            lblTotal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // paymentPanel
            // 
            paymentPanel.Controls.Add(paymentLayout);
            paymentPanel.Dock = DockStyle.Fill;
            paymentPanel.Location = new Point(687, 3);
            paymentPanel.Name = "paymentPanel";
            paymentPanel.Size = new Size(350, 113);
            paymentPanel.TabIndex = 1;
            // 
            // paymentLayout
            // 
            paymentLayout.ColumnCount = 1;
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            paymentLayout.Controls.Add(btnProcessPayment, 0, 0);
            paymentLayout.Dock = DockStyle.Fill;
            paymentLayout.Location = new Point(0, 0);
            paymentLayout.Name = "paymentLayout";
            paymentLayout.RowCount = 1;
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            paymentLayout.Size = new Size(350, 113);
            paymentLayout.TabIndex = 0;
            // 
            // btnProcessPayment
            // 
            btnProcessPayment.BackColor = Color.SeaGreen;
            btnProcessPayment.Dock = DockStyle.Fill;
            btnProcessPayment.FlatAppearance.BorderSize = 0;
            btnProcessPayment.FlatStyle = FlatStyle.Flat;
            btnProcessPayment.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            btnProcessPayment.ForeColor = Color.White;
            btnProcessPayment.Location = new Point(10, 10);
            btnProcessPayment.Margin = new Padding(10);
            btnProcessPayment.Name = "btnProcessPayment";
            btnProcessPayment.Size = new Size(330, 93);
            btnProcessPayment.TabIndex = 1;
            btnProcessPayment.Text = "PROCESS PAYMENT";
            btnProcessPayment.UseVisualStyleBackColor = false;
            // 
            // spacer
            // 
            spacer.AutoSize = true;
            spacer.Dock = DockStyle.Fill;
            spacer.Location = new Point(170, 0);
            spacer.Margin = new Padding(0);
            spacer.Name = "spacer";
            spacer.Size = new Size(0, 35);
            spacer.TabIndex = 1;
            // 
            // BillingForm
            // 
            BackColor = Color.White;
            ClientSize = new Size(1066, 345);
            Controls.Add(container);
            Name = "BillingForm";
            Padding = new Padding(10);
            Text = "Billing";
            container.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            tableLayoutHeader.ResumeLayout(false);
            tableLayoutHeader.PerformLayout();
            billIdPanel.ResumeLayout(false);
            barcodePanel.ResumeLayout(false);
            barcodeContainer.ResumeLayout(false);
            barcodeContainer.PerformLayout();
            summaryPanel.ResumeLayout(false);
            summaryLayout.ResumeLayout(false);
            itemsTotalPanel.ResumeLayout(false);
            itemsTotalLayout.ResumeLayout(false);
            paymentPanel.ResumeLayout(false);
            paymentLayout.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}