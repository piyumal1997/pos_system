namespace pos_system.pos.UI.Forms.Inventory
{
    partial class ItemForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainPanel = new System.Windows.Forms.Panel();
            this.container = new System.Windows.Forms.Panel();
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.groupSizes = new System.Windows.Forms.GroupBox();
            this.tableLayoutSizes = new System.Windows.Forms.TableLayoutPanel();
            this.dgvSizes = new System.Windows.Forms.DataGridView();
            this.panelSizeButtons = new System.Windows.Forms.Panel();
            this.btnAddSize = new System.Windows.Forms.Button();
            this.btnRemoveSize = new System.Windows.Forms.Button();
            this.flpBarcode = new System.Windows.Forms.FlowLayoutPanel();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtMinStock = new System.Windows.Forms.TextBox();
            this.txtMaxDiscount = new System.Windows.Forms.TextBox();
            this.cmbBrand = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.flpImage = new System.Windows.Forms.FlowLayoutPanel();
            this.picItemImage = new System.Windows.Forms.PictureBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnClearImage = new System.Windows.Forms.Button();
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
            this.groupSizes.SuspendLayout();
            this.tableLayoutSizes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSizes)).BeginInit();
            this.panelSizeButtons.SuspendLayout();
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
            this.mainPanel.Size = new System.Drawing.Size(800, 600);
            this.mainPanel.TabIndex = 0;
            // 
            // container
            // 
            this.container.AutoScroll = true;
            this.container.BackColor = System.Drawing.Color.White;
            this.container.Controls.Add(this.tableLayout);
            this.container.Controls.Add(this.buttonPanel);
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(0, 40);
            this.container.Name = "container";
            this.container.Padding = new System.Windows.Forms.Padding(10, 10, 10, 20);
            this.container.Size = new System.Drawing.Size(800, 500);
            this.container.TabIndex = 1;
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 2;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayout.Controls.Add(this.groupSizes, 1, 0);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Barcode:", Anchor = System.Windows.Forms.AnchorStyles.Left, AutoSize = true }, 0, 0);
            this.tableLayout.Controls.Add(this.flpBarcode, 0, 1);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Max Discount Value:", Anchor = System.Windows.Forms.AnchorStyles.Left, AutoSize = true }, 0, 2);
            this.tableLayout.Controls.Add(this.txtMaxDiscount, 0, 3);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Min Stock Level:", Anchor = System.Windows.Forms.AnchorStyles.Left, AutoSize = true }, 0, 4);
            this.tableLayout.Controls.Add(this.txtMinStock, 0, 5);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Brand:", Anchor = System.Windows.Forms.AnchorStyles.Left, AutoSize = true }, 0, 6);
            this.tableLayout.Controls.Add(this.cmbBrand, 0, 7);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Category:", Anchor = System.Windows.Forms.AnchorStyles.Left, AutoSize = true }, 0, 8);
            this.tableLayout.Controls.Add(this.cmbCategory, 0, 9);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Gender:", Anchor = System.Windows.Forms.AnchorStyles.Left, AutoSize = true }, 0, 10);
            this.tableLayout.Controls.Add(this.cmbGender, 0, 11);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Description:", Anchor = System.Windows.Forms.AnchorStyles.Left, AutoSize = true }, 0, 12);
            this.tableLayout.Controls.Add(this.txtDescription, 0, 13);
            this.tableLayout.Controls.Add(new System.Windows.Forms.Label() { Text = "Image:", Anchor = System.Windows.Forms.AnchorStyles.Left, AutoSize = true }, 0, 14);
            this.tableLayout.Controls.Add(this.flpImage, 0, 15);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(20, 10);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.Padding = new System.Windows.Forms.Padding(10,0,10,10);
            this.tableLayout.RowCount = 16;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.tableLayout.SetRowSpan(this.groupSizes, 16);
            this.tableLayout.Size = new System.Drawing.Size(780, 490);
            this.tableLayout.TabIndex = 0;
            // 
            // groupSizes
            // 
            this.groupSizes.Controls.Add(this.tableLayoutSizes);
            this.groupSizes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupSizes.Location = new System.Drawing.Point(468, 13);
            this.groupSizes.Name = "groupSizes";
            this.groupSizes.Padding = new System.Windows.Forms.Padding(10);
            this.groupSizes.Size = new System.Drawing.Size(280, 400);
            this.groupSizes.TabIndex = 8;
            this.groupSizes.TabStop = false;
            this.groupSizes.Text = "Size Variants";
            // 
            // tableLayoutSizes
            // 
            this.tableLayoutSizes.ColumnCount = 1;
            this.tableLayoutSizes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutSizes.Controls.Add(this.dgvSizes, 0, 0);
            this.tableLayoutSizes.Controls.Add(this.panelSizeButtons, 0, 1);
            this.tableLayoutSizes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutSizes.Location = new System.Drawing.Point(10, 28);
            this.tableLayoutSizes.Name = "tableLayoutSizes";
            this.tableLayoutSizes.RowCount = 2;
            this.tableLayoutSizes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutSizes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutSizes.Size = new System.Drawing.Size(259, 420);
            this.tableLayoutSizes.TabIndex = 3;
            // 
            // dgvSizes
            // 
            this.dgvSizes.AllowUserToAddRows = false;
            this.dgvSizes.AllowUserToDeleteRows = true;
            this.dgvSizes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSizes.BackgroundColor = System.Drawing.Color.White;
            this.dgvSizes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSizes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSizes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSizes.Location = new System.Drawing.Point(3, 3);
            this.dgvSizes.Name = "dgvSizes";
            this.dgvSizes.RowHeadersVisible = false;
            this.dgvSizes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSizes.Size = new System.Drawing.Size(260, 390);
            this.dgvSizes.TabIndex = 0;
            // 
            // panelSizeButtons
            // 
            this.panelSizeButtons.Controls.Add(this.btnAddSize);
            this.panelSizeButtons.Controls.Add(this.btnRemoveSize);
            this.panelSizeButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSizeButtons.Location = new System.Drawing.Point(0, 396);
            this.panelSizeButtons.Margin = new System.Windows.Forms.Padding(0);
            this.panelSizeButtons.Name = "panelSizeButtons";
            this.panelSizeButtons.Size = new System.Drawing.Size(259, 40);
            this.panelSizeButtons.TabIndex = 1;
            // 
            // btnAddSize
            // 
            this.btnAddSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnAddSize.FlatAppearance.BorderSize = 0;
            this.btnAddSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddSize.ForeColor = System.Drawing.Color.White;
            this.btnAddSize.Location = new System.Drawing.Point(79, 5);
            this.btnAddSize.Name = "btnAddSize";
            this.btnAddSize.Size = new System.Drawing.Size(80, 30);
            this.btnAddSize.TabIndex = 1;
            this.btnAddSize.Text = "Add Size";
            this.btnAddSize.UseVisualStyleBackColor = false;
            // 
            // btnRemoveSize
            // 
            this.btnRemoveSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnRemoveSize.FlatAppearance.BorderSize = 0;
            this.btnRemoveSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRemoveSize.ForeColor = System.Drawing.Color.White;
            this.btnRemoveSize.Location = new System.Drawing.Point(165, 5);
            this.btnRemoveSize.Name = "btnRemoveSize";
            this.btnRemoveSize.Size = new System.Drawing.Size(90, 30);
            this.btnRemoveSize.TabIndex = 2;
            this.btnRemoveSize.Text = "Remove";
            this.btnRemoveSize.UseVisualStyleBackColor = false;
            // 
            // flpBarcode
            // 
            this.flpBarcode.AutoSize = true;
            this.flpBarcode.Controls.Add(this.txtBarcode);
            this.flpBarcode.Controls.Add(this.btnGenerate);
            this.flpBarcode.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpBarcode.Location = new System.Drawing.Point(13, 43);
            this.flpBarcode.Name = "flpBarcode";
            this.flpBarcode.Size = new System.Drawing.Size(440, 25);
            this.flpBarcode.TabIndex = 0;
            // 
            // btnClearImage
            // 
            this.btnClearImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnClearImage.FlatAppearance.BorderSize = 0;
            this.btnClearImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearImage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClearImage.ForeColor = System.Drawing.Color.White;
            this.btnClearImage.Location = new System.Drawing.Point(220, 45);
            this.btnClearImage.Margin = new System.Windows.Forms.Padding(0, 45, 0, 0);
            this.btnClearImage.Name = "btnClearImage";
            this.btnClearImage.Size = new System.Drawing.Size(90, 30);
            this.btnClearImage.TabIndex = 2;
            this.btnClearImage.Text = "Clear";
            this.btnClearImage.UseVisualStyleBackColor = false;
            this.btnClearImage.Click += new System.EventHandler(this.BtnClearImage_Click);
            // 
            // txtBarcode
            // 
            this.txtBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBarcode.Location = new System.Drawing.Point(0, 0);
            this.txtBarcode.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(300, 25);
            this.txtBarcode.TabIndex = 0;
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnGenerate.FlatAppearance.BorderSize = 0;
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Location = new System.Drawing.Point(310, 0);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(80, 25);
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = false;
            // 
            // txtMinStock
            // 
            this.txtMinStock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMinStock.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtMinStock.Location = new System.Drawing.Point(13, 163);
            this.txtMinStock.Name = "txtMinStock";
            this.txtMinStock.Size = new System.Drawing.Size(439, 25);
            this.txtMinStock.TabIndex = 2;
            // 
            // txtMaxDiscount
            // 
            this.txtMaxDiscount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaxDiscount.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtMaxDiscount.Location = new System.Drawing.Point(13, 103);
            this.txtMaxDiscount.Name = "txtMaxDiscount";
            this.txtMaxDiscount.Size = new System.Drawing.Size(439, 25);
            this.txtMaxDiscount.TabIndex = 1;
            // 
            // cmbBrand
            // 
            this.cmbBrand.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbBrand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBrand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBrand.FormattingEnabled = true;
            this.cmbBrand.Location = new System.Drawing.Point(13, 223);
            this.cmbBrand.Name = "cmbBrand";
            this.cmbBrand.Size = new System.Drawing.Size(439, 25);
            this.cmbBrand.TabIndex = 3;
            // 
            // cmbCategory
            // 
            this.cmbCategory.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(13, 283);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(439, 25);
            this.cmbCategory.TabIndex = 4;
            // 
            // cmbGender
            // 
            this.cmbGender.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Location = new System.Drawing.Point(13, 343);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(439, 25);
            this.cmbGender.TabIndex = 5;
            // 
            // txtDescription
            // 
            this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtDescription.Location = new System.Drawing.Point(13, 403);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(439, 64);
            this.txtDescription.TabIndex = 6;
            // 
            // flpImage
            // 
            this.flpImage.AutoSize = true;
            this.flpImage.Controls.Add(this.picItemImage);
            this.flpImage.Controls.Add(this.btnBrowse);
            this.flpImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpImage.Location = new System.Drawing.Point(13, 493);
            this.flpImage.Name = "flpImage";
            this.flpImage.Size = new System.Drawing.Size(439, 150);
            this.flpImage.TabIndex = 7;
            // 
            // picItemImage
            // 
            this.picItemImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picItemImage.Location = new System.Drawing.Point(0, 0);
            this.picItemImage.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.picItemImage.Name = "picItemImage";
            this.picItemImage.Size = new System.Drawing.Size(120, 120);
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
            this.btnBrowse.Location = new System.Drawing.Point(130, 45);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0, 45, 0, 0);
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
            this.buttonPanel.Location = new System.Drawing.Point(20, 510);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.buttonPanel.Size = new System.Drawing.Size(760, 80);
            this.buttonPanel.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(650, 13);
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
            this.btnSave.Location = new System.Drawing.Point(544, 13);
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
            this.topPanel.Size = new System.Drawing.Size(800, 40);
            this.topPanel.TabIndex = 0;
            // 
            // flowLayout
            // 
            this.flowLayout.AutoSize = true;
            this.flowLayout.Controls.Add(this.btnClose);
            this.flowLayout.Controls.Add(this.btnMinimize);
            this.flowLayout.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayout.Location = new System.Drawing.Point(655, 0);
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
            this.lblTitle.Size = new System.Drawing.Size(100, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Add Product";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ItemForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.mainPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ItemForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Product Form";
            this.mainPanel.ResumeLayout(false);
            this.container.ResumeLayout(false);
            this.container.PerformLayout();
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            this.groupSizes.ResumeLayout(false);
            this.tableLayoutSizes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSizes)).EndInit();
            this.panelSizeButtons.ResumeLayout(false);
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

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel container;
        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.FlowLayoutPanel flpBarcode;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtMinStock;
        private System.Windows.Forms.TextBox txtMaxDiscount;
        private System.Windows.Forms.ComboBox cmbBrand;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.ComboBox cmbGender;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.FlowLayoutPanel flpImage;
        private System.Windows.Forms.PictureBox picItemImage;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox groupSizes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutSizes;
        private System.Windows.Forms.DataGridView dgvSizes;
        private System.Windows.Forms.Panel panelSizeButtons;
        private System.Windows.Forms.Button btnAddSize;
        private System.Windows.Forms.Button btnRemoveSize;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayout;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClearImage;
        private System.Windows.Forms.Label lblTitle;
    }
}