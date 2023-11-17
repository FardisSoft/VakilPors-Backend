using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services
{
    public class AntiSpamServiceTests
    {
        private readonly AntiSpamService antiSpamService;

        public AntiSpamServiceTests()
        {
            antiSpamService = new AntiSpamService();
        }

        [Fact]
        public async Task is_spam()
        {
            //Arrange 
            var input = "this is a test @alireza";
            var input2 = "09306224674";
            var input3 = "this is www.sample.com hahah";
            var input4 = "this an ok test";

            //Act
            var result =await  antiSpamService.IsSpam(input);
            var result2 = await antiSpamService.IsSpam(input2);
            var result3 = await antiSpamService.IsSpam(input3);
            var result4 = await antiSpamService.IsSpam(input4);

            //Assert 
            Assert.Equal("This message is detected as a spam and can not be shown.", result);
            Assert.Equal("This message is detected as a spam and can not be shown.", result2);
            Assert.Equal("This message is detected as a spam and can not be shown.", result3);
            Assert.Equal("ok", result4);

        }
        [Fact]
        public async Task spam_check()
        {
            //Arrange 
            var input = "this is a test @alireza";
            var input2 = "09306224674";
            var input3 = "this is www.sample.com hahah";
            var input4 = "this an ok test";

            //Act
            var result = await antiSpamService.SpamCheck(input);
            var result2 = await antiSpamService.SpamCheck(input2);
            var result3 = await antiSpamService.SpamCheck(input3);
            var result4 = await antiSpamService.SpamCheck(input4);

            //Assert 
            Assert.True(result);
            Assert.True(result2);
            Assert.True(result3);
            Assert.False(result4);

        }
        [Fact]
        public async Task repeated_sequence()
        {
            //Arrange 
            var input = "alialialialialialialialialialialialialialialialialialialai";
            var input2 = "ok";

            //Act
            var result =await  antiSpamService.CheckForRepeatedSequences(input);
            var result2 = await antiSpamService.CheckForRepeatedSequences(input2);

            //Assert
            Assert.True(result);
            Assert.False(result2);
        }
    }
}
