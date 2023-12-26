using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Response;

namespace VakilPors.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatController : MyControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatServices _chatServices;

        public ChatController(ILogger<ChatController> logger, IChatServices chatServices)
        {
            this._logger = logger;
            this._chatServices = chatServices;
        }
        [HttpGet]
        public async Task<ActionResult<AppResponse<ICollection<Chat>>>> GetChats()
        {
            //returns a list of chat the user is a member of.
            var userId = GetUserId();
            var phoneNumber = GetPhoneNumber();
            _logger.LogInformation($"Get Chats of user {phoneNumber}");
            var chats = await _chatServices.GetChatsOfUser(userId);
            _logger.LogInformation($"Chats of user {phoneNumber} recieved successfully");
            return new AppResponse<ICollection<Chat>>(chats, $"get {chats.Count} chats successfully!");
        }
        [HttpGet]
        public async Task<ActionResult<AppResponse<ICollection<Chat>>>> GetChatsWithMessages()
        {
            //returns a list of chat the user is a member of.
            var userId = GetUserId();
            var phoneNumber = GetPhoneNumber();
            _logger.LogInformation($"Get Chats and Messages of user {phoneNumber}");
            var chats = await _chatServices.GetChatsWithMessagesOfUser(userId);
            _logger.LogInformation($"Chats of user {phoneNumber} recieved successfully");
            return new AppResponse<ICollection<Chat>>(chats, $"get {chats.Count} chats successfully!");
        }
        [HttpGet]
        public async Task<ActionResult<AppResponse<ICollection<ChatMessage>>>> GetChatMessages([FromQuery] int chatId)
        {
            var userId = GetUserId();
            var phoneNumber = GetPhoneNumber();
            _logger.LogInformation($"Get Messages of chatid: {chatId} of user {phoneNumber}");
            var messages = await _chatServices.GetMessagesOfChat(userId, chatId);
            _logger.LogInformation($"Messages of chatid: {chatId} of user {phoneNumber} recieved successfully");
            return new AppResponse<ICollection<ChatMessage>>(messages, $"get {messages.Count} messages successfully!");
        }
        [HttpPost]
        public async Task<ActionResult<AppResponse<Chat>>> StartChat(int recieverUserId)
        {
            var userId1 = GetUserId();
            var phoneNumber1 = GetPhoneNumber();
            _logger.LogInformation($"Start Chat user {phoneNumber1} with userid:{recieverUserId}");
            var chat = await _chatServices.CreateChat(recieverUserId, userId1); //returns a chat id.
            _logger.LogInformation($"Chat user {phoneNumber1} with userid:{recieverUserId} started successfully!");
            return new AppResponse<Chat>(chat, "chat created/fetched successfully!");
        }

    }
}