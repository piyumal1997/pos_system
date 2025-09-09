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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CustomerGender { get; private set; }

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
            string input = txtCardDigits.Text;
            string digitsOnly = new string(input.Where(char.IsDigit).ToArray());

            // Truncate to 4 digits if needed
            if (digitsOnly.Length > 4)
            {
                digitsOnly = digitsOnly.Substring(0, 4);
            }

            // Only update if there's a mismatch
            if (digitsOnly != input)
            {
                int selectionStart = txtCardDigits.SelectionStart;
                txtCardDigits.Text = digitsOnly;
                txtCardDigits.SelectionStart = selectionStart > digitsOnly.Length
                    ? digitsOnly.Length
                    : selectionStart;
            }

            btnComplete.Enabled = txtCardDigits.Text.Length == 4;
        }

        private void txtBankDigits_TextChanged(object sender, EventArgs e)
        {
            string input = txtBankDigits.Text;
            string digitsOnly = new string(input.Where(char.IsDigit).ToArray());

            // Truncate to 4 digits if needed
            if (digitsOnly.Length > 4)
            {
                digitsOnly = digitsOnly.Substring(0, 4);
            }

            // Only update if there's a mismatch
            if (digitsOnly != input)
            {
                int selectionStart = txtBankDigits.SelectionStart;
                txtBankDigits.Text = digitsOnly;
                txtBankDigits.SelectionStart = selectionStart > digitsOnly.Length
                    ? digitsOnly.Length
                    : selectionStart;
            }

            btnComplete.Enabled = txtBankDigits.Text.Length == 4;
        }

        private void txtCustomerContact_TextChanged(object sender, EventArgs e)
        {
            string contact = txtCustomerContact.Text;
            string digitsOnly = new string(contact.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length > 10)
            {
                digitsOnly = digitsOnly.Substring(0, 10);
            }

            if (digitsOnly != contact)
            {
                int selectionStart = txtCustomerContact.SelectionStart;
                txtCustomerContact.Text = digitsOnly;
                txtCustomerContact.SelectionStart = selectionStart > digitsOnly.Length
                    ? digitsOnly.Length
                    : selectionStart;
            }

            bool hasValidContact = (txtCustomerContact.Text.Length == 10);
            cmbGender.Enabled = hasValidContact;

            // Visual feedback for required gender field
            if (hasValidContact)
            {
                lblGender.ForeColor = Color.Red;
                lblGender.Text = "Gender:*"; // Add asterisk to indicate required
                cmbGender.BackColor = Color.LightYellow;
            }
            else
            {
                lblGender.ForeColor = SystemColors.ControlText;
                lblGender.Text = "Gender:";
                cmbGender.BackColor = SystemColors.Window;
                cmbGender.SelectedIndex = -1;
            }

            // Call the validation method
            ValidateContact();

            // Enable gender only when valid contact is provided
            bool hasContact = (txtCustomerContact.Text.Length == 10);
            cmbGender.Enabled = hasContact;

            if (!hasContact)
            {
                cmbGender.SelectedIndex = -1;
            }

            UpdateCompleteButtonState();
        }

        //private void ValidateContact()
        //{
        //    string contact = txtCustomerContact.Text;

        //    if (string.IsNullOrEmpty(contact))
        //    {
        //        lblContactError.Visible = false;
        //        UpdateCompleteButtonState();
        //        return;
        //    }

        //    if (contact.Length != 10)
        //    {
        //        lblContactError.Text = "Contact must be 10 digits";
        //        lblContactError.Visible = true;
        //        btnComplete.Enabled = false;
        //    }
        //    else
        //    {
        //        lblContactError.Visible = false;
        //        UpdateCompleteButtonState();
        //    }
        //}

        private void ValidateContact()
        {
            string contact = txtCustomerContact.Text;

            if (string.IsNullOrEmpty(contact))
            {
                lblContactError.Visible = false;
                lblContactError.Text = "";
                UpdateCompleteButtonState();
                txtCustomerContact.BackColor = Color.White;
                return;
            }

            if (contact.Length != 10)
            {
                lblContactError.Text = "Contact must be exactly 10 digits";
                lblContactError.Visible = true;
                lblContactError.ForeColor = Color.Red;
                txtCustomerContact.BackColor = Color.LightPink;
            }
            else
            {
                lblContactError.Visible = false;
                txtCustomerContact.BackColor = Color.LightGreen;
                //lblContactError.Text = "";
                //txtCustomerContact.BackColor = SystemColors.Window;

                //// Check if gender is required but not selected
                //if (cmbGender.Enabled && cmbGender.SelectedIndex < 0)
                //{
                //    lblContactError.Text = "Gender selection is required";
                //    lblContactError.Visible = true;
                //    lblContactError.ForeColor = Color.Orange;
                //}
            }

            UpdateCompleteButtonState();
        }

        //private void UpdateCompleteButtonState()
        //{
        //    bool contactValid = string.IsNullOrEmpty(txtCustomerContact.Text) ||
        //                       txtCustomerContact.Text.Length == 10;

        //    bool genderValid = string.IsNullOrEmpty(txtCustomerContact.Text) ||
        //                      (cmbGender.Enabled && cmbGender.SelectedIndex >= 0);

        //    if (cmbPaymentMethod.Text == "Cash")
        //    {
        //        btnComplete.Enabled = contactValid && genderValid &&
        //                              decimal.TryParse(txtCashTendered.Text, out _);
        //    }
        //    else if (cmbPaymentMethod.Text == "Card")
        //    {
        //        btnComplete.Enabled = contactValid && genderValid &&
        //                              txtCardDigits.Text.Length == 4;
        //    }
        //    else if (cmbPaymentMethod.Text == "Bank Transfer")
        //    {
        //        btnComplete.Enabled = contactValid && genderValid &&
        //                              txtBankDigits.Text.Length == 4;
        //    }
        //    else
        //    {
        //        btnComplete.Enabled = contactValid && genderValid;
        //    }
        //}

        private void UpdateCompleteButtonState()
        {
            bool contactValid = string.IsNullOrEmpty(txtCustomerContact.Text) ||
                               txtCustomerContact.Text.Length == 10;

            bool genderValid = string.IsNullOrEmpty(txtCustomerContact.Text) ||
                              (cmbGender.Enabled && cmbGender.SelectedIndex >= 0);

            // Additional check for payment method specific validations
            bool paymentValid = true;

            if (cmbPaymentMethod.Text == "Cash")
            {
                paymentValid = decimal.TryParse(txtCashTendered.Text, out decimal tendered) &&
                              tendered >= _amountDue;
            }
            else if (cmbPaymentMethod.Text == "Card")
            {
                paymentValid = txtCardDigits.Text.Length == 4;
            }
            else if (cmbPaymentMethod.Text == "Bank Transfer")
            {
                paymentValid = txtBankDigits.Text.Length == 4;
            }

            btnComplete.Enabled = contactValid && genderValid && paymentValid;
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            string contact = txtCustomerContact.Text;

            // Validate contact
            if (!string.IsNullOrEmpty(contact) && contact.Length != 10)
            {
                MessageBox.Show("Customer contact must be 10 digits if provided",
                                "Validation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Validate gender if contact provided
            if (!string.IsNullOrEmpty(contact) && cmbGender.SelectedIndex < 0)
            {
                MessageBox.Show("Please select customer gender",
                                "Validation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
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

            CustomerGender = cmbGender.Enabled && cmbGender.SelectedItem != null
                ? cmbGender.SelectedItem.ToString()
                : null;

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

        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Provide visual feedback for gender validation
            if (cmbGender.Enabled && cmbGender.SelectedIndex < 0)
            {
                cmbGender.BackColor = Color.LightPink;
            }
            else
            {
                cmbGender.BackColor = SystemColors.Window;
            }

            UpdateCompleteButtonState();
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            if (_amountDue == 0)
            {
                btnComplete.Enabled = true;
            }
            // txtCustomerContact.KeyDown += TxtCustomerContact_KeyDown;

            // Enter Key:
            this.AcceptButton = btnComplete;
        }

        private void txtCustomerContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DigitsOnlyTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        //private void TxtCustomerContact_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter && btnComplete.Enabled)
        //    {
        //        btnComplete.PerformClick();
        //    }
        //}

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