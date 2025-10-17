using pos_system.pos.BLL.Services;
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
    public partial class ItemsManagement : Form
    {
        private readonly ItemService _itemService = new ItemService();
        private DataGridView dgvItems;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnSearch;
        private TextBox txtSearch;
        private System.Windows.Forms.Timer searchTimer;

        // Theme colors
        private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
        private static readonly Color ForegroundColor = Color.Black;
        private static readonly Color SecondaryColor = Color.Gray;
        private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
        private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

        public ItemsManagement()
        {
            InitializeComponent();
            InitializeSearchTimer();
            LoadItems();
        }
        private void InitializeSearchTimer()
        {
            searchTimer = new System.Windows.Forms.Timer { Interval = 300 };
            searchTimer.Tick += (s, e) => {
                searchTimer.Stop();
                LoadItems(txtSearch.Text);
            };
        }
        private void InitializeComponent()
        {
            // Form setup
            this.Size = new Size(1200, 700);
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

            // Title panel
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "PRODUCT MANAGEMENT",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
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
                WrapContents = false,
                AutoScroll = true
            };

            // Toolbar buttons
            btnAdd = CreateToolbarButton("ADD NEW", PrimaryColor);
            btnEdit = CreateToolbarButton("EDIT", PrimaryColor);
            btnDelete = CreateToolbarButton("DELETE", DeleteColor);
            btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);
            btnSearch = CreateToolbarButton("SEARCH", PrimaryColor);

            txtSearch = new TextBox
            {
                Width = 220,
                Height = 80,
                Font = new Font("Segoe UI", 15, FontStyle.Regular),
                //Margin = new Padding(10, 0, 10, 0),
                Padding = new Padding(0, 40, 0, 0),
                ForeColor = ForegroundColor,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Barcode && Description"
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;


            // DataGrid
            dgvItems = new DataGridView
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
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
            };

            // Grid styling
            dgvItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 5, 10, 5)
            };
            //dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvItems.ColumnHeadersHeight = 50;

            dgvItems.RowTemplate.Height = 40;
            dgvItems.RowTemplate.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            dgvItems.RowTemplate.Height = 40;

            dgvItems.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BackgroundColor,
                ForeColor = ForegroundColor,
                Font = new Font("Segoe UI", 12),
                SelectionBackColor = SelectionColor,
                SelectionForeColor = ForegroundColor
            };

            dgvItems.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(245, 249, 255)
            };

            // Events
            btnAdd.Click += (s, e) => ShowItemForm();
            btnEdit.Click += (s, e) => EditItem();
            btnDelete.Click += (s, e) => DeleteItem();
            btnRefresh.Click += (s, e) => LoadItems();
            btnSearch.Click += (s, e) => LoadSearchItem();

            // Layout
            titlePanel.Controls.Add(lblTitle);
            toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh, btnSearch, txtSearch });

            container.Controls.Add(dgvItems);
            container.Controls.Add(toolbar);
            container.Controls.Add(titlePanel);

            this.Controls.Add(container);
            ConfigureGridColumns();

            // Handle form closing to clean up images
            this.FormClosing += (s, e) => CleanupImages();
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

        private void ConfigureGridColumns()
        {
            dgvItems.Columns.Clear();

            dgvItems.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Product_ID",
                    HeaderText = "ID",
                    Visible = false
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Barcode",
                    HeaderText = "BARCODE",
                    Width = 120,
                    ReadOnly = true
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Description",
                    HeaderText = "DESCRIPTION",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "BrandName",
                    HeaderText = "BRAND",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "CategoryName",
                    HeaderText = "CATEGORY",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "GenderName",
                    HeaderText = "GENDER",
                    Width = 40
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "SizesSummary",
                    HeaderText = "SIZES/STOCK",
                    Width = 320,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleLeft
                    }
                }
            );
        }
        private void LoadItems(string searchTerm = null)
        {
            // Clean up previous images
            CleanupImages();

            dgvItems.DataSource = null;
            var items = string.IsNullOrWhiteSpace(searchTerm)
                ? _itemService.GetAllItems()
                : _itemService.SearchItems(searchTerm);

            // Process items
            foreach (var item in items)
            {
                item.SizesSummary = GetSizesSummary(item.Sizes);
            }

            dgvItems.DataSource = items;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Search with 300ms delay to avoid excessive database calls
            if (searchTimer == null)
            {
                searchTimer = new System.Windows.Forms.Timer { Interval = 300 };
                searchTimer.Tick += (s, args) => {
                    searchTimer.Stop();
                    LoadItems(txtSearch.Text);
                };
            }

            searchTimer.Stop();
            searchTimer.Start();
        }

        private void CleanupImages()
        {
            if (dgvItems.DataSource is IEnumerable<Item> items)
            {
                foreach (var item in items)
                {
                    if (item.ImageObject != null)
                    {
                        item.ImageObject.Dispose();
                        item.ImageObject = null;
                    }
                }
            }
        }

        private string GetSizesSummary(List<ProductSize> sizes)
        {
            if (sizes == null || sizes.Count == 0)
                return "No Sizes";

            var summary = new List<string>();
            foreach (var size in sizes)
            {
                summary.Add($"{size.SizeLabel}: {size.Quantity}");
            }
            return string.Join(", ", summary);
        }

        private void LoadSearchItem()
        {
            using var form = new pos_system.pos.UI.Forms.Inventory.SearchItemForm();
            if (form.ShowDialog() == DialogResult.OK)
                LoadItems();
        }

        private void ShowItemForm(Item item = null)
        {
            using var form = new ItemForm(item);
            if (form.ShowDialog() == DialogResult.OK)
                LoadItems();
        }

        private void EditItem()
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                ShowMessage("Please select a product to edit");
                return;
            }

            var item = dgvItems.SelectedRows[0].DataBoundItem as Item;
            if (item == null) return;

            var fullItem = _itemService.GetItemById(item.Product_ID);
            ShowItemForm(fullItem);
        }

        private void DeleteItem()
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                ShowMessage("Please select a product to delete");
                return;
            }

            var item = dgvItems.SelectedRows[0].DataBoundItem as Item;
            if (item == null) return;

            if (ConfirmDelete($"Are you sure you want to delete '{item.Description}'?"))
            {
                if (_itemService.DeleteItem(item.Product_ID))
                    LoadItems();
                else
                    ShowMessage("Error deleting product");
            }
        }

        private void ShowMessage(string text)
        {
            MessageBox.Show(text, "Product Management",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ConfirmDelete(string message)
        {
            return MessageBox.Show(message, "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }
    }
}
