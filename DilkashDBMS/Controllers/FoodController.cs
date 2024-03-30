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

        public IActionResult Edit(int id)
        {
            Food? food = _foodRepository.GetById(id);
            FoodViewModel foodObj = new FoodViewModel
            {
                FoodId = food.FoodId,
                FoodName = food.FoodName,
                FoodDescription = food.FoodDescription,
                FoodImage = food.FoodImage,
                FoodType = food.FoodType,
                Availability =food.Availability,
                Price = food.Price,
                CreatedAt = food.CreatedAt,
                ImageFile = null
            };
            return View(foodObj);
        }

        [HttpPost]
        public IActionResult Edit(FoodViewModel food)
        {
            try
            {
                Food foodObj = new Food
                {
                    FoodId = food.FoodId,
                    FoodName = food.FoodName,
                    FoodDescription = food.FoodDescription,
                    FoodImage = food.FoodImage,
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
                _foodRepository.Update(foodObj);
                return RedirectToAction("Details", new { id = food.FoodId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(food);
            }
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
