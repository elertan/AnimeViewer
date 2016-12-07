using System;
using System.IO;
using AnimeViewer.iOS.Services;
using AnimeViewer.Services;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinIOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(DatabaseService))]

namespace AnimeViewer.iOS.Services
{
    public class DatabaseService : IDatabaseService
    {
        public string GetDatabaseFilePath()
        {
            const string dbFilename = "database.db3";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentsPath, "..", "Library");
            return Path.Combine(libraryPath, dbFilename);
        }

        public SQLiteAsyncConnection GetConnection()
        {
            var filePath = GetDatabaseFilePath();
            var platform = new SQLitePlatformIOS();
            var connectionWithLock = new SQLiteConnectionWithLock(
                platform,
                new SQLiteConnectionString(filePath, true));

            return new SQLiteAsyncConnection(() => connectionWithLock);
        }
    }
}