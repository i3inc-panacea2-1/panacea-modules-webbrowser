using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.Billing;
using Panacea.Modularity.Favorites;
using Panacea.Modularity.WebBrowsing;
using Panacea.Modules.WebBrowser.Models;
using Panacea.Modules.WebBrowser.Views;
using Panacea.Multilinguality;
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
        public WebBrowserPageViewModel(IWebViewManager webViewManager, LinkItemProvider provider, WebBrowserPlugin plugin, PanaceaServices core)
        {
            _webViewManager = webViewManager;
            ItemProvider = provider;
            ItemClickCommand = new RelayCommand(args =>
            {
                var link = args as Link;
                NavigateCommand?.Execute(link.DataUrl);
            });
            IsFavoriteCommand = new RelayCommand((arg) =>
            {
            },
            (arg) =>
            {
                var link = arg as Link;
                if (plugin.Favorites == null) return false;
                return plugin.Favorites.Any(l => l.Id == link.Id);
            });
            FavoriteCommand = new AsyncCommand(async (args) =>
            {
                var game = args as Link;
                if (game == null) return;
                if (core.TryGetFavoritesPlugin(out IFavoritesManager _favoritesManager))
                {
                    try
                    {
                        if (await _favoritesManager.AddOrRemoveFavoriteAsync("WebBrowser", game))
                            OnPropertyChanged(nameof(IsFavoriteCommand));
                    }
                    catch (Exception e)
                    {
                        core.Logger.Error(this, e.Message);
                    }
                }
            });
            NavigateCommand = new RelayCommand(async (args) =>
            {
                if (CurrentWebView == null)
                {
                    CreateTab();
                }
                var url = args.ToString();
                if (url.ToLower() == "about:blank")
                {
                    WebViewContainerVisibility = Visibility.Collapsed;
                    CurrentWebView?.Navigate(url);
                    return;
                }
                if (core.TryGetBilling(out IBillingManager bill))
                {
                    var service = await bill.GetOrRequestServiceAsync(new Translator("WebBrowser").Translate("Web browser requires service."), "WebBrowser");
                    if (service == null) return;

                }
                if (CurrentWebView == null)
                {
                    CreateTab();
                }
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    var uri = new Uri(url);

                    if (uri.Scheme != "https" && uri.Scheme != "http" && uri.Scheme != "javascript" && !uri.ToString().StartsWith("about:blank"))
                    {
                        //_window.ThemeManager.Toast(new Translator("WebBrowser").Translate("Access denied"));
                        return;
                    }
                    WebViewContainerVisibility = Visibility.Visible;
                    CurrentWebView?.Navigate(url);
                }
                else if (url.Contains(".") &&
                         Uri.IsWellFormedUriString("http://" + url, UriKind.Absolute))
                {
                    WebViewContainerVisibility = Visibility.Visible;
                    CurrentWebView?.Navigate(url);
                }
                else
                {
                    WebViewContainerVisibility = Visibility.Visible;
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

        Visibility _webViewContainerVisibility = Visibility.Collapsed;
        public Visibility WebViewContainerVisibility
        {
            get => _webViewContainerVisibility;
            set
            {
                _webViewContainerVisibility = value;
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

        public LinkItemProvider ItemProvider { get; }



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
            CurrentWebView = null;

        }

        public RelayCommand NavigateCommand { get; }

        public RelayCommand BackCommand { get; }

        public RelayCommand ForwardCommand { get; }

        public RelayCommand SwitchTabSelectorCommand { get; }

        public RelayCommand SelectTabCommand { get; }

        public RelayCommand AddTabCommand { get; }

        public ICommand ItemClickCommand { get; }

        public ICommand IsFavoriteCommand { get; }

        public AsyncCommand FavoriteCommand { get; }
    }
}
