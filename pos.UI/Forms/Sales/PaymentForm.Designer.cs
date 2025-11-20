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
        private TextBox txtCardDigits;
        private Panel pnlBank;
        private TextBox txtBankDigits;
        private Button btnComplete;
        private Button btnCancel;
        private Panel pnlContact;
        private TextBox txtCustomerContact;
        private Label lblContact;
        private Label lblContactError;
        private Label lblCardPrompt;
        private Label lblBankPrompt;
        private Label lblCashTendered;
        private Label lblGender;
        private ComboBox cmbGender;

        // Mixed Payment Controls
        private Panel pnlMixed;
        private Label lblFirstPayment;
        private ComboBox cmbFirstPaymentMethod;
        private TextBox txtFirstPaymentAmount;
        private Panel pnlFirstCard;
        private TextBox txtFirstCardDigits;
        private Label lblFirstCardPrompt;
        private Panel pnlFirstBank;
        private TextBox txtFirstBankDigits;
        private Label lblFirstBankPrompt;
        private Label lblSecondPayment;
        private ComboBox cmbSecondPaymentMethod;
        private TextBox txtSecondPaymentAmount;
        private Panel pnlSecondCard;
        private TextBox txtSecondCardDigits;
        private Label lblSecondCardPrompt;
        private Panel pnlSecondBank;
        private TextBox txtSecondBankDigits;
        private Label lblSecondBankPrompt;
        private Label lblMixedSummary;
        private Label lblMixedTotalLabel;
        private Label lblMixedTotal;
        private Label lblMixedRemainingLabel;
        private Label lblMixedRemaining;
        private Label lblMixedError;

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
            lblTitle = new Label();
            lblTotalLabel = new Label();
            lblTotalAmount = new Label();
            lblTokenLabel = new Label();
            lblTokenValue = new Label();
            lblDueLabel = new Label();
            lblAmountDue = new Label();
            cmbPaymentMethod = new ComboBox();
            pnlCash = new Panel();
            lblChange = new Label();
            lblChangeLabel = new Label();
            txtCashTendered = new TextBox();
            lblCashTendered = new Label();
            pnlCard = new Panel();
            lblCardPrompt = new Label();
            txtCardDigits = new TextBox();
            pnlBank = new Panel();
            lblBankPrompt = new Label();
            txtBankDigits = new TextBox();
            btnComplete = new Button();
            btnCancel = new Button();
            pnlContact = new Panel();
            lblGender = new Label();
            cmbGender = new ComboBox();
            lblContactError = new Label();
            txtCustomerContact = new TextBox();
            lblContact = new Label();
            pnlMixed = new Panel();
            lblMixedError = new Label();
            lblMixedRemaining = new Label();
            lblMixedRemainingLabel = new Label();
            lblMixedTotal = new Label();
            lblMixedTotalLabel = new Label();
            lblMixedSummary = new Label();
            pnlSecondCard = new Panel();
            lblSecondCardPrompt = new Label();
            txtSecondCardDigits = new TextBox();
            pnlSecondBank = new Panel();
            lblSecondBankPrompt = new Label();
            txtSecondBankDigits = new TextBox();
            txtSecondPaymentAmount = new TextBox();
            cmbSecondPaymentMethod = new ComboBox();
            lblSecondPayment = new Label();
            pnlFirstCard = new Panel();
            lblFirstCardPrompt = new Label();
            txtFirstCardDigits = new TextBox();
            pnlFirstBank = new Panel();
            lblFirstBankPrompt = new Label();
            txtFirstBankDigits = new TextBox();
            txtFirstPaymentAmount = new TextBox();
            cmbFirstPaymentMethod = new ComboBox();
            lblFirstPayment = new Label();
            pnlCash.SuspendLayout();
            pnlCard.SuspendLayout();
            pnlBank.SuspendLayout();
            pnlContact.SuspendLayout();
            pnlMixed.SuspendLayout();
            pnlSecondCard.SuspendLayout();
            pnlSecondBank.SuspendLayout();
            pnlFirstCard.SuspendLayout();
            pnlFirstBank.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(210, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Complete Payment";
            // 
            // lblTotalLabel
            // 
            lblTotalLabel.AutoSize = true;
            lblTotalLabel.Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTotalLabel.Location = new Point(20, 65);
            lblTotalLabel.Name = "lblTotalLabel";
            lblTotalLabel.Size = new Size(123, 25);
            lblTotalLabel.TabIndex = 1;
            lblTotalLabel.Text = "Total Amount:";
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalAmount.Location = new Point(250, 65);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(72, 25);
            lblTotalAmount.TabIndex = 2;
            lblTotalAmount.Text = "Rs.0.00";
            // 
            // lblTokenLabel
            // 
            lblTokenLabel.AutoSize = true;
            lblTokenLabel.Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTokenLabel.Location = new Point(20, 100);
            lblTokenLabel.Name = "lblTokenLabel";
            lblTokenLabel.Size = new Size(109, 25);
            lblTokenLabel.TabIndex = 3;
            lblTokenLabel.Text = "Token Value:";
            // 
            // lblTokenValue
            // 
            lblTokenValue.AutoSize = true;
            lblTokenValue.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTokenValue.ForeColor = Color.Red;
            lblTokenValue.Location = new Point(250, 100);
            lblTokenValue.Name = "lblTokenValue";
            lblTokenValue.Size = new Size(72, 25);
            lblTokenValue.TabIndex = 4;
            lblTokenValue.Text = "Rs.0.00";
            // 
            // lblDueLabel
            // 
            lblDueLabel.AutoSize = true;
            lblDueLabel.Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDueLabel.Location = new Point(20, 135);
            lblDueLabel.Name = "lblDueLabel";
            lblDueLabel.Size = new Size(118, 25);
            lblDueLabel.TabIndex = 5;
            lblDueLabel.Text = "Amount Due:";
            // 
            // lblAmountDue
            // 
            lblAmountDue.AutoSize = true;
            lblAmountDue.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAmountDue.Location = new Point(250, 135);
            lblAmountDue.Name = "lblAmountDue";
            lblAmountDue.Size = new Size(72, 25);
            lblAmountDue.TabIndex = 6;
            lblAmountDue.Text = "Rs.0.00";
            // 
            // cmbPaymentMethod
            // 
            cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentMethod.Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbPaymentMethod.FormattingEnabled = true;
            cmbPaymentMethod.Items.AddRange(new object[] { "Cash", "Card", "Bank Transfer", "Mixed" });
            cmbPaymentMethod.Location = new Point(25, 175);
            cmbPaymentMethod.Name = "cmbPaymentMethod";
            cmbPaymentMethod.Size = new Size(290, 31);
            cmbPaymentMethod.TabIndex = 7;
            cmbPaymentMethod.SelectedIndexChanged += cmbPaymentMethod_SelectedIndexChanged;
            // 
            // pnlCash
            // 
            pnlCash.Controls.Add(lblChange);
            pnlCash.Controls.Add(lblChangeLabel);
            pnlCash.Controls.Add(txtCashTendered);
            pnlCash.Controls.Add(lblCashTendered);
            pnlCash.Location = new Point(25, 350);
            pnlCash.Name = "pnlCash";
            pnlCash.Size = new Size(350, 130);
            pnlCash.TabIndex = 8;
            pnlCash.Visible = false;
            // 
            // lblChange
            // 
            lblChange.AutoSize = true;
            lblChange.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblChange.Location = new Point(100, 85);
            lblChange.Name = "lblChange";
            lblChange.Size = new Size(75, 25);
            lblChange.TabIndex = 2;
            lblChange.Text = "Rs.0.00";
            // 
            // lblChangeLabel
            // 
            lblChangeLabel.AutoSize = true;
            lblChangeLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblChangeLabel.Location = new Point(15, 85);
            lblChangeLabel.Name = "lblChangeLabel";
            lblChangeLabel.Size = new Size(84, 25);
            lblChangeLabel.TabIndex = 1;
            lblChangeLabel.Text = "Change:";
            // 
            // txtCashTendered
            // 
            txtCashTendered.Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCashTendered.Location = new Point(15, 40);
            txtCashTendered.Name = "txtCashTendered";
            txtCashTendered.Size = new Size(320, 31);
            txtCashTendered.TabIndex = 0;
            txtCashTendered.TextChanged += txtCashTendered_TextChanged;
            txtCashTendered.KeyPress += DecimalTextBox_KeyPress;
            // 
            // lblCashTendered
            // 
            lblCashTendered.AutoSize = true;
            lblCashTendered.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCashTendered.Location = new Point(15, 15);
            lblCashTendered.Name = "lblCashTendered";
            lblCashTendered.Size = new Size(141, 25);
            lblCashTendered.TabIndex = 3;
            lblCashTendered.Text = "Cash Tendered:";
            // 
            // pnlCard
            // 
            pnlCard.Controls.Add(lblCardPrompt);
            pnlCard.Controls.Add(txtCardDigits);
            pnlCard.Location = new Point(25, 350);
            pnlCard.Name = "pnlCard";
            pnlCard.Size = new Size(350, 100);
            pnlCard.TabIndex = 9;
            pnlCard.Visible = false;
            // 
            // lblCardPrompt
            // 
            lblCardPrompt.AutoSize = true;
            lblCardPrompt.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCardPrompt.Location = new Point(15, 10);
            lblCardPrompt.Name = "lblCardPrompt";
            lblCardPrompt.Size = new Size(193, 25);
            lblCardPrompt.TabIndex = 1;
            lblCardPrompt.Text = "Last 4 digits of Card:";
            // 
            // txtCardDigits
            // 
            txtCardDigits.Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCardDigits.Location = new Point(15, 40);
            txtCardDigits.Name = "txtCardDigits";
            txtCardDigits.Size = new Size(320, 31);
            txtCardDigits.TabIndex = 0;
            txtCardDigits.TextChanged += txtCardDigits_TextChanged;
            txtCardDigits.KeyPress += DigitsOnlyTextBox_KeyPress;
            // 
            // pnlBank
            // 
            pnlBank.Controls.Add(lblBankPrompt);
            pnlBank.Controls.Add(txtBankDigits);
            pnlBank.Location = new Point(25, 350);
            pnlBank.Name = "pnlBank";
            pnlBank.Size = new Size(350, 100);
            pnlBank.TabIndex = 10;
            pnlBank.Visible = false;
            // 
            // lblBankPrompt
            // 
            lblBankPrompt.AutoSize = true;
            lblBankPrompt.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBankPrompt.Location = new Point(15, 10);
            lblBankPrompt.Name = "lblBankPrompt";
            lblBankPrompt.Size = new Size(257, 25);
            lblBankPrompt.TabIndex = 1;
            lblBankPrompt.Text = "Last 4 digits of Account No:";
            // 
            // txtBankDigits
            // 
            txtBankDigits.Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtBankDigits.Location = new Point(15, 40);
            txtBankDigits.Name = "txtBankDigits";
            txtBankDigits.Size = new Size(320, 31);
            txtBankDigits.TabIndex = 0;
            txtBankDigits.TextChanged += txtBankDigits_TextChanged;
            txtBankDigits.KeyPress += DigitsOnlyTextBox_KeyPress;
            // 
            // btnComplete
            // 
            btnComplete.BackColor = Color.SeaGreen;
            btnComplete.Enabled = false;
            btnComplete.FlatStyle = FlatStyle.Flat;
            btnComplete.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnComplete.ForeColor = Color.White;
            btnComplete.Location = new Point(40, 603);
            btnComplete.Name = "btnComplete";
            btnComplete.Size = new Size(160, 50);
            btnComplete.TabIndex = 11;
            btnComplete.Text = "Complete";
            btnComplete.UseVisualStyleBackColor = false;
            btnComplete.Click += btnComplete_Click;
            // 
            // btnCancel
            // 
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancel.Location = new Point(250, 603);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(160, 50);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // pnlContact
            // 
            pnlContact.Controls.Add(lblGender);
            pnlContact.Controls.Add(cmbGender);
            pnlContact.Controls.Add(lblContactError);
            pnlContact.Controls.Add(txtCustomerContact);
            pnlContact.Controls.Add(lblContact);
            pnlContact.Location = new Point(25, 220);
            pnlContact.Name = "pnlContact";
            pnlContact.Size = new Size(350, 120);
            pnlContact.TabIndex = 13;
            // 
            // lblGender
            // 
            lblGender.AutoSize = true;
            lblGender.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblGender.Location = new Point(5, 70);
            lblGender.Name = "lblGender";
            lblGender.Size = new Size(80, 25);
            lblGender.TabIndex = 4;
            lblGender.Text = "Gender:";
            // 
            // cmbGender
            // 
            cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGender.Enabled = false;
            cmbGender.Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbGender.FormattingEnabled = true;
            cmbGender.Items.AddRange(new object[] { "Male", "Female" });
            cmbGender.Location = new Point(90, 67);
            cmbGender.Name = "cmbGender";
            cmbGender.Size = new Size(150, 31);
            cmbGender.TabIndex = 3;
            cmbGender.SelectedIndexChanged += cmbGender_SelectedIndexChanged;
            // 
            // lblContactError
            // 
            lblContactError.AutoSize = true;
            lblContactError.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblContactError.ForeColor = Color.Red;
            lblContactError.Location = new Point(5, 100);
            lblContactError.Name = "lblContactError";
            lblContactError.Size = new Size(0, 20);
            lblContactError.TabIndex = 2;
            // 
            // txtCustomerContact
            // 
            txtCustomerContact.Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCustomerContact.Location = new Point(5, 30);
            txtCustomerContact.Name = "txtCustomerContact";
            txtCustomerContact.Size = new Size(320, 31);
            txtCustomerContact.TabIndex = 1;
            txtCustomerContact.TextChanged += txtCustomerContact_TextChanged;
            txtCustomerContact.KeyPress += txtCustomerContact_KeyPress;
            // 
            // lblContact
            // 
            lblContact.AutoSize = true;
            lblContact.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblContact.Location = new Point(5, 0);
            lblContact.Name = "lblContact";
            lblContact.Size = new Size(259, 25);
            lblContact.TabIndex = 0;
            lblContact.Text = "Customer Contact (optional):";
            // 
            // pnlMixed
            // 
            pnlMixed.Controls.Add(lblMixedError);
            pnlMixed.Controls.Add(lblMixedRemaining);
            pnlMixed.Controls.Add(lblMixedRemainingLabel);
            pnlMixed.Controls.Add(lblMixedTotal);
            pnlMixed.Controls.Add(lblMixedTotalLabel);
            pnlMixed.Controls.Add(lblMixedSummary);
            pnlMixed.Controls.Add(pnlSecondCard);
            pnlMixed.Controls.Add(pnlSecondBank);
            pnlMixed.Controls.Add(txtSecondPaymentAmount);
            pnlMixed.Controls.Add(cmbSecondPaymentMethod);
            pnlMixed.Controls.Add(lblSecondPayment);
            pnlMixed.Controls.Add(pnlFirstCard);
            pnlMixed.Controls.Add(pnlFirstBank);
            pnlMixed.Controls.Add(txtFirstPaymentAmount);
            pnlMixed.Controls.Add(cmbFirstPaymentMethod);
            pnlMixed.Controls.Add(lblFirstPayment);
            pnlMixed.Location = new Point(25, 341);
            pnlMixed.Name = "pnlMixed";
            pnlMixed.Size = new Size(400, 256);
            pnlMixed.TabIndex = 16;
            pnlMixed.Visible = false;
            // 
            // lblMixedError
            // 
            lblMixedError.AutoSize = true;
            lblMixedError.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMixedError.ForeColor = Color.Red;
            lblMixedError.Location = new Point(15, 230);
            lblMixedError.Name = "lblMixedError";
            lblMixedError.Size = new Size(0, 20);
            lblMixedError.TabIndex = 15;
            // 
            // lblMixedRemaining
            // 
            lblMixedRemaining.AutoSize = true;
            lblMixedRemaining.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblMixedRemaining.Location = new Point(280, 206);
            lblMixedRemaining.Name = "lblMixedRemaining";
            lblMixedRemaining.Size = new Size(62, 21);
            lblMixedRemaining.TabIndex = 14;
            lblMixedRemaining.Text = "Rs.0.00";
            // 
            // lblMixedRemainingLabel
            // 
            lblMixedRemainingLabel.AutoSize = true;
            lblMixedRemainingLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblMixedRemainingLabel.Location = new Point(15, 206);
            lblMixedRemainingLabel.Name = "lblMixedRemainingLabel";
            lblMixedRemainingLabel.Size = new Size(97, 21);
            lblMixedRemainingLabel.TabIndex = 13;
            lblMixedRemainingLabel.Text = "Remaining:";
            // 
            // lblMixedTotal
            // 
            lblMixedTotal.AutoSize = true;
            lblMixedTotal.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblMixedTotal.Location = new Point(280, 181);
            lblMixedTotal.Name = "lblMixedTotal";
            lblMixedTotal.Size = new Size(62, 21);
            lblMixedTotal.TabIndex = 12;
            lblMixedTotal.Text = "Rs.0.00";
            // 
            // lblMixedTotalLabel
            // 
            lblMixedTotalLabel.AutoSize = true;
            lblMixedTotalLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblMixedTotalLabel.Location = new Point(15, 181);
            lblMixedTotalLabel.Name = "lblMixedTotalLabel";
            lblMixedTotalLabel.Size = new Size(52, 21);
            lblMixedTotalLabel.TabIndex = 11;
            lblMixedTotalLabel.Text = "Total:";
            // 
            // lblMixedSummary
            // 
            lblMixedSummary.AutoSize = true;
            lblMixedSummary.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblMixedSummary.Location = new Point(15, 156);
            lblMixedSummary.Name = "lblMixedSummary";
            lblMixedSummary.Size = new Size(98, 25);
            lblMixedSummary.TabIndex = 10;
            lblMixedSummary.Text = "Summary:";
            // 
            // pnlSecondCard
            // 
            pnlSecondCard.Controls.Add(lblSecondCardPrompt);
            pnlSecondCard.Controls.Add(txtSecondCardDigits);
            pnlSecondCard.Location = new Point(205, 95);
            pnlSecondCard.Name = "pnlSecondCard";
            pnlSecondCard.Size = new Size(180, 58);
            pnlSecondCard.TabIndex = 8;
            pnlSecondCard.Visible = false;
            // 
            // lblSecondCardPrompt
            // 
            lblSecondCardPrompt.AutoSize = true;
            lblSecondCardPrompt.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSecondCardPrompt.Location = new Point(5, 5);
            lblSecondCardPrompt.Name = "lblSecondCardPrompt";
            lblSecondCardPrompt.Size = new Size(93, 19);
            lblSecondCardPrompt.TabIndex = 1;
            lblSecondCardPrompt.Text = "Last 4 Digits:";
            // 
            // txtSecondCardDigits
            // 
            txtSecondCardDigits.Font = new Font("Segoe UI", 11F);
            txtSecondCardDigits.Location = new Point(5, 25);
            txtSecondCardDigits.Name = "txtSecondCardDigits";
            txtSecondCardDigits.Size = new Size(170, 27);
            txtSecondCardDigits.TabIndex = 0;
            txtSecondCardDigits.TextChanged += txtSecondCardDigits_TextChanged;
            txtSecondCardDigits.KeyPress += DigitsOnlyTextBox_KeyPress;
            // 
            // pnlSecondBank
            // 
            pnlSecondBank.Controls.Add(lblSecondBankPrompt);
            pnlSecondBank.Controls.Add(txtSecondBankDigits);
            pnlSecondBank.Location = new Point(205, 95);
            pnlSecondBank.Name = "pnlSecondBank";
            pnlSecondBank.Size = new Size(180, 58);
            pnlSecondBank.TabIndex = 9;
            pnlSecondBank.Visible = false;
            // 
            // lblSecondBankPrompt
            // 
            lblSecondBankPrompt.AutoSize = true;
            lblSecondBankPrompt.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblSecondBankPrompt.Location = new Point(5, 5);
            lblSecondBankPrompt.Name = "lblSecondBankPrompt";
            lblSecondBankPrompt.Size = new Size(93, 19);
            lblSecondBankPrompt.TabIndex = 1;
            lblSecondBankPrompt.Text = "Last 4 Digits:";
            // 
            // txtSecondBankDigits
            // 
            txtSecondBankDigits.Font = new Font("Segoe UI", 11F);
            txtSecondBankDigits.Location = new Point(5, 25);
            txtSecondBankDigits.Name = "txtSecondBankDigits";
            txtSecondBankDigits.Size = new Size(170, 27);
            txtSecondBankDigits.TabIndex = 0;
            txtSecondBankDigits.TextChanged += txtSecondBankDigits_TextChanged;
            txtSecondBankDigits.KeyPress += DigitsOnlyTextBox_KeyPress;
            // 
            // txtSecondPaymentAmount
            // 
            txtSecondPaymentAmount.Font = new Font("Segoe UI", 12F);
            txtSecondPaymentAmount.Location = new Point(205, 65);
            txtSecondPaymentAmount.Name = "txtSecondPaymentAmount";
            txtSecondPaymentAmount.Size = new Size(180, 29);
            txtSecondPaymentAmount.TabIndex = 7;
            txtSecondPaymentAmount.Text = "0.00";
            txtSecondPaymentAmount.TextChanged += txtSecondPaymentAmount_TextChanged;
            txtSecondPaymentAmount.KeyPress += DecimalTextBox_KeyPress;
            // 
            // cmbSecondPaymentMethod
            // 
            cmbSecondPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSecondPaymentMethod.Font = new Font("Segoe UI", 12F);
            cmbSecondPaymentMethod.FormattingEnabled = true;
            cmbSecondPaymentMethod.Items.AddRange(new object[] { "Cash", "Card", "Bank Transfer" });
            cmbSecondPaymentMethod.Location = new Point(205, 30);
            cmbSecondPaymentMethod.Name = "cmbSecondPaymentMethod";
            cmbSecondPaymentMethod.Size = new Size(180, 29);
            cmbSecondPaymentMethod.TabIndex = 6;
            cmbSecondPaymentMethod.SelectedIndexChanged += cmbSecondPaymentMethod_SelectedIndexChanged;
            // 
            // lblSecondPayment
            // 
            lblSecondPayment.AutoSize = true;
            lblSecondPayment.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSecondPayment.Location = new Point(205, 5);
            lblSecondPayment.Name = "lblSecondPayment";
            lblSecondPayment.Size = new Size(142, 21);
            lblSecondPayment.TabIndex = 5;
            lblSecondPayment.Text = "Second Payment:";
            // 
            // pnlFirstCard
            // 
            pnlFirstCard.Controls.Add(lblFirstCardPrompt);
            pnlFirstCard.Controls.Add(txtFirstCardDigits);
            pnlFirstCard.Location = new Point(15, 95);
            pnlFirstCard.Name = "pnlFirstCard";
            pnlFirstCard.Size = new Size(180, 58);
            pnlFirstCard.TabIndex = 3;
            pnlFirstCard.Visible = false;
            // 
            // lblFirstCardPrompt
            // 
            lblFirstCardPrompt.AutoSize = true;
            lblFirstCardPrompt.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblFirstCardPrompt.Location = new Point(5, 5);
            lblFirstCardPrompt.Name = "lblFirstCardPrompt";
            lblFirstCardPrompt.Size = new Size(93, 19);
            lblFirstCardPrompt.TabIndex = 1;
            lblFirstCardPrompt.Text = "Last 4 Digits:";
            // 
            // txtFirstCardDigits
            // 
            txtFirstCardDigits.Font = new Font("Segoe UI", 11F);
            txtFirstCardDigits.Location = new Point(5, 25);
            txtFirstCardDigits.Name = "txtFirstCardDigits";
            txtFirstCardDigits.Size = new Size(170, 27);
            txtFirstCardDigits.TabIndex = 0;
            txtFirstCardDigits.TextChanged += txtFirstCardDigits_TextChanged;
            txtFirstCardDigits.KeyPress += DigitsOnlyTextBox_KeyPress;
            // 
            // pnlFirstBank
            // 
            pnlFirstBank.Controls.Add(lblFirstBankPrompt);
            pnlFirstBank.Controls.Add(txtFirstBankDigits);
            pnlFirstBank.Location = new Point(15, 95);
            pnlFirstBank.Name = "pnlFirstBank";
            pnlFirstBank.Size = new Size(180, 58);
            pnlFirstBank.TabIndex = 4;
            pnlFirstBank.Visible = false;
            // 
            // lblFirstBankPrompt
            // 
            lblFirstBankPrompt.AutoSize = true;
            lblFirstBankPrompt.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblFirstBankPrompt.Location = new Point(5, 5);
            lblFirstBankPrompt.Name = "lblFirstBankPrompt";
            lblFirstBankPrompt.Size = new Size(93, 19);
            lblFirstBankPrompt.TabIndex = 1;
            lblFirstBankPrompt.Text = "Last 4 Digits:";
            // 
            // txtFirstBankDigits
            // 
            txtFirstBankDigits.Font = new Font("Segoe UI", 11F);
            txtFirstBankDigits.Location = new Point(5, 25);
            txtFirstBankDigits.Name = "txtFirstBankDigits";
            txtFirstBankDigits.Size = new Size(170, 27);
            txtFirstBankDigits.TabIndex = 0;
            txtFirstBankDigits.TextChanged += txtFirstBankDigits_TextChanged;
            txtFirstBankDigits.KeyPress += DigitsOnlyTextBox_KeyPress;
            // 
            // txtFirstPaymentAmount
            // 
            txtFirstPaymentAmount.Font = new Font("Segoe UI", 12F);
            txtFirstPaymentAmount.Location = new Point(15, 65);
            txtFirstPaymentAmount.Name = "txtFirstPaymentAmount";
            txtFirstPaymentAmount.Size = new Size(180, 29);
            txtFirstPaymentAmount.TabIndex = 2;
            txtFirstPaymentAmount.Text = "0.00";
            txtFirstPaymentAmount.TextChanged += txtFirstPaymentAmount_TextChanged;
            txtFirstPaymentAmount.KeyPress += DecimalTextBox_KeyPress;
            // 
            // cmbFirstPaymentMethod
            // 
            cmbFirstPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFirstPaymentMethod.Font = new Font("Segoe UI", 12F);
            cmbFirstPaymentMethod.FormattingEnabled = true;
            cmbFirstPaymentMethod.Items.AddRange(new object[] { "Cash", "Card", "Bank Transfer" });
            cmbFirstPaymentMethod.Location = new Point(15, 30);
            cmbFirstPaymentMethod.Name = "cmbFirstPaymentMethod";
            cmbFirstPaymentMethod.Size = new Size(180, 29);
            cmbFirstPaymentMethod.TabIndex = 1;
            cmbFirstPaymentMethod.SelectedIndexChanged += cmbFirstPaymentMethod_SelectedIndexChanged;
            // 
            // lblFirstPayment
            // 
            lblFirstPayment.AutoSize = true;
            lblFirstPayment.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblFirstPayment.Location = new Point(15, 5);
            lblFirstPayment.Name = "lblFirstPayment";
            lblFirstPayment.Size = new Size(118, 21);
            lblFirstPayment.TabIndex = 0;
            lblFirstPayment.Text = "First Payment:";
            // 
            // PaymentForm
            // 
            ClientSize = new Size(450, 669);
            Controls.Add(pnlMixed);
            Controls.Add(pnlContact);
            Controls.Add(btnCancel);
            Controls.Add(btnComplete);
            Controls.Add(pnlBank);
            Controls.Add(pnlCard);
            Controls.Add(pnlCash);
            Controls.Add(cmbPaymentMethod);
            Controls.Add(lblAmountDue);
            Controls.Add(lblDueLabel);
            Controls.Add(lblTokenValue);
            Controls.Add(lblTokenLabel);
            Controls.Add(lblTotalAmount);
            Controls.Add(lblTotalLabel);
            Controls.Add(lblTitle);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PaymentForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Payment Processing";
            Load += PaymentForm_Load;
            pnlCash.ResumeLayout(false);
            pnlCash.PerformLayout();
            pnlCard.ResumeLayout(false);
            pnlCard.PerformLayout();
            pnlBank.ResumeLayout(false);
            pnlBank.PerformLayout();
            pnlContact.ResumeLayout(false);
            pnlContact.PerformLayout();
            pnlMixed.ResumeLayout(false);
            pnlMixed.PerformLayout();
            pnlSecondCard.ResumeLayout(false);
            pnlSecondCard.PerformLayout();
            pnlSecondBank.ResumeLayout(false);
            pnlSecondBank.PerformLayout();
            pnlFirstCard.ResumeLayout(false);
            pnlFirstCard.PerformLayout();
            pnlFirstBank.ResumeLayout(false);
            pnlFirstBank.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }
    }
}