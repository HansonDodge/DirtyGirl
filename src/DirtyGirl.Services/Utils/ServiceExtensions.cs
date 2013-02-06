﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Services.Utils
{
    public static class ServiceExtensions
    {
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

    }
}
