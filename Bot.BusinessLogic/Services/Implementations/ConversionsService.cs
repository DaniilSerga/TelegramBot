using Bot.BusinessLogic.Services.Contracts;
using Bot.Common.Models;
using Bot.Model;
using Bot.Model.DatabaseModels;
using System;
using VideoLibrary;
using MediaToolkit;
using MediaToolkit.Model;

namespace Bot.BusinessLogic.Services.Implementations
{
    public class ConversionsService : IConversionsService
    {
        private static readonly string _pathToFolder = Environment.CurrentDirectory + @"\ConvertedMp3\";
        public ApplicationContext _context;

        public ConversionsService() { }

        public ConversionsService(ApplicationContext context)
        {
            _context = context;
        }
        
        // TODO Compete deleting files from local repository after sending the message in telegram
        public string Convert(string url)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(_pathToFolder);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            var source = _pathToFolder;
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(url);
            var songPath = source + vid.FullName;

            File.WriteAllBytes(songPath, vid.GetBytes());

            var inputFile = new MediaFile { Filename = source + vid.FullName };
            var outputFile = new MediaFile { Filename = $"{source + vid.FullName}.mp3" };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }

            return $"{source + vid.FullName}.mp3";
        }
    }
}
