using AutoMapper;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services
{
    public class ThreadCommentServiceTests
    {
        private readonly ThreadCommentService threadCommentService;
        private readonly Mock<IAppUnitOfWork> appUnitOfWorkmock;
        private readonly Mock<IMapper> mappermock;
        private readonly Mock<ILawyerServices> lawyerservicesmock;
        private readonly Mock<IPremiumService> premiumservicemock;
        private Mock<IAntiSpam> antispammock;

        public ThreadCommentServiceTests()
        {
            appUnitOfWorkmock = new Mock<IAppUnitOfWork>();
            mappermock = new Mock<IMapper>();
            lawyerservicesmock = new Mock<ILawyerServices>();
            premiumservicemock = new Mock<IPremiumService>();
            threadCommentService = new ThreadCommentService
                (
                appUnitOfWorkmock.Object, mappermock.Object, lawyerservicesmock.Object, premiumservicemock.Object
                );
        }

        [Fact]
        public async Task CreateComment()
        {
            //Arrange
            var userid = 1;
            var commentdto = new ThreadCommentDto { Text = "sample" };
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>()
            {
                new ThreadComment {UserLikes = new List<UserCommentLike>(){ new UserCommentLike() { UserId = 1 , CommentId = 1} } , Id = 1}
            };
            var mock = threadComments.BuildMock();
            //var antspm = new Mock<IAntiSpam>();
            antispammock = new Mock<IAntiSpam>();
            IEnumerable<Lawyer> lawyers = new List<Lawyer>()
            {
                new Lawyer()
            };
            var mock2 = lawyers.BuildMock();

            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock);
            antispammock.Setup(u => u.IsSpam(It.IsAny<string>())).ReturnsAsync("ok");
            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.AddAsync(It.IsAny <ThreadComment>()));
            appUnitOfWorkmock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            appUnitOfWorkmock.Setup(u => u.LawyerRepo.AsQueryable()).Returns(mock2);
            lawyerservicesmock.Setup(u => u.IsLawyer(It.IsAny<int>())).ReturnsAsync(true);
            premiumservicemock.Setup(u => u.DoseUserHaveAnyActiveSubscription(It.IsAny<int>())).ReturnsAsync(false);


            //Act & Assert
            var exception = await Assert.ThrowsAsync<BadArgumentException>(() => threadCommentService.CreateComment(userid , commentdto));
            Assert.Equal("comment not found", exception.Message);

        }

        [Fact]
        public async Task checkwithin2minutes()
        {
            //Arrange
            var userid = 1; 
            var threadcommentdto = new ThreadCommentDto();
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>()
            {
                new ThreadComment {UserLikes = new List<UserCommentLike>(){ new UserCommentLike() { UserId = 1 , CommentId = 1} } , Id = 1}
            };
            var mock = threadComments.BuildMock();
            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock);

            //Act 
            var result = await threadCommentService.CheckWithin2Minutes(userid, threadcommentdto);

            //Assert 
            Assert.Equal("ok", result);

        }

        [Fact]
        public async Task updatecomment()
        {
            //Arrange 
            var userid = 1;
            var threadcommentdto = new ThreadCommentDto() { Text = "@alireza" };
            antispammock = new Mock<IAntiSpam>();
            antispammock.Setup(u => u.IsSpam(threadcommentdto.Text)).ReturnsAsync("This message is detected as a spam and can not be shown.");

            //Act & Assert
            var exception = await Assert.ThrowsAsync<BadArgumentException>(() => threadCommentService.UpdateComment(userid, threadcommentdto));
            Assert.Equal("This message is detected as a spam and can not be shown.", exception.Message);
        }

        [Fact]
        public async Task deletecomment()
        {
            //Arrange 
            var userid = 1;
            var commentid = 1;
            var threadcommentdto = new ThreadCommentDto() { Text = "@alireza" };
            ThreadComment threadComment = new ThreadComment { UserId = 2 };
            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.FindAsync(commentid)).ReturnsAsync(threadComment);
            //Act & Assert
            var exception = await Assert.ThrowsAsync<AccessViolationException>(() => threadCommentService.DeleteComment(userid, commentid));
            Assert.Equal("You do not have permission to perform this action", exception.Message);
        }

        [Fact]
        public async Task getcommentbyid()
        {
            //Arrange 
            var userid = 1;
            var commentid = 1;
            var threadcommentdto = new ThreadCommentDto() { Text = "@alireza" };
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>()
            {
                
            };
            var mock = threadComments.BuildMock();
            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock);
            //Act & Assert
            var exception = await Assert.ThrowsAsync<BadArgumentException>(() => threadCommentService.GetCommentById(userid, commentid));
            Assert.Equal("comment not found", exception.Message);

        }

        [Fact]
        public async Task likecomment()
        {
            //Arrange
            var userid = 1;
            var commentid = 1;
            var threadcommentdto = new ThreadCommentDto();
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>()
            {
                new ThreadComment {UserLikes = new List<UserCommentLike>(){ new UserCommentLike() { UserId = 1 , CommentId = 1} } , Id = 1}

            };
            var mock = threadComments.BuildMock();
            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock);
            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.Update(It.IsAny<ThreadComment>()));
            appUnitOfWorkmock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            //Act 
            var result = await threadCommentService.LikeComment(userid, commentid);

            //Assert 
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task undolikecomment()
        {
            //Arrange
            var userid = 1;
            var commentid = 1;
            var threadcommentdto = new ThreadCommentDto();
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>()
            {
                new ThreadComment {UserLikes = new List<UserCommentLike>(){ new UserCommentLike() { UserId = 1 , CommentId = 1} } , Id = 1}

            };
            var mock = threadComments.BuildMock();
            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock);
            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.Update(It.IsAny<ThreadComment>()));
            appUnitOfWorkmock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            //Act 
            var result = await threadCommentService.UndoLikeComment(userid, commentid);

            //Assert 
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task setasanswer()
        {
            //Arrange
            var userid = 1;
            var commentid = 1;
            var threadcommentdto = new ThreadCommentDto();
            IEnumerable<ThreadComment> threadComments = new List<ThreadComment>()
            {
                new ThreadComment {UserLikes = new List<UserCommentLike>(){ new UserCommentLike() { UserId = 1 , CommentId = 1} } , Id = 1 , Thread = new ForumThread(){UserId = 1 }  , UserId = 10}

            };
            var mock = threadComments.BuildMock();

            IEnumerable<Lawyer> lawyers = new List<Lawyer>()
            {
                new Lawyer(){UserId =10 }
            };
            var mock2 = lawyers.BuildMock();


            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.AsQueryable()).Returns(mock);
            appUnitOfWorkmock.Setup(u => u.ThreadCommentRepo.Update(It.IsAny<ThreadComment>()));
            appUnitOfWorkmock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            appUnitOfWorkmock.Setup(u => u.LawyerRepo.AsQueryable()).Returns(mock2);
            lawyerservicesmock.Setup(u => u.AddToken(It.IsAny<int>(), It.IsAny<int>()));

            //Act 
            var result =await  threadCommentService.SetAsAnswer(userid, commentid);

            //Assert 
            Assert.True(result);

        }
    }
}
