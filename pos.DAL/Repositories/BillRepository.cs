using System.Data;
using System.Data.SqlClient;
using pos_system.pos.DAL;
using RetailPOS.DAL;
using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Services;
using pos_system.pos.Models;
using System.Data;

namespace RetailPOS.DAL.Repositories
{
    public class BillRepository
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

        //public DataTable SearchBills(int? billId, DateTime startDate, DateTime endDate, string customerContact)
        //{
        //    string query = @"
        //        SELECT 
        //            b.Bill_ID,
        //            b.[date] AS BillDate,
        //            b.PaymentMethod,
        //            b.Discount_Method,
        //            b.Discount AS BillDiscount,
        //            b.CustomerContact,
        //            b.CardLast4,
        //            b.BankAccountLast4,
        //            b.Token_ReturnID,
        //            r.TotalRefund AS TokenValue,
        //            (SELECT SUM(bi.ItemSellingPrice * bi.quantity) 
        //             FROM Bill_Item bi WHERE bi.Bill_ID = b.Bill_ID) AS Subtotal,
        //            (SELECT SUM(bi.Per_item_Discount * bi.quantity) 
        //             FROM Bill_Item bi WHERE bi.Bill_ID = b.Bill_ID) AS TotalPerItemDiscount,
        //            (SELECT SUM((bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity) 
        //             FROM Bill_Item bi WHERE bi.Bill_ID = b.Bill_ID) 
        //                - CASE WHEN b.Discount_Method = 'TotalBill' THEN b.Discount ELSE 0 END
        //                AS NetTotal
        //        FROM Bill b
        //        LEFT JOIN [Return] r ON b.Token_ReturnID = r.Return_ID
        //        WHERE b.BillStatus = 'Completed'
        //            AND (@BillID IS NULL OR b.Bill_ID = @BillID)
        //            AND (b.[date] >= @StartDate AND b.[date] < DATEADD(DAY, 1, @EndDate))
        //            AND (@CustomerContact IS NULL OR b.CustomerContact LIKE '%' + @CustomerContact + '%')";

        //    var parameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@BillID", billId ?? (object)DBNull.Value),
        //        new SqlParameter("@StartDate", startDate),
        //        new SqlParameter("@EndDate", endDate),
        //        new SqlParameter("@CustomerContact", customerContact ?? (object)DBNull.Value)
        //    };

        //    return DbHelper.GetDataTable(query, CommandType.Text, parameters);
        //}

        //public BillSummary GetBillSummary(int billId)
        //{
        //    var summary = new BillSummary
        //    {
        //        Header = GetBillHeader(billId),
        //        Items = GetBillItems(billId)
        //    };

        //    // Calculate totals
        //    CalculateBillTotals(summary);

        //    return summary;
        //}

        //private BillHeader GetBillHeader(int billId)
        //{
        //    string query = @"
        //        SELECT 
        //            b.[date], 
        //            b.PaymentMethod,
        //            b.Discount_Method,
        //            b.Discount,
        //            b.CustomerContact,
        //            b.CardLast4,
        //            b.BankAccountLast4,
        //            b.Token_ReturnID,
        //            r.TotalRefund AS TokenValue,
        //            e.firstName + ' ' + e.lastName AS CashierName
        //        FROM Bill b
        //        LEFT JOIN [Return] r ON b.Token_ReturnID = r.Return_ID
        //        INNER JOIN Employee e ON b.Employee_ID = e.Employee_ID
        //        WHERE b.Bill_ID = @BillID";

        //    var parameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@BillID", billId)
        //    };

        //    var dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);

        //    if (dt.Rows.Count == 0)
        //        return null;

        //    DataRow row = dt.Rows[0];

        //    return new BillHeader
        //    {
        //        BillDate = Convert.ToDateTime(row["date"]),
        //        PaymentMethod = row["PaymentMethod"] as string ?? "Unknown",
        //        DiscountMethod = row["Discount_Method"] as string,
        //        BillDiscount = Convert.ToDecimal(row["Discount"]),
        //        CustomerContact = row["CustomerContact"] as string,
        //        CardLast4 = row["CardLast4"] as string,
        //        BankLast4 = row["BankAccountLast4"] as string,
        //        TokenReturnID = row["Token_ReturnID"] as int?,
        //        TokenValue = row["TokenValue"] as decimal?,
        //        CashierName = row["CashierName"] as string
        //    };
        //}

        //private List<BillItem> GetBillItems(int billId)
        //{
        //    string query = @"
        //        SELECT 
        //            p.description,
        //            bi.ItemSellingPrice,
        //            bi.quantity,
        //            bi.Per_item_Discount,
        //            (bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity AS NetPrice
        //        FROM Bill_Item bi
        //        INNER JOIN ProductSize ps ON bi.ProductSize_ID = ps.ProductSize_ID
        //        INNER JOIN Product p ON ps.Product_ID = p.Product_ID
        //        WHERE bi.Bill_ID = @BillID";

        //    var parameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@BillID", billId)
        //    };

        //    var dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);
        //    var items = new List<BillItem>();

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        items.Add(new BillItem
        //        {
        //            Description = row["description"].ToString(),
        //            ItemSellingPrice = Convert.ToDecimal(row["ItemSellingPrice"]),
        //            Quantity = Convert.ToInt32(row["quantity"]),
        //            PerItemDiscount = Convert.ToDecimal(row["Per_item_Discount"]),
        //            NetPrice = Convert.ToDecimal(row["NetPrice"])
        //        });
        //    }

        //    return items;
        //}

        //private void CalculateBillTotals(BillSummary summary)
        //{
        //    if (summary.Items == null || summary.Header == null)
        //        return;

        //    // Calculate subtotal (sum of all items' selling price * quantity)
        //    summary.Subtotal = summary.Items.Sum(item => item.ItemSellingPrice * item.Quantity);

        //    // Calculate total per item discount
        //    summary.TotalPerItemDiscount = summary.Items.Sum(item => item.PerItemDiscount * item.Quantity);

        //    // Calculate bill discount (only if discount method is TotalBill)
        //    summary.TotalBillDiscount = summary.Header.DiscountMethod == "TotalBill" ?
        //        summary.Header.BillDiscount : 0;

        //    // Calculate final total
        //    summary.Total = summary.Subtotal - summary.TotalPerItemDiscount - summary.TotalBillDiscount;
        //}

        //public DataTable GetBillItemsDataTable(int billId)
        //{
        //    string query = @"
        //        SELECT 
        //            p.description,
        //            bi.ItemSellingPrice,
        //            bi.quantity,
        //            bi.Per_item_Discount,
        //            (bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity AS NetPrice
        //        FROM Bill_Item bi
        //        INNER JOIN ProductSize ps ON bi.ProductSize_ID = ps.ProductSize_ID
        //        INNER JOIN Product p ON ps.Product_ID = p.Product_ID
        //        WHERE bi.Bill_ID = @BillID";

        //    var parameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@BillID", billId)
        //    };

        //    return DbHelper.GetDataTable(query, CommandType.Text, parameters);
        //}

        public DataTable SearchBills(int? billId, DateTime startDate, DateTime endDate, string customerContact)
        {
            string query = @"
                    SELECT 
                        b.Bill_ID,
                        b.[date] AS BillDate,
                        b.Discount_Method,
                        b.Discount AS BillDiscount,
                        b.CustomerContact,
                        b.Token_ReturnID,
                        b.TotalAmount,
                        -- Payment method information
                        CASE 
                            WHEN vw.PaymentCount > 1 THEN 'Mixed Payment'
                            WHEN vw.PaymentCount = 1 THEN 
                                COALESCE(
                                    (SELECT TOP 1 PaymentMethod FROM BillPayment WHERE Bill_ID = b.Bill_ID ORDER BY PaymentOrder),
                                    'Cash' -- Default fallback
                                )
                            ELSE 'Cash' -- Default for bills without payment records
                        END AS PaymentMethod,
                        -- Payment summary from view
                        vw.PaymentSummary,
                        vw.PaymentCount,
                        e.firstName + ' ' + e.lastName AS CashierName,
                        -- Calculate net total
                        CASE 
                            WHEN b.Discount_Method = 'PerItem' THEN 
                                (SELECT SUM((bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity) 
                                 FROM Bill_Item bi WHERE bi.Bill_ID = b.Bill_ID)
                            WHEN b.Discount_Method = 'TotalBill' THEN 
                                (SELECT SUM(bi.ItemSellingPrice * bi.quantity) 
                                 FROM Bill_Item bi WHERE bi.Bill_ID = b.Bill_ID) - b.Discount
                            ELSE 
                                (SELECT SUM(bi.ItemSellingPrice * bi.quantity) 
                                 FROM Bill_Item bi WHERE bi.Bill_ID = b.Bill_ID)
                        END AS NetTotal,
                        -- Additional payment details for mixed payments
                        (SELECT COUNT(*) FROM BillPayment bp WHERE bp.Bill_ID = b.Bill_ID) AS ActualPaymentCount
                    FROM Bill b
                    LEFT JOIN [Return] r ON b.Token_ReturnID = r.Return_ID
                    LEFT JOIN vw_BillWithPayments vw ON b.Bill_ID = vw.Bill_ID
                    INNER JOIN Employee e ON b.Employee_ID = e.Employee_ID
                    WHERE b.BillStatus = 'Completed'
                        AND (@BillID IS NULL OR b.Bill_ID = @BillID)
                        AND (b.[date] >= @StartDate AND b.[date] < DATEADD(DAY, 1, @EndDate))
                        AND (@CustomerContact IS NULL OR b.CustomerContact LIKE '%' + @CustomerContact + '%')";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BillID", billId ?? (object)DBNull.Value),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate),
                new SqlParameter("@CustomerContact", customerContact ?? (object)DBNull.Value)
            };

            return DbHelper.GetDataTable(query, CommandType.Text, parameters);
        }

        public BillSummary GetBillSummary(int billId)
        {
            var summary = new BillSummary
            {
                Header = GetBillHeader(billId),
                Items = GetBillItems(billId)
            };

            // Calculate totals
            CalculateBillTotals(summary);

            return summary;
        }

        private BillHeader GetBillHeader(int billId)
        {
            string query = @"
                SELECT 
                    b.[date], 
                    b.Discount_Method,
                    b.Discount,
                    b.CustomerContact,
                    b.Token_ReturnID,
                    b.TotalAmount,
                    r.TotalRefund AS TokenValue,
                    e.firstName + ' ' + e.lastName AS CashierName,
                    vw.PaymentSummary,
                    vw.PaymentCount
                FROM Bill b
                LEFT JOIN [Return] r ON b.Token_ReturnID = r.Return_ID
                LEFT JOIN vw_BillWithPayments vw ON b.Bill_ID = vw.Bill_ID
                INNER JOIN Employee e ON b.Employee_ID = e.Employee_ID
                WHERE b.Bill_ID = @BillID";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BillID", billId)
            };

            var dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            var header = new BillHeader
            {
                BillDate = Convert.ToDateTime(row["date"]),
                DiscountMethod = row["Discount_Method"] as string,
                BillDiscount = Convert.ToDecimal(row["Discount"]),
                CustomerContact = row["CustomerContact"] as string,
                TokenReturnID = row["Token_ReturnID"] as int?,
                TokenValue = row["TokenValue"] as decimal?,
                CashierName = row["CashierName"] as string,
                TotalAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0,
                PaymentSummary = row["PaymentSummary"] as string,
                PaymentCount = row["PaymentCount"] != DBNull.Value ? Convert.ToInt32(row["PaymentCount"]) : 1
            };

            // Load detailed payment information
            header.Payments = GetBillPayments(billId);

            // Set legacy properties for backward compatibility
            if (header.Payments.Count > 0)
            {
                var firstPayment = header.Payments[0];
                header.PaymentMethod = firstPayment.PaymentMethod;
                header.CardLast4 = firstPayment.CardLast4;
                header.BankLast4 = firstPayment.BankAccountLast4;
            }

            return header;
        }

        private List<BillPaymentDetail> GetBillPayments(int billId)
        {
            string query = @"
                SELECT 
                    PaymentMethod,
                    PaymentAmount,
                    CardLast4,
                    BankAccountLast4,
                    PaymentOrder
                FROM BillPayment
                WHERE Bill_ID = @BillID
                ORDER BY PaymentOrder";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BillID", billId)
            };

            var dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);
            var payments = new List<BillPaymentDetail>();

            foreach (DataRow row in dt.Rows)
            {
                payments.Add(new BillPaymentDetail
                {
                    PaymentMethod = row["PaymentMethod"].ToString(),
                    PaymentAmount = Convert.ToDecimal(row["PaymentAmount"]),
                    CardLast4 = row["CardLast4"] as string,
                    BankAccountLast4 = row["BankAccountLast4"] as string,
                    PaymentOrder = Convert.ToInt32(row["PaymentOrder"])
                });
            }

            return payments;
        }

        private List<BillItem> GetBillItems(int billId)
        {
            string query = @"
                SELECT 
                    p.description,
                    bi.ItemSellingPrice,
                    bi.quantity,
                    bi.Per_item_Discount,
                    (bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity AS NetPrice
                FROM Bill_Item bi
                INNER JOIN ProductSize ps ON bi.ProductSize_ID = ps.ProductSize_ID
                INNER JOIN Product p ON ps.Product_ID = p.Product_ID
                WHERE bi.Bill_ID = @BillID";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BillID", billId)
            };

            var dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);
            var items = new List<BillItem>();

            foreach (DataRow row in dt.Rows)
            {
                items.Add(new BillItem
                {
                    Description = row["description"].ToString(),
                    ItemSellingPrice = Convert.ToDecimal(row["ItemSellingPrice"]),
                    Quantity = Convert.ToInt32(row["quantity"]),
                    PerItemDiscount = Convert.ToDecimal(row["Per_item_Discount"]),
                    NetPrice = Convert.ToDecimal(row["NetPrice"])
                });
            }

            return items;
        }

        private void CalculateBillTotals(BillSummary summary)
        {
            if (summary.Items == null || summary.Header == null)
                return;

            // Calculate subtotal (sum of all items' selling price * quantity)
            summary.Subtotal = summary.Items.Sum(item => item.ItemSellingPrice * item.Quantity);

            // Calculate total per item discount
            summary.TotalPerItemDiscount = summary.Items.Sum(item => item.PerItemDiscount * item.Quantity);

            // Calculate bill discount (only if discount method is TotalBill)
            summary.TotalBillDiscount = summary.Header.DiscountMethod == "TotalBill" ?
                summary.Header.BillDiscount : 0;

            // Use TotalAmount from header if available, otherwise calculate
            summary.Total = summary.Header.TotalAmount > 0 ?
                summary.Header.TotalAmount :
                summary.Subtotal - summary.TotalPerItemDiscount - summary.TotalBillDiscount;
        }

        public DataTable GetBillItemsDataTable(int billId)
        {
            string query = @"
                SELECT 
                    p.description,
                    bi.ItemSellingPrice,
                    bi.quantity,
                    bi.Per_item_Discount,
                    (bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity AS NetPrice
                FROM Bill_Item bi
                INNER JOIN ProductSize ps ON bi.ProductSize_ID = ps.ProductSize_ID
                INNER JOIN Product p ON ps.Product_ID = p.Product_ID
                WHERE bi.Bill_ID = @BillID";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BillID", billId)
            };

            return DbHelper.GetDataTable(query, CommandType.Text, parameters);
        }

    }
}
