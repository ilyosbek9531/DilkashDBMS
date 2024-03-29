namespace DilkashDBMS.DAL.Models
{
    public class FoodViewModel
    {
        public int? FoodId { get; set; }
        public required string FoodName { get; set; }
        public string? FoodDescription { get; set; }
        public byte[]? FoodImage { get; set; }
        public required string FoodType { get; set; }
        public required bool Availability { get; set; }
        public required int Price { get; set; }
        public DateTime? CreatedAt { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
