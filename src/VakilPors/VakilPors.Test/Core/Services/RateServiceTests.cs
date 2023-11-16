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
using VakilPors.Core.Domain.Dtos.Rate;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services
{
    public class RateServiceTests
    {
        private readonly RateService rateService;
        private readonly Mock<IAppUnitOfWork> appUnitOfWorkMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ILawyerServices> lawyerServicesMock;

        public RateServiceTests()
        {
            appUnitOfWorkMock = new Mock<IAppUnitOfWork>(); 
            mapperMock = new Mock<IMapper>();
            lawyerServicesMock = new Mock<ILawyerServices>();

            rateService = new RateService( mapperMock.Object ,appUnitOfWorkMock.Object, lawyerServicesMock.Object);
            
        }

        [Fact]
        public async Task add_rate_async()
        {
            //Arrange 
            var userid = 1;
            var lawyerid = 1;
            var ratedto = new RateDto { RateNum = 3 };
            var rate = new Rate();
            var lawyer = new Lawyer { Rating = 0 };
            IEnumerable<Rate> rates = new List<Rate>
            {

            };
            var mock = rates.AsQueryable().BuildMock();

            IEnumerable<Lawyer> lawyers = new List<Lawyer>
            {
                new Lawyer { Id = lawyerid , Rating = 0}
            };
            var mock2 = lawyers.AsQueryable().BuildMock();

            appUnitOfWorkMock.Setup(x => x.RateRepo.AsQueryable()).Returns(mock);
            mapperMock.Setup(x => x.Map<Rate>(It.IsAny<RateDto>())).Returns(rate);
            appUnitOfWorkMock.Setup(x => x.RateRepo.AddAsync(rate)).Returns(Task.CompletedTask);
            appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable()).Returns(mock2);
            lawyerServicesMock.Setup(x=> x.AddToken(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            appUnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            //Act 
            await rateService.AddRateAsync(ratedto, userid , lawyerid);

            //Assert 
            Assert.Equal(3, lawyers.ToList()[0].Rating);

        }

        [Fact]
        public async Task calculate_rating()
        {
            //Arrange
            var lawyerid = 1;
            var avg = 3.5;
            IEnumerable<Rate> rates = new List<Rate>
            {
                new Rate{ LawyerId = lawyerid  , RateNum = 3},
                new Rate{ LawyerId = lawyerid ,RateNum = 4},
            };
            var ratesqueriabe = rates.AsQueryable();
            var mock = ratesqueriabe.BuildMock();
            appUnitOfWorkMock.Setup(x => x.RateRepo.AsQueryable()).Returns(mock);

            //Act
            var result = await rateService.CalculateRatingAsync(lawyerid);

            //Assert
            Assert.Equal(avg, result);
        }
    }
}
