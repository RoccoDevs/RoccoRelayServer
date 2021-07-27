using FluentAssertions;
using Rocco.RelayServer.Core.Domain;
using Rocco.RelayServer.Core.Helpers;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.Helpers
{
    public class SixtyNineMessageTypeHelperTests
    {
        [Fact]
        public void ToString_WithCloseType_ReturnsCLOSE()
        {
            // Act
            var result = SixtyNineMessageTypeHelper.ToString(
                SixtyNineMessageType.Close);

            // Assert
            result.Should().Be("CLOSE");
        }

        [Fact]
        public void ToString_WithInitType_ReturnsINIT()
        {
            // Act
            var result = SixtyNineMessageTypeHelper.ToString(
                SixtyNineMessageType.Init);

            // Assert
            result.Should().Be("INIT");
        }

        [Fact]
        public void ToString_WithPAYLOADType_ReturnsMESSAGE()
        {
            // Act
            var result = SixtyNineMessageTypeHelper.ToString(
                SixtyNineMessageType.Payload);

            // Assert
            result.Should().Be("MESSAGE");
        }

        [Fact]
        public void ToString_WithERRORType_ReturnsERROR()
        {
            // Act
            var result = SixtyNineMessageTypeHelper.ToString(
                SixtyNineMessageType.Error);

            // Assert
            result.Should().Be("ERROR");
        }
    }
}