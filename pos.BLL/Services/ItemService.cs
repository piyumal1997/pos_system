using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System;
using System.Linq;

namespace pos_system.pos.BLL.Services
{
    internal class ItemService
    {
        private readonly ItemRepository _repository = new ItemRepository();
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly GenderService _genderService = new GenderService();

        public bool AddItem(Item item)
        {
            if (!new BrandService().BrandExists(item.Brand_ID))
                throw new ArgumentException($"Brand with ID {item.Brand_ID} does not exist");

            if (!_categoryService.CategoryExists(item.Category_ID))
                throw new ArgumentException($"Category with ID {item.Category_ID} does not exist");

            if (!_genderService.GenderExists(item.Gender_ID))
                throw new ArgumentException($"Gender with ID {item.Gender_ID} does not exist");

            return _repository.AddItem(item);
        }

        public bool UpdateItem(Item item)
        {
            if (!new BrandService().BrandExists(item.Brand_ID))
                throw new ArgumentException($"Brand with ID {item.Brand_ID} does not exist");

            if (!_categoryService.CategoryExists(item.Category_ID))
                throw new ArgumentException($"Category with ID {item.Category_ID} does not exist");

            if (!_genderService.GenderExists(item.Gender_ID))
                throw new ArgumentException($"Gender with ID {item.Gender_ID} does not exist");

            return _repository.UpdateItem(item);
        }

        public bool DeleteItem(int productId)
        {
            return _repository.DeleteItem(productId);
        }

        public Item GetItemById(int productId)
        {
            return _repository.GetItem(productId);
        }

        public List<Item> GetAllItems()
        {
            return _repository.GetAllItems();
        }

        public List<Item> SearchItems(string searchTerm, int brandId, int categoryId)
        {
            return _repository.SearchItems(searchTerm, brandId, categoryId);
        }

        public string GenerateBarcode()
        {
            return _repository.GenerateUniqueBarcode();
        }

        public List<Item> SearchItemsWithVariants(string searchTerm, int brandId, int categoryId)
        {
            return _repository.SearchItemsWithVariants(searchTerm, brandId, categoryId);
        }

        public List<Item> SearchItemsWithFilters(
            string searchTerm,
            int brandId,
            int categoryId,
            int sizeId,
            int genderId,
            decimal minPrice,
            decimal maxPrice)
        {
            return _repository.SearchItemsWithFilters(
                searchTerm,
                brandId,
                categoryId,
                sizeId,
                genderId,
                minPrice,
                maxPrice
            );
        }
    }
}
