using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;

namespace pos_system.pos.BLL.Services
{
    internal class BrandService
    {
        private readonly BrandRepository _repository = new BrandRepository();
        public List<Brand> GetAllBrand()
        {
            return _repository.GetAllBrand();
        }

        public List<Brand> GetAllBrands()
        {
            return _repository.GetAllBrand();
        }
        //public DataTable GetAllBrands() => _repository.GetAllBrand();

        public bool AddBrand(string name) =>
            !string.IsNullOrWhiteSpace(name) && _repository.Add(name);

        public bool UpdateBrand(int id, string name) =>
            id > 0 && !string.IsNullOrWhiteSpace(name) && _repository.Update(id, name);

        public bool DeleteBrand(int id) => id > 0 && _repository.Delete(id);

        public bool CheckBrandExists(string name, int? id = null) =>
            _repository.Exists(name, id);

        public bool BrandExists(int brandId)
        {
            return _repository.BrandExists(brandId);
        }
    }
}
