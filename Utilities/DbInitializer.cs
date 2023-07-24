using Microsoft.AspNetCore.Identity;
using OurSunday.Data;
using OurSunday.Models;

namespace OurSunday.Utilities
{
    public class Dbinitializer : IDbinitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public Dbinitializer(ApplicationDbContext context,
                                UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> rolemanager)
        {
            _context = context;
            _usermanager = userManager;
            _roleManager = rolemanager;
        }
        public void Initialize()
        {
            if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAuthor)).GetAwaiter().GetResult();
                _usermanager.CreateAsync(new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin"
                }, "Admin@0011").Wait();


                var appuser = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
                if (appuser != null)
                {
                    _usermanager.AddToRoleAsync(appuser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();

                }


                var listOfPages = new List<Page>()
                {
                    new Page()
                    {
                        Title = "About Us",
                        Slug = "about"
                    },
                    new Page()
                    {
                        Title = "Contact Us",
                        Slug = "contact"
                    },
                    new Page()
                    {
                        Title = "Privacy Policy",
                        Slug = "privacy"
                    }
                 };

                _context.Pages!.AddRange(listOfPages);
                _context.SaveChanges();
            }
        }
    }
}
