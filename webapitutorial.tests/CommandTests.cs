using System;
using webapitutorial.Models;
using Xunit;

namespace webapitutorial.tests
{
    public class CommandTests : IDisposable
    {
        Command _testCommand;

        public CommandTests()
        {
            _testCommand = new Command
            {
                HowTo = "Do something",
                Platform = "Some platform",
                Line = "Some commandline"
            };
        }

        public void Dispose()
        {
            _testCommand = null;
        }


        [Fact]
        public void CanChangeHowTo()
        {
            //Arrange

            //Act
            _testCommand.HowTo = "Execute Unit Tests";

            //Assert
            Assert.Equal("Execute Unit Tests", _testCommand.HowTo);
        }

        [Fact]
        public void CanChangePlatform()
        {
            //Arrange

            //Act
            _testCommand.Platform = "New Platform";

            //Assert
            Assert.Equal("New Platform", _testCommand.Platform);
        }

        [Fact]
        public void CanChangeLine()
        {
            //Arrange

            //Act
            _testCommand.Line = "new Line";

            //Assert
            Assert.Equal("new Line", _testCommand.Line);
        }
    }
}
