using System.Threading.Tasks;
using AnimeViewer.Models;
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
            await conn.CreateTablesAsync(
                typeof(Episode),
                typeof(Anime));
            return conn;
        }
    }
}