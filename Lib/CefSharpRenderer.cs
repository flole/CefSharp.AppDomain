using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CefSharp.Internals;
using CefSharp.OffScreen;

namespace CefSharp.AppDomain.Lib
{
    public class CefSharpRenderer : MarshalByRefObject, ICefSharpRenderer
    {
        private ChromiumWebBrowser _browser;
        private TaskCompletionSource<JavascriptResponse> _taskCompletionSource;
        public string RenderSomething()
        {
            if (!Cef.IsInitialized)
            {
                var settings = new CefSettings();
                var assemblyPath = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath);

                settings.BrowserSubprocessPath = Path.Combine(assemblyPath, "CefSharp.BrowserSubprocess.exe");
                settings.ResourcesDirPath = assemblyPath;
                settings.LocalesDirPath = Path.Combine(assemblyPath, "locales");

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

            _taskCompletionSource = new TaskCompletionSource<JavascriptResponse>();
            _taskCompletionSource.Task.Wait();
            return _taskCompletionSource.Task.Result.Result as string;
        }

        private void _browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
            {
                return;
            }

            //Google has been loaded
            //Yay!

            var t1 = _browser.EvaluateScriptAsync("(function() { return document.title })();");

            var complete = t1.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;

                }

                _taskCompletionSource.TrySetFromTask(t);

            }, TaskScheduler.Default);
        }

        private void _browser_BrowserInitialized(object sender, EventArgs e)
        {
            _browser.Load("http://www.google.de");
        }
    }
}
