using COMP2084GetMeAGame.Controllers;
using COMP2084GetMeAGame.Data;
using COMP2084GetMeAGame.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace GetMeAGameTests
{
    [TestClass]
    public class ProductsControllerTest
    {
        // class-level variables used for all unit tests
        // mock in-memory db
        private ApplicationDbContext _context;

        // mock list of products
        List<Product> products = new List<Product>();

        // controller object used for all unit tests
        ProductsController controller;

        // arrange code that runs automatically before every unit test
        [TestInitialize]
        public void TestInitialize()
        {
            // create new in-memory db
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;
            _context = new ApplicationDbContext(options);

            // create mock data and add to in-memory db
            var category = new Category { Id = 300, Name = "Some Category" };

            _context.Categories.Add(category);
            _context.SaveChanges();

            products.Add(new Product { Id = 46, Name = "Product Forty-Six", Price = 46.46, CategoryId = 300, Category = category });
            products.Add(new Product { Id = 88, Name = "Product Eight-Eight", Price = 88.88, CategoryId = 300, Category = category });
            products.Add(new Product { Id = 51, Name = "Product Fifty-One", Price = 51.51, CategoryId = 300, Category = category });

            foreach (var p in products)
            {
                _context.Add(p);
            }

            _context.SaveChanges();

            // now create the controller and pass it the dbcontext
            controller = new ProductsController(_context);
        }

        [TestMethod]
        public void IndexViewIsNotNull()
        {
            // 1. arrange - set up what we need to execute the method we want to test (variables, etc.)
            // arrange is now done in TestInitialize

            // 2. act - execute the method we want to test & obtain a result
            var result = (ViewResult)controller.Index().Result;

            // 3. assert - check if the result we got is the result we expected
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void IndexLoadsIndexView()
        {
            //act
            var result = (ViewResult)controller.Index().Result;

            //assert
            Assert.AreEqual("Index", result.ViewName);
        }
    }
}
