using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apBookStore
{
       public class Book : IBook
    {
        public BookID ID
        {
            get
            {
                return new BookID { Author = this.Author, Title = this.Title, Price = this.Price };
            }
        }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }
        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }
        [JsonProperty(PropertyName = "InStock")]
        public int InStock { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", this.Title, this.Author, this.Price, this.InStock);
        }

    }

    //The books have no id so I make the assumption that  Author + Title + Price makes a book unique
    public struct BookID
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }

        public override bool Equals(Object obj)
        {
            return obj is BookID && this == (BookID)obj;
        }
        public override int GetHashCode()
        {
            return Title.GetHashCode() ^ Author.GetHashCode() ^ Price.GetHashCode();
        }
        public static bool operator ==(BookID x, BookID y)
        {
            return x.Author == y.Author
                && x.Title == y.Title
                && x.Price == y.Price;
        }
        public static bool operator !=(BookID x, BookID y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", this.Title, this.Author, this.Price);
        }
    }
}
