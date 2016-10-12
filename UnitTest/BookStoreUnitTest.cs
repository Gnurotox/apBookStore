using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using apBookStore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class BookStoreUnitTest
    {
        [TestMethod]
        public async Task TestCartAddCartCheckout()
        {

            // arrange
            var bs = new BookstoreService();
            var gba = await bs.GetBooksAsync();
            var books = gba.Cast<Book>().ToList();

            var cart = new BookCart();
            var BookID = books[0].ID;
            cart.AddBook(BookID);


            // act
            var result = await cart.CheckOut();
            var cartCount = cart.GetCartItems().Count;
            
            // assert
            Assert.IsTrue(result.CartItems.Any(ci => ci.BookID == BookID && ci.Quantity == 1));
            Assert.AreEqual(0, cartCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task TestCartAdd()
        {

            // arrange
            var bs = new BookstoreService();
            var gba = await bs.GetBooksAsync();
            var books = gba.Cast<Book>().ToList();

            var cart = new BookCart();
            var BookID = books[0].ID;


            // act
            cart.AddBook(BookID, -1);
        }

    }
}
