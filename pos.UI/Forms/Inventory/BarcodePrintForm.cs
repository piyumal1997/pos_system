using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BarTender;
using pos_system.pos.BLL.Services;
using pos_system.pos.Models;
using BTFormat = BarTender.Format;
using BTApplication = BarTender.Application;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using pos_system.pos.Core;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.UI.Forms.Common;

namespace pos_system.pos.UI.Forms.Inventory
{
    public partial class BarcodePrintForm : Form
    {
        private readonly Item _item;
        private readonly SizeService _sizeService = new SizeService();
        private PictureBox picBarcodePreview;
        private Button btnPrint;
        private ComboBox cboPrinters;
        private ComboBox cboTemplates;
        private DataGridView dgvSizes;
        private volatile bool _isClosing = false;
        private List<TemplateItem> templateItems = new List<TemplateItem>();
        private BindingList<SizePrintInfo> _sizePrintInfos;
        private SizePrintInfo _currentPreviewSize;
        private String _categoryName;

        public BarcodePrintForm(Item item)
        {
            if (item?.Sizes == null || item.Sizes.Count == 0)
                throw new ArgumentException("Item must have size variants");

            _item = item ?? throw new ArgumentNullException(nameof(item));
            InitializeSizePrintInfos();
            InitializeForm();
            LoadTemplates();
            LoadPrinters();
            this.FormClosing += (s, e) => _isClosing = true;
            new DropShadow().ApplyShadows(this);
        }

        private void InitializeSizePrintInfos()
        {
            var list = new List<SizePrintInfo>();
            foreach (var size in _item.Sizes)
            {
                var sizeObj = _sizeService.GetSizeById((int)size.Size_ID);
                list.Add(new SizePrintInfo
                {
                    ProductSize = size,
                    SizeLabel = sizeObj?.SizeLabel ?? "N/A",
                    PrintCount = size.Quantity,
                });
            }
            _sizePrintInfos = new BindingList<SizePrintInfo>(list);
        }

        private void InitializeForm()
        {
            this.Text = "PRINT BARCODE LABEL";
            this.Size = new Size(700, 660);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Padding = new Padding(10);
            this.StartPosition = FormStartPosition.CenterScreen;

            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                BackColor = Color.White,
                Padding = new Padding(0, 0, 0, 10)
            };
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));

            // Title Panel
            var titlePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            var lblTitle = new Label
            {
                Text = "PRINT BARCODE LABEL",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White
            };

            var btnClose = new Button
            {
                Text = "X",
                Dock = DockStyle.Right,
                Width = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => this.Close();

            // Preview Panel
            var previewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            picBarcodePreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Grid Panel
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoScroll = true
            };

            dgvSizes = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            var colSize = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SizeLabel",
                HeaderText = "Size",
                ReadOnly = true,
                Width = 80
            };

            var colStock = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "StockQuantity",
                HeaderText = "Stock",
                ReadOnly = true,
                Width = 80
            };

            var colPrice = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RetailPrice",
                HeaderText = "Price",
                ReadOnly = true,
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            };

            var colPrintQty = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PrintCount",
                HeaderText = "Print Qty",
                Width = 80
            };

            dgvSizes.Columns.AddRange(colSize, colStock, colPrice, colPrintQty);
            dgvSizes.DataSource = _sizePrintInfos;
            dgvSizes.CellEndEdit += DgvSizes_CellEndEdit;

            // SAFE SELECTION: Use HandleCreated to ensure grid is ready
            dgvSizes.HandleCreated += (s, e) =>
            {
                if (!_isClosing && _sizePrintInfos.Count > 0)
                {
                    dgvSizes.ClearSelection();
                    dgvSizes.Rows[0].Selected = true;
                    _currentPreviewSize = _sizePrintInfos[0];
                }
            };

            dgvSizes.SelectionChanged += DgvSizes_SelectionChanged;

            // Ensure grid fits content
            dgvSizes.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvSizes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Controls Panel
            var controlsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 2,
                Padding = new Padding(10)
            };
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34F));

            var lblTemplate = new Label
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
            cboTemplates.SelectedIndexChanged += (s, e) => GenerateBarTenderPreview();

            var lblPrinter = new Label
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

            btnPrint = new Button
            {
                Text = "PRINT LABELS",
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Margin = new Padding(5)
            };
            btnPrint.Click += BtnPrint_Click;

            // Assemble controls
            titlePanel.Controls.Add(btnClose);
            titlePanel.Controls.Add(lblTitle);
            previewPanel.Controls.Add(picBarcodePreview);
            gridPanel.Controls.Add(dgvSizes);

            controlsPanel.Controls.Add(lblTemplate, 0, 0);
            controlsPanel.Controls.Add(cboTemplates, 1, 0);
            controlsPanel.Controls.Add(lblPrinter, 0, 1);
            controlsPanel.Controls.Add(cboPrinters, 1, 1);
            controlsPanel.Controls.Add(btnPrint, 0, 2);
            controlsPanel.SetColumnSpan(btnPrint, 2);

            container.Controls.Add(titlePanel, 0, 0);
            container.Controls.Add(previewPanel, 0, 1);
            container.Controls.Add(gridPanel, 0, 2);
            container.Controls.Add(controlsPanel, 0, 3);

            this.Controls.Add(container);

            // Handle empty size list
            if (_sizePrintInfos.Count == 0)
            {
                btnPrint.Enabled = false;
                ShowEmptyPreview();
            }
        }

        private void DgvSizes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSizes.SelectedRows.Count == 0)
            {
                _currentPreviewSize = null;
                ShowEmptyPreview();
                return;
            }

            var selectedRow = dgvSizes.SelectedRows[0];
            if (selectedRow.DataBoundItem is SizePrintInfo spi)
            {
                _currentPreviewSize = spi;
                GenerateBarTenderPreview();
            }
        }

        // NEW: Show empty preview when no selection
        private void ShowEmptyPreview()
        {
            if (picBarcodePreview.Image != null)
            {
                var oldImg = picBarcodePreview.Image;
                picBarcodePreview.Image = null;
                oldImg.Dispose();
            }

            // Create placeholder image
            Bitmap bmp = new Bitmap(400, 200);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.WhiteSmoke);
                g.DrawString("No size selected",
                    new Font("Arial", 12, FontStyle.Bold),
                    Brushes.DarkRed,
                    new PointF(50, 80));
            }
            picBarcodePreview.Image = bmp;
        }
        private Image CreateEmptyPreviewImage()
        {
            Bitmap bmp = new Bitmap(400, 200);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.WhiteSmoke);
                g.DrawString("No sizes available",
                    new Font("Arial", 12, FontStyle.Bold),
                    Brushes.DarkRed,
                    new PointF(50, 80));
            }
            return bmp;
        }

        private void DgvSizes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.RowIndex >= 0) // Print Count column
            {
                var editedSize = _sizePrintInfos[e.RowIndex];
                if (editedSize == _currentPreviewSize)
                {
                    GenerateBarTenderPreview();
                }
            }
        }

        private void LoadTemplates()
        {
            try
            {
                string templateDir = Path.Combine(System.Windows.Forms.Application.StartupPath, "Barcode");
                if (!Directory.Exists(templateDir))
                {
                    ThemedMessageBox.Show($"Template directory not found: {templateDir}", "Missing Templates", ThemedMessageBoxIcon.Warning);
                    return;
                }

                var templateFiles = Directory.GetFiles(templateDir, "*.btw");
                templateItems.Clear();
                cboTemplates.Items.Clear();

                foreach (string file in templateFiles)
                {
                    string fileName = Path.GetFileName(file);
                    templateItems.Add(new TemplateItem
                    {
                        FileName = fileName,
                        FilePath = file,
                        LabelsPerRow = fileName.Contains("2up") ? 2 :
                                       fileName.Contains("3up") ? 3 :
                                       fileName.Contains("4up") ? 4 : 1
                    });
                }

                templateItems = templateItems.OrderBy(t => t.FileName).ToList();
                foreach (var template in templateItems)
                {
                    cboTemplates.Items.Add(template);
                }

                if (cboTemplates.Items.Count > 0) cboTemplates.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading templates: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPrinters()
        {
            try
            {
                cboPrinters.DataSource = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList();
                if (cboPrinters.Items.Count > 0) cboPrinters.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading printers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateBarTenderPreview()
        {
            if (cboTemplates.SelectedItem == null || _isClosing ||
                _sizePrintInfos.Count == 0 || _currentPreviewSize == null)
                return;

            TemplateItem template = (TemplateItem)cboTemplates.SelectedItem;
            var previewSize = _currentPreviewSize; // Capture for thread safety

            Thread staThread = new Thread(() =>
            {
                if (_isClosing) return;

                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                string outputFile = null;
                BTApplication btApp = null;
                BTFormat btFormat = null;

                try
                {
                    if (_isClosing) return;

                    btApp = new BTApplication();
                    btApp.Visible = false;
                    btFormat = btApp.Formats.Open(template.FilePath, false, "");

                    // Safe data setting
                    SafeSetFormatData(btFormat, _item, previewSize);
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
                                if (!_isClosing && !this.IsDisposed &&
                                    !picBarcodePreview.IsDisposed &&
                                    _currentPreviewSize == previewSize) // Verify still current
                                {
                                    if (picBarcodePreview.Image != null)
                                    {
                                        var oldImg = picBarcodePreview.Image;
                                        picBarcodePreview.Image = null;
                                        oldImg.Dispose();
                                    }
                                    picBarcodePreview.Image = new Bitmap(previewImage);
                                }
                                previewImage.Dispose();
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
                            MessageBox.Show($"Preview Error: {ex.Message}\n\n{ex.StackTrace}",
                                "Preview Generation Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void SafeSetFormatData(BTFormat format, Item item, SizePrintInfo spi)
        {
            //if (item.CategoryName == null) 
            //{
            //    var categoryService = new CategoryService();
            //    _categoryName = categoryService.GetCategoryName(item.Category_ID);
            //}
            //else 
            //{
            //    _categoryName = item.CategoryName;
            //}
            var categoryService = new CategoryService();
            _categoryName = categoryService.GetCategoryName(item.Category_ID);
            try
                {

                    format.SetNamedSubStringValue("Barcode", item.Barcode ?? "");
                    format.SetNamedSubStringValue("Category", _categoryName ?? "");
                    format.SetNamedSubStringValue("Sex", $"({GetGenderCode(item.Gender_ID)})");
                    format.SetNamedSubStringValue("RetailPrice", $"Rs.{spi.RetailPrice:N2}");
                    format.SetNamedSubStringValue("Size", $"({ItemSize(spi.SizeLabel ?? "N/A")})");


                    // Handle cost code safely
                    string costCode = "ERROR";
                    try
                    {
                        if (spi.ProductSize.UnitCost != null)
                        {
                            costCode = ConvertNumberToCode(Convert.ToInt32(spi.ProductSize.UnitCost).ToString());
                        }
                    }
                    catch
                    {
                        costCode = "INVALID";
                    }
                    format.SetNamedSubStringValue("CostCode", costCode);
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
                format.ExportPrintPreviewToImage(
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
            string pattern = Path.GetFileNameWithoutExtension(fileNameTemplate) + "*" +
                             Path.GetExtension(fileNameTemplate);

            try
            {
                var files = Directory.GetFiles(directory, pattern);
                return files.Length > 0 ? files[0] : null;
            }
            catch
            {
                return null;
            }
        }

        private void CleanupBarTender(ref BTFormat format, ref BTApplication app)
        {
            try
            {
                if (format != null)
                {
                    format.Close(BtSaveOptions.btDoNotSaveChanges);
                    Marshal.FinalReleaseComObject(format);
                    format = null;
                }

                if (app != null)
                {
                    app.Quit(BtSaveOptions.btDoNotSaveChanges);
                    Marshal.FinalReleaseComObject(app);
                    app = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Cleanup error: {ex.Message}");
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private string ItemSize(string size)
        {
            return size switch
            {
                "One Size" => "One",  // One Size
                "XXXL" => "3XL",
                "XXXXl" => "4XL",
                "XXXXXl" => "5XL",
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

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (cboTemplates.SelectedItem == null)
            {
                ThemedMessageBox.Show("Please select a template,", "Template Required", ThemedMessageBoxIcon.Warning);
                return;
            }

            if (cboPrinters.SelectedItem == null)
            {
                ThemedMessageBox.Show("Please select a printer", "Printer Required!", ThemedMessageBoxIcon.Warning);
                return;
            }

            TemplateItem template = (TemplateItem)cboTemplates.SelectedItem;
            string printerName = cboPrinters.SelectedItem.ToString();

            bool hasValidPrintCount = _sizePrintInfos.Any(spi => spi.PrintCount > 0);
            if (!hasValidPrintCount)
            {
                ThemedMessageBox.Show("Please set print quantity for at least one size", "Print Quantity Required!", ThemedMessageBoxIcon.Warning);
                return;
            }

            Thread staThread = new Thread(() => PrintWithBarTender(template, printerName));
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.IsBackground = true;
            staThread.Start();
        }

        private void PrintWithBarTender(TemplateItem template, string printerName)
        {
            BTApplication btApp = null;
            BTFormat btFormat = null;

            try
            {
                btApp = new BTApplication();
                btApp.Visible = false;
                btFormat = btApp.Formats.Open(template.FilePath, true, "");
                btFormat.PrintSetup.Printer = printerName;

                foreach (var spi in _sizePrintInfos)
                {
                    if (spi.PrintCount <= 0) continue;

                    SafeSetFormatData(btFormat, _item, spi);
                    btFormat.PrintSetup.IdenticalCopiesOfLabel = spi.PrintCount;
                    btFormat.PrintSetup.NumberSerializedLabels = template.LabelsPerRow;
                    btFormat.PrintOut(false, true);
                }

                int total = _sizePrintInfos.Sum(x => x.PrintCount);
                this.Invoke((MethodInvoker)delegate
                {
                    ThemedMessageBox.Show($"{total} label(s) sent to printer", "Success", ThemedMessageBoxIcon.None);
                    Close();
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ThemedMessageBox.Show($"Print Error: {ex.Message}", "Error", ThemedMessageBoxIcon.Error);
                });
            }
            finally
            {
                CleanupBarTender(ref btFormat, ref btApp);
            }
        }

        private class TemplateItem
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public int LabelsPerRow { get; set; }
            public override string ToString() => $"{FileName} ({LabelsPerRow} per row)";
        }

        public class SizePrintInfo
        {
            public ProductSize ProductSize { get; set; }
            public string SizeLabel { get; set; }
            private int _printCount;
            public int PrintCount
            {
                get => _printCount;
                set => _printCount = value; // Allow assignment
            }
            public string StockQuantity => ProductSize?.Quantity.ToString() ?? "0";
            public decimal RetailPrice => ProductSize?.RetailPrice ?? 0;
        }
    }
}