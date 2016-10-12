using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using apBookStore;
using System.Text;


namespace WebBookStore
{
    public partial class _default : System.Web.UI.Page
    {
        BookCart Cart;
        List<Book> ListBox1DS;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["BookCart"] == null)
                Session["BookCart"] = new BookCart();
            if (Session["ListBox1DS"] == null)
                Session["ListBox1DS"] = new List<Book>();

            Cart = (BookCart)Session["BookCart"];
            ListBox1DS = (List<Book>)Session["ListBox1DS"];



            if (!IsPostBack)
            {
                UpdateBooks();
            }
        }

        private void UpdateCart()
        {
            var sb = new StringBuilder();
            var ci = Cart.GetCartItems();
            ci.ForEach(i => sb.AppendLine(i.ToString()));
            CartLiteral.InnerHtml = sb.ToString().Replace(Environment.NewLine,"<br />");
        }

        private async void UpdateBooks()
        {
            var bs = new BookstoreService();
            IEnumerable<IBook> ret;
            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                ret = await bs.GetBooksAsync(searchTextBox.Text);
            }
            else
            {
                ret = await bs.GetBooksAsync();
            }

            ListBox1.DataSource = ret.Cast<Book>().ToList();
            ListBox1DS = (List<Book>)ListBox1.DataSource;
            Session["ListBox1DS"] = (List<Book>)ListBox1.DataSource;
            ListBox1.DataBind();
        }

        protected void SearchBT_Click(object sender, EventArgs e)
        {
            UpdateBooks();
        }

        protected void ShowAllBT_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = null;
            UpdateBooks();
        }

        protected void AddToCartBT_Click(object sender, EventArgs e)
        {
            if (ListBox1.SelectedItem != null)
            {
                try
                {
                    var sBook = ListBox1DS[ListBox1.SelectedIndex];
                    Cart.AddBook(sBook.ID);
                }
                catch (Exception ex)
                {
                    //ToDo: use a string builder
                    InfoLiteral.InnerHtml = "Error:" + Environment.NewLine;
                    InfoLiteral.InnerHtml += ex.Message;
                    InfoLiteral.InnerHtml.Replace(Environment.NewLine, "<br />");
                }
                UpdateCart();
            }
        }

        protected async void CeckoutBT_Click(object sender, EventArgs e)
        {
            var updatedCartInfo = await Cart.CheckOut();

            var sb = new StringBuilder();
            if (!updatedCartInfo.CartItems.Any())
                sb.AppendLine("Cart is empty");
            else
            {
                sb.AppendLine("Order details:");
                updatedCartInfo.CartItems.ForEach(i => sb.AppendLine(i.ToString()));
                sb.AppendLine("TotalCost: " + updatedCartInfo.CartItems.Sum(ci => ci.PriceSum));
                sb.AppendLine();
            }

            if (updatedCartInfo.RemovedCartItems.Any())
            {
                sb.AppendLine("Removed cart items (not in stock):");
                updatedCartInfo.RemovedCartItems.ForEach(ri => sb.AppendLine(ri.ToString()));
            }

            UpdateCart();
            InfoLiteral.InnerHtml = sb.ToString().Replace(Environment.NewLine, " <br />");
        }
    }

   
}