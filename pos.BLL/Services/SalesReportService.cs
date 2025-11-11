using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;

namespace RetailPOS.BLL.Services
{
    public class SalesReportService
    {
        private readonly ISalesReportRepository _salesRepository;

        public SalesReportService(ISalesReportRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public SalesReport GetSalesReport(SalesFilter filter)
        {
            return _salesRepository.GetSalesReport(filter);
        }

        public List<Brand> GetBrands()
        {
            return _salesRepository.GetBrands();
        }

        public List<Category> GetCategories()
        {
            return _salesRepository.GetCategories();
        }
    }
}