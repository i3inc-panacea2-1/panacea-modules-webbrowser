using Panacea.Modules.WebBrowser.Google;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Panacea.Modules.WebBrowser.Controls
{
    public class AutoCompleteBox : Control
    {
        static AutoCompleteBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteBox),
                new FrameworkPropertyMetadata(typeof(AutoCompleteBox)));
        }

        #region deps
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(AutoCompleteBox), new PropertyMetadata("lock"));



        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
            typeof(AutoCompleteBox), new FrameworkPropertyMetadata("", OnTextChanged));

        public Brush HttpsBackground
        {
            get { return (Brush)GetValue(HttpsBackgroundProperty); }
            set { SetValue(HttpsBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HttpsBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HttpsBackgroundProperty =
            DependencyProperty.Register("HttpsBackground", typeof(Brush), typeof(AutoCompleteBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(7, 230, 138))));

        public Brush HttpsBorderBackground
        {
            get { return (Brush)GetValue(HttpsBorderBackgroundProperty); }
            set { SetValue(HttpsBorderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HttpsBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HttpsBorderBackgroundProperty =
            DependencyProperty.Register("HttpsBorderBackground", typeof(Brush), typeof(AutoCompleteBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(7, 230, 138))));

        public Visibility HttpsVisibility
        {
            get { return (Visibility)GetValue(HttpsVisibilityProperty); }
            set { SetValue(HttpsVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HttpsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HttpsVisibilityProperty =
            DependencyProperty.Register("HttpsVisibility", typeof(Visibility), typeof(AutoCompleteBox), new PropertyMetadata(Visibility.Collapsed));

        public Brush HttpsForeground
        {
            get { return (Brush)GetValue(HttpsForegroundProperty); }
            set { SetValue(HttpsForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HttpsForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HttpsForegroundProperty =
            DependencyProperty.Register("HttpsForeground", typeof(Brush), typeof(AutoCompleteBox), new PropertyMetadata(Brushes.White));



        public bool InvalidCertificate
        {
            get { return (bool)GetValue(InvalidCertificateProperty); }
            set { SetValue(InvalidCertificateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InvalidCertificate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InvalidCertificateProperty =
            DependencyProperty.Register("InvalidCertificate", typeof(bool), typeof(AutoCompleteBox), new PropertyMetadata(true, OnInvalidCertificateChanged));



        public string InternalText
        {
            get { return (string)GetValue(InternalTextProperty); }
            set { SetValue(InternalTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalTextProperty =
            DependencyProperty.Register("InternalText", typeof(string), typeof(AutoCompleteBox), new PropertyMetadata(null));



        public ICommand NavigateCommand
        {
            get { return (ICommand)GetValue(NavigateCommandProperty); }
            set { SetValue(NavigateCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigateCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigateCommandProperty =
            DependencyProperty.Register("NavigateCommand", typeof(ICommand), typeof(AutoCompleteBox), new PropertyMetadata(null));


        #endregion deps

        public AutoCompleteBox()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent,
          new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent,
              new RoutedEventHandler(SelectAllText), true);
            
            InvalidCertificate = false;
            History = new List<string>();
        }

        List<string> History { get; set; }

        public new bool IsFocused { get; private set; }


        private static void SelectivelyIgnoreMouseButton(object sender,
                                                     MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focussed, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }

        internal void ClearHistory()
        {
            History.Clear();
        }


        private static void OnInvalidCertificateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as AutoCompleteBox;
            var val = (bool)e.NewValue;
            if (val)
            {
                box.Icon = "lock_open";

                box.HttpsBackground = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                
                box.HttpsBorderBackground = new SolidColorBrush(Color.FromRgb(224, 57, 44));
            }
            else
            {
                box.Icon = "lock";
               
                box.HttpsBackground = new SolidColorBrush(Color.FromRgb(139, 195, 74));
                box.HttpsBorderBackground = new SolidColorBrush(Color.FromRgb(119, 175, 64));
            }
        }


        void ApplyText(string text)
        {
            try
            {
                if (richtxt == null) return;
                txt.Text = text;
                if (!String.IsNullOrEmpty(text))
                {
                    if (text == "about:blank") text = "Home";
                    if (Uri.TryCreate(text, UriKind.Absolute, out Uri ouri))
                    {
                        var uri = new Uri(text);

                        var flow = new FlowDocument
                        {
                            MaxPageWidth = 9999,
                            PageWidth = 9999
                        };

                        var paragraph = new Paragraph();

                        if (uri.Scheme == Uri.UriSchemeHttps)
                        {
                            HttpsVisibility = Visibility.Visible;
                        }
                        else
                        {
                            HttpsVisibility = Visibility.Collapsed;
                        }

                        var domain =
                            new TextBlock()
                            {
                                Text = uri.Host +
                                       (uri.Port != 80 && (uri.Port == 443 && uri.Scheme != Uri.UriSchemeHttps)
                                           ? ":" + uri.Port
                                           : ""),
                                TextWrapping = TextWrapping.NoWrap,
                                Foreground = Brushes.Black,
                                Background = Brushes.Transparent,
                                FontWeight = FontWeights.SemiBold
                            };


                        paragraph.Inlines.Add(domain);

                        var path = new TextBlock() { Text = WebUtility.UrlDecode(uri.PathAndQuery), Foreground = Brushes.Silver, Background = Brushes.Transparent };
                        paragraph.Inlines.Add(path);

                        path = new TextBlock() { Text = WebUtility.UrlDecode(uri.Fragment), Foreground = Brushes.Silver, Background = Brushes.Transparent };
                        paragraph.Inlines.Add(path);

                        flow.Blocks.Add(paragraph);

                        richtxt.Document = flow;
                        richtxt.Document.PageWidth = 99999;
                    }
                    else
                    {
                        var flow = new FlowDocument();
                        var paragraph = new Paragraph();
                        var scheme = new Run(text) { Foreground = Brushes.White };
                        paragraph.Inlines.Add(scheme);
                        flow.Blocks.Add(paragraph);
                        if (richtxt == null) return;
                        richtxt.Document = flow;
                    }

                }
                else
                {
                    richtxt.Document = new FlowDocument();
                }
            }
            catch
            {

            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as AutoCompleteBox;
            box.ApplyText(e.NewValue?.ToString());
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }



        public static readonly DependencyProperty ImageProperty =
           DependencyProperty.Register("Image", typeof(string),
               typeof(AutoCompleteBox), new FrameworkPropertyMetadata(null));

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        private static readonly DependencyProperty FilteredItemsProperty =
            DependencyProperty.Register("FilteredItems", typeof(ObservableCollection<string>),
                typeof(AutoCompleteBox), new FrameworkPropertyMetadata(new ObservableCollection<string>()));

        protected ObservableCollection<string> FilteredItems
        {
            get { return (ObservableCollection<string>)GetValue(FilteredItemsProperty); }
            set { SetValue(FilteredItemsProperty, value); }
        }

        public event EventHandler ReturnPress;

        private void OnReturnPress()
        {
            cts?.Cancel();
            History.Remove(Text);
            History.Insert(0, Text);
            var h = ReturnPress;
            h?.Invoke(this, new EventArgs());
            NavigateCommand?.Execute(txt.Text);
        }

        TextBox txt;
        private RichTextBox richtxt;
        CancellationTokenSource cts;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            txt = this.Template.FindName("PART_textbox", this) as TextBox;
            if (txt == null) return;

            richtxt = this.Template.FindName("FlowDoc", this) as RichTextBox;
            if (richtxt == null) return;

            var border = this.Template.FindName("PART_richborder", this) as Border;
            if (border == null) return;



            var popup = Template.FindName("PART_popup", this) as Popup;
            if (popup == null) return;

            var listbox = Template.FindName("PART_listbox", this) as ListBox;
            if (listbox == null) return;

            var search = new SearchSuggestionsAPI();
            
            Task<List<GoogleSuggestion>> task;

            border.PreviewMouseDown += (oo, ee) =>
            {
                border.Visibility = Visibility.Hidden;
                txt.Visibility = Visibility.Visible;
                txt.Focus();
                ee.Handled = true;
                IsFocused = true;
            };
            var arrows = new List<Key>() { Key.Up, Key.Down };
            txt.PreviewKeyUp += async (oo, ee) =>
            {
                cts?.Cancel();
                if (arrows.Contains(ee.Key))
                {
                    ee.Handled = true;
                    if (ee.Key == Key.Down && listbox.SelectedIndex < listbox.Items.Count - 1) listbox.SelectedIndex += 1;
                    else if (ee.Key == Key.Up && listbox.SelectedIndex > 0) listbox.SelectedIndex -= 1;

                    return;
                }


                if (ee.Key == Key.Back || ee.Key == Key.Tab) return;
                var localcts = new CancellationTokenSource();
                cts = localcts;
                try
                {

                    FilteredItems.Clear();
                    if (History.Any(i => i.StartsWith(txt.Text)))
                    {

                        popup.IsOpen = History.Count(i => i.StartsWith(txt.Text)) != 1;
                        History.Where(i => i.StartsWith(txt.Text)).ToList().ForEach(i =>
                        {
                            if (FilteredItems.Count >= 7) return;
                            FilteredItems.Add(i);
                        });
                        var length = txt.SelectionStart;
                        popup.IsOpen = History.Count(i => i.StartsWith(txt.Text)) != 1 && FilteredItems.Count > 0;
                        txt.Text = History.First(i => i.StartsWith(txt.Text));
                        txt.Select(length, txt.Text.Length - length);

                    }
                    else
                    {
                        popup.IsOpen = false;
                    }
                    await Task.Delay(500, cts.Token);

                    task = search.GetSearchSuggestions(txt.Text, localcts.Token);
                    var list = await task;
                    if (localcts.IsCancellationRequested) return;
                    popup.IsOpen = (list.Count > 0 || FilteredItems.Count > 0);
                    if (FilteredItems.Count == 0 && list.First().Phrase.StartsWith(txt.Text) && FilteredItems.Count == 0)
                    {

                        var length = txt.SelectionStart;
                        txt.Text = list.First().Phrase;
                        txt.Select(length, txt.Text.Length - length);
                    }
                    foreach (var suggestion in list)
                    {
                        if (FilteredItems.Count >= 7) continue;
                        if (FilteredItems.Contains(suggestion.Phrase)) continue;
                        FilteredItems.Add(suggestion.Phrase);
                    }
                }
                catch
                {
                    //ignore
                }

            };
            listbox.SelectionChanged += (oo, ee) =>
            {
                var t = listbox.SelectedItem as string;
                if (t == null) return;
                Text = t;
                txt.CaretIndex = t.Length;
            };
            listbox.PreviewMouseUp += (oo, ee) =>
            {
                popup.IsOpen = false;
                OnReturnPress();
            };

            listbox.PreviewKeyDown += (oo, ee) =>
            {
                if (ee.Key != Key.Return) return;
                popup.IsOpen = false;
                OnReturnPress();
            };

            txt.PreviewKeyDown += (oo, ee) =>
            {
                if (ee.Key != Key.Return) return;
                popup.IsOpen = false;
                OnReturnPress();
            };

            txt.LostKeyboardFocus += (sender, args) =>
            {
                txt.Visibility = Visibility.Hidden;
                border.Visibility = Visibility.Visible;
                IsFocused = false;
            };

            listbox.ItemsSource = FilteredItems;
            ApplyText(Text);
        }

    }
}
