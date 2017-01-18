using System.Threading.Tasks;
using AnimeViewer.Models;
using AnimeViewer.Services;
using SQLite.Net.Async;
using Xamarin.Forms;

namespace AnimeViewer
{
    public class Database
    {
        /// <summary>
        ///     Returns a connection to the database
        /// </summary>
        /// <returns></returns>
        public static async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            // Get our platform specific implementation (AnimeViewer.iOS.Services.DatabaseService, AnimeViewer.Droid.Services.DatabaseService)
            var conn = DependencyService.Get<IDatabaseService>().GetConnection();
            // Create the tables for each of our models for CRUD operations
            await conn.CreateTablesAsync(
                typeof(Episode),
                typeof(Anime)
            );

            return conn;
        }
    }
}