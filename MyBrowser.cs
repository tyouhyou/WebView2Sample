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

        private event EventHandler<Action> AwaitInitialization;

        InitStatus status { set; get; } = InitStatus.UnInit;

        private Task task { set; get; } = null;

        public MyBrowser()
        {
            status = InitStatus.UnInit;
            AwaitInitialization += HandleInitAwait;
        }

        private void HandleInitAwait(object sender, Action act)
        {
            if (null == SynchronizationContext.Current) throw new InvalidOperationException("Should be on UI thread");
            SynchronizationContext.Current.Post(async (_) =>
            {
                await task;
                if (!task.IsCompleted) return;  // TODO: log
                act();
            }, null);
        }

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
            if (!e.IsSuccess)
            {
                status = InitStatus.UnInit;
                return;
            }
            status = InitStatus.Inited;
        }

        public void Navigate(string url)
        {
            if (AwaitInit(() => Navigate(url))) return;
            CoreWebView2.Navigate(url);
        }

        public Uri Url
        {
            set
            {
                if (value == null) return;
                if (AwaitInit(() => Url = value)) return;
                Source = value;
            }

            get => Source;
        }

        private bool AwaitInit(Action act)
        {
            if (InitStatus.UnInit == status)
            {
                throw new InvalidOperationException("Call Initialize() first, before using browser's function.");
            }

            if (InitStatus.Inited == status)
            {
                return false;
            }

            AwaitInitialization?.Invoke(this, act);
            return true;
        }
    }
}
