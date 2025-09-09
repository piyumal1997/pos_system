using pos_system.pos.BLL.Services;
using pos_system.pos.Core;
using pos_system.pos.Models;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace pos_system.pos.UI.Forms.Inventory
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
            new DropShadow().ApplyShadows(this);
        }

        private void InitializeForm()
        {
            // Configure form
            this.Size = new Size(920, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            // Configure DataGridView
            dgvItems.ReadOnly = true;
            dgvItems.AutoGenerateColumns = false;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.BackgroundColor = Color.White;
            dgvItems.RowHeadersVisible = false;
            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.Font = new Font("Segoe UI", 11);

            // Configure header
            dgvItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(60, 162, 230),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvItems.RowsDefaultCellStyle.BackColor = Color.White;
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvItems.RowTemplate.Height = 60;

            // Add borders to search fields
            AddBottomBorder(txtSearch, 250);
            AddBottomBorder(cmbBrand, 250);
            AddBottomBorder(cmbCategory, 250);

            // Populate dropdowns
            PopulateBrands();
            PopulateCategories();

            // Configure grid columns
            ConfigureGridColumns();

            // Event handlers
            btnClose.Click += (s, e) => this.Close();
            btnClear.Click += (s, e) => ClearFilters();
            txtSearch.TextChanged += (s, e) => SearchItems();
            cmbBrand.SelectedIndexChanged += (s, e) => SearchItems();
            cmbCategory.SelectedIndexChanged += (s, e) => SearchItems();
            dgvItems.CellDoubleClick += (s, e) => SelectItem();
            // Remove byte array formatting handler
            //dgvItems.CellFormatting += DgvItems_CellFormatting;

            // Add new image rendering handler
            dgvItems.CellPainting += DgvItems_CellPainting;
        }

        private void AddBottomBorder(Control control, int width)
        {
            var border = new Panel
            {
                Size = new Size(width, 1),
                Location = new Point(control.Left, control.Bottom + 2),
                BackColor = Color.Silver
            };
            searchPanel.Controls.Add(border);
            border.BringToFront();
        }

        private void ConfigureGridColumns()
        {
            dgvItems.Columns.Clear();

            dgvItems.Columns.AddRange(new DataGridViewColumn[] {
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "ProductSize_ID",
                    Name = "ProductSize_ID",
                    HeaderText = "ID",
                    Visible = false
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "Product_ID",
                    Name = "Product_ID",
                    HeaderText = "Prod ID",
                    Visible = true
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "Barcode",
                    HeaderText = "Barcode",
                    Name = "Barcode",
                    Width = 120
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "Description",
                    HeaderText = "Description",
                    Name = "Description",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "BrandName",
                    HeaderText = "Brand",
                    Name = "BrandName",
                    Width = 100
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "CategoryName",
                    HeaderText = "Category",
                    Name = "CategoryName",
                    Width = 100
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "SizeLabel",
                    HeaderText = "Size",
                    Name = "SizeLabel",
                    Width = 80
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "RetailPrice",
                    HeaderText = "Price",
                    Name = "RetailPrice",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                    Width = 80
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "UnitCost",
                    HeaderText = "Cost",
                    Name = "UnitCost",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                    Width = 80
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "Quantity",
                    HeaderText = "Stock",
                    Name = "Quantity",
                    Width = 60
                },
                new DataGridViewTextBoxColumn {
                    DataPropertyName = "ItemImage",
                    HeaderText = "Image",
                    Name = "ItemImage",
                    Visible = false  // Hide filename column
                },
                
                // Add new unbound column for image display
                new DataGridViewImageColumn {
                    HeaderText = "Image",
                    Name = "ImagePreview",
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 80
                }
            });
        }

        private void DgvItems_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Only handle image column and rows (not headers)
            if (e.RowIndex < 0 || e.ColumnIndex != dgvItems.Columns["ImagePreview"].Index)
                return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

            // Get filename from hidden column
            var filename = dgvItems.Rows[e.RowIndex].Cells["ItemImage"].Value?.ToString();
            var image = ImageHelper.LoadProductImage(filename);

            if (image != null)
            {
                // Calculate aspect ratio
                float ratio = Math.Min(
                    (float)(e.CellBounds.Width - 4) / image.Width,
                    (float)(e.CellBounds.Height - 4) / image.Height
                );

                int newWidth = (int)(image.Width * ratio);
                int newHeight = (int)(image.Height * ratio);
                int x = e.CellBounds.X + (e.CellBounds.Width - newWidth) / 2;
                int y = e.CellBounds.Y + (e.CellBounds.Height - newHeight) / 2;

                e.Graphics.DrawImage(image, x, y, newWidth, newHeight);
            }
            else
            {
                // Draw placeholder text
                TextRenderer.DrawText(e.Graphics, "No Image",
                    dgvItems.Font, e.CellBounds, Color.Gray,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            e.Handled = true;
        }

        private void PopulateBrands()
        {
            var brands = _brandService.GetAllBrands().ToList();
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

            var items = _itemService.SearchItemsOwner(searchTerm, brandId, categoryId);
            dgvItems.DataSource = items;
        }

        private void ClearFilters()
        {
            txtSearch.Text = string.Empty;
            cmbBrand.SelectedIndex = 0;
            cmbCategory.SelectedIndex = 0;
            SearchItems();
        }

        private void DgvItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvItems.Columns[e.ColumnIndex].Name == "ItemImage" && e.Value != null)
            {
                try
                {
                    byte[] imageData = e.Value as byte[];
                    if (imageData != null && imageData.Length > 0)
                    {
                        using (var ms = new MemoryStream(imageData))
                        {
                            e.Value = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
                catch
                {
                    e.Value = null;
                }
            }
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
                ProductSize_ID = Convert.ToInt32(selectedRow.Cells["ProductSize_ID"].Value),
                Product_ID = Convert.ToInt32(selectedRow.Cells["Product_ID"].Value),
                Barcode = selectedRow.Cells["Barcode"].Value?.ToString(),
                Description = selectedRow.Cells["Description"].Value?.ToString(),
                BrandName = selectedRow.Cells["BrandName"].Value?.ToString(),
                CategoryName = selectedRow.Cells["CategoryName"].Value?.ToString(),
                SizeLabel = selectedRow.Cells["SizeLabel"].Value?.ToString(),
                RetailPrice = Convert.ToDecimal(selectedRow.Cells["RetailPrice"].Value),
                Quantity = Convert.ToInt32(selectedRow.Cells["Quantity"].Value),
                UnitCost = 0,
                ItemImage = selectedRow.Cells["ItemImage"].Value?.ToString()
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}