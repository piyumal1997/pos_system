using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using pos_system.pos.Core; // For DropShadow

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class BillDiscountForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal SelectedDiscount { get; private set; }

        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color LightBlue = Color.FromArgb(189, 224, 254);
        private readonly Color White = Color.White;
        private readonly Color DarkText = Color.FromArgb(64, 64, 64);
        private readonly Color LightGray = Color.FromArgb(240, 240, 240);
        private readonly Color ButtonHoverColor = Color.FromArgb(31, 118, 175);

        private Panel topPanel;
        private Label lblTitle;
        private Button btnClose;
        private NumericUpDown numDiscount;

        public BillDiscountForm(decimal maxDiscount = 100, decimal initialDiscount = 0)
        {
            InitializeUI(maxDiscount, initialDiscount);
            new DropShadow().ApplyShadows(this);
        }

        private void InitializeUI(decimal maxDiscount, decimal initialDiscount)
        {
            // ============== FORM SETUP ==============
            this.Size = new Size(500, 340);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Apply Bill Discount";
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
                Text = "APPLY BILL DISCOUNT",
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

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = White
            };
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F)); // Instruction
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // Quick buttons
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F)); // Input
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15F)); // Actions
            container.Controls.Add(tableLayout);

            // ============== INSTRUCTION LABEL ==============
            var lblInstruction = new Label
            {
                Text = $"Enter bill discount percentage (0-{maxDiscount:F2}%):",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = DarkText,
                Padding = new Padding(0, 0, 0, 5)
            };
            tableLayout.Controls.Add(lblInstruction, 0, 0);

            // ============== QUICK BUTTONS ==============
            var buttonPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0, 10, 0, 10)
            };
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            CreateQuickButton(buttonPanel, "5%", 5, maxDiscount);
            CreateQuickButton(buttonPanel, "10%", 10, maxDiscount);
            CreateQuickButton(buttonPanel, "15%", 15, maxDiscount);
            CreateQuickButton(buttonPanel, "20%", 20, maxDiscount);
            tableLayout.Controls.Add(buttonPanel, 0, 1);

            // ============== INPUT FIELD ==============
            var inputPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 10)
            };
            numDiscount = new NumericUpDown
            {
                Minimum = 0,
                Maximum = maxDiscount,
                DecimalPlaces = 2,
                Value = initialDiscount,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 14),
                TextAlign = HorizontalAlignment.Center,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = LightGray,
                ForeColor = DarkText
            };
            inputPanel.Controls.Add(numDiscount);
            tableLayout.Controls.Add(inputPanel, 0, 2);

            // ============== ACTION BUTTONS ==============
            var actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };

            var btnOK = new Button
            {
                Text = "APPLY",
                Size = new Size(120, 40),
                BackColor = PrimaryBlue,
                ForeColor = White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0)
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.FlatAppearance.MouseOverBackColor = ButtonHoverColor;
            btnOK.Click += (s, e) => ApplyDiscount();

            var btnCancel = new Button
            {
                Text = "CANCEL",
                Size = new Size(120, 40),
                BackColor = LightGray,
                ForeColor = DarkText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0),
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            actionPanel.Controls.Add(btnOK);
            actionPanel.Controls.Add(btnCancel);
            tableLayout.Controls.Add(actionPanel, 0, 3);

            // ============== BEHAVIOR CONFIG ==============
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            numDiscount.Select();
            numDiscount.Select(0, numDiscount.Text.Length);

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

        private void CreateQuickButton(TableLayoutPanel panel, string text, decimal value, decimal maxDiscount)
        {
            var button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 0, 10, 0),
                Height = 50,
                BackColor = LightBlue,
                ForeColor = PrimaryBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Tag = value
            };
            button.FlatAppearance.BorderColor = PrimaryBlue;
            button.FlatAppearance.BorderSize = 1;
            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(170, 210, 254);
            button.MouseLeave += (s, e) => button.BackColor = LightBlue;

            if (value > maxDiscount)
            {
                button.Enabled = false;
                button.BackColor = Color.LightGray;
                button.ForeColor = Color.DarkGray;
            }

            button.Click += (s, e) =>
            {
                SelectedDiscount = value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            panel.Controls.Add(button);
        }

        private void ApplyDiscount()
        {
            SelectedDiscount = numDiscount.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter && numDiscount.Focused)
            {
                ApplyDiscount();
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