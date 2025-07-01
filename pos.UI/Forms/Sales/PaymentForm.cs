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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CustomerContact { get; private set; }

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
            lblTokenValue.Text = _tokenValue > 0 ? $"-{_tokenValue:N2}" : "None";
            lblAmountDue.Text = _amountDue.ToString("N2");
            lblTotalAmount.Text = _totalAmount.ToString("N2");

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

            if (cmbPaymentMethod.Text == "Cash")
            {
                txtCardDigits.Clear();
                txtBankDigits.Clear();
            }

            if (pnlCash.Visible) txtCashTendered.Select();
            if (pnlCard.Visible) txtCardDigits.Select();
            if (pnlBank.Visible) txtBankDigits.Select();
        }

        private void txtCashTendered_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtCashTendered.Text, out decimal tendered))
            {
                decimal change = tendered - _amountDue;
                lblChange.Text = change >= 0 ? change.ToString("N2") : "Insufficient";
                btnComplete.Enabled = change >= 0;
            }
            else
            {
                lblChange.Text = "Rs.0.00";
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

        private void txtCustomerContact_TextChanged(object sender, EventArgs e)
        {
            // Enable complete button if contact is valid or empty
            ValidateContact();
        }

        private void ValidateContact()
        {
            string contact = txtCustomerContact.Text.Replace(" ", "").Replace("-", "");
            
            if (string.IsNullOrEmpty(contact))
            {
                // Contact is optional, so valid if empty
                return;
            }
            
            if (contact.Length != 10)
            {
                lblContactError.Text = "Contact must be 10 digits";
                lblContactError.Visible = true;
                btnComplete.Enabled = false;
            }
            else
            {
                lblContactError.Visible = false;
                // Only enable if other validations pass
                UpdateCompleteButtonState();
            }
        }

        private void UpdateCompleteButtonState()
        {
            bool contactValid = string.IsNullOrEmpty(txtCustomerContact.Text) || 
                               txtCustomerContact.Text.Replace(" ", "").Replace("-", "").Length == 10;

            if (cmbPaymentMethod.Text == "Cash")
            {
                btnComplete.Enabled = contactValid && decimal.TryParse(txtCashTendered.Text, out _);
            }
            else if (cmbPaymentMethod.Text == "Card")
            {
                btnComplete.Enabled = contactValid && txtCardDigits.Text.Length == 4;
            }
            else if (cmbPaymentMethod.Text == "Bank Transfer")
            {
                btnComplete.Enabled = contactValid && txtBankDigits.Text.Length == 4;
            }
            else
            {
                btnComplete.Enabled = contactValid;
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            string contact = txtCustomerContact.Text.Replace(" ", "").Replace("-", "");
            
            if (!string.IsNullOrEmpty(contact) && contact.Length != 10)
            {
                MessageBox.Show("Customer contact must be 10 digits if provided");
                return;
            }

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

            PaymentMethod = cmbPaymentMethod.Text;
            AmountTendered = cmbPaymentMethod.Text == "Cash" ? 
                decimal.Parse(txtCashTendered.Text) : _amountDue;

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
                decimal.Parse(lblChange.Text.Replace("Rs.", "").Replace("(", "").Replace(")", "")) : 0;

            CustomerContact = string.IsNullOrEmpty(contact) ? null : contact;
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
            txtCustomerContact.KeyDown += TxtCustomerContact_KeyDown;
        }

        private void TxtCustomerContact_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnComplete.Enabled)
            {
                btnComplete.PerformClick();
            }
        }

        private void txtCashTendered_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}