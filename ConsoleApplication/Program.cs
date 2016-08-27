using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp.AppDomain.Lib;

namespace ConsoleApplication
{
    class Program
    {
        const bool RunInNonDefaultAppDomain = false;
        static void Main(string[] args)
        {
            var nonDefaultAppDomain = AppDomain.CreateDomain("nonDefaultAppDomain");
            nonDefaultAppDomain.DoCallBack(() =>
            {
                try
                {
                    if (RunInNonDefaultAppDomain)
                    {
                        ICefSharpRenderer renderer = new CefSharpRenderer();
                        //Try to render something in non default appdomain
                        Console.WriteLine("Render something in non default AppDomain:");
                        renderer.RenderSomething();
                        Console.WriteLine("Works!");
                    }
                    else
                    {
                        ICefSharpRenderer renderer = new CefSharpRendererProxy();
                        //Try to render something in default appdomain
                        Console.WriteLine("Render something in default AppDomain: ");
                        renderer.RenderSomething();
                        Console.WriteLine("Works!");
                    }
                }
                catch (Exception)
                {

                    Console.WriteLine("Something went wrong");
                }
            });
        }
    }
}
