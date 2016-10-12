using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace apBookStore
{
    class Program
    {
        static void Main(string[] args)
        {

            var bookStoreService = new BookstoreService();
            var Books = bookStoreService.GetBooksAsync().Result.ToList();
            var cart = new BookCart();

            
            Console.WriteLine("Enter 'help' to get help...");

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
                            Console.WriteLine(cart);
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
        [Description("List of actual cart (cart is uppdated with latest data)")]
        listcart,
        [Description("Bye books in cart of book is not in stock they vill first be removed")]
        checkout,
        [Description("Exit bookstore")]
        exit,
    }
}
