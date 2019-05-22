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
using System.Windows;
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

        private Visibility _tabSelectorVisibility = Visibility.Collapsed;
        public Visibility TabSelectorVisibility
        {
            get => _tabSelectorVisibility;
            set
            {
                _tabSelectorVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _mainUiVisibility = Visibility.Visible;
        public Visibility MainUiVisibility
        {
            get => _mainUiVisibility;
            set
            {
                _mainUiVisibility = value;
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

            BackCommand = new RelayCommand(args =>
            {
                CurrentWebView?.GoBack();
            },
            args =>
            {
                return CurrentWebView?.CanGoBack == true;
            });
            ForwardCommand = new RelayCommand(args =>
            {
                CurrentWebView?.GoForward();
            },
            args =>
            {
                return CurrentWebView?.CanGoForward == true;
            });
            SwitchTabSelectorCommand = new RelayCommand((args) =>
            {
                ShowTabSelector();
            });
            SelectTabCommand = new RelayCommand((args) =>
            {
                SwitchToTab(args as IWebView);
            });
            AddTabCommand = new RelayCommand((args) =>
            {
                CreateTab();
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
            ShowMainUi();
        }

        void ShowMainUi()
        {
            MainUiVisibility = Visibility.Visible;
            TabSelectorVisibility = Visibility.Collapsed;
        }

        void ShowTabSelector()
        {
            MainUiVisibility = Visibility.Collapsed;
            TabSelectorVisibility = Visibility.Visible;
        }

        void AttachToWebView(IWebView webview)
        {
            webview.HasInvalidCertificateChanged += Webview_HasInvalidCertificateChanged;
            webview.CanGoBackChanged += Webview_CanGoBackChanged;
            webview.CanGoForwardChanged += Webview_CanGoForwardChanged;
        }

        private void Webview_CanGoForwardChanged(object sender, bool e)
        {
            ForwardCommand.RaiseCanExecuteChanged();
        }

        private void Webview_CanGoBackChanged(object sender, bool e)
        {
            BackCommand.RaiseCanExecuteChanged();
        }

        private void Webview_HasInvalidCertificateChanged(object sender, bool e)
        {
            if (CurrentWebView == sender)
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

        public RelayCommand BackCommand { get; }

        public RelayCommand ForwardCommand { get; }

        public RelayCommand SwitchTabSelectorCommand { get; }

        public RelayCommand SelectTabCommand { get; }

        public RelayCommand AddTabCommand { get; }
    }
}
