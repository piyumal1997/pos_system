using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using pos_system.pos.UI.Forms;
using pos_system;
using pos_system.pos;
using pos_system.pos.UI;

namespace pos_system.pos.UI.Forms.Common
{
    partial class ThemedMessageBoxYesNo
    {
        // Layout constants
        private const int HorizontalPadding = 30;
        private const int VerticalPadding = 20;
        private const int VerticalSpacing = 15;
        private const int ButtonSpacing = 15;
        private const int MaxWidth = 500;
        private const int MinWidth = 300;
        private const int MinHeight = 150;
        private const int MaxLabelHeight = 300;
        private const int TitleHeight = 40;
        private const int CornerRadius = 15;

        private Label lblTitle;
        private Button btnYes;
        private Button btnNo;
        private Label lblMessage;

        public ThemedMessageBoxYesNo(string message, string title, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            InitializeComponent();
            lblMessage.Text = message;
            lblTitle.Text = title;
            SetTitleBarColor(icon);
            AdjustLayout();
        }

        private void SetTitleBarColor(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    lblTitle.BackColor = Color.FromArgb(220, 100, 100);  // Red
                    break;
                case MessageBoxIcon.Information:
                    lblTitle.BackColor = Color.FromArgb(41, 128, 185);   // Blue
                    break;
                case MessageBoxIcon.Warning:
                    lblTitle.BackColor = Color.FromArgb(255, 193, 7);    // Amber
                    break;
                case MessageBoxIcon.Question:
                    lblTitle.BackColor = Color.FromArgb(106, 176, 76);   // Green
                    break;
                default:
                    lblTitle.BackColor = Color.FromArgb(180, 200, 220);  // Light blue
                    break;
            }
        }

        private void InitializeComponent()
        {
            // Create controls
            btnYes = new Button();
            btnNo = new Button();
            lblMessage = new Label();
            lblTitle = new Label();

            // Title label
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Black;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = TitleHeight;
            lblTitle.Padding = new Padding(0, 5, 0, 5);

            // Yes button
            btnYes.BackColor = Color.FromArgb(231, 76, 60); // Green
            btnYes.FlatAppearance.BorderSize = 0;
            btnYes.FlatStyle = FlatStyle.Flat;
            btnYes.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnYes.ForeColor = Color.White;
            btnYes.Size = new Size(90, 35);
            btnYes.Text = "Yes";
            btnYes.Click += (s, e) => { Result = DialogResult.Yes; Close(); };

            // No button
            btnNo.BackColor = Color.FromArgb(76, 175, 80); // Red
            btnNo.FlatAppearance.BorderSize = 0;
            btnNo.FlatStyle = FlatStyle.Flat;
            btnNo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnNo.ForeColor = Color.White;
            btnNo.Size = new Size(90, 35);
            btnNo.Text = "No";
            btnNo.Click += (s, e) => { Result = DialogResult.No; Close(); };

            // Message label
            lblMessage.Font = new Font("Segoe UI", 12F);
            lblMessage.ForeColor = Color.FromArgb(64, 64, 64);
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            lblMessage.AutoSize = false;

            // Form properties
            BackColor = Color.FromArgb(221, 226, 235);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            Padding = new Padding(0, 0, 0, 10);

            // Add controls
            Controls.Add(lblTitle);
            Controls.Add(lblMessage);
            Controls.Add(btnYes);
            Controls.Add(btnNo);
        }

        private void AdjustLayout()
        {
            using (Graphics g = CreateGraphics())
            {
                // Calculate maximum text area
                int maxTextWidth = MaxWidth - 2 * HorizontalPadding;
                Size maxSize = new Size(maxTextWidth, MaxLabelHeight);

                // Measure message text
                SizeF messageSizeF = g.MeasureString(
                    lblMessage.Text,
                    lblMessage.Font,
                    maxSize.Width,
                    new StringFormat(StringFormatFlags.LineLimit)
                );

                // Convert to integer size
                Size messageSize = new Size(
                    (int)Math.Ceiling(messageSizeF.Width),
                    (int)Math.Ceiling(messageSizeF.Height)
                );

                // Measure title text
                SizeF titleSizeF = g.MeasureString(
                    lblTitle.Text,
                    lblTitle.Font,
                    maxTextWidth,
                    StringFormat.GenericTypographic
                );

                // Determine form width
                int formWidth = Math.Max(
                    MinWidth,
                    Math.Min(
                        MaxWidth,
                        Math.Max(messageSize.Width, (int)titleSizeF.Width) + 2 * HorizontalPadding
                    )
                );

                // Determine form height
                int formHeight = TitleHeight + VerticalPadding + messageSize.Height +
                                VerticalSpacing + btnYes.Height + VerticalPadding;
                formHeight = Math.Max(MinHeight, formHeight);

                // Set form size
                ClientSize = new Size(formWidth, formHeight);

                // Position and size title
                lblTitle.Width = formWidth;

                // Position and size message
                lblMessage.Location = new Point(
                    (formWidth - messageSize.Width) / 2,
                    TitleHeight + VerticalPadding
                );
                lblMessage.Size = new Size(
                    Math.Min(messageSize.Width, maxTextWidth),
                    messageSize.Height
                );
            }

            // Position buttons
            int totalButtonWidth = btnYes.Width + btnNo.Width + ButtonSpacing;
            int buttonX = (ClientSize.Width - totalButtonWidth) / 2;
            int buttonY = ClientSize.Height - btnYes.Height - VerticalPadding;

            btnYes.Location = new Point(buttonX, buttonY);
            btnNo.Location = new Point(buttonX + btnYes.Width + ButtonSpacing, buttonY);
        }

        public DialogResult Result { get; private set; } = DialogResult.Cancel;

        // Rounded corners API
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Apply rounded corners
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, CornerRadius, CornerRadius));
        }

        // Keyboard shortcuts
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                btnYes.PerformClick();
                return true;
            }
            else if (keyData == Keys.Escape)
            {
                btnNo.PerformClick();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        public static DialogResult Show(string message, string title = "", MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (var form = new pos_system.pos.UI.Forms.Common.ThemedMessageBoxYesNo(message, title, icon))
            {
                form.ShowDialog();
                return form.Result;
            }
        }
    }
}