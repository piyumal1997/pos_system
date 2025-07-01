using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pos_system.pos.UI.Forms.Sales
{
    public partial class SizeSelectionForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedProductSizeId { get; private set; } = -1;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedQuantity { get; private set; } = 1;

        public SizeSelectionForm(DataTable sizes)
        {
            InitializeComponent();
            dgvSizes.DataSource = sizes;
            ConfigureDataGridView();
        }

        private void ConfigureDataGridView()
        {
            dgvSizes.AutoGenerateColumns = false;
            dgvSizes.Columns.Clear();

            dgvSizes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SizeLabel",
                HeaderText = "Size",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvSizes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RetailPrice",
                HeaderText = "Price",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dgvSizes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "nitCost",
                HeaderText = "Code",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dgvSizes.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AvailableStock",
                HeaderText = "Stock",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            // Hidden columns for IDs
            var idColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductSize_ID",
                Name = "ProductSize_ID",
                HeaderText = "ID",
                Visible = false
            };
            dgvSizes.Columns.Add(idColumn);

            dgvSizes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvSizes.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvSizes.RowHeadersVisible = false;
            dgvSizes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSizes.ReadOnly = true;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvSizes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a size", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedProductSizeId = (int)dgvSizes.SelectedRows[0].Cells["ProductSize_ID"].Value;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void dgvSizes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSelect.PerformClick();
            }
        }

        private void dgvSizes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgvSizes.SelectedRows.Count > 0)
            {
                btnSelect.PerformClick();
                e.Handled = true;
            }
        }
    }
}
