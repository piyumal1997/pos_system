using BarTender;
using pos_system.pos.BLL.Services;
using pos_system.pos.Models;
using System.Data;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;
using BTApplication = BarTender.Application;
using BTFormat = BarTender.Format;

namespace pos_system.pos.UI.Forms.Inventory
{
    public partial class BarcodePrint : Form
    {
        // Theme colors
        private static readonly Color PrimaryColor = Color.FromArgb(41, 128, 185);
        private static readonly Color BackgroundColor = Color.White;
        private static readonly Color HeaderColor = Color.FromArgb(230, 244, 253);
        private static readonly Color ForegroundColor = Color.Black;
        private static readonly Color SecondaryColor = Color.Gray;
        private static readonly Color SelectionColor = Color.FromArgb(200, 230, 255);

        // UI Controls
        private DataGridView dgvItems;
        private Button btnPreview;
        private Button btnPrint;
        private ComboBox cboPrinters;
        private ComboBox cboTemplates;
        private Label lblPrinter;
        private Label lblTemplate;
        private NumericUpDown nudPrintCount;
        private PictureBox picPreview;
        private TextBox txtSearch;
        private ComboBox cboBrand;
        private ComboBox cboCategory;
        private Button btnSearch;
        private Button btnClear;
        private TextBox txtBarcode;
        private TextBox txtPrice;
        private TextBox txtBrand;
        private TextBox txtCategory;
        private TextBox txtSize;
        private TextBox txtQuantity;

        // Data
        private List<Item> items = new List<Item>();
        private Item selectedItem;
        private List<TemplateItem> templateItems = new List<TemplateItem>();
        private readonly ItemService _itemService = new ItemService();

        // State
        private volatile bool _isClosing = false;

        public BarcodePrint()
        {
            InitializeForm();
            LoadTemplates();
            LoadPrinters();
            LoadBrands();
            LoadCategories();
            LoadItems();

            this.FormClosing += (s, e) => _isClosing = true;
        }

        private void InitializeForm()
        {
            // Form setup
            this.Text = "BARCODE LABEL PRINTING";
            this.Size = new Size(980, 700);
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
                Text = "BARCODE LABEL PRINTING",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Padding = new Padding(20, 0, 0, 0)
            };
            titlePanel.Controls.Add(lblTitle);

            // Search panel
            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = HeaderColor,
                Padding = new Padding(10)
            };

            // Build search controls
            txtSearch = new TextBox
            {
                PlaceholderText = "Search by description or barcode...",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 0, 10, 0)
            };
            txtSearch.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter) PerformSearch();
            };

            cboBrand = new ComboBox
            {
                Dock = DockStyle.Right,
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 0, 10, 0)
            };

            cboCategory = new ComboBox
            {
                Dock = DockStyle.Right,
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 0, 10, 0)
            };

            btnSearch = CreateButton("SEARCH", PrimaryColor, 90, 30);
            btnSearch.Click += (s, e) => PerformSearch();

            btnClear = CreateButton("CLEAR", SecondaryColor, 80, 30);
            btnClear.Click += (s, e) => ClearSearch();

            // Search container layout
            var searchContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                Padding = new Padding(0)
            };
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Search box
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Category
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Brand
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));  // Clear
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90)); // Search
            searchContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));  // Spacer

            searchContainer.Controls.Add(txtSearch, 0, 0);
            searchContainer.Controls.Add(cboCategory, 1, 0);
            searchContainer.Controls.Add(cboBrand, 2, 0);
            searchContainer.Controls.Add(btnClear, 3, 0);
            searchContainer.Controls.Add(btnSearch, 4, 0);
            searchPanel.Controls.Add(searchContainer);

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

            // Right panel - Details and controls
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 0, 0, 0)
            };

            // Details section
            var detailsGroup = new GroupBox
            {
                Text = "ITEM DETAILS",
                Dock = DockStyle.Top,
                Height = 190,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor
            };

            var detailsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(10, 0, 10, 10)
            };

            // Create detail labels and text boxes
            detailsLayout.Controls.Add(CreateDetailLabel("Barcode:"), 0, 0);
            txtBarcode = CreateDetailTextBox();
            detailsLayout.Controls.Add(txtBarcode, 1, 0);

            detailsLayout.Controls.Add(CreateDetailLabel("Price:"), 0, 1);
            txtPrice = CreateDetailTextBox();
            detailsLayout.Controls.Add(txtPrice, 1, 1);

            detailsLayout.Controls.Add(CreateDetailLabel("Brand:"), 0, 2);
            txtBrand = CreateDetailTextBox();
            detailsLayout.Controls.Add(txtBrand, 1, 2);

            detailsLayout.Controls.Add(CreateDetailLabel("Category:"), 0, 3);
            txtCategory = CreateDetailTextBox();
            detailsLayout.Controls.Add(txtCategory, 1, 3);

            detailsLayout.Controls.Add(CreateDetailLabel("Size:"), 0, 4);
            txtSize = CreateDetailTextBox();
            detailsLayout.Controls.Add(txtSize, 1, 4);

            detailsLayout.Controls.Add(CreateDetailLabel("Quantity:"), 0, 5);
            txtQuantity = CreateDetailTextBox();
            detailsLayout.Controls.Add(txtQuantity, 1, 5);

            detailsGroup.Controls.Add(detailsLayout);

            // Preview section
            var previewGroup = new GroupBox
            {
                Text = "LABEL PREVIEW",
                Dock = DockStyle.Top,
                Height = 160,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = PrimaryColor
            };

            var previewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            picPreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            previewPanel.Controls.Add(picPreview);
            previewGroup.Controls.Add(previewPanel);

            // Controls section
            var controlsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 3,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 0)
            };
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32)); // Template
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32)); // Printer
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32)); // Print Count
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Buttons

            // Template selection
            lblTemplate = new Label
            {
                Text = "Template:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            cboTemplates = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cboTemplates.SelectedIndexChanged += (s, e) => RegeneratePreview();

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

            // Create Preview and Print buttons
            btnPreview = CreateButton("PREVIEW", Color.SteelBlue, 140, 35);
            btnPreview.Enabled = false;
            btnPreview.Click += BtnPreview_Click;

            btnPrint = CreateButton("PRINT LABELS", PrimaryColor, 140, 35);
            btnPrint.Enabled = false;
            btnPrint.Click += BtnPrint_Click;

            // Layout controls
            controlsPanel.Controls.Add(lblTemplate, 0, 0);
            controlsPanel.Controls.Add(cboTemplates, 1, 0);
            controlsPanel.SetColumnSpan(cboTemplates, 2);

            controlsPanel.Controls.Add(lblPrinter, 0, 1);
            controlsPanel.Controls.Add(cboPrinters, 1, 1);
            controlsPanel.SetColumnSpan(cboPrinters, 2);

            controlsPanel.Controls.Add(lblPrintCount, 0, 2);
            controlsPanel.Controls.Add(nudPrintCount, 1, 2);
            controlsPanel.SetColumnSpan(nudPrintCount, 2);

            controlsPanel.Controls.Add(btnPreview, 1, 3);
            controlsPanel.Controls.Add(btnPrint, 2, 3);

            // Events
            dgvItems.SelectionChanged += DgvItems_SelectionChanged;

            // Build panels
            leftPanel.Controls.Add(dgvItems);

            rightPanel.Controls.Add(controlsPanel);
            rightPanel.Controls.Add(previewGroup);
            rightPanel.Controls.Add(detailsGroup);

            titlePanel.Controls.Add(lblTitle);

            contentPanel.Controls.Add(rightPanel);
            contentPanel.Controls.Add(leftPanel);

            container.Controls.Add(contentPanel);
            container.Controls.Add(searchPanel);
            container.Controls.Add(titlePanel);

            this.Controls.Add(container);
            ConfigureGridColumns();
        }

        private Label CreateDetailLabel(string text) => new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Segoe UI", 10)
        };

        private TextBox CreateDetailTextBox() => new TextBox
        {
            ReadOnly = true,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            BackColor = HeaderColor,
            Font = new Font("Segoe UI", 10),
            Margin = new Padding(3, 5, 3, 5)
        };

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
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Margin = new Padding(5, 0, 5, 0),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
        }

        private void ConfigureGridColumns()
        {
            dgvItems.Columns.Clear();

            dgvItems.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ProductSize_ID",
                    HeaderText = "PS_ID",
                    Width = 40,
                    Name = "Item_ID",
                    Visible = false
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Product_ID",
                    HeaderText = "ID",
                    Width = 40,
                    Name = "Product_ID"
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Barcode",
                    HeaderText = "BARCODE",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Description",
                    HeaderText = "DESCRIPTION",
                    Width = 200
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "RetailPrice",
                    HeaderText = "PRICE",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
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
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "SizeLabel",
                    HeaderText = "SIZE",
                    Width = 70
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Quantity",
                    HeaderText = "QTY",
                    Width = 60,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                }
            );
        }

        private void LoadTemplates()
        {
            try
            {
                string templateDir = Path.Combine(System.Windows.Forms.Application.StartupPath, "Barcode");

                if (!Directory.Exists(templateDir))
                {
                    MessageBox.Show($"Template directory not found:\n{templateDir}",
                                    "Missing Templates", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var templateFiles = Directory.GetFiles(templateDir, "*.btw");

                if (templateFiles.Length == 0)
                {
                    MessageBox.Show("No template files found in the template directory",
                                    "Missing Templates", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                templateItems.Clear();
                cboTemplates.Items.Clear();

                foreach (string file in templateFiles)
                {
                    string fileName = Path.GetFileName(file);
                    int labelsPerRow = GetLabelsPerRowFromFileName(fileName);

                    templateItems.Add(new TemplateItem
                    {
                        FileName = fileName,
                        FilePath = file,
                        LabelsPerRow = labelsPerRow
                    });
                }

                // Sort templates alphabetically
                templateItems = templateItems.OrderBy(t => t.FileName).ToList();

                foreach (var template in templateItems)
                {
                    cboTemplates.Items.Add(template);
                }

                if (cboTemplates.Items.Count > 0)
                {
                    cboTemplates.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading templates: {ex.Message}", "Template Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetLabelsPerRowFromFileName(string fileName)
        {
            // Default to 1 label per row
            int labelsPerRow = 1;

            if (fileName.Contains("2up")) return 2;
            if (fileName.Contains("3up")) return 3;
            if (fileName.Contains("4up")) return 4;

            return labelsPerRow;
        }

        private void RegeneratePreview()
        {
            if (selectedItem != null && btnPreview.Enabled)
            {
                GenerateBarTenderPreview();
            }
        }

        private void LoadPrinters()
        {
            try
            {
                var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
                cboPrinters.DataSource = printers;

                // Set default printer if available
                using (PrintDocument doc = new PrintDocument())
                {
                    if (printers.Contains(doc.PrinterSettings.PrinterName))
                    {
                        cboPrinters.SelectedItem = doc.PrinterSettings.PrinterName;
                    }
                    else if (cboPrinters.Items.Count > 0)
                    {
                        cboPrinters.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading printers: {ex.Message}", "Printer Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBrands()
        {
            try
            {
                var brandService = new BrandService();
                var brands = brandService.GetAllBrands();

                cboBrand.Items.Clear();
                cboBrand.Items.Add(new ComboBoxItem("All Brands", 0));

                foreach (var brand in brands)
                {
                    cboBrand.Items.Add(new ComboBoxItem(brand.brandName, brand.Brand_ID));
                }
                cboBrand.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading brands: {ex.Message}", "Data Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                var categoryService = new CategoryService();
                var categories = categoryService.GetAllCategorie();

                cboCategory.Items.Clear();
                cboCategory.Items.Add(new ComboBoxItem("All Categories", 0));

                if (categories != null)
                {
                    foreach (var category in categories)
                    {
                        if (category != null)
                        {
                            cboCategory.Items.Add(new ComboBoxItem(
                                category.categoryName ?? "Unnamed Category",
                                category.Category_ID
                            ));
                        }
                    }
                }
                cboCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Data Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadItems(string searchTerm = "", int brandId = 0, int categoryId = 0)
        {
            try
            {
                items = _itemService.SearchItemsWithVariants(searchTerm, brandId, categoryId);
                //var repository = new ItemRepository();
                //items = repository.SearchItemsWithVariants(searchTerm, brandId, categoryId);

                dgvItems.Rows.Clear();
                foreach (var item in items)
                {
                    dgvItems.Rows.Add(
                        item.ProductSize_ID,
                        item.Product_ID,
                        item.Barcode,
                        item.Description,
                        item.RetailPrice,
                        item.BrandName,
                        item.CategoryName,
                        item.SizeLabel ?? "N/A",
                        item.Quantity
                    );
                }

                // Clear selection
                dgvItems.ClearSelection();
                selectedItem = null;
                ClearDetails();
                btnPrint.Enabled = false;
                btnPreview.Enabled = false;
                ClearPreviewImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading items: {ex.Message}", "Data Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDetails()
        {
            txtBarcode.Clear();
            txtPrice.Clear();
            txtBrand.Clear();
            txtCategory.Clear();
            txtSize.Clear();
            txtQuantity.Clear();
        }

        private void ClearPreviewImage()
        {
            if (picPreview.InvokeRequired)
            {
                picPreview.Invoke((MethodInvoker)delegate
                {
                    if (!picPreview.IsDisposed)
                    {
                        picPreview.Image?.Dispose();
                        picPreview.Image = null;
                    }
                });
            }
            else
            {
                if (!picPreview.IsDisposed)
                {
                    picPreview.Image?.Dispose();
                    picPreview.Image = null;
                }
            }
        }

        private void PerformSearch()
        {
            string searchTerm = txtSearch.Text.Trim();
            int brandId = (cboBrand.SelectedItem as ComboBoxItem)?.Value ?? 0;
            int categoryId = (cboCategory.SelectedItem as ComboBoxItem)?.Value ?? 0;

            LoadItems(searchTerm, brandId, categoryId);
        }

        private void ClearSearch()
        {
            txtSearch.Clear();
            cboBrand.SelectedIndex = 0;
            cboCategory.SelectedIndex = 0;
            LoadItems();
        }

        private void DgvItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                int selectedId = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["Item_ID"].Value);
                selectedItem = items.FirstOrDefault(i => i.ProductSize_ID == selectedId);

                if (selectedItem != null)
                {
                    // Populate details
                    txtBarcode.Text = selectedItem.Barcode;
                    txtPrice.Text = selectedItem.RetailPrice.ToString("N2");
                    txtBrand.Text = selectedItem.BrandName;
                    txtCategory.Text = selectedItem.CategoryName;
                    txtSize.Text = selectedItem.SizeLabel ?? "N/A";
                    txtQuantity.Text = selectedItem.Quantity.ToString();

                    // Set reasonable print count
                    nudPrintCount.Value = Math.Max(1, Math.Min(selectedItem.Quantity, 100));

                    // Enable printing and preview
                    btnPrint.Enabled = true;
                    btnPreview.Enabled = true;

                    // Generate preview immediately
                    GenerateBarTenderPreview();
                }
            }
            else
            {
                btnPrint.Enabled = false;
                btnPreview.Enabled = false;
                ClearDetails();
                ClearPreviewImage();
            }
        }

        private void GenerateBarTenderPreview()
        {
            // Capture current item to avoid race conditions
            Item currentItem = selectedItem;

            if (cboTemplates.SelectedItem == null)
            {
                return;
            }

            TemplateItem template = (TemplateItem)cboTemplates.SelectedItem;

            if (currentItem == null || !File.Exists(template.FilePath))
            {
                ClearPreviewImage();
                return;
            }

            Thread staThread = new Thread(() =>
            {
                // Check if form is closing before processing
                if (_isClosing) return;

                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                string outputFile = null;
                BTApplication btApp = null;
                BTFormat btFormat = null;

                try
                {
                    // Double-check if form is closing
                    if (_isClosing) return;

                    btApp = new BTApplication();
                    btApp.Visible = false;

                    btFormat = btApp.Formats.Open(template.FilePath, false, "");

                    // Use captured currentItem reference
                    SetFormatData(btFormat, currentItem);

                    // Set labels per row
                    btFormat.PrintSetup.NumberSerializedLabels = template.LabelsPerRow;

                    string fileNameTemplate = "preview_" + Guid.NewGuid().ToString("N") + ".png";
                    ExportLabelToImage(btFormat, tempDir, fileNameTemplate, out outputFile);

                    if (!string.IsNullOrEmpty(outputFile) && File.Exists(outputFile))
                    {
                        using (FileStream fs = new FileStream(outputFile, FileMode.Open, FileAccess.Read))
                        {
                            Image previewImage = Image.FromStream(fs);

                            this.Invoke((MethodInvoker)delegate
                            {
                                // Only update if the current item is still selected
                                if (!_isClosing && !picPreview.IsDisposed && selectedItem == currentItem)
                                {
                                    picPreview.Image?.Dispose();
                                    picPreview.Image = new Bitmap(previewImage);
                                }
                                else
                                {
                                    previewImage.Dispose();
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!_isClosing)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show($"Preview Error: {ex.Message}");
                        });
                    }
                }
                finally
                {
                    CleanupBarTender(ref btFormat, ref btApp);

                    try
                    {
                        if (Directory.Exists(tempDir))
                            Directory.Delete(tempDir, true);
                    }
                    catch { }
                }
            });

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.IsBackground = true;
            staThread.Start();
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            if (selectedItem == null) return;
            GenerateBarTenderPreview();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (selectedItem == null)
            {
                MessageBox.Show("Please select an item first", "No Selection",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboTemplates.SelectedItem == null)
            {
                MessageBox.Show("Please select a template", "Template Required",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string printerName = cboPrinters.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(printerName))
            {
                MessageBox.Show("Please select a printer", "Printer Required",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Capture UI values on the main thread before starting background thread
            TemplateItem template = (TemplateItem)cboTemplates.SelectedItem;
            int copies = (int)nudPrintCount.Value;
            Item itemToPrint = selectedItem;  // Capture current item

            Thread staThread = new Thread(() => PrintWithBarTender(template, printerName, copies, itemToPrint));
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.IsBackground = true;
            staThread.Start();
        }

        private void PrintWithBarTender(TemplateItem template, string printerName, int copies, Item item)
        {
            if (!File.Exists(template.FilePath))
            {
                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show($"Template file not found:\n{template.FilePath}",
                                    "Template Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
                return;
            }

            BTApplication btApp = null;
            BTFormat btFormat = null;

            try
            {
                btApp = new BTApplication();
                btApp.Visible = false;
                btApp.Save(true);  // Prevent save prompts

                btFormat = btApp.Formats.Open(template.FilePath, true, "");
                SetFormatData(btFormat, item);  // Use captured item

                // Configure printer
                btFormat.PrintSetup.Printer = printerName;
                btFormat.PrintSetup.IdenticalCopiesOfLabel = copies;
                btFormat.PrintSetup.NumberSerializedLabels = template.LabelsPerRow;

                // Print
                btFormat.PrintOut(false, true);  // (ShowStatusWindow, WaitUntilFinished)

                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show($"{copies} label(s) sent to printer", "Print Successful",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate {
                    MessageBox.Show($"Print Error: {ex.Message}\n\n{ex.StackTrace}",
                                    "Print Failure",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                });
            }
            finally
            {
                CleanupBarTender(ref btFormat, ref btApp);
            }
        }

        private void SetFormatData(BTFormat format, Item item)
        {
            if (item == null) return;

            try
            {
                format.SetNamedSubStringValue("Barcode", item.Barcode ?? "");
                format.SetNamedSubStringValue("RetailPrice", $"Rs.{item.RetailPrice.ToString("N2")}");
                format.SetNamedSubStringValue("Category", item.CategoryName ?? "");
                format.SetNamedSubStringValue("Size", $"({ItemSize(item.SizeLabel ?? "N/A")})");
                format.SetNamedSubStringValue("Sex", $"({GetGenderCode(item.Gender_ID)})");
                format.SetNamedSubStringValue("CostCode", $"{ConvertNumberToCode(Convert.ToInt32(item.UnitCost).ToString())}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting template data: {ex.Message}");
            }
        }

        public static string ConvertNumberToCode(string number)
        {
            if (string.IsNullOrEmpty(number))
                return "XXXX";

            StringBuilder result = new StringBuilder();
            foreach (char digit in number)
            {
                switch (digit)
                {
                    case '0': result.Append('X'); break;
                    case '1': result.Append('A'); break;
                    case '2': result.Append('B'); break;
                    case '3': result.Append('C'); break;
                    case '4': result.Append('D'); break;
                    case '5': result.Append('E'); break;
                    case '6': result.Append('F'); break;
                    case '7': result.Append('G'); break;
                    case '8': result.Append('H'); break;
                    case '9': result.Append('I'); break;
                    default: result.Append('?'); break;
                }
            }
            return result.ToString();
        }
        private string ItemSize(string size)
        {
            return size switch
            {
                "One Size" => "One",  // One Size
                "XXXL" => "3XL",
                "XXXXL" => "4XL",
                "XXXXXL" => "5XL",
                "Free Size" => "Fr",
                "Standard Size" => "SS",
                "18ml" => "18",
                "30ml" => "30",
                "50ml" => "50",
                "80ml" => "80",
                "100ml" => "100",
                "120ml" => "120",
                "150ml" => "150",
                "200ml" => "200",
                "Zipper Half" => "Z Half",
                "Zipper Pocket" => "Z P",
                "Zipper Full" => "Z Full",
                _ => size,  // Other Any Sizes
            };
        }

        private string GetGenderCode(int genderId)
        {
            return genderId switch
            {
                1 => "Male",  // Male
                2 => "Female",  // Female
                3 => "Unisex",  // Unisex
                4 => "None",  // None
                _ => "None"   // Default to None
            };
        }

        private void ExportLabelToImage(BTFormat format, string directory,
                                       string fileNameTemplate, out string outputFilePath)
        {
            outputFilePath = null;

            try
            {
                BarTender.Messages messages;
                _ = format.ExportPrintPreviewToImage(
                    directory,
                    fileNameTemplate,
                    "PNG",
                    BtColors.btColors24Bit,
                    203,
                    0,
                    BtSaveOptions.btDoNotSaveChanges,
                    false,
                    true,
                    out messages
                );

                outputFilePath = FindGeneratedFile(directory, fileNameTemplate);
            }
            catch (Exception ex)
            {
                throw new Exception("Export failed: " + ex.Message);
            }
        }

        private string FindGeneratedFile(string directory, string fileNameTemplate)
        {
            string searchPattern = Path.GetFileNameWithoutExtension(fileNameTemplate) + "*" +
                                   Path.GetExtension(fileNameTemplate);

            var files = Directory.GetFiles(directory, searchPattern);
            if (files.Length > 0) return files[0];

            string exactPath = Path.Combine(directory, fileNameTemplate);
            return File.Exists(exactPath) ? exactPath : null;
        }

        private void CleanupBarTender(ref BTFormat format, ref BTApplication app)
        {
            try
            {
                if (format != null)
                {
                    format.Close(BtSaveOptions.btDoNotSaveChanges);
                    while (Marshal.ReleaseComObject(format) > 0) { }
                    format = null;
                }

                if (app != null)
                {
                    app.Quit(BtSaveOptions.btDoNotSaveChanges);
                    while (Marshal.ReleaseComObject(app) > 0) { }
                    app = null;
                }
            }
            catch { }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        // Helper classes
        private class ComboBoxItem
        {
            public string Text { get; }
            public int Value { get; }

            public ComboBoxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString() => Text;
        }

        private class TemplateItem
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public int LabelsPerRow { get; set; }

            public override string ToString()
            {
                return $"{FileName} ({LabelsPerRow} per row)";
            }
        }
    }
}
