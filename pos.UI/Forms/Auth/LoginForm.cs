using pos_system.pos.BLL.Services;
using pos_system.pos.Models;
using pos_system.pos.UI.Forms;
using System;
using System.Diagnostics;
using pos_system;
using pos_system.pos.UI.Forms.Common;

namespace pos_system.pos.UI.Forms.Auth
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            WireUpEvents();
            this.AcceptButton = btnLogin;
        }

        private void WireUpEvents()
        {
            txtUsername.Enter += ClearPlaceholderText;
            txtUsername.Leave += RestorePlaceholderText;
            txtPassword.Enter += ClearPlaceholderText;
            txtPassword.Leave += RestorePlaceholderText;
            txtUsername.KeyDown += TextBox_KeyDown;
            txtPassword.KeyDown += TextBox_KeyDown;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
                e.SuppressKeyPress = true; // Prevent system beep
            }
        }

        private void ClearPlaceholderText(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (textBox == txtUsername && textBox.Text == "Username")
            {
                textBox.Text = string.Empty;
                textBox.ForeColor = SystemColors.WindowText;
            }
            else if (textBox == txtPassword && textBox.Text == "Password")
            {
                textBox.Text = string.Empty;
                textBox.ForeColor = SystemColors.WindowText;
                textBox.UseSystemPasswordChar = true;
            }
        }

        private void RestorePlaceholderText(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (textBox == txtUsername && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Username";
                textBox.ForeColor = SystemColors.GrayText;
            }
            else if (textBox == txtPassword && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Password";
                textBox.ForeColor = SystemColors.GrayText;
                textBox.UseSystemPasswordChar = false;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || (password == "Password" && username == "Username"))
                {
                    ThemedMessageBox.Show("Please enter username and password", "Warning", ThemedMessageBoxIcon.Warning
                        );
                    return;
                }

                var userService = new UserService();
                Employee loggedInUser = userService.AuthenticateUser(username, password);
                Debug.WriteLine("LoginForm.cs - " + loggedInUser);

                if (loggedInUser != null)
                {
                    // Determine role and open appropriate form
                    if (loggedInUser.Role_ID == 4) // Cashier role
                    {
                        pos_system.pos.UI.Forms.Dashboard.CashierForm cashierForm = new pos_system.pos.UI.Forms.Dashboard.CashierForm(loggedInUser);
                        cashierForm.Show();
                    }
                    else // Owner/Admin/Manager
                    {
                        pos_system.pos.UI.Forms.Dashboard.OwnerDashboard ownerForm = new pos_system.pos.UI.Forms.Dashboard.OwnerDashboard(loggedInUser);
                        ownerForm.Show();
                    }

                    this.Hide();
                }
                else
                {
                    pos_system.pos.UI.Forms.Common.ThemedMessageBox.Show("Invalid username or password", "Warning", ThemedMessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Login error: " + ex.Message);
                ShowThemedMessage($"Login error: {ex.Message}");
                ShowThemedMessage($"Login error: {ex.Message}");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult result = pos_system.pos.UI.Forms.Common.ThemedMessageBoxYesNo.Show("Are you sure you want to leave?", "Warning");

            if (result == DialogResult.Yes)
            {
                this.Close(); // Close form only on "Yes"
            }

        }

        // These initialization events for textboxes
        private void txtUsername_Enter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Username")
            {
                txtUsername.Text = string.Empty;
                txtUsername.ForeColor = Color.Black;
            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = "Username";
                txtUsername.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Password")
            {
                txtPassword.Text = string.Empty;
                txtPassword.ForeColor = System.Drawing.Color.Black;
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.Text = "Password";
                txtPassword.ForeColor = System.Drawing.Color.Gray;
                txtPassword.UseSystemPasswordChar = false;
            }
        }

        public void ShowThemedMessage(string message)
        {
            using (var msgBox = new pos_system.pos.UI.Forms.Common.ThemedMessageBox(message))
            {
                msgBox.ShowDialog();
            }
        }


    }
}
