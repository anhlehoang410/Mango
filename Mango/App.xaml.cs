using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Mango.Core.Database;
using Mango.Core.Model;
using System.Runtime.CompilerServices;
using Mango.GUI;

namespace Mango
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow Window;
        public static bool CacheImages
        {
            get
            {
                return Mango.Properties.Settings.Default.cache_images;
            }
        }

        public static bool CachePages
        {
            get
            {
                return Mango.Properties.Settings.Default.cache_pages;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            foreach (Manga m in MangaList.List)
            {
                if (m.IsDownloading)
                {
                    m.CancelDownload();
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!IsNet45OrNewer())
            {
                var window = new Core.WrongNet();
                window.Show();
            }
            else
            {
                LoadMainWindow();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void LoadMainWindow()
        {
            MangaList.Load();
            Window = new MainWindow();
            Window.Show();
        }



        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MangaList.Load();
        }

        public static bool IsNet45OrNewer()
        {
            // Class "ReflectionContext" exists from .NET 4.5 onwards.
            return Type.GetType("System.Reflection.ReflectionContext", false) != null;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

        }
    }
}
