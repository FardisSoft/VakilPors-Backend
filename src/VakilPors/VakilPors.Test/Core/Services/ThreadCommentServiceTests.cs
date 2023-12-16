﻿using AutoMapper;
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

    }
}