using pos_system.pos.BLL.Services;
using pos_system.pos.Core;
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

namespace pos_system.pos.UI.Forms.Inventory
{
    public partial class BrandFormEditor : Form
    {
        private readonly Brand _brand;
        private readonly BrandService _service = new BrandService();
        private TextBox txtName;
        private Button btnClose;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;

        private Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private Color ButtonHoverColor = Color.FromArgb(31, 97, 141);
        private Color ButtonGray = Color.FromArgb(120, 120, 120);

        public BrandFormEditor(Brand brand = null)
        {
            _brand = brand ?? new Brand();
            InitializeComponent();
            new DropShadow().ApplyShadows(this);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Size = new Size(400, 220);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);
            this.Text = _brand.Brand_ID > 0 ? "Edit Brand" : "Add Brand";
            this.Padding = new Padding(1);

            // Main container panel - holds everything
            Panel mainContainer = new Panel();
            mainContainer.Dock = DockStyle.Fill;
            mainContainer.BackColor = Color.White;
            this.Controls.Add(mainContainer);

            // TOP PANEL (header)
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 40;
            topPanel.BackColor = PrimaryColor;
            topPanel.Padding = new Padding(0, 0, 10, 0);
            mainContainer.Controls.Add(topPanel);

            // Title label
            lblTitle = new Label();
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Text = _brand.Brand_ID > 0 ? "Edit Brand" : "Add Brand";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Padding = new Padding(10, 10, 0, 0);
            lblTitle.AutoSize = true;
            topPanel.Controls.Add(lblTitle);

            // Close button
            btnClose = new Button();
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F);
            btnClose.ForeColor = Color.White;
            btnClose.Text = "✕";
            btnClose.Dock = DockStyle.Right;
            btnClose.Size = new Size(40, 40);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            topPanel.Controls.Add(btnClose);

            // CONTENT PANEL - holds form elements
            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.White;
            contentPanel.Padding = new Padding(20, 40, 20, 20);
            mainContainer.Controls.Add(contentPanel);

            // Create a container for the form elements
            Panel formContainer = new Panel();
            formContainer.Dock = DockStyle.Fill;
            formContainer.BackColor = Color.White;
            contentPanel.Controls.Add(formContainer);

            // Brand Name Label
            Label lblName = new Label();
            lblName.Text = "Brand Name:";
            lblName.Location = new Point(20, 30);
            lblName.AutoSize = true;
            lblName.ForeColor = Color.Black;
            formContainer.Controls.Add(lblName);

            // Brand Name TextBox
            txtName = new TextBox();
            txtName.Location = new Point(150, 30);
            txtName.Size = new Size(200, 30);
            txtName.Text = _brand.brandName;
            txtName.BorderStyle = BorderStyle.FixedSingle;
            formContainer.Controls.Add(txtName);

            // BUTTON PANEL - at bottom of content panel
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.BackColor = Color.White;
            contentPanel.Controls.Add(buttonPanel);

            FlowLayoutPanel buttonFlow = new FlowLayoutPanel();
            buttonFlow.FlowDirection = FlowDirection.RightToLeft;
            buttonFlow.Dock = DockStyle.Fill;
            buttonFlow.Padding = new Padding(0, 10, 10, 0);
            buttonPanel.Controls.Add(buttonFlow);

            // Cancel button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(100, 40);
            btnCancel.BackColor = ButtonGray;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Margin = new Padding(0);
            btnCancel.Click += (s, e) => this.Close();
            buttonFlow.Controls.Add(btnCancel);

            // Save button
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Size = new Size(100, 40);
            btnSave.BackColor = PrimaryColor;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Margin = new Padding(0, 0, 10, 0);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += (s, e) =>
            {
                if (ValidateAndSave())
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };
            buttonFlow.Controls.Add(btnSave);

            // Add button hover effects
            btnSave.MouseEnter += (s, e) => btnSave.BackColor = ButtonHoverColor;
            btnSave.MouseLeave += (s, e) => btnSave.BackColor = PrimaryColor;
            btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.Gray;
            btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = ButtonGray;
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(200, 50, 50);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.Transparent;

            this.ResumeLayout(true);
        }

        private bool ValidateAndSave()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowThemedMessage("Brand name cannot be empty");
                return false;
            }

            if (_service.CheckBrandExists(txtName.Text,
                _brand.Brand_ID > 0 ? _brand.Brand_ID : (int?)null))
            {
                ShowThemedMessage("Brand name already exists");
                return false;
            }

            _brand.brandName = txtName.Text.Trim();

            bool success = _brand.Brand_ID > 0
                ? _service.UpdateBrand(_brand.Brand_ID, _brand.brandName)
                : _service.AddBrand(_brand.brandName);

            if (!success)
            {
                ShowThemedMessage("Error saving brand");
                return false;
            }
            return true;
        }

        private void ShowThemedMessage(string message)
        {
            MessageBox.Show(this, message, "Brand",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Enable form dragging
        private bool _dragging;
        private Point _startPoint = new Point(0, 0);

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.Y < 40) // Only drag from header area
            {
                _dragging = true;
                _startPoint = new Point(e.X, e.Y);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._startPoint.X, p.Y - this._startPoint.Y);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;
        }
    }
}
