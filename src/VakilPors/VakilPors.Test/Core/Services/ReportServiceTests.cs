using Moq;
using MockQueryable.Moq;
using AutoMapper;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Domain.Dtos.Report;
using VakilPors.Core.Contracts.Services;
using VakilPors.Contracts.Repositories;
using VakilPors.Core.Services;
using Org.BouncyCastle.Crypto.Parameters;


public class ReportServiceTests{


    private readonly Mock<IMapper> _mapper;
    private  Mock<IAppUnitOfWork> _uow;
    IReportServices reportService;
    public ReportServiceTests()
    {
         _uow = new Mock<IAppUnitOfWork>();
        _mapper = new Mock<IMapper>();
        reportService = new ReportServices(_uow.Object,_mapper.Object);

    }
    [Fact]
    public async void GetAllReport_HaveOneReport_GetOneReport(){
         var testData = new List<Report>
        {
            new Report
            {
                // Set properties for your report
                Id = 1,
                // ...
                User = new User
                {
                    Id = 101,
                },
                ThreadComment = new ThreadComment
                {
                    Id = 201,
                    User = new User
                    {
                        Id = 301,
                    }
                }
            },
            // Add more test data as needed
        };

        // Configure the reportRepoMock to return the test data
        // var chatRepositoryMock = new Mock<IGenericRepo<Chat>>();
        var reportmock = testData.BuildMock();
        var reportRepositorymock = new Mock<IGenericRepo<Report>>();

        _uow.Setup(x => x.ReportRepo)
            .Returns(reportRepositorymock.Object);
        reportRepositorymock.Setup(x=>x.AsQueryable()).Returns(reportmock);
        // Configure the mapperMock as needed

        // Act
        var result = await reportService.GetAllReport();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<ReportDto>>(result);
        Assert.Equal(testData.Count, result.Count);

        // Additional assertions based on your specific requirements

        // Verify that the necessary methods were called
        // _uow.Verify(x => x.AsQueryable(), Times.Once);
        // Add more verifications as needed
    }
    [Fact]
    public async void GetAllReport_HaveZeroReport_GetZeroReport(){
         var testData = new List<Report>
        {                
        };

        // Configure the reportRepoMock to return the test data
        // var chatRepositoryMock = new Mock<IGenericRepo<Chat>>();
        var reportmock = testData.BuildMock();
        var reportRepositorymock = new Mock<IGenericRepo<Report>>();

        _uow.Setup(x => x.ReportRepo)
            .Returns(reportRepositorymock.Object);
        reportRepositorymock.Setup(x=>x.AsQueryable()).Returns(reportmock);
        // Configure the mapperMock as needed

        // Act
        var result = await reportService.GetAllReport();

        // Assert
        // Assert.Null(result);
        Assert.IsType<List<ReportDto>>(result);
        Assert.Equal(testData.Count, result.Count);

        // Additional assertions based on your specific requirements

        // Verify that the necessary methods were called
        // _uow.Verify(x => x.AsQueryable(), Times.Once);
        // Add more verifications as needed
    }
    
}