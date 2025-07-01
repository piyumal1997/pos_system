using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;

namespace RetailPOS.BLL.Services
{
    public class SalesService
    {
        private readonly ISalesRepository _salesRepository;

        public SalesService(ISalesRepository salesRepository)
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