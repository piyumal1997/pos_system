using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using pos_system.pos.Core;

namespace pos_system.pos.UI.Forms.Common
{
    public partial class ThemedMessageBoxGreen : Form
    {
        // Layout constants
        private const int HorizontalPadding = 30;
        private const int VerticalPadding = 25;
        private const int VerticalSpacing = 20;
        private const int MaxWidth = 600;
        private const int MinWidth = 400;
        private const int MinHeight = 200;
        private const int MaxLabelHeight = 500;
        private const int TitleHeight = 60;

        // Green theme colors
        private readonly Color PrimaryGreen = Color.FromArgb(76, 175, 80);
        private readonly Color LightGreen = Color.FromArgb(232, 245, 233);
        private readonly Color DarkGreen = Color.FromArgb(56, 142, 60);
        private readonly Color ButtonGreen = Color.FromArgb(105, 192, 80);

        private Button btnOK;
        private Label lblMessage;
        private Label lblTitle;
        private ThemedMessageBoxIcon _icon;
        private PictureBox iconPicture;

        public ThemedMessageBoxGreen(string message, string title = "", ThemedMessageBoxIcon icon = ThemedMessageBoxIcon.None)
        {
            InitializeComponent();
            lblMessage.Text = message;
            lblTitle.Text = title;
            _icon = icon;
            SetIcon();
            AdjustLayout();
            new DropShadow().ApplyShadows(this);
        }

        private void SetIcon()
        {
            switch (_icon)
            {
                case ThemedMessageBoxIcon.Error:
                    iconPicture.Image = SystemIcons.Error.ToBitmap();
                    break;
                case ThemedMessageBoxIcon.Information:
                    iconPicture.Image = SystemIcons.Information.ToBitmap();
                    break;
                case ThemedMessageBoxIcon.Warning:
                    iconPicture.Image = SystemIcons.Warning.ToBitmap();
                    break;
                case ThemedMessageBoxIcon.Question:
                    iconPicture.Image = SystemIcons.Question.ToBitmap();
                    break;
                default:
                    iconPicture.Visible = false;
                    break;
            }
        }

        private void InitializeComponent()
        {
            // Set up form properties
            BackColor = LightGreen;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(0);
            ShowInTaskbar = false;

            // Title panel
            Panel titlePanel = new Panel();
            titlePanel.BackColor = PrimaryGreen;
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = TitleHeight;
            titlePanel.Padding = new Padding(20, 0, 20, 0);

            // Title label
            lblTitle = new Label();
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Padding = new Padding(0, 5, 0, 5);
            titlePanel.Controls.Add(lblTitle);

            // Icon picture box
            iconPicture = new PictureBox();
            iconPicture.SizeMode = PictureBoxSizeMode.Zoom;
            iconPicture.Size = new Size(32, 32);
            iconPicture.Location = new Point(HorizontalPadding, VerticalPadding);
            iconPicture.BackColor = Color.Transparent;

            // Message label
            lblMessage = new Label();
            lblMessage.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblMessage.ForeColor = Color.FromArgb(64, 64, 64);
            lblMessage.TextAlign = ContentAlignment.TopLeft;
            lblMessage.AutoSize = false;
            lblMessage.BackColor = Color.Transparent;

            // OK button
            btnOK = new Button();
            btnOK.BackColor = ButtonGreen;
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnOK.ForeColor = Color.White;
            btnOK.Size = new Size(120, 45);
            btnOK.Text = "OK";
            btnOK.Cursor = Cursors.Hand;
            btnOK.Click += (s, e) => this.Close();

            // Add hover effects to button
            btnOK.MouseEnter += (s, e) => btnOK.BackColor = DarkGreen;
            btnOK.MouseLeave += (s, e) => btnOK.BackColor = ButtonGreen;

            // Add controls to form
            Controls.Add(titlePanel);
            Controls.Add(iconPicture);
            Controls.Add(lblMessage);
            Controls.Add(btnOK);
        }

        private void AdjustLayout()
        {
            using (Graphics g = CreateGraphics())
            {
                // Calculate available text width
                int maxTextWidth = MaxWidth - 2 * HorizontalPadding - (iconPicture.Visible ? 50 : 0);
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

                // Determine form width
                int formWidth = Math.Max(
                    MinWidth,
                    Math.Min(
                        MaxWidth,
                        Math.Max(messageSize.Width, lblTitle.Width) + 2 * HorizontalPadding + (iconPicture.Visible ? 50 : 0)
                    )
                );

                // Determine form height
                int formHeight = TitleHeight + VerticalPadding + messageSize.Height +
                                VerticalSpacing + btnOK.Height + VerticalPadding;
                formHeight = Math.Max(MinHeight, formHeight);

                // Set form size
                ClientSize = new Size(formWidth, formHeight);

                // Position icon
                if (iconPicture.Visible)
                {
                    iconPicture.Location = new Point(
                        HorizontalPadding,
                        TitleHeight + VerticalPadding
                    );
                }

                // Position message
                lblMessage.Location = new Point(
                    iconPicture.Visible ? iconPicture.Right + 15 : HorizontalPadding,
                    TitleHeight + VerticalPadding
                );
                lblMessage.Size = new Size(
                    formWidth - (iconPicture.Visible ? iconPicture.Right + 30 : 2 * HorizontalPadding),
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
            using (var msgBox = new ThemedMessageBoxGreen(text, caption, icon))
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
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Add subtle shadow
           
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

    // Extension for drop shadow effect
    //public static class ShadowExtensions
    //{
    //    [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    //    private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect,
    //        int nBottomRect, int nWidthEllipse, int nHeightEllipse);

    //    [DllImport("dwmapi.dll")]
    //    public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

    //    [DllImport("dwmapi.dll")]
    //    public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    //    [DllImport("dwmapi.dll")]
    //    public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

    //    private struct MARGINS
    //    {
    //        public int leftWidth;
    //        public int rightWidth;
    //        public int topHeight;
    //        public int bottomHeight;
    //    }

    //    public static void DropShadow(this Form form)
    //    {
    //        if (!DropShadowSupported()) return;

    //        var v = 2;
    //        DwmSetWindowAttribute(form.Handle, 2, ref v, 4);

    //        MARGINS margins = new MARGINS()
    //        {
    //            bottomHeight = 1,
    //            leftWidth = 0,
    //            rightWidth = 0,
    //            topHeight = 0
    //        };

    //        DwmExtendFrameIntoClientArea(form.Handle, ref margins);
    //    }

    //    private static bool DropShadowSupported()
    //    {
    //        if (Environment.OSVersion.Version.Major < 6) return false;
    //        int pfEnabled = 0;
    //        DwmIsCompositionEnabled(ref pfEnabled);
    //        return (pfEnabled == 1);
    //    }
    //}

    //public enum ThemedMessageBoxIcon
    //{
    //    None,
    //    Error,
    //    Information,
    //    Warning,
    //    Question
    //}
}