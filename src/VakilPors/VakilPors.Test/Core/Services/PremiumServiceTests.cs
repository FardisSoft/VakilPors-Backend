using AutoMapper;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Premium;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services
{
    public class PremiumServiceTests
    {
        private readonly PremiumService premiumService;
        private readonly Mock<IAppUnitOfWork> appUnitOfWorkMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IWalletServices> walletserviceMock;

        public PremiumServiceTests()
        {
            appUnitOfWorkMock = new Mock<IAppUnitOfWork>();
            mapperMock = new Mock<IMapper>();
            walletserviceMock = new Mock<IWalletServices>();

            premiumService = new PremiumService(
                appUnitOfWorkMock.Object,
                mapperMock.Object,
                walletserviceMock.Object );
        }

        [Fact]
        public async Task activate_premium()
        {
            //Arrange
            int id = 1;
            var user = new User { Id = 1 };
            var subscribed_result = new Subscribed { UserId = id, ExpireDate = DateTime.Now.AddDays(90) , PremiumID = 3 };
            string premium = "gold";
            
            IEnumerable<Subscribed> subscribeds = new List<Subscribed>()
            {
                new Subscribed
                {
                    User = user,
                    UserId = user.Id
                }
            };
            var subscribedqueriable = subscribeds.AsQueryable();
            var mock = subscribedqueriable.BuildMock();

            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.AsQueryable()).Returns(mock);
            appUnitOfWorkMock.Setup(u => u.UserRepo.FindAsync(id)).ReturnsAsync(user);
            walletserviceMock.Setup(u => u.AddTransaction(id, It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>() , It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.CompletedTask);
            appUnitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            //Act

            var result =await  premiumService.ActivatePremium(premium, id);

            //Assert

            Assert.Equal(3, result.PremiumID);
            Assert.Equal(1, result.UserId);


        }

        //[Fact]
        //public async Task transact_user()
        //{
        //    //Arrange
        //    var user = new User { Id = 1 };
        //    var id = 1;
        //    walletserviceMock.Setup(u => u.AddTransaction(id, It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.CompletedTask);
        //    appUnitOfWorkMock.Setup(u => u.UserRepo.FindAsync(id)).ReturnsAsync(user);
        //    //Act 
        //    await premiumService.TransactUser("", id, 4000, "همراه");
        //    //Assert
        //    Assert.Equal( 4000, user.Transactions.ToList()[0].Amount);
        //}

        [Fact]
        public async Task diactivate_premium()
        {
            //Arrange
            var id = 0;
            var subscription = new Subscribed();
            IEnumerable<Subscribed> subscribeds = new List<Subscribed>()
            {
                new Subscribed
                {
                    PremiumID = 3,
                    ExpireDate = DateTime.Today.AddDays(5)

                }
            };

            var subqueirable = subscribeds.AsQueryable();
            var mock = subqueirable.BuildMock();

            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.AsQueryable()).Returns(mock);
            appUnitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);


            //Act 

            await premiumService.DeactivatePremium(id);

            //Arrange

            Assert.Equal(1, subscribeds.ToList()[0].PremiumID);
            Assert.Equal(DateTime.MaxValue, subscribeds.ToList()[0].ExpireDate);



        }


        [Fact]
        public async Task update_plan()
        {
            //Arrange
            var subdto = new SubscribedDto { ID = 1 , Premium = new PremiumDto()};
            subdto.Premium.ServiceType = Plan.Gold;
            var sub = new Subscribed { Premium = new Premium()};
            sub.Premium.ServiceType = Plan.Free;

            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.FindAsync(subdto.ID)).ReturnsAsync(sub);
            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.Update(sub));
            appUnitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            //Act
            await premiumService.UpdatePlan(subdto);

            //Assert
            Assert.Equal(Plan.Gold, sub.Premium.ServiceType);
          
        }

        [Fact]
        public async Task get_premium_status()
        {
            //Arrange
            var id = 1;
            var subscribed = new Subscribed();
            var subscribeddto = new SubscribedDto { ID = 2, Premium = new PremiumDto() };
            IEnumerable<Subscribed> subscribeds = new List<Subscribed>
            {
                new Subscribed{ID = 2 , Premium = new Premium() , UserId = id  }
            };

            var subscribedquriable = subscribeds.AsQueryable();
            var mock = subscribedquriable.BuildMock();

            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.AsQueryable()).Returns(mock);
            mapperMock.Setup(u => u.Map<SubscribedDto>(subscribeds.ToList()[0])).Returns(subscribeddto);

            //Act
            var result = await premiumService.GetPremiumStatus(id);

            //Assert
            Assert.Equal(2, result.ID);


        }

        [Fact]
        public async Task does_user_have_subscription()
        {
            //Arrange 
            var id = 1;
            var subscribed = new Subscribed();
            var subscribeddto = new SubscribedDto { ID = 2, Premium = new PremiumDto()   };
            subscribeddto.Premium.ServiceType = Plan.Gold;
            //subscribeddto.IsExpired = false;
            


            IEnumerable<Subscribed> subscribeds = new List<Subscribed>
            {
                new Subscribed{ID = 2 , Premium = new Premium() , UserId = id  }
            };

            var subscribedquriable = subscribeds.AsQueryable();
            var mock = subscribedquriable.BuildMock();

            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.AsQueryable()).Returns(mock);
            mapperMock.Setup(u => u.Map<SubscribedDto>(subscribeds.ToList()[0])).Returns(subscribeddto);

            //Act

            var result = await premiumService.DoseUserHaveAnyActiveSubscription(id);

            //Assert 

            Assert.True(result);

        }
    }
}
