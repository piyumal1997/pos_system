using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Services;
using pos_system.pos.BLL.Utilities;
using pos_system.pos.Core;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace pos_system.pos.UI.Forms.Controls
{
    public partial class EmployeeForm : Form
    {
        private readonly EmployeeService _service = new EmployeeService();
        private readonly RoleRepository _roleRepo = new RoleRepository();
        private readonly EmployeeRepository _repo = new EmployeeRepository();
        private readonly bool _isEdit;
        private readonly Employee _employee;

        private bool _isDirty = false;
        private bool _validationFailed = false;
        private List<string> _lastValidationErrors;

        private TextBox txtFirstName, txtLastName, txtNIC, txtUsername,
                        txtPassword, txtAddress, txtContact, txtEmail;
        private ComboBox cmbRole, cmbStatus;
        private PictureBox pictureBox;
        private Button btnSave, btnCancel, btnUpload;
        private Panel topPanel, mainPanel, container;
        private Label lblTitle;
        private Button btnClose, btnMinimize;

        public EmployeeForm(Employee employee = null)
        {
            _isEdit = employee != null;
            _employee = employee ?? new Employee();
            InitializeComponent();
            LoadRoles();
            LoadEmployeeData();
            SetupChangeTracking();
            new DropShadow().ApplyShadows(this);
        }

        private void InitializeComponent()
        {
            
            // Form Properties
            this.Size = new Size(480, 680);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(41, 128, 185); // Primary blue
            
            // Main Container Panel
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(1),
                BackColor = Color.White
            };

            // Title Bar
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(41, 128, 185),
                Cursor = Cursors.SizeAll
            };

            lblTitle = new Label
            {
                Text = _isEdit ? "Edit Employee" : "Add New Employee",
                Dock = DockStyle.Left,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Padding = new Padding(15, 10, 0, 0),
                AutoSize = true
            };

            // Window control buttons
            btnClose = new Button
            {
                Text = "✕",
                Dock = DockStyle.Right,
                Size = new Size(40, 40),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.Transparent,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };

            topPanel.Controls.Add(btnClose);
            topPanel.Controls.Add(btnMinimize);
            topPanel.Controls.Add(lblTitle);

            // Content Container
            container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(35, 15, 35, 10),
                AutoScroll = true,
                BackColor = Color.White
            };

            // Create Form Controls
            CreateFormControls();

            // Form Layout Table
            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Padding = new Padding(10,0,10,0),
                ColumnStyles =
                {
                    new ColumnStyle(SizeType.Percent, 30F),
                    new ColumnStyle(SizeType.Percent, 70F)
                }
            };

            // Add form rows
            AddFormRow(mainTable, "First Name:", txtFirstName);
            AddFormRow(mainTable, "Last Name:", txtLastName);
            AddFormRow(mainTable, "NIC:", txtNIC);
            AddFormRow(mainTable, "Username:", txtUsername);
            AddFormRow(mainTable, "Password:", txtPassword);
            AddFormRow(mainTable, "Address:", txtAddress);
            AddFormRow(mainTable, "Contact:", txtContact);
            AddFormRow(mainTable, "Email:", txtEmail);
            AddFormRow(mainTable, "Role:", cmbRole);
            AddFormRow(mainTable, "Status:", cmbStatus);

            // Image section
            var imgLabel = new Label
            {
                Text = "Photo:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 15, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var imgContainer = new Panel
            {
                AutoSize = true,
                Height = 150
            };

            imgContainer.Controls.Add(pictureBox);
            btnUpload.Location = new Point(160, 5);
            imgContainer.Controls.Add(btnUpload);

            mainTable.Controls.Add(imgLabel, 0, mainTable.RowCount);
            mainTable.Controls.Add(imgContainer, 1, mainTable.RowCount);
            mainTable.RowCount++;

            // Button Panel
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(0, 10, 0, 0)
            };

            btnSave = CreateStyledButton(_isEdit ? "Update" : "Save", Color.FromArgb(41, 128, 185));
            btnCancel = CreateStyledButton("Cancel", Color.FromArgb(120, 120, 120));

            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnSave);
            btnPanel.Controls.Add(new Panel { Width = 20, Height = 1 }); // Spacer

            // Event Handlers
            btnClose.Click += (s, e) => this.Close();
            //btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            topPanel.MouseDown += TopPanel_MouseDown;
            lblTitle.MouseDown += TopPanel_MouseDown;
            btnUpload.Click += BtnUpload_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            this.FormClosing += EmployeeForm_FormClosing;

            // Assemble Form
            container.Controls.Add(mainTable);
            container.Controls.Add(btnPanel);
            mainPanel.Controls.Add(container);
            mainPanel.Controls.Add(topPanel);
            this.Controls.Add(mainPanel);
        }

        private void CreateFormControls()
        {
            // Common textbox styling
            Func<TextBox> createTextBox = () => new TextBox
            {
                Width = 300,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 10),
                Font = new Font("Segoe UI", 9.5F)
            };

            txtFirstName = createTextBox();
            txtFirstName.MaxLength = 20;

            txtLastName = createTextBox();
            txtLastName.MaxLength = 20;

            txtNIC = createTextBox();
            txtNIC.MaxLength = 13;
            txtNIC.ReadOnly = _isEdit;

            txtUsername = createTextBox();
            txtUsername.MaxLength = 12;

            txtPassword = createTextBox();
            txtPassword.PasswordChar = '*';
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Enabled = !_isEdit;

            txtAddress = createTextBox();
            txtAddress.MaxLength = 120;

            txtContact = createTextBox();
            txtContact.MaxLength = 10;

            txtEmail = createTextBox();
            txtEmail.MaxLength = 50;

            // ComboBox styling
            Func<ComboBox> createComboBox = () => new ComboBox
            {
                Width = 300,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F)
            };

            cmbRole = createComboBox();
            cmbStatus = createComboBox();
            cmbStatus.Items.AddRange(new object[] { "Active", "Inactive" });

            // Image controls
            pictureBox = new PictureBox
            {
                Size = new Size(140, 140),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                Margin = new Padding(0, 5, 10, 10)
            };

            btnUpload = CreateStyledButton("Upload Image", Color.FromArgb(41, 128, 185));
            btnUpload.AutoSize = true;
            btnUpload.Size = new Size(110, 35);
        }

        private Button CreateStyledButton(string text, Color bgColor)
        {
            var btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = bgColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                MinimumSize = new Size(100, 35),
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };

            // Hover effects
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(bgColor, 0.15f);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(bgColor, 0.3f);

            return btn;
        }

        private void AddFormRow(TableLayoutPanel panel, string label, Control control)
        {
            var rowIndex = panel.RowCount++;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            panel.Controls.Add(new Label
            {
                Text = label,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 10, 0, 0),
                Font = new Font("Segoe UI", 9.5F)
            }, 0, rowIndex);

            panel.Controls.Add(control, 1, rowIndex);
        }

        private void SetupChangeTracking()
        {
            // Track changes in all input fields
            txtFirstName.TextChanged += (s, e) => _isDirty = true;
            txtLastName.TextChanged += (s, e) => _isDirty = true;
            txtUsername.TextChanged += (s, e) => _isDirty = true;
            txtPassword.TextChanged += (s, e) => _isDirty = true;
            txtAddress.TextChanged += (s, e) => _isDirty = true;
            txtContact.TextChanged += (s, e) => _isDirty = true;
            txtEmail.TextChanged += (s, e) => _isDirty = true;
            cmbRole.SelectedIndexChanged += (s, e) => _isDirty = true;
            cmbStatus.SelectedIndexChanged += (s, e) => _isDirty = true;
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                using var dialog = new OpenFileDialog
                {
                    Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp",
                    Title = "Select Employee Photo"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (var image = Image.FromFile(dialog.FileName))
                    {
                        pictureBox.Image = (Image)image.Clone();
                        _isDirty = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Image Upload Error", $"Failed to load image: {ex.Message}");
            }
        }

        private void LoadRoles()
        {
            try
            {
                cmbRole.DataSource = _roleRepo.GetAllRoles();
                cmbRole.DisplayMember = "RoleName";
                cmbRole.ValueMember = "Role_ID";

                if (!_isEdit && cmbRole.Items.Count > 0)
                    cmbRole.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowError("Data Loading Error", $"Failed to load roles: {ex.Message}");
            }
        }

        private void LoadEmployeeData()
        {
            if (!_isEdit)
            {
                cmbStatus.SelectedIndex = 0;
                return;
            }

            try
            {
                txtFirstName.Text = _employee.firstName;
                txtLastName.Text = _employee.lastName;
                txtNIC.Text = _employee.nic;
                txtUsername.Text = _employee.userName;
                txtAddress.Text = _employee.address;
                txtContact.Text = _employee.contactNo;
                txtEmail.Text = _employee.email;
                cmbStatus.SelectedItem = _employee.status;
                cmbRole.SelectedValue = _employee.Role_ID;

                if (_employee.picture != null && _employee.picture.Length > 0)
                {
                    using (var ms = new MemoryStream(_employee.picture))
                    {
                        pictureBox.Image = Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Data Loading Error", $"Failed to load employee data: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateEmployee(out var errors))
            {
                _validationFailed = true;
                _lastValidationErrors = errors;
                ShowValidationErrors(errors);
                return;
            }

            try
            {
                MapFormToModel();
                var result = _service.SaveEmployee(_employee, _isEdit);

                if (result.success)
                {
                    _isDirty = false;
                    _validationFailed = false;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Save Failed", result.message);
                }
            }
            catch (SqlException sqlEx)
            {
                HandleDatabaseError(sqlEx);
            }
            catch (Exception ex)
            {
                ShowError("Unexpected Error", ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EmployeeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent closing if validation failed
            if (_validationFailed)
            {
                e.Cancel = true;
                ShowValidationErrors(_lastValidationErrors);
                return;
            }

            // Confirm close if there are unsaved changes
            if (_isDirty && this.DialogResult != DialogResult.OK)
            {
                var result = MessageBox.Show(this,
                    "You have unsaved changes. Close without saving?",
                    "Confirm Close",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                e.Cancel = (result == DialogResult.No);
            }

            // Clean up image resources
            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = null;
            }
        }

        private bool ValidateEmployee(out List<string> errors)
        {
            errors = new List<string>();
            var firstName = txtFirstName.Text.Trim();
            var lastName = txtLastName.Text.Trim();
            var nic = txtNIC.Text.Trim();
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text.Trim();
            var address = txtAddress.Text.Trim();
            var contact = txtContact.Text.Trim();
            var email = txtEmail.Text.Trim();

            // Required fields validation
            if (string.IsNullOrWhiteSpace(firstName))
                errors.Add("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                errors.Add("Last name is required");

            if (!_isEdit && string.IsNullOrWhiteSpace(password))
                errors.Add("Password is required");

            if (cmbRole.SelectedValue == null)
                errors.Add("Role is required");

            if (cmbStatus.SelectedItem == null)
                errors.Add("Status is required");

            // Field length validation
            if (firstName.Length > 20)
                errors.Add("First name cannot exceed 20 characters");

            if (lastName.Length > 20)
                errors.Add("Last name cannot exceed 20 characters");

            if (username.Length > 12)
                errors.Add("Username cannot exceed 12 characters");

            if (address.Length > 120)
                errors.Add("Address cannot exceed 120 characters");

            if (email.Length > 50)
                errors.Add("Email cannot exceed 50 characters");

            // NIC validation
            if (!Regex.IsMatch(nic, @"^([0-9]{9}[VvXx]|[0-9]{12})$"))
                errors.Add("Invalid NIC format (e.g., 123456789V or 123456789012)");

            // Contact validation
            if (!string.IsNullOrEmpty(contact) &&
                !Regex.IsMatch(contact, @"^0[0-9]{9}$"))
                errors.Add("Contact must be 10 digits starting with 0");

            // Email validation
            if (!string.IsNullOrEmpty(email) &&
                !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Invalid email format (e.g., user@example.com)");

            // Password policy
            if (!_isEdit && password.Length < 6)
                errors.Add("Password must be at least 6 characters");

            if (!_isEdit && password.Contains(username))
                errors.Add("Password cannot contain username");

            // Check for existing records
            if (!_isEdit)
            {
                try
                {
                    if (_repo.CheckExisting(nic, username, contact, email))
                        errors.Add("NIC, Username, Contact or Email already exists");
                }
                catch (Exception ex)
                {
                    errors.Add($"Validation error: {ex.Message}");
                }
            }

            return errors.Count == 0;
        }

        private void MapFormToModel()
        {
            _employee.firstName = txtFirstName.Text.Trim();
            _employee.lastName = txtLastName.Text.Trim();
            _employee.userName = txtUsername.Text.Trim();
            _employee.address = txtAddress.Text.Trim();
            _employee.contactNo = txtContact.Text.Trim();
            _employee.email = txtEmail.Text.Trim();
            _employee.status = cmbStatus.SelectedItem?.ToString();
            _employee.Role_ID = (int)cmbRole.SelectedValue;

            if (!_isEdit)
            {
                _employee.nic = txtNIC.Text.Trim();
                _employee.password = HashHelper.ComputeSqlCompatibleHash(txtPassword.Text);
            }

            if (pictureBox.Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    // Create a new Bitmap to avoid GDI+ errors
                    using (Bitmap bmp = new Bitmap(pictureBox.Image))
                    {
                        bmp.Save(ms, ImageFormat.Jpeg);
                    }
                    _employee.picture = ms.ToArray();
                }
            }
        }

        private void TopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, 0xA1, 0x2, 0);
            }
        }

        #region Error Handling Methods
        private void ShowValidationErrors(List<string> errors)
        {
            var message = "Please fix the following errors:\n\n" +
                          string.Join("\n• ", errors);

            MessageBox.Show(this, message, "Validation Errors",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(this, message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void HandleDatabaseError(SqlException ex)
        {
            string errorMessage = "Database error occurred:\n";

            for (int i = 0; i < ex.Errors.Count; i++)
            {
                errorMessage += $"\nError {i + 1}:\n" +
                               $"Message: {ex.Errors[i].Message}\n" +
                               $"Line: {ex.Errors[i].LineNumber}\n" +
                               $"Procedure: {ex.Errors[i].Procedure}\n";
            }

            if (ex.Number == 2627) // Unique constraint violation
            {
                errorMessage += "\nPossible duplicate entry. " +
                               "Please check NIC, Username, Contact or Email.";
            }
            else if (ex.Number == 547) // Constraint check violation
            {
                errorMessage += "\nInvalid reference. Please check related data.";
            }

            ShowError("Database Error", errorMessage);
        }
        #endregion

        #region Native Methods for Form Dragging
        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

            [DllImport("user32.dll")]
            public static extern bool ReleaseCapture();
        }
        #endregion
    }
}