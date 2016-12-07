using System;
using System.IO;
using AnimeViewer.Droid.Services;
using AnimeViewer.Services;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinAndroid;
using Xamarin.Forms;

[assembly: Dependency(typeof(DatabaseService))]

namespace AnimeViewer.Droid.Services
{
    public class DatabaseService : IDatabaseService
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var filePath = GetDatabaseFilePath();
            var platform = new SQLitePlatformAndroid();
            var connectionWithLock = new SQLiteConnectionWithLock(
                platform,
                new SQLiteConnectionString(filePath, true));
            return new SQLiteAsyncConnection(() => connectionWithLock);
        }

        public string GetDatabaseFilePath()
        {
            const string dbFilename = "database.db3";
            var savingFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(savingFolder, dbFilename);
        }
    }
}