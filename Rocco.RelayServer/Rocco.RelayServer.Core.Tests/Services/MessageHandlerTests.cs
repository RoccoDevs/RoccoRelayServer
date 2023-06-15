using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMoq;
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
            var mocker = new AutoMoqer();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            connectionContext.ConnectionId = "someId";
            var socketMessage = fixture.Create<CloseMessage>();
            var messageHandler = mocker.Resolve<MessageHandler>();

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
            var mocker = new AutoMoqer();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            connectionContext.ConnectionId = "someId";
            var expected = fixture.Create<ErrorMessage>();
            var messageHandler = mocker.Resolve<MessageHandler>();

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
            var mocker = new AutoMoqer();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            connectionContext.ConnectionId = "someId";
            var expected = fixture.Create<PayloadMessage>();
            var messageHandler = mocker.Resolve<MessageHandler>();

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
            var mocker = new AutoMoqer();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            connectionContext.ConnectionId = "someId";
            var messageHandler = mocker.Resolve<MessageHandler>();

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
            var mocker = new AutoMoqer();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var messageHandler = mocker.Resolve<MessageHandler>();
            var socketMessage = fixture.Create<InitMessage>();
            var connectionContext = fixture.Create<DefaultConnectionContext>();

            // Act
            var result = messageHandler.HandleMessage(
                connectionContext, socketMessage);

            // Assert
            result.Destination.Should().BeEquivalentTo(socketMessage.Source);
            result.PayloadType.Should().Be(socketMessage.PayloadType);
            result.Source.Should().BeNull();
            result.Source.Should().BeNull();

            mocker.GetMock<ConnectionStore>().Verify(x => x.Add(connectionContext), Times.Once);
            mocker.GetMock<ConnectionStore>().Verify(x => x.Contains(socketMessage.Source), Times.Once);
        }

        [Fact]
        public void
            HandleInitMessage_WithNullSourceAndConnectionStoreDoesNotContainSource_ShouldAddNewConnectionContextAndReturnInitResponse()
        {
            // Arrange
            var mocker = new AutoMoqer();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var messageHandler = mocker.Resolve<MessageHandler>();
            var socketMessage = new InitMessage(null);
            var connectionContext = fixture.Create<DefaultConnectionContext>();

            // Act
            var result = messageHandler.HandleMessage(connectionContext, socketMessage);

            // Assert
            result.Destination.Should().BeEquivalentTo(connectionContext.ConnectionId);
            result.PayloadType.Should().Be(socketMessage.PayloadType);
            result.Source.Should().BeNull();
            result.Payload.Should().BeNull();
        }

        [Fact]
        public void HandleInitMessage_WithOutNullSourceAndConnectionStoreContainSource_ShouldReturnErrorResponse()
        {
            // Arrange
            var mocker = new AutoMoqer();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var messageHandler = mocker.Resolve<MessageHandler>();
            var socketMessage = fixture.Create<InitMessage>();
            var connectionContext = fixture.Create<DefaultConnectionContext>();
            mocker.GetMock<ConnectionStore>().Setup(x => x.Contains(socketMessage.Source)).Returns(true);

            // Act
            var result = messageHandler.HandleMessage(connectionContext, socketMessage);

            // Assert
            result.PayloadType.Should().Be(SixtyNineMessageType.Error);
            result.Destination.Should().BeEquivalentTo(connectionContext.ConnectionId);
            result.Source.Should().BeNull();
            result.Payload?.ToArray().Should().Contain(Encoding.UTF8.GetBytes(socketMessage.Source));

            mocker.GetMock<ConnectionStore>().Verify(x => x.Add(connectionContext), Times.Never);
            mocker.GetMock<ConnectionStore>().Verify(x => x.Contains(socketMessage.Source), Times.Once);
        }
    }
}