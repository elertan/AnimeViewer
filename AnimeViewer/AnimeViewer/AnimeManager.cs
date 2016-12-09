using System.Threading.Tasks;
using KissAnime;

namespace AnimeViewer
{
    public class AnimeManager
    {
        public static AnimeManager Instance { get; set; }

        public static async Task InitializeAsync()
        {
            if (!Api.HasInitialized)
            {
                // Stuff
            }

            Instance = new AnimeManager();
        }
    }
}