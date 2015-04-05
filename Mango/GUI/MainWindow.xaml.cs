using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Mango.Core.Database;
using Mango.Core.Database.Impl;
using Mango.Core.GUI;
using Mango.Core.Model;

namespace Mango.GUI
{
    public partial class MainWindow
    {
        public class Request
        {
            public Mango.Core.Model.Manga manga;
            public MangaBox box;
        }
        private static readonly IMangaDatabase[] DATABASE = new IMangaDatabase[] {
            new MangaReaderDatabase(),
            new MangaHereDatabase()
        };

        private bool searching;
        public MainWindow()
        {
            InitializeComponent();

            settingsPane.IsOpenChanged += delegate
            {
                Properties.Settings.Default.Save();
            };
            this.Title = "Mango";
            Loader.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MangaList.List.Count == 0)
            {
                Content.Children.Add(FragmentHelper.ExtractUI<NoManga>());
            }
            else
            {
                var scrollViewer = new ScrollViewer();
                scrollViewer.Background = new SolidColorBrush(Color.FromRgb(249,249,249));
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollViewer.Padding = new Thickness(10);
                scrollViewer.Height = 502;
                var grid = new UniformGrid();
                grid.Rows = 1;
                scrollViewer.Content = grid;
                Content.Children.Add(scrollViewer);
                foreach (Manga m in MangaList.List)
                {
                    AddMangaTile(m, grid);
                }
                new Thread(new ThreadStart(delegate
                {
                    foreach (Request request in this.request)
                    {
                        ImageSource img = request.manga.GetCover();
                        Dispatcher.BeginInvoke(new Action(delegate
                        {
                            request.box.MangaCover = img;
                        }));
                        Thread.Sleep(10);
                    }
                })).Start();
                
            }
        }

        private void AddMangaTile(Manga m, UniformGrid parentGrid)
        {
            MangaBox box = new MangaBox();
            box.MangaTitle = m.Title;
            box.DatabaseText = (m.DatabaseParent == null ? "Local" : m.DatabaseParent.Name);

            Grid grid = FragmentHelper.ExtractUI<Grid>(box);
            Tile tile = new Tile();
            tile.Width = grid.Width;
            tile.Height = grid.Height;
            tile.Content = grid;
            tile.Click += delegate
            {
                Reader read = new Reader(m);
                read.Show();
                this.Hide();
            };
            parentGrid.Children.Add(tile);

            Request request = new Request();
            request.manga = m;
            request.box = box;
            this.request.Add(request);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            cPages.IsChecked = App.CachePages;
            cManga.IsChecked = App.CacheImages;
            settingsPane.IsOpen = !settingsPane.IsOpen;
        }

        private void cManga_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.cache_images = (bool)cManga.IsChecked;
        }

        private void cPages_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.cache_pages = (bool)cPages.IsChecked;
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!searching)
            {
                searching = true;
                SearchBtn.Content = "Cancel";
                Loader.Visibility = System.Windows.Visibility.Visible;
                SearchForManga();
            }
            else
            {
                SearchBtn.Content = "Search";
                Loader.Visibility = System.Windows.Visibility.Hidden;
                searching = false;
            }
        }

        List<Request> request = new List<Request>();
        private void AddMangaSearchTile(Manga m, ImageSource img)
        {
            MangaBox box = new MangaBox();
            box.MangaTitle = m.Title;
            box.DatabaseText = (m.DatabaseParent == null ? "Local" : m.DatabaseParent.Name);

            Grid grid = FragmentHelper.ExtractUI<Grid>(box);
            Tile tile = new Tile();
            tile.Width = grid.Width;
            tile.Height = grid.Height;
            tile.Content = grid;
            tile.Click += delegate
            {
                Reader read = new Reader(m);
                read.Show();
                this.Hide();
            };
            Tiles.Children.Add(tile);

            box.MangaCover = img;
        }

        private async void SearchForManga()
        {
            Tiles.Children.Clear();
            ScrollView.Content = Tiles;

            List<Manga> mangas = new List<Manga>();
            List<Manga> lastResult = null;
            foreach (IMangaDatabase db in DATABASE)
            {
                if (!searching)
                    break;
                Task<List<Manga>> result = db.Search(SearchBox.Text);
                if (lastResult != null)
                {
                    foreach (Manga m in lastResult)
                    {
                        ImageSource img = m.GetCover();
                        Dispatcher.BeginInvoke(new Action(delegate
                        {
                            AddMangaSearchTile(m, img);
                        }));
                    }
                }
                await result;
                lastResult = result.Result;
            }

            if (lastResult != null)
            {
                foreach (Manga m in lastResult)
                {
                    ImageSource img = m.GetCover();
                    await Dispatcher.BeginInvoke(new Action(delegate
                    {
                        AddMangaSearchTile(m, img);
                    }));
                }
            }

            if (Tiles.Children.Count == 0)
            {
                UIElement element = FragmentHelper.ExtractUI<NoMangaFound>();
                ScrollView.Content = element;
            }

            Dispatcher.Invoke(new Action(delegate
            {
                SearchBtn.Content = "Search";
                Loader.Visibility = System.Windows.Visibility.Hidden;
                searching = false;
            }));
        }
    }
}
