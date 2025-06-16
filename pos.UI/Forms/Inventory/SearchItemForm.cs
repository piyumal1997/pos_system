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
    public partial class SearchItemForm : Form
    {
        private readonly ItemService _itemService = new ItemService();
        private readonly BrandService _brandService = new BrandService();
        private readonly CategoryService _categoryService = new CategoryService();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Item SelectedItem { get; private set; }

        public SearchItemForm()
        {
            InitializeComponent();
            InitializeForm();
            SearchItems();
            
        }

        private void InitializeForm()
        {
            this.Font = new Font("Segoe UI", 9);
            this.Size = new Size(920, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Padding = new Padding(0);

            var container = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            PopulateBrands();
            PopulateCategories();

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(60, 162, 230)
            };

            var lblTitle = new Label
            {
                Text = "SEARCH ITEMS",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };

            var btnClose = new Button
            {
                Text = "✕",
                Dock = DockStyle.Right,
                Width = 60,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 76, 60);
            btnClose.Click += (s, e) => this.Close();

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(btnClose);

            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(20, 10, 20, 10),
                BackColor = Color.White
            };

            Panel CreateBottomBorder(Control ctrl, int width)
            {
                return new Panel
                {
                    Size = new Size(width, 1),
                    Location = new Point(ctrl.Left, ctrl.Bottom + 2),
                    BackColor = Color.Silver
                };
            }

            var lblSearch = new Label
            {
                Text = "SEARCH:",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.BackColor = Color.White;
            txtSearch.Font = new Font("Segoe UI", 10);
            var searchBorder = CreateBottomBorder(txtSearch, txtSearch.Width);

            var lblBrand = new Label
            {
                Text = "BRAND:",
                Location = new Point(310, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            cmbBrand.FlatStyle = FlatStyle.Flat;
            cmbBrand.BackColor = Color.White;
            cmbBrand.Font = new Font("Segoe UI", 10);
            var brandBorder = CreateBottomBorder(cmbBrand, cmbBrand.Width);

            var lblCategory = new Label
            {
                Text = "CATEGORY:",
                Location = new Point(620, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            cmbCategory.FlatStyle = FlatStyle.Flat;
            cmbCategory.BackColor = Color.White;
            cmbCategory.Font = new Font("Segoe UI", 10);
            var categoryBorder = CreateBottomBorder(cmbCategory, cmbCategory.Width);

            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.BackColor = Color.White;
            btnClear.ForeColor = Color.FromArgb(41, 128, 185);
            btnClear.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(41, 128, 185);
            btnClear.FlatAppearance.BorderSize = 1;
            btnClear.Click += (s, e) => ClearFilters();

            searchPanel.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, searchBorder,
                lblBrand, cmbBrand, brandBorder,
                lblCategory, cmbCategory, categoryBorder,
                btnClear
            });

            dgvItems.ReadOnly = true;
            dgvItems.AutoGenerateColumns = false;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.BackgroundColor = Color.White;
            dgvItems.RowHeadersVisible = false;
            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.Font = new Font("Segoe UI", 9);

            dgvItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(60, 162, 230),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvItems.RowsDefaultCellStyle.BackColor = Color.White;
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvItems.RowTemplate.Height = 40;

            ConfigureGridColumns();

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 0, 20, 20),
                BackColor = Color.White
            };
            contentPanel.Controls.Add(dgvItems);

            container.Controls.Add(contentPanel);
            container.Controls.Add(searchPanel);
            container.Controls.Add(headerPanel);
            this.Controls.Add(container);

            txtSearch.TextChanged += (s, e) => SearchItems();
            cmbBrand.SelectedIndexChanged += (s, e) => SearchItems();
            cmbCategory.SelectedIndexChanged += (s, e) => SearchItems();
            dgvItems.CellDoubleClick += (s, e) => SelectItem();
        }

        private void ConfigureGridColumns()
        {
            dgvItems.Columns.Clear();

            dgvItems.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "Item_ID",
                    Name = "Item_ID",
                    HeaderText = "ID",
                    Visible = false
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "barcode",
                    HeaderText = "Barcode",
                    Name = "barcode"
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "description",
                    HeaderText = "Description",
                    Name = "description",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "BrandName",
                    HeaderText = "Brand"
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "CategoryName",
                    HeaderText = "Category"
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "RetailPrice",
                    HeaderText = "Price",
                    Name = "RetailPrice",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "quantity",
                    HeaderText = "Stock"
                },
                new DataGridViewImageColumn
                {
                    DataPropertyName = "ItemImage",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 65
                }
            });
        }

        private void PopulateBrands()
        {
            var brands = _brandService.GetAllBrand().ToList();
            brands.Insert(0, new Brand { Brand_ID = 0, brandName = "All Brands" });
            cmbBrand.DataSource = brands;
            cmbBrand.DisplayMember = "brandName";
            cmbBrand.ValueMember = "Brand_ID";
        }

        private void PopulateCategories()
        {
            var categories = _categoryService.GetAllCategorie().ToList();
            categories.Insert(0, new Category { Category_ID = 0, categoryName = "All Categories" });
            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "categoryName";
            cmbCategory.ValueMember = "Category_ID";
        }

        private void SearchItems()
        {
            string searchTerm = txtSearch.Text.Trim();
            int brandId = (int)cmbBrand.SelectedValue;
            int categoryId = (int)cmbCategory.SelectedValue;

            var items = _itemService.SearchItems(searchTerm, brandId, categoryId);
            dgvItems.DataSource = items;
        }

        private void ClearFilters()
        {
            txtSearch.Text = string.Empty;
            cmbBrand.SelectedIndex = 0;
            cmbCategory.SelectedIndex = 0;
            SearchItems();
        }

        private void SelectItem()
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item first", "Selection Required",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvItems.SelectedRows[0];
            SelectedItem = new Item
            {
                Item_ID = (int)selectedRow.Cells["Item_ID"].Value,
                barcode = selectedRow.Cells["barcode"].Value?.ToString(),
                description = selectedRow.Cells["description"].Value?.ToString(),
                RetailPrice = Convert.ToDecimal(selectedRow.Cells["RetailPrice"].Value),
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}