using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Miocas.Infrastructure;
using Miocas.Models;
using Miocas.Models.ViewModels;

namespace Miocas.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly DataContext _context;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = dataContext;
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(User user) //Регистрация
        {
            if (ModelState.IsValid)
            {
                User newUser = new User { UserName = user.UserName, Email = user.Email, Miocas_Password = user.Miocas_Password };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Miocas_Password);

                if (result.Succeeded)
                {
                    if (user.UserName == "Admin")
                    {
                        await _userManager.AddToRoleAsync(newUser, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, "User");
                    }
                    return Redirect("/account/login");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(user);
        }
        public IActionResult Login(string returnUrl) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM) //Вход
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);

                if (result.Succeeded)
                {
                    return Redirect(loginVM.ReturnUrl ?? "/");
                }

                ModelState.AddModelError("", "Invalid username or password");
            }

            return View(loginVM);
        }

        public async Task<IActionResult> ViewHistory() //Просмотр истории заказов
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                var allOrders = _context.Orders
                    .Include(o => o.User) // Include the User property
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ToList();

                return View(allOrders);
            }
            else
            {
                var userOrders = _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Where(o => o.UserId == currentUser.Id)
                    .ToList();

                return View(userOrders);
            }
        }



        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();

            return Redirect(returnUrl);
        }
    }
}
