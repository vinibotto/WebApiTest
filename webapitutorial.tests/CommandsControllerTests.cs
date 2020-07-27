using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using webapitutorial.Controllers;
using webapitutorial.Data;
using webapitutorial.Dtos;
using webapitutorial.Models;
using webapitutorial.Profiles;
using Xunit;

namespace webapitutorial.tests
{
    public class CommandsControllerTests : IDisposable
    {
        DbContextOptionsBuilder<CommanderContext> _optionsBuilder;
        CommanderContext _dbContext;
        CommandsController _controller;
        SqlCommanderRepo _commanderRepo;
        IMapper _mapper;


        public CommandsControllerTests()
        {
            _optionsBuilder = new DbContextOptionsBuilder<CommanderContext>();
            _optionsBuilder.UseInMemoryDatabase("UnitTestInMemBD");
            _dbContext = new CommanderContext(_optionsBuilder.Options);

            //Mapper
            var myProfile = new CommandsProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            _mapper = new Mapper(configuration);

            _commanderRepo = new SqlCommanderRepo(_dbContext);

            _controller = new CommandsController(_commanderRepo, _mapper);
        }

        public void Dispose()
        {
            _optionsBuilder = null;
            foreach (var cmd in _dbContext.Commands)
            {
                _dbContext.Commands.Remove(cmd);
            }
            _dbContext.SaveChanges();
            _dbContext.Dispose();
            _mapper = null;
            _commanderRepo = null;
            _controller = null;
        }



        [Fact]
        public void GetCommandItems_ReturnsNull_WhenDBIsEmpty()
        {
            //Arrange

            //ACT
            var actionResult = _controller.GetAllCommands();

            var result = actionResult.Result as OkObjectResult;

            //ASSERT
            Assert.NotNull(result);
            var value = (IEnumerable<CommandReadDto>)result.Value;
            Assert.Empty(value);
        }

        [Fact]
        public void GetCommandItemsReturnsOneItemWhenDBHasOneObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            _dbContext.Commands.Add(command);
            _dbContext.SaveChanges();

            //Act
            var actionResult = _controller.GetAllCommands();

            var result = actionResult.Result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            var value = (IEnumerable<CommandReadDto>)result.Value;
            Assert.Single(value);
        }

        [Fact]
        public void GetCommandItemsReturnNItemsWhenDBHasNObjects()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            var command2 = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            _dbContext.Commands.Add(command);
            _dbContext.Commands.Add(command2);
            _dbContext.SaveChanges();
            //Act
            var actionResult = _controller.GetAllCommands();

            var result = actionResult.Result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            var value = (IEnumerable<CommandReadDto>)result.Value;
            Assert.Equal(2, value.Count());
        }

        [Fact]
        public void GetCommandItemsReturnsTheCorrectType()
        {
            //Arrange
            //Act
            var result = _controller.GetAllCommands();
            //Assert
            Assert.IsType<ActionResult<IEnumerable<CommandReadDto>>>(result);
        }


        [Fact]
        public void GetCommandItemReturnsNullResultWhenInvalidID()
        {
            //Arrange
            //DB should be empty, any ID will be invalid
            //Act
            var result = _controller.GetCommandById(0);
            //Assert
            Assert.Null(result.Value);
        }

        [Fact]
        public void GetCommandItemReturns404NotFoundWhenInvalidID()
        {
            //Arrange
            //DB should be empty, any ID will be invalid
            //Act
            var result = _controller.GetCommandById(0);
            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetCommandItemReturnsTheCorrectType()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            _dbContext.Commands.Add(command);
            _dbContext.SaveChanges();
            var cmdId = command.Id;

            //Act
            var result = _controller.GetCommandById(cmdId);

            //Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void GetCommandItemReturnsTheCorrectResouce()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            _dbContext.Commands.Add(command);
            _dbContext.SaveChanges();
            var cmdId = command.Id;
            //Act
            var actionResult = _controller.GetCommandById(cmdId);
            var result = actionResult.Result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            var value = (CommandReadDto)result.Value;
            Assert.Equal(cmdId, value.Id);
        }

        [Fact]
        public void PostCommandItemObjectCountIncrementWhenValidObject()
        {
            //Arrange
            var command = new CommandCreateDto
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            var oldCount = _dbContext.Commands.Count();
            //Act
            var result = _controller.CreateCommand(command);
            //Assert
            Assert.Equal(oldCount + 1, _dbContext.Commands.Count());
        }

        [Fact]
        public void PostCommandItemReturns201CreatedWhenValidObject()
        {
            //Arrange
            var command = new CommandCreateDto
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            //Act
            var result = _controller.CreateCommand(command);
            //Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }


        [Fact]
        public void PutCommandItem_AttributeUpdated_WhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };

            var commandDto = new CommandUpdateDto
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            _dbContext.Commands.Add(command);
            _dbContext.SaveChanges();
            var cmdId = command.Id;
            command.HowTo = "UPDATED";

            //Act
            _controller.UpdateCommand(cmdId, commandDto);
            var result = _dbContext.Commands.Find(cmdId);
            //Assert
            Assert.Equal(command.HowTo, result.HowTo);
        }

        [Fact]
        public void PutCommandItem_Returns204_WhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };

            var commandDto = new CommandUpdateDto
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };

            _dbContext.Commands.Add(command);
            _dbContext.SaveChanges();
            var cmdId = command.Id;
            command.HowTo = "UPDATED";
            //Act
            var result = _controller.UpdateCommand(cmdId, commandDto);
            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void PutCommandItem_Returns400_WhenInvalidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };

            var commandDto = new CommandUpdateDto
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };

            _dbContext.Commands.Add(command);
            _dbContext.SaveChanges();
            var cmdId = command.Id + 1;
            command.HowTo = "UPDATED";
            //Act
            var result = _controller.UpdateCommand(cmdId, commandDto);
            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteCommandItem_ObjectsDecrement_WhenValidObjectID()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            _dbContext.Commands.Add(command);
            _dbContext.SaveChanges();
            var cmdId = command.Id;
            var objCount = _dbContext.Commands.Count();
            //Act
            _controller.DeleteCommand(cmdId);
            //Assert
            Assert.Equal(objCount - 1, _dbContext.Commands.Count());
        }

        [Fact]
        public void DeleteCommandItem_Returns204NoContent_WhenValidObjectID()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            _dbContext.Commands.Add(command);
            _dbContext.SaveChanges();
            var cmdId = command.Id;
            //Act
            var result = _controller.DeleteCommand(cmdId);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteCommandItem_Returns404NotFound_WhenValidObjectID()
        {
            //Arrange

            //Act
            var result = _controller.DeleteCommand(-1);
            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteCommandItem_ObjectCountNotDecremented_WhenValidObjectID()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                Line = "Some Command"
            };
            _dbContext.Commands.Add(command);
            _dbContext.SaveChanges();
            var cmdId = command.Id;
            var objCount = _dbContext.Commands.Count();
            //Act
            var result = _controller.DeleteCommand(cmdId + 1);
            //Assert
            Assert.Equal(objCount, _dbContext.Commands.Count());
        }


    }
}
