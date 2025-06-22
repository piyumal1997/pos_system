using Neodynamic.SDK.Barcode;
using pos_system.pos.BLL.Services;
using pos_system.pos.Core;
using pos_system.pos.Models;
using System;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Linq;

namespace pos_system.pos.UI.Forms.Inventory
{
    public partial class BarcodePrintForm : Form
    {
        private readonly Item _item;
        private PictureBox picBarcodePreview;
        private Button btnPrint;
        private ComboBox cboPrinters;
        private NumericUpDown nudPrintCount;
        private const int LABEL_WIDTH_MM = 30;
        private const int LABEL_HEIGHT_MM = 20;

        public BarcodePrintForm(Item item)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));

            var itemService = new ItemService();
            _item = itemService.GetItemById(item.Item_ID) ?? item;

            InitializeForm();
            LoadPrinters();
            GenerateBarcodePreview();
            new DropShadow().ApplyShadows(this);
        }

        private void InitializeForm()
        {
            // Form setup
            this.Text = "PRINT BARCODE LABEL";
            this.Size = new Size(600, 650);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Padding = new Padding(20);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main container
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // Title panel
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
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

            // Close button
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

            // Preview section
            var previewPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 350,
                Padding = new Padding(20)
            };

            picBarcodePreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Controls section
            var controlsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 2,
                Padding = new Padding(20, 20, 20, 10)
            };
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));

            // Printer selection
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
                Value = _item.quantity > 0 ? _item.quantity : 1,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            // Print button
            btnPrint = new Button
            {
                Text = "PRINT LABELS",
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Height = 20
            };
            btnPrint.Click += BtnPrint_Click;

            // Layout controls
            controlsPanel.Controls.Add(lblPrinter, 0, 0);
            controlsPanel.Controls.Add(cboPrinters, 1, 0);
            controlsPanel.Controls.Add(lblPrintCount, 0, 1);
            controlsPanel.Controls.Add(nudPrintCount, 1, 1);
            controlsPanel.Controls.Add(btnPrint, 0, 2);
            controlsPanel.SetColumnSpan(btnPrint, 2);

            // Build panels
            previewPanel.Controls.Add(picBarcodePreview);
            titlePanel.Controls.Add(btnClose);
            titlePanel.Controls.Add(lblTitle);

            container.Controls.Add(controlsPanel);
            container.Controls.Add(previewPanel);
            container.Controls.Add(titlePanel);

            this.Controls.Add(container);
        }

        private void LoadPrinters()
        {
            try
            {
                cboPrinters.DataSource = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
                if (cboPrinters.Items.Count > 0)
                    cboPrinters.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading printers: {ex.Message}", "Printer Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateBarcodePreview()
        {
            try
            {
                Bitmap labelImage = GenerateBarcodeLabel(_item);
                picBarcodePreview.Image = labelImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating barcode: {ex.Message}", "Preview Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Bitmap GenerateBarcodeLabel(Item item)
        {
            // Convert mm to pixels at 300 DPI
            int widthPixels = (int)(LABEL_WIDTH_MM * 300 / 25.4);
            int heightPixels = (int)(LABEL_HEIGHT_MM * 300 / 25.4);

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

                float margin = 25f;

                // 1. Top Section: Style NewAge
                RectangleF titleRect = new RectangleF(0, 5, widthPixels, 40);
                g.DrawString("Style NewAge", titleFont, Brushes.Black, titleRect, centerFormat);

                // 2. Bottom Section: Brand + Category (Size)
                string sizePart = !string.IsNullOrEmpty(item.SizeLabel) ? $" ({item.SizeLabel})" : string.Empty;
                string bottomText = $"{item.BrandName}  {item.CategoryName}{sizePart}";
                RectangleF bottomRect = new RectangleF(0, heightPixels - 35, widthPixels, 30);
                g.DrawString(bottomText, bottomFont, Brushes.Black, bottomRect, centerFormat);

                // 3. Retail Price (Left side, rotated)
                g.TranslateTransform(margin + 15, heightPixels / 2f);
                g.RotateTransform(-90);
                g.DrawString($"Rs.{item.RetailPrice}", verticalFont, Brushes.Black, new RectangleF(-heightPixels / 2f, -15, heightPixels, 30), centerFormat);
                g.ResetTransform();

                // 4. Unit Cost (Right side, rotated)
                g.TranslateTransform(widthPixels - margin - 15, heightPixels / 2f);
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

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (cboPrinters.SelectedItem == null) return;

            try
            {
                int printCount = (int)nudPrintCount.Value;
                Bitmap labelImage = (Bitmap)picBarcodePreview.Image;

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
}