using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Threading;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

using VakilPors.Core.Services;
using Moq;
namespace VakilPors.Test.Core.Services;
public class ThreadCommentServicesTests{
    private  Mock<IAppUnitOfWork> _uow;
    private readonly Mock<ILawyerServices> _lawyerServices;
    private readonly Mock<IPremiumService> _premiumService;
    private readonly Mock<IMapper> _mapper;
    private Mock<IAntiSpam> _antiSpam;
    private ThreadCommentService _service;
    public ThreadCommentServicesTests()
    {
        _antiSpam = new Mock<IAntiSpam>();
        _uow = new Mock<IAppUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _lawyerServices = new Mock<ILawyerServices>();
        _premiumService = new Mock<IPremiumService>();

        _service = new ThreadCommentService(_uow.Object , _mapper.Object,_lawyerServices.Object,_premiumService.Object);
        // _threadDto = new Mock<ThreadDto>();
    }

    [Fact(Skip ="jk")]
    public async void CreateComment_ShouldThrowBadArgumentException_IfCommentIsSpam() {        
        var comment_service = new Mock<ThreadCommentService>();
        var commentDto = new ThreadCommentDto() { Text = "This message is spam." };
        _antiSpam.Setup(x => x.IsSpam(commentDto.Text)).ReturnsAsync("This message is detected as a spam and can not be shown.");
        // comment_service.Setup(s => s.CheckWithin2Minutes(It.IsAny<int>(),It.IsAny<ThreadCommentDto>())).Returns(); 


        await Assert.ThrowsAsync<BadArgumentException>(async () => await comment_service.Object.CreateComment(It.IsAny<int>(),It.IsAny<ThreadCommentDto>()));
    }

    // [Fact]
    // public async Task CreateComment_ShouldThrowBadArgumentException_IfCommentIsSentWithin2MinutesAfterLastComment() {
    //     var commentDto = new ThreadCommentDto() { Text = "This is a valid comment." };
    //     _uow.Setup(x => x.ThreadCommentRepo.CheckWithin2Minutes(1, commentDto)).Returns("wrong");

    //     await Assert.ThrowsExceptionAsync<BadArgumentException>(async () => await _service.CreateComment(1, commentDto));
    // }

    // [Fact]
    // public async Task CreateComment_ShouldCreateCommentAndAddTokenToLawyer_IfCommentIsValid() {
    //     var commentDto = new ThreadCommentDto() { Text = "This is a valid comment." };
    //     _mockUow.Setup(x => x.ThreadCommentRepo.CheckWithin2Minutes(1, commentDto)).Returns("ok");
    //     _mockAntiSpamService.Setup(x => x.IsSpam(commentDto.Text)).Returns("This message is not spam.");

    //     var lawyer = new Lawyer() { UserId = 1 };
    //     _mockUow.Setup(x => x.LawyerRepo.AsQueryable().FirstOrDefaultAsync(x => x.UserId == 1)).Returns(lawyer);

    //     await _service.CreateComment(1, commentDto);

    //     _mockUow.Verify(x => x.ThreadCommentRepo.AddAsync(It.IsAny<ThreadComment>()));
    //     _mockUow.Verify(x => x.SaveChangesAsync());
    //     _mockLawyerService.Verify(x => x.AddToken(1, 1));
    // }

}