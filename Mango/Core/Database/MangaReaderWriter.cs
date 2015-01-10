using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ionic.Zip;
using Mango.Core.Model;

namespace Mango.Core.Database
{
    public class MangaReaderWriter
    {
        public static void EnsureSaveExists(Manga manga)
        {
            string file = MakeValidFileName(manga.Title + ".manga");
            if (File.Exists("mangas/" + file)) return;
            using (var zip = new ZipFile())
            {
                zip.AddEntry(".keep", new byte[1]);
                zip.Save("mangas/" + file);
            }
        }

        public static void SaveImageInManga(Manga manga, int volume, int chapter, int page, byte[] data)
        {
            EnsureSaveExists(manga);

            string file = MakeValidFileName(manga.Title + ".manga");
            using (var zip = new ZipFile("mangas/" + file))
            {
                zip.AddEntry(volume + "-" + chapter + "-" + page, data);
                zip.Save();
            }
        }

        public static bool MangaFileExists(Manga manga)
        {
            string file = MakeValidFileName(manga.Title + ".manga");
            return File.Exists("mangas/" + file);
        }

        public static byte[] GetMangaPage(Manga manga, int volume, int chapter, int page)
        {
            if (!MangaFileExists(manga)) return new byte[0];
            string file = MakeValidFileName(manga.Title + ".manga");

            var toReturn = new byte[0];
            using (var stream = new MemoryStream())
            {
                using (var zip = new ZipFile("mangas/" + file))
                {
                    ZipEntry entry;
                    if ((entry = zip["" + volume + "-" + chapter + "-" + page]) != null)
                    {
                        entry.Extract(stream);
                        toReturn = stream.ToArray();
                    }
                }
            }

            return toReturn;
        }

        public static int GetPageCountForChapter(Manga manga, int volume, int chapter)
        {
            if (!MangaFileExists(manga)) return 0;
            string file = MakeValidFileName(manga.Title + ".manga");
            int count = 0;
            var regex = new Regex(volume + "-" + chapter + "-[0-9]+");

            using (var zip = new ZipFile("mangas/" + file))
            {
                count = zip.Entries.Count(e => regex.IsMatch(e.FileName));
            }

            return count;
        }

        public static int GetChapterCountForVolume(Manga manga, int volume)
        {
            if (!MangaFileExists(manga)) return 0;
            string file = MakeValidFileName(manga.Title + ".manga");
            int count = 0;
            var regex = new Regex(volume + "-" + "-[0-9]+");

            using (var zip = new ZipFile("mangas/" + file))
            {
                count = zip.Entries.Count(e => regex.IsMatch(e.FileName));
            }

            return count;
        }

        public static void MarkCompleted(Manga manga)
        {
            if (MangaFileExists(manga))
            {
                string file = MakeValidFileName(manga.Title + ".manga");
                using (var zip = new ZipFile("mangas/" + file))
                {
                    zip.AddEntry(".completed", new byte[1]);
                    zip.Save();
                }
            }
        }

        public static bool IsCompleted(Manga manga)
        {
            if (MangaFileExists(manga))
            {
                string file = MakeValidFileName(manga.Title + ".manga");
                int count;
                using (var zip = new ZipFile("mangas/" + file))
                {
                    count = zip.Entries.Count(e => e.FileName == ".completed");
                }

                return count > 0;
            }
            return false;
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
