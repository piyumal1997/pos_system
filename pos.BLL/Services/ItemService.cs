using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;

namespace pos_system.pos.BLL.Services
{
    internal class ItemService
    {
        private readonly ItemRepository _repository = new ItemRepository();
        private readonly CategoryService _categoryService = new CategoryService();

        public List<Item> SearchItems(string searchTerm, int brandId, int categoryId)
        {
            return _repository.SearchItems(searchTerm, brandId, categoryId);
        }

        public IEnumerable<Item> GetAllItems() => _repository.GetAllItems();

        public bool AddItem(Item item)
        {
            // Validate brand and category existence
            if (!new BrandService().BrandExists(item.Brand_ID))
                throw new ArgumentException($"Brand with ID {item.Brand_ID} does not exist");

            if (!_categoryService.CategoryExists(item.Category_ID))
                throw new ArgumentException($"Category with ID {item.Category_ID} does not exist");

            // Validate size if provided
            if (item.Size_ID.HasValue && !IsValidSizeForCategory(item.Category_ID, item.Size_ID.Value))
                return false;

            if (_repository.CheckItemExists(item.description, item.barcode))
                return false;

            return _repository.AddItem(item);
        }

        public bool UpdateItem(Item item)
        {
            // Validate brand and category existence
            if (!new BrandService().BrandExists(item.Brand_ID))
                throw new ArgumentException($"Brand with ID {item.Brand_ID} does not exist");

            if (!_categoryService.CategoryExists(item.Category_ID))
                throw new ArgumentException($"Category with ID {item.Category_ID} does not exist");

            // Validate size if provided
            if (item.Size_ID.HasValue && !IsValidSizeForCategory(item.Category_ID, item.Size_ID.Value))
                return false;

            if (_repository.CheckItemExists(item.description, item.barcode, item.Item_ID))
                return false;

            return _repository.UpdateItem(item);
        }

        public bool DeleteItem(int itemId) => _repository.DeleteItem(itemId);

        public string GenerateBarcode() => _repository.GenerateUniqueBarcode();

        private bool IsValidSizeForCategory(int categoryId, int sizeId)
        {
            var sizeService = new SizeService();
            var validSizes = sizeService.GetSizesByCategoryId(categoryId);
            return validSizes.Any(s => s.Size_ID == sizeId);
        }
    }
}
