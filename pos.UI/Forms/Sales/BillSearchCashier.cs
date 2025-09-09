using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using pos_system.pos.Models;
using RetailPOS.DAL.Repositories;
using pos_system.pos.Core;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class BillSearchCashier : Form
    {
        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color LightBlue = Color.FromArgb(189, 224, 254);
        private readonly Color White = Color.White;
        private readonly Color DarkText = Color.FromArgb(64, 64, 64);
        private readonly Color LightGray = Color.FromArgb(240, 240, 240);
        private readonly Color HoverBlue = Color.FromArgb(31, 118, 175);
        private readonly Color HoverGray = Color.FromArgb(220, 220, 220);

        private Employee _currentUser;
        private TextBox txtBillId;
        private TextBox txtContact;
        private Button btnSearch;
        private Button btnClear;
        private DataGridView dgvBills;
        private Panel panelTitle;
        private Label lblTitle;
        private Panel txtBillIdUnderline;
        private Panel txtContactUnderline;

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
            this.Padding = new Padding(12);
            this.BackColor = White;
            this.Text = "Bill Search";

            // Title Panel
            panelTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = PrimaryBlue,
                Padding = new Padding(15, 0, 0, 0)
            };

            lblTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = White,
                Text = "Bill Search",
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 8, 0, 0),
            };
            panelTitle.Controls.Add(lblTitle);
            this.Controls.Add(panelTitle);

            // Search Panel
            Panel searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(10, 15, 10, 10),
                BackColor = White
            };

            // Bill ID Field
            txtBillId = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                ForeColor = DarkText,
                BackColor = White,
                BorderStyle = BorderStyle.None,
                Location = new Point(10, 10),
                Width = 150,
                Height = 30,
                PlaceholderText = "Bill Number"
            };

            txtBillIdUnderline = new Panel
            {
                BackColor = Color.Gray,
                Location = new Point(10, 40),
                Size = new Size(150, 1)
            };

            // Contact Field
            txtContact = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                ForeColor = DarkText,
                BackColor = White,
                BorderStyle = BorderStyle.None,
                Location = new Point(180, 10),
                Width = 200,
                Height = 30,
                PlaceholderText = "Contact Number"
            };

            txtContactUnderline = new Panel
            {
                BackColor = Color.Gray,
                Location = new Point(180, 40),
                Size = new Size(200, 1)
            };

            // Search Button
            btnSearch = new Button
            {
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = PrimaryBlue,
                ForeColor = White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(400, 10),
                Size = new Size(100, 35),
                Text = "Search",
                Cursor = Cursors.Hand
            };

            // Clear Button
            btnClear = new Button
            {
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = LightGray,
                ForeColor = DarkText,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(510, 10),
                Size = new Size(100, 35),
                Text = "Clear",
                Cursor = Cursors.Hand
            };

            // Add event handlers
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            txtBillId.KeyPress += txtBillId_KeyPress;
            txtContact.KeyPress += txtContact_KeyPress;

            // Textbox focus events
            txtBillId.Enter += (s, e) => txtBillIdUnderline.BackColor = PrimaryBlue;
            txtBillId.Leave += (s, e) => txtBillIdUnderline.BackColor = Color.Gray;
            txtContact.Enter += (s, e) => txtContactUnderline.BackColor = PrimaryBlue;
            txtContact.Leave += (s, e) => txtContactUnderline.BackColor = Color.Gray;

            // Button hover effects
            btnSearch.MouseEnter += (s, e) => btnSearch.BackColor = HoverBlue;
            btnSearch.MouseLeave += (s, e) => btnSearch.BackColor = PrimaryBlue;
            btnClear.MouseEnter += (s, e) => btnClear.BackColor = HoverGray;
            btnClear.MouseLeave += (s, e) => btnClear.BackColor = LightGray;

            // Add controls to panel
            searchPanel.Controls.Add(txtBillId);
            searchPanel.Controls.Add(txtBillIdUnderline);
            searchPanel.Controls.Add(txtContact);
            searchPanel.Controls.Add(txtContactUnderline);
            searchPanel.Controls.Add(btnSearch);
            searchPanel.Controls.Add(btnClear);

            // DataGridView
            dgvBills = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                BackgroundColor = White,
                BorderStyle = BorderStyle.None,
                GridColor = LightGray,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };

            // Style headers
            dgvBills.EnableHeadersVisualStyles = false;
            dgvBills.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = LightBlue,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = DarkText,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5, 5, 5, 5)
            };
            dgvBills.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvBills.ColumnHeadersHeight = 40;
            dgvBills.RowTemplate.Height = 35;
            dgvBills.RowTemplate.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvBills.DefaultCellStyle.ForeColor = DarkText;
            dgvBills.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, LightBlue);
            dgvBills.DefaultCellStyle.SelectionForeColor = DarkText;
            dgvBills.ScrollBars = ScrollBars.Vertical;

            // Create columns
            DataGridViewTextBoxColumn colBillId = new DataGridViewTextBoxColumn
            {
                Name = "colBillId",
                HeaderText = "BILL #",
                DataPropertyName = "Bill_ID",
                MinimumWidth = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5, 0, 5, 0)
                },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            DataGridViewTextBoxColumn colContact = new DataGridViewTextBoxColumn
            {
                Name = "colContact",
                HeaderText = "CONTACT",
                DataPropertyName = "CustomerContact",
                MinimumWidth = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Padding = new Padding(5, 0, 5, 0)
                }
            };

            DataGridViewTextBoxColumn colBrand = new DataGridViewTextBoxColumn
            {
                Name = "colBrand",
                HeaderText = "BRAND",
                DataPropertyName = "brandName",
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Padding = new Padding(5, 0, 5, 0)
                }
            };

            DataGridViewTextBoxColumn colCategory = new DataGridViewTextBoxColumn
            {
                Name = "colCategory",
                HeaderText = "CATEGORY",
                DataPropertyName = "categoryName",
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Padding = new Padding(5, 0, 5, 0)
                }
            };

            DataGridViewTextBoxColumn colSize = new DataGridViewTextBoxColumn
            {
                Name = "colSize",
                HeaderText = "SIZE",
                DataPropertyName = "SizeLabel",
                MinimumWidth = 70,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5, 0, 5, 0)
                },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            DataGridViewTextBoxColumn colRetailPrice = new DataGridViewTextBoxColumn
            {
                Name = "colRetailPrice",
                HeaderText = "RETAIL PRICE",
                DataPropertyName = "BaseRetailPrice",
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(0, 0, 10, 0)
                }
            };

            DataGridViewTextBoxColumn colSoldPrice = new DataGridViewTextBoxColumn
            {
                Name = "colSoldPrice",
                HeaderText = "SOLD PRICE",
                DataPropertyName = "ActualSellingPrice",
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(0, 0, 10, 0)
                }
            };

            DataGridViewTextBoxColumn colImageFilename = new DataGridViewTextBoxColumn
            {
                Name = "colImageFilename",
                DataPropertyName = "ImageFilename",
                Visible = false
            };

            // Use a TEXT column for image display (not image column)
            DataGridViewTextBoxColumn colImage = new DataGridViewTextBoxColumn
            {
                Name = "colImage",
                HeaderText = "IMAGE",
                // No DataPropertyName - we'll handle manually
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5)
                }
            };

            // Add columns to grid
            dgvBills.Columns.AddRange(new DataGridViewColumn[] {
                colBillId, colContact, colBrand, colCategory, colSize,
                colRetailPrice, colSoldPrice, colImageFilename, colImage
            });

            dgvBills.RowTemplate.Height = 70;

            dgvBills.CellPainting += DgvBills_CellPainting;
            dgvBills.CellValueNeeded += DgvBills_CellValueNeeded;

            // Handle form resizing
            this.Resize += (s, e) => AdjustColumnWidths();
            dgvBills.DataBindingComplete += (s, e) => AdjustColumnWidths();

            // Add panels to form
            this.Controls.Add(dgvBills);
            this.Controls.Add(searchPanel);
            this.Controls.Add(panelTitle);
        }

        private void DgvBills_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // For our image column, always return empty string
            if (e.ColumnIndex == dgvBills.Columns["colImage"].Index && e.RowIndex >= 0)
            {
                e.Value = string.Empty;
            }
        }

        private void DgvBills_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Always paint the cell background
            e.PaintBackground(e.CellBounds, true);

            // Only handle our image column and valid rows
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvBills.Columns["colImage"].Index)
            {
                e.PaintContent(e.CellBounds);

                try
                {
                    // Get filename from hidden column
                    string filename = dgvBills.Rows[e.RowIndex].Cells["colImageFilename"].Value?.ToString();
                    Image image = ImageHelper.LoadProductImage(filename);

                    if (image != null)
                    {
                        // Calculate aspect ratio
                        float ratio = Math.Min(
                            (float)(e.CellBounds.Width - 10) / image.Width,
                            (float)(e.CellBounds.Height - 10) / image.Height
                        );

                        int newWidth = (int)(image.Width * ratio);
                        int newHeight = (int)(image.Height * ratio);
                        int x = e.CellBounds.X + (e.CellBounds.Width - newWidth) / 2;
                        int y = e.CellBounds.Y + (e.CellBounds.Height - newHeight) / 2;

                        e.Graphics.DrawImage(image, x, y, newWidth, newHeight);
                    }
                    else
                    {
                        // Draw placeholder text
                        TextRenderer.DrawText(e.Graphics, "No Image",
                            dgvBills.Font, e.CellBounds, Color.Gray,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }
                }
                catch
                {
                    // Draw error text
                    TextRenderer.DrawText(e.Graphics, "Error",
                        dgvBills.Font, e.CellBounds, Color.Red,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                e.Handled = true;
            }
        }
        private void AdjustColumnWidths()
        {
            if (dgvBills == null || dgvBills.IsDisposed || dgvBills.Columns.Count < 8)
                return;

            try
            {
                dgvBills.SuspendLayout();

                // Calculate available width
                int verticalScrollbarWidth = dgvBills.Controls.OfType<VScrollBar>().FirstOrDefault(v => v.Visible)?.Width ?? 0;
                int availableWidth = dgvBills.ClientSize.Width - verticalScrollbarWidth;

                // Fixed widths for non-text columns
                const int billIdWidth = 80;
                const int sizeWidth = 70;
                const int priceWidth = 100;
                const int imageWidth = 150;

                // Calculate remaining width
                int remainingWidth = availableWidth - billIdWidth - sizeWidth - (priceWidth * 2) - imageWidth;
                int contactWidth = (int)(remainingWidth * 0.4);  // 40% of remaining
                int brandWidth = (int)(remainingWidth * 0.3);    // 30% of remaining
                int categoryWidth = remainingWidth - contactWidth - brandWidth; // 30% of remaining

                // Apply widths
                dgvBills.Columns["colBillId"].Width = billIdWidth;
                dgvBills.Columns["colContact"].Width = Math.Max(contactWidth, 120);
                dgvBills.Columns["colBrand"].Width = Math.Max(brandWidth, 100);
                dgvBills.Columns["colCategory"].Width = Math.Max(categoryWidth, 100);
                dgvBills.Columns["colSize"].Width = sizeWidth;
                dgvBills.Columns["colRetailPrice"].Width = priceWidth;
                dgvBills.Columns["colSoldPrice"].Width = priceWidth;
                dgvBills.Columns["colImage"].Width = imageWidth;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adjusting columns: {ex.Message}");
            }
            finally
            {
                dgvBills.ResumeLayout();
            }
        }

        //private void AdjustColumnWidths()
        //{
        //    // Safe check: Ensure grid and columns exist
        //    if (dgvBills == null || dgvBills.IsDisposed || dgvBills.Columns.Count == 0)
        //        return;

        //    try
        //    {
        //        dgvBills.SuspendLayout();

        //        // Calculate available width (accounting for scrollbar)
        //        int verticalScrollbarWidth = dgvBills.Controls.OfType<VScrollBar>().FirstOrDefault(v => v.Visible)?.Width ?? 0;
        //        int availableWidth = dgvBills.ClientSize.Width - verticalScrollbarWidth - 1;

        //        // Set proportional widths
        //        const double totalPercentage = 0.84; // 84% for content columns
        //        int billIdWidth = (int)(availableWidth * 0.08);   // 8%
        //        int contactWidth = (int)(availableWidth * 0.15);  // 15%
        //        int brandWidth = (int)(availableWidth * 0.12);    // 12%
        //        int categoryWidth = (int)(availableWidth * 0.12); // 12%
        //        int sizeWidth = (int)(availableWidth * 0.07);     // 7%
        //        int retailWidth = (int)(availableWidth * 0.10);   // 10%
        //        int soldWidth = (int)(availableWidth * 0.10);     // 10%
        //        int imageWidth = (int)(availableWidth * 0.10);    // 10%

        //        // Apply calculated widths with minimums
        //        dgvBills.Columns["colBillId"].Width = Math.Max(billIdWidth, 80);
        //        dgvBills.Columns["colContact"].Width = Math.Max(contactWidth, 120);
        //        dgvBills.Columns["colBrand"].Width = Math.Max(brandWidth, 100);
        //        dgvBills.Columns["colCategory"].Width = Math.Max(categoryWidth, 100);
        //        dgvBills.Columns["colSize"].Width = Math.Max(sizeWidth, 70);
        //        dgvBills.Columns["colRetailPrice"].Width = Math.Max(retailWidth, 100);
        //        dgvBills.Columns["colSoldPrice"].Width = Math.Max(soldWidth, 100);
        //        dgvBills.Columns["colImage"].Width = Math.Max(imageWidth, 100);

        //        // Fill column will automatically take remaining space
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log error or show message if needed
        //        Console.WriteLine($"Error adjusting column widths: {ex.Message}");
        //    }
        //    finally
        //    {
        //        dgvBills.ResumeLayout();
        //    }
        //}

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchBills();
        }

        private void SearchBills()
        {
            if (string.IsNullOrWhiteSpace(txtBillId.Text) &&
                string.IsNullOrWhiteSpace(txtContact.Text))
            {
                MessageBox.Show("Please enter Bill Number or Contact Number to search", "Search Criteria",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("Please enter a valid bill number", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            string contact = txtContact.Text.Trim();

            try
            {
                DataTable dt = BillRepository.SearchBills(billId, contact);
                dgvBills.DataSource = dt;

                // Force repaint to show images
                dgvBills.Refresh();

                if (dt != null && dt.Rows.Count > 0)
                {
                    AdjustColumnWidths();
                }
                else
                {
                    MessageBox.Show("No bills found matching your criteria", "Search Results",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching bills: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBillId.Clear();
            txtContact.Clear();
            dgvBills.DataSource = null;
            dgvBills.Rows.Clear();
        }

        private void txtBillId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchBills();
                e.Handled = true;
            }
            else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AdjustColumnWidths();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            AdjustColumnWidths();
        }
    }
}