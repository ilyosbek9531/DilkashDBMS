using DilkashDBMS.DAL;
using DilkashDBMS.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace DilkashDBMS.Controllers
{
    public class FoodController : Controller
    {
        private readonly IFoodRepository _foodRepository;

        public FoodController(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        public IActionResult Index()
        {
            var list = _foodRepository.GetAll();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            return View(_foodRepository.GetById(id));
        }


        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(FoodViewModel food)
        {
            try
            {
                Food foodObj = new Food
                {
                    FoodName = food.FoodName,
                    FoodDescription = food.FoodDescription,
                    FoodType = food.FoodType,
                    Availability = food.Availability,
                    Price = food.Price,
                    CreatedAt = food.CreatedAt,
                };

                if (food.ImageFile != null)
                {
                    using (var memory = new MemoryStream())
                    {
                        food.ImageFile.CopyTo(memory);
                        foodObj.FoodImage = memory.ToArray();
                    }
                }
                var id = _foodRepository.Insert(foodObj);
                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(food);
            }
        }
        public IActionResult Delete(int id)
        {
            _foodRepository.Delete(id);
            return RedirectToAction("Index");
        }

        public FileResult? ShowPhoto(int id)
        {
            var food = _foodRepository.GetById(id);

            if (food != null && food.FoodImage?.Length > 0)
            {
                return File(food.FoodImage, "image/jpeg", food.FoodName + ".jpg");
            }
            return null;
        }
    }
}
