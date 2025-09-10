using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pos_system.pos.Core;

namespace pos_system.pos.UI.Forms.Inventory
{
    public partial class ImagePreviewDialog : Form
    {
        private PictureBox pictureBox;
        private Label lblNoImage;
        private Panel headerPanel;
        private Button btnClose;
        private Button btnMaximize;
        private bool isMaximized = false;
        private Point restoreLocation;
        private Size restoreSize;

        // For form dragging
        private bool isDragging = false;
        private Point startPoint;

        public ImagePreviewDialog(Image image)
        {
            InitializeComponent();

            if (image != null)
            {
                pictureBox.Image = image;
                lblNoImage.Visible = false;
            }
            else
            {
                pictureBox.Visible = false;
                lblNoImage.Visible = true;
            }

            this.Text = "Product Image Preview";
        }

        private void InitializeComponent()
        {
            // Main Form
            this.ClientSize = new Size(500, 550);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Padding = new Padding(0);
            this.MaximizeBox = false;

            // Apply shadow effect (ensure DropShadow class exists in project)
            new DropShadow().ApplyShadows(this);

            // Main container panel
            Panel mainContainer = new Panel();
            mainContainer.Dock = DockStyle.Fill;
            mainContainer.BorderStyle = BorderStyle.None;
            mainContainer.Padding = new Padding(0);
            this.Controls.Add(mainContainer);

            // Header Panel (for dragging and controls)
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.BackColor = Color.FromArgb(0, 120, 215); // PrimaryBlue
            headerPanel.Height = 40;
            headerPanel.MouseDown += HeaderPanel_MouseDown;
            headerPanel.MouseMove += HeaderPanel_MouseMove;
            headerPanel.MouseUp += HeaderPanel_MouseUp;
            headerPanel.DoubleClick += HeaderPanel_DoubleClick;
            mainContainer.Controls.Add(headerPanel);

            // Title Label
            Label lblTitle = new Label();
            lblTitle.Text = "PRODUCT IMAGE PREVIEW";
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Padding = new Padding(20, 10, 0, 0);
            lblTitle.MouseDown += HeaderPanel_MouseDown;
            lblTitle.MouseMove += HeaderPanel_MouseMove;
            lblTitle.MouseUp += HeaderPanel_MouseUp;
            headerPanel.Controls.Add(lblTitle);

            // Maximize Button
            btnMaximize = new Button();
            btnMaximize.Text = "🗖";
            btnMaximize.Dock = DockStyle.Right;
            btnMaximize.FlatStyle = FlatStyle.Flat;
            btnMaximize.Font = new Font("Segoe UI", 10);
            btnMaximize.ForeColor = Color.White;
            btnMaximize.BackColor = Color.Transparent;
            btnMaximize.Size = new Size(40, 40);
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.FlatAppearance.MouseOverBackColor = Color.FromArgb(80, 80, 80);
            btnMaximize.Click += BtnMaximize_Click;
            headerPanel.Controls.Add(btnMaximize);

            // Close Button
            btnClose = new Button();
            btnClose.Text = "✕";
            btnClose.Dock = DockStyle.Right;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12);
            btnClose.ForeColor = Color.White;
            btnClose.BackColor = Color.Transparent;
            btnClose.Size = new Size(40, 40);
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 0, 0);
            btnClose.Click += (s, e) => this.Close();
            headerPanel.Controls.Add(btnClose);

            // Image Container
            Panel imageContainer = new Panel();
            imageContainer.Dock = DockStyle.Fill;
            imageContainer.BackColor = Color.WhiteSmoke;
            imageContainer.Padding = new Padding(20);
            mainContainer.Controls.Add(imageContainer);

            // PictureBox
            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BackColor = Color.WhiteSmoke;
            imageContainer.Controls.Add(pictureBox);

            // No Image Label
            lblNoImage = new Label();
            lblNoImage.Dock = DockStyle.Fill;
            lblNoImage.Text = "No image available";
            lblNoImage.TextAlign = ContentAlignment.MiddleCenter;
            lblNoImage.Font = new Font("Arial", 14, FontStyle.Bold);
            lblNoImage.ForeColor = Color.Gray;
            lblNoImage.Visible = false;
            imageContainer.Controls.Add(lblNoImage);
        }

        private void BtnMaximize_Click(object sender, EventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            if (isMaximized)
            {
                // Restore to previous size and position
                this.Size = restoreSize;
                this.Location = restoreLocation;
                btnMaximize.Text = "🗖";
                isMaximized = false;
            }
            else
            {
                // Save current size and position
                restoreSize = this.Size;
                restoreLocation = this.Location;

                // Maximize to the current screen
                Rectangle screenArea = Screen.FromControl(this).WorkingArea;
                this.Location = screenArea.Location;
                this.Size = screenArea.Size;
                btnMaximize.Text = "🗗";
                isMaximized = true;
            }
        }

        private void HeaderPanel_DoubleClick(object sender, EventArgs e)
        {
            ToggleMaximize();
        }

        private void HeaderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                startPoint = new Point(e.X, e.Y);
            }
        }

        private void HeaderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newPoint = headerPanel.PointToScreen(new Point(e.X, e.Y));
                newPoint.Offset(-startPoint.X, -startPoint.Y);
                this.Location = newPoint;
            }
        }

        private void HeaderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pictureBox.Image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
