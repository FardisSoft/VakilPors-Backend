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
using VakilPors.Core.Domain.Dtos.Case;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
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
            var legaldocumentdto = new LegalDocumentDto { File = null };
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

        [Fact]
        public async Task updatedocument()
        {
            //Arrange 
            var userid = 1;
            var legaldocumentdto = new LegalDocumentDto { File = null };
            var founddocument = new LegalDocument { };
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.FindAsync(It.IsAny<int>())).ReturnsAsync(founddocument);
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.Update(founddocument));
            _appUnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _mappermock.Setup(x => x.Map<LegalDocumentDto>(founddocument)).Returns(legaldocumentdto);

            //Act 
            var result = await legalDocumentService.UpdateDocument(legaldocumentdto);

            //Assert 
            Assert.Equal(legaldocumentdto, result);
        }

        [Fact]
        public async Task deletedocument()
        {
            //Arrange
            var docid = 1;
            var founddocument = new LegalDocument { };
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.FindAsync(It.IsAny<int>())).ReturnsAsync(founddocument);
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.Remove(founddocument));
            _appUnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            //Act 
            var result = await legalDocumentService.DeleteDocument(docid);

            //Assert 
            Assert.True(result);

        }

        [Fact]
        public async Task getdocumentbyid()
        {
            //Arrange 
            var docid = 1;
            IEnumerable<LegalDocument> legalDocuments = new List<LegalDocument>();
            var mock = legalDocuments.AsQueryable().BuildMock();
            var legaldocumentdto = new LegalDocumentDto { File = null };
            var legaldocument = new LegalDocument { };
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.AsQueryable()).Returns(mock);
            _mappermock.Setup(x => x.Map<LegalDocumentDto>(legaldocument)).Returns(legaldocumentdto);

            //Act and Assert 
            var exception = await Assert.ThrowsAsync<BadArgumentException>(() => legalDocumentService.GetDocumentById(docid));
            Assert.Equal("document not found", exception.Message);

        }

        // [Fact(Skip = "pagination")]
        // public async Task getdocumentbyuserid()
        // {
        //     //Arrange 
        //     var userid = 1;
        //     IEnumerable<LegalDocument> legalDocuments = new List<LegalDocument>();
        //     var mock = legalDocuments.AsQueryable().BuildMock();
        //     var legaldocumentdto = new LegalDocumentDto { File = null };
        //     var legaldocument = new LegalDocument { };
        //     _appUnitOfWorkMock.Setup(x => x.DocumentRepo.AsQueryable()).Returns(mock);
        //     _mappermock.Setup(x => x.Map<LegalDocumentDto>(legaldocument)).Returns(legaldocumentdto);
        //
        //     //Act 
        //     var result = await legalDocumentService.GetDocumentsByUserId(userid,null);
        //
        //     //Assert 
        //     Assert.Equal(0,result.Count);
        //
        // }

        [Fact]
        public async Task grantaccess()
        {
            //Arrange 
            var legaldocumentdto = new LegalDocumentDto { File = null };
            var legaldocument = new LegalDocument { };
            var docaccessdto = new DocumentAccessDto();
            List<DocumentAccess> documentAccesses = new List<DocumentAccess> { new DocumentAccess { } };
            IEnumerable<LegalDocument> legalDocuments = new List<LegalDocument>
            { new LegalDocument{Accesses = documentAccesses}, new LegalDocument() , new LegalDocument() ,new LegalDocument() };
            var mock = legalDocuments.AsQueryable().BuildMock();
            IEnumerable<Lawyer> lawyers = new List<Lawyer>()
            {
                new Lawyer() , new Lawyer()
            };
            var mock2 = lawyers.AsQueryable().BuildMock();
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.AsQueryable()).Returns(mock);
            _appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable()).Returns(mock2);
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.Update(It.IsAny<LegalDocument>()));
            _appUnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _emailsenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));

            //Act 
            var exception = await Assert.ThrowsAsync<BadArgumentException>(() => legalDocumentService.GrantAccessToLawyer(docaccessdto));

            //Assert 

            Assert.Equal("Lawyer already has access to this document", exception.Message);

        }

        [Fact]
        public async Task takeaccess()
        {
            //Arrange 
            var legaldocumentdto = new LegalDocumentDto { File = null };
            var legaldocument = new LegalDocument { };
            var docaccessdto = new DocumentAccessDto();
            List<DocumentAccess> documentAccesses = new List<DocumentAccess> { new DocumentAccess { } };
            IEnumerable<LegalDocument> legalDocuments = new List<LegalDocument>
            { new LegalDocument{Accesses = documentAccesses}, new LegalDocument() , new LegalDocument() ,new LegalDocument() };
            var mock = legalDocuments.AsQueryable().BuildMock();
            IEnumerable<Lawyer> lawyers = new List<Lawyer>()
            {
                new Lawyer() , new Lawyer()
            };
            var mock2 = lawyers.AsQueryable().BuildMock();
            var lawyerdto = new LawyerDto();
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.AsQueryable()).Returns(mock);
            _appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable()).Returns(mock2);
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.Update(It.IsAny<LegalDocument>()));
            _appUnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _emailsenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            _lawyerServicesMock.Setup(x => x.GetLawyerById(docaccessdto.LawyerId)).ReturnsAsync(lawyerdto);

            //Act 

            var result = await legalDocumentService.TakeAccessFromLawyer(docaccessdto);

            //Assert 

            Assert.True(result);


        }

        [Fact]
        public async Task lawyersaccesstodoument()
        {
            //Arrange 
            int docid = 1;
            var legaldocumentdto = new LegalDocumentDto { File = null };
            var legaldocument = new LegalDocument { };
            var docaccessdto = new DocumentAccessDto();
            List<DocumentAccess> documentAccesses = new List<DocumentAccess> { new DocumentAccess { } };
            IEnumerable<LegalDocument> legalDocuments = new List<LegalDocument>
            { new LegalDocument{Accesses = documentAccesses}, new LegalDocument() , new LegalDocument() ,new LegalDocument() };
            var mock = legalDocuments.AsQueryable().BuildMock();
            IEnumerable<Lawyer> lawyers = new List<Lawyer>()
            {
                new Lawyer() , new Lawyer()
            };
            var mock2 = lawyers.AsQueryable().BuildMock();
            var lawyerdto = new LawyerDto();
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.AsQueryable()).Returns(mock);
            _appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable()).Returns(mock2);
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.Update(It.IsAny<LegalDocument>()));
            _appUnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _emailsenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            _lawyerServicesMock.Setup(x => x.GetLawyerById(It.IsAny<int>())).ReturnsAsync(lawyerdto);

            //Act and Assert 
            //var result = legalDocumentService.GetLawyersThatHaveAccessToDocument(docid);
            var exception = await Assert.ThrowsAsync<BadArgumentException>(() => legalDocumentService.GetLawyersThatHaveAccessToDocument(docid));
            Assert.Equal("document not found", exception.Message);


        }

        [Fact]
        public async Task GetDocumentsThatLawyerHasAccessToByUserId()
        {
            //Arrange 
            int docid = 1;
            var legaldocumentdto = new LegalDocumentDto { File = null };
            var number = 0;
            var legaldocument = new LegalDocument { };
            var docaccessdto = new DocumentAccessDto();
            List<DocumentAccess> documentAccesses = new List<DocumentAccess> { new DocumentAccess { } };
            IEnumerable<LegalDocument> legalDocuments = new List<LegalDocument>
            { new LegalDocument{Accesses = documentAccesses}, new LegalDocument() , new LegalDocument() ,new LegalDocument() };
            var mock = legalDocuments.AsQueryable().BuildMock();
            IEnumerable<Lawyer> lawyers = new List<Lawyer>()
            {
                new Lawyer() , new Lawyer()
            };
            var mock2 = lawyers.AsQueryable().BuildMock();
            var lawyerdto = new LawyerDto();
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.AsQueryable()).Returns(mock);
            _appUnitOfWorkMock.Setup(x => x.LawyerRepo.AsQueryable()).Returns(mock2);
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.Update(It.IsAny<LegalDocument>()));
            _appUnitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            _emailsenderMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            _lawyerServicesMock.Setup(x => x.GetLawyerById(It.IsAny<int>())).ReturnsAsync(lawyerdto);
            _mappermock.Setup(x => x.Map<LegalDocumentDto>(legaldocument)).Returns(legaldocumentdto);

            //Act 
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => legalDocumentService.GetDocumentsThatLawyerHasAccessToByUserId(new LawyerDocumentAccessDto(), null));


        }

        [Fact]
        public async Task UpdateDocumentStatus()
        {
            //Arrange 
            var lawyeruserid = 1;
            IEnumerable<DocumentAccess> documentAccesses = new List<DocumentAccess>
            {
                new DocumentAccess { Lawyer = new Lawyer{ UserId = 1 }  , DocumentId = 1}
            };
            var docmock = documentAccesses.AsQueryable().BuildMock();
            var docdto = new DocumentStatusUpdateDto { DocumentId = lawyeruserid };
            _appUnitOfWorkMock.Setup(x => x.DocumentAccessRepo.AsQueryable()).Returns(docmock);

            //Act 
            await legalDocumentService.UpdateDocumentStatus(docdto, lawyeruserid);

            //Assert 
            Assert.Equal(documentAccesses.First().DocumentStatus, docdto.DocumentStatus);


        }

        [Fact]
        public async Task GetUsersThatLawyerHasAccessToTheirDocuments()
        {
            //Arrnage 
            var lawyeruserid = 1;
            UserDto userDto = new UserDto();
            User user = new User { Id = 1};
            List<DocumentAccess> accesss = new List<DocumentAccess> { new DocumentAccess { LawyerId = 1 } , new DocumentAccess() };
            IEnumerable<LegalDocument> legalDocuments = new List<LegalDocument>
            {
                new LegalDocument { User = user , Accesses = accesss }
            };
            var mock = legalDocuments.AsQueryable().BuildMock();
            _appUnitOfWorkMock.Setup(x => x.DocumentRepo.AsQueryable()).Returns(mock);
            _premiumservice.Setup(x => x.DoseUserHaveAnyActiveSubscription(1)).ReturnsAsync(true);

            //Act and Assert
            var result = legalDocumentService.GetUsersThatLawyerHasAccessToTheirDocuments(lawyeruserid);

        }
    }
}
