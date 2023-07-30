using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OurSunday.Data;
using OurSunday.Models;
using OurSunday.Utilities;
using OurSunday.ViewModel;
using System.Runtime.InteropServices;

namespace OurSunday.Areas.Admin.Controllers
{
    [Area("Admin")]


    public class PostController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _WebHostenvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notification { get; }

        public PostController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostenvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notification = notyfService;
            _WebHostenvironment = webHostenvironment;
            _userManager = userManager;
        }


        [HttpGet]

        public async Task<IActionResult> Index()
        {

            var listofpost = new List<Post>();

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] == WebsiteRoles.WebsiteAdmin)
            {
                listofpost = await _context.Posts.Include(x => x.ApplicationUser).ToListAsync();
            }
            else
            {
                listofpost = await _context.Posts!.Include(x => x.ApplicationUser).Where(x => x.ApplicationUser!.Id == loggedInUser!.Id).ToListAsync();
            }

            var listOfPostVM = listofpost.Select(x => new PostVM()
            {
                Id = x.Id,
                Title = x.Title,
                CreatedDate = x.CreateDate,
                ThumbnailUrl = x.ThumbnailUrl,
                AuthorName = x.ApplicationUser!.FirstName + " " + x.ApplicationUser!.LastName


            }).ToList();
            return View(listOfPostVM);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View(new CreatePostVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }

            //get login id 

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

            var post = new Post();

            post.Title = vm.Title;
            post.Description = vm.Description;
            post.ShortDescription = vm.ShortDescription;
            post.ApplicationUserid = loggedInUser!.Id;

            if (post.Title != null)
            {
                string slug = vm.Title!.Trim();
                slug = slug.Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }


            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            await _context.Posts!.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post Published Succesfully.");
            return RedirectToAction("Index");


        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] == WebsiteRoles.WebsiteAdmin || loggedInUser?.Id == post!.ApplicationUserid)
            {
                _context.Posts!.Remove(post!);
                await _context.SaveChangesAsync();
                _notification.Success("Post Deleted Succesfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });

            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
            {
                _notification.Error("Post not Found");
                return View();
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] != WebsiteRoles.WebsiteAdmin && loggedInUser!.Id != post.ApplicationUserid)
            {
                _notification.Error("You are not authorized");
                return RedirectToAction("Index");
            }


            var vm = new CreatePostVM()
            {
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Description = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,

            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var post = await _context.Posts!.FirstOrDefaultAsync(y => y.Id == vm.Id);
            if (post == null)
            {
                _notification.Error("Post not Found");
                return View();
            }
            post.Title = vm.Title;
            post.ShortDescription = vm.ShortDescription;
            post.Description = vm.Description;

            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            await _context.SaveChangesAsync();
            _notification.Success("Post Updated Succesfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }


        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderpath = Path.Combine(_WebHostenvironment.WebRootPath, "thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderpath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }

    }
}
