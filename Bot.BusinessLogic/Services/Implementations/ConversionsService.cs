using Bot.BusinessLogic.Services.Contracts;
using Bot.Common.Models;
using Bot.Model;
using Bot.Model.DatabaseModels;
using System;
using VideoLibrary;
using MediaToolkit;
using MediaToolkit.Model;
using System.Runtime.Serialization;

namespace Bot.BusinessLogic.Services.Implementations
{
    public class ConversionsService : IConversionsService
    {
        private static readonly string _pathToFolder = Environment.CurrentDirectory + @"\ConvertedMp3\";

        public ConversionsService() { }

        //TODO Complete uri check if video exists
        public string Convert(string url, long userId, string userName)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url), "Неверная ссылка");
            }

            DeleteMusicFromLocalRep();

            DirectoryInfo dirInfo = new DirectoryInfo(_pathToFolder);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nGetting video from youtube...");
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(url);
            Console.WriteLine("Success");

            if (vid is null)
            {
                throw new ArgumentException("Video was null");
            }

            Console.Write("Normalizing video's name...");
            string videoName = NormalizeString(vid.FullName);
            string videoTitle = videoName.Substring(0, videoName.LastIndexOf(".mp4"));
            Console.WriteLine("Success");

            File.WriteAllBytes(Path.Combine(_pathToFolder, videoName), vid.GetBytes());

            Console.Write("Saving video...");
            var inputFile = new MediaFile { Filename = Path.Combine(_pathToFolder, videoName) };
            var outputFile = new MediaFile { Filename = Path.Combine(_pathToFolder, $"{videoTitle}.mp3") };
            Console.WriteLine("Success");

            Console.Write("Converting video to .mp3 file...");
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }
            Console.WriteLine("Success");

            if (File.Exists(outputFile.Filename))
            {
                Console.WriteLine("Adding data to database...");
                CreateDbNote(new Conversion { UserId = userId, YtLink = url, Username = userName });
            }

            Console.ResetColor();

            return $"{outputFile.Filename}";
        }

        // Deletes music from local repository
        public static void DeleteMusicFromLocalRep()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(_pathToFolder);

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
