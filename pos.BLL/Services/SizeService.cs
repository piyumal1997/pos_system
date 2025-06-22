using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System;
using System.Linq;

namespace pos_system.pos.BLL.Services
{
    internal class SizeService
    {
        private readonly SizeRepository _repository = new SizeRepository();

        public List<CategorySize> GetAllSizes() => _repository.GetAll();

        public List<CategorySize> GetSizesByCategoryId(int categoryId)
        {
            return _repository.GetSizesByCategory(categoryId);
        }
    }
}
