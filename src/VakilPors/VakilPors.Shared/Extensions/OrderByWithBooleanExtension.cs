using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace VakilPors.Shared.Extensions
{
    public static class OrderByWithBooleanExtension
    {
        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool isAscending)
        {
            return isAscending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
        }
        public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, string propertyName, bool isAscending)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(TSource)).Find(propertyName,true);
            return isAscending ? source.OrderBy(x => prop.GetValue(x)) : source.OrderByDescending(x => prop.GetValue(x));
        }
    }

}