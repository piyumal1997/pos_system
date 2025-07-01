namespace pos_system.pos.UI.Forms.Sales
{
    partial class SizeSelectionForm
    {
        private System.ComponentModel.IContainer components = null;

        private DataGridView dgvSizes;
        private Button btnSelect;
        private Button btnCancel;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvSizes = new DataGridView();
            panel1 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnCancel = new Button();
            btnSelect = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvSizes).BeginInit();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // dgvSizes
            // 
            dgvSizes.AllowUserToAddRows = false;
            dgvSizes.AllowUserToDeleteRows = false;
            dgvSizes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSizes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSizes.Dock = DockStyle.Fill;
            dgvSizes.Location = new Point(0, 0);
            dgvSizes.Margin = new Padding(4);
            dgvSizes.Name = "dgvSizes";
            dgvSizes.ReadOnly = true;
            dgvSizes.RowHeadersWidth = 51;
            dgvSizes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSizes.Size = new Size(582, 324);
            dgvSizes.TabIndex = 0;
            dgvSizes.CellDoubleClick += dgvSizes_CellDoubleClick;
            dgvSizes.KeyDown += dgvSizes_KeyDown;
            // 
            // panel1
            // 
            panel1.Controls.Add(tableLayoutPanel1);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 324);
            panel1.Name = "panel1";
            panel1.Size = new Size(582, 60);
            panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tableLayoutPanel1.Controls.Add(btnCancel, 2, 0);
            tableLayoutPanel1.Controls.Add(btnSelect, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(582, 60);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.IndianRed;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(465, 10);
            btnCancel.Margin = new Padding(10);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 40);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSelect
            // 
            btnSelect.BackColor = Color.SeaGreen;
            btnSelect.Dock = DockStyle.Fill;
            btnSelect.FlatStyle = FlatStyle.Flat;
            btnSelect.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSelect.ForeColor = Color.White;
            btnSelect.Location = new Point(345, 10);
            btnSelect.Margin = new Padding(10);
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(100, 40);
            btnSelect.TabIndex = 0;
            btnSelect.Text = "Select";
            btnSelect.UseVisualStyleBackColor = false;
            btnSelect.Click += btnSelect_Click;
            // 
            // SizeSelectionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 384);
            Controls.Add(dgvSizes);
            Controls.Add(panel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SizeSelectionForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Size";
            ((System.ComponentModel.ISupportInitialize)dgvSizes).EndInit();
            panel1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}