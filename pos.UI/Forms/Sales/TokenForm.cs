using pos_system.pos.BLL.Services;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class TokenForm : Form
    {
        private readonly SalesService _salesService;
        public ReturnToken SelectedToken { get; private set; }

        public TokenForm()
        {
            InitializeComponent();
            _salesService = new SalesService();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form setup
            this.Size = new Size(430, 230);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Apply Return Token";
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main container
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20),
                RowCount = 3,
                ColumnCount = 2
            };
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Token ID Label
            var lblTokenId = new Label
            {
                Text = "Token ID:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            // Token ID TextBox
            var txtTokenId = new TextBox
            {
                Font = new Font("Segoe UI", 14),
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 5, 10, 5)
            };

            // Buttons panel
            var buttonPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 10, 0, 0)
            };
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            // Apply Button
            var btnApply = new Button
            {
                Text = "APPLY TOKEN",
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                Height = 40
            };
            btnApply.FlatAppearance.BorderSize = 0;

            // Cancel Button
            var btnCancel = new Button
            {
                Text = "CANCEL",
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                Height = 40
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            // Message Label
            var lblMessage = new Label
            {
                Text = "Enter the return token ID to apply its value to this bill",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            // Add controls to containers
            container.Controls.Add(lblTokenId, 0, 0);
            container.Controls.Add(txtTokenId, 1, 0);
            container.Controls.Add(lblMessage, 0, 1);
            container.SetColumnSpan(lblMessage, 2);
            container.Controls.Add(buttonPanel, 0, 2);
            container.SetColumnSpan(buttonPanel, 2);

            buttonPanel.Controls.Add(btnApply, 0, 0);
            buttonPanel.Controls.Add(btnCancel, 1, 0);

            this.Controls.Add(container);

            // Event handlers
            btnApply.Click += (s, e) => ApplyToken(txtTokenId.Text);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            txtTokenId.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    ApplyToken(txtTokenId.Text);
                    e.Handled = true;
                }
            };

            this.ResumeLayout();
        }

        private void ApplyToken(string tokenIdText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tokenIdText))
                {
                    MessageBox.Show("Please enter a token ID", "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(tokenIdText, out int tokenId))
                {
                    MessageBox.Show("Invalid token format. Please enter a valid token ID.",
                        "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SelectedToken = _salesService.GetTokenDetails(tokenId);

                if (SelectedToken == null)
                {
                    MessageBox.Show("Token not found", "Token Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (SelectedToken.IsUsed)
                {
                    MessageBox.Show("Token has already been used", "Token Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying token: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
