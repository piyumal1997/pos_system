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
    }
}