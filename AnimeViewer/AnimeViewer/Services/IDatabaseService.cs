using SQLite.Net.Async;

namespace AnimeViewer.Services
{
    public interface IDatabaseService
    {
        SQLiteAsyncConnection GetConnection();
        string GetDatabaseFilePath();
    }
}