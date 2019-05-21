using Panacea.Modularity.WebBrowsing;
using Panacea.Modules.WebBrowser.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.WebBrowser.ViewModels
{
    [View(typeof(WebBrowserPage))]
    public class WebBrowserPageViewModel:ViewModelBase
    {
        private readonly IWebViewManager _webViewManager;

        private IWebView _currentWebView;
        public IWebView CurrentWebView
        {
            get => _currentWebView;
            set
            {
                _currentWebView = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IWebView> _tabs = new ObservableCollection<IWebView>();
        public ObservableCollection<IWebView> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = value;
                OnPropertyChanged();
            }
        }

        public WebBrowserPageViewModel(IWebViewManager webViewManager)
        {
            _webViewManager = webViewManager;
        }

        public override void Activate()
        {
            if(CurrentWebView == null)
            {
                CurrentWebView = _webViewManager.CreateTab();
                Tabs.Add(CurrentWebView);
            }
        }

        public override void Deactivate()
        {
            var tabs = Tabs.ToList();
            Tabs.Clear();
            foreach(var tab in tabs)
            {
                tab.Dispose();
            }

        }
    }
}
