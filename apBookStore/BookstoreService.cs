using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;

namespace apBookStore
{
    public class BookstoreService : IBookstoreService
    {
        public BookstoreService()
        {

        }

        public async Task<IEnumerable<IBook>> GetBooksAsync(string searchString = null)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://www.contribe.se/arbetsprov-net/books.json");
            var resp = await response.Content.ReadAsStringAsync();
            var res = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<JBooks>(resp));

            return res.books.Search<IBook>(searchString);
        }
    }
}
