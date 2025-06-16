using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pos_system.pos.Models;
using pos_system.pos.BLL.Services;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.BLL.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using pos_system.pos.UI.Forms;
using System.Diagnostics;


namespace pos_system.pos.UI.Forms
{
    public partial class EmployeeForm : Form
    {
        private readonly EmployeeService _service = new EmployeeService();
        private readonly RoleRepository _roleRepo = new RoleRepository();
        private readonly EmployeeRepository _repo = new EmployeeRepository();
        private readonly bool _isEdit;
        private readonly Employee _employee;

        private TextBox txtFirstName, txtLastName, txtNIC, txtUsername, txtPassword, txtAddress, txtContact;
        private ComboBox cmbRole, cmbStatus;
        private PictureBox pictureBox;
        private Button btnSave, btnCancel, btnUpload;

        public EmployeeForm(Employee employee = null)
        {
            _isEdit = employee != null;
            _employee = employee ?? new Employee();
            InitializeComponent();
            LoadRoles();
            LoadEmployeeData();
            this.Text = _isEdit ? "Edit Employee" : "Add New Employee"; // Dynamic title
        }

        private void InitializeComponent()
        {
            // Set form properties first
            this.Size = new Size(550, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Fixed size with close button
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;    // Disable maximize button
            this.MinimizeBox = false;    // Disable minimize button
            this.ControlBox = false;     // Ensure close button is visible
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);
            this.CancelButton = btnCancel; // Set cancel button for ESC key

            // Main Table Layout
            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 10,
                Padding = new Padding(40, 40, 25, 20),
                BackColor = Color.White
            };

            // Create Controls with Theme
            CreateFormControls();

            // Add Controls with Styling
            AddFormRow(mainTable, "First Name:", txtFirstName);
            AddFormRow(mainTable, "Last Name:", txtLastName);
            AddFormRow(mainTable, "NIC:", txtNIC);
            AddFormRow(mainTable, "Username:", txtUsername);
            AddFormRow(mainTable, "Password:", txtPassword);
            AddFormRow(mainTable, "Address:", txtAddress);
            AddFormRow(mainTable, "Contact:", txtContact);
            AddFormRow(mainTable, "Role:", cmbRole);
            AddFormRow(mainTable, "Status:", cmbStatus);
            AddFormRow(mainTable, "Photo:", pictureBox);
            AddFormRow(mainTable, "", btnUpload);

            // Style Upload Button
            btnUpload.FlatStyle = FlatStyle.Flat;
            btnUpload.BackColor = ColorTranslator.FromHtml("#4a90e2");
            btnUpload.ForeColor = Color.White;
            btnUpload.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnUpload.FlatAppearance.BorderSize = 0;

            // Buttons Panel
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#f8f9fa"),
                Padding = new Padding(0, 15, 20, 0)
            };

            // Style Save/Cancel Buttons
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.BackColor = ColorTranslator.FromHtml("#28a745");
            btnSave.ForeColor = Color.White;
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.Size = new Size(100, 35);
            btnSave.FlatAppearance.BorderSize = 0;

            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.BackColor = ColorTranslator.FromHtml("#6c757d");
            btnCancel.ForeColor = Color.White;
            btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancel.Size = new Size(100, 35);
            btnCancel.FlatAppearance.BorderSize = 0;

            btnPanel.Controls.AddRange(new[] { btnCancel, btnSave });

            // Events
            btnUpload.Click += BtnUpload_Click;
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(mainTable);
            this.Controls.Add(btnPanel);
        }


        private void CreateFormControls()
        {
            txtFirstName = new TextBox { Width = 300 };
            txtLastName = new TextBox { Width = 300 };
            txtNIC = new TextBox { ReadOnly = _isEdit, Width = 300 };
            txtUsername = new TextBox { Width = 300 };
            txtPassword = new TextBox { ReadOnly = _isEdit, Width = 300};
            txtAddress = new TextBox { Width = 300 };
            txtContact = new TextBox { Width = 300 };
            cmbRole = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
            cmbStatus = new ComboBox { Items = { "Active", "Inactive" }, Width = 300 };
            pictureBox = new PictureBox { Size = new Size(140, 140), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            btnUpload = new Button { Text = "Upload Image", AutoSize = true };
            btnSave = new Button { Text = _isEdit ? "Update" : "Save", DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel };
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp" // Expanded filter options
            };
            if (dialog.ShowDialog() == DialogResult.OK)
                pictureBox.Image = Image.FromFile(dialog.FileName);
        }

        private void AddFormRow(TableLayoutPanel panel, string label, Control control)
        {
            panel.Controls.Add(new Label { Text = label, AutoSize = true });
            panel.Controls.Add(control);
        }

        private void LoadRoles()
        {
            cmbRole.DataSource = _roleRepo.GetAllRoles();
            cmbRole.DisplayMember = "RoleName";
            cmbRole.ValueMember = "Role_ID";
        }

        private void LoadEmployeeData()
        {
            if (!_isEdit) return;

            txtFirstName.Text = _employee.firstName;
            txtLastName.Text = _employee.lastName;
            txtNIC.Text = _employee.nic;
            txtUsername.Text = _employee.userName;
            //txtPassword.Text = _employee.password;
            txtAddress.Text = _employee.address;
            txtContact.Text = _employee.contactNo;
            cmbStatus.SelectedItem = _employee.status;
            cmbRole.SelectedValue = _employee.Role_ID;

            if (_employee.picture != null)
                pictureBox.Image = Image.FromStream(new MemoryStream(_employee.picture));
        }

        //private void BtnUpload_Click(object sender, EventArgs e)
        //{
        //    using var dialog = new OpenFileDialog { Filter = "Images|*.jpg;*.png;*.bmp" };
        //    if (dialog.ShowDialog() == DialogResult.OK)
        //        pictureBox.Image = Image.FromFile(dialog.FileName);
        //}

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateEmployee(out var errors))
            {
                ShowThemedMessage("Validation Errors : " + "\n" + string.Join("\n", errors) );
                Debug.WriteLine($"Validation Errors : {string.Join(", ", errors)}");
                return;
            }

            try
            {
                MapFormToModel();
                var result = _service.SaveEmployee(_employee, _isEdit);
                
                if (result.success)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    ShowThemedMessage(result.message);
                }
            }
            catch (Exception ex)
            {
                ShowThemedMessage($"Error: {ex.Message}");
            }
        }

        public static void ShowThemedMessage(string message)
        {
            using (var msgBox = new ThemedMessageBox(message))
            {
                msgBox.ShowDialog();
            }
        }

        private bool ValidateEmployee(out List<string> errors)
        {
            errors = new List<string>();
            var employee = new Employee
            {
                firstName = txtFirstName.Text.Trim(),
                lastName = txtLastName.Text.Trim(),
                nic = txtNIC.Text.Trim(),
                userName = txtUsername.Text.Trim(),
                password = _isEdit ? "dummy-value" : txtPassword.Text.Trim(), // Bypass required for edit
                address = txtAddress.Text.Trim(),
                contactNo = txtContact.Text.Trim(),
                status = cmbStatus.SelectedItem?.ToString(),
                Role_ID = cmbRole.SelectedValue is int roleId ? roleId : 0
            };

            // Data Annotations Validation
            var validationContext = new ValidationContext(employee);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(employee, validationContext, validationResults, true);

            // Custom Validation
            if (!_isEdit && string.IsNullOrEmpty(txtPassword.Text))
                validationResults.Add(new ValidationResult("Password required", new[] { "Password" }));

            if (employee.password.Contains(employee.userName))
                validationResults.Add(new ValidationResult("Password cannot contain username"));

            // NIC Validation
            if (!Regex.IsMatch(employee.nic, @"^([0-9]{9}[VvXx]|[0-9]{12})$"))
                errors.Add("Invalid NIC format");

            // Contact Validation
            if (!string.IsNullOrEmpty(employee.contactNo) &&
                !Regex.IsMatch(employee.contactNo, @"^0[0-9]{9}$"))
                errors.Add("Invalid phone number format");

            // Role Validation
            if (cmbRole.SelectedValue == null || (int)cmbRole.SelectedValue < 1)
                errors.Add("Role selection required");

            // Existing Check
            if (!_isEdit && _repo.CheckExisting(employee.nic, employee.userName, employee.contactNo))
                errors.Add("NIC/Username/Contact already exists");

            // Compile errors
            errors.AddRange(validationResults.Select(vr => vr.ErrorMessage));
            errors = errors.Distinct().Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            return !errors.Any();
        }

        private void MapFormToModel()
        {
            _employee.firstName = txtFirstName.Text.Trim();
            _employee.lastName = txtLastName.Text.Trim();
            _employee.userName = txtUsername.Text.Trim();
            _employee.address = txtAddress.Text.Trim();
            _employee.contactNo = txtContact.Text.Trim();
            _employee.status = cmbStatus.SelectedItem?.ToString();
            _employee.Role_ID = (int)cmbRole.SelectedValue;

            if (!_isEdit)
            {
                _employee.nic = txtNIC.Text.Trim();
                _employee.password = HashHelper.ComputeSqlCompatibleHash(txtPassword.Text);
            }

            if (pictureBox.Image != null)
            {
                using var ms = new MemoryStream();
                pictureBox.Image.Save(ms, ImageFormat.Jpeg);
                _employee.picture = ms.ToArray();
            }
        }

    }
}
