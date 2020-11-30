using COMP2084GetMeAGame.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetMeAGameTest
{
    [TestClass]
    public class DummiesControllerTest
    {
        [TestMethod]
        public void IndexReturnsSomething()
        {
            //Arrange
            var controller = new DummiesController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexLoadsIndexView()
        {
            //arrange
            var controller = new DummiesController();

            //act
            var result = (ViewResult)controller.Index();

            //assert
            Assert.AreEqual("Index", result.ViewName);
        }

    }
}
