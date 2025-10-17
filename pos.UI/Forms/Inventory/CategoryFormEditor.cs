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
    public partial class CategoryFormEditor : Form
    {
        private readonly Category _category;
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly SizeService _sizeService = new SizeService();
        private List<CategorySize> _allSizes;
        private CheckedListBox clbSizes;
        private TextBox txtName;
        private Button btnClose;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;

        private Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private Color ButtonHoverColor = Color.FromArgb(31, 97, 141);
        private Color ButtonGray = Color.FromArgb(120, 120, 120);

        public CategoryFormEditor(Category category = null)
        {
            _category = category ?? new Category();
            InitializeComponent();
            this.Load += CategoryForm_Load;
            new DropShadow().ApplyShadows(this);
        }

        private void CategoryForm_Load(object sender, EventArgs e)
        {
            LoadSizes();
        }

        private void LoadSizes()
        {
            try
            {
                _allSizes = _sizeService.GetAllSizes();
                clbSizes.Items.Clear();

                foreach (var group in _allSizes.GroupBy(s => s.SizeType))
                {
                    // Add header with null Size_ID
                    clbSizes.Items.Add(new SizeItem
                    {
                        Text = $"--- {group.Key} ---",
                        Size_ID = null
                    }, false);

                    foreach (var size in group)
                    {
                        // Store both text and Size_ID
                        clbSizes.Items.Add(new SizeItem
                        {
                            Text = size.SizeLabel,
                            Size_ID = size.Size_ID
                        }, false);
                    }
                }

                if (_category.Category_ID > 0)
                {
                    var assignedSizes = _categoryService.GetSizesByCategoryId(_category.Category_ID);
                    var assignedSizeIds = assignedSizes.Select(s => s.Size_ID).ToList();

                    for (int i = 0; i < clbSizes.Items.Count; i++)
                    {
                        var item = (SizeItem)clbSizes.Items[i];
                        // Check if item is a valid size and assigned to category
                        if (item.Size_ID.HasValue && assignedSizeIds.Contains(item.Size_ID.Value))
                        {
                            clbSizes.SetItemChecked(i, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowThemedMessage($"Error loading sizes: {ex.Message}");
            }
        }

        private class SizeItem
        {
            public string Text { get; set; }
            public int? Size_ID { get; set; }
            public override string ToString() => Text;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Size = new Size(500, 550);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);
            this.Text = _category.Category_ID > 0 ? "Edit Category" : "Add Category";
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
            lblTitle.Text = _category.Category_ID > 0 ? "Edit Category" : "Add Category";
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

            // CONTENT CONTAINER - holds form elements and buttons
            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.White;
            contentPanel.Padding = new Padding(20, 40, 20, 20);
            mainContainer.Controls.Add(contentPanel);

            // Main table layout for form fields
            TableLayoutPanel tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 2;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayout.Padding = new Padding(10);
            tableLayout.AutoScroll = true;
            contentPanel.Controls.Add(tableLayout);

            // Row definitions
            tableLayout.RowCount = 3;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Category name row
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Sizes label row
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Sizes list row

            // Category Name
            tableLayout.Controls.Add(new Label()
            {
                Text = "Category Name:",
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                ForeColor = Color.Black
            }, 0, 0);

            txtName = new TextBox();
            txtName.Dock = DockStyle.Fill;
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.Text = _category.categoryName;
            tableLayout.Controls.Add(txtName, 1, 0);

            // Sizes Label
            tableLayout.Controls.Add(new Label()
            {
                Text = "Applicable Sizes:",
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                AutoSize = true,
                ForeColor = Color.Black,
                Margin = new Padding(0, 8, 0, 0)
            }, 0, 1);

            // Sizes CheckedListBox - spans both columns
            Panel sizesContainer = new Panel();
            sizesContainer.Dock = DockStyle.Fill;
            sizesContainer.BackColor = Color.White;
            tableLayout.SetColumnSpan(sizesContainer, 2);
            tableLayout.SetRow(sizesContainer, 2);

            clbSizes = new CheckedListBox();
            clbSizes.Dock = DockStyle.Fill;
            clbSizes.BorderStyle = BorderStyle.FixedSingle;
            clbSizes.CheckOnClick = true;
            clbSizes.BackColor = Color.White;
            clbSizes.Margin = new Padding(10);
            sizesContainer.Controls.Add(clbSizes);
            tableLayout.Controls.Add(sizesContainer, 0, 2);

            // BUTTON PANEL (at bottom of content area)
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.BackColor = Color.White;
            contentPanel.Controls.Add(buttonPanel);

            FlowLayoutPanel buttonFlow = new FlowLayoutPanel();
            buttonFlow.FlowDirection = FlowDirection.RightToLeft;
            buttonFlow.Dock = DockStyle.Fill;
            buttonFlow.Padding = new Padding(0, 10, 0, 0);
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
            btnSave.Click += (s, e) => SaveCategory();
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

        private void SaveCategory()
        {
            if (ValidateInput())
            {
                try
                {
                    bool success;
                    int categoryId = _category.Category_ID;

                    if (categoryId > 0)
                    {
                        success = _categoryService.UpdateCategory(categoryId, txtName.Text);
                    }
                    else
                    {
                        success = _categoryService.AddCategory(txtName.Text);
                        categoryId = _categoryService.GetCategoryIdByName(txtName.Text);
                    }

                    if (success && categoryId > 0)
                    {
                        UpdateCategorySizes(categoryId);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        ShowThemedMessage("Error saving category");
                    }
                }
                catch (Exception ex)
                {
                    ShowThemedMessage($"Error: {ex.Message}");
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowThemedMessage("Category name cannot be empty");
                return false;
            }

            if (_categoryService.CheckCategoryExists(txtName.Text,
                _category.Category_ID > 0 ? _category.Category_ID : (int?)null))
            {
                ShowThemedMessage("Category name already exists");
                return false;
            }

            return true;
        }

        private void UpdateCategorySizes(int categoryId)
        {
            var selectedSizeIds = new List<int>();

            for (int i = 0; i < clbSizes.Items.Count; i++)
            {
                var item = (SizeItem)clbSizes.Items[i];
                // Skip headers and unchecked items
                if (item.Size_ID == null || !clbSizes.GetItemChecked(i))
                    continue;

                selectedSizeIds.Add(item.Size_ID.Value);
            }

            if (!_categoryService.UpdateCategorySizes(categoryId, selectedSizeIds))
            {
                ShowThemedMessage("Error updating sizes for category");
            }
        }

        private void ShowThemedMessage(string message)
        {
            MessageBox.Show(this, message, "Category",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Enable form dragging
        private bool _dragging;
        private Point _startPoint = new Point(0, 0);

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
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
