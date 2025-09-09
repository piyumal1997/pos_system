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
        private string _tempImagePath;

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
                btnGenerate.Visible = false;
            }
            else
            {
                btnGenerate.Visible = true;
            }

            // Initialize sizes binding list
            _sizesBindingList = new BindingList<ProductSize>(_item.Sizes);
            dgvSizes.DataSource = _sizesBindingList;
            ConfigureGridColumns();

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
            btnClearImage.Click += BtnClearImage_Click;

            // NEW: Disable category combo for existing items with sizes
            if (_item.Product_ID > 0 && _item.Sizes.Any())
            {
                cmbCategory.Enabled = false;
            }

            // Enable form dragging
            topPanel.MouseDown += TopPanel_MouseDown;
            topPanel.MouseMove += TopPanel_MouseMove;
            lblTitle.MouseDown += TopPanel_MouseDown;
            lblTitle.MouseMove += TopPanel_MouseMove;

            cmbCategory.SelectedValueChanged += CmbCategory_SelectedValueChanged;
        }

        private void ConfigureGridColumns()
        {
            dgvSizes.Columns.Clear();

            // Size column
            DataGridViewComboBoxColumn sizeColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "Size_ID",
                HeaderText = "Size",
                Name = "colSize",
                Width = 120,
                FlatStyle = FlatStyle.Flat
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

            dgvSizes.AutoGenerateColumns = false;
            dgvSizes.AllowUserToAddRows = false;
            dgvSizes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void CmbCategory_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_sizesBindingList.Count == 0) return;

            cmbCategory.SelectedValueChanged -= CmbCategory_SelectedValueChanged;
            cmbCategory.SelectedValue = _item.Category_ID;
            cmbCategory.SelectedValueChanged += CmbCategory_SelectedValueChanged;
        }

        private void UpdateAddSizeButtonState()
        {
            btnAddSize.Enabled = cmbCategory.SelectedItem != null;
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

                // Load image using filename
                if (!string.IsNullOrEmpty(_item.ItemImage))
                {
                    try
                    {
                        picItemImage.Image = ImageHelper.LoadProductImage(_item.ItemImage);
                    }
                    catch
                    {
                        picItemImage.Image = ImageHelper.GenerateDefaultImage();
                    }
                }
                else
                {
                    picItemImage.Image = ImageHelper.GenerateDefaultImage();
                }
            }
            else
            {
                picItemImage.Image = ImageHelper.GenerateDefaultImage();
            }

            UpdateAddSizeButtonState();
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

            cmbCategory.SelectedValueChanged += (s, e) => UpdateAddSizeButtonState();
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
            _item.Sizes = _sizesBindingList.ToList();

            // Store temp image path for later processing
            _item.TempImagePath = _tempImagePath;

            // Clear actual image path - will be set after saving
            _item.ItemImage = null;
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
                maxDiscount < 0 || maxDiscount > 2000)
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


        //private bool ValidateInputs()
        //{
        //    var errors = new List<string>();

        //    if (_item.Product_ID == 0 && !Regex.IsMatch(txtBarcode.Text, @"^\d{8}$"))
        //    {
        //        errors.Add("Barcode must be an 8-digit number");
        //    }

        //    if (string.IsNullOrWhiteSpace(txtDescription.Text))
        //        errors.Add("Description is required");
        //    else if (txtDescription.Text.Length > 200)
        //        errors.Add("Description cannot exceed 200 characters");

        //    if (!decimal.TryParse(txtMaxDiscount.Text, out decimal maxDiscount) ||
        //        maxDiscount < 0 || maxDiscount > 2000)
        //    {
        //        errors.Add("Max discount must be between 0 and 100");
        //    }

        //    if (!int.TryParse(txtMinStock.Text, out int minStock) || minStock < 0)
        //        errors.Add("Min stock level cannot be negative");

        //    if (cmbBrand.SelectedItem == null)
        //        errors.Add("Please select a brand");

        //    if (cmbCategory.SelectedItem == null)
        //        errors.Add("Please select a category");

        //    if (cmbGender.SelectedItem == null)
        //        errors.Add("Please select a gender");

        //    if (_sizesBindingList.Count == 0)
        //    {
        //        errors.Add("At least one size variant is required");
        //    }

        //    foreach (var size in _sizesBindingList)
        //    {
        //        if (size.Quantity < 0)
        //            errors.Add("Quantity cannot be negative");

        //        if (size.RetailPrice <= 0)
        //            errors.Add("Retail price must be greater than 0");

        //        if (size.UnitCost <= 0)
        //            errors.Add("Unit cost must be greater than 0");

        //        if (size.RetailPrice < size.UnitCost)
        //            errors.Add("Retail price must be greater than unit cost");
        //    }

        //    if (errors.Count > 0)
        //    {
        //        MessageBox.Show(string.Join(Environment.NewLine, errors), "Validation Errors",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return false;
        //    }

        //    return true;
        //}

        private void BtnClearImage_Click(object sender, EventArgs e)
        {
            picItemImage.Image = null;
            _tempImagePath = null;
        }

        private void BrowseImage(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Create temp copy to avoid file locks
                    _tempImagePath = Path.GetTempFileName() + Path.GetExtension(openFileDialog.FileName);
                    File.Copy(openFileDialog.FileName, _tempImagePath, true);

                    // Load image via temp file
                    using (var image = Image.FromFile(_tempImagePath))
                    {
                        picItemImage.Image = new Bitmap(image);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            txtBarcode.Text = _itemService.GenerateBarcode();
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
            var sizeList = sizeColumn?.DataSource as IList;

            if (sizeList == null || sizeList.Count == 0)
            {
                MessageBox.Show("No sizes available for the selected category");
                return;
            }

            object firstSize = sizeList[0];
            int firstSizeId = (int)firstSize.GetType().GetProperty("Size_ID").GetValue(firstSize);

            _sizesBindingList.Add(new ProductSize
            {
                Size_ID = firstSizeId,
                Quantity = 0,
                RetailPrice = 0,
                UnitCost = 0
            });

            dgvSizes.ClearSelection();
            dgvSizes.Rows[_sizesBindingList.Count - 1].Selected = true;
            dgvSizes.FirstDisplayedScrollingRowIndex = _sizesBindingList.Count - 1;

            if (_sizesBindingList.Count == 1)
            {
                cmbCategory.Enabled = false;
            }
        }

        private void BtnRemoveSize_Click(object sender, EventArgs e)
        {
            if (dgvSizes.SelectedRows.Count == 0) return;

            var selectedSize = dgvSizes.SelectedRows[0].DataBoundItem as ProductSize;
            if (selectedSize != null)
            {
                _sizesBindingList.Remove(selectedSize);
            }

            if (_sizesBindingList.Count == 0)
            {
                cmbCategory.Enabled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveItem();
        }

        private void SaveItem()
        {
            try
            {
                if (!ValidateInputs()) return;

                MapFormToItem();

                bool success = false;
                int productId = 0;

                if (_item.Product_ID > 0)
                {
                    // Update existing item
                    success = _itemService.UpdateItem(_item);
                    productId = _item.Product_ID;
                }
                else
                {
                    // Add new item and get generated ID
                    productId = _itemService.AddItem(_item);
                    success = productId > 0;
                }

                if (success)
                {
                    // Handle image saving after item is saved
                    if (!string.IsNullOrEmpty(_item.TempImagePath))
                    {
                        try
                        {
                            // Save image using actual product ID
                            using (Image image = Image.FromFile(_item.TempImagePath))
                            {
                                string fileName = ImageHelper.SaveProductImage(image, productId);
                                _itemService.UpdateItemImage(productId, fileName);
                            }

                            // Clean up temp file
                            File.Delete(_item.TempImagePath);
                            _tempImagePath = null;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Image saving failed: {ex.Message}", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    if (_item.Sizes.Count > 0)
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
                    MessageBox.Show("Operation completed but no changes were made.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item:\n{ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // Clean up temp image if exists
            if (!string.IsNullOrEmpty(_tempImagePath) && File.Exists(_tempImagePath))
            {
                try
                {
                    File.Delete(_tempImagePath);
                }
                catch { /* Ignore deletion errors */ }
            }

            DialogResult = DialogResult.Cancel;
            Close();
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
    }
}