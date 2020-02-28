using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener.DatabaseManager
{
    public partial class Service
    {
        private static Lazy<Service> _context = new Lazy<Service>(() => new Service(), true);
        public static Service Context => _context.Value;
        public Service()
        {
            this.Connector = new SQLite();
        }
    }
}
