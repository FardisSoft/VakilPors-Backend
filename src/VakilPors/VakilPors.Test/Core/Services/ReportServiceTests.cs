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
using VakilPors.Core.Exceptions;


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
    [Fact]
    public async void PostReport_Successful_ReturnTrue(){

// Arrange
        var reportDto = new PostReportDto{
        Description = "Test Description",
        UserId = 1,
        CommentId = 1
        
        };
            
        var report = new List<Report>(){
            new Report{
                Description = reportDto.Description,
                CommentId = reportDto.CommentId,

                UserId = reportDto.UserId,
            },
        
        };
        
        var reportmock = report.BuildMock();
        var reportRepositorymock = new Mock<IGenericRepo<Report>>();

        _mapper.Setup(m => 
            m.Map<PostReportDto, Report>(It.IsAny<PostReportDto>()))
            .Returns(report[0]);

        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _uow.Setup(x => x.ReportRepo)
            .Returns(reportRepositorymock.Object);
        reportRepositorymock.Setup(x=>x.AddAsync(report[0]));

        // Act
        var result = await reportService.PostReport(reportDto);
            
        // Assert
        Assert.True(result);
            
        // Verify 
        _mapper.Verify(m => 
            m.Map<PostReportDto, Report>(reportDto), Times.Once());

        _uow.Verify(uow => 
            uow.ReportRepo.AddAsync(report[0]), Times.Once());

        _uow.Verify(uow => 
            uow.SaveChangesAsync(), Times.Once());    }
    
    [Fact]
    public async void DeleteReport_CantFindReport_ReturnFalse(){

        // var reportmock = report.BuildMock();
        var reportRepositorymock = new Mock<IGenericRepo<Report>>();
        _uow.Setup(x => x.ReportRepo)
            .Returns(reportRepositorymock.Object);
        reportRepositorymock.Setup(x=>x.AsQueryable()).Returns((IQueryable<Report>)null);

        var result = await reportService.DeleteReport(It.IsAny<int>());

        Assert.False(result);
        _uow.Verify(uow => 
            uow.SaveChangesAsync(), Times.Never());
    }
    [Fact]
    public async void DeleteReport_Successful_ReturnTrue(){
        var report = new List<Report>(){
            new Report{
                Description = "Test Description",
                UserId = 1,
            },
        
        };
        var reportmock = report.BuildMock();
        var reportRepositorymock = new Mock<IGenericRepo<Report>>();
        _uow.Setup(x => x.ReportRepo)
            .Returns(reportRepositorymock.Object);
        reportRepositorymock.Setup(x=>x.AsQueryable()).Returns(reportmock);

        var result = await reportService.DeleteReport(It.IsAny<int>());

        Assert.True(result);
        _uow.Verify(uow => 
            uow.SaveChangesAsync(), Times.Once());
        reportRepositorymock.Verify(uow => 
            uow.Remove(report[0]), Times.Once());
    }
    [Fact]
    public async void DeleteReport_AsQueryableThrowException_ThrowException(){

        // var reportmock = report.BuildMock();
        var reportRepositorymock = new Mock<IGenericRepo<Report>>();
        _uow.Setup(x => x.ReportRepo)
            .Returns(reportRepositorymock.Object);
        reportRepositorymock.Setup(x=>x.AsQueryable()). Throws(new Exception());//.Returns((IQueryable<Report>)null);

        var result = await reportService.DeleteReport(It.IsAny<int>());

        Assert.False(result);
        // _uow.Verify(uow => 
        //     uow.SaveChangesAsync(), Times.Never());
    }
}