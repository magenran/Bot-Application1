﻿using System.Web;
using System.Web.Mvc;

namespace Bot_Application1
{
    public class FilterConfig
    {
        public  void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
