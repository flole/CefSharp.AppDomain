using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using mscoree;

namespace CefSharp.AppDomain.Lib
{
    public class CefSharpRendererProxy : ICefSharpRenderer
    {
        public string RenderSomething()
        {
            //Get the default appdomain. This will also work if the default appdomain comes from a service like the IIS
            var defaultAppDomain = GetAppDomains().Single(domain => domain.IsDefaultAppDomain());
            //Get the path to the assembly where the CefSharpRenderer is implemented
            var pathToAssembly = new Uri(Assembly.GetAssembly(typeof(CefSharpRenderer)).CodeBase).LocalPath;

            //Create a new instance of the CefSharpRenderer in the context of the default appdomain
            var instance = (ICefSharpRenderer)defaultAppDomain.CreateInstanceFromAndUnwrap(pathToAssembly, typeof(CefSharpRenderer).FullName);
            return instance.RenderSomething();
        }

        private static List<System.AppDomain> GetAppDomains()
        {
            var appDomains = new List<System.AppDomain>();
            var enumHandle = IntPtr.Zero;
            var host = new CorRuntimeHostClass();
            try
            {
                host.EnumDomains(out enumHandle);

                while (true)
                {
                    object domain;
                    host.NextDomain(enumHandle, out domain);
                    if (domain == null) break;
                    var appDomain = (System.AppDomain)domain;
                    appDomains.Add(appDomain);
                }
                return appDomains;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                host.CloseEnum(enumHandle);
                Marshal.ReleaseComObject(host);
            }
        }
    }
}
