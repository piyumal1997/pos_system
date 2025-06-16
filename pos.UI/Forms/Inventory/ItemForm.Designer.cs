namespace pos_system.pos.UI.Forms
{
    partial class ItemForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtBarcode;
        private TextBox txtDescription;
        private TextBox txtRetailPrice;
        private TextBox txtUnitCost;
        private TextBox txtQuantity;
        private TextBox txtMaxDiscount;
        private TextBox txtMinStock;
        private ComboBox cmbBrand;
        private ComboBox cmbCategory;
        private ComboBox cmbSize;
        private PictureBox picItemImage;
        private Button btnSave;
        private Button btnGenerate;
        private Button btnBrowse;
        private Button btnCancel;
        private Button btnClose;
        private Button btnMinimize;
        private Label lblTitle;
        private Panel mainPanel;
        private Panel topPanel;
        private Panel container;
        private TableLayoutPanel tableLayout;
        private FlowLayoutPanel buttonPanel;
        private FlowLayoutPanel flowLayout;
        private FlowLayoutPanel flpBarcode;
        private FlowLayoutPanel flpImage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.mainPanel = new System.Windows.Forms.Panel();
            this.container = new System.Windows.Forms.Panel();
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.flpBarcode = new System.Windows.Forms.FlowLayoutPanel();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.txtRetailPrice = new System.Windows.Forms.TextBox();
            this.txtUnitCost = new System.Windows.Forms.TextBox();
            this.cmbBrand = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.cmbSize = new System.Windows.Forms.ComboBox();
            this.txtMaxDiscount = new System.Windows.Forms.TextBox();
            this.txtMinStock = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.flpImage = new System.Windows.Forms.FlowLayoutPanel();
            this.picItemImage = new System.Windows.Forms.PictureBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.topPanel = new System.Windows.Forms.Panel();
            this.flowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.container.SuspendLayout();
            this.tableLayout.SuspendLayout();
            this.flpBarcode.SuspendLayout();
            this.flpImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picItemImage)).BeginInit();
            this.buttonPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.flowLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.Controls.Add(this.container);
            this.mainPanel.Controls.Add(this.topPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(500, 690);
            this.mainPanel.TabIndex = 0;
            // 
            // container
            // 
            this.container.BackColor = System.Drawing.Color.White;
            this.container.Controls.Add(this.tableLayout);
            this.container.Controls.Add(this.buttonPanel);
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(0, 40);
            this.container.Name = "container";
            this.container.Padding = new System.Windows.Forms.Padding(20, 10, 20, 20);
            this.container.Size = new System.Drawing.Size(500, 650);
            this.container.TabIndex = 1;
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 2;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Barcode:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 0);
            this.tableLayout.Controls.Add(this.flpBarcode, 1, 0);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Quantity:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 1);
            this.tableLayout.Controls.Add(this.txtQuantity, 1, 1);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Retail Price:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 2);
            this.tableLayout.Controls.Add(this.txtRetailPrice, 1, 2);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Unit Cost:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 3);
            this.tableLayout.Controls.Add(this.txtUnitCost, 1, 3);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Brand:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 4);
            this.tableLayout.Controls.Add(this.cmbBrand, 1, 4);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Category:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 5);
            this.tableLayout.Controls.Add(this.cmbCategory, 1, 5);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Size:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 6);
            this.tableLayout.Controls.Add(this.cmbSize, 1, 6);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Max Discount (%):", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 7);
            this.tableLayout.Controls.Add(this.txtMaxDiscount, 1, 7);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Min Stock Level:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 8);
            this.tableLayout.Controls.Add(this.txtMinStock, 1, 8);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Description:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 9);
            this.tableLayout.Controls.Add(this.txtDescription, 1, 9);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Image:", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 10);
            this.tableLayout.Controls.Add(this.flpImage, 1, 10);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(20, 10);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.Padding = new System.Windows.Forms.Padding(10);
            this.tableLayout.RowCount = 11;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayout.Size = new System.Drawing.Size(460, 560);
            this.tableLayout.TabIndex = 0;
            // 
            // flpBarcode
            // 
            this.flpBarcode.AutoSize = true;
            this.flpBarcode.Controls.Add(this.txtBarcode);
            this.flpBarcode.Controls.Add(this.btnGenerate);
            this.flpBarcode.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flpBarcode.Location = new System.Drawing.Point(143, 13);
            this.flpBarcode.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flpBarcode.Name = "flpBarcode";
            this.flpBarcode.Size = new System.Drawing.Size(285, 25);
            this.flpBarcode.TabIndex = 0;
            // 
            // txtBarcode
            // 
            this.txtBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBarcode.Location = new System.Drawing.Point(0, 0);
            this.txtBarcode.Margin = new System.Windows.Forms.Padding(0);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(200, 25);
            this.txtBarcode.TabIndex = 0;
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnGenerate.FlatAppearance.BorderSize = 0;
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(205, 0);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(80, 25);
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = false;
            // 
            // txtQuantity
            // 
            this.txtQuantity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtQuantity.Location = new System.Drawing.Point(143, 43);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(250, 25);
            this.txtQuantity.TabIndex = 1;
            // 
            // txtRetailPrice
            // 
            this.txtRetailPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRetailPrice.Location = new System.Drawing.Point(143, 73);
            this.txtRetailPrice.Name = "txtRetailPrice";
            this.txtRetailPrice.Size = new System.Drawing.Size(250, 25);
            this.txtRetailPrice.TabIndex = 2;
            // 
            // txtUnitCost
            // 
            this.txtUnitCost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUnitCost.Location = new System.Drawing.Point(143, 103);
            this.txtUnitCost.Name = "txtUnitCost";
            this.txtUnitCost.Size = new System.Drawing.Size(250, 25);
            this.txtUnitCost.TabIndex = 3;
            // 
            // cmbBrand
            // 
            this.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBrand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBrand.FormattingEnabled = true;
            this.cmbBrand.Location = new System.Drawing.Point(143, 133);
            this.cmbBrand.Name = "cmbBrand";
            this.cmbBrand.Size = new System.Drawing.Size(250, 25);
            this.cmbBrand.TabIndex = 4;
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(143, 163);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(250, 25);
            this.cmbCategory.TabIndex = 5;
            // 
            // cmbSize
            // 
            this.cmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Location = new System.Drawing.Point(143, 193);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(250, 25);
            this.cmbSize.TabIndex = 6;
            // 
            // txtMaxDiscount
            // 
            this.txtMaxDiscount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaxDiscount.Location = new System.Drawing.Point(143, 223);
            this.txtMaxDiscount.Name = "txtMaxDiscount";
            this.txtMaxDiscount.Size = new System.Drawing.Size(250, 25);
            this.txtMaxDiscount.TabIndex = 7;
            // 
            // txtMinStock
            // 
            this.txtMinStock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMinStock.Location = new System.Drawing.Point(143, 253);
            this.txtMinStock.Name = "txtMinStock";
            this.txtMinStock.Size = new System.Drawing.Size(250, 25);
            this.txtMinStock.TabIndex = 8;
            // 
            // txtDescription
            // 
            this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescription.Location = new System.Drawing.Point(143, 283);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(250, 94);
            this.txtDescription.TabIndex = 9;
            // 
            // flpImage
            // 
            this.flpImage.AutoSize = true;
            this.flpImage.Controls.Add(this.picItemImage);
            this.flpImage.Controls.Add(this.btnBrowse);
            this.flpImage.Location = new System.Drawing.Point(143, 383);
            this.flpImage.Name = "flpImage";
            this.flpImage.Size = new System.Drawing.Size(245, 150);
            this.flpImage.TabIndex = 10;
            // 
            // picItemImage
            // 
            this.picItemImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picItemImage.Location = new System.Drawing.Point(0, 0);
            this.picItemImage.Margin = new System.Windows.Forms.Padding(0);
            this.picItemImage.Name = "picItemImage";
            this.picItemImage.Size = new System.Drawing.Size(150, 150);
            this.picItemImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picItemImage.TabIndex = 0;
            this.picItemImage.TabStop = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(155, 50);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(5, 50, 0, 0);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(90, 30);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = false;
            // 
            // buttonPanel
            // 
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.Controls.Add(this.btnCancel);
            this.buttonPanel.Controls.Add(this.btnSave);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonPanel.Location = new System.Drawing.Point(20, 570);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.buttonPanel.Size = new System.Drawing.Size(460, 60);
            this.buttonPanel.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(360, 23);
            this.btnCancel.MinimumSize = new System.Drawing.Size(100, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(254, 23);
            this.btnSave.MinimumSize = new System.Drawing.Size(100, 40);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 40);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.topPanel.Controls.Add(this.flowLayout);
            this.topPanel.Controls.Add(this.lblTitle);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.topPanel.Size = new System.Drawing.Size(500, 40);
            this.topPanel.TabIndex = 0;
            // 
            // flowLayout
            // 
            this.flowLayout.AutoSize = true;
            this.flowLayout.Controls.Add(this.btnClose);
            this.flowLayout.Controls.Add(this.btnMinimize);
            this.flowLayout.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayout.Location = new System.Drawing.Point(355, 0);
            this.flowLayout.Name = "flowLayout";
            this.flowLayout.Size = new System.Drawing.Size(135, 40);
            this.flowLayout.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(100, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(35, 35);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "✕";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnMinimize
            // 
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnMinimize.ForeColor = System.Drawing.Color.White;
            this.btnMinimize.Location = new System.Drawing.Point(59, 3);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(35, 35);
            this.btnMinimize.TabIndex = 1;
            this.btnMinimize.Text = "-";
            this.btnMinimize.UseVisualStyleBackColor = true;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(10, 10, 0, 0);
            this.lblTitle.Size = new System.Drawing.Size(80, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Add Item";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ItemForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 690);
            this.Controls.Add(this.mainPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ItemForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Item";
            this.mainPanel.ResumeLayout(false);
            this.container.ResumeLayout(false);
            this.container.PerformLayout();
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            this.flpBarcode.ResumeLayout(false);
            this.flpBarcode.PerformLayout();
            this.flpImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picItemImage)).EndInit();
            this.buttonPanel.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.flowLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}