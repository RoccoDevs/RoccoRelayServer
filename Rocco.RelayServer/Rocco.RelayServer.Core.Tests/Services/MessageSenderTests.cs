using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMoq;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Moq;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Server.Services;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.Services
{
    public class MessageSenderTests
    {
        [Fact]
        public async Task TrySendAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var mocker = new AutoMoqer();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var messageSender = mocker.Create<MessageSender>();
            SixtyNineSendibleMessage requestMessage =
                new PayloadMessage("source", "destination", Encoding.UTF8.GetBytes("payload"));
            var cancellationToken = default(CancellationToken);

            mocker.GetMock<ConnectionStore>()
                .Setup(x => x[It.IsAny<string>()]).Returns(fixture.Create<DefaultConnectionContext>());

            // Act
            await messageSender.TrySendAsync(
                requestMessage,
                cancellationToken);

            // Assert
            mocker.GetMock<IMessageWriter<SixtyNineSendibleMessage>>()
                .Verify(x =>
                    x.WriteMessage(requestMessage, It.IsAny<PipeWriter>()), Times.Once);

            mocker.GetMock<ConnectionStore>()
                .Verify(x =>
                    x[requestMessage.Destination], Times.Once);
        }
    }
}