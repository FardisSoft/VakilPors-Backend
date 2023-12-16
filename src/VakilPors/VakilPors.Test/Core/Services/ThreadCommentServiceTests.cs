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
    public class ThreadCommentServiceTests
    {
        private readonly ThreadCommentService threadCommentService;
        private readonly Mock<IAppUnitOfWork> appUnitOfWorkmock;
        private readonly Mock<IMapper> mappermock;
        private readonly Mock<ILawyerServices> lawyerservicesmock;
        private readonly Mock<IPremiumService> premiumservicemock;

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

    }
}
