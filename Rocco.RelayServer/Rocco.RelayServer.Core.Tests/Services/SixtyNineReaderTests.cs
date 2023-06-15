using System;
using System.Buffers;
using System.Collections.Generic;
using AutoMoq;
using FluentAssertions;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Services;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.Services;

public class SixtyNineReaderTests
{
    [Fact]
    public void TryParseMessage_WithInitMessage_ShouldYieldInitResponse()
    {
        // Arrange
        var initMessage = new byte[]
        {
            0x00, 0x00, 0x00, 0x23, 0x7B, 0x22, 0x70, 0x61, 0x79, 0x6C, 0x6F, 0x61, 0x64, 0x54, 0x79, 0x70, 0x65,
            0x22, 0x3A, 0x22, 0x49, 0x4E, 0x49, 0x54, 0x22, 0x2C, 0x22, 0x73, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x22,
            0x3A, 0x22, 0x31, 0x22, 0x7D
        };

        var mocker = new AutoMoqer();
        var sixtyNineReader = mocker.Create<SixtyNineReader>();
        var input = new ReadOnlySequence<byte>(initMessage);
        var consumed = new SequencePosition(input, 0);
        var examined = new SequencePosition(input, 0);

        var messages = new List<SixtyNineMessage>();
        // Act
        while (sixtyNineReader.TryParseMessage(in input, ref consumed, ref examined, out var message))
        {
            input = input.Slice(consumed);
            // Assert
            messages.Add(message);
        }

        messages.Count.Should().Be(1);
        messages[0].Should().BeOfType<InitMessage>();
    }

    [Fact]
    public void TryParseMessage_WithPayloadMessage_ShouldYieldInitResponse()
    {
        // Arrange
        var payloadMessage = new byte[]
        {
            0x00, 0x00, 0x00, 0x4D, 0x7B, 0x22, 0x70, 0x61, 0x79, 0x6C, 0x6F, 0x61, 0x64, 0x54, 0x79, 0x70, 0x65,
            0x22, 0x3A, 0x22, 0x4D, 0x45, 0x53, 0x53, 0x41, 0x47, 0x45, 0x22, 0x2C, 0x22, 0x73, 0x6F, 0x75, 0x72,
            0x63, 0x65, 0x22, 0x3A, 0x22, 0x31, 0x22, 0x2C, 0x22, 0x64, 0x65, 0x73, 0x74, 0x69, 0x6E, 0x61, 0x74,
            0x69, 0x6F, 0x6E, 0x22, 0x3A, 0x22, 0x30, 0x22, 0x2C, 0x22, 0x70, 0x61, 0x79, 0x6C, 0x6F, 0x61, 0x64,
            0x22, 0x3A, 0x22, 0x50, 0x61, 0x79, 0x6C, 0x6F, 0x61, 0x64, 0x30, 0x22, 0x7D
        };

        var mocker = new AutoMoqer();
        var sixtyNineReader = mocker.Create<SixtyNineReader>();
        var input = new ReadOnlySequence<byte>(payloadMessage);
        var consumed = new SequencePosition(input, 0);
        var examined = new SequencePosition(input, 0);

        var messages = new List<SixtyNineMessage>();
        // Act
        while (sixtyNineReader.TryParseMessage(in input, ref consumed, ref examined, out var message))
        {
            input = input.Slice(consumed);
            // Assert
            messages.Add(message);
        }

        messages.Count.Should().Be(1);
        messages[0].Should().BeOfType<PayloadMessage>();
    }
}