using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COMP2084GetMeAGame.Data;
using COMP2084GetMeAGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace COMP2084GetMeAGame.Controllers
{
    public class ShopController : Controller
    {
        // DB connection
        private readonly ApplicationDbContext _context;

        // configuration dependency needed to read Stripe Keys from appsettings.json or the secret key store
        private IConfiguration _configuration;

        // connect to the db whenever this controller is used
        // this controller uses Depedency Injection - it requires a db connection object when it's created
        public ShopController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            //get list of categories to display to customers on the main shopping page
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();
            return View(categories);
        }

        //shop/browse/3
        public IActionResult Browse(int id)
        {
            // get products from selected category
            var products = _context.Products.Where(p => p.CategoryId == id).OrderBy(p => p.Name).ToList();
            //load the browse page and pass it to display
            return View(products);
        }

        // GET: /Shop/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int ProductId, int Quantity)
        {
            // get current price of the product
            var price = _context.Products.Find(ProductId).Price;

            // identify the customer
            var customerId = GetCustomerId();

            // check if product already exists in this user's cart
            var cartItem = _context.Carts.SingleOrDefault(c => c.ProductId == ProductId && c.CustomerId == customerId);

            if (cartItem != null)
            {
                // product already exists so update the quantity
                cartItem.Quantity += Quantity;
                _context.Update(cartItem);
                _context.SaveChanges();
            }
            else
            {
                // create a new Cart object
                var cart = new Cart
                {
                    ProductId = ProductId,
                    Quantity = Quantity,
                    Price = price,
                    CustomerId = customerId,
                    DateCreated = DateTime.Now
                };

                // use the Carts DbSet in ApplicationContext.cs to save to the database
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            // redirect to show the current cart
            return RedirectToAction("Cart");
        }

        private string GetCustomerId()
        {
            // is there already a session variable for the customer
            if (HttpContext.Session.GetString("CustomerId") == null)
            {
                // cart is empty user is unknown
                var customerId = "";

                // use a GUID to make a unique Id
                customerId = Guid.NewGuid().ToString();

                //store new Id in session variable
                HttpContext.Session.SetString("CustomerId", customerId);
            }

            return HttpContext.Session.GetString("CustomerId");
        }

        //Shop/Cart
        public IActionResult Cart()
        {
            // get CustomerId from the session variable
            var customerId = HttpContext.Session.GetString("CustomerId");
            // get items in this customer's cart
            var cartItems = _context.Carts.Include(c => c.Product).Where(c => c.CustomerId == customerId).ToList();

            // count the # of items in the Cart and write to a session variable to display in the navbar
            var itemCount = (from c in _context.Carts
                             where c.CustomerId == customerId
                             select c.Quantity).Sum();
            HttpContext.Session.SetInt32("ItemCount", itemCount);
            // load the cart page and display the customer's items
            return View(cartItems);
        }

        // GET: /Shop/RemoveFromCart/12
        public IActionResult RemoveFromCart(int id)
        {
            // remove the selected item from Carts table
            var cartItem = _context.Carts.Find(id);

            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }

            // redirect to updated Cart page
            return RedirectToAction("Cart");
        }

        //get/shop/checkout
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        //POST / Shop/checkout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("Address, City, Provence, PostalCode")]Order order)
        {
            //auto fill 3 fields in form
            order.OrderDate = DateTime.Now;
            order.CustomerId = User.Identity.Name;
            order.Total = (from c in _context.Carts
                           where c.CustomerId == HttpContext.Session.GetString("CustomerId")
                           select c.Quantity * c.Price).Sum();

            //store order in a session variable before moving to payment page
            HttpContext.Session.SetObject("Order", order);

            //load payment page
            return RedirectToAction("Payment");
        }

        //get shop/payment
        [Authorize]
        public IActionResult Payment()
        {
            //get order from session variable
            var order = HttpContext.Session.GetObject<Order>("Order");
            //fech and display the order total to customer
            ViewBag.Total = order.Total;
            // use view bag to set publishable key that we can read from the configuration
            ViewBag.PublishableKey = _configuration.GetSection("Stripe")["PUblishableKey"];
            // load the PAyment view
            return View();
        }

    }
}
