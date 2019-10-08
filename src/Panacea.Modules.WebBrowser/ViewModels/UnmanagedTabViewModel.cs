using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.Keyboard;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.WebBrowsing;
using Panacea.Modules.WebBrowser.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Panacea.Modules.WebBrowser.ViewModels
{
    [View(typeof(UnmanagedTab))]
    class UnmanagedTabViewModel : ViewModelBase
    {
        IWebView _webView;
        private readonly PanaceaServices _core;
        private readonly bool _blockDomains;
        private readonly List<string> _whiteList;

        public IWebView WebView
        {
            get => _webView;
            set
            {
                _webView = value;
                OnPropertyChanged();
            }
        }

        public UnmanagedTabViewModel(IWebView tab, PanaceaServices core, bool blockDomains, List<string> whiteList)
        {
            _core = core;
            _blockDomains = blockDomains;
            _whiteList = whiteList;
            WebView = tab;
            WebView.Navigating += WebView_Navigating;
            WebView.CanGoBackChanged += WebView_CanGoBackChanged;
            WebView.FullscreenChanged += WebView_FullscreenChanged;
            WebView.ElementFocus += WebView_ElementFocus;
            WebView.ElementLostFocus += WebView_ElementLostFocus;
            WebView.Close += WebView_Close;
            BackCommand = new RelayCommand(arg =>
            {
                if (WebView?.CanGoBack == true) WebView?.GoBack();
            },
            arg => WebView?.CanGoBack == true);
        }

        private void WebView_Navigating(object sender, NavigatingEventArgs e)
        {
            if (_blockDomains)
            {
                if(!_whiteList.Any(w=> new Uri(e.Url).Host.ToLower().Contains(w.ToLower())))
                {
                    e.Cancel = true;
                    _core.Logger.Warn(this, "Blocked url: " + e.Url);
                }
            }
        }

        private void WebView_ElementLostFocus(object sender, EventArgs e)
        {
            if (_core.TryGetKeyboard(out IKeyboardPlugin keyboard))
            {
                keyboard.HideKeyboard();
            }
        }

        private void WebView_ElementFocus(object sender, string e)
        {
            if (_core.TryGetKeyboard(out IKeyboardPlugin keyboard))
            {
                keyboard.ShowKeyboard(KeyboardType.Normal);
            }
        }

        private void WebView_Close(object sender, EventArgs e)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                if (ui.CurrentPage == this)
                {
                    ui.GoBack();
                }
            }
        }

        private void WebView_FullscreenChanged(object sender, bool e)
        {
            if (e)
            {
                var w = new Window()
                {
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None,
                    Background = Brushes.Black,
                    ResizeMode = ResizeMode.NoResize,
                    WindowState = WindowState.Maximized,
                    Content = WebView,
                    Topmost = true
                };
                WebView = null;
                w.Closed += W_Closed;
                w.Show();
            }
            else
            {
                var w = Window.GetWindow(sender as FrameworkElement);
                if (w != null)
                {
                    w.Close();
                }
            }
        }

        private void W_Closed(object sender, EventArgs e)
        {
            var w = sender as Window;
            var c = w.Content;
            w.Content = null;
            if (WebView == null)
            {
                WebView = c as IWebView;
            }
        }

        private void WebView_CanGoBackChanged(object sender, bool e)
        {
            BackCommand?.RaiseCanExecuteChanged();
        }

        public override void Deactivate()
        {
            WebView.FullscreenChanged -= WebView_FullscreenChanged;
            WebView.CanGoBackChanged -= WebView_CanGoBackChanged;
            base.Deactivate();
            WebView?.Dispose();
        }

        public RelayCommand BackCommand { get; }
    }
}
