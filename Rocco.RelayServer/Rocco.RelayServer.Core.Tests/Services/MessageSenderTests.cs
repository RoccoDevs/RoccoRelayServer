using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Automoqer;
using Bedrock.Framework.Protocols;
using FluentAssertions;
using Microsoft.AspNetCore.Connections;
using Moq;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Domain.Exceptions;
using Rocco.RelayServer.Core.Server.Services;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.Services
{
    public class MessageSenderTests
    {
        [Fact]
        public async Task TrySendAsync_WithValidMessage_ShouldSend()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageSender>().Build();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var messageSender = mocker.Service;
            SixtyNineMessage requestMessage =
                new PayloadMessage("source", "destination", Encoding.UTF8.GetBytes("payload"));
            var cancellationToken = default(CancellationToken);

            mocker.Param<ConnectionStore>()
                .Setup(x => x[It.IsAny<string>()]).Returns(fixture.Create<DefaultConnectionContext>());

            // Act
            await messageSender.TrySendAsync(
                requestMessage,
                cancellationToken);

            // Assert
            mocker.Param<IMessageWriter<SixtyNineMessage>>()
                .Verify(x =>
                    x.WriteMessage(requestMessage, It.IsAny<PipeWriter>()), Times.Once);

            mocker.Param<ConnectionStore>()
                .Verify(x =>
                    x[requestMessage.Destination], Times.Once);
        }
        
        [Fact]
        public async Task TrySendAsync_FaultyDestination_ShouldError()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageSender>().Build();

            var messageSender = mocker.Service;
            SixtyNineMessage requestMessage =
                new PayloadMessage("source", "destination", Encoding.UTF8.GetBytes("payload"));
            var cancellationToken = default(CancellationToken);

            mocker.Param<ConnectionStore>()
                .Setup(x => x[It.IsAny<string>()]).Returns((ConnectionContext)null);

            // Act
            messageSender.Invoking(async x => await x.TrySendAsync(
                requestMessage,
                cancellationToken)).Should().Throw<ConnectionNotFoundException>().And.Message.Should().Be("destination");

            // Assert
            mocker.Param<IMessageWriter<SixtyNineMessage>>()
                .Verify(x =>
                    x.WriteMessage(requestMessage, It.IsAny<PipeWriter>()), Times.Never);

            mocker.Param<ConnectionStore>()
                .Verify(x =>
                    x[requestMessage.Destination], Times.Once);
        }
        
        [Fact]
        public async Task TrySendAsync_NullDestination_ShouldError()
        {
            // Arrange
            using var mocker = new AutoMoqer<MessageSender>().Build();

            var messageSender = mocker.Service;
            SixtyNineMessage requestMessage =
                new InitMessage("source");
            var cancellationToken = default(CancellationToken);


            // Act
            messageSender.Invoking(async x => await x.TrySendAsync(
                requestMessage,
                cancellationToken)).Should().Throw<ConnectionNotFoundException>().And.Message.Should().Be("null");

            // Assert
            mocker.Param<IMessageWriter<SixtyNineMessage>>()
                .Verify(x =>
                    x.WriteMessage(requestMessage, It.IsAny<PipeWriter>()), Times.Never);

            mocker.Param<ConnectionStore>()
                .Verify(x =>
                    x[requestMessage.Destination], Times.Never);
        }
    }
}