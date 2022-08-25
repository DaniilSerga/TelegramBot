using Bot.BusinessLogic.Services.Contracts;
using Bot.Model;
using Bot.Model.DatabaseModels;
using VideoLibrary;
using MediaToolkit;
using MediaToolkit.Model;

namespace Bot.BusinessLogic.Services.Implementations
{
    public class ConversionsService : IConversionsService
    {
        private static readonly string _pathToFolder = Environment.CurrentDirectory + @"\ConvertedMp3\";

        public ConversionsService() { }

        public async Task<string> Convert(string url, long userId, string userName)
        {
            string path = string.Empty;

            try
            {
                path = await GetAudioFile(url, userId, userName);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            return path;
        }

        // Converting video into /mp3 file
        private static async Task<string> GetAudioFile(string url, long userId, string userName)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url), "Url was null.");
            }

            if (userName is null)
            {
                throw new ArgumentNullException(nameof(userName), "userName was null.");
            }

            await Task.Delay(0);

            DeleteMusicFromLocalRep();

            DirectoryInfo dirInfo = new (_pathToFolder);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            Console.ForegroundColor = ConsoleColor.Green;

            #region Getting Youtube video
            Console.Write("\nGetting video from youtube...");
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(url);
            Console.WriteLine("Success");
            #endregion

            if (vid is null)
            {
                throw new ArgumentException("No video was found.", nameof(url));
            }

            #region Normalizing video name
            Console.Write("Normalizing video's name...");

            string videoName = NormalizeString(vid.FullName);
            string videoTitle = videoName[..videoName.LastIndexOf(".mp4")];

            Console.WriteLine("Success");
            #endregion

            #region Saving video to a local folder
            File.WriteAllBytes(Path.Combine(_pathToFolder, videoName), vid.GetBytes());

            Console.Write("Saving video...");
            var inputFile = new MediaFile { Filename = Path.Combine(_pathToFolder, videoName) };
            var outputFile = new MediaFile { Filename = Path.Combine(_pathToFolder, $"{videoTitle}.mp3") };
            Console.WriteLine("Success");
            #endregion

            #region Converting region
            Console.Write("Converting video to .mp3 file...");
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }
            Console.WriteLine("Success");
            #endregion

            // Checks if audio exists
            if (File.Exists(outputFile.Filename))
            {
                Console.WriteLine("Adding data to database...");
                CreateDbNote(new Conversion { UserId = userId, YtLink = url, Username = userName });
            }

            Console.ResetColor();

            // returns path to the audio
            return outputFile.Filename;
        }

        // Deletes music from local repository
        private static void DeleteMusicFromLocalRep()
        {
            DirectoryInfo dirInfo = new (_pathToFolder);

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                file.Delete();
            }
        }
        
        // Creates note in database
        private static void CreateDbNote(Conversion conversion)
        {
            if (conversion is null)
            {
                throw new ArgumentNullException(nameof(conversion), "conversion variable was null.");
            }

            using ApplicationContext db = new();
            db.Conversions.Add(conversion);
            db.SaveChanges();

            Console.WriteLine("\nNote was saved successfully");
            Console.WriteLine($"{conversion.Id}. {conversion.UserId} - {conversion.Username}: {conversion.YtLink} Date: {conversion.ConversionDate}");
        }

        // Deletes all punctuation chars from file name
        private static string NormalizeString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text), "Text variable is null or empty.");
            }

            char[] charArray = text.ToCharArray();

            // Removes every char containing punctuation, except for extension
            for (int i = 0; i < charArray.Length - 4; i++)
            {
                if (char.IsPunctuation(charArray[i]))
                {
                    charArray[i] = '_';
                }
            }

            return new string(charArray);
        }
    }
}
