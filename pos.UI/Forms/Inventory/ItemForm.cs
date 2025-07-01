using pos_system.pos.BLL.Services;
using pos_system.pos.Core;
using pos_system.pos.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace pos_system.pos.UI.Forms.Inventory
{
    public partial class ItemForm : Form
    {
        private readonly Item _item;
        private readonly BrandService _brandService = new BrandService();
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly ItemService _itemService = new ItemService();
        private readonly SizeService _sizeService = new SizeService();
        private readonly GenderService _genderService = new GenderService();
        private BindingList<ProductSize> _sizesBindingList;
        private Point _startPos;

        public ItemForm(Item item = null)
        {
            _item = item ?? new Item();
            InitializeComponent();
            InitializeFormCustomizations();
            LoadInitialData();
            new DropShadow().ApplyShadows(this);
        }

        private void InitializeFormCustomizations()
        {
            lblTitle.Text = _item.Product_ID > 0 ? "Edit Product" : "Add Product";
            btnSave.Text = _item.Product_ID > 0 ? "Update" : "Save";

            if (_item.Product_ID > 0)
            {
                txtBarcode.ReadOnly = true;
                txtBarcode.BackColor = SystemColors.Control;
            }
            else
            {
                btnGenerate.Visible = true;
            }

            // Initialize sizes binding list
            _sizesBindingList = new BindingList<ProductSize>(_item.Sizes);
            dgvSizes.DataSource = _sizesBindingList;

            // Configure grid columns
            ConfigureGridColumns();

            // Add event handlers for grid validation
            dgvSizes.CellValidating += DgvSizes_CellValidating;
            dgvSizes.CellEndEdit += DgvSizes_CellEndEdit;
            dgvSizes.DataError += DgvSizes_DataError;

            // Event handlers
            btnGenerate.Click += BtnGenerate_Click;
            btnBrowse.Click += BrowseImage;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            btnClose.Click += (s, e) => Close();
            btnMinimize.Click += (s, e) => WindowState = FormWindowState.Minimized;
            btnAddSize.Click += BtnAddSize_Click;
            btnRemoveSize.Click += BtnRemoveSize_Click;

            // Enable form dragging
            topPanel.MouseDown += TopPanel_MouseDown;
            topPanel.MouseMove += TopPanel_MouseMove;
            lblTitle.MouseDown += TopPanel_MouseDown;
            lblTitle.MouseMove += TopPanel_MouseMove;
        }

        private void ConfigureGridColumns()
        {
            // Clear existing columns
            dgvSizes.Columns.Clear();

            // Size column (ComboBox)
            DataGridViewComboBoxColumn sizeColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "Size_ID",
                HeaderText = "Size",
                Name = "colSize",
                Width = 120,
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            };
            dgvSizes.Columns.Add(sizeColumn);

            // Quantity column
            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity",
                HeaderText = "Quantity",
                Name = "colQuantity",
                Width = 100
            };
            dgvSizes.Columns.Add(quantityColumn);

            // Retail Price column
            DataGridViewTextBoxColumn retailPriceColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RetailPrice",
                HeaderText = "Retail Price",
                Name = "colRetailPrice",
                Width = 120
            };
            dgvSizes.Columns.Add(retailPriceColumn);

            // Unit Cost column
            DataGridViewTextBoxColumn unitCostColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UnitCost",
                HeaderText = "Unit Cost",
                Name = "colUnitCost",
                Width = 120
            };
            dgvSizes.Columns.Add(unitCostColumn);

            // Set grid properties
            dgvSizes.AutoGenerateColumns = false;
            dgvSizes.AllowUserToAddRows = false;
            dgvSizes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSizes.RowHeadersVisible = false;
            dgvSizes.MultiSelect = false;
            dgvSizes.BackgroundColor = SystemColors.Window;
            dgvSizes.BorderStyle = BorderStyle.None;
            dgvSizes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void LoadInitialData()
        {
            PopulateBrands();
            PopulateCategories();
            PopulateGenders();
            LoadSizeOptions();

            if (_item.Product_ID > 0)
            {
                txtBarcode.Text = _item.Barcode;
                txtDescription.Text = _item.Description;
                txtMaxDiscount.Text = _item.MaxDiscount.ToString("0.00");
                txtMinStock.Text = _item.MinStockLevel.ToString();

                cmbBrand.SelectedValue = _item.Brand_ID;
                cmbCategory.SelectedValue = _item.Category_ID;
                cmbGender.SelectedValue = _item.Gender_ID;

                if (_item.ItemImage != null)
                {
                    using (var ms = new MemoryStream(_item.ItemImage))
                    {
                        picItemImage.Image = Image.FromStream(ms);
                    }
                }
            }
        }

        private void PopulateBrands()
        {
            cmbBrand.DataSource = _brandService.GetAllBrands();
            cmbBrand.DisplayMember = "brandName";
            cmbBrand.ValueMember = "Brand_ID";
        }

        private void PopulateCategories()
        {
            cmbCategory.DataSource = _categoryService.GetAllCategorie();
            cmbCategory.DisplayMember = "categoryName";
            cmbCategory.ValueMember = "Category_ID";
        }

        private void PopulateGenders()
        {
            cmbGender.DataSource = _genderService.GetAllGenders();
            cmbGender.DisplayMember = "GenderName";
            cmbGender.ValueMember = "Gender_ID";
        }

        private void LoadSizeOptions()
        {
            if (cmbCategory.SelectedItem is Category selectedCategory)
            {
                var sizes = _sizeService.GetSizesByCategoryId(selectedCategory.Category_ID);
                var sizeColumn = dgvSizes.Columns["colSize"] as DataGridViewComboBoxColumn;

                if (sizeColumn != null)
                {
                    sizeColumn.DataSource = sizes;
                    sizeColumn.DisplayMember = "SizeLabel";
                    sizeColumn.ValueMember = "Size_ID";

                    // Refresh the grid to update display values
                    dgvSizes.Refresh();
                }
            }
        }

        private void MapFormToItem()
        {
            if (_item.Product_ID == 0)
                _item.Barcode = txtBarcode.Text;

            _item.Description = txtDescription.Text;
            _item.MaxDiscount = decimal.Parse(txtMaxDiscount.Text);
            _item.MinStockLevel = int.Parse(txtMinStock.Text);
            _item.Brand_ID = (int)cmbBrand.SelectedValue;
            _item.Category_ID = (int)cmbCategory.SelectedValue;
            _item.Gender_ID = (int)cmbGender.SelectedValue;

            // Update sizes from binding list
            _item.Sizes = _sizesBindingList.ToList();

            if (picItemImage.Image != null)
            {
                using var ms = new MemoryStream();
                // Create a clone to avoid GDI+ errors
                using (var imageClone = new Bitmap(picItemImage.Image))
                {
                    imageClone.Save(ms, ImageFormat.Jpeg);
                }
                _item.ItemImage = ms.ToArray();
                //using var ms = new MemoryStream();
                //picItemImage.Image.Save(ms, ImageFormat.Jpeg);
                //_item.ItemImage = ms.ToArray();
            }
            else
            {
                _item.ItemImage = null;
            }
        }

        private bool ValidateInputs()
        {
            var errors = new List<string>();

            if (_item.Product_ID == 0 && !Regex.IsMatch(txtBarcode.Text, @"^\d{8}$"))
            {
                errors.Add("Barcode must be an 8-digit number");
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
                errors.Add("Description is required");
            else if (txtDescription.Text.Length > 200)
                errors.Add("Description cannot exceed 200 characters");

            if (!decimal.TryParse(txtMaxDiscount.Text, out decimal maxDiscount) ||
                maxDiscount < 0 || maxDiscount > 100)
            {
                errors.Add("Max discount must be between 0 and 100");
            }

            if (!int.TryParse(txtMinStock.Text, out int minStock) || minStock < 0)
                errors.Add("Min stock level cannot be negative");

            if (cmbBrand.SelectedItem == null)
                errors.Add("Please select a brand");

            if (cmbCategory.SelectedItem == null)
                errors.Add("Please select a category");

            if (cmbGender.SelectedItem == null)
                errors.Add("Please select a gender");

            // Validate sizes
            if (_sizesBindingList.Count == 0)
            {
                errors.Add("At least one size variant is required");
            }

            foreach (var size in _sizesBindingList)
            {
                if (size.Quantity < 0)
                    errors.Add("Quantity cannot be negative");

                if (size.RetailPrice <= 0)
                    errors.Add("Retail price must be greater than 0");

                if (size.UnitCost <= 0)
                    errors.Add("Unit cost must be greater than 0");

                if (size.RetailPrice < size.UnitCost)
                    errors.Add("Retail price must be greater than unit cost");
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, errors), "Validation Errors",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private static bool IsValidImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 8) return false;

            return bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 || // PNG
                   bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF ||                      // JPEG
                   bytes[0] == 0x42 && bytes[1] == 0x4D ||                                          // BMP
                   bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x38;    // GIF
        }

        private void TopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _startPos = e.Location;
        }

        private void TopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Location = PointToScreen(new Point(e.X - _startPos.X, e.Y - _startPos.Y));
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            txtBarcode.Text = _itemService.GenerateBarcode();
        }

        private void BrowseImage(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                var fileImage = Image.FromFile(openFileDialog.FileName);
                picItemImage.Image = fileImage;

                var imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                if (!IsValidImage(imageBytes))
                {
                    MessageBox.Show("Invalid image format", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                // Dispose previous image
                if (picItemImage.Image != null)
                {
                    var oldImage = picItemImage.Image;
                    picItemImage.Image = null; // Detach first
                    oldImage.Dispose();
                }

                var imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                if (!IsValidImage(imageBytes)) 
                { 
                }

                // Load via MemoryStream to avoid file locks
                using (var ms = new MemoryStream(imageBytes))
                {
                    picItemImage.Image = new Bitmap(ms); // Create standalone copy
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Error : {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSizeOptions();
        }

        private void BtnAddSize_Click(object sender, EventArgs e)
        {
            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Please select a category first to load available sizes");
                return;
            }

            var sizeColumn = dgvSizes.Columns["colSize"] as DataGridViewComboBoxColumn;

            // FIX: Use IList instead of casting to List<Size>
            var sizeList = sizeColumn?.DataSource as IList;
            if (sizeList == null || sizeList.Count == 0)
            {
                MessageBox.Show("No sizes available for the selected category");
                return;
            }

            // FIX: Get first item using IList indexer instead of LINQ
            object firstSize = sizeList[0];
            int firstSizeId = (int)firstSize.GetType().GetProperty("Size_ID").GetValue(firstSize);

            _sizesBindingList.Add(new ProductSize
            {
                Size_ID = firstSizeId,
                Quantity = 0,
                RetailPrice = 0,
                UnitCost = 0
            });

            // Select and scroll to the new row
            dgvSizes.ClearSelection();
            dgvSizes.Rows[_sizesBindingList.Count - 1].Selected = true;
            dgvSizes.FirstDisplayedScrollingRowIndex = _sizesBindingList.Count - 1;
        }

        private void BtnRemoveSize_Click(object sender, EventArgs e)
        {
            if (dgvSizes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a size variant to remove", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedSize = dgvSizes.SelectedRows[0].DataBoundItem as ProductSize;
            if (selectedSize != null)
            {
                _sizesBindingList.Remove(selectedSize);
            }
        }

        private void DgvSizes_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvSizes.Rows[e.RowIndex].IsNewRow) return;

            string headerText = dgvSizes.Columns[e.ColumnIndex].HeaderText;
            string value = e.FormattedValue?.ToString() ?? string.Empty;

            // Validate quantity
            if (headerText.Equals("Quantity"))
            {
                if (!int.TryParse(value, out int quantity) || quantity < 0)
                {
                    dgvSizes.Rows[e.RowIndex].ErrorText = "Quantity must be a positive integer";
                    e.Cancel = true;
                }
            }
            // Validate prices
            else if (headerText.Equals("Retail Price") || headerText.Equals("Unit Cost"))
            {
                if (!decimal.TryParse(value, out decimal price) || price <= 0)
                {
                    dgvSizes.Rows[e.RowIndex].ErrorText = "Price must be a positive number";
                    e.Cancel = true;
                }
            }
        }

        private void DgvSizes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear row error
            dgvSizes.Rows[e.RowIndex].ErrorText = null;

            // Validate retail price vs unit cost
            if (e.ColumnIndex == dgvSizes.Columns["colRetailPrice"].Index ||
                e.ColumnIndex == dgvSizes.Columns["colUnitCost"].Index)
            {
                var row = dgvSizes.Rows[e.RowIndex];
                if (row.DataBoundItem is ProductSize size)
                {
                    if (size.RetailPrice < size.UnitCost)
                    {
                        row.ErrorText = "Retail price must be greater than unit cost";
                    }
                }
            }
        }

        private void DgvSizes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Data error: {e.Exception.Message}", "Input Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.ThrowException = false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveItem();
        }

        private void SaveItem()
        {
            if (!ValidateInputs()) return;

            try
            {
                MapFormToItem();

                bool success = _item.Product_ID > 0 ?
                    _itemService.UpdateItem(_item) :
                    _itemService.AddItem(_item);

                if (success)
                {
                    // Only open print form if sizes exist
                    if (_item.Sizes != null && _item.Sizes.Count > 0)
                    {
                        using (var printForm = new BarcodePrintForm(_item))
                        {
                            printForm.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Item saved successfully! Add size variants to print barcodes.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Error saving item. It may already exist.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}