using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Services;

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

        public LawyerServiceTests()
        {
            appUnitOfWorkMock = new Mock<IAppUnitOfWork>();
            userServicesMock = new Mock<IUserServices>();
            imapperMock = new Mock<IMapper>();
            awsFileServiceMock = new Mock<IAwsFileService>();
            chatServicesMock = new Mock<IChatServices>();
            walletServicesMock = new Mock<IWalletServices>();
            _lawyerservicesMock = new Mock<LawyerServices>();




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
            // var id = 1;
            var lawyerDto = new LawyerDto {Rating = 1 , AboutMe = "example", Education = "sample"};
            //var lawyerdto_result = new LawyerDto { }
            var found_lawyer = new Lawyer { Rating = 5};
            var found_user = new User();
            appUnitOfWorkMock.Setup(m => m.LawyerRepo.FindAsync(It.IsAny<int>())).ReturnsAsync(found_lawyer);
            appUnitOfWorkMock.Setup(m => m.LawyerRepo.Update(found_lawyer));//.Returns(IdentityResult.Success);
            appUnitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            awsFileServiceMock.Setup(u => u.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(It.IsAny<string>() );
            //userServicesMock.Setup(u => u.UpdateUser(found_lawyer)).ReturnsAsync(IdentityResult.Success);
            appUnitOfWorkMock.Setup(u => u.UserRepo.Update(found_user));//Returns(IdentityResult.Success);
            imapperMock.Setup(u => u.Map<LawyerDto>(found_lawyer)).Returns(lawyerDto);
            // _lawyerservicesMock.Setup(u => u.GetLawyerDtoFormLawyer(found_lawyer)).ReturnsAsync(lawyerDto);
            //Act

            var result = await lawyerServices.UpdateLawyer(lawyerDto);

            //Assert
            Assert.Equal(result,lawyerDto);
        }

    }
}
