﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wonderly_Blog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "Details",
                url: "Blog/{slug}",
                defaults: new { controller = "BlogPosts", action = "Details", slug = UrlParameter.Optional }
            );
/*
            routes.MapRoute(
                name: "Delete",
                url: "Blog/{action}/{slug}",
                defaults: new { controller = "BlogPosts", action = "Delete", slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Edit",
                url: "Blog/{action}/{slug}",
                defaults: new { controller = "BlogPosts", action = "Edit", slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Index",
                url: "Blog/{action}/{slug}",
                defaults: new { controller = "BlogPosts", action = "Index", slug = UrlParameter.Optional }
            );

    */
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "BlogPosts", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
