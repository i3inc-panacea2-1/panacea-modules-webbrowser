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
        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Geometry), typeof(AutoCompleteBox), new PropertyMetadata(null));



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


        #endregion deps

        public AutoCompleteBox()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent,
          new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent,
              new RoutedEventHandler(SelectAllText), true);
            Icon = Geometry.Parse("F1 M 36.002, 0.000488281C 52.3457, 0.000488281 66, 13.6509 66, 30.0005L 66, 36.0005L 72.002, 36.0005L 72.002, 96L 0, 96L 0, 36.0005L 6, 36.0005L 6, 30.0005C 6, 13.6509 19.6543, 0.000488281 36.002, 0.000488281 Z M 14.666, 30.646L 14.666, 36.0005L 57.333, 36.0005L 57.333, 30.646C 57.333, 19.7476 46.8955, 9.31299 36.001, 9.31299C 25.1035, 9.31299 14.666, 19.7476 14.666, 30.646 Z M 44, 82.001L 38.5273, 65.5859C 41.7061, 64.5288 44, 61.5352 44, 58.001C 44, 53.582 40.4199, 50.001 36, 50.001C 31.5811, 50.001 28, 53.582 28, 58.001C 28, 61.5352 30.293, 64.5288 33.4717, 65.5859L 28, 82.001L 44, 82.001 Z ");
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
                box.Icon = Geometry.Parse("F1 M 36.002,0.000488281C 52.3457,0.000488281 66,13.6509 66,30.0005L 66,36.0005L 72.002,36.0005L 72.002,96L 0,96L 0,36.0005L 57.334,36.0005L 57.334,30.645C 57.334,19.7466 46.8965,9.31201 36.002,9.31201C 29.9551,9.31201 24.0566,12.5298 19.9951,17.2524L 12.1367,12.0005C 17.6543,4.74951 26.3604,0.000488281 36.002,0.000488281 Z M 44,82.001L 38.5273,65.5859C 41.7061,64.5288 44,61.5352 44,58.001C 44,53.582 40.4199,50.001 36,50.001C 31.5811,50.001 28,53.582 28,58.001C 28,61.5352 30.293,64.5288 33.4717,65.5859L 28,82.001L 44,82.001 Z ");
                LinearGradientBrush linear = new LinearGradientBrush();
                linear.StartPoint = new Point(0, 0);
                linear.EndPoint = new Point(0, 1);
                linear.SpreadMethod = GradientSpreadMethod.Pad;
                linear.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;

                linear.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 255, 20, 20), 0));
                linear.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 210, 10, 10), 1));
                linear.Freeze();
                box.HttpsBackground = linear;
                var brush = new SolidColorBrush(Color.FromRgb(180, 0, 0));
                brush.Freeze();
                box.HttpsBorderBackground = brush;
            }
            else
            {
                box.Icon = Geometry.Parse("F1 M 36.002, 0.000488281C 52.3457, 0.000488281 66, 13.6509 66, 30.0005L 66, 36.0005L 72.002, 36.0005L 72.002, 96L 0, 96L 0, 36.0005L 6, 36.0005L 6, 30.0005C 6, 13.6509 19.6543, 0.000488281 36.002, 0.000488281 Z M 14.666, 30.646L 14.666, 36.0005L 57.333, 36.0005L 57.333, 30.646C 57.333, 19.7476 46.8955, 9.31299 36.001, 9.31299C 25.1035, 9.31299 14.666, 19.7476 14.666, 30.646 Z M 44, 82.001L 38.5273, 65.5859C 41.7061, 64.5288 44, 61.5352 44, 58.001C 44, 53.582 40.4199, 50.001 36, 50.001C 31.5811, 50.001 28, 53.582 28, 58.001C 28, 61.5352 30.293, 64.5288 33.4717, 65.5859L 28, 82.001L 44, 82.001 Z ");
                LinearGradientBrush linear = new LinearGradientBrush();
                linear.StartPoint = new Point(0, 0);
                linear.EndPoint = new Point(0, 1);
                linear.SpreadMethod = GradientSpreadMethod.Pad;
                linear.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;

                linear.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 7, 230, 138), 0));
                linear.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 6, 200, 120), 1));
                linear.Freeze();

                box.HttpsBackground = linear;
                var brush = new SolidColorBrush(Color.FromRgb(1, 180, 120));
                brush.Freeze();
                box.HttpsBorderBackground = brush;
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
                                Foreground = Brushes.White,
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
            History.Remove(Text);
            History.Insert(0, Text);
            var h = ReturnPress;
            h?.Invoke(this, new EventArgs());
        }
        TextBox txt;
        private RichTextBox richtxt;
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
            var cts = new CancellationTokenSource();
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
