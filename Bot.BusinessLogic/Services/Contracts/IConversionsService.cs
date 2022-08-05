using Bot.Common.Models;

namespace Bot.BusinessLogic.Services.Contracts
{
    internal interface IConversionsService
    {
        string Convert(string url, long userId, string userName);
    }
}
