using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apBookStore
{

    public class CartItem
    {
        public BookID BookID { get; set; }
        public int Quantity { get; set; }
        public decimal PriceSum { get { return BookID.Price * Quantity; } }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}", BookID.Title, BookID.Author, Quantity, BookID.Price, PriceSum);
        }

    }

    public class CartUpdateInfo
    {
        public List<CartItem> CartItems { get; internal set; }
        public List<CartItem> RemovedCartItems { get; internal set; }

    }


    public class BookCart
    {
        private Dictionary<BookID, int> CartDic = new Dictionary<BookID, int>();
        public void AddBook(BookID bookID, int quantity = 1)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException("quantity");

            //Comment rows are if we want to check current stock status on a add. but not all where checked and I did not know if it where necessary 
            //var inCartBefore = Quantity(bookID);

            if (CartDic.ContainsKey(bookID))
            {
                CartDic[bookID] += quantity;
            }
            else
            {
                CartDic.Add(bookID, quantity);
            }

            //await GetAndUpdateCartItems();

            //var inCartAfter = Quantity(bookID);

            //if (inCartBefore >= inCartAfter)
            //    throw new Exception("Selected book are not in stock");
        }

        public List<CartItem> GetCartItems()
        {
            var ret = CartDic
               .Select(ci => new CartItem { BookID = ci.Key, Quantity = ci.Value}).ToList();
            return ret;

        }

        public int InCart(BookID bookID)
        {
            if (CartDic.ContainsKey(bookID))
            {
                return CartDic[bookID];
            }
            else
            {
                return 0;
            }
        }

        private async Task<CartUpdateInfo> GetAndUpdateCartItems()
        {
            ////////////////////////////////
            //Get updated/latest data
            ////////////////////////////////
            var bookStoreService = new BookstoreService();
            var books = await bookStoreService.GetBooksAsync();
            
            //Save current state of CartItems 
            var CartItemsStart = GetCartItems();

            ////////////////////////////////
            //Update with latest data
            ////////////////////////////////

            //Only let existing books remain in cart.
            //Select CartItems get updated Book info from books and current Quantity from CartDic,
            //Check stock 
            //Only let books having Quantity > 0 remain
            var CartItems = CartDic.Where(ci => books.Any(b => b.ID == ci.Key)).ToDictionary(d => d.Key, d => d.Value)
                .Select(cdi => new { book = ((Book)books.Single(b => b.ID == cdi.Key)), ci = cdi }) 
                .Select(i => new CartItem { BookID = i.book.ID, Quantity = i.ci.Value > i.book.InStock ? i.book.InStock : i.ci.Value })
                .Where(ci => ci.Quantity > 0).ToList();

            //Update cart
            CartDic.Clear();
            CartItems.ForEach(ci => {
                 CartDic.Add(ci.BookID, ci.Quantity);
            });

            var ret = new CartUpdateInfo();
            ret.CartItems = CartItems;
            ret.RemovedCartItems = new List<CartItem>();

            //////////////////////////
            //build RemovedCartItems
            ///////////////////
            CartItemsStart.ForEach(startCi => {
                var newCI = CartItems.SingleOrDefault(ci => ci.BookID == startCi.BookID);
                if (newCI == null) //no book in cart
                    ret.RemovedCartItems.Add(startCi);
                else if (newCI.Quantity < startCi.Quantity)
                {
                    ret.RemovedCartItems.Add(new CartItem { BookID = startCi.BookID, Quantity = startCi.Quantity - newCI.Quantity });
                }

            });

            return ret;
        }

        public async Task<CartUpdateInfo> CheckOut()
        {
            //Get updated and adjusted cart items
            var CartUpdateInfo = await GetAndUpdateCartItems();

            //Clear cart
            CartDic.Clear();

            //return updated and adjusted cart items
            return CartUpdateInfo;
        }
    }
}
