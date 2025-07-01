namespace pos_system.pos.UI.Forms.Sales
{
    partial class PaymentForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Label lblTotalLabel;
        private Label lblTotalAmount;
        private Label lblTokenLabel;
        private Label lblTokenValue;
        private Label lblDueLabel;
        private Label lblAmountDue;
        private ComboBox cmbPaymentMethod;
        private Panel pnlCash;
        private TextBox txtCashTendered;
        private Label lblChangeLabel;
        private Label lblChange;
        private Panel pnlCard;
        private MaskedTextBox txtCardDigits;
        private Panel pnlBank;
        private MaskedTextBox txtBankDigits;
        private Button btnComplete;
        private Button btnCancel;
        private Panel pnlContact;
        private MaskedTextBox txtCustomerContact;
        private Label lblContact;
        private Label lblContactError;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTotalLabel = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblTokenLabel = new System.Windows.Forms.Label();
            this.lblTokenValue = new System.Windows.Forms.Label();
            this.lblDueLabel = new System.Windows.Forms.Label();
            this.lblAmountDue = new System.Windows.Forms.Label();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.pnlCash = new System.Windows.Forms.Panel();
            this.lblChange = new System.Windows.Forms.Label();
            this.lblChangeLabel = new System.Windows.Forms.Label();
            this.txtCashTendered = new System.Windows.Forms.TextBox();
            this.pnlCard = new System.Windows.Forms.Panel();
            this.txtCardDigits = new System.Windows.Forms.MaskedTextBox();
            this.pnlBank = new System.Windows.Forms.Panel();
            this.txtBankDigits = new System.Windows.Forms.MaskedTextBox();
            this.btnComplete = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlContact = new System.Windows.Forms.Panel();
            this.lblContactError = new System.Windows.Forms.Label();
            this.txtCustomerContact = new System.Windows.Forms.MaskedTextBox();
            this.lblContact = new System.Windows.Forms.Label();
            this.pnlCash.SuspendLayout();
            this.pnlCard.SuspendLayout();
            this.pnlBank.SuspendLayout();
            this.pnlContact.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(165, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Complete Payment";
            // 
            // lblTotalLabel
            // 
            this.lblTotalLabel.AutoSize = true;
            this.lblTotalLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalLabel.Location = new System.Drawing.Point(14, 50);
            this.lblTotalLabel.Name = "lblTotalLabel";
            this.lblTotalLabel.Size = new System.Drawing.Size(82, 17);
            this.lblTotalLabel.TabIndex = 1;
            this.lblTotalLabel.Text = "Total Amount:";
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalAmount.Location = new System.Drawing.Point(200, 50);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(43, 17);
            this.lblTotalAmount.TabIndex = 2;
            this.lblTotalAmount.Text = "$0.00";
            // 
            // lblTokenLabel
            // 
            this.lblTokenLabel.AutoSize = true;
            this.lblTokenLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTokenLabel.Location = new System.Drawing.Point(14, 75);
            this.lblTokenLabel.Name = "lblTokenLabel";
            this.lblTokenLabel.Size = new System.Drawing.Size(79, 17);
            this.lblTokenLabel.TabIndex = 3;
            this.lblTokenLabel.Text = "Token Value:";
            // 
            // lblTokenValue
            // 
            this.lblTokenValue.AutoSize = true;
            this.lblTokenValue.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTokenValue.ForeColor = System.Drawing.Color.Red;
            this.lblTokenValue.Location = new System.Drawing.Point(200, 75);
            this.lblTokenValue.Name = "lblTokenValue";
            this.lblTokenValue.Size = new System.Drawing.Size(43, 17);
            this.lblTokenValue.TabIndex = 4;
            this.lblTokenValue.Text = "$0.00";
            // 
            // lblDueLabel
            // 
            this.lblDueLabel.AutoSize = true;
            this.lblDueLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDueLabel.Location = new System.Drawing.Point(14, 100);
            this.lblDueLabel.Name = "lblDueLabel";
            this.lblDueLabel.Size = new System.Drawing.Size(80, 17);
            this.lblDueLabel.TabIndex = 5;
            this.lblDueLabel.Text = "Amount Due:";
            // 
            // lblAmountDue
            // 
            this.lblAmountDue.AutoSize = true;
            this.lblAmountDue.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAmountDue.Location = new System.Drawing.Point(200, 100);
            this.lblAmountDue.Name = "lblAmountDue";
            this.lblAmountDue.Size = new System.Drawing.Size(43, 17);
            this.lblAmountDue.TabIndex = 6;
            this.lblAmountDue.Text = "$0.00";
            // 
            // cmbPaymentMethod
            // 
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPaymentMethod.FormattingEnabled = true;
            this.cmbPaymentMethod.Items.AddRange(new object[] {
            "Cash",
            "Card",
            "Bank Transfer"});
            this.cmbPaymentMethod.Location = new System.Drawing.Point(17, 130);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(226, 25);
            this.cmbPaymentMethod.TabIndex = 7;
            this.cmbPaymentMethod.SelectedIndexChanged += new System.EventHandler(this.cmbPaymentMethod_SelectedIndexChanged);
            // 
            // pnlCash
            // 
            this.pnlCash.Controls.Add(this.lblChange);
            this.pnlCash.Controls.Add(this.lblChangeLabel);
            this.pnlCash.Controls.Add(this.txtCashTendered);
            this.pnlCash.Location = new System.Drawing.Point(17, 220);
            this.pnlCash.Name = "pnlCash";
            this.pnlCash.Size = new System.Drawing.Size(300, 100);
            this.pnlCash.TabIndex = 8;
            // 
            // lblChange
            // 
            this.lblChange.AutoSize = true;
            this.lblChange.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChange.Location = new System.Drawing.Point(80, 50);
            this.lblChange.Name = "lblChange";
            this.lblChange.Size = new System.Drawing.Size(43, 17);
            this.lblChange.TabIndex = 2;
            this.lblChange.Text = "$0.00";
            // 
            // lblChangeLabel
            // 
            this.lblChangeLabel.AutoSize = true;
            this.lblChangeLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChangeLabel.Location = new System.Drawing.Point(10, 50);
            this.lblChangeLabel.Name = "lblChangeLabel";
            this.lblChangeLabel.Size = new System.Drawing.Size(56, 17);
            this.lblChangeLabel.TabIndex = 1;
            this.lblChangeLabel.Text = "Change:";
            // 
            // txtCashTendered
            // 
            this.txtCashTendered.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCashTendered.Location = new System.Drawing.Point(10, 10);
            this.txtCashTendered.Name = "txtCashTendered";
            this.txtCashTendered.Size = new System.Drawing.Size(280, 25);
            this.txtCashTendered.TabIndex = 0;
            this.txtCashTendered.TextChanged += new System.EventHandler(this.txtCashTendered_TextChanged);
            this.txtCashTendered.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCashTendered_KeyPress);
            // 
            // pnlCard
            // 
            this.pnlCard.Controls.Add(this.txtCardDigits);
            this.pnlCard.Location = new System.Drawing.Point(17, 220);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(300, 60);
            this.pnlCard.TabIndex = 9;
            this.pnlCard.Visible = false;
            // 
            // txtCardDigits
            // 
            this.txtCardDigits.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCardDigits.Location = new System.Drawing.Point(10, 10);
            this.txtCardDigits.Mask = "0000";
            this.txtCardDigits.Name = "txtCardDigits";
            this.txtCardDigits.Size = new System.Drawing.Size(280, 25);
            this.txtCardDigits.TabIndex = 0;
            this.txtCardDigits.TextChanged += new System.EventHandler(this.txtCardDigits_TextChanged);
            // 
            // pnlBank
            // 
            this.pnlBank.Controls.Add(this.txtBankDigits);
            this.pnlBank.Location = new System.Drawing.Point(17, 220);
            this.pnlBank.Name = "pnlBank";
            this.pnlBank.Size = new System.Drawing.Size(300, 60);
            this.pnlBank.TabIndex = 10;
            this.pnlBank.Visible = false;
            // 
            // txtBankDigits
            // 
            this.txtBankDigits.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBankDigits.Location = new System.Drawing.Point(10, 10);
            this.txtBankDigits.Mask = "0000";
            this.txtBankDigits.Name = "txtBankDigits";
            this.txtBankDigits.Size = new System.Drawing.Size(280, 25);
            this.txtBankDigits.TabIndex = 0;
            this.txtBankDigits.TextChanged += new System.EventHandler(this.txtBankDigits_TextChanged);
            // 
            // btnComplete
            // 
            this.btnComplete.BackColor = System.Drawing.Color.SeaGreen;
            this.btnComplete.Enabled = false;
            this.btnComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComplete.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnComplete.ForeColor = System.Drawing.Color.White;
            this.btnComplete.Location = new System.Drawing.Point(17, 300);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(140, 40);
            this.btnComplete.TabIndex = 11;
            this.btnComplete.Text = "Complete Payment";
            this.btnComplete.UseVisualStyleBackColor = false;
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(177, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 40);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlContact
            // 
            this.pnlContact.Controls.Add(this.lblContactError);
            this.pnlContact.Controls.Add(this.txtCustomerContact);
            this.pnlContact.Controls.Add(this.lblContact);
            this.pnlContact.Location = new System.Drawing.Point(17, 160);
            this.pnlContact.Name = "pnlContact";
            this.pnlContact.Size = new System.Drawing.Size(300, 60);
            this.pnlContact.TabIndex = 13;
            // 
            // lblContactError
            // 
            this.lblContactError.AutoSize = true;
            this.lblContactError.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContactError.ForeColor = System.Drawing.Color.Red;
            this.lblContactError.Location = new System.Drawing.Point(10, 40);
            this.lblContactError.Name = "lblContactError";
            this.lblContactError.Size = new System.Drawing.Size(0, 13);
            this.lblContactError.TabIndex = 2;
            // 
            // txtCustomerContact
            // 
            this.txtCustomerContact.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtCustomerContact.Location = new System.Drawing.Point(10, 25);
            this.txtCustomerContact.Mask = "0000 000 000";
            this.txtCustomerContact.Name = "txtCustomerContact";
            this.txtCustomerContact.Size = new System.Drawing.Size(280, 25);
            this.txtCustomerContact.TabIndex = 1;
            this.txtCustomerContact.TextChanged += new System.EventHandler(this.txtCustomerContact_TextChanged);
            // 
            // lblContact
            // 
            this.lblContact.AutoSize = true;
            this.lblContact.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContact.Location = new System.Drawing.Point(7, 7);
            this.lblContact.Name = "lblContact";
            this.lblContact.Size = new System.Drawing.Size(146, 15);
            this.lblContact.TabIndex = 0;
            this.lblContact.Text = "Customer Contact (optional):";
            // 
            // PaymentForm
            // 
            this.ClientSize = new System.Drawing.Size(334, 351);
            this.Controls.Add(this.pnlContact);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnComplete);
            this.Controls.Add(this.pnlBank);
            this.Controls.Add(this.pnlCard);
            this.Controls.Add(this.pnlCash);
            this.Controls.Add(this.cmbPaymentMethod);
            this.Controls.Add(this.lblAmountDue);
            this.Controls.Add(this.lblDueLabel);
            this.Controls.Add(this.lblTokenValue);
            this.Controls.Add(this.lblTokenLabel);
            this.Controls.Add(this.lblTotalAmount);
            this.Controls.Add(this.lblTotalLabel);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PaymentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Payment Processing";
            this.Load += new System.EventHandler(this.PaymentForm_Load);
            this.pnlCash.ResumeLayout(false);
            this.pnlCash.PerformLayout();
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.pnlBank.ResumeLayout(false);
            this.pnlBank.PerformLayout();
            this.pnlContact.ResumeLayout(false);
            this.pnlContact.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}