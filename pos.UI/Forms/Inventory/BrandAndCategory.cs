using pos_system.pos.BLL.Services;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static pos_system.pos.UI.Forms.Dashboard.OwnerDashboard;

namespace pos_system.pos.UI.Forms.Inventory
{
    public partial class BrandAndCategory : Form
    {
        private readonly BrandService _brandService = new BrandService();
        private readonly CategoryService _categoryService = new CategoryService();
        private TabControl tabControl;
        private DataGridView dgvBrands;
        private DataGridView dgvCategories;

        // Theme colors based on LoginForm
        private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
        private static readonly Color ForegroundColor = Color.Black;
        private static readonly Color SecondaryColor = Color.Gray;
        private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
        private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

        private DataGridView dgvSizes;
        private readonly SizeService _sizeService = new SizeService();

        public BrandAndCategory()
        {
            InitializeComponent();
            LoadBrands();
            LoadCategories();
            LoadSizes();
        }

        private Panel CreateSizeManagementLayout()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = BackgroundColor };

            // Title panel
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "SIZE MANAGEMENT",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Padding = new Padding(20, 0, 0, 0)
            };

            // Toolbar
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 15, 0, 15),
                BackColor = HeaderColor,
                WrapContents = false
            };

            // Toolbar buttons
            var btnAdd = CreateToolbarButton("ADD", PrimaryColor);
            var btnEdit = CreateToolbarButton("EDIT", PrimaryColor);
            var btnDelete = CreateToolbarButton("DELETE", DeleteColor);
            var btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);

            // DataGrid
            dgvSizes = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = BackgroundColor,
                ForeColor = ForegroundColor,
                AutoGenerateColumns = false,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };

            // Grid styling
            dgvSizes.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };

            dgvSizes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvSizes.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BackgroundColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 11),
                SelectionBackColor = SelectionColor,
                SelectionForeColor = ForegroundColor,
                Padding = new Padding(10, 5, 10, 5),
            };

            dgvSizes.RowTemplate.Height = 32;
            dgvSizes.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 249, 255)
            };

            // Events
            btnAdd.Click += (s, e) => ShowSizeForm();
            btnEdit.Click += (s, e) => EditSize();
            btnDelete.Click += (s, e) => DeleteSize();
            btnRefresh.Click += (s, e) => LoadSizes();

            // Layout
            titlePanel.Controls.Add(lblTitle);
            toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });
            panel.Controls.Add(dgvSizes);
            panel.Controls.Add(toolbar);
            panel.Controls.Add(titlePanel);

            // Configure columns
            dgvSizes.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Size_ID",
                    HeaderText = "ID",
                    Visible = false
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "SizeLabel",
                    HeaderText = "SIZE LABEL",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "SizeType",
                    HeaderText = "SIZE TYPE",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                }
            );

            LoadSizes();

            return panel;
        }

        private void LoadSizes()
        {
            dgvSizes.DataSource = null;
            dgvSizes.DataSource = _sizeService.GetAllSize();
            dgvSizes.ClearSelection();
        }

        private void ShowSizeForm(Sizes size = null)
        {
            using var form = new SizeFormEditor(size);
            if (form.ShowDialog() == DialogResult.OK)
                LoadSizes();
        }

        private void EditSize()
        {
            if (dgvSizes.SelectedRows.Count == 0) return;
            dynamic size = dgvSizes.SelectedRows[0].DataBoundItem;
            ShowSizeForm(new Sizes
            {
                Size_ID = size.Size_ID,
                SizeLabel = size.SizeLabel,
                SizeType = size.SizeType
            });
        }

        private void DeleteSize()
        {
            if (dgvSizes.SelectedRows.Count == 0) return;
            dynamic size = dgvSizes.SelectedRows[0].DataBoundItem;

            if (ConfirmAction($"Delete size '{size.SizeLabel}' ({size.SizeType})?"))
            {
                if (_sizeService.DeleteSize(size.Size_ID))
                    LoadSizes();
                else
                    ShowMessage("Error deleting size");
            }
        }

        private void InitializeComponent()
        {
            // Form setup
            this.Size = new Size(980, 656);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = BackgroundColor;

            // Main container
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundColor,
                Padding = new Padding(20)
            };

            // Tab control with modern styling
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.FlatButtons,
                ItemSize = new Size(180, 40),
                SizeMode = TabSizeMode.Fixed,
                DrawMode = TabDrawMode.OwnerDrawFixed
            };

            // Custom tab drawing
            tabControl.DrawItem += (sender, e) =>
            {
                var tabPage = tabControl.TabPages[e.Index];
                var tabRect = tabControl.GetTabRect(e.Index);
                var isSelected = tabControl.SelectedIndex == e.Index;

                // Background
                using (var brush = new SolidBrush(isSelected ? PrimaryColor : HeaderColor))
                    e.Graphics.FillRectangle(brush, tabRect);

                // Text
                TextRenderer.DrawText(
                    e.Graphics,
                    tabPage.Text,
                    new Font("Segoe UI", 13, isSelected ? FontStyle.Bold : FontStyle.Regular),
                    tabRect,
                    isSelected ? Color.White : Color.Black,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            };

            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;

            // Brands tab
            var tabBrands = new TabPage { Text = "BRANDS" };
            tabBrands.BackColor = BackgroundColor;
            tabBrands.Controls.Add(CreateBrandManagementLayout());

            // Categories tab
            var tabCategories = new TabPage { Text = "CATEGORIES" };
            tabCategories.BackColor = BackgroundColor;
            tabCategories.Controls.Add(CreateCategoryManagementLayout());

            tabControl.TabPages.Add(tabBrands);
            tabControl.TabPages.Add(tabCategories);
            container.Controls.Add(tabControl);
            this.Controls.Add(container);

            var tabSizes = new TabPage { Text = "SIZES" };
            tabSizes.BackColor = BackgroundColor;
            tabSizes.Controls.Add(CreateSizeManagementLayout());
            tabControl.TabPages.Add(tabSizes);
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab?.Text == "SIZES")
            {
                LoadSizes();
            }
        }

        private Panel CreateBrandManagementLayout()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = BackgroundColor };

            // Title panel
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "BRAND MANAGEMENT",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Padding = new Padding(20, 0, 0, 0)
            };

            // Toolbar
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 15, 0, 15),
                BackColor = HeaderColor,
                WrapContents = false
            };

            // Toolbar buttons
            var btnAdd = CreateToolbarButton("ADD", PrimaryColor);
            var btnEdit = CreateToolbarButton("EDIT", PrimaryColor);
            var btnDelete = CreateToolbarButton("DELETE", DeleteColor);
            var btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);

            // DataGrid
            dgvBrands = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = BackgroundColor,
                ForeColor = ForegroundColor,
                AutoGenerateColumns = false,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };

            dgvBrands.RowTemplate.Height = 32;
            // Grid styling
            dgvBrands.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };

            dgvBrands.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvBrands.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BackgroundColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 11),
                SelectionBackColor = SelectionColor,
                SelectionForeColor = ForegroundColor,
                Padding = new Padding(10, 5, 10, 5),
            };


            dgvBrands.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 249, 255)
            };

            // Events
            btnAdd.Click += (s, e) => ShowBrandForm();
            btnEdit.Click += (s, e) => EditBrand();
            btnDelete.Click += (s, e) => DeleteBrand();
            btnRefresh.Click += (s, e) => LoadBrands();

            // Layout
            titlePanel.Controls.Add(lblTitle);
            toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });
            panel.Controls.Add(dgvBrands);
            panel.Controls.Add(toolbar);
            panel.Controls.Add(titlePanel);

            // Configure columns
            dgvBrands.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Brand_ID",
                    Name = "Brand_ID",
                    HeaderText = "ID",
                    Visible = false
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "brandName",
                    Name = "brandName",
                    HeaderText = "BRAND NAME",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                }
            );

            return panel;
        }

        private Panel CreateCategoryManagementLayout()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = BackgroundColor };

            // Title panel
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "CATEGORY MANAGEMENT",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Padding = new Padding(20, 0, 0, 0)
            };

            // Toolbar
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 15, 0, 15),
                BackColor = HeaderColor,
                WrapContents = false
            };

            // Toolbar buttons
            var btnAdd = CreateToolbarButton("ADD", PrimaryColor);
            var btnEdit = CreateToolbarButton("EDIT", PrimaryColor);
            var btnDelete = CreateToolbarButton("DELETE", DeleteColor);
            var btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);

            // DataGrid
            dgvCategories = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = BackgroundColor,
                ForeColor = ForegroundColor,
                AutoGenerateColumns = false,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };

            // Grid styling
            dgvCategories.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };

            dgvCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvCategories.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BackgroundColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 11),
                SelectionBackColor = SelectionColor,
                SelectionForeColor = ForegroundColor,
                Padding = new Padding(10, 5, 10, 5),

            };

            dgvCategories.RowTemplate.Height = 32;
            dgvCategories.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 249, 255)
            };

            // Events
            btnAdd.Click += (s, e) => ShowCategoryForm();
            btnEdit.Click += (s, e) => EditCategory();
            btnDelete.Click += (s, e) => DeleteCategory();
            btnRefresh.Click += (s, e) => LoadCategories();

            // Layout
            titlePanel.Controls.Add(lblTitle);
            toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });
            panel.Controls.Add(dgvCategories);
            panel.Controls.Add(toolbar);
            panel.Controls.Add(titlePanel);

            // Configure columns
            dgvCategories.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Category_ID",
                    Name = "Category_ID",
                    HeaderText = "ID",
                    Visible = false
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "categoryName",
                    Name = "categoryName",
                    HeaderText = "CATEGORY NAME",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                }
            );

            return panel;
        }

        private Button CreateToolbarButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Size = new Size(120, 45),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = {
                BorderSize = 0,
                MouseOverBackColor = ControlPaint.Light(backColor, 0.2f)
            },
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Margin = new Padding(10, 0, 10, 0),
                Cursor = Cursors.Hand,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
        }

        private void LoadBrands() => dgvBrands.DataSource = _brandService.GetAllBrands();

        private void LoadCategories() => dgvCategories.DataSource = _categoryService.GetAllCategories();

        private void EditBrand()
        {
            if (dgvBrands.SelectedRows.Count == 0) return;

            // Cast directly to your Brand object
            var brand = dgvBrands.SelectedRows[0].DataBoundItem as Brand;
            Debug.WriteLine($"Data {brand}");
            if (brand == null) return;

            ShowBrandForm(brand);
        }

        private void ShowBrandForm(Brand brand = null)
        {
            using var form = new BrandFormEditor(brand);
            if (form.ShowDialog() == DialogResult.OK)
                LoadBrands();
        }

        private void DeleteBrand()
        {
            if (dgvBrands.SelectedRows.Count == 0) return;

            var brand = dgvBrands.SelectedRows[0].DataBoundItem as Brand;
            if (brand == null) return;

            if (ConfirmAction($"Delete brand '{brand.brandName}'?"))
            {
                if (_brandService.DeleteBrand(brand.Brand_ID))
                    LoadBrands();
                else
                    ShowMessage("Error deleting brand");
            }
        }

        private void ShowCategoryForm(Category category = null)
        {
            using var form = new CategoryFormEditor(category);
            if (form.ShowDialog() == DialogResult.OK)
                LoadCategories();
        }

        private void EditCategory()
        {
            if (dgvCategories.SelectedRows.Count == 0) return;

            var row = (dgvCategories.SelectedRows[0].DataBoundItem as DataRowView)?.Row;
            if (row == null) return;

            var category = new Category
            {
                Category_ID = (int)row["Category_ID"],
                categoryName = row["categoryName"].ToString()
            };

            ShowCategoryForm(category);
        }

        private void DeleteCategory()
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                ShowMessage("Please select a category to delete");
                return;
            }

            var row = dgvCategories.SelectedRows[0];
            int id = (int)row.Cells["Category_ID"].Value;
            string name = row.Cells["categoryName"].Value?.ToString() ?? string.Empty;

            if (ConfirmAction($"Delete category '{name}'?"))
            {
                if (_categoryService.DeleteCategory(id))
                    LoadCategories();
                else
                    ShowMessage("Error deleting category");
            }
        }

        private void ShowMessage(string text)
        {
            MessageBox.Show(text, "Brand & Category",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ConfirmAction(string message)
        {
            return MessageBox.Show(message, "Confirm Action",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
