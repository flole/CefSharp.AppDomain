using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CefSharp.AppDomain.Lib;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            ICefSharpRenderer renderer = new CefSharpRendererProxy();
            //Try to render something in default appdomain
            Console.WriteLine("Render something in default AppDomain: ");
            renderer.RenderSomething();
            Console.WriteLine("Works!");

            return View();
        }
    }
}
