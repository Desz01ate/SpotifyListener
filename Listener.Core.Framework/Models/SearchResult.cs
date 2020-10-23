using Listener.Core.Framework.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Core.Framework.Models
{
    public class SearchResult
    {
        public SearchResult(string track, SearchType searchType, string uri)
        {
            this.Track = track;
            this.SearchType = SearchType;
            this.Uri = new Uri(uri);
        }

        public string Track { get; }
        public SearchType SearchType { get; }
        public Uri Uri { get; }
    }
}
