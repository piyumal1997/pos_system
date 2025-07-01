using Microsoft.Data.SqlClient;
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

        public List<Sizes> GetAllSize() => _repository.GetAlls();

        public List<CategorySize> GetSizesByCategoryId(int categoryId)
        {
            return _repository.GetSizesByCategory(categoryId);
        }

        public CategorySize GetSizeById(int sizeId)
        {
            return _repository.GetSizeById(sizeId);
        }

        public bool DeleteSize(int id) => _repository.Delete(id);

        public bool SaveSize(Sizes size)
        {
            try
            {
                if (size.Size_ID == 0)
                    return _repository.Insert(size);
                else
                    return _repository.Update(size);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation
                {
                    MessageBox.Show("Size label must be unique");
                    return false;
                }
                throw;
            }
        }
    }
}
