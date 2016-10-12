using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Net.Http;
using Newtonsoft.Json;

namespace apBookStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter 'help' to get help...");
            var bookStoreService = new BookstoreService();
            List<Book> Books = new List<Book>();
            try
            {
                Books = bookStoreService.GetBooksAsync().Result.Cast<Book>().ToList();
            }
            catch (AggregateException ex)
            {
                ex.Handle((e) => {
                    if (e is HttpRequestException)
                    {
                        Console.WriteLine(e.Message);
                        return true;
                    }
                    if (e is JsonSerializationException)
                    {
                        Console.WriteLine(e.Message);
                        return true;
                    }
                    return false;
                });
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex);
            }

            var cart = new BookCart();
            var type = typeof(BookCommand);

            while (true)
            {
                Console.Write("> ");

                var input = Console.ReadLine().ToLower().Split(' ');
                var cmdTxt = input[0];
                BookCommand cmd;

                if (!Enum.TryParse(cmdTxt, out cmd))
                {
                    Console.WriteLine("No such command");
                }
                else
                {
                    switch (cmd)
                    {
                        case BookCommand.exit:
                            Environment.Exit(0);
                            break;
                        case BookCommand.listcart:
                            var sb = new StringBuilder();
                            var ci = cart.GetCartItems();
                            ci.ForEach(i => sb.AppendLine(i.ToString()));    
                            Console.WriteLine(sb.ToString());
                            break;
                        case BookCommand.listbooks:
                            Books.ForEach(b => Console.WriteLine(Books.IndexOf(b) + ": " + b.Title));
                            break;

                        case BookCommand.add:
                            if (input.Length == 2)
                            {
                                int bId;
                                if (int.TryParse(input[1], out bId) && Books.Count > bId)
                                {
                                    var selected = Books[bId];
                                    cart.AddBook(selected.ID);
                                    Console.WriteLine("Book added to cart");
                                }
                                else
                                    Console.WriteLine("No such book");
                            }
                            else
                                Console.WriteLine("Syntax error");
                            break;
                        case BookCommand.checkout:
                            var updatedCartInfo = cart.CheckOut().Result;
                             
                            var sbC = new StringBuilder();
                            sbC.AppendLine("Order Details:");
                            if (!updatedCartInfo.CartItems.Any())
                                sbC.AppendLine("Cart is empty");
                            else
                            {

                                updatedCartInfo.CartItems.ForEach(i => sbC.AppendLine(i.ToString()));
                                sbC.AppendLine("TotalCost: " + updatedCartInfo.CartItems.Sum(ci1 => ci1.PriceSum));
                                sbC.AppendLine();
                            }

                            if (updatedCartInfo.RemovedCartItems.Any())
                            {
                                sbC.AppendLine("Removed cart items (not in stock):");
                                updatedCartInfo.RemovedCartItems.ForEach(ri => sbC.AppendLine(ri.ToString()));
                            }

                            Console.Write(sbC.ToString());

                            break;
                        case BookCommand.details:
                           
                            if (input.Length == 2)
                            {
                                int bId;
                                if (int.TryParse(input[1], out bId) && Books.Count > bId)
                                {
                                    var selected = Books[bId];
                                    Console.WriteLine(selected.ToString());
                                }
                                else
                                    Console.WriteLine("No such book");
                            }
                            else
                                Console.WriteLine("Syntax error");
                            break;

                        case BookCommand.help:
                            var commandDescList = Enum.GetValues(typeof(BookCommand)).Cast<BookCommand>()
                                .Select(val => val.ToString() + " : " +  val.GetAttribute<DescriptionAttribute>().Description)
                                .ToList();

                            commandDescList.ForEach(c => Console.WriteLine(c));


                            break;

                        default:
                            break;
                    }
                }

            }


        }


    }

    public enum BookCommand
    {
        [Description("List commands")]
        help,
        [Description("List all books")]
        listbooks,
        [Description("Show details eg.: 'details <index>'")]
        details,
        [Description("Add to cart eg.: 'add <index>'")]
        add,
        [Description("List of actual cart (cart is updated with latest data)")]
        listcart,
        [Description("Bye books in cart of book is not in stock they will first be removed")]
        checkout,
        [Description("Exit bookstore")]
        exit,
    }
}
