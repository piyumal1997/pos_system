using System;
using System.ComponentModel;
using pos_system.pos.UI.Forms;
using pos_system;
using pos_system.pos;
using pos_system.pos.UI;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class PaymentForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PaymentMethod { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal AmountTendered { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CardLastFour { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string BankLastFour { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal Change { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsConfirmed { get; private set; }

        private readonly decimal _totalAmount;
        private readonly decimal _tokenValue;
        private readonly decimal _amountDue;

        public PaymentForm(decimal totalAmount, decimal tokenValue)
        {
            _totalAmount = totalAmount;
            _tokenValue = tokenValue;
            _amountDue = Math.Max(0, totalAmount - tokenValue);

            InitializeComponent();
            SetupInitialState();
        }

        private void SetupInitialState()
        {
            // Display token info if applicable
            lblTokenValue.Text = _tokenValue > 0 ? $"-{_tokenValue:C2}" : "None";
            lblAmountDue.Text = _amountDue.ToString("C2");
            lblTotalAmount.Text = _totalAmount.ToString("C2");

            // Default to Cash if token covers full amount
            if (_amountDue == 0)
            {
                cmbPaymentMethod.SelectedIndex = -1;
                cmbPaymentMethod.Enabled = false;
                txtCashTendered.Text = "0.00";
                txtCashTendered.Enabled = false;
                pnlCash.Visible = false;
            }
        }

        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlCash.Visible = cmbPaymentMethod.Text == "Cash";
            pnlCard.Visible = cmbPaymentMethod.Text == "Card";
            pnlBank.Visible = cmbPaymentMethod.Text == "Bank Transfer";

            // Clear fields when switching to cash
            if (cmbPaymentMethod.Text == "Cash")
            {
                txtCardDigits.Clear();
                txtBankDigits.Clear();
            }

            // Auto-focus first input
            if (pnlCash.Visible) txtCashTendered.Select();
            if (pnlCard.Visible) txtCardDigits.Select();
            if (pnlBank.Visible) txtBankDigits.Select();
        }

        private void txtCashTendered_TextChanged(object sender, EventArgs e)
        {
            // Auto-calculate change
            if (decimal.TryParse(txtCashTendered.Text, out decimal tendered))
            {
                decimal change = tendered - _amountDue;
                lblChange.Text = change >= 0 ? change.ToString("C2") : "Insufficient";
                btnComplete.Enabled = change >= 0;
            }
            else
            {
                lblChange.Text = "$0.00";
                btnComplete.Enabled = false;
            }
        }

        private void txtCardDigits_TextChanged(object sender, EventArgs e)
        {
            btnComplete.Enabled = txtCardDigits.Text.Length == 4;
        }

        private void txtBankDigits_TextChanged(object sender, EventArgs e)
        {
            btnComplete.Enabled = txtBankDigits.Text.Length == 4;
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (cmbPaymentMethod.Text == "Card" && txtCardDigits.Text.Length != 4)
            {
                MessageBox.Show("Please enter last 4 digits of card");
                return;
            }

            if (cmbPaymentMethod.Text == "Bank Transfer" && txtBankDigits.Text.Length != 4)
            {
                MessageBox.Show("Please enter last 4 digits of account");
                return;
            }

            // Set properties
            PaymentMethod = cmbPaymentMethod.Text;
            AmountTendered = cmbPaymentMethod.Text == "Cash" ?
                decimal.Parse(txtCashTendered.Text) : _amountDue;

            // Clear card/bank details for cash payments
            if (PaymentMethod == "Cash")
            {
                CardLastFour = null;
                BankLastFour = null;
            }
            else
            {
                CardLastFour = PaymentMethod == "Card" ? txtCardDigits.Text : null;
                BankLastFour = PaymentMethod == "Bank Transfer" ? txtBankDigits.Text : null;
            }

            Change = cmbPaymentMethod.Text == "Cash" ?
                decimal.Parse(lblChange.Text.Replace("$", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty)) : 0;

            IsConfirmed = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            if (_amountDue == 0)
            {
                btnComplete.Enabled = true;
            }
        }
    }
}