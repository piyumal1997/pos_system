using System.Data;

namespace pos_system.pos.Models
{
    public class SalesReport
    {
        public decimal TotalSales { get; set; }
        public decimal TotalCost { get; set; }
        public int TotalItemsSold { get; set; }
        public int BillCount { get; set; }
        public decimal CashSales { get; set; }
        public decimal CardSales { get; set; }
        public decimal BankTransferSales { get; set; }
        public int ReturnCount { get; set; }
        public int ReturnItemCount { get; set; }
        public decimal ReturnTotalValue { get; set; }
        public List<SalesItemDetail> SalesItems { get; set; }
        public List<ReturnItemDetail> ReturnItems { get; set; }
    }
}