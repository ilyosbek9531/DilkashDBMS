using DilkashDBMS.DAL.Models;

namespace DilkashDBMS.DAL
{
    public interface IFoodRepository
    {
        IAsyncEnumerable<Food> GetAllAsync();
        IList<Food> GetAll();
        Food? GetById(int id);
        int Insert(Food food);
        void Update(Food food);
        void Delete(int id);

        IEnumerable<Food> Filter(
            out int totalCount,
            string? foodName = null,
            string? foodType = null,
            DateTime? createdAt = null,
            string? sortColumn = nameof(Food.FoodId),
            bool sortDesc = false,
            int pageNumber = 1,
            int pageSize = 4
            );
    }
}
