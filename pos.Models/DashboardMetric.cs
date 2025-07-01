using System;
using System.Linq;

namespace pos_system.pos.Models
{
    public class DashboardMetrics
    {
        public int TotalItems { get; set; }
        public int ActiveEmployees { get; set; }
        public int TotalBills { get; set; }
        public int TotalReturns { get; set; }
        public int TotalCategories { get; set; }
        public int TotalBrands { get; set; }
        public decimal TodaysSales { get; set; }
        public decimal TodaysCOGS { get; set; }
        public int TodaysQuantity { get; set; }
        public List<SalesData> DailySales { get; set; }
        public List<SalesData> MonthlySales { get; set; }

        // Cashier-specific metrics
        public decimal DailySalesAll { get; set; }
        public decimal DailyCashIncome { get; set; }
        public int DailyQuantitySold { get; set; }
        public int DailyItemsSold { get; set; }
        public decimal DailyBankPayments { get; set; }
        public decimal DailyCashPayments { get; set; }
        public decimal DailyCardPayments { get; set; }
        public decimal DailyReturnAmount { get; set; }
        public int DailyReturnQuantity { get; set; }
    }
}
