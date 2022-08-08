using Bot.Common.Models;

namespace Bot.BusinessLogic.Services.Contracts
{
    internal interface IConversionsService
    {
        Task<string> Convert(string url, long userId, string userName);
    }
}
