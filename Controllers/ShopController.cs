using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Miocas.Infrastructure;
using Miocas.Models;

namespace Miocas.Controllers
{
    public class ShopController : Controller
    {
        private readonly DataContext _context;

        public ShopController(DataContext context)
        {
            _context = context;

        }

        public async Task<Product> LoadData(long id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList(); // Получаем все товары из базы данных

            return View(products);
        }

        [Authorize]
        public IActionResult Create()
        {


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create(Product product) //Создаение продукта ( для админа)
        {
            if (ModelState.IsValid)
            {
                // Создайте список для хранения данных изображений
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await product.ImageFile.CopyToAsync(ms);
                        product.Image = ms.ToArray();
                    }
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(product);
        }

        public async Task<IActionResult> Details(long id) //Детали продукта
        {

            var product = await LoadData(id);

            if (product == null)
            {
                return NotFound(); // Если продукт с указанным id не найден, возвращаем HTTP 404
            }

            return View(product);
        }

        [Authorize(Roles = "Admin")] //Проверка на админа
        public async Task<IActionResult> DeleteProduct(long id) //Удаление продукта ( для админа)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");  // Redirect to the admin page after deletion
        }
    }
}
