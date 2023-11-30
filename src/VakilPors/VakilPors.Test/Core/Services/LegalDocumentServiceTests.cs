using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Case;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services
{
    public class LegalDocumentServiceTests
    {
        private readonly LegalDocumentService legalDocumentService;
        private readonly Mock<IAppUnitOfWork> _appUnitOfWorkMock;
        private readonly Mock<IAwsFileService> _awsFileServiceMock;
        private readonly Mock<ILawyerServices> _lawyerServicesMock;
        private readonly Mock<IMapper> _mappermock;
        private readonly Mock<IEmailSender> _emailsenderMock;
        private readonly Mock<ITelegramService> _telegramserviceMock;
        private readonly Mock<IPremiumService> _premiumservice;

        public LegalDocumentServiceTests()
        {
            _appUnitOfWorkMock = new Mock<IAppUnitOfWork>();
            _awsFileServiceMock = new Mock<IAwsFileService>();
            _lawyerServicesMock = new Mock<ILawyerServices>();
            _mappermock = new Mock<IMapper>();
            _emailsenderMock = new Mock<IEmailSender>();
            _telegramserviceMock = new Mock<ITelegramService>();
            _premiumservice = new Mock<IPremiumService>();

            legalDocumentService = new LegalDocumentService
                (_lawyerServicesMock.Object, _awsFileServiceMock.Object, _appUnitOfWorkMock.Object, _mappermock.Object, _emailsenderMock.Object, _telegramserviceMock.Object, _premiumservice.Object);
                
                
        }


        [Fact]
        public async Task adddocument()
        {
            //Arrange 
            var userid = 1;
            var legaldocumentdto = new LegalDocumentDto { File = null};
            var legaldocument = new LegalDocument { };
            _lawyerServicesMock.Setup(x => x.IsLawyer(It.IsAny<int>())).ReturnsAsync(false);
            _mappermock.Setup(x => x.Map<LegalDocument>(legaldocumentdto)).Returns(legaldocument);
            _mappermock.Setup(x => x.Map<LegalDocumentDto>(legaldocument)).Returns(legaldocumentdto);
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.AddAsync(legaldocument)).Returns(Task.CompletedTask);
            _appUnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            //Act 
            var reseult = await legalDocumentService.AddDocument(userid, legaldocumentdto);

            //Assert 
            Assert.Equal(userid, reseult.UserId);
        }
    }
}
