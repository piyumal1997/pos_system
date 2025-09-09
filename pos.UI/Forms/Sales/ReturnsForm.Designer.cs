namespace pos_system.pos.UI.Forms.Sales
{
    partial class ReturnsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblBillId;
        private System.Windows.Forms.TextBox txtBillId;
        private System.Windows.Forms.Button btnSearchBill;
        private System.Windows.Forms.DataGridView dgvBillItems;
        private System.Windows.Forms.Button btnProcessReturn;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblBillDateLabel;
        private System.Windows.Forms.Label lblBillDate;
        private System.Windows.Forms.Label lblCurrentDateLabel;
        private System.Windows.Forms.Label lblCurrentDate;
        private System.Windows.Forms.Panel txtBillIdUnderline;

        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color LightBlue = Color.FromArgb(189, 224, 254);
        private readonly Color White = Color.White;
        private readonly Color DarkText = Color.FromArgb(64, 64, 64);
        private readonly Color SuccessGreen = Color.FromArgb(46, 204, 113);
        private readonly Color LightGray = Color.FromArgb(240, 240, 240);

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
            this.lblBillId = new System.Windows.Forms.Label();
            this.txtBillId = new System.Windows.Forms.TextBox();
            this.btnSearchBill = new System.Windows.Forms.Button();
            this.dgvBillItems = new System.Windows.Forms.DataGridView();
            this.btnProcessReturn = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblBillDateLabel = new System.Windows.Forms.Label();
            this.lblBillDate = new System.Windows.Forms.Label();
            this.lblCurrentDateLabel = new System.Windows.Forms.Label();
            this.lblCurrentDate = new System.Windows.Forms.Label();
            this.txtBillIdUnderline = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBillItems)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();

            // lblBillId
            this.lblBillId.AutoSize = true;
            this.lblBillId.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblBillId.ForeColor = this.DarkText;
            this.lblBillId.Location = new System.Drawing.Point(30, 80);
            this.lblBillId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBillId.Name = "lblBillId";
            this.lblBillId.Size = new System.Drawing.Size(58, 21);
            this.lblBillId.TabIndex = 0;
            this.lblBillId.Text = "Bill ID:";

            // txtBillId
            this.txtBillId.BackColor = this.White;
            this.txtBillId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBillId.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtBillId.ForeColor = this.DarkText;
            this.txtBillId.Location = new System.Drawing.Point(100, 80);
            this.txtBillId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBillId.Name = "txtBillId";
            this.txtBillId.Size = new System.Drawing.Size(250, 22);
            this.txtBillId.TabIndex = 1;
            this.txtBillId.Text = "Enter Bill ID";
            this.txtBillId.Enter += new System.EventHandler(this.txtBillId_Enter);
            this.txtBillId.Leave += new System.EventHandler(this.txtBillId_Leave);

            // txtBillIdUnderline
            this.txtBillIdUnderline.BackColor = System.Drawing.Color.Gray;
            this.txtBillIdUnderline.Location = new System.Drawing.Point(100, 105);
            this.txtBillIdUnderline.Name = "txtBillIdUnderline";
            this.txtBillIdUnderline.Size = new System.Drawing.Size(250, 1);
            this.txtBillIdUnderline.TabIndex = 12;

            // btnSearchBill
            this.btnSearchBill.BackColor = this.PrimaryBlue;
            this.btnSearchBill.FlatAppearance.BorderSize = 0;
            this.btnSearchBill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchBill.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearchBill.ForeColor = System.Drawing.Color.White;
            this.btnSearchBill.Location = new System.Drawing.Point(370, 75);
            this.btnSearchBill.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSearchBill.Name = "btnSearchBill";
            this.btnSearchBill.Size = new System.Drawing.Size(120, 35);
            this.btnSearchBill.TabIndex = 2;
            this.btnSearchBill.Text = "Search Bill";
            this.btnSearchBill.UseVisualStyleBackColor = false;
            this.btnSearchBill.Click += new System.EventHandler(this.btnSearchBill_Click);

            // dgvBillItems
            this.dgvBillItems.AllowUserToAddRows = false;
            this.dgvBillItems.AllowUserToDeleteRows = false;
            this.dgvBillItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBillItems.BackgroundColor = this.White;
            this.dgvBillItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvBillItems.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvBillItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBillItems.ColumnHeadersDefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle()
            {
                BackColor = this.LightBlue,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                ForeColor = this.DarkText
            };
            this.dgvBillItems.EnableHeadersVisualStyles = false;
            this.dgvBillItems.GridColor = this.LightGray;
            this.dgvBillItems.Location = new System.Drawing.Point(30, 150);
            this.dgvBillItems.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvBillItems.Name = "dgvBillItems";
            this.dgvBillItems.RowHeadersVisible = false;
            this.dgvBillItems.RowTemplate.Height = 40;
            this.dgvBillItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBillItems.Size = new System.Drawing.Size(940, 450);
            this.dgvBillItems.TabIndex = 3;
            this.dgvBillItems.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvBillItems_CellValidating);
            this.dgvBillItems.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBillItems_CellEndEdit);
            this.dgvBillItems.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvBillItems_DataError);

            // btnProcessReturn
            this.btnProcessReturn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProcessReturn.BackColor = this.SuccessGreen;
            this.btnProcessReturn.Enabled = false;
            this.btnProcessReturn.FlatAppearance.BorderSize = 0;
            this.btnProcessReturn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcessReturn.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.btnProcessReturn.ForeColor = System.Drawing.Color.White;
            this.btnProcessReturn.Location = new System.Drawing.Point(740, 590);
            this.btnProcessReturn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnProcessReturn.Name = "btnProcessReturn";
            this.btnProcessReturn.Size = new System.Drawing.Size(240, 65);
            this.btnProcessReturn.TabIndex = 4;
            this.btnProcessReturn.Text = "Process Return (F12)";
            this.btnProcessReturn.UseVisualStyleBackColor = false;
            this.btnProcessReturn.Click += new System.EventHandler(this.btnProcessReturn_Click);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(11, 9);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(196, 32);
            this.lblTitle.TabIndex = 6;
            this.lblTitle.Text = "Process Returns";

            // panel1
            this.panel1.BackColor = this.PrimaryBlue;
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(10, 10);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(980, 50);
            this.panel1.TabIndex = 7;

            // lblBillDateLabel
            this.lblBillDateLabel.AutoSize = true;
            this.lblBillDateLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblBillDateLabel.ForeColor = this.DarkText;
            this.lblBillDateLabel.Location = new System.Drawing.Point(510, 82);
            this.lblBillDateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBillDateLabel.Name = "lblBillDateLabel";
            this.lblBillDateLabel.Size = new System.Drawing.Size(72, 20);
            this.lblBillDateLabel.TabIndex = 8;
            this.lblBillDateLabel.Text = "Bill Date:";

            // lblBillDate
            this.lblBillDate.AutoSize = true;
            this.lblBillDate.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblBillDate.ForeColor = this.DarkText;
            this.lblBillDate.Location = new System.Drawing.Point(590, 82);
            this.lblBillDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBillDate.Name = "lblBillDate";
            this.lblBillDate.Size = new System.Drawing.Size(0, 20);
            this.lblBillDate.TabIndex = 9;

            // lblCurrentDateLabel
            this.lblCurrentDateLabel.AutoSize = true;
            this.lblCurrentDateLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblCurrentDateLabel.ForeColor = this.DarkText;
            this.lblCurrentDateLabel.Location = new System.Drawing.Point(680, 82);
            this.lblCurrentDateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentDateLabel.Name = "lblCurrentDateLabel";
            this.lblCurrentDateLabel.Size = new System.Drawing.Size(103, 20);
            this.lblCurrentDateLabel.TabIndex = 10;
            this.lblCurrentDateLabel.Text = "Current Date:";

            // lblCurrentDate
            this.lblCurrentDate.AutoSize = true;
            this.lblCurrentDate.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCurrentDate.ForeColor = this.DarkText;
            this.lblCurrentDate.Location = new System.Drawing.Point(805, 82);
            this.lblCurrentDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentDate.Name = "lblCurrentDate";
            this.lblCurrentDate.Size = new System.Drawing.Size(86, 20);
            this.lblCurrentDate.TabIndex = 11;
            this.lblCurrentDate.Text = DateTime.Now.ToString("dd MMM yyyy");

            // ReturnsForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = this.White;
            this.ClientSize = new System.Drawing.Size(1000, 680);
            this.Controls.Add(this.txtBillIdUnderline);
            this.Controls.Add(this.lblCurrentDate);
            this.Controls.Add(this.lblCurrentDateLabel);
            this.Controls.Add(this.lblBillDate);
            this.Controls.Add(this.lblBillDateLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnProcessReturn);
            this.Controls.Add(this.dgvBillItems);
            this.Controls.Add(this.btnSearchBill);
            this.Controls.Add(this.txtBillId);
            this.Controls.Add(this.lblBillId);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ReturnsForm";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Returns Management";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBillItems)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}