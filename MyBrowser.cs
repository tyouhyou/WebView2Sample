using Microsoft.Web.WebView2.WinForms;
using System;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace WebView2Sample
{
    internal class MyBrowser : WebView2
    {
        private enum InitStatus
        {
            UnInit = 0,
            Initing,
            Inited
        }

        InitStatus status { set; get; } = InitStatus.UnInit;

        private Task task { set; get; } = null;

        public void Initialize()
        {
            status = InitStatus.Initing;
            CoreWebView2InitializationCompleted += MyBrowser_CoreWebView2InitializationCompleted;
            SynchronizationContext.Current.Post((_) => 
            {
                task = EnsureCoreWebView2Async();
            }, null);
        }

        private void MyBrowser_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            CoreWebView2.DOMContentLoaded += (o, args) => { };
            status = InitStatus.Inited;
        }

        public void Navigate(string url)
        {
            Await(()=> _Navigate(url));
        }
        private void _Navigate(string url)
        {
            CoreWebView2.Navigate(url);
        }

        public Uri Url
        {
            set => Await(() => Source = value);
            get => Source;
        }

        private void Await(Action act)
        {
            if (InitStatus.UnInit == status)
            {
                return;
            }

            if (InitStatus.Inited == status)
            {
                act();
                return;
            }

            if (null == SynchronizationContext.Current) throw new InvalidOperationException("Should be on UI thread");
            SynchronizationContext.Current.Post(async (_) => 
            {
                await task;
                if (!task.IsCompleted) return;  // TODO: log
                act(); 
            }, null);
        }
    }
}
