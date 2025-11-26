using pos_system.pos.Models;
using pos_system.pos.DAL.Repositories;


namespace pos_system.pos.BLL.Services
{
    public class DashboardService
    {
        private readonly DashboardRepository _repository = new DashboardRepository();

        public DashboardMetrics GetDashboardMetrics()
        {
            return _repository.GetDashboardMetrics();
        }

        public DashboardMetrics GetCashierMetrics(int employeeId)
        {
            return _repository.GetCashierMetrics(employeeId, DateTime.Today);
        }

        // NEW: Method to get payment breakdown
        public Dictionary<string, decimal> GetPaymentMethodBreakdown(int employeeId, DateTime date)
        {
            var metrics = _repository.GetCashierMetrics(employeeId, date);

            return new Dictionary<string, decimal>
            {
                { "Cash", metrics.DailyCashPayments },
                { "Card", metrics.DailyCardPayments },
                { "Bank Transfer", metrics.DailyBankPayments },
                { "Mixed", metrics.DailyMixedPayments },
                { "Token", metrics.DailyTokenPayment }
            };
        }
    }
}