using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using Automoqer;
using FluentAssertions;
using Microsoft.AspNetCore.Connections;
using Moq;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Server.Services;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.Services
{
    public class MessageHandlerTests
    {
        [Fact]
        public void HandleCloseMessage_WithValidConnection_ShouldReturnNull()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageHandler>().Build();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            connectionContext.ConnectionId = "someId";
            var socketMessage = fixture.Create<CloseMessage>();
            var messageHandler = mocker.Service;

            // Act
            var result = messageHandler.HandleMessage(
                connectionContext, socketMessage);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void HandleErrorMessage_WithValidConnection_ExpectedErrorMessage()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageHandler>().Build();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            connectionContext.ConnectionId = "someId";
            var expected = fixture.Create<ErrorMessage>();
            var messageHandler = mocker.Service;

            // Act
            var result = messageHandler.HandleMessage(
                connectionContext, expected);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void HandlePayloadMessage_WithValidConnection_ExpectedPayloadMessage()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageHandler>().Build();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            connectionContext.ConnectionId = "someId";
            var expected = fixture.Create<PayloadMessage>();
            var messageHandler = mocker.Service;

            // Act
            var result = messageHandler.HandleMessage(
                connectionContext, expected);

            // Assert
            result.Should().Be(expected);
        }


        [Fact]
        public void HandleNull_WithValidConnection_ReturnsNull()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageHandler>().Build();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            connectionContext.ConnectionId = "someId";
            var messageHandler = mocker.Service;

            // Act
            var result = messageHandler.HandleMessage(
                connectionContext, null);

            // Assert
            result.Should().BeNull();
        }


        [Fact]
        public void HandleInitMessage_WithSource_ShouldAddNewConnectionContextAndReturnInitResponse()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageHandler>().Build();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var messageHandler = mocker.Service;
            var socketMessage = fixture.Create<InitMessage>();
            var connectionContext = fixture.Create<DefaultConnectionContext>();

            // Act
            var result = messageHandler.HandleMessage(
                connectionContext, socketMessage);

            // Assert
            result.Destination.Should().BeEquivalentTo(socketMessage.Source);
            result.PayloadType.Should().BeEquivalentTo(socketMessage.PayloadType);
            result.Source.Should().BeNull();
            result.Source.Should().BeNull();

            mocker.Param<ConnectionStore>().Verify(x => x.Add(connectionContext), Times.Once);
            mocker.Param<ConnectionStore>().Verify(x => x.Contains(socketMessage.Source), Times.Once);
        }

        [Fact]
        public void
            HandleInitMessage_WithNullSourceAndConnectionStoreDoesNotContainSource_ShouldAddNewConnectionContextAndReturnInitResponse()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageHandler>().Build();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var messageHandler = mocker.Service;
            var socketMessage = new InitMessage(null);
            var connectionContext = fixture.Create<DefaultConnectionContext>();

            // Act
            var result = messageHandler.HandleMessage(connectionContext, socketMessage);

            // Assert
            result.Destination.Should().BeEquivalentTo(connectionContext.ConnectionId);
            result.PayloadType.Should().BeEquivalentTo(socketMessage.PayloadType);
            result.Source.Should().BeNull();
            result.Payload.Should().BeNull();
        }

        [Fact]
        public void HandleInitMessage_WithOutNullSourceAndConnectionStoreContainSource_ShouldReturnErrorResponse()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageHandler>().Build();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var messageHandler = mocker.Service;
            var socketMessage = fixture.Create<InitMessage>();
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            mocker.Param<ConnectionStore>().Setup(x => x.Contains(socketMessage.Source)).Returns(true);

            // Act
            var result = messageHandler.HandleMessage(connectionContext, socketMessage);

            // Assert
            result.PayloadType.Should().BeEquivalentTo(SixtyNineMessageType.Error);
            result.Destination.Should().BeEquivalentTo(connectionContext.ConnectionId);
            result.Source.Should().BeNull();
            result.Payload?.ToArray().Should().Contain(Encoding.UTF8.GetBytes(socketMessage.Source));

            mocker.Param<ConnectionStore>().Verify(x => x.Add(connectionContext), Times.Never);
            mocker.Param<ConnectionStore>().Verify(x => x.Contains(socketMessage.Source), Times.Once);
        }
    }
}