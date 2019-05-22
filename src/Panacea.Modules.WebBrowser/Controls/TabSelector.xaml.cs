using Panacea.Modularity.WebBrowsing;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Panacea.Modules.WebBrowser.Controls
{
    /// <summary>
    /// Interaction logic for TabSelector.xaml
    /// </summary>
    public partial class TabSelector : UserControl
    {
        public ICommand AddTabCommand
        {
            get { return (ICommand)GetValue(AddTabCommandProperty); }
            set { SetValue(AddTabCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddTabCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddTabCommandProperty =
            DependencyProperty.Register("AddTabCommand", typeof(ICommand), typeof(TabSelector), new PropertyMetadata(null));

        public ICommand SelectTabCommand
        {
            get { return (ICommand)GetValue(SelectTabCommandProperty); }
            set { SetValue(SelectTabCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectTabCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectTabCommandProperty =
            DependencyProperty.Register("SelectTabCommand", typeof(ICommand), typeof(TabSelector), new PropertyMetadata(null));


        public ObservableCollection<IWebView> Tabs
        {
            get { return (ObservableCollection<IWebView>)GetValue(TabsProperty); }
            set { SetValue(TabsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tabs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabsProperty =
            DependencyProperty.Register("Tabs", typeof(ObservableCollection<IWebView>), typeof(TabSelector), new PropertyMetadata(null, OnTabsChanged));

        private static void OnTabsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as TabSelector;
            if (self == null) return;
            var old = e.OldValue as ObservableCollection<IWebView>;
            if(old!= null)
            {
                old.CollectionChanged -= self.N_CollectionChanged;
            }
            var n = e.NewValue as ObservableCollection<IWebView>;
            if (n != null)
            {
                n.CollectionChanged += self.N_CollectionChanged;
            }
        }

        private void N_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TabsViewModel = new ObservableCollection<WebViewModel>();
            foreach(var view in Tabs)
            {
                TabsViewModel.Add(new WebViewModel(view));
            }
        }

        class WebViewModel:PropertyChangedBase
        {
            private readonly IWebView _view;

            public WebViewModel(IWebView webview)
            {
                _view = webview;
                _view.Navigated += _view_Navigated;
                _view.TitleChanged += _view_TitleChanged;
            }

            private void _view_TitleChanged(object sender, string e)
            {
                OnPropertyChanged("Title");
            }

            private void _view_Navigated(object sender, string e)
            {
                OnPropertyChanged("Url");
                OnPropertyChanged("Image");
            }

            public IWebView WebView { get => _view; }
            public string Title { get => _view.Url == "about:blank" ? "Home" : _view.Title; }

            public string Url { get => _view.Url; }

            public Uri Image {
                get
                {
                    if (Uri.IsWellFormedUriString(_view.Url, UriKind.Absolute))
                    {
                        var left = new Uri(_view.Url).GetLeftPart(UriPartial.Authority);
                        if(string.IsNullOrEmpty(left))
                        {
                            return null;
                        }
                        return new Uri(
                            new Uri(left), "favicon.ico");
                    }
                    return null;
                }
            }
        }


        ObservableCollection<WebViewModel> TabsViewModel
        {
            get { return (ObservableCollection<WebViewModel>)GetValue(TabsViewModelProperty); }
            set { SetValue(TabsViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabsViewModel.  This enables animation, styling, binding, etc...
        static readonly DependencyProperty TabsViewModelProperty =
            DependencyProperty.Register("TabsViewModel", typeof(ObservableCollection<WebViewModel>), typeof(TabSelector), new PropertyMetadata(null));



        public TabSelector()
        {
            InitializeComponent();
        }
    }
}
