using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apBookStore
{
    public static class Extentions
    {
        public static IEnumerable<apBookStore.IBook> Search<IBook>(this IEnumerable<apBookStore.IBook> data, string searchString)
        {
            Func<apBookStore.IBook, bool> predicate =
                b => searchString == null
                || b.Author.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                || b.Title.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;

            foreach (apBookStore.IBook value in data)
            {
                if (predicate(value)) yield return value;
            }
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}
