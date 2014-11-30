using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Core.Model;

namespace Mango.Core.Database
{
    public interface IMangaDatabase
    {
        string Name
        {
            get;
        }

        string SiteUrl
        {
            get;
        }

        Task<List<Manga>> Search(string keyword);
    }
}
