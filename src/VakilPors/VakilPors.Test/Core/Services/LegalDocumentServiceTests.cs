using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
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
    }
}
