using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Services;
using VakilPors.Data.UnitOfWork;

namespace VakilPors.Test.Core.Services
{
    public class TelegramServiceTests
    {
        private readonly TelegramService telegramService;
        private readonly Mock<IAppUnitOfWork> AppUnitofWorkMock;
        private readonly Mock<IMapper> MapperMock;
        private readonly Mock<ILogger<TelegramService>> LoggerMock;
        private readonly Mock<HttpClient> Httpclientmock;
        public TelegramServiceTests()
        {
            AppUnitofWorkMock = new Mock<IAppUnitOfWork>();
            MapperMock = new Mock<IMapper>();
            LoggerMock = new Mock<ILogger<TelegramService>>();
            Httpclientmock = new Mock<HttpClient>();

            telegramService = new TelegramService(AppUnitofWorkMock.Object,MapperMock.Object,LoggerMock.Object);    

        }

        [Fact]
        public async Task save_chat_id()
        {
            //Arrange 
            var phone_number = "09123456789";
            var chatid = "33";
            var telegramdto = new TelegramDto {phone_number = phone_number , chat_id = chatid };
            IEnumerable<User> users = new List<User>()
            {
                new User()
                {
                    PhoneNumber = phone_number
                }
            };
            var users_queriable = users.AsQueryable();
            var mock = users_queriable.BuildMock();

            AppUnitofWorkMock.Setup(u => u.UserRepo.AsQueryable()).Returns(mock);
            AppUnitofWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            //Act 
            await telegramService.SaveChatId(telegramdto);

            //Assert 
            Assert.Equal(chatid, users.ToList()[0].Telegram);

        
        }
        [Fact]
        public async Task sendtotelegram()
        {
            //Arrange 
            var phone_number = "09123456789";
            var chatid = "33";
            var text = "text";
            var url = "https://fardissoft.pythonanywhere.com/post";

            string jsonData = "{\"chat_id\":\"" + "33" + "\",\"text\":\"" + text + "\"}";
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var telegramdto = new TelegramDto { phone_number = phone_number, chat_id = chatid };
            IEnumerable<User> users = new List<User>()
            {
                new User()
                {
                    PhoneNumber = phone_number
                }
            };
            var users_queriable = users.AsQueryable();
            var mock = users_queriable.BuildMock();


            AppUnitofWorkMock.Setup(u => u.UserRepo.AsQueryable()).Returns(mock);
            AppUnitofWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            //Httpclientmock.Setup(u => u.PostAsync(url, content)).ReturnsAsync(new HttpResponseMessage());

            var httpClientHandler = new Mock<HttpMessageHandler>();
            httpClientHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            //Act 
            await telegramService.SendToTelegram(text, chatid);

        }
    }

}
