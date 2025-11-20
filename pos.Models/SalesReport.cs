using pos_system.pos.Models;

namespace pos_system.pos.Models
{
    public class SalesReport
    {
        public AccountingSummary AccountingSummary { get; set; }
        public CashFlowSummary CashFlowSummary { get; set; }
        public TokenActivity TokenActivity { get; set; }
        public List<SalesItemDetail> SalesItems { get; set; }
        public List<ReturnItemDetail> ReturnItems { get; set; }
        public List<BillSummary> BillSummaries { get; set; }
    }

    public class AccountingSummary
    {
        public decimal GrossSales { get; set; }
        public decimal Discounts { get; set; }
        public decimal Returns { get; set; }
        public decimal NetSales { get; set; }
        public decimal GrossCOGS { get; set; }
        public decimal ReturnsCOGS { get; set; }
        public decimal NetCOGS { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public int GrossItemsSold { get; set; }
        public int ReturnedItems { get; set; }
        public int NetItemsSold { get; set; }
        public int BillCount { get; set; }
        public decimal ActualCost { get; set; }
        public decimal ActualProfit { get; set; }
        public decimal ActualSales { get; set; }

        // NEW: Payment breakdown from accounting summary
        public decimal CashSales { get; set; }
        public decimal CardSales { get; set; }
        public decimal BankSales { get; set; }
        public decimal MixedSales { get; set; }
    }

    public class CashFlowSummary
    {
        public decimal CashInflow { get; set; }
        public decimal CashOutflow { get; set; }
        public decimal NetCashFlow { get; set; }
        public decimal CashSales { get; set; }
        public decimal CardSales { get; set; }
        public decimal BankSales { get; set; }
        public decimal MixedSales { get; set; } // NEW: Added for mixed payments
        public decimal TokenRedemptions { get; set; }
    }

    public class TokenActivity
    {
        public int TokensIssued { get; set; }
        public decimal TokenValueIssued { get; set; }
        public int TokensUsed { get; set; }
        public decimal TokenValueUsed { get; set; }
        public int TokensOutstanding { get; set; }
        public decimal TokenValueOutstanding { get; set; }
    }

    //public class BillSummary
    //{
    //    public int Bill_ID { get; set; }
    //    public string PaymentMethod { get; set; }
    //    public int Employee_ID { get; set; }
    //    public string Discount_Method { get; set; }
    //    public string CustomerContact { get; set; }
    //    public decimal? Token_Value { get; set; }
    //    public DateTime SaleDate { get; set; }
    //    public decimal GrossAmount { get; set; }
    //    public decimal NetAmount { get; set; }
    //    public decimal CashPayment { get; set; }
    //}
}