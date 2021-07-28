using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Automoqer;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Moq;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Interfaces.Services;
using Rocco.RelayServer.Core.Server.ConnectionHandlers;
using Rocco.RelayServer.Core.Services;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.ConnectionHandlers
{
    public class SixtyNineProtocolHandlerTests
    {
        [Fact(Skip = "wip")]
        public async Task OnConnectedAsync_StateUnderTest_ExpectedBehavior()
        {
            
            // Arrange
            var readerMock = new Mock<SixtyNineReader>();
            var ff = new AutoMoqer<PipeReader>().Build();
            var protocolReaderMock = new AutoMoqer<ProtocolReader>()
                .With("reader",ff.Service)
                .Build();
            
            using var sixtyNineProtocolHandlerMock = new AutoMoqer<SixtyNineProtocolHandler>()
                .With("messageReader",readerMock.Object)
                .Build();
            var sixtyNineProtocolHandler = sixtyNineProtocolHandlerMock.Service;
            var token = new CancellationTokenSource();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            ConnectionContext connection = fixture.Build<DefaultConnectionContext>()
                .With(x => x.ConnectionClosed, token.Token)
                .With(x => x.CreateReader(),protocolReaderMock.Service).WithAutoProperties().Create();

            SixtyNineMessage message = new InitMessage("megakek");
            var expected = new ProtocolReadResult<SixtyNineMessage> (message,false,true);

            readerMock.Setup(x => x.TryParseMessage(It.IsAny<ReadOnlySequence<byte>>(),
                ref It.Ref<SequencePosition>.IsAny, ref It.Ref<SequencePosition>.IsAny, out message));
            
            //protocolReaderMock.Setup(x => x.ReadAsync(readerMock.Object,token.Token)).ReturnsAsync(expected);


            // Act
            
            
            var tokenSource2 = new CancellationTokenSource();

            var task = Task.Run(() =>
            {
                sixtyNineProtocolHandler.OnConnectedAsync(
                    connection);
            }, tokenSource2.Token);



            Thread.Sleep(10000000);
            tokenSource2.Cancel();
            
            
            
            // Assert

            sixtyNineProtocolHandlerMock.Param<IMessageHandler>()
                .Verify(x => x.HandleMessage(It.IsAny<ConnectionContext>(), It.IsAny<InitMessage>()), Times.Once);
            task.Dispose();
        }
    }
}