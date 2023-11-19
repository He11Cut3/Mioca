using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Miocas.Infrastructure;
using Miocas.Models;
using Miocas.Models.ViewModels;

namespace Miocas.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }




        // GET: CartController
        public ActionResult Index() //Отображение корзины
        {
            List<ItemCart> cart = HttpContext.Session.GetJson<List<ItemCart>>("Cart") ?? new List<ItemCart>();

            CartViewModel cartVM = new()
            {
                ItemCarts = cart,
                GrandTotal = cart.Sum(x => x.Quantity * x.Price)
            };

            return View(cartVM);
        }

        public async Task<IActionResult> Add(long id) //Добавление товара в корзине
        {
            Product product = await _context.Products.FindAsync(id);

            List<ItemCart> cart = HttpContext.Session.GetJson<List<ItemCart>>("Cart") ?? new List<ItemCart>();

            ItemCart cartItem = cart.Where(c => c.Id == id).FirstOrDefault();

            if (cartItem == null)
            {
                cart.Add(new ItemCart(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);


            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> Decrease(long id) //Уменьшение товара
        {
            List<ItemCart> cart = HttpContext.Session.GetJson<List<ItemCart>>("Cart");

            ItemCart cartItem = cart.Where(c => c.Id == id).FirstOrDefault();

            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.Id == id);
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["Success"] = "The product has been removed!";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(long id) // Удаление товара из корзины
        {
            List<ItemCart> cart = HttpContext.Session.GetJson<List<ItemCart>>("Cart");

            cart.RemoveAll(p => p.Id == id);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["Success"] = "The product has been removed!";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder() //Создание заказа ( при нажатии на кнопку оформить)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            string userId = _userManager.GetUserId(User);

            List<ItemCart> cart = HttpContext.Session.GetJson<List<ItemCart>>("Cart") ?? new List<ItemCart>();

            if (cart.Count == 0)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }

            // Загрузите пользователя и обновите его данные перед созданием заказа
            User user = await _userManager.FindByIdAsync(userId);
            await _userManager.UpdateAsync(user);

            Order order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                OrderItems = cart.Select(item => new OrderItem
                {
                    ProductId = item.Id,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            _context.Orders.Add(order);

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Order has been placed successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Обработка исключения
                TempData["Error"] = "Error placing the order. Please try again.";
                return RedirectToAction("Index", "Cart");
            }
        }

        public IActionResult Clear() //Чистка корзины
        {
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Index");
        }
    }
}
