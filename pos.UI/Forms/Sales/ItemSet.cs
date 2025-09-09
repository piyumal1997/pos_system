using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class ItemSet : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedQuantity { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal SelectedDiscountPerItem { get; private set; }

        private readonly decimal _unitPrice;
        private readonly decimal _unitCost;
        private readonly int _availableStock;
        private readonly decimal _maxDiscount;
        private NumericUpDown _numQuantity;
        private NumericUpDown _numNetPrice;

        public ItemSet(string description, string barcode, string brand, string category,
                       string size, int availableStock, decimal unitPrice, decimal unitCost,
                       decimal maxDiscount, int currentQuantity = 1, decimal currentDiscount = 0)
        {
            InitializeComponent();
            _unitPrice = unitPrice;
            _unitCost = unitCost;
            _availableStock = availableStock;
            _maxDiscount = maxDiscount;
            SelectedQuantity = currentQuantity;
            SelectedDiscountPerItem = currentDiscount;

            // Form setup
            this.Text = "Set Item for Billing";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(480, 420);
            this.BackColor = Color.White;
            this.Padding = new Padding(10);

            InitializeUI(description, barcode, brand, category, size);
        }

        private void InitializeUI(string description, string barcode, string brand,
                                  string category, string size)
        {
            // Main table layout
            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 10,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(5)
            };
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            for (int i = 0; i < 6; i++)
                mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));

            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Row 6 - Cost
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Row 7 - Selling
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Row 8 - Quantity
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Row 9 - Net Price

            // Title
            var lblTitle = new Label
            {
                Text = "Item Details and Set Item",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 35,
                ForeColor = Color.SteelBlue,
                Margin = new Padding(0, 10, 0, 10)
            };
            this.Controls.Add(lblTitle);

            // Add item information rows
            AddInfoRow(mainTable, "Description:", description, 0);
            AddInfoRow(mainTable, "Barcode:", barcode, 1);
            AddInfoRow(mainTable, "Brand:", brand, 2);
            AddInfoRow(mainTable, "Category:", category, 3);
            AddInfoRow(mainTable, "Size:", size, 4);
            AddInfoRow(mainTable, "Available Stock:", _availableStock.ToString(), 5);

            // Unit Cost row - highlighted with large font
            var lblCostTitle = new Label
            {
                Text = "Cost Price:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            mainTable.Controls.Add(lblCostTitle, 0, 6);

            var lblCostValue = new Label
            {
                Text = $"{ConvertNumberToCode(Convert.ToInt32(_unitCost).ToString())}",
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            mainTable.Controls.Add(lblCostValue, 1, 6);

            // Unit Price row - highlighted with large font
            var lblPriceTitle = new Label
            {
                Text = "Selling Price:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            mainTable.Controls.Add(lblPriceTitle, 0, 7);

            var lblPriceValue = new Label
            {
                Text = $"Rs.{_unitPrice:N2}",
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
            };
            mainTable.Controls.Add(lblPriceValue, 1, 7);

            // Quantity row
            var lblQuantity = new Label
            {
                Text = "Quantity:",
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            mainTable.Controls.Add(lblQuantity, 0, 8);

            _numQuantity = new NumericUpDown
            {
                Minimum = 1,
                Maximum = _availableStock,
                Value = SelectedQuantity,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                DecimalPlaces = 0
            };

            // Add text selection behavior
            _numQuantity.Enter += (s, e) => {
                _numQuantity.Select(0, _numQuantity.Text.Length);
            };
            _numQuantity.GotFocus += (s, e) => {
                _numQuantity.Select(0, _numQuantity.Text.Length);
            };

            mainTable.Controls.Add(_numQuantity, 1, 8);

            // Net Price row
            var lblNetPrice = new Label
            {
                Text = "Net Price:",
                TextAlign = ContentAlignment.TopRight,
                Padding = new Padding(0, 5, 0, 0),
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            mainTable.Controls.Add(lblNetPrice, 0, 9);

            // Calculate initial net price
            decimal initialNetPrice = _unitPrice - SelectedDiscountPerItem;
            decimal minNetPrice = Math.Max(_unitCost, _unitPrice - _maxDiscount);

            _numNetPrice = new NumericUpDown
            {
                Minimum = minNetPrice,
                Maximum = _unitPrice,
                Value = initialNetPrice,
                DecimalPlaces = 2,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Increment = 0.1m
            };

            // Add text selection behavior
            _numNetPrice.Enter += (s, e) => {
                _numNetPrice.Select(0, _numNetPrice.Text.Length);
            };
            _numNetPrice.GotFocus += (s, e) => {
                _numNetPrice.Select(0, _numNetPrice.Text.Length);
            };

            // Add validation
            _numNetPrice.ValueChanged += (s, e) => ValidateNetPrice();

            mainTable.Controls.Add(_numNetPrice, 1, 9);

            // Button panel
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(0, 10, 0, 0)
            };

            var btnCancel = new System.Windows.Forms.Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                DialogResult = DialogResult.Cancel,
                Size = new Size(80, 35),
                Location = new Point(220, 10),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            var btnOK = new System.Windows.Forms.Button
            {
                Text = "OK",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(80, 35),
                Location = new Point(310, 10),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnOK);

            // Add controls to form
            this.Controls.Add(mainTable);
            this.Controls.Add(buttonPanel);

            btnOK.Click += (s, e) =>
            {
                SelectedQuantity = (int)_numQuantity.Value;
                SelectedDiscountPerItem = _unitPrice - _numNetPrice.Value;
            };

            // Set focus to quantity control
            _numQuantity.Select();
            _numQuantity.Select(0, _numQuantity.Text.Length);

            // Keyboard handling
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnOK.PerformClick();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    btnCancel.PerformClick();
                }
                else if (e.KeyCode == Keys.Tab && _numNetPrice.Focused)
                {
                    btnOK.Focus();
                    e.Handled = true;
                }
            };

            // Initial validation
            ValidateNetPrice();
        }



        private void AddInfoRow(TableLayoutPanel panel, string label, string value, int row)
        {
            var lbl = new Label
            {
                Text = label,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 5, 5)
            };

            var val = new Label
            {
                Text = value,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            panel.Controls.Add(lbl, 0, row);
            panel.Controls.Add(val, 1, row);
        }

        public static string ConvertNumberToCode(string number)
        {
            if (string.IsNullOrEmpty(number))
                return "XXXX";

            StringBuilder result = new StringBuilder();
            foreach (char digit in number)
            {
                switch (digit)
                {
                    case '0': result.Append('X'); break;
                    case '1': result.Append('A'); break;
                    case '2': result.Append('B'); break;
                    case '3': result.Append('C'); break;
                    case '4': result.Append('D'); break;
                    case '5': result.Append('E'); break;
                    case '6': result.Append('F'); break;
                    case '7': result.Append('G'); break;
                    case '8': result.Append('H'); break;
                    case '9': result.Append('I'); break;
                    default: result.Append('?'); break;
                }
            }
            return result.ToString();
        }

        private void ValidateNetPrice()
        {
            // Validate net price
            if (_numNetPrice.Value < _unitCost)
            {
                _numNetPrice.ForeColor = Color.Red;
            }
            else
            {
                _numNetPrice.ForeColor = Color.Black;
            }
        }
    }
}