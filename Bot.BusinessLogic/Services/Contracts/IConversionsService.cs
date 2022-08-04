using Bot.Common.Models;

namespace Bot.BusinessLogic.Services.Contracts
{
    internal interface IConversionsService
    {
        void CreateNote(ConversionModel conversion);
    }
}
