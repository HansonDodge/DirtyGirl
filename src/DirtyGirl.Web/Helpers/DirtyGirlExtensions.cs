using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DirtyGirl.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Web.Helpers
{
    public static class DirtyGirlExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource>(this IEnumerable<TSource> items, Func<TSource, TSource, bool> equalityComparer) where TSource : class
        {
            return items.Distinct(new LambdaComparer<TSource>(equalityComparer));
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e, Name = e.ToString() };

            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static string GetAttributeValue<T>(this Enum e, Func<T, object> selector) where T : Attribute
        {

            var output = e.ToString();
            var member = e.GetType().GetMember(output).First();
            var attributes = member.GetCustomAttributes(typeof(T), false);

            if (attributes.Length > 0)
            {
                var firstAttr = (T)attributes[0];
                var str = selector(firstAttr).ToString();
                output = string.IsNullOrWhiteSpace(str) ? output : str;
            }

            return output;
        }

        public static IList<SelectListItem> ConvertToSelectList<T>(int? selectedItem = null)
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(x => new SelectListItem
            {
                Text = (x as Enum).GetAttributeValue<DisplayAttribute>(y => y.Name),
                Value = Convert.ToInt32(x).ToString(),
                Selected = selectedItem.HasValue ? selectedItem == Convert.ToInt32(x) : false
            }).ToList();
        }        

    }
}