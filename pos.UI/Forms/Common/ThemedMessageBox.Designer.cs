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
    public enum ThemedMessageBoxIcon
    {
        None,
        Error,
        Information,
        Warning,
        Question
    }

    public partial class ThemedMessageBox : Form
    {
        // Layout constants
        private const int HorizontalPadding = 30;
        private const int VerticalPadding = 20;
        private const int VerticalSpacing = 15;
        private const int MaxWidth = 500;
        private const int MinWidth = 300;
        private const int MinHeight = 150;
        private const int MaxLabelHeight = 300;
        private const int TitleHeight = 40;

        private Button btnOK;
        private Label lblMessage;
        private Label lblTitle;
        private ThemedMessageBoxIcon _icon;

        public ThemedMessageBox(string message, string title = "", ThemedMessageBoxIcon icon = ThemedMessageBoxIcon.None)
        {
            InitializeComponent();
            lblMessage.Text = message;
            lblTitle.Text = title;
            _icon = icon;
            SetTitleBarColor();
            AdjustLayout();
        }

        private void SetTitleBarColor()
        {
            switch (_icon)
            {
                case ThemedMessageBoxIcon.Error:
                    lblTitle.BackColor = Color.FromArgb(220, 100, 100);  // Red
                    break;
                case ThemedMessageBoxIcon.Information:
                    lblTitle.BackColor = Color.FromArgb(41, 128, 185);   // Blue
                    break;
                case ThemedMessageBoxIcon.Warning:
                    lblTitle.BackColor = Color.FromArgb(255, 193, 7);    // Amber
                    break;
                case ThemedMessageBoxIcon.Question:
                    lblTitle.BackColor = Color.FromArgb(106, 176, 76);   // Green
                    break;
                default:
                    lblTitle.BackColor = Color.FromArgb(180, 200, 220);  // Original light blue
                    break;
            }
        }

        private void InitializeComponent()
        {
            // Set up form properties
            BackColor = Color.FromArgb(221, 226, 235);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(0, 0, 0, 10);
            ShowInTaskbar = false;

            // Title label - now with dynamic color
            lblTitle = new Label();
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.Black;  // White text for better contrast
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = TitleHeight;
            lblTitle.Padding = new Padding(0, 5, 0, 5);

            // Message label
            lblMessage = new Label();
            lblMessage.Font = new Font("Segoe UI", 12F);
            lblMessage.ForeColor = Color.FromArgb(64, 64, 64);
            lblMessage.TextAlign = ContentAlignment.MiddleLeft;
            lblMessage.AutoSize = false;
            lblMessage.BackColor = Color.Transparent;

            // OK button
            btnOK = new Button();
            btnOK.BackColor = Color.FromArgb(41, 128, 185);
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Font = new Font("Segoe UI", 12F);
            btnOK.ForeColor = Color.White;
            btnOK.Size = new Size(100, 40);
            btnOK.Text = "OK";
            btnOK.Click += (s, e) => this.Close();

            // Add controls to form
            Controls.Add(lblTitle);
            Controls.Add(lblMessage);
            Controls.Add(btnOK);
        }

        private void AdjustLayout()
        {
            using (Graphics g = CreateGraphics())
            {
                // Calculate available text width
                int maxTextWidth = MaxWidth - 2 * HorizontalPadding;
                Size maxSize = new Size(maxTextWidth, MaxLabelHeight);

                // Measure message text
                SizeF messageSizeF = g.MeasureString(
                    lblMessage.Text,
                    lblMessage.Font,
                    maxSize.Width,
                    new StringFormat(StringFormatFlags.LineLimit)
                );

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
                                VerticalSpacing + btnOK.Height + VerticalPadding;
                formHeight = Math.Max(MinHeight, formHeight);

                // Set form size
                ClientSize = new Size(formWidth, formHeight);

                // Position message
                lblMessage.Location = new Point(
                    HorizontalPadding,
                    TitleHeight + VerticalPadding
                );
                lblMessage.Size = new Size(
                    Math.Min(messageSize.Width, maxTextWidth),
                    messageSize.Height
                );
            }

            // Position button
            btnOK.Location = new Point(
                (ClientSize.Width - btnOK.Width) / 2,
                ClientSize.Height - btnOK.Height - VerticalPadding
            );

            // Ensure title stays on top
            lblTitle.BringToFront();
        }

        public static DialogResult Show(string text, string caption = "", ThemedMessageBoxIcon icon = ThemedMessageBoxIcon.None)
        {
            using (var msgBox = new pos_system.pos.UI.Forms.Common.ThemedMessageBox(text, caption, icon))
            {
                return msgBox.ShowDialog();
            }
        }

        // Rounded corners for modern look
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Apply rounded corners
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        // Add keyboard shortcuts
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter || keyData == Keys.Escape || keyData == Keys.Space)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}