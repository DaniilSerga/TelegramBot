using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Bot.BusinessLogic.Services.Contracts;

namespace Bot.Api.Controllers
{
    [ApiController]
    [Route("api/message/update")]
    public class BotController : Controller
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ICommandService _commandService;

        public BotController(ITelegramBotClient telegramBotClient, ICommandService commandService)
        {
            _telegramBotClient = telegramBotClient;
            _commandService = commandService;
        }

        [HttpGet]
        public IActionResult Get() => Ok("Started");

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update == null) return Ok();

            var message = update.Message;

            foreach (var command in _commandService.Get())
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, _telegramBotClient);
                    break;
                }
            }
            return Ok();
        }
    }
}
