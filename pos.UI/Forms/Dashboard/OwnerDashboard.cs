using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using pos_system.pos.Models;
using System.Security.Cryptography;
using System.IO;
using pos_system.pos.BLL.Services;
using pos_system.pos.BLL.Utilities;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.DAL;
using static pos_system.pos.UI.Forms.OwnerDashboard;
using pos_system.pos.UI.Forms;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Diagnostics;
using Neodynamic.Windows.ThermalLabelEditor;
using System.Drawing.Printing;
using Neodynamic.SDK.Barcode;
using System.Drawing.Drawing2D;
using System.Drawing.Text;


namespace pos_system.pos.UI.Forms
{
    public partial class OwnerDashboard : Form
    {
        public Employee _currentUser;


        public static Color BackgroundColor => Color.FromArgb(214, 208, 208); // Dark background
        public static Color ForegroundColor => Color.WhiteSmoke;
        public static Color HeaderColor => Color.FromArgb(170, 170, 170);
        public static Color GridLineColor => Color.FromArgb(70, 70, 70);
        public static Color SelectionColor => Color.FromArgb(0, 120, 215); // Accent color

        public static void ShowThemedMessage(string message)
        {
            using (var msgBox = new ThemedMessageBox(message))
            {
                msgBox.ShowDialog();
            }
        }

        public OwnerDashboard(Employee user)
        {
            InitializeComponent(); // Designer initialization
            _currentUser = user;

            // Set user-specific UI components
            lblWelcome.Text = $"Welcome, {_currentUser.firstName} {_currentUser.lastName}";
            btnClose.Click += (s, e) => Application.Exit();

            // Create sidebar buttons
            CreateSidebarButton("Dashboard", "🏠", 80);
            CreateSidebarButton("Items", "📦", 140);
            CreateSidebarButton("Employees", "👥", 200);
            CreateSidebarButton("Reports", "📊", 260);
            CreateSidebarButton("Brand && Category", "🏷️", 320);
            CreateSidebarButton("Barcode Print", "🖨️", 380);
            CreateSidebarButton("Logout", "🔒", 500);

            OpenChildForm(new DashboardView(), _dashboardButton);
        }

        private void CreateSidebarButton(string text, string icon, int yPos)
        {
            Button btn = new Button();
            btn.Text = $"{icon}  {text}";
            btn.Tag = text;
            btn.ForeColor = Color.FromArgb(71, 71, 71);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 11);
            btn.Size = new Size(220, 50);
            btn.Location = new Point(0, yPos);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(20, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

            if (text == "Dashboard") _dashboardButton = btn;

            // Hover Effects
            btn.MouseEnter += (s, e) =>
            {
                if (btn != _currentButton) btn.BackColor = Color.FromArgb(225, 225, 225);
            };
            btn.MouseLeave += (s, e) =>
            {
                if (btn != _currentButton) btn.BackColor = Color.Transparent;
            };

            btn.Click += (s, e) =>
            {
                ActivateButton(btn);
                switch (text)
                {
                    case "Dashboard":
                        OpenChildForm(new DashboardView(), btn);
                        break;
                    case "Items":
                        OpenChildForm(new ItemsManagement(), btn);
                        break;
                    case "Brand && Category":
                        OpenChildForm(new BrandAndCategory(), btn);
                        break;
                    case "Employees":
                        OpenChildForm(new EmployeesManagement(), btn);
                        break;
                    case "Reports":
                        OpenChildForm(new ReportsView(), btn);
                        break;
                    case "Barcode Print":
                        OpenChildForm(new BarcodePrint(), btn);
                        break;
                    case "Logout":
                        Logout();
                        break;
                }
            };

            _leftPanel.Controls.Add(btn);
        }

        private void ActivateButton(Button btn)
        {
            if (btn == null) return;
            if (_currentButton != null)
            {
                _currentButton.BackColor = Color.Transparent;
                _currentButton.ForeColor = Color.Gray;
            }
            btn.BackColor = Color.FromArgb(41, 128, 185); ;
            btn.ForeColor = Color.White;
            _currentButton = btn;
        }

        private void OpenChildForm(Form childForm, Button btn)
        {
            if (_activeForm != null) _activeForm.Close();
            ActivateButton(btn);
            _activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            _mainPanel.Controls.Add(childForm);
            _mainPanel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void Logout()
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }


        // Nested view classes
        public class DashboardView : Form { }
        public partial class ItemsManagement : Form
        {
            private readonly ItemService _itemService = new ItemService();
            private DataGridView dgvItems = null!;
            private Button btnAdd = null!;
            private Button btnEdit = null!;
            private Button btnDelete = null!;
            private Button btnRefresh = null!;
            private Button btnSearch = null!;

            // Theme colors based on LoginForm
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
                LoadItems();
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

                // Title panel (matches login form's blue)
                var titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor
                };

                var lblTitle = new Label
                {
                    Text = "ITEM MANAGEMENT",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.White,
                    Padding = new Padding(20, 0, 0, 0)
                };

                // Toolbar (light blue background)
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
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                };

                // Grid styling
                dgvItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };
                dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

                dgvItems.RowTemplate.Height = 80;
                dgvItems.RowTemplate.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);

                dgvItems.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor
                };

                dgvItems.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                // Grid scrollbar styling
                dgvItems.AdvancedRowHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                dgvItems.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

                // Events
                btnAdd.Click += (s, e) => ShowItemForm();
                btnEdit.Click += (s, e) => EditItem();
                btnDelete.Click += (s, e) => DeleteItem();
                btnRefresh.Click += (s, e) => LoadItems();
                btnSearch.Click += (s, e) => LoadSearchItem();

                // Layout
                titlePanel.Controls.Add(lblTitle);
                toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh, btnSearch });

                container.Controls.Add(dgvItems);
                container.Controls.Add(toolbar);
                container.Controls.Add(titlePanel);

                this.Controls.Add(container);
                ConfigureGridColumns();
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
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
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
                        DataPropertyName = "Item_ID",
                        HeaderText = "ID",
                        Visible = false
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "barcode",
                        HeaderText = "BARCODE",
                        Width = 150
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "description",
                        HeaderText = "DESCRIPTION",
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "BrandName",
                        HeaderText = "BRAND",
                        Width = 120
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "CategoryName",
                        HeaderText = "CATEGORY",
                        Width = 150
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "SizeLabel", // New size column
                        HeaderText = "SIZE",
                        Width = 80
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "RetailPrice",
                        HeaderText = "PRICE",
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Format = "C2",
                            Alignment = DataGridViewContentAlignment.MiddleRight
                        },
                        Width = 100
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "quantity",
                        HeaderText = "STOCK",
                        Width = 80,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleRight
                        }
                    },
                    new DataGridViewImageColumn
                    {
                        DataPropertyName = "ItemImage",
                        HeaderText = "IMAGE",
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        Width = 80,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Padding = new Padding(5),
                            Alignment = DataGridViewContentAlignment.MiddleCenter
                        }
                    }
                );
            }

            private void LoadItems() => dgvItems.DataSource = _itemService.GetAllItems();

            private void LoadSearchItem()
            {
                using var form = new SearchItemForm();
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
                    ShowMessage("Please select an item to edit");
                    return;
                }

                var item = dgvItems.SelectedRows[0].DataBoundItem as Item;
                if (item == null) return;

                ShowItemForm(item);
            }

            private void DeleteItem()
            {
                if (dgvItems.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select an item to delete");
                    return;
                }

                var item = dgvItems.SelectedRows[0].DataBoundItem as Item;
                if (item == null) return;

                if (ConfirmDelete($"Are you sure you want to delete '{item.description}'?"))
                {
                    if (_itemService.DeleteItem(item.Item_ID))
                        LoadItems();
                    else
                        ShowMessage("Error deleting item");
                }
            }

            private void ShowMessage(string text)
            {
                MessageBox.Show(text, "Item Management",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            private bool ConfirmDelete(string message)
            {
                return MessageBox.Show(message, "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }
        }
        public class EmployeesManagement : Form
        {
            private readonly EmployeeService _service = new EmployeeService();
            private DataGridView dgvEmployees = null!;
            private Button btnAdd = null!;
            private Button btnEdit = null!;
            private Button btnToggleStatus = null!;
            private Button btnRefresh = null!;

            // Theme colors based on LoginForm
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
            private static readonly Color ForegroundColor = Color.Black;
            private static readonly Color SecondaryColor = Color.Gray;
            private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
            private static readonly Color WarningColor = Color.FromArgb(241, 196, 15);
            private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

            public EmployeesManagement()
            {
                InitializeComponents();
                LoadEmployees();
            }

            private void InitializeComponents()
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

                // Title panel
                var titlePanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = PrimaryColor
                };

                var lblTitle = new Label
                {
                    Text = "EMPLOYEE MANAGEMENT",
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
                btnToggleStatus = CreateToolbarButton("DELETE", DeleteColor);
                btnRefresh = CreateToolbarButton("REFRESH", PrimaryColor);

                // DataGrid
                dgvEmployees = new DataGridView
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
                dgvEmployees.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };

                dgvEmployees.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dgvEmployees.RowTemplate.Height = 100;
                dgvEmployees.RowTemplate.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);

                dgvEmployees.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor
                };

                dgvEmployees.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                // Events
                btnAdd.Click += (s, e) => ShowEmployeeForm();
                btnEdit.Click += (s, e) => EditEmployee();
                btnToggleStatus.Click += (s, e) => ToggleStatus();
                btnRefresh.Click += (s, e) => LoadEmployees();

                // Layout
                titlePanel.Controls.Add(lblTitle);
                toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnToggleStatus, btnRefresh });

                container.Controls.Add(dgvEmployees);
                container.Controls.Add(toolbar);
                container.Controls.Add(titlePanel);

                this.Controls.Add(container);
                ConfigureGridColumns();
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
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Margin = new Padding(10, 0, 10, 0),
                    Cursor = Cursors.Hand,
                    TextImageRelation = TextImageRelation.ImageBeforeText
                };
            }

            private void ConfigureGridColumns()
            {
                dgvEmployees.Columns.Clear();

                dgvEmployees.Columns.AddRange(
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Employee_ID",
                        HeaderText = "ID",
                        Name = "Employee_ID",
                        Visible = false
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "firstName",
                        HeaderText = "FIRST NAME",
                        Name = "firstName",
                        Width = 150
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "lastName",
                        HeaderText = "LAST NAME",
                        Name = "lastName",
                        Width = 150
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "nic",
                        HeaderText = "NIC",
                        Width = 150
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "RoleName",
                        HeaderText = "ROLE",
                        Width = 150
                    },
                    new DataGridViewImageColumn
                    {
                        DataPropertyName = "picture",
                        HeaderText = "PHOTO",
                        ImageLayout = DataGridViewImageCellLayout.Zoom,
                        Width = 80,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Padding = new Padding(5),
                            Alignment = DataGridViewContentAlignment.MiddleCenter
                        }
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "status",
                        HeaderText = "STATUS",
                        Name = "status",
                        Width = 120,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleCenter
                        }
                    }
                );
            }

            private void LoadEmployees() => dgvEmployees.DataSource = _service.GetAllEmployees();

            private void ShowEmployeeForm(Employee employee = null)
            {
                using var form = new EmployeeForm(employee);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadEmployees();
            }

            private void EditEmployee()
            {
                if (dgvEmployees.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select an employee to edit");
                    return;
                }

                var row = (dgvEmployees.SelectedRows[0].DataBoundItem as DataRowView).Row;
                var employee = new Employee
                {
                    Employee_ID = (int)row["Employee_ID"],
                    firstName = row["firstName"].ToString(),
                    lastName = row["lastName"].ToString(),
                    nic = row["nic"].ToString(),
                    userName = row["userName"].ToString(),
                    password = row["password"].ToString(), //Hashing Password is there
                    contactNo = row["contactNo"].ToString(),
                    status = row["status"].ToString(),
                    Role_ID = (int)row["Role_ID"],
                    address = row["address"].ToString(),
                    picture = row["picture"] as byte[]
                };

                ShowEmployeeForm(employee);
            }

            private void ToggleStatus()
            {
                if (dgvEmployees.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select an employee");
                    return;
                }

                var row = dgvEmployees.SelectedRows[0];
                int id = (int)row.Cells["Employee_ID"].Value;

                if (id == 1)
                {
                    ShowMessage("Cannot change status of the Owner");
                    return;
                }

                string name = $"{row.Cells["firstName"].Value} {row.Cells["lastName"].Value}";
                string currentStatus = row.Cells["status"].Value?.ToString() ?? "";
                string newStatus = currentStatus == "Active" ? "Inactive" : "Active";

                if (ConfirmAction($"Change status of {name} to {newStatus}?"))
                {
                    _service.ToggleStatus(id);
                    LoadEmployees();
                }
            }

            private void ShowMessage(string text)
            {
                MessageBox.Show(text, "Employee Management",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            private bool ConfirmAction(string message)
            {
                return MessageBox.Show(message, "Confirm Action",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }
        }
        public class BrandAndCategory : Form
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

            public BrandAndCategory()
            {
                InitializeComponent();
                LoadBrands();
                LoadCategories();
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
                        new Font("Segoe UI", 12, isSelected ? FontStyle.Bold : FontStyle.Regular),
                        tabRect,
                        isSelected ? Color.White : Color.Black,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    );
                };

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
                    ReadOnly = true,
                    AutoGenerateColumns = false,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    BackgroundColor = BackgroundColor,
                    RowHeadersVisible = false,
                    BorderStyle = BorderStyle.None,
                    EnableHeadersVisualStyles = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                };

                // Grid styling
                dgvBrands.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };

                dgvBrands.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

                dgvBrands.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor,
                    Padding = new Padding(10, 5, 10, 5)
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
                        HeaderText = "ID",
                        Visible = false
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "brandName",
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
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 5, 10, 5)
                };

                dgvCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

                dgvCategories.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor,
                    Padding = new Padding(10, 5, 10, 5),
                    
                };


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
                        HeaderText = "ID",
                        Visible = false
                    },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "categoryName",
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
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Margin = new Padding(10, 0, 10, 0),
                    Cursor = Cursors.Hand,
                    TextImageRelation = TextImageRelation.ImageBeforeText
                };
            }

            private void LoadBrands() => dgvBrands.DataSource = _brandService.GetAllBrands();

            private void LoadCategories() => dgvCategories.DataSource = _categoryService.GetAllCategories();

            private void ShowBrandForm(Brand brand = null)
            {
                using var form = new BrandForm(brand);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadBrands();
            }

            private void EditBrand()
            {
                if (dgvBrands.SelectedRows.Count == 0) return;

                var row = (dgvBrands.SelectedRows[0].DataBoundItem as DataRowView)?.Row;
                if (row == null) return;

                var brand = new Brand
                {
                    Brand_ID = (int)row["Brand_ID"],
                    brandName = row["brandName"].ToString()
                };

                ShowBrandForm(brand);
            }

            private void DeleteBrand()
            {
                if (dgvBrands.SelectedRows.Count == 0)
                {
                    ShowMessage("Please select a brand to delete");
                    return;
                }

                var row = dgvBrands.SelectedRows[0];
                int id = (int)row.Cells["Brand_ID"].Value;
                string name = row.Cells["brandName"].Value?.ToString() ?? "";

                if (ConfirmAction($"Delete brand '{name}'?"))
                {
                    if (_brandService.DeleteBrand(id))
                        LoadBrands();
                    else
                        ShowMessage("Error deleting brand");
                }
            }

            private void ShowCategoryForm(Category category = null)
            {
                using var form = new CategoryForm(category);
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
                string name = row.Cells["categoryName"].Value?.ToString() ?? "";

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
        public class BrandForm : Form
        {
            private readonly Brand _brand;
            private readonly BrandService _service = new BrandService();
            private TextBox txtName;

            public BrandForm(Brand brand = null)
            {
                _brand = brand ?? new Brand();
                InitializeComponent();
            }

            private void InitializeComponent()
            {
                this.Size = new Size(400, 220);
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.StartPosition = FormStartPosition.CenterParent;
                this.Text = _brand.Brand_ID > 0 ? "Edit Brand" : "Add Brand";
                this.BackColor = Color.White;
                this.Font = new Font("Segoe UI", 10);
                this.MaximizeBox = false;    // Disable maximize button
                this.MinimizeBox = false;    // Disable minimize button
                this.ControlBox = false;     // Ensure close button is visible

                // Container panel
                var container = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(60, 60, 60),
                    Padding = new Padding(20)
                };

                // Brand Name
                var lblName = new Label
                {
                    Text = "Brand Name:",
                    Location = new Point(20, 30),
                    AutoSize = true,
                    ForeColor = OwnerDashboard.ForegroundColor
                };

                txtName = new TextBox
                {
                    Location = new Point(150, 30),
                    Size = new Size(200, 30),
                    Text = _brand.brandName,
                    BackColor = Color.FromArgb(70, 70, 70),
                    ForeColor = OwnerDashboard.ForegroundColor,
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Buttons panel
                var buttonPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 60,
                    BackColor = Color.Transparent
                };

                var btnSave = new Button
                {
                    Text = "Save",
                    Size = new Size(100, 40),
                    BackColor = Color.SeaGreen,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    DialogResult = DialogResult.OK,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    Size = new Size(100, 40),
                    BackColor = Color.IndianRed,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    DialogResult = DialogResult.Cancel,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };

                btnSave.Location = new Point(container.Width - btnSave.Width - btnCancel.Width - 20, 10);
                btnCancel.Location = new Point(container.Width - btnCancel.Width - 10, 10);

                btnSave.Click += (s, e) =>
                {
                    if (ValidateAndSave())
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                };

                btnCancel.Click += (s, e) => this.Close();

                // Add controls
                buttonPanel.Controls.Add(btnSave);
                buttonPanel.Controls.Add(btnCancel);
                container.Controls.Add(lblName);
                container.Controls.Add(txtName);
                container.Controls.Add(buttonPanel);
                this.Controls.Add(container);
            }

            private bool ValidateAndSave()
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    OwnerDashboard.ShowThemedMessage("Brand name cannot be empty");
                    return false;
                }

                if (_service.CheckBrandExists(txtName.Text, _brand.Brand_ID > 0 ? _brand.Brand_ID : (int?)null))
                {
                    OwnerDashboard.ShowThemedMessage("Brand name already exists");
                    return false;
                }

                _brand.brandName = txtName.Text.Trim();

                bool success = _brand.Brand_ID > 0
                    ? _service.UpdateBrand(_brand.Brand_ID, _brand.brandName)
                    : _service.AddBrand(_brand.brandName);

                if (!success)
                {
                    OwnerDashboard.ShowThemedMessage("Error saving brand");
                    return false;
                }
                return true;
            }
        }
        public partial class CategoryForm : Form
        {
            private readonly Category _category;
            private readonly CategoryService _categoryService = new CategoryService();
            private readonly SizeService _sizeService = new SizeService();
            private List<CategorySize> _allSizes;
            private CheckedListBox clbSizes;
            private TextBox txtName;

            public CategoryForm(Category category = null)
            {
                _category = category ?? new Category();
                InitializeComponent();
                this.Load += CategoryForm_Load;
            }

            private void CategoryForm_Load(object sender, EventArgs e)
            {
                LoadSizes();
            }

            private void LoadSizes()
            {
                try
                {
                    // Get all available sizes
                    _allSizes = _sizeService.GetAllSizes();
                    clbSizes.Items.Clear();

                    // Add sizes grouped by type
                    foreach (var group in _allSizes.GroupBy(s => s.SizeType))
                    {
                        clbSizes.Items.Add($"--- {group.Key} ---", false);
                        foreach (var size in group)
                        {
                            clbSizes.Items.Add($"{size.SizeLabel}", false);
                        }
                    }

                    // Check sizes already assigned to category
                    if (_category.Category_ID > 0)
                    {
                        var assignedSizes = _categoryService.GetSizesByCategoryId(_category.Category_ID);
                        var assignedSizeLabels = assignedSizes.Select(s => s.SizeLabel).ToList();

                        for (int i = 0; i < clbSizes.Items.Count; i++)
                        {
                            var item = clbSizes.Items[i].ToString();
                            if (assignedSizeLabels.Contains(item))
                            {
                                clbSizes.SetItemChecked(i, true);
                            }
                            else if (item.StartsWith("---")) // Header items
                            {
                                clbSizes.SetItemChecked(i, false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowThemedMessage($"Error loading sizes: {ex.Message}");
                }
            }

            private void InitializeComponent()
            {
                this.Size = new Size(500, 450);
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.StartPosition = FormStartPosition.CenterParent;
                this.Text = _category.Category_ID > 0 ? "Edit Category" : "Add Category";
                this.BackColor = Color.White;
                this.Font = new Font("Segoe UI", 10);
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.ControlBox = false;

                // Container panel
                var container = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(60, 60, 60),
                    Padding = new Padding(20)
                };

                // Category Name
                var lblName = new Label
                {
                    Text = "Category Name:",
                    Location = new Point(20, 30),
                    AutoSize = true,
                    ForeColor = ForegroundColor
                };

                txtName = new TextBox
                {
                    Location = new Point(150, 30),
                    Size = new Size(300, 30),
                    Text = _category.categoryName,
                    BackColor = Color.FromArgb(70, 70, 70),
                    ForeColor = ForegroundColor,
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Sizes Label
                var lblSizes = new Label
                {
                    Text = "Applicable Sizes:",
                    Location = new Point(20, 70),
                    AutoSize = true,
                    ForeColor = ForegroundColor
                };

                // Sizes CheckedListBox
                clbSizes = new CheckedListBox
                {
                    Location = new Point(150, 70),
                    Size = new Size(300, 250),
                    BackColor = Color.FromArgb(70, 70, 70),
                    ForeColor = ForegroundColor,
                    BorderStyle = BorderStyle.FixedSingle,
                    CheckOnClick = true,
                    IntegralHeight = false
                };

                // Buttons panel
                var buttonPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 60,
                    BackColor = Color.Transparent
                };

                var btnSave = new Button
                {
                    Text = "Save",
                    Size = new Size(100, 40),
                    BackColor = Color.SeaGreen,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    Size = new Size(100, 40),
                    BackColor = Color.IndianRed,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };

                btnSave.Location = new Point(container.Width - btnSave.Width - btnCancel.Width - 20, 10);
                btnCancel.Location = new Point(container.Width - btnCancel.Width - 10, 10);

                btnSave.Click += (s, e) => SaveCategory();
                btnCancel.Click += (s, e) => this.Close();

                // Add controls
                buttonPanel.Controls.Add(btnSave);
                buttonPanel.Controls.Add(btnCancel);
                container.Controls.Add(lblName);
                container.Controls.Add(txtName);
                container.Controls.Add(lblSizes);
                container.Controls.Add(clbSizes);
                container.Controls.Add(buttonPanel);
                this.Controls.Add(container);
            }

            private void SaveCategory()
            {
                if (ValidateInput())
                {
                    try
                    {
                        bool success;
                        int categoryId = _category.Category_ID;

                        if (categoryId > 0)
                        {
                            success = _categoryService.UpdateCategory(categoryId, txtName.Text);
                        }
                        else
                        {
                            success = _categoryService.AddCategory(txtName.Text);
                            categoryId = _categoryService.GetCategoryIdByName(txtName.Text);
                        }

                        if (success && categoryId > 0)
                        {
                            UpdateCategorySizes(categoryId);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            ShowThemedMessage("Error saving category");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowThemedMessage($"Error: {ex.Message}");
                    }
                }
            }

            private bool ValidateInput()
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    ShowThemedMessage("Category name cannot be empty");
                    return false;
                }

                if (_categoryService.CheckCategoryExists(txtName.Text,
                    _category.Category_ID > 0 ? _category.Category_ID : (int?)null))
                {
                    ShowThemedMessage("Category name already exists");
                    return false;
                }

                return true;
            }

            private void UpdateCategorySizes(int categoryId)
            {
                var selectedSizeIds = new List<int>();

                // Get selected size IDs
                for (int i = 0; i < clbSizes.Items.Count; i++)
                {
                    if (clbSizes.GetItemChecked(i) && !clbSizes.Items[i].ToString().StartsWith("---"))
                    {
                        var sizeLabel = clbSizes.Items[i].ToString();
                        var size = _allSizes.FirstOrDefault(s => s.SizeLabel == sizeLabel);
                        if (size != null)
                        {
                            selectedSizeIds.Add(size.Size_ID);
                        }
                    }
                }

                // Update sizes in database
                if (!_categoryService.UpdateCategorySizes(categoryId, selectedSizeIds))
                {
                    ShowThemedMessage("Error updating sizes for category");
                }
            }
        }
        public class BarcodePrint : Form
        {
            // Theme colors based on EmployeesManagement
            private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
            private static readonly Color BackgroundColor = Color.White;
            private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
            private static readonly Color ForegroundColor = Color.Black;
            private static readonly Color SecondaryColor = Color.Gray;
            private static readonly Color DeleteColor = Color.FromArgb(231, 76, 60);
            private static readonly Color WarningColor = Color.FromArgb(241, 196, 15);
            private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

            private DataGridView dgvItems;
            private PictureBox picBarcodePreview;
            private Button btnPreview;
            private Button btnPrint;
            private ComboBox cboPrinters;
            private Label lblPrinter;
            private List<Item> items = new List<Item>();
            private Item selectedItem;
            private NumericUpDown nudPrintCount;
            private const int LABEL_WIDTH_MM = 30;
            private const int LABEL_HEIGHT_MM = 20;

            public BarcodePrint()
            {
                InitializeForm();
                LoadPrinters();
                LoadItems(); // Auto-load items on initialization
            }

            private void InitializeForm()
            {
                // Form setup
                this.Text = "BARCODE LABEL PRINTING";
                this.Size = new Size(1100, 700);
                this.FormBorderStyle = FormBorderStyle.None;
                this.Dock = DockStyle.Fill;
                this.BackColor = BackgroundColor;
                this.Padding = new Padding(20);

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
                    Text = "BARCODE LABEL PRINTING",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.White,
                    Padding = new Padding(20, 0, 0, 0)
                };

                // Main content panel
                var contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor
                };

                // Left panel - DataGridView
                var leftPanel = new Panel
                {
                    Dock = DockStyle.Left,
                    Width = 500,
                    Padding = new Padding(0, 0, 20, 0)
                };

                dgvItems = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = BackgroundColor,
                    BorderStyle = BorderStyle.Fixed3D,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    RowHeadersVisible = false,
                    AllowUserToAddRows = false,
                    ReadOnly = true,
                    EnableHeadersVisualStyles = false
                };

                // Grid styling
                dgvItems.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = PrimaryColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(0)
                };

                dgvItems.DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BackgroundColor,
                    ForeColor = ForegroundColor,
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = SelectionColor,
                    SelectionForeColor = ForegroundColor
                };

                dgvItems.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                // Right panel - Preview and controls
                var rightPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(20, 0, 0, 0)
                };

                // Preview section
                var previewPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 350,
                    BackColor = BackgroundColor
                };

                var previewLabel = new Label
                {
                    Text = "BARCODE PREVIEW",
                    Dock = DockStyle.Top,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = PrimaryColor,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                picBarcodePreview = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Margin = new Padding(0, 5, 0, 0)
                };

                // Controls section
                var controlsPanel = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 3,
                    ColumnCount = 2,
                    AutoSize = true,
                    Padding = new Padding(0, 20, 0, 0)
                };
                controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
                controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
                controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

                // Printer selection
                lblPrinter = new Label
                {
                    Text = "Printer:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                cboPrinters = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10)
                };

                // Print count
                var lblPrintCount = new Label
                {
                    Text = "Print Count:",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                nudPrintCount = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = 1000,
                    Value = 1,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10)
                };

                // Buttons
                btnPreview = CreateButton("PREVIEW", PrimaryColor, 150, 40);
                btnPrint = CreateButton("PRINT LABELS", PrimaryColor, 150, 40);
                btnPreview.Enabled = false;
                btnPrint.Enabled = false;

                // Layout controls
                controlsPanel.Controls.Add(lblPrinter, 0, 0);
                controlsPanel.Controls.Add(cboPrinters, 1, 0);
                controlsPanel.Controls.Add(lblPrintCount, 0, 1);
                controlsPanel.Controls.Add(nudPrintCount, 1, 1);
                controlsPanel.Controls.Add(btnPreview, 0, 2);
                controlsPanel.Controls.Add(btnPrint, 1, 2);

                // Events
                dgvItems.SelectionChanged += DgvItems_SelectionChanged;
                btnPreview.Click += BtnPreview_Click;
                btnPrint.Click += BtnPrint_Click;

                // Build panels
                previewPanel.Controls.Add(picBarcodePreview);
                previewPanel.Controls.Add(previewLabel);

                rightPanel.Controls.Add(controlsPanel);
                rightPanel.Controls.Add(previewPanel);

                leftPanel.Controls.Add(dgvItems);

                contentPanel.Controls.Add(rightPanel);
                contentPanel.Controls.Add(leftPanel);

                titlePanel.Controls.Add(lblTitle);

                container.Controls.Add(contentPanel);
                container.Controls.Add(titlePanel);

                this.Controls.Add(container);
                ConfigureGridColumns();
            }

            private Button CreateButton(string text, Color backColor, int width, int height)
            {
                return new Button
                {
                    Text = text,
                    Size = new Size(width, height),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    BackColor = backColor,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Margin = new Padding(5),
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
            }

            private void ConfigureGridColumns()
            {
                dgvItems.Columns.Clear();

                dgvItems.Columns.AddRange(
                    new DataGridViewTextBoxColumn { DataPropertyName = "Item_ID", HeaderText = "ID", Width = 40, Name = "Item_ID" },
                    new DataGridViewTextBoxColumn { DataPropertyName = "Barcode", HeaderText = "BARCODE", Width = 120 },
                    new DataGridViewTextBoxColumn { DataPropertyName = "Description", HeaderText = "DESCRIPTION", Width = 200 },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "RetailPrice",
                        HeaderText = "PRICE",
                        Width = 80,
                        DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                    },
                    new DataGridViewTextBoxColumn { DataPropertyName = "BrandName", HeaderText = "BRAND", Width = 100 },
                    new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "CATEGORY", Width = 100 },
                    new DataGridViewTextBoxColumn { DataPropertyName = "SizeLabel", HeaderText = "SIZE", Width = 70 },
                    new DataGridViewTextBoxColumn
                    {
                        DataPropertyName = "Quantity",
                        HeaderText = "QTY",
                        Width = 60,
                        DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
                    }
                );
            }

            private void LoadPrinters()
            {
                try
                {
                    cboPrinters.DataSource = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList();
                    if (cboPrinters.Items.Count > 0)
                        cboPrinters.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading printers: {ex.Message}", "Printer Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void LoadItems()
            {
                try
                {
                    items = GetItemsFromDatabase();

                    dgvItems.Rows.Clear();
                    foreach (var item in items)
                    {
                        dgvItems.Rows.Add(
                            item.Item_ID,
                            item.barcode,
                            item.description,
                            item.RetailPrice,
                            item.BrandName,
                            item.CategoryName,
                            item.SizeLabel ?? "N/A",
                            item.quantity
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading items: {ex.Message}", "Data Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private List<Item> GetItemsFromDatabase()
            {
                try
                {
                    ItemRepository repository = new ItemRepository();
                    return repository.SearchItems("", 0, 0);  // Empty search returns all items
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}", "Database Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Item>();
                }
            }

            private void DgvItems_SelectionChanged(object sender, EventArgs e)
            {
                if (dgvItems.SelectedRows.Count > 0)
                {
                    int selectedId = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["Item_ID"].Value);
                    selectedItem = items.FirstOrDefault(i => i.Item_ID == selectedId);

                    btnPreview.Enabled = true;
                    btnPrint.Enabled = true;

                    // Set default print count to item quantity
                    nudPrintCount.Value = Math.Max(1, Math.Min(selectedItem.quantity, nudPrintCount.Maximum));
                }
                else
                {
                    btnPreview.Enabled = false;
                    btnPrint.Enabled = false;
                }
            }

            private void BtnPreview_Click(object sender, EventArgs e)
            {
                if (selectedItem == null) return;

                try
                {
                    Bitmap labelImage = GenerateBarcodeLabel(selectedItem);
                    picBarcodePreview.Image = labelImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Preview error: {ex.Message}", "Preview Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnPrint_Click(object sender, EventArgs e)
            {
                if (selectedItem == null || cboPrinters.SelectedItem == null) return;

                try
                {
                    int printCount = (int)nudPrintCount.Value;
                    Bitmap labelImage = GenerateBarcodeLabel(selectedItem);

                    for (int i = 0; i < printCount; i++)
                    {
                        PrintLabel(labelImage);
                    }

                    MessageBox.Show($"{printCount} label(s) sent to printer successfully!", "Print Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Print error: {ex.Message}", "Print Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private Bitmap GenerateBarcodeLabel(Item item)
            {
                // Convert mm to pixels at 300 DPI
                int widthPixels = (int)(LABEL_WIDTH_MM * 300 / 25.4);  // ~354 pixels
                int heightPixels = (int)(LABEL_HEIGHT_MM * 300 / 25.4); // ~236 pixels

                Bitmap bmp = new Bitmap(widthPixels, heightPixels);
                bmp.SetResolution(300, 300);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    // Define fonts
                    Font titleFont = new Font("Arial", 9, FontStyle.Bold);
                    Font verticalFont = new Font("Arial", 6, FontStyle.Bold);
                    Font barcodeTextFont = new Font("Arial", 6, FontStyle.Bold);
                    Font bottomFont = new Font("Arial", 6, FontStyle.Bold);

                    StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                    float margin = 25f; // A small margin

                    // 1. Top Section: Style NewAge
                    RectangleF titleRect = new RectangleF(0, 5, widthPixels, 40);
                    g.DrawString("Style NewAge", titleFont, Brushes.Black, titleRect, centerFormat);

                    // 2. Bottom Section: Brand + Category (Size)
                    string sizePart = !string.IsNullOrEmpty(item.SizeLabel) ? $" ({item.SizeLabel})" : "";
                    string bottomText = $"{item.BrandName}  {item.CategoryName}{sizePart}";
                    RectangleF bottomRect = new RectangleF(0, heightPixels - 35, widthPixels, 30);
                    g.DrawString(bottomText, bottomFont, Brushes.Black, bottomRect, centerFormat);

                    // 3. Retail Price (Left side, rotated)
                    g.TranslateTransform(margin + 15, heightPixels / 2f);
                    g.RotateTransform(-90);
                    g.DrawString($"Rs.{item.RetailPrice}", verticalFont, Brushes.Black, new RectangleF(-heightPixels / 2f, -15, heightPixels, 30), centerFormat);
                    g.ResetTransform();

                    // 4. Unit Cost (Right side, rotated)
                    g.TranslateTransform(widthPixels - margin - 20, heightPixels / 2f);
                    g.RotateTransform(90);
                    g.DrawString($"Rs.{item.unitCost}", verticalFont, Brushes.Black, new RectangleF(-heightPixels / 2f, -15, heightPixels, 30), centerFormat);
                    g.ResetTransform();

                    // 5. Central Rectangle for Barcode
                    float centralRectX = 50;
                    float centralRectY = 50;
                    float centralRectWidth = widthPixels - 100;
                    float centralRectHeight = heightPixels - 85;

                    // 6. Barcode Image
                    using (var barcode = new BarcodeProfessional())
                    {
                        barcode.Symbology = Symbology.Code128;
                        barcode.Code = item.barcode;
                        barcode.BarHeight = 0.35f;
                        barcode.BarWidth = 0.005f;
                        barcode.DisplayCode = false;

                        Image barcodeImg = barcode.GetBarcodeImage(centralRectWidth + 50, centralRectHeight - 30);

                        float barcodeX = centralRectX + (centralRectWidth - barcodeImg.Width) / 2;
                        float barcodeY = centralRectY + 10;
                        g.DrawImage(barcodeImg, barcodeX, barcodeY);
                    }

                    // 7. Barcode Code Text
                    RectangleF barcodeTextRect = new RectangleF(
                        centralRectX,
                        centralRectY + centralRectHeight - 35,
                        centralRectWidth,
                        25
                    );
                    g.DrawString(item.barcode, barcodeTextFont, Brushes.Black, barcodeTextRect, centerFormat);
                }

                return bmp;
            }

            private void PrintLabel(Bitmap labelImage)
            {
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = cboPrinters.SelectedItem.ToString();

                // Configure label size
                PaperSize customSize = new PaperSize("Custom",
                    (int)(LABEL_WIDTH_MM * 100 / 25.4),
                    (int)(LABEL_HEIGHT_MM * 100 / 25.4));

                pd.DefaultPageSettings.PaperSize = customSize;
                pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                pd.PrintPage += (sender, e) =>
                {
                    e.Graphics.DrawImage(
                        labelImage,
                        e.PageBounds,
                        new Rectangle(0, 0, labelImage.Width, labelImage.Height),
                        GraphicsUnit.Pixel
                    );
                    e.HasMorePages = false;
                };

                pd.Print();
            }
        }
        public class ReportsView : Form { }
    }
}
