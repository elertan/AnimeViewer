using SQLite.Net.Async;

namespace AnimeViewer.Services
{
    /// <summary>
    ///     Handles platform specific database connection
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        ///     Gets a connection to the database
        /// </summary>
        /// <returns></returns>
        SQLiteAsyncConnection GetConnection();

        /// <summary>
        ///     Gets the filepath of the database, where its stored
        /// </summary>
        /// <returns></returns>
        string GetDatabaseFilePath();
    }
}