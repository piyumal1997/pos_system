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

namespace pos_system.pos.UI.Forms.Controls
{
    public partial class EmployeesManagement : Form
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
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 11),
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
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
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
                    DataPropertyName = "contactno",
                    HeaderText = "CONTACT",
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
            using var form = new pos_system.pos.UI.Forms.Controls.EmployeeForm(employee);
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
                email = row["email"].ToString(),
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
            string currentStatus = row.Cells["status"].Value?.ToString() ?? string.Empty;
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
}
