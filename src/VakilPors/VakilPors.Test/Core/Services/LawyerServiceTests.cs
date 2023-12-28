    using AutoMapper;
using Bogus.DataSets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Pagination.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.Repositories;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Search;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Services;
using VakilPors.Data.UnitOfWork;

namespace VakilPors.Test.Core.Services
{
    public class LawyerServiceTests
    {
        private readonly LawyerServices lawyerServices;
        private readonly Mock<LawyerServices> _lawyerservicesMock;
        private readonly Mock<IAppUnitOfWork> appUnitOfWorkMock;
        private readonly Mock<IUserServices> userServicesMock;
        private readonly Mock<IMapper> imapperMock;
        private readonly Mock<IAwsFileService > awsFileServiceMock;
        private readonly Mock<IChatServices> chatServicesMock;
        private readonly Mock<IWalletServices> walletServicesMock;
        private readonly Mapper mapper;
        private readonly Mock<UserManager<User>> userManagerMock;
        private readonly Mock<Pagination<Lawyer>> paginationMock;
        public LawyerServiceTests()
        {
            appUnitOfWorkMock = new Mock<IAppUnitOfWork>();
            userServicesMock = new Mock<IUserServices>();
            imapperMock = new Mock<IMapper>();
            awsFileServiceMock = new Mock<IAwsFileService>();
            chatServicesMock = new Mock<IChatServices>();
            walletServicesMock = new Mock<IWalletServices>();
            _lawyerservicesMock = new Mock<LawyerServices>();
            userManagerMock = new Mock<UserManager<User>>();
            paginationMock = new Mock<Pagination<Lawyer>>();



            lawyerServices = new LawyerServices(
                appUnitOfWorkMock.Object,
                imapperMock.Object,
                userServicesMock.Object,
                awsFileServiceMock.Object,
                chatServicesMock.Object,
                walletServicesMock.Object

                );
            
        }

        [Fact]
        public async Task Update_lawyer()
        {
            //Arrange
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>
            {
                new ThreadComment (),
                new ThreadComment ()
            };
            IEnumerable<Chat> chats = new List<Chat>
            {
                new Chat(),
                new Chat ()
            };
            IEnumerable<Subscribed> subscribeds = new List<Subscribed>
            {
                new Subscribed(),
                new Subscribed()
            };
            IQueryable<ThreadComment> threadCommentsQueryable = threadComments.AsQueryable();
            var mock = threadCommentsQueryable.BuildMock();

            IQueryable<Chat> chatQueryable = chats.AsQueryable();
            var mock2 = chatQueryable.BuildMock();

            IQueryable<Subscribed> subscribedQueryable = subscribeds.AsQueryable();
            var mock3 = subscribedQueryable.BuildMock();

            var threadCommentRepo = new Mock<ThreadComment>();
            var lawyerDto = new LawyerDto { Rating = 1, AboutMe = "example", Education = "sample" };
            var found_lawyer = new Lawyer { Rating = 5 };
            var found_user = new User();
            appUnitOfWorkMock.Setup(m => m.LawyerRepo.FindAsync(It.IsAny<int>())).ReturnsAsync(found_lawyer);
            appUnitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            awsFileServiceMock.Setup(u => u.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(It.IsAny<string>());
            userServicesMock.Setup(u => u.UpdateUser(new VakilPors.Core.Domain.Dtos.User.UserDto())).ReturnsAsync(new VakilPors.Core.Domain.Dtos.User.UserDto());
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.Update(found_lawyer));//Returns(IdentityResult.Success);
            imapperMock.Setup(u => u.Map<LawyerDto>(found_lawyer)).Returns(lawyerDto);
            //appUnitOfWorkMock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns((IQueryable<ThreadComment>)threadComments);
            appUnitOfWorkMock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock);
            appUnitOfWorkMock.Setup(u => u.ChatRepo.AsQueryable()).Returns(mock2);
            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.AsQueryable()).Returns(mock3);

            //Act
            var result = await lawyerServices.UpdateLawyer(lawyerDto);

            //Assert
            Assert.Equal(result, lawyerDto);
        }

        [Fact]
        public async Task get_all_lawyers()
        {
            //Arrange 

            var lawyerDto = new LawyerDto { Rating = 1, AboutMe = "example", Education = "sample" };
            var found_lawyer = new Lawyer { Rating = 5 };
            var found_user = new User();
            var p = 0;
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>
            {
                new ThreadComment (),
                new ThreadComment ()
            };
            IEnumerable<Chat> chats = new List<Chat>
            {
                new Chat(),
                new Chat ()
            };
            IEnumerable<Subscribed> subscribeds = new List<Subscribed>
            {
                new Subscribed(),
                new Subscribed()
            };
            IEnumerable<Lawyer> _lawyers = new List<Lawyer>()
            {
                new Lawyer{Id = 1},
                new Lawyer{Id = 2},
                new Lawyer{Id = 3}
            };
            IEnumerable<LawyerDto> lawyerdtos = new List<LawyerDto>()
            {
                new LawyerDto{Id = 1 , NumberOfAnswers = 2},
                new LawyerDto{Id = 2},
                new LawyerDto{Id = 3}
            };
            IQueryable<ThreadComment> threadCommentsQueryable = threadComments.AsQueryable();
            var mock = threadCommentsQueryable.BuildMock();

            IQueryable<Chat> chatQueryable = chats.AsQueryable();
            var mock2 = chatQueryable.BuildMock();

            IQueryable<Subscribed> subscribedQueryable = subscribeds.AsQueryable();
            var mock3 = subscribedQueryable.BuildMock();

            IQueryable<Lawyer> lawyers = _lawyers.AsQueryable();
            var _mock = lawyers.BuildMock();

            foreach (var lawyer in _lawyers)
            {
                imapperMock.Setup(u => u.Map<LawyerDto>(lawyer)).Returns(lawyerdtos.ToList()[p]);
                p++;
            }
            appUnitOfWorkMock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock);            
            appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable()).Returns(_mock);
            appUnitOfWorkMock.Setup(u => u.ChatRepo.AsQueryable()).Returns(mock2);
            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.AsQueryable()).Returns(mock3);

            
            //Act
            var results =await  lawyerServices.GetAllLawyers();

            //Assert

            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(lawyerdtos.ToList()[i], results[i]);
            }
        }

        [Fact]
        public async Task verify_lawyer()
        {
            //Arrange
            var id = 1;
            var found_lawyer = new Lawyer();
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.FindAsync(id)).ReturnsAsync(found_lawyer);
            appUnitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.Update(found_lawyer));

            //Act
            var result = await lawyerServices.VerifyLawyer(id);

            //Assert 
            Assert.Equal(true,result);
        }
        [Fact]
        public async Task add_token()
        {
            //Arrange
            var found_lawyer = new Lawyer { Tokens = 10};
            var tokens = 10;
            var id = 1;
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.FindAsync(id)).ReturnsAsync(found_lawyer);
            appUnitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.Update(found_lawyer));

            //Act
            await lawyerServices.AddToken(id,tokens);

            //Assert 
            Assert.Equal(20, found_lawyer.Tokens);
        }

        [Fact]
        public async Task set_tokens()
        {
            //Arrange
            var found_lawyer = new Lawyer { Tokens = 0 };
            var tokens = 10;
            var id = 1;
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.FindAsync(id)).ReturnsAsync(found_lawyer);
            appUnitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.Update(found_lawyer));

            //Act
            await lawyerServices.SetTokens(id, tokens);

            //Act 
            Assert.Equal(10, found_lawyer.Tokens);

        }
        [Fact]
        public async Task sample()
        {
            var id = 1;
            IEnumerable<Lawyer> _lawyers = new List<Lawyer>()
            {
                new Lawyer{Id = 1,UserId = 1},
                new Lawyer{Id = 2},
                new Lawyer{Id = 3}
            };
            IQueryable<Lawyer> lawyers = _lawyers.AsQueryable();
            
            var mockLawyerRepository = new Mock<IGenericRepo<Lawyer>>();
            var mock = lawyers.BuildMock();
            //appUnitOfWorkMock.Setup(u => u.LawyerRepo).Returns(mockLawyerRepository.Object);
            //mockLawyerRepository.Setup(r => r.AsQueryable()).Returns(lawyers);

            appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable()).Returns(mock);
            // appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable().Provider).Returns(lawyers.Provider);
            // appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable().ElementType).Returns(lawyers.ElementType);
            // appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable().Expression).Returns(lawyers.Expression);

            
            // //Act and Assert 
            // await Assert.ThrowsAsync<BadArgumentException>(() => lawyerServices.sample(id));


            //Act
            var result = await lawyerServices.sample(id);

            //Act 
            Assert.Equal(result, _lawyers.First());
        }

        [Fact]
        public async Task transfer_token()
        {
            //Arrange 
            int id = 1;
            var user = new User { Id = id};
            var userd = new UserDto { Id = id };
            var found_lawyer = new Lawyer();
            IEnumerable<Lawyer> lawyers = new List<Lawyer>()
            {
                new Lawyer{User = user , UserId = id}
            };
            IEnumerable<LawyerDto> lawyerdtos = new List<LawyerDto>()
            {
                new LawyerDto{User = userd }
            };
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>
            {
                new ThreadComment (),
                new ThreadComment ()
            };
            IEnumerable<Chat> chats = new List<Chat>
            {
                new Chat(),
                new Chat ()
            };
            IEnumerable<Subscribed> subscribeds = new List<Subscribed>
            {
                new Subscribed(),
                new Subscribed()
            };

            IQueryable<ThreadComment> threadCommentsQueryable = threadComments.AsQueryable();
            var mock = threadCommentsQueryable.BuildMock();

            IQueryable<Chat> chatQueryable = chats.AsQueryable();
            var mock2 = chatQueryable.BuildMock();

            IQueryable<Subscribed> subscribedQueryable = subscribeds.AsQueryable();
            var mock3 = subscribedQueryable.BuildMock();

            IQueryable<Lawyer> lawyerQuriable = lawyers.AsQueryable();
            var _mock = lawyerQuriable.BuildMock();

            IQueryable<LawyerDto> lawyerdtosQueriable = lawyerdtos.AsQueryable();
            var _mock2 = lawyerdtosQueriable.BuildMock();

            var t = new IdentityResult();
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.AsQueryable()).Returns(_mock);
            imapperMock.Setup(u => u.Map<LawyerDto>(lawyers.ToList()[0])).Returns(lawyerdtos.ToList()[0]);
            appUnitOfWorkMock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock);
            appUnitOfWorkMock.Setup(u => u.ChatRepo.AsQueryable()).Returns(mock2);
            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.AsQueryable()).Returns(mock3);
            userManagerMock.Setup(u => u.FindByIdAsync(id.ToString())).ReturnsAsync(user);
            userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.FindAsync(It.IsAny<int>())).ReturnsAsync(found_lawyer);
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.FindAsync(id)).ReturnsAsync(found_lawyer);
            appUnitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            appUnitOfWorkMock.Setup(u => u.LawyerRepo.Update(found_lawyer));


            //Act

            var result = await lawyerServices.TransferToken(id);

            //Assert

            Assert.True(result);

        }

        [Fact]
        public async Task is_lawyer()
        {
            //Arrange 
            int id = 1;
            var user = new User { Id = id };
            var userd = new UserDto { Id = id };
            var found_lawyer = new Lawyer();
            IEnumerable<Lawyer> lawyers = new List<Lawyer>()
            {
                new Lawyer{User = user , UserId = id}
            };
            IQueryable<Lawyer> lawyerQuriable = lawyers.AsQueryable();
            var _mock = lawyerQuriable.BuildMock();

            appUnitOfWorkMock.Setup(u => u.LawyerRepo.AsQueryable()).Returns(_mock);

            //Act 
            var result = await lawyerServices.IsLawyer(id);

            //Assert

            Assert.True(result);
        
        }


        //[Fact]
        //public async Task get_lawyers()
        //{
        //    //Arrange
        //    var pagedparams = new PagedParams();
        //    var sortparams = new SortParams();
        //    var filterparams = new LawyerFilterParams { Rating = 2 };
        //    IEnumerable<Lawyer> lawyers = new List<Lawyer>()
        //    {
        //        new Lawyer{Rating = 1},
        //        new Lawyer{Rating = 2},
        //        new Lawyer{Rating = 3},
        //        new Lawyer{Rating = 4},
        //        new Lawyer{Rating = 5},
        //    };
        //    IEnumerable<Lawyer> result_lawyers = new List<Lawyer>()
        //    {
        //        new Lawyer{Rating = 2},
        //        new Lawyer{Rating = 3},
        //        new Lawyer{Rating = 4},
        //        new Lawyer{Rating = 5},
        //    };

        //    var lawyersqueriable = lawyers.AsQueryable();
        //    var mock = lawyersqueriable.BuildMock();

        //    var pagination = new Pagination<Lawyer>(result_lawyers,4 );


        //    appUnitOfWorkMock.Setup(u => u.LawyerRepo.AsQueryableNoTracking()).Returns(mock);
        //    appUnitOfWorkMock.Setup(u => u.LawyerRepo.AsQueryableNoTracking().
        //    AsPaginationAsync<Lawyer>(mock, It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(pagination);


        //    //Act

        //    var result =await  lawyerServices.GetLawyers(pagedparams, sortparams, filterparams);

        //    //Assert 
        //}


        [Fact]
        public async Task GetLawyerById()
        {
            //Arrange
            int id = 4;
            //IEnumerable<Lawyer> lawyers = new List<Lawyer>
            //{
            //    new Lawyer{ User = new User() , Id = id}
            //};
            //var mock = lawyers.AsQueryable().BuildMock(); 
            //appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable()).Returns(mock);

            var lawyerDto = new LawyerDto { Rating = 1, AboutMe = "example", Education = "sample" };
            var found_lawyer = new Lawyer { Rating = 5 };
            var found_user = new User();
            var p = 0;
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>
            {
                new ThreadComment (),
                new ThreadComment ()
            };
            IEnumerable<Chat> chats = new List<Chat>
            {
                new Chat(),
                new Chat ()
            };
            IEnumerable<Subscribed> subscribeds = new List<Subscribed>
            {
                new Subscribed(),
                new Subscribed()
            };
            IEnumerable<Lawyer> _lawyers = new List<Lawyer>()
            {
                new Lawyer{Id = 1},
                new Lawyer{Id = 2},
                new Lawyer{Id = 3},
                new Lawyer{ User = new User() , Id = id}
            };
            IEnumerable<LawyerDto> lawyerdtos = new List<LawyerDto>()
            {
                new LawyerDto{Id = 1 , NumberOfAnswers = 2},
                new LawyerDto{Id = 2},
                new LawyerDto{Id = 3},
                new LawyerDto{ Id = 4},
            };
            IQueryable<ThreadComment> threadCommentsQueryable = threadComments.AsQueryable();
            var mock1 = threadCommentsQueryable.BuildMock();

            IQueryable<Chat> chatQueryable = chats.AsQueryable();
            var mock2 = chatQueryable.BuildMock();

            IQueryable<Subscribed> subscribedQueryable = subscribeds.AsQueryable();
            var mock3 = subscribedQueryable.BuildMock();

            IQueryable<Lawyer> lawyers = _lawyers.AsQueryable();
            var _mock = lawyers.BuildMock();

            foreach (var lawyer in _lawyers)
            {
                imapperMock.Setup(u => u.Map<LawyerDto>(lawyer)).Returns(lawyerdtos.ToList()[p]);
                p++;
            }
            appUnitOfWorkMock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock1);
            appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable()).Returns(_mock);
            appUnitOfWorkMock.Setup(u => u.ChatRepo.AsQueryable()).Returns(mock2);
            appUnitOfWorkMock.Setup(u => u.SubscribedRepo.AsQueryable()).Returns(mock3);


            //Act 
            var result = await lawyerServices.GetLawyerById(id);
        }
    }
}
