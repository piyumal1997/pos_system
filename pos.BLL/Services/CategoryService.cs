using Microsoft.Data.SqlClient;
using pos_system.pos.DAL;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System;
using System.Data;
using System.Linq;

namespace pos_system.pos.BLL.Services
{
    internal class CategoryService
    {
        private readonly CategoryRepository _repository = new CategoryRepository();

        public List<Category> GetAllCategorie()
        {
            return _repository.GetAllCategorie();
        }

        public DataTable GetAllCategories() => _repository.GetAll();

        public bool AddCategory(string name) =>
            !string.IsNullOrWhiteSpace(name) && _repository.Add(name);

        public bool UpdateCategory(int id, string name) =>
            id > 0 && !string.IsNullOrWhiteSpace(name) && _repository.Update(id, name);

        public bool DeleteCategory(int id) => id > 0 && _repository.Delete(id);

        public bool CheckCategoryExists(string name, int? id = null) =>
            _repository.Exists(name, id);

        public List<CategorySize> GetSizesByCategoryId(int categoryId) =>
            _repository.GetSizesByCategory(categoryId);

        public bool UpdateCategorySizes(int categoryId, List<int> sizeIds) =>
            _repository.UpdateCategorySizes(categoryId, sizeIds);

        public int GetCategoryIdByName(string name)
        {
            string query = "SELECT Category_ID FROM Category WHERE categoryName = @name";
            var parameters = new SqlParameter[] { new SqlParameter("@name", name) };
            var result = DbHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }
        public bool CategoryExists(int categoryId)
        {
            return _repository.CategoryExists(categoryId);
        }

        public string GetCategoryName(int categoryId)
        {
            if (categoryId <= 0) return string.Empty;
            return _repository.GetCategoryNameById(categoryId);
        }
    }
}
