using Bot.BusinessLogic.Services.Contracts;
using Bot.Common.Models;
using Bot.Model;
using Bot.Model.DatabaseModels;
using NReco.VideoConverter;

namespace Bot.BusinessLogic.Services.Implementations
{
    internal class ConversionsService : IConversionsService
    {
        private static readonly string _pathToFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\ConvertedMp3\";
        private static readonly FFMpegConverter _converter = new FFMpegConverter();
        public ApplicationContext _context;

        public ConversionsService(ApplicationContext context)
        {
            _context = context;
        }

        public void CreateNote(ConversionModel conversion)
        {
            if (conversion is null)
            {
                throw new ArgumentNullException(nameof(conversion), "conversion was null.");
            }

            _context.Conversions.Add(new Conversion { UserId = conversion.UserId, YtLink = conversion.YtLink });
        }

        public static void ConvertFile(string filename)
        {
            _converter.ConvertMedia(_pathToFolder + filename, _pathToFolder + filename + ".mp3", ".mp3");
        }

        private static void SendAudio(string audioPath)
        {

        }
    }
}
