using System;
using System.Web.Mvc;

namespace E_Auction.Helpers
{
    public static class CustomModelBindersConfig
    {
        public static void RegisterCustomModelBinders()
        {
            ModelBinders.Binders.Add(typeof(DateTime), new CustomDateBinder());
        }
    }
}