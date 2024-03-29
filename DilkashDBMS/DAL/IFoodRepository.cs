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
    }
}
