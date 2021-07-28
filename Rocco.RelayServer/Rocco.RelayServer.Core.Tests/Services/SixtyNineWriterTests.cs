using System.Buffers;
using System.Text;
using Automoqer;
using FluentAssertions;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Services;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.Services
{
    public class SixtyNineWriterTests
    {
        [Fact]
        public void WriteMessage_WithInitMessageHasSource_ShouldWrite()
        {
            // Arrange
            using var mocker = new AutoMoqer<SixtyNineWriter>().Build();

            var sixtyNineWriter = mocker.Service;
            var message = new InitMessage("scr1");

            var stream = new ArrayBufferWriter<byte>(1024);


            // Act
            sixtyNineWriter.WriteMessage(
                message,
                stream);

            // Assert
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("scr1"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("INIT"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("source"));
        }
        
        [Fact]
        public void WriteMessage_WithInitMessageHasNoSourceOrDestination_ShouldWrite()
        {
            // Arrange
            using var mocker = new AutoMoqer<SixtyNineWriter>().Build();

            var sixtyNineWriter = mocker.Service;
            var message = new InitMessage(null);

            var stream = new ArrayBufferWriter<byte>(1024);


            // Act
            sixtyNineWriter.WriteMessage(
                message,
                stream);

            // Assert
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("INIT"));
        }
        
        [Fact]
        public void WriteMessage_WithInitMessageHasDestination_ShouldWrite()
        {
            // Arrange
            using var mocker = new AutoMoqer<SixtyNineWriter>().Build();

            var sixtyNineWriter = mocker.Service;
            var message = new InitMessage(null,"test");

            var stream = new ArrayBufferWriter<byte>(1024);


            // Act
            sixtyNineWriter.WriteMessage(
                message,
                stream);

            // Assert
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("INIT"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("test"));
        }
        
        [Fact]
        public void WriteMessage_WithCloseMessage_ShouldWrite()
        {
            // Arrange
            using var mocker = new AutoMoqer<SixtyNineWriter>().Build();

            var sixtyNineWriter = mocker.Service;
            var message = new CloseMessage();

            var stream = new ArrayBufferWriter<byte>(1024);


            // Act
            sixtyNineWriter.WriteMessage(
                message,
                stream);

            // Assert
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("CLOSE"));
        }
        
        [Fact]
        public void WriteMessage_WithInitResponseMessage_ShouldWrite()
        {
            // Arrange
            using var mocker = new AutoMoqer<SixtyNineWriter>().Build();

            var sixtyNineWriter = mocker.Service;
            var message = new InitResponseMessage("dest1");

            var stream = new ArrayBufferWriter<byte>(1024);


            // Act
            sixtyNineWriter.WriteMessage(
                message,
                stream);

            // Assert
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("dest1"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("INIT"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("destination"));
        }

        [Fact]
        public void WriteMessage_WithErrorMessageWithSource_ShouldWrite()
        {
            // Arrange
            using var mocker = new AutoMoqer<SixtyNineWriter>().Build();

            var sixtyNineWriter = mocker.Service;
            var message = new ErrorMessage("dest1", Encoding.UTF8.GetBytes("ContainInOrders"), "numerone");

            var stream = new ArrayBufferWriter<byte>(1024);


            // Act
            sixtyNineWriter.WriteMessage(
                message,
                stream);

            // Assert
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("dest1"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("ERROR"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("destination"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("source"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("ContainInOrders"));
        }

        [Fact]
        public void WriteMessage_WithErrorMessageWithoutSource_ShouldWrite()
        {
            // Arrange
            using var mocker = new AutoMoqer<SixtyNineWriter>().Build();

            var sixtyNineWriter = mocker.Service;
            var message = new ErrorMessage("dest1", Encoding.UTF8.GetBytes("ContainInOrders"));

            var stream = new ArrayBufferWriter<byte>(1024);


            // Act
            sixtyNineWriter.WriteMessage(
                message,
                stream);

            // Assert
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("dest1"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("ERROR"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("destination"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("ContainInOrders"));
        }

        [Fact]
        public void WriteMessage_WithPayloadMessage_ShouldWrite()
        {
            // Arrange
            using var mocker = new AutoMoqer<SixtyNineWriter>().Build();

            var sixtyNineWriter = mocker.Service;
            var message = new PayloadMessage("src1", "dst1", Encoding.UTF8.GetBytes("ContainInOrders"));

            var stream = new ArrayBufferWriter<byte>(1024);


            // Act
            sixtyNineWriter.WriteMessage(
                message,
                stream);

            // Assert
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("dst1"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("MESSAGE"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("destination"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("ContainInOrders"));
            stream.WrittenSpan.ToArray().Should().ContainInOrder(Encoding.UTF8.GetBytes("src1"));
        }
        
       
    }
}