using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using pos_system.pos.Models;
using pos_system.pos.DAL.Repositories;
using RetailPOS.DAL.Repositories;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class BillSearchCashier : Form
    {
        private Employee _currentUser;
        private TextBox txtBillId;
        private TextBox txtContact;
        private Button btnSearch;
        private Button btnClear;
        private DataGridView dgvBills;

        public BillSearchCashier(Employee user)
        {
            _currentUser = user;
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Form setup
            this.Size = new Size(980, 656);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(10);

            // Create search panel
            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 50;
            searchPanel.Padding = new Padding(0, 0, 0, 10);

            // Create search controls
            txtBillId = new TextBox
            {
                Location = new Point(10, 10),
                Width = 100,
                Height = 30,
                PlaceholderText = "Bill Number"
            };

            txtContact = new TextBox
            {
                Location = new Point(120, 10),
                Width = 200,
                Height = 30,
                PlaceholderText = "Contact Number"
            };

            btnSearch = new Button
            {
                Location = new Point(330, 10),
                Text = "Search",
                Size = new Size(80, 30)
            };

            btnClear = new Button
            {
                Location = new Point(420, 10),
                Text = "Clear",
                Size = new Size(80, 30)
            };

            // Add event handlers
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            txtBillId.KeyPress += txtBillId_KeyPress;
            txtContact.KeyPress += txtContact_KeyPress;

            // Add controls to panel
            searchPanel.Controls.Add(txtBillId);
            searchPanel.Controls.Add(txtContact);
            searchPanel.Controls.Add(btnSearch);
            searchPanel.Controls.Add(btnClear);

            // Create DataGridView
            dgvBills = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                BackgroundColor = SystemColors.Window
            };

            // Create columns
            DataGridViewTextBoxColumn colBillId = new DataGridViewTextBoxColumn
            {
                Name = "colBillId",
                HeaderText = "Bill Number",
                DataPropertyName = "Bill_ID",
                Width = 80
            };

            DataGridViewTextBoxColumn colContact = new DataGridViewTextBoxColumn
            {
                Name = "colContact",
                HeaderText = "Contact",
                DataPropertyName = "CustomerContact",
                Width = 120
            };

            DataGridViewTextBoxColumn colBrand = new DataGridViewTextBoxColumn
            {
                Name = "colBrand",
                HeaderText = "Brand",
                DataPropertyName = "brandName",
                Width = 100
            };

            DataGridViewTextBoxColumn colCategory = new DataGridViewTextBoxColumn
            {
                Name = "colCategory",
                HeaderText = "Category",
                DataPropertyName = "categoryName",
                Width = 100
            };

            DataGridViewTextBoxColumn colSize = new DataGridViewTextBoxColumn
            {
                Name = "colSize",
                HeaderText = "Size",
                DataPropertyName = "SizeLabel",
                Width = 70
            };

            DataGridViewTextBoxColumn colRetailPrice = new DataGridViewTextBoxColumn
            {
                Name = "colRetailPrice",
                HeaderText = "Retail Price",
                DataPropertyName = "RetailPrice",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            };

            DataGridViewTextBoxColumn colSoldPrice = new DataGridViewTextBoxColumn
            {
                Name = "colSoldPrice",
                HeaderText = "Sold Price",
                DataPropertyName = "ItemSellingPrice",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            };

            DataGridViewImageColumn colImage = new DataGridViewImageColumn
            {
                Name = "colImage",
                HeaderText = "Image",
                DataPropertyName = "ItemImage",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 100
            };

            // Add columns to grid
            dgvBills.Columns.AddRange(new DataGridViewColumn[] {
                colBillId, colContact, colBrand, colCategory, colSize,
                colRetailPrice, colSoldPrice, colImage
            });

            // Add panels to form
            this.Controls.Add(dgvBills);
            this.Controls.Add(searchPanel);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchBills();
        }

        private void SearchBills()
        {
            // Fixed condition to check for empty/whitespace
            if (string.IsNullOrWhiteSpace(txtBillId.Text) &&
                string.IsNullOrWhiteSpace(txtContact.Text))
            {
                MessageBox.Show("Please enter Bill Number or Contact Number to search");
                return;
            }

            int? billId = null;
            if (!string.IsNullOrWhiteSpace(txtBillId.Text))
            {
                if (int.TryParse(txtBillId.Text, out int id))
                {
                    billId = id;
                }
                else
                {
                    MessageBox.Show("Please enter a valid bill number");
                    return;
                }
            }

            string contact = txtContact.Text.Trim();

            try
            {
                DataTable dt = BillRepository.SearchBills(billId, contact);
                dgvBills.DataSource = dt;

                // Handle image rendering
                dgvBills.CellFormatting += (s, ev) =>
                {
                    if (dgvBills.Columns[ev.ColumnIndex].Name == "colImage")
                    {
                        if (ev.Value != null && ev.Value is byte[])
                        {
                            using (var ms = new System.IO.MemoryStream((byte[])ev.Value))
                            {
                                ev.Value = Image.FromStream(ms);
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching bills: {ex.Message}");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBillId.Clear();
            txtContact.Clear();
            dgvBills.DataSource = null;
        }

        private void txtBillId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchBills();
                e.Handled = true;
            }
        }

        private void txtContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchBills();
                e.Handled = true;
            }
        }
    }
}