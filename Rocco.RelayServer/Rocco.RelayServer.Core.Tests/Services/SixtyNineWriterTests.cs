using System.Buffers;
using System.Text;
using AutoMoq;
using FluentAssertions;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Services;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.Services;

public class SixtyNineWriterTests
{
    [Fact]
    public void WriteMessage_WithInitMessage_ShouldWrite()
    {
        // Arrange
        var mocker = new AutoMoqer();

        var sixtyNineWriter = mocker.Create<SixtyNineWriter>();
        var message = new InitResponseMessage("dest1");

        var stream = new ArrayBufferWriter<byte>(1024);


        // Act
        sixtyNineWriter.WriteMessage(
            message,
            stream);

        // Assert
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("dest1"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("INIT"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("destination"));
    }

    [Fact]
    public void WriteMessage_WithErrorMessageWithSource_ShouldWrite()
    {
        // Arrange
        var mocker = new AutoMoqer();

        var sixtyNineWriter = mocker.Create<SixtyNineWriter>();
        var message = new ErrorMessage("dest1", Encoding.UTF8.GetBytes("contains"), "numerone");

        var stream = new ArrayBufferWriter<byte>(1024);


        // Act
        sixtyNineWriter.WriteMessage(
            message,
            stream);

        // Assert
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("dest1"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("ERROR"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("destination"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("source"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("contains"));
    }

    [Fact]
    public void WriteMessage_WithErrorMessageWithoutSource_ShouldWrite()
    {
        // Arrange
        var mocker = new AutoMoqer();

        var sixtyNineWriter = mocker.Create<SixtyNineWriter>();
        var message = new ErrorMessage("dest1", Encoding.UTF8.GetBytes("contains"));

        var stream = new ArrayBufferWriter<byte>(1024);


        // Act
        sixtyNineWriter.WriteMessage(
            message,
            stream);

        // Assert
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("dest1"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("ERROR"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("destination"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("contains"));
    }

    [Fact]
    public void WriteMessage_WithPayloadMessage_ShouldWrite()
    {
        // Arrange
        var mocker = new AutoMoqer();

        var sixtyNineWriter = mocker.Create<SixtyNineWriter>();
        var message = new PayloadMessage("src1", "dst1", Encoding.UTF8.GetBytes("contains"));

        var stream = new ArrayBufferWriter<byte>(1024);


        // Act
        sixtyNineWriter.WriteMessage(
            message,
            stream);

        // Assert
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("dst1"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("MESSAGE"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("destination"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("contains"));
        stream.WrittenSpan.ToArray().Should().Contain(Encoding.UTF8.GetBytes("src1"));
    }
}