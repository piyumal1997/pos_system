using System.Data;
using System.Data.SqlClient;
using pos_system.pos.DAL;
using RetailPOS.DAL;
using Microsoft.Data.SqlClient;

namespace RetailPOS.DAL.Repositories
{
    public static class BillRepository
    {
        public static DataTable SearchBills(int? billId, string contact)
        {
            try
            {
                string query = @"
                       SELECT
                            b.Bill_ID,
                            b.CustomerContact,
                            br.brandName,
                            c.categoryName,
                            s.SizeLabel,
                            ps.RetailPrice AS BaseRetailPrice,
                            (bi.ItemSellingPrice - bi.Per_item_Discount) AS ActualSellingPrice,
                            p.ItemImage AS ImageFilename
                        FROM Bill b
                            INNER JOIN Bill_Item bi ON b.Bill_ID = bi.Bill_ID
                            INNER JOIN ProductSize ps ON bi.ProductSize_ID = ps.ProductSize_ID
                            INNER JOIN Product p ON ps.Product_ID = p.Product_ID
                            LEFT JOIN Brand br ON p.Brand_ID = br.Brand_ID
                            LEFT JOIN Category c ON p.Category_ID = c.Category_ID
                        LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
                        WHERE 
                            (@BillID IS NULL OR b.Bill_ID = @BillID)
                            AND (@Contact IS NULL OR b.CustomerContact LIKE '%' + @Contact + '%')";

                    SqlParameter[] parameters = {
                    new SqlParameter("@BillID", billId.HasValue ? (object)billId.Value : DBNull.Value),
                    new SqlParameter("@Contact", string.IsNullOrEmpty(contact) ? DBNull.Value : (object)contact)
                };

                    return DbHelper.GetDataTable(query, CommandType.Text, parameters);
                }
            catch (Exception ex)
            {
                // Log detailed error
                File.WriteAllText("bill_search_errors.log", $"{DateTime.Now}: {ex}\n\n");
                return new DataTable(); // Return empty table
            }
            
        }
    }
}
