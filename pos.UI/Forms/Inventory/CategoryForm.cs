using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pos_system.pos.BLL.Services;
using pos_system.pos.Models;

namespace pos_system.pos.UI.Forms
{
    public partial class CategoryForm : Form
    {
        private readonly Category _category;
        private readonly CategoryService _service = new CategoryService();

        public CategoryForm(Category category = null)
        {
            _category = category ?? new Category();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(350, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _category.Category_ID > 0 ? "Edit Category" : "Add Category";
            this.BackColor = Color.White;

            // Category Name
            var lblName = new Label
            {
                Text = "Category Name:",
                Location = new Point(20, 30),
                Width = 100
            };

            var txtName = new TextBox
            {
                Location = new Point(130, 30),
                Width = 180,
                Text = _category.categoryName
            };

            // Buttons
            var btnSave = new Button
            {
                Text = "Save",
                Location = new Point(130, 100),
                Width = 80,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(220, 100),
                Width = 80,
                BackColor = Color.IndianRed,
                ForeColor = Color.White
            };

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Category name cannot be empty");
                    return;
                }

                if (_service.CheckCategoryExists(txtName.Text, _category.Category_ID > 0 ? _category.Category_ID : (int?)null))
                {
                    MessageBox.Show("Category name already exists");
                    return;
                }

                _category.categoryName = txtName.Text.Trim();

                bool success = _category.Category_ID > 0
                    ? _service.UpdateCategory(_category.Category_ID, _category.categoryName)
                    : _service.AddCategory(_category.categoryName);

                if (success)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Error saving category");
                }
            };

            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            this.Controls.AddRange(new Control[] { lblName, txtName, btnSave, btnCancel });
        }
    }
}
