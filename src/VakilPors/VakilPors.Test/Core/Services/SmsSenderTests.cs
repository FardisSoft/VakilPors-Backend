using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services;

public class SMSSenderTests
{
    private readonly Mock<IConfiguration> configurationMock;
    private readonly Mock<ILogger<SMSSender>> loggerMock;
    private readonly Mock<HttpClient> httpClientMock;
    private readonly SMSSender smsSender;

    public SMSSenderTests()
    {
        configurationMock = new Mock<IConfiguration>();
        loggerMock = new Mock<ILogger<SMSSender>>();
        httpClientMock = new Mock<HttpClient>();
        smsSender = new SMSSender(configurationMock.Object, loggerMock.Object, httpClientMock.Object);
    }

    [Fact]
    public async Task SendSmsAsync_ValidParameters_ShouldSendSmsMessage()
    {
        // Arrange
        const string number = "1234567890";
        const string message = "Test message";
        const string username = "testuser";
        const string password = "testpassword";
        const string senderPhoneNumber = "9876543210";

        configurationMock.Setup(c => c["RAYGAN_SMS:SENDER_NUMBER"]).Returns(senderPhoneNumber);
        configurationMock.Setup(c => c["RAYGAN_SMS:USERNAME"]).Returns(username);
        configurationMock.Setup(c => c["RAYGAN_SMS:PASSWORD"]).Returns(password);

        const HttpStatusCode successStatusCode = HttpStatusCode.OK;
        const string successResponse = "0";
        SetupHttpClient(successStatusCode, successResponse);

        // Act
        await smsSender.SendSmsAsync(number, message);

        // Assert
        VerifyHttpClientRequest(number, message, username, password, senderPhoneNumber);
        loggerMock.Verify(
            m => m.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task SendSmsAsync_HttpRequestFails_ShouldThrowInternalServerException()
    {
        // Arrange
        const string number = "1234567890";
        const string message = "Test message";
        const string username = "testuser";
        const string password = "testpassword";
        const string senderPhoneNumber = "9876543210";

        configurationMock.Setup(c => c["RAYGAN_SMS:SENDER_NUMBER"]).Returns(senderPhoneNumber);
        configurationMock.Setup(c => c["RAYGAN_SMS:USERNAME"]).Returns(username);
        configurationMock.Setup(c => c["RAYGAN_SMS:PASSWORD"]).Returns(password);

        const HttpStatusCode failureStatusCode = HttpStatusCode.BadRequest;
        SetupHttpClient(failureStatusCode);

        // Act and Assert
        await Assert.ThrowsAsync<InternalServerException>(() => smsSender.SendSmsAsync(number, message));
        VerifyHttpClientRequest(number, message, username, password, senderPhoneNumber);
        loggerMock.Verify(
            m => m.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Never);
    }

    private void SetupHttpClient(HttpStatusCode statusCode, string responseContent = "")
    {
        httpClientMock
            .Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent)
            });
    }

    private void VerifyHttpClientRequest(string number, string message, string username, string password,
        string senderPhoneNumber)
    {
        httpClientMock.Verify(
            c => c.PostAsync(
                It.Is<string>(url => url == "https://RayganSMS.com/SendMessageWithPost.ashx"),
                It.Is<FormUrlEncodedContent>(content =>
                    GetFormContentField(content, "UserName") == username &&
                    GetFormContentField(content, "Password") == password &&
                    GetFormContentField(content, "PhoneNumber") == senderPhoneNumber &&
                    GetFormContentField(content, "MessageBody") == message &&
                    GetFormContentField(content, "RecNumber") == number &&
                    GetFormContentField(content, "SmsClass") == "1"
                )),
            Times.Once);
    }

    private string GetFormContentField(FormUrlEncodedContent content, string fieldName)
    {
        var fields = content.ReadAsStringAsync().Result;
        var keyValuePairs = fields.Split('&')
            .Select(field => field.Split('='))
            .ToDictionary(field => field[0], field => field[1]);
        return keyValuePairs[fieldName];
    }
}