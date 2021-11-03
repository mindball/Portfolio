using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarTrade.Web.Infrastructure.Extensions
{
    public static class IEnumerableToSelecteListItemsExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItems<T>(
            this IEnumerable<T> items,
            Func<T, string> nameSelector,
            Func<T, string> valueSelector
     )
        {
            return items.OrderBy(item => nameSelector(item))
                   .Select(item =>
                           new SelectListItem
                           {                               
                               Text = nameSelector(item),
                               Value = valueSelector(item)
                           });
        }
    }
}
