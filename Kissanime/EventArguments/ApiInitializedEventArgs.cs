using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KissAnime.EventArguments
{
    public class ApiInitializedEventArgs : EventArgs
    {
        public CookieContainer CookieContainer { get; set; }
    }
}
