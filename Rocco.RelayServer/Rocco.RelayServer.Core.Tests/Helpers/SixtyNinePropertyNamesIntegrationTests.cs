using System.Text.Json;
using FluentAssertions;
using Rocco.RelayServer.Core.Helpers;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.Helpers
{
    public class SixtyNinePropertyNamesIntegrationTests
    {
        [Fact]
        public void PayloadTypePropertyNameBytes_ReturnsExpected()
        {
            // Act

            var jsonEncodedBytes = SixtyNinePropertyNames.PayloadTypePropertyNameBytes;
            // Assert
            jsonEncodedBytes.Should().Be(JsonEncodedText.Encode("payloadType"));
        }

        [Fact]
        public void PayloadTypeInitPropertyValue_ReturnsExpected()
        {
            // Act

            var jsonEncodedBytes = SixtyNinePropertyNames.PayloadTypeInitPropertyValue;
            // Assert
            jsonEncodedBytes.Should().Be(JsonEncodedText.Encode("INIT"));
        }

        [Fact]
        public void PayloadTypePayloadPropertyValue_ReturnsExpected()
        {
            // Act

            var jsonEncodedBytes = SixtyNinePropertyNames.PayloadTypePayloadPropertyValue;
            // Assert
            jsonEncodedBytes.Should().Be(JsonEncodedText.Encode("MESSAGE"));
        }

        [Fact]
        public void PayloadTypeErrorPropertyValue_ReturnsExpected()
        {
            // Act

            var jsonEncodedBytes = SixtyNinePropertyNames.PayloadTypeErrorPropertyValue;
            // Assert
            jsonEncodedBytes.Should().Be(JsonEncodedText.Encode("ERROR"));
        }

        [Fact]
        public void PayloadTypeClosePropertyValue_ReturnsExpected()
        {
            // Act

            var jsonEncodedBytes = SixtyNinePropertyNames.PayloadTypeClosePropertyValue;
            // Assert
            jsonEncodedBytes.Should().Be(JsonEncodedText.Encode("CLOSE"));
        }
    }
}