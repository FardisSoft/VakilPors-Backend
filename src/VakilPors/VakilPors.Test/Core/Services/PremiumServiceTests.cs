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


    }
}
