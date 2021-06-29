using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Infrastructure.Extensions
{
    public static class TempDataPopUpMessage
    {
        public static void AddSuccessMessage(this ITempDataDictionary tempData, string message)
        {
            tempData[TempDataSuccessMessageKey] = message;
        }

        public static void AddFailureMessage(this ITempDataDictionary tempData, string message)
        {
            tempData[TempDataErrorMessageKey] = message;
        }

        public static void EditFailureMessage(this ITempDataDictionary tempData, string message)
        {
            tempData[TempDataErrorMessageKey] = message;
        }

        public static void EditSuccessMessage(this ITempDataDictionary tempData, string message)
        {
            tempData[TempDataSuccessMessageKey] = message;
        }


        public static void AddDeniedAccessMessage(this ITempDataDictionary tempData, string message)
        {
            tempData[AccessDenied] = message;
        }
    }
}
