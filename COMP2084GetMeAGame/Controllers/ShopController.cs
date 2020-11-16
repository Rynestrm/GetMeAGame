using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COMP2084GetMeAGame.Data;
using COMP2084GetMeAGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    }
}
