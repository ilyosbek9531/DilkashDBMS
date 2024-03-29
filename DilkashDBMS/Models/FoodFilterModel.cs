using DilkashDBMS.DAL.Models;

namespace DilkashDBMS.Models
{
    public class FoodFilterModel
    {
        public int TotalCount { get; set; }
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((decimal)TotalCount / PageSize);
            }
        }
        public string? FoodName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? SortColumn { get; set; }
        public bool SortDesc { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public IEnumerable<Food> Foods { get; set; }
    }
}
