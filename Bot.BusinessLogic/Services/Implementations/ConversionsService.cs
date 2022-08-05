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

        //TODO Compete deleting files from local repository after sending the message in telegram
        public string Convert(string url, long userId, string userName)
        {
            DeleteMusicFromLocalRep();

            DirectoryInfo dirInfo = new DirectoryInfo(_pathToFolder);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(url);
            string videopath = Path.Combine(_pathToFolder, vid.FullName);

            File.WriteAllBytes(videopath, vid.GetBytes());

            var inputFile = new MediaFile { Filename = Path.Combine(_pathToFolder, vid.FullName) };
            var outputFile = new MediaFile { Filename = Path.Combine(_pathToFolder, $"{vid.Title}.mp3") };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);


                engine.Convert(inputFile, outputFile);
            }

            CreateDbNote(new Conversion { UserId = userId, YtLink = url });

            return $"{outputFile.Filename}";
        }

        public static void DeleteMusicFromLocalRep()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(_pathToFolder);

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                file.Delete();
            }
        }
        
        private static void CreateDbNote(Conversion conversion)
        {
            if (conversion is null)
            {
                throw new ArgumentNullException(nameof(conversion), "conversion variable was null.");
            }

            using ApplicationContext db = new();
            db.Conversions.Add(conversion);
            db.SaveChanges();
        }
    }
}
