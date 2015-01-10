using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Core.Database;
using System.Threading;

namespace Mango.Core.Model
{
    public abstract class Manga
    {
        private readonly Dictionary<string, string> _extras = new Dictionary<string, string>();
        public abstract bool HasNext();

        public abstract bool HasPrevious();

        public abstract Task<bool> Next();

        public abstract Task<bool> Previous();

        public abstract void PrepareDisplay();

        public abstract void Display(System.Windows.Controls.Grid displayGrid);

        public abstract System.Windows.Media.ImageSource GetCover();

        public abstract void Download();

        public abstract void CancelDownload(Thread downloadThread);

        public void CancelDownload()
        {
            CancelDownload(null);
        }

        public string GetExtraData(string key)
        {
            return !HasExtraData(key) ? null : _extras[key];
        }

        public bool HasExtraData(string key)
        {
            return _extras.ContainsKey(key);
        }

        public void SetChapterLength(int volume, int chapter, int lastPage)
        {
            SetExtraData("__INTERNAL__volume" + volume + "chapter" + chapter, lastPage.ToString(CultureInfo.InvariantCulture));
        }

        public int GetChapterLength(int volume, int chapter)
        {
            return int.Parse(GetExtraData("__INTERNAL__volume" + volume + "chapter" + chapter) ?? "0");
        }

        public void SetExtraData(string key, string obj)
        {
            if (HasExtraData(key))
                _extras[key] = obj;
            else
                _extras.Add(key, obj);
        }

        public abstract IMangaDatabase DatabaseParent
        {
            get;
        }

        public abstract int CurrentPage
        {
            get;
            internal set;
        }

        public abstract int CurrentChapter
        {
            get;
            internal set;
        }

        public abstract int CurrentVolume
        {
            get;
            internal set;
        }

        public abstract bool UsesVolumes
        {
            get;
            internal set;
        }

        public abstract bool IsDownloaded
        {
            get;
        }

        public abstract bool IsDownloading
        {
            get;
        }

        public abstract bool IsDownloadComplete
        {
            get;
        }
        
        public string Title
        {
            get;
            set;
        }

        public int ChapterCount
        {
            get;
            set;
        }
    }
}
