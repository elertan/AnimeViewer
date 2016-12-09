using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimeViewer.Services;
using SQLite.Net.Async;
using Xamarin.Forms;

namespace AnimeViewer
{
    public class Database
    {
        public static async Task<SQLiteAsyncConnection> GetConnection()
        {
            var conn = DependencyService.Get<IDatabaseService>().GetConnection();
            return conn;
        }
    }
}
