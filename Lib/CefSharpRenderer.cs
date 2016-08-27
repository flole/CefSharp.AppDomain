using System;
using System.Reflection;
using System.Threading;
using CefSharp.OffScreen;

namespace CefSharp.AppDomain.Lib
{
    public class CefSharpRenderer : MarshalByRefObject, ICefSharpRenderer
    {
        private ChromiumWebBrowser _browser;
        private SemaphoreSlim _renderingFinishedSemaphore = new SemaphoreSlim(0, 1);
        public void RenderSomething()
        {
            if (!Cef.IsInitialized)
            {
                var settings = new CefSettings();

                var osVersion = Environment.OSVersion;
                //Disable GPU for Windows 7
                if (osVersion.Version.Major == 6 && osVersion.Version.Minor == 1)
                {
                    // Disable GPU in WPF and Offscreen examples until #1634 has been resolved
                    settings.CefCommandLineArgs.Add("disable-gpu", "1");
                }

                //Perform dependency check to make sure all relevant resources are in our output directory.
                Cef.Initialize(settings, shutdownOnProcessExit: true, performDependencyCheck: false);
            }


            _browser = new ChromiumWebBrowser();
            _browser.BrowserInitialized += _browser_BrowserInitialized;
            _browser.LoadingStateChanged += _browser_LoadingStateChanged;

            _renderingFinishedSemaphore.Wait();
        }

        private void _browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
            {
                return;
            }

            //Google has been loaded
            //Yay!
            _renderingFinishedSemaphore.Release();
        }

        private void _browser_BrowserInitialized(object sender, EventArgs e)
        {
            _browser.Load("http://www.google.de");
        }
    }
}
