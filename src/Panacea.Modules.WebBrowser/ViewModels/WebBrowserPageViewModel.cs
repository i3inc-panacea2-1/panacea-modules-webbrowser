using Panacea.Controls;
using Panacea.Modularity.WebBrowsing;
using Panacea.Modules.WebBrowser.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Panacea.Modules.WebBrowser.ViewModels
{
    [View(typeof(WebBrowserPage))]
    public class WebBrowserPageViewModel : ViewModelBase
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

        private bool _hasInvalidCertificate;
        public bool HasInvalidCertificate
        {
            get => _hasInvalidCertificate;
            set
            {
                _hasInvalidCertificate = value;
                OnPropertyChanged();
            }
        }

        public WebBrowserPageViewModel(IWebViewManager webViewManager)
        {
            _webViewManager = webViewManager;
            NavigateCommand = new RelayCommand((args) =>
            {
                if (CurrentWebView == null)
                {
                    CreateTab();
                }
                var url = args.ToString();
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    var uri = new Uri(url);

                    if (uri.Scheme != "https" && uri.Scheme != "http" && uri.Scheme != "javascript" && !uri.ToString().StartsWith("about:blank"))
                    {
                        //_window.ThemeManager.Toast(new Translator("WebBrowser").Translate("Access denied"));
                        return;
                    }
                    CurrentWebView?.Navigate(url);
                }
                else if (url.Contains(".") &&
                         Uri.IsWellFormedUriString("http://" + url, UriKind.Absolute))
                {
                    CurrentWebView?.Navigate(url);
                }
                else
                {
                    CurrentWebView?.Navigate("https://www.google.com/search?q=" + url);
                }

                Keyboard.ClearFocus();
            });
        }

        void CreateTab()
        {
            var view = _webViewManager.CreateTab();
            Tabs.Add(view);
            SwitchToTab(view);
            
        }

        void SwitchToTab(IWebView webView)
        {
            AttachToWebView(webView);
            CurrentWebView = webView;
        }

        void AttachToWebView(IWebView webview)
        {
            webview.HasInvalidCertificateChanged += Webview_HasInvalidCertificateChanged;
        }

        private void Webview_HasInvalidCertificateChanged(object sender, bool e)
        {
            if(CurrentWebView == sender)
            {
                HasInvalidCertificate = e;
            }
        }

        public override void Activate()
        {
            if (CurrentWebView == null)
            {
                CreateTab();
            }
        }

        public override void Deactivate()
        {
            var tabs = Tabs.ToList();
            Tabs.Clear();
            foreach (var tab in tabs)
            {
                tab.Dispose();
            }

        }

        public RelayCommand NavigateCommand { get; }
    }
}
