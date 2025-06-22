using System;
using System.ComponentModel;
using System.Linq;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class QuantityForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedQuantity { get; private set; }

        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color LightBlue = Color.FromArgb(189, 224, 254);
        private readonly Color White = Color.White;
        private readonly Color DarkText = Color.FromArgb(64, 64, 64);
        private readonly Color LightGray = Color.FromArgb(240, 240, 240);

        public QuantityForm(int maxQuantity, int currentQuantity)
        {
            InitializeUI(maxQuantity, currentQuantity);
        }

        private void InitializeUI(int maxQuantity, int currentQuantity)
        {
            this.Size = new Size(350, 240);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Set Quantity";
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = White;
            this.Padding = new Padding(15);
            this.Font = new Font("Segoe UI", 10);

            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = White
            };
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            var headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = PrimaryBlue,
                Padding = new Padding(10, 5, 10, 5)
            };

            var lblInfo = new Label
            {
                Text = $"Set quantity (1-{maxQuantity}):",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = White,
                Padding = new Padding(0, 5, 0, 0)
            };
            headerPanel.Controls.Add(lblInfo);
            container.Controls.Add(headerPanel, 0, 0);

            var inputPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = White,
                Padding = new Padding(10, 15, 10, 5)
            };

            var numQuantity = new NumericUpDown
            {
                Minimum = 1,
                Maximum = maxQuantity,
                Value = currentQuantity,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 14),
                TextAlign = HorizontalAlignment.Center,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = LightGray,
                ForeColor = DarkText
            };
            inputPanel.Controls.Add(numQuantity);
            container.Controls.Add(inputPanel, 0, 1);

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = White,
                Padding = new Padding(10, 5, 10, 10)
            };

            var buttonLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                ColumnStyles = {
                    new ColumnStyle(SizeType.Percent, 50F),
                    new ColumnStyle(SizeType.Percent, 50F)
                },
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0, 0, 0, 0),
                Margin = new Padding(0)
            };

            var btnCancel = new Button
            {
                Text = "CANCEL",
                Dock = DockStyle.Fill,
                BackColor = LightGray,
                ForeColor = DarkText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 5, 0)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            var btnOK = new Button
            {
                Text = "CONFIRM",
                Dock = DockStyle.Fill,
                BackColor = PrimaryBlue,
                ForeColor = White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(5, 0, 0, 0)
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += (s, e) =>
            {
                SelectedQuantity = (int)numQuantity.Value;
                this.DialogResult = DialogResult.OK;
            };

            buttonLayout.Controls.Add(btnCancel, 0, 0);
            buttonLayout.Controls.Add(btnOK, 1, 0);
            buttonPanel.Controls.Add(buttonLayout);
            container.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Add(container);
            numQuantity.Select();
            numQuantity.Select(0, numQuantity.Text.Length);
        }
    }
}
