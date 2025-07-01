using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class DiscountAmountForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal SelectedDiscountAmount { get; private set; }

        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color LightBlue = Color.FromArgb(189, 224, 254);
        private readonly Color White = Color.White;
        private readonly Color DarkText = Color.FromArgb(64, 64, 64);
        private readonly Color ButtonHoverColor = Color.FromArgb(31, 118, 175);

        private Panel topPanel;
        private Label lblTitle;
        private Button btnClose;
        private NumericUpDown numDiscountAmount;

        public DiscountAmountForm(decimal maxDiscountAmount, decimal itemPrice, decimal currentDiscount = 0)
        {
            InitializeUI(maxDiscountAmount, itemPrice, currentDiscount);
        }

        private void InitializeUI(decimal maxDiscountAmount, decimal itemPrice, decimal currentDiscount)
        {
            // ============== FORM SETUP ==============
            this.Size = new Size(500, 300);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Apply Item Discount";
            this.BackColor = White;
            this.Padding = new Padding(0);
            this.Font = new Font("Segoe UI", 10);
            this.KeyPreview = true;

            // Main container
            var mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = White };
            this.Controls.Add(mainPanel);

            // ============== TITLE BAR ==============
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = PrimaryBlue,
                Padding = new Padding(0)
            };
            mainPanel.Controls.Add(topPanel);

            // Title label
            lblTitle = new Label
            {
                Text = "APPLY ITEM DISCOUNT",
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = White,
                Padding = new Padding(10, 10, 0, 0),
                Height = topPanel.Height,
                AutoSize = true,
                Width = 250
            };
            topPanel.Controls.Add(lblTitle);

            // Close button
            var flowLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };
            btnClose = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 12),
                ForeColor = White,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Size = new Size(40, 40),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
            btnClose.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            flowLayout.Controls.Add(btnClose);
            topPanel.Controls.Add(flowLayout);

            // ============== CONTENT CONTAINER ==============
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 20, 20, 20),
                BackColor = White
            };
            mainPanel.Controls.Add(container);

            // Item price info
            var lblPriceInfo = new Label
            {
                Text = $"Item Price: Rs.{itemPrice:N2}",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                Height = 30
            };
            container.Controls.Add(lblPriceInfo);

            // Max discount info
            var lblMaxDiscount = new Label
            {
                Text = $"Maximum Discount: Rs.{maxDiscountAmount:N2}",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DarkRed,
                Height = 25
            };
            container.Controls.Add(lblMaxDiscount);

            // Input panel
            var inputPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(0, 10, 0, 0)
            };

            var lblDiscount = new Label
            {
                Text = "Discount Amount (Rs.):",
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10),
                ForeColor = DarkText,
                Width = 180
            };
            inputPanel.Controls.Add(lblDiscount);

            numDiscountAmount = new NumericUpDown
            {
                Minimum = 0,
                Maximum = maxDiscountAmount,
                DecimalPlaces = 2,
                Value = currentDiscount,
                Dock = DockStyle.Right,
                Font = new Font("Segoe UI", 14),
                TextAlign = HorizontalAlignment.Right,
                Width = 150
            };
            inputPanel.Controls.Add(numDiscountAmount);
            container.Controls.Add(inputPanel);

            // Action buttons
            var actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 50
            };

            var btnOK = new Button
            {
                Text = "APPLY",
                Size = new Size(120, 40),
                BackColor = PrimaryBlue,
                ForeColor = White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            btnOK.Click += (s, e) => SelectedDiscountAmount = numDiscountAmount.Value;

            var btnCancel = new Button
            {
                Text = "CANCEL",
                Size = new Size(120, 40),
                BackColor = Color.LightGray,
                ForeColor = DarkText,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                DialogResult = DialogResult.Cancel,
                Margin = new Padding(0, 0, 10, 0),
                Cursor = Cursors.Hand
            };

            actionPanel.Controls.Add(btnOK);
            actionPanel.Controls.Add(btnCancel);
            container.Controls.Add(actionPanel);

            // ============== BEHAVIOR CONFIG ==============
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            numDiscountAmount.Select();
            numDiscountAmount.Select(0, numDiscountAmount.Text.Length);

            // Enable form dragging
            topPanel.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, 0xA1, 0x2, 0);
                }
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter && numDiscountAmount.Focused)
            {
                this.DialogResult = DialogResult.OK;
                SelectedDiscountAmount = numDiscountAmount.Value;
                this.Close();
                e.Handled = true;
            }
        }

        // ============== WINDOWS DRAGGING API ==============
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
    }
}