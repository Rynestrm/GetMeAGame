using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COMP2084GetMeAGame.Data;
using Microsoft.AspNetCore.Mvc;

namespace COMP2084GetMeAGame.Controllers
{
    public class ShopController : Controller
    {
        // DB connection
        private readonly ApplicationDbContext _context;
        //connect to db whenever controller is used
        public ShopController(ApplicationDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            //get list of categories to sidplay to customers on the main shopping page
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();
            return View();
        }
    }
}
