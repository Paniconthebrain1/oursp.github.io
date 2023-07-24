using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurSunday.Models;
using OurSunday.Utilities;
using OurSunday.ViewModel;

namespace OurSunday.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;



        public UserController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                INotyfService notyfService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notification = notyfService;

        }

        [Authorize(Roles ="admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var vm = users.Select(x => new UserVM()
            {
                Id = x.Id,
                FristName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Email = x.Email

            }).ToList();

            return View(vm);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Register()
        {
            return View( new RegisterVM());
        }

        //[Authorize(Roles = "admin")]
        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterVM vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var Checkuserbyemail = await _userManager.FindByEmailAsync(vm.Email);
        //        if (Checkuserbyemail != null)
        //        {
        //            _notification.Error("Email Already Exists");
        //            return View(vm);
        //        }
        //        var checkuserbyusername = await _userManager.FindByNameAsync(vm.UserName);
        //        if (checkuserbyusername != null)
        //        {
        //            _notification.Error("UserName Already Exists");
        //            return View(vm);
        //        }
        //        var applicationUser = new ApplicationUser()
        //        {
        //            Email = vm.Email,
        //            UserName = vm.UserName,
        //            FirstName = vm.FristName,
        //            LastName = vm.LastName,
        //        };
        //        var result = await _userManager.CreateAsync(applicationUser, vm.Password);
        //        if (result.Succeeded)
        //        {
        //            if (vm.IsAdmin)
        //            {
        //                await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAdmin);
        //            }
        //            else
        //            {
        //                await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAuthor);

        //            }
        //            _notification.Success("User Registered Succesfully.");
        //            RedirectToAction("Index", "Home", new { area = "Admin" });
        //        }
        //    }           
        //    return View();
        //}
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (checkUserByEmail != null)
            {
                _notification.Error("Email already exists");
                return View(vm);
            }
            var checkUserByUsername = await _userManager.FindByNameAsync(vm.UserName);
            if (checkUserByUsername != null)
            {
                _notification.Error("Username already exists");
                return View(vm);
            }

            var applicationUser = new ApplicationUser()
            {
                Email = vm.Email,
                UserName = vm.UserName,
                FirstName = vm.FristName,
                LastName = vm.LastName,
            };

            var result = await _userManager.CreateAsync(applicationUser, vm.Password);
            if (result.Succeeded)
            {
                if (vm.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAdmin);
                }
                else
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAuthor);
                }
                _notification.Success("User registered successfully");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            _notification.Warning("Password doesnot match the requirement.");
            return View(vm);
        }



        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View(new LoginVM());
            }
            return RedirectToAction("Index", "User", new { area = "Admin" });

        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username);
            if (existingUser == null)
            {
                _notification.Error("UserName Doesnot exists!!");
                return View(vm);
            }

            var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, vm.Password);
            if (!verifyPassword)
            {
                _notification.Error("Password Doenot Match");
                return View(vm);
            }
            await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, true);
            _notification.Success("Logged In Succesfully");

            return RedirectToAction("Index", "User", new { area = "Admin" });

        }

        [HttpPost]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _notification.Success("Logged out Success");
            return RedirectToAction("Index", "Home" , new {area = ""});
        }
    }
}
