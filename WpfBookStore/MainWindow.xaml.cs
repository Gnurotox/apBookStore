using apBookStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text;
namespace WpfBookStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BookCart Cart = new BookCart();
        public MainWindow()
        {
            InitializeComponent();
            UpdateBooks();
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

            booksLB.ItemsSource = ret.Cast<Book>().ToList();
        }

        private void booksLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          //textBlock.Text = ((Book)(booksLB.SelectedItem)).ToString();
        }

        private async void btCheckoutt_Click(object sender, RoutedEventArgs e)
        {
            var updatedCartInfo = await Cart.CheckOut();

            var sb = new StringBuilder();
            if (!updatedCartInfo.CartItems.Any())
                    sb.AppendLine("Cart is empty");
            else
            {

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
            MessageBox.Show(sb.ToString(), "Order details:");
        }



        //Updates the text for the cart
        private void UpdateCart()
        {
            var sb = new StringBuilder();
            var ci = Cart.GetCartItems();
            ci.ForEach(i => sb.AppendLine(i.ToString()));
            cartTxt.Text = sb.ToString();
        }


        private void btAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (booksLB.SelectedItem != null)
            {
                try
                {
                   Cart.AddBook(((Book)(booksLB.SelectedItem)).ID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
                UpdateCart();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            UpdateBooks();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = null;
            UpdateBooks();
        }
    }
}
