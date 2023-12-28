using Xunit;

using VakilPors.Core.Services;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Logging;
using Moq;
using MailKit;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using System.Formats.Asn1;

using Microsoft.AspNetCore.Identity;
// using Moq;
using VakilPors.Contracts.Repositories;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
// using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services;

// [TestFixture]
public class EmailSenderTests{
    // [Test]
    // public void SendEmailAsync_SuccessfullySendEmail_InvokeTrueFunction(){
    // } 
    private Mock<SmtpClient> _smtpclient;
    private Mock<MimeMessage> _mimemessage;
    private Mock<ILogger<EmailSender>> _logger;
    private Mock<IConfiguration> _config;
    
    public EmailSenderTests(){
        _smtpclient = new Mock<SmtpClient>();
        _mimemessage = new Mock<MimeMessage>();
        _logger = new Mock<ILogger<EmailSender>>();
        _logger.SetupAllProperties(); // This is necessary to use the LogCapture extension method.

        _config = new Mock<IConfiguration>();
           _config.Setup(c => c["Email:Name"]).Returns("YourName");
    _config.Setup(c => c["Email:From"]).Returns("YourFromEmail");
    _config.Setup(c => c["Email:Host"]).Returns("YourHost");
    _config.Setup(c => c["Email:Port"]).Returns("25"); // Port as a string
    _config.Setup(c => c["Email:Username"]).Returns("YourUsername");
    _config.Setup(c => c["Email:Password"]).Returns("YourPassword");
    _config.Setup(c => c["Email:UseSSL"]).Returns("true"); // UseSSL as a string
    }
    [Fact]
    public async Task SendEmailAsync_SuccessfullySendEmail_InvokeTrueFunction(){
        // #region Arrange
 
        var email = "recipient@example.com";
        var name = "Recipient Name";
        var subject = "Test Email";
        var htmlMessage = "<p>This is a test email.</p>";
        var useHtml = true;
        var fromName = "Your Name";
        var fromEmail = "from@example.com";

        
        // using var emailService = new EmailSender(_config.Object,_logger.Object);// ,"somename" , "someemail"); // Create an instance of your email service
        var emailService = new EmailSender(_config.Object,_logger.Object ,_mimemessage.Object, _smtpclient.Object);// ,"somename" , "someemail"); // Create an instance of your email service

        await emailService.SendEmailAsync(email, name, subject, htmlMessage);

        _smtpclient.Verify(client => client.SendAsync(It.IsAny<MimeMessage>(),It.IsAny<CancellationToken>() ,It.IsAny<ITransferProgress>()));
    }
    [Fact]
    public async Task SendEmailAsync_FailedToSendEmail_LogError(){
 
        var email = "recipient@example.com";
        var name = "Recipient Name";
        var subject = "Test Email";
        var htmlMessage = "<p>This is a test email.</p>";
        // var useHtml = true;
        // var fromName = "Your Name";
        // var fromEmail = "from@example.com";        
        _smtpclient.Setup(client => client.SendAsync(It.IsAny<MimeMessage>(),
        It.IsAny<CancellationToken>() ,
        It.IsAny<ITransferProgress>())).
        Throws(new Exception());//(new SystemException("An error occurred while sending the email."));

        // using var emailService = new EmailSender(_config.Object,_logger.Object );//,"somename" , "someemail"); // Create an instance of your email service
        var emailService = new EmailSender(_config.Object,_logger.Object);//,_smtpclient.Object);//,"somename" , "someemail"); // Create an instance of your email service
        
        await emailService.SendEmailAsync(email, name, subject, htmlMessage);

        
        _logger.Verify(l =>
            l.Log(LogLevel.Error,
                It.IsAny<EventId>(),
                // It.Is<Exception>(e => e is Exception),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("An Error occured while sending Email")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),Times.Once
    );
        // _logger.Verify(l => l.Log(LogLevel.Information, $"Email sent to test@example.com."));

        // _logger.Verify(
        //     x => x.LogInformation(
        //         It.IsAny<string>(), // You can use more specific matchers here
        //         It.IsAny<object[]>()), // Additional parameters, if any
        //     Times.Once);
        // _logger.Verify(log => log.LogError(It.IsAny<string>()));
        // _logger.Verify(log => log.LogError($"An Error occured while sending Email YourName to {email} with subj:{subject} name:{name}"), Times.Once);



        // Assert.ThrowsAsync<SystemException>( () =>   emailService.SendEmailAsync(email, name, subject, htmlMessage));



    }
}