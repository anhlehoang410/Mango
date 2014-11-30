using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace ECPLauncher
{
    public class Updater
    {
        public static string GetProgramLocation()
        {

            if (!File.Exists("program.json"))
                return "";

            Program program = JsonConvert.DeserializeObject<Program>(File.ReadAllText("program.json"));

            return program.location;
        }
        public static string GetProgramName()
        {
            if (!File.Exists("program.json"))
                return "";

            Program program = JsonConvert.DeserializeObject<Program>(File.ReadAllText("program.json"));

            return program.name;
        }
        public static bool IsUpdateReady()
        {
            return GetUpdates().Count > 0;
        }

        public static List<Update> GetUpdates()
        {
            if (!File.Exists("program.json"))
                return new List<Update>();

            Program program = JsonConvert.DeserializeObject<Program>(File.ReadAllText("program.json"));

            try
            {
                string json;
                using (WebClient client = new WebClient())
                {
                    json = client.DownloadString(program.infoUrl);
                }

                Updates all_updates = JsonConvert.DeserializeObject<Updates>(json);

                List<Update> updates = new List<Update>();

                foreach (Update update in all_updates.updates)
                {
                    if (update.IsHigherThan(program.version))
                    {
                        updates.Add(update);
                    }
                }

                return updates;
            }
            catch
            {
                return new List<Update>();
            }
        }

        internal static string HigherVersion(string ve, string ve2, int index = 0, bool recurse = true)
        {
            string version1 = ve.Replace("b", ".");
            string version2 = ve2.Replace("b", ".");
            int v1 = int.Parse(version1.Split('.')[index]);
            int v2 = int.Parse(version2.Split('.')[index]);
            if (v1 > v2)
                return version1;
            if (v1 < v2)
                return version2;

            if (version1.Split('.').Length >= index + 2 && version2.Split('.').Length >= index + 2 && recurse)
                return HigherVersion(ve, ve2, index + 1, true);
            else if (version1.Split('.').Length >= index + 2 && version2.Split('.').Length < index + 2)
            {
                if (ve.IndexOf("b") != -1 && ve.IndexOf("b") == -1)
                    return version2;
                else
                    return version1; //Assume version1 is higher than version2
            }
            else if (version1.Split('.').Length < index + 2 && version2.Split('.').Length >= index + 2)
            {
                if (ve.IndexOf("b") == -1 && ve2.IndexOf("b") != -1)
                    return version1;
                else
                    return version2; //Assume version2 is higher than version1
            }
            return "";
        }

        struct Program
        {
            public string location;
            public string name;
            public string infoUrl;
            public string website;
            public string version;
        }

        struct Updates
        {
            public Update[] updates;
        }
    }

    public class Update
    {
        public string uid;
        public string version;
        public string download_url;
        public string update_type;

        public bool IsHigherThan(Update update)
        {
            return Updater.HigherVersion(version, update.version) == version;
        }

        public bool IsHigherThan(string version)
        {
            return Updater.HigherVersion(this.version, version) == this.version;
        }

        public void PerformUpdate(Action<int> downloadProgress, Action<int> installProgress, Action completed)
        {
            throw new NotImplementedException();
        }
    }
}
