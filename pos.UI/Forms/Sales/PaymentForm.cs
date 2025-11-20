using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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

        // Mixed Payment Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FirstPaymentMethod { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal FirstPaymentAmount { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FirstCardLastFour { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FirstBankLastFour { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SecondPaymentMethod { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal SecondPaymentAmount { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SecondCardLastFour { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SecondBankLastFour { get; private set; }

        private readonly decimal _totalAmount;
        private readonly decimal _tokenValue;
        private readonly decimal _amountDue;
        private bool _autoFillingAmounts = false;

        private bool _updatingFirstAmount = false;
        private bool _updatingSecondAmount = false;
        private bool _userEditingFirst = false;
        private bool _userEditingSecond = false;

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
                pnlMixed.Visible = false;
            }

            // Initialize mixed payment amounts
            txtFirstPaymentAmount.Text = "0.00";
            txtSecondPaymentAmount.Text = "0.00";
        }

        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlCash.Visible = cmbPaymentMethod.Text == "Cash";
            pnlCard.Visible = cmbPaymentMethod.Text == "Card";
            pnlBank.Visible = cmbPaymentMethod.Text == "Bank Transfer";
            pnlMixed.Visible = cmbPaymentMethod.Text == "Mixed";

            if (cmbPaymentMethod.Text == "Cash")
            {
                txtCardDigits.Clear();
                txtBankDigits.Clear();
                ResetMixedPayment();
            }
            else if (cmbPaymentMethod.Text == "Mixed")
            {
                ResetSinglePayment();
                InitializeMixedPayment();
            }
            else
            {
                ResetMixedPayment();
            }

            // Set focus to appropriate control
            if (pnlCash.Visible) txtCashTendered.Select();
            if (pnlCard.Visible) txtCardDigits.Select();
            if (pnlBank.Visible) txtBankDigits.Select();
            if (pnlMixed.Visible) cmbFirstPaymentMethod.Select();

            UpdateCompleteButtonState();
        }

        private void InitializeMixedPayment()
        {
            ResetMixedPaymentEditingFlags();

            // Set default values for mixed payment
            cmbFirstPaymentMethod.SelectedIndex = 0; // Cash
            cmbSecondPaymentMethod.SelectedIndex = 1; // Card

            // Ensure panels are in correct initial state
            pnlFirstCard.Visible = false;
            pnlFirstBank.Visible = false;
            pnlSecondCard.Visible = true; // Card is selected by default for second payment
            pnlSecondBank.Visible = false;

            // Initialize amounts without triggering auto-fill
            _updatingFirstAmount = true;
            _updatingSecondAmount = true;

            txtFirstPaymentAmount.Text = _amountDue.ToString("0.00");
            txtSecondPaymentAmount.Text = "0.00";

            _updatingFirstAmount = false;
            _updatingSecondAmount = false;

            UpdateMixedPaymentSummary();
        }

        private void ResetMixedPayment()
        {
            cmbFirstPaymentMethod.SelectedIndex = -1;
            cmbSecondPaymentMethod.SelectedIndex = -1;
            txtFirstPaymentAmount.Clear();
            txtSecondPaymentAmount.Clear();
            txtFirstCardDigits.Clear();
            txtFirstBankDigits.Clear();
            txtSecondCardDigits.Clear();
            txtSecondBankDigits.Clear();

            pnlFirstCard.Visible = false;
            pnlFirstBank.Visible = false;
            pnlSecondCard.Visible = false;
            pnlSecondBank.Visible = false;
        }

        private void ResetSinglePayment()
        {
            txtCashTendered.Clear();
            txtCardDigits.Clear();
            txtBankDigits.Clear();
        }

        private void cmbFirstPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetMixedPaymentEditingFlags();

            // First, hide all first payment method panels
            pnlFirstCard.Visible = false;
            pnlFirstBank.Visible = false;

            // Then show the appropriate panel
            if (cmbFirstPaymentMethod.Text == "Card")
            {
                pnlFirstCard.Visible = true;
                pnlFirstCard.BringToFront();
            }
            else if (cmbFirstPaymentMethod.Text == "Bank Transfer")
            {
                pnlFirstBank.Visible = true;
                pnlFirstBank.BringToFront();
            }
            // For Cash, both panels remain hidden

            // Clear digits when switching methods
            if (cmbFirstPaymentMethod.Text != "Card")
                txtFirstCardDigits.Clear();
            if (cmbFirstPaymentMethod.Text != "Bank Transfer")
                txtFirstBankDigits.Clear();

            // Force validation update
            UpdateMixedPaymentSummary();
        }

        private void cmbSecondPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetMixedPaymentEditingFlags();

            // First, hide all second payment method panels
            pnlSecondCard.Visible = false;
            pnlSecondBank.Visible = false;

            // Then show the appropriate panel
            if (cmbSecondPaymentMethod.Text == "Card")
            {
                pnlSecondCard.Visible = true;
                pnlSecondCard.BringToFront();
            }
            else if (cmbSecondPaymentMethod.Text == "Bank Transfer")
            {
                pnlSecondBank.Visible = true;
                pnlSecondBank.BringToFront();
            }
            // For Cash, both panels remain hidden

            // Clear digits when switching methods
            if (cmbSecondPaymentMethod.Text != "Card")
                txtSecondCardDigits.Clear();
            if (cmbSecondPaymentMethod.Text != "Bank Transfer")
                txtSecondBankDigits.Clear();

            // Force validation update
            UpdateMixedPaymentSummary();
        }

        private void UpdateMixedPaymentSummary()
        {
            decimal firstAmount = decimal.TryParse(txtFirstPaymentAmount.Text, out decimal first) ? first : 0;
            decimal secondAmount = decimal.TryParse(txtSecondPaymentAmount.Text, out decimal second) ? second : 0;
            decimal totalMixed = firstAmount + secondAmount;

            lblMixedTotal.Text = totalMixed.ToString("N2");
            lblMixedRemaining.Text = (_amountDue - totalMixed).ToString("N2");

            // Clear previous error
            lblMixedError.Text = "";
            lblMixedError.Visible = false;

            // Early validation - if no payment methods selected yet, don't show errors
            if (cmbFirstPaymentMethod.SelectedItem == null || cmbSecondPaymentMethod.SelectedItem == null)
            {
                return;
            }

            // Validate payment methods are different
            if (cmbFirstPaymentMethod.Text == cmbSecondPaymentMethod.Text)
            {
                ShowMixedError("Payment methods must be different");
                return;
            }

            // Validate amounts
            if (firstAmount <= 0)
            {
                ShowMixedError("First payment amount must be greater than 0");
                return;
            }

            if (secondAmount <= 0)
            {
                ShowMixedError("Second payment amount must be greater than 0");
                return;
            }

            if (totalMixed < _amountDue)
            {
                ShowMixedError($"Total payment (Rs.{totalMixed:N2}) is less than amount due (Rs.{_amountDue:N2})");
                return;
            }

            // Validate payment details
            string firstError = GetPaymentMethodError(cmbFirstPaymentMethod.Text, txtFirstCardDigits.Text, txtFirstBankDigits.Text, "first");
            if (!string.IsNullOrEmpty(firstError))
            {
                ShowMixedError(firstError);
                return;
            }

            string secondError = GetPaymentMethodError(cmbSecondPaymentMethod.Text, txtSecondCardDigits.Text, txtSecondBankDigits.Text, "second");
            if (!string.IsNullOrEmpty(secondError))
            {
                ShowMixedError(secondError);
                return;
            }

            // All validations passed
            lblMixedError.Visible = false;
        }

        private string GetPaymentMethodError(string method, string cardDigits, string bankDigits, string paymentPosition)
        {
            if (method == "Card")
            {
                if (string.IsNullOrWhiteSpace(cardDigits))
                {
                    return $"Please enter last 4 digits for {paymentPosition} payment (Card)";
                }
                if (cardDigits.Length != 4)
                {
                    return $"Please enter exactly 4 digits for {paymentPosition} payment (Card)";
                }
            }
            else if (method == "Bank Transfer")
            {
                if (string.IsNullOrWhiteSpace(bankDigits))
                {
                    return $"Please enter last 4 digits for {paymentPosition} payment (Bank Transfer)";
                }
                if (bankDigits.Length != 4)
                {
                    return $"Please enter exactly 4 digits for {paymentPosition} payment (Bank Transfer)";
                }
            }
            // Cash method doesn't require additional validation
            return null;
        }

        private string ValidatePaymentMethodDetails(string method, string cardDigits, string bankDigits, string paymentNumber)
        {
            if (method == "Card")
            {
                if (string.IsNullOrWhiteSpace(cardDigits) || cardDigits.Length != 4)
                {
                    return $"Please enter last 4 digits for {paymentNumber} payment (Card)";
                }
            }
            else if (method == "Bank Transfer")
            {
                if (string.IsNullOrWhiteSpace(bankDigits) || bankDigits.Length != 4)
                {
                    return $"Please enter last 4 digits for {paymentNumber} payment (Bank Transfer)";
                }
            }
            // Cash method doesn't require additional validation

            return null; // No error
        }

        private void ShowMixedError(string message)
        {
            lblMixedError.Text = message;
            lblMixedError.Visible = true;
        }

        private void txtFirstPaymentAmount_TextChanged(object sender, EventArgs e)
        {
            if (_updatingFirstAmount) return;

            // Track user interaction
            if (txtFirstPaymentAmount.Focused)
            {
                _userEditingFirst = true;
                _userEditingSecond = false;
            }

            // Auto-fill second amount only if user is actively editing first amount
            if (_userEditingFirst && !_userEditingSecond && txtFirstPaymentAmount.Focused)
            {
                if (decimal.TryParse(txtFirstPaymentAmount.Text, out decimal firstAmount) && firstAmount > 0)
                {
                    decimal remaining = _amountDue - firstAmount;
                    if (remaining >= 0)
                    {
                        _updatingSecondAmount = true;
                        txtSecondPaymentAmount.Text = remaining.ToString("0.00");
                        _updatingSecondAmount = false;
                    }
                    else
                    {
                        // If first amount exceeds due, set second to 0
                        _updatingSecondAmount = true;
                        txtSecondPaymentAmount.Text = "0.00";
                        _updatingSecondAmount = false;
                    }
                }
            }

            // Always update validation
            UpdateMixedPaymentSummary();
        }


        private void txtSecondPaymentAmount_TextChanged(object sender, EventArgs e)
        {
            if (_updatingSecondAmount) return;

            // Track user interaction
            if (txtSecondPaymentAmount.Focused)
            {
                _userEditingSecond = true;
                _userEditingFirst = false;
            }

            // Auto-fill first amount only if user is actively editing second amount
            if (_userEditingSecond && !_userEditingFirst && txtSecondPaymentAmount.Focused)
            {
                if (decimal.TryParse(txtSecondPaymentAmount.Text, out decimal secondAmount) && secondAmount > 0)
                {
                    decimal remaining = _amountDue - secondAmount;
                    if (remaining >= 0)
                    {
                        _updatingFirstAmount = true;
                        txtFirstPaymentAmount.Text = remaining.ToString("0.00");
                        _updatingFirstAmount = false;
                    }
                    else
                    {
                        // If second amount exceeds due, set first to 0
                        _updatingFirstAmount = true;
                        txtFirstPaymentAmount.Text = "0.00";
                        _updatingFirstAmount = false;
                    }
                }
            }

            // Always update validation
            UpdateMixedPaymentSummary();
        }


        // Add focus events to better track user intent
        private void txtFirstPaymentAmount_Enter(object sender, EventArgs e)
        {
            _userEditingFirst = true;
            _userEditingSecond = false;
        }

        private void txtSecondPaymentAmount_Enter(object sender, EventArgs e)
        {
            _userEditingSecond = true;
            _userEditingFirst = false;
        }

        private void txtFirstCardDigits_Enter(object sender, EventArgs e)
        {
            // When user focuses on digits, update validation immediately
            UpdateMixedPaymentSummary();
        }

        private void txtFirstBankDigits_Enter(object sender, EventArgs e)
        {
            // When user focuses on digits, update validation immediately
            UpdateMixedPaymentSummary();
        }

        private void txtSecondCardDigits_Enter(object sender, EventArgs e)
        {
            // When user focuses on digits, update validation immediately
            UpdateMixedPaymentSummary();
        }

        private void txtSecondBankDigits_Enter(object sender, EventArgs e)
        {
            // When user focuses on digits, update validation immediately
            UpdateMixedPaymentSummary();
        }

        private void ResetMixedPaymentEditingFlags()
        {
            _userEditingFirst = false;
            _userEditingSecond = false;
            _updatingFirstAmount = false;
            _updatingSecondAmount = false;
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

        private void ValidateDigitsTextBox(TextBox textBox)
        {
            string input = textBox.Text;
            string digitsOnly = new string(input.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length > 4)
            {
                digitsOnly = digitsOnly.Substring(0, 4);
            }

            if (digitsOnly != input)
            {
                int selectionStart = textBox.SelectionStart;
                textBox.Text = digitsOnly;
                textBox.SelectionStart = selectionStart > digitsOnly.Length
                    ? digitsOnly.Length
                    : selectionStart;
            }
        }

        private void txtCardDigits_TextChanged(object sender, EventArgs e)
        {
            ValidateDigitsTextBox(txtCardDigits);
            btnComplete.Enabled = txtCardDigits.Text.Length == 4;
        }

        private void txtBankDigits_TextChanged(object sender, EventArgs e)
        {
            ValidateDigitsTextBox(txtBankDigits);
            btnComplete.Enabled = txtBankDigits.Text.Length == 4;
        }

        private void txtFirstCardDigits_TextChanged(object sender, EventArgs e)
        {
            ValidateDigitsTextBox(txtFirstCardDigits);
            // Trigger validation immediately when digits are entered
            UpdateMixedPaymentSummary();
            UpdateCompleteButtonState();
        }

        private void txtFirstBankDigits_TextChanged(object sender, EventArgs e)
        {
            ValidateDigitsTextBox(txtFirstBankDigits);
            // Trigger validation immediately when digits are entered
            UpdateMixedPaymentSummary();
            UpdateCompleteButtonState();
        }

        private void txtSecondCardDigits_TextChanged(object sender, EventArgs e)
        {
            ValidateDigitsTextBox(txtSecondCardDigits);
            // Trigger validation immediately when digits are entered
            UpdateMixedPaymentSummary();
            UpdateCompleteButtonState();
        }

        private void txtSecondBankDigits_TextChanged(object sender, EventArgs e)
        {
            ValidateDigitsTextBox(txtSecondBankDigits);
            // Trigger validation immediately when digits are entered
            UpdateMixedPaymentSummary();
            UpdateCompleteButtonState();
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

            if (hasValidContact)
            {
                lblGender.ForeColor = Color.Red;
                lblGender.Text = "Gender:*";
                cmbGender.BackColor = Color.LightYellow;
            }
            else
            {
                lblGender.ForeColor = SystemColors.ControlText;
                lblGender.Text = "Gender:";
                cmbGender.BackColor = SystemColors.Window;
                cmbGender.SelectedIndex = -1;
            }

            ValidateContact();
            UpdateCompleteButtonState();
        }

        private void ValidateContact()
        {
            string contact = txtCustomerContact.Text;

            if (string.IsNullOrEmpty(contact))
            {
                lblContactError.Visible = false;
                lblContactError.Text = "";
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
            }

            UpdateCompleteButtonState();
        }

        private void UpdateCompleteButtonState()
        {
            bool contactValid = string.IsNullOrEmpty(txtCustomerContact.Text) ||
                               txtCustomerContact.Text.Length == 10;

            bool genderValid = string.IsNullOrEmpty(txtCustomerContact.Text) ||
                              (cmbGender.Enabled && cmbGender.SelectedIndex >= 0);

            bool paymentValid = false;

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
            else if (cmbPaymentMethod.Text == "Mixed")
            {
                // Use the same validation as UpdateMixedPaymentSummary
                paymentValid = IsMixedPaymentValid();
            }
            else if (_amountDue == 0)
            {
                // Token covers full amount or zero amount due
                paymentValid = true;
            }

            btnComplete.Enabled = contactValid && genderValid && paymentValid;
        }

        private bool IsMixedPaymentValid()
        {
            // Check if both payment methods are selected
            if (cmbFirstPaymentMethod.SelectedItem == null || cmbSecondPaymentMethod.SelectedItem == null)
                return false;

            // Check if payment methods are different
            if (cmbFirstPaymentMethod.Text == cmbSecondPaymentMethod.Text)
                return false;

            // Check amounts
            if (!decimal.TryParse(txtFirstPaymentAmount.Text, out decimal firstAmount) ||
                !decimal.TryParse(txtSecondPaymentAmount.Text, out decimal secondAmount))
                return false;

            if (firstAmount <= 0 || secondAmount <= 0)
                return false;

            if (firstAmount + secondAmount < _amountDue)
                return false;

            // Check payment details
            if (!ValidatePaymentDetails(cmbFirstPaymentMethod.Text, txtFirstCardDigits.Text, txtFirstBankDigits.Text))
                return false;

            if (!ValidatePaymentDetails(cmbSecondPaymentMethod.Text, txtSecondCardDigits.Text, txtSecondBankDigits.Text))
                return false;

            return true;
        }

        private bool ValidatePaymentDetails(string method, string cardDigits, string bankDigits)
        {
            if (method == "Card")
                return !string.IsNullOrWhiteSpace(cardDigits) && cardDigits.Length == 4;
            else if (method == "Bank Transfer")
                return !string.IsNullOrWhiteSpace(bankDigits) && bankDigits.Length == 4;
            else // Cash
                return true;
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            if (!ValidatePayment())
                return;

            SetPaymentProperties();
            IsConfirmed = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool ValidatePayment()
        {
            string contact = txtCustomerContact.Text;

            // Validate contact
            if (!string.IsNullOrEmpty(contact) && contact.Length != 10)
            {
                MessageBox.Show("Customer contact must be 10 digits if provided",
                                "Validation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return false;
            }

            // Validate gender if contact provided
            if (!string.IsNullOrEmpty(contact) && cmbGender.SelectedIndex < 0)
            {
                MessageBox.Show("Please select customer gender",
                                "Validation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return false;
            }

            // Validate mixed payment
            if (cmbPaymentMethod.Text == "Mixed")
            {
                decimal firstAmount = decimal.Parse(txtFirstPaymentAmount.Text);
                decimal secondAmount = decimal.Parse(txtSecondPaymentAmount.Text);
                decimal totalMixed = firstAmount + secondAmount;

                if (totalMixed < _amountDue)
                {
                    MessageBox.Show($"Mixed payment total (Rs.{totalMixed:N2}) is less than amount due (Rs.{_amountDue:N2})",
                                    "Validation Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return false;
                }

                if (cmbFirstPaymentMethod.Text == cmbSecondPaymentMethod.Text)
                {
                    MessageBox.Show("Mixed payment requires two different payment methods",
                                    "Validation Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void SetPaymentProperties()
        {
            PaymentMethod = cmbPaymentMethod.Text;
            CustomerContact = string.IsNullOrEmpty(txtCustomerContact.Text) ? null : txtCustomerContact.Text;
            CustomerGender = cmbGender.Enabled && cmbGender.SelectedItem != null
                ? cmbGender.SelectedItem.ToString()
                : null;

            if (PaymentMethod == "Mixed")
            {
                // Set mixed payment properties
                FirstPaymentMethod = cmbFirstPaymentMethod.Text;
                FirstPaymentAmount = decimal.Parse(txtFirstPaymentAmount.Text);
                SecondPaymentMethod = cmbSecondPaymentMethod.Text;
                SecondPaymentAmount = decimal.Parse(txtSecondPaymentAmount.Text);

                FirstCardLastFour = FirstPaymentMethod == "Card" ? txtFirstCardDigits.Text : null;
                FirstBankLastFour = FirstPaymentMethod == "Bank Transfer" ? txtFirstBankDigits.Text : null;
                SecondCardLastFour = SecondPaymentMethod == "Card" ? txtSecondCardDigits.Text : null;
                SecondBankLastFour = SecondPaymentMethod == "Bank Transfer" ? txtSecondBankDigits.Text : null;

                AmountTendered = FirstPaymentAmount + SecondPaymentAmount;
                Change = AmountTendered - _amountDue;
            }
            else
            {
                // Set single payment properties
                AmountTendered = PaymentMethod == "Cash" ?
                    decimal.Parse(txtCashTendered.Text) : _amountDue;

                CardLastFour = PaymentMethod == "Card" ? txtCardDigits.Text : null;
                BankLastFour = PaymentMethod == "Bank Transfer" ? txtBankDigits.Text : null;

                Change = PaymentMethod == "Cash" ?
                    decimal.Parse(lblChange.Text.Replace("Rs.", "").Replace("(", "").Replace(")", "")) : 0;

                // Clear mixed payment properties
                FirstPaymentMethod = null;
                FirstPaymentAmount = 0;
                FirstCardLastFour = null;
                FirstBankLastFour = null;
                SecondPaymentMethod = null;
                SecondPaymentAmount = 0;
                SecondCardLastFour = null;
                SecondBankLastFour = null;
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
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
            this.AcceptButton = btnComplete;
        }

        private void DigitsOnlyTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DecimalTextBox_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtCustomerContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}