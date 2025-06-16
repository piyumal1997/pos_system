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
    public partial class BrandForm : Form
    {
        private readonly Brand _brand;
        private readonly BrandService _service = new BrandService();

        public BrandForm(Brand brand = null)
        {
            _brand = brand ?? new Brand();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(350, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _brand.Brand_ID > 0 ? "Edit Brand" : "Add Brand";
            this.BackColor = Color.White;

            // Brand Name
            var lblName = new Label
            {
                Text = "Brand Name:",
                Location = new Point(20, 30),
                Width = 100
            };

            var txtName = new TextBox
            {
                Location = new Point(130, 30),
                Width = 180,
                Text = _brand.brandName
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
                    MessageBox.Show("Brand name cannot be empty");
                    return;
                }

                if (_service.CheckBrandExists(txtName.Text, _brand.Brand_ID > 0 ? _brand.Brand_ID : (int?)null))
                {
                    MessageBox.Show("Brand name already exists");
                    return;
                }

                _brand.brandName = txtName.Text.Trim();

                bool success = _brand.Brand_ID > 0
                    ? _service.UpdateBrand(_brand.Brand_ID, _brand.brandName)
                    : _service.AddBrand(_brand.brandName);

                if (success)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Error saving brand");
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
