using pos_system.pos.Models;
using System.Collections.Generic;

namespace pos_system.pos.DAL.Repositories
{
    public interface ISalesReportRepository
    {
        SalesReport GetSalesReport(SalesFilter filter);
        List<Brand> GetBrands();
        List<Category> GetCategories();
    }
}