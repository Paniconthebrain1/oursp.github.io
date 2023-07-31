﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurSunday.Data;
using OurSunday.Models;
using OurSunday.ViewModel;
using System.Diagnostics;
using System.Drawing.Printing;
using X.PagedList;

namespace OurSunday.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }



        public async Task<IActionResult> Index(int? page)
        {
            var vm = new HomeVM();
            var setting  =  _context.Settings!.ToList();
            vm.Title = setting[0].Title;
            vm.ShortDescription = setting[0].ShortDescription;
            vm.ThumbnailUrl = setting[0].ThumbnailUrl;
            int pagesize = 4;
            int pagenumber = (page ?? 1);
            vm.Posts = await _context.Posts!.Include(x => x.ApplicationUser).ToPagedListAsync(pagenumber, pagesize); 
            //vm.Posts = await _context.Posts!.Include(x => x.ApplicationUser).OrderByDescending(x => x.CreatedDate).ToPagedListAsync(pageNumber, pageSize);

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}