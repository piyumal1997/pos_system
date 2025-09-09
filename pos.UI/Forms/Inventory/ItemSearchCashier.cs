using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using pos_system.pos.BLL.Services;
using pos_system.pos.Core;
using pos_system.pos.Models;

namespace pos_system.pos.UI.Forms.Inventory
{
    public partial class ItemSearchCashier : Form
    {
        // Theme Colors
        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color LightBlue = Color.FromArgb(189, 224, 254);
        private readonly Color White = Color.White;
        private readonly Color DarkText = Color.FromArgb(64, 64, 64);
        private readonly Color SuccessGreen = Color.FromArgb(46, 204, 113);
        private readonly Color LightGray = Color.FromArgb(240, 240, 240);

        private readonly ItemService _itemService = new ItemService();
        private readonly BrandService _brandService = new BrandService();
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly SizeService _sizeService = new SizeService();
        private readonly GenderService _genderService = new GenderService();
        private Employee _currentUser;
        private List<Item> _searchResults = new List<Item>();
        private Item _selectedItem;

        // Control references
        private TextBox txtSearch;
        private ComboBox cboBrand;
        private ComboBox cboCategory;
        private ComboBox cboSize;
        private ComboBox cboGender;
        private NumericUpDown numMinPrice;
        private NumericUpDown numMaxPrice;
        private DataGridView dgvItems;
        private Button btnClear;
        private Button btnPreview;
        private TableLayoutPanel mainLayout;

        public ItemSearchCashier(Employee user)
        {
            _currentUser = user;
            InitializeComponent();
            InitializeForm();
            LoadFilterData();
            SearchItems();
        }

        private void InitializeForm()
        {
            // Form setup
            // Form setup
            this.Size = new Size(980, 656);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(10);

            // Create main table layout
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Padding = new Padding(0)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110)); // Filters
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Data Grid
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Buttons
            this.Controls.Add(mainLayout);

            // Create header panel
            CreateHeaderPanel();

            // Create filter panel
            CreateFilterPanel();

            // Create data grid
            CreateDataGridView();

            // Create buttons
            CreateButtonPanel();
        }

        private void CreateHeaderPanel()
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = PrimaryBlue,
                Padding = new Padding(10, 0, 0, 0),
                Height = 70
            };

            var lblTitle = new Label
            {
                Text = "Item Search",
                Dock = DockStyle.Left,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = White,
                AutoSize = true,
                Padding = new Padding(0, 3, 0, 0)
            };

            headerPanel.Controls.Add(lblTitle);
            mainLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void CreateFilterPanel()
        {
            var filterPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = LightGray,
                Padding = new Padding(20, 15, 20, 15)
            };

            // Search TextBox with underline
            txtSearch = new TextBox
            {
                Location = new Point(20, 15),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 11),
                BackColor = White,
                BorderStyle = BorderStyle.None,
                ForeColor = DarkText,
                PlaceholderText = "Barcode or Description"
            };
            txtSearch.TextChanged += (s, e) => SearchItems();
            filterPanel.Controls.Add(txtSearch);

            var txtSearchUnderline = new Panel
            {
                BackColor = Color.Gray,
                Location = new Point(20, 40),
                Size = new Size(250, 1)
            };
            filterPanel.Controls.Add(txtSearchUnderline);

            txtSearch.Enter += (s, e) => {
                txtSearchUnderline.BackColor = PrimaryBlue;
            };

            txtSearch.Leave += (s, e) => {
                txtSearchUnderline.BackColor = Color.Gray;
            };

            // Brand ComboBox
            cboBrand = CreateComboBox(filterPanel, "Brand:", 290, 15);
            cboBrand.SelectedIndexChanged += (s, e) => SearchItems();

            // Category ComboBox
            cboCategory = CreateComboBox(filterPanel, "Category:", 560, 15);
            cboCategory.SelectedIndexChanged += (s, e) => SearchItems();

            // Size ComboBox
            cboSize = CreateComboBox(filterPanel, "Size:", 20, 65);
            cboSize.SelectedIndexChanged += (s, e) => SearchItems();

            // Gender ComboBox
            cboGender = CreateComboBox(filterPanel, "Gender:", 290, 65);
            cboGender.SelectedIndexChanged += (s, e) => SearchItems();

            // Price Range
            var pricePanel = new Panel
            {
                Location = new Point(560, 65),
                Size = new Size(350, 30)
            };

            var lblPrice = new Label
            {
                Text = "Price Range:",
                Location = new Point(0, 5),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = DarkText,
                AutoSize = true
            };
            pricePanel.Controls.Add(lblPrice);

            numMinPrice = new NumericUpDown
            {
                Location = new Point(100, 3),
                Size = new Size(80, 25),
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 10000,
                Font = new Font("Segoe UI", 10)
            };
            numMinPrice.ValueChanged += (s, e) => SearchItems();
            pricePanel.Controls.Add(numMinPrice);

            var lblTo = new Label
            {
                Text = "to",
                Location = new Point(185, 5),
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            };
            pricePanel.Controls.Add(lblTo);

            numMaxPrice = new NumericUpDown
            {
                Location = new Point(210, 3),
                Size = new Size(80, 25),
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 10000,
                Font = new Font("Segoe UI", 10)
            };
            numMaxPrice.ValueChanged += (s, e) => SearchItems();
            pricePanel.Controls.Add(numMaxPrice);

            filterPanel.Controls.Add(pricePanel);
            mainLayout.Controls.Add(filterPanel, 0, 1);
        }

        private ComboBox CreateComboBox(Panel parent, string labelText, int x, int y)
        {
            var container = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(250, 40)
            };

            var label = new Label
            {
                Text = labelText,
                Location = new Point(0, 5),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = DarkText,
                AutoSize = true
            };
            container.Controls.Add(label);

            var combo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(80, 0),
                Size = new Size(160, 28),
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            container.Controls.Add(combo);
            parent.Controls.Add(container);

            return combo;
        }

        private void CreateDataGridView()
        {
            dgvItems = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 40,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 20 }
            };

            // Style headers
            dgvItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = LightBlue,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = DarkText,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.GridColor = LightGray;
            dgvItems.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            dgvItems.DefaultCellStyle.ForeColor = DarkText;
            dgvItems.DefaultCellStyle.BackColor = White;
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvItems.RowTemplate.Height = 35;

            dgvItems.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Barcode", HeaderText = "Barcode", DataPropertyName = "barcode", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", DataPropertyName = "description", FillWeight = 150 },
                new DataGridViewTextBoxColumn { Name = "Brand", HeaderText = "Brand", DataPropertyName = "BrandName", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", DataPropertyName = "CategoryName", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Size", HeaderText = "Size", DataPropertyName = "SizeLabel", Width = 70 },
                new DataGridViewTextBoxColumn
                {
                    Name = "Price",
                    HeaderText = "Price",
                    DataPropertyName = "RetailPrice",
                    //DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" },
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight },
                    Width = 80
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Quantity",
                    HeaderText = "Qty",
                    DataPropertyName = "quantity",
                    Width = 60,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                }
                //new DataGridViewImageColumn
                //{
                //    DataPropertyName = "ItemImage",
                //    HeaderText = "Image",
                //    ImageLayout = DataGridViewImageCellLayout.Zoom,
                //    Width = 80
                //}
            );

            dgvItems.DataSource = new BindingList<Item>(_searchResults);
            dgvItems.SelectionChanged += (s, e) =>
            {
                if (dgvItems.SelectedRows.Count > 0)
                {
                    _selectedItem = dgvItems.SelectedRows[0].DataBoundItem as Item;
                    btnPreview.Enabled = true;
                }
                else
                {
                    btnPreview.Enabled = false;
                }
            };

            mainLayout.Controls.Add(dgvItems, 0, 2);
        }

        private void CreateButtonPanel()
        {
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = LightGray,
                Padding = new Padding(20, 10, 20, 0)
            };

            btnClear = new Button
            {
                Text = "Clear Filters",
                Size = new Size(140, 40),
                Location = new Point(20, 10),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = PrimaryBlue,
                ForeColor = White,
                Cursor = Cursors.Hand
            };
            btnClear.Click += (s, e) => ClearFilters();
            buttonPanel.Controls.Add(btnClear);

            btnPreview = new Button
            {
                Text = "Preview Item",
                Size = new Size(140, 40),
                Location = new Point(180, 10),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = SuccessGreen,
                ForeColor = White,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnPreview.Click += (s, e) => PreviewItem();
            buttonPanel.Controls.Add(btnPreview);

            mainLayout.Controls.Add(buttonPanel, 0, 3);
        }

        private void LoadFilterData()
        {
            // Load brands
            cboBrand.Items.Add(new { Text = "All Brands", Id = 0 });
            foreach (var brand in _brandService.GetAllBrands())
            {
                cboBrand.Items.Add(new { Text = brand.brandName, Id = brand.Brand_ID });
            }
            cboBrand.DisplayMember = "Text";
            cboBrand.ValueMember = "Id";
            cboBrand.SelectedIndex = 0;

            // Load categories
            cboCategory.Items.Add(new { Text = "All Categories", Id = 0 });
            foreach (var category in _categoryService.GetAllCategorie())
            {
                cboCategory.Items.Add(new { Text = category.categoryName, Id = category.Category_ID });
            }
            cboCategory.DisplayMember = "Text";
            cboCategory.ValueMember = "Id";
            cboCategory.SelectedIndex = 0;

            // Load sizes
            cboSize.Items.Add(new { Text = "All Sizes", Id = 0 });
            foreach (var size in _sizeService.GetAllSizes())
            {
                cboSize.Items.Add(new { Text = size.SizeLabel, Id = size.Size_ID });
            }
            cboSize.DisplayMember = "Text";
            cboSize.ValueMember = "Id";
            cboSize.SelectedIndex = 0;

            // Load genders
            cboGender.Items.Add(new { Text = "All Genders", Id = 0 });
            foreach (var gender in _genderService.GetAllGenders())
            {
                cboGender.Items.Add(new { Text = gender.GenderName, Id = gender.Gender_ID });
            }
            cboGender.DisplayMember = "Text";
            cboGender.ValueMember = "Id";
            cboGender.SelectedIndex = 0;
        }

        private void SearchItems()
        {
            var searchTerm = txtSearch.Text;
            var brandId = (cboBrand.SelectedItem as dynamic)?.Id ?? 0;
            var categoryId = (cboCategory.SelectedItem as dynamic)?.Id ?? 0;
            var sizeId = (cboSize.SelectedItem as dynamic)?.Id ?? 0;
            var genderId = (cboGender.SelectedItem as dynamic)?.Id ?? 0;
            var minPrice = numMinPrice.Value;
            var maxPrice = numMaxPrice.Value > minPrice ? numMaxPrice.Value : 10000;

            _searchResults = _itemService.SearchItemsWithFilters(
                searchTerm,
                brandId,
                categoryId,
                sizeId,
                genderId,
                minPrice,
                maxPrice
            );

            dgvItems.DataSource = new BindingList<Item>(_searchResults);

            // Visual feedback for no results
            if (_searchResults.Count == 0)
            {
                dgvItems.BackgroundColor = LightGray;
                dgvItems.DefaultCellStyle.BackColor = LightGray;
                dgvItems.DefaultCellStyle.ForeColor = Color.DarkGray;
                dgvItems.DefaultCellStyle.SelectionBackColor = Color.LightSlateGray;
            }
            else
            {
                dgvItems.BackgroundColor = White;
                dgvItems.DefaultCellStyle.BackColor = White;
                dgvItems.DefaultCellStyle.ForeColor = DarkText;
                dgvItems.DefaultCellStyle.SelectionBackColor = LightBlue;
            }
        }

        private void ClearFilters()
        {
            txtSearch.Text = string.Empty;
            cboBrand.SelectedIndex = 0;
            cboCategory.SelectedIndex = 0;
            cboSize.SelectedIndex = 0;
            cboGender.SelectedIndex = 0;
            numMinPrice.Value = 0;
            numMaxPrice.Value = 0;
            SearchItems();
        }

        private void PreviewItem()
        {
            if (_selectedItem == null) return;

            var previewForm = new Form
            {
                Text = "Item Preview",
                Size = new Size(500, 550),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = White,
                FormBorderStyle = FormBorderStyle.None,
                MaximizeBox = false,
                Padding = new Padding(0),
            };

            new DropShadow().ApplyShadows(previewForm);

            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(0)
            };
            previewForm.Controls.Add(mainContainer);

            // Create table layout for preview form
            var previewLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            previewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 250)); // Image
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Details
            mainContainer.Controls.Add(previewLayout);

            // Header panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = PrimaryBlue,
                Height = 40
            };

            var lblTitle = new Label
            {
                Text = "ITEM DETAILS",
                Dock = DockStyle.Left,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = White,
                AutoSize = true,
                Padding = new Padding(20, 10, 0, 0)
            };
            headerPanel.Controls.Add(lblTitle);

            var btnClose = new Button
            {
                Text = "✕",
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12),
                ForeColor = White,
                BackColor = Color.Transparent,
                Size = new Size(40, 40),
                FlatAppearance = { BorderSize = 0 }
            };
            btnClose.Click += (s, e) => previewForm.Close();
            headerPanel.Controls.Add(btnClose);
            mainContainer.Controls.Add(headerPanel);

            // Image panel
            var pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(20,0,20,0)
            };

            if (!string.IsNullOrEmpty(_selectedItem.ItemImage))
            {
                try
                {
                    // Use ImageHelper to load image from filename
                    pictureBox.Image = ImageHelper.LoadProductImage(_selectedItem.ItemImage);
                }
                catch
                {
                    // Fallback to default image on error
                    SetNoImageLabel(pictureBox);
                }
            }
            else
            {
                // No image filename available
                SetNoImageLabel(pictureBox);
            }
            previewLayout.Controls.Add(pictureBox, 0, 0);

            // Create details table
            var detailsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                AutoScroll = false,
                AutoSize = true,
                Padding = new Padding(20, 5, 20, 20),
                Font = new Font("Segoe UI", 10),
                BackColor = White
            };
            detailsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120)); // Field names
            detailsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Values

            // Add rows with fixed height
            for (int i = 0; i < 8; i++)
            {
                detailsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            }

            // Add details to table
            AddTableRow(detailsTable, "BARCODE:", _selectedItem.Barcode, 0);
            AddTableRow(detailsTable, "DESCRIPTION:", _selectedItem.Description, 1);
            AddTableRow(detailsTable, "BRAND:", _selectedItem.BrandName, 2);
            AddTableRow(detailsTable, "CATEGORY:", _selectedItem.CategoryName, 3);
            AddTableRow(detailsTable, "SIZE:", _selectedItem.SizeLabel, 4);
            AddTableRow(detailsTable, "GENDER:", _selectedItem.GenderName, 5);
            AddTableRow(detailsTable, "PRICE:", _selectedItem.RetailPrice.ToString("N2"), 6);
            AddTableRow(detailsTable, "QUANTITY:", _selectedItem.Quantity.ToString(), 7);

            previewLayout.Controls.Add(detailsTable, 0, 1);
            previewForm.ShowDialog();
        }

        // Helper method to add table rows
        private void AddTableRow(TableLayoutPanel table, string label, string value, int row)
        {
            var lblField = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = DarkText,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 10),
                ForeColor = DarkText,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            table.Controls.Add(lblField, 0, row);
            table.Controls.Add(lblValue, 1, row);
        }

        // Set no image label
        private void SetNoImageLabel(PictureBox pictureBox)
        {
            pictureBox.BackColor = LightGray;
            var lblNoImage = new Label
            {
                Text = "No Image Available",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.DarkGray
            };
            pictureBox.Controls.Add(lblNoImage);
        }
    }
}