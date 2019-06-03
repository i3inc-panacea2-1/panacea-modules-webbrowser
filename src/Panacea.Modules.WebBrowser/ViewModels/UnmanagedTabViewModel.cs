using Panacea.Controls;
using Panacea.Modularity.WebBrowsing;
using Panacea.Modules.WebBrowser.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.WebBrowser.ViewModels
{
    [View(typeof(UnmanagedTab))]
    class UnmanagedTabViewModel : ViewModelBase
    {
        public IWebView WebView { get; }
        public UnmanagedTabViewModel(IWebView tab)
        {
            WebView = tab;

            BackCommand = new RelayCommand(arg =>
            {
                if (WebView?.CanGoBack == true) WebView?.GoBack();
            },
            arg => WebView?.CanGoBack == true);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            WebView?.Dispose();
        }

        public RelayCommand BackCommand { get; }
    }
}
