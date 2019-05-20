using Panacea.Models;
using Panacea.Multilinguality;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Panacea.Modules.WebBrowser.Models
{
    [DataContract]
    public class Link : ServerItem
    {
        public static event EventHandler FavoritesChanged;

        static void OnFavoritesChanged()
        {
            var handler = FavoritesChanged;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        private static List<ServerItem> _favorites;

        public static List<ServerItem> Favorites
        {
            get { return _favorites; }
            set
            {
                _favorites = value;
                OnFavoritesChanged();
            }
        }


        [DataMember(Name = "dataUrl")]
        public string DataUrl { get; set; }

        [IsTranslatable]
        [DataMember(Name = "description")]
        public string Description
        {
            get => GetTranslation();
            set => SetTranslation(value);
        }

        public Brush Background
        {
            get
            {
                if (IsFavorite) return (Brush)Application.Current.FindResource("ColorError");
                return (Brush)Application.Current.FindResource("ColorInformation");
            }
        }


        public bool IsFavorite
        {
            get
            {
                if (Favorites == null) return false;
                return Favorites.Any(l => l.Id == this.Id);
            }
        }

        public void AddToFavorites()
        {
            Favorites.Add(this);
            OnPropertyChanged("IsFavorite");
            OnPropertyChanged("Background");
        }

        public void RemoveFromFavorites()
        {
            if (Favorites.Any(l => l.Id == this.Id))
                Favorites.Remove(Favorites.First(l => l.Id == this.Id));
            OnPropertyChanged("IsFavorite");
            OnPropertyChanged("Background");
        }
    }

    [DataContract]
    public class HistoryItem
    {
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Url")]
        public string Url { get; set; }

        [DataMember(Name = "Count")]
        public int Count { get; set; }
    }

    [DataContract]
    public class History : PropertyChangedBase
    {
        //private object sorted;

        public History()
        {
            Items = new ObservableCollection<HistoryItem>();
        }

        public List<HistoryItem> SortedItems
        {
            get { return Items.OrderBy(i => i.Count).ToList(); }
            set { }
        }

        [DataMember(Name = "Items")]
        public ObservableCollection<HistoryItem> Items { get; set; }


        public void AddOne(string url, string title)
        {
            if (Items.Any(i => i.Url == url))
            {
                Items.First(i => i.Url == url).Count++;
                var name = Items.First(i => i.Url == url).Name;
                if (name != null && name != title) Items.First(i => i.Url == url).Name = title;
            }
            else
            {
                Items.Add(new HistoryItem { Name = title, Url = url, Count = 1 });
            }
            OnPropertyChanged("SortedItems");
        }
    }
}
