using pos_system.pos.BLL.Services;
using pos_system.pos.Core;
using pos_system.pos.Models;
using pos_system.pos.UI.Forms.Inventory;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using pos_system.pos.UI.Forms;
using pos_system;
using pos_system.pos;
using pos_system.pos.UI;

namespace pos_system.pos.UI.Forms.Inventory
{
    public partial class ItemForm : Form
    {
        private readonly Item _item;
        private readonly BrandService _brandService = new BrandService();
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly ItemService _itemService = new ItemService();
        private readonly SizeService _sizeService = new SizeService();
        private List<CategorySize> _currentSizes;
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
            lblTitle.Text = _item.Item_ID > 0 ? "Edit Item" : "Add Item";
            btnSave.Text = _item.Item_ID > 0 ? "Update" : "Save";

            if (_item.Item_ID > 0)
            {
                txtBarcode.ReadOnly = true;
                txtBarcode.BackColor = SystemColors.Control;
            }
            else
            {
                btnGenerate.Visible = true;
            }

            // Event handlers
            btnGenerate.Click += BtnGenerate_Click;
            btnBrowse.Click += BrowseImage;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            btnClose.Click += (s, e) => Close();
            btnMinimize.Click += (s, e) => WindowState = FormWindowState.Minimized;

            // Enable form dragging
            topPanel.MouseDown += TopPanel_MouseDown;
            topPanel.MouseMove += TopPanel_MouseMove;
            lblTitle.MouseDown += TopPanel_MouseDown;
            lblTitle.MouseMove += TopPanel_MouseMove;
        }

        private void LoadInitialData()
        {
            PopulateBrands();
            PopulateCategories();

            if (_item.Item_ID > 0)
            {
                txtBarcode.Text = _item.barcode;
                txtDescription.Text = _item.description;
                txtRetailPrice.Text = _item.RetailPrice.ToString("0.00");
                txtUnitCost.Text = _item.unitCost.ToString("0.00");
                txtQuantity.Text = _item.quantity.ToString();
                txtMaxDiscount.Text = _item.maxDiscount.ToString("0.00");
                txtMinStock.Text = _item.MinStockLevel.ToString();

                cmbBrand.SelectedValue = _item.Brand_ID;
                cmbCategory.SelectedValue = _item.Category_ID;

                LoadSizesForCategory();
                cmbSize.SelectedValue = _item.Size_ID ?? -1;

                if (_item.ItemImage != null)
                {
                    using (var ms = new MemoryStream(_item.ItemImage))
                    {
                        picItemImage.Image = CloneImage(Image.FromStream(ms));
                    }
                }
            }
        }

        private Image CloneImage(Image sourceImage)
        {
            if (sourceImage == null) return null;

            var bitmap = new Bitmap(sourceImage.Width, sourceImage.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(sourceImage, 0, 0, sourceImage.Width, sourceImage.Height);
            }
            return bitmap;
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

        private void LoadSizesForCategory()
        {
            if (cmbCategory.SelectedItem is not Category selectedCategory) return;

            _currentSizes = _sizeService.GetSizesByCategoryId(selectedCategory.Category_ID);
            var sizeList = new List<CategorySize>(_currentSizes)
            {
                new CategorySize { Size_ID = -1, SizeLabel = "No Size" }
            };

            cmbSize.DataSource = sizeList;
            cmbSize.DisplayMember = "SizeLabel";
            cmbSize.ValueMember = "Size_ID";
        }

        private void SaveItem()
        {
            if (!ValidateInputs()) return;

            try
            {
                MapFormToItem();

                bool success = _item.Item_ID > 0 ?
                    _itemService.UpdateItem(_item) :
                    _itemService.AddItem(_item);

                if (success)
                {
                    DialogResult = DialogResult.OK;

                    // Show barcode printing FIRST
                    if (_item.Item_ID == 0 || _item.Item_ID > 0)
                    {
                        var printForm = new BarcodePrintForm(_item);
                        printForm.ShowDialog();  // Show modal before closing
                    }

                    Close();  // Close AFTER showing print form
                }
                else
                {
                    MessageBox.Show("Item with same description or barcode already exists!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MapFormToItem()
        {
            if (_item.Item_ID == 0)
                _item.barcode = txtBarcode.Text;

            _item.description = txtDescription.Text;
            _item.RetailPrice = decimal.Parse(txtRetailPrice.Text);
            _item.unitCost = decimal.Parse(txtUnitCost.Text);
            _item.maxDiscount = decimal.Parse(txtMaxDiscount.Text);
            _item.MinStockLevel = int.Parse(txtMinStock.Text);
            _item.quantity = int.Parse(txtQuantity.Text);

            _item.Brand_ID = (int)cmbBrand.SelectedValue;
            _item.Category_ID = (int)cmbCategory.SelectedValue;

            _item.BrandName = (cmbBrand.SelectedItem as Brand)?.brandName;
            _item.CategoryName = (cmbCategory.SelectedItem as Category)?.categoryName;

            if (cmbSize.SelectedItem is CategorySize selectedSize && selectedSize.Size_ID != -1)
            {
                _item.SizeLabel = selectedSize.SizeLabel;
            }

            if ((int)cmbSize.SelectedValue != -1)
                _item.Size_ID = (int)cmbSize.SelectedValue;
            else
                _item.Size_ID = null;

            if (picItemImage.Image != null)
            {
                using var ms = new MemoryStream();
                picItemImage.Image.Save(ms, ImageFormat.Jpeg);
                _item.ItemImage = ms.ToArray();
            }
            else
            {
                _item.ItemImage = null;
            }
        }

        private bool ValidateInputs()
        {
            var errors = new List<string>();

            if (_item.Item_ID == 0 && !Regex.IsMatch(txtBarcode.Text, @"^\d{7}$"))
            {
                errors.Add("Barcode must be a 7-digit number");
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
                errors.Add("Description is required");
            else if (txtDescription.Text.Length > 200)
                errors.Add("Description cannot exceed 200 characters");

            if (!decimal.TryParse(txtRetailPrice.Text, out decimal retailPrice) || retailPrice < 0.01m)
                errors.Add("Retail price must be at least 0.01");

            if (!decimal.TryParse(txtUnitCost.Text, out decimal unitCost) || unitCost < 0.01m)
                errors.Add("Unit cost must be at least 0.01");

            if (retailPrice <= unitCost)
                errors.Add("Retail price must be greater than unit cost");

            if (cmbBrand.SelectedItem == null)
                errors.Add("Please select a brand");

            if (cmbCategory.SelectedItem == null)
                errors.Add("Please select a category");

            if (!decimal.TryParse(txtMaxDiscount.Text, out decimal maxDiscount) ||
                maxDiscount < 0 || maxDiscount > 100)
            {
                errors.Add("Max discount must be between 0 and 100");
            }

            if (!int.TryParse(txtMinStock.Text, out int minStock) || minStock < 0)
                errors.Add("Min stock level cannot be negative");

            if (!int.TryParse(txtQuantity.Text, out int qty) || qty < 0)
                errors.Add("Quantity cannot be negative");

            if (errors.Count == 0) return true;

            MessageBox.Show(string.Join(Environment.NewLine, errors), "Validation Errors",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private static bool IsValidImage(byte[] bytes)
        {
            if (bytes.Length < 8) return false;

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
                picItemImage.Image = CloneImage(fileImage);
                fileImage.Dispose();

                var imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                if (!IsValidImage(imageBytes))
                {
                    MessageBox.Show("Invalid image format", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                picItemImage.Image?.Dispose();
                picItemImage.Image = Image.FromFile(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSizesForCategory();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveItem();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}