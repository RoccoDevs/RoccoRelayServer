using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Connections;
using Rocco.RelayServer.Core.Server.Services;
using Xunit;

namespace Rocco.RelayServer.Core.Tests.Services;

public class ConnectionStoreTests
{
    [Fact]
    public void Contains_ContainsGivenString_ReturnsTrue()
    {
        // Arrange
        var mocker = new AutoMoqer();
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var connection = fixture.Create<DefaultConnectionContext>();

        var connectionStore = mocker.Resolve<ConnectionStore>();
        connectionStore.Add(connection);


        // Act
        var result = connectionStore.Contains(
            connection.ConnectionId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Contains_DoesNotContainGivenString_ReturnsFalse()
    {
        // Arrange
        var mocker = new AutoMoqer();
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var connection = fixture.Create<DefaultConnectionContext>();

        var connectionStore = mocker.Resolve<ConnectionStore>();
        connectionStore.Add(connection);


        // Act
        var result = connectionStore.Contains(
            "wrongId");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Contains_NullParam_Throws()
    {
        // Arrange
        var mocker = new AutoMoqer();
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var connection = fixture.Create<DefaultConnectionContext>();

        var connectionStore = mocker.Resolve<ConnectionStore>();
        connectionStore.Add(connection);


        // Act
        connectionStore.Invoking(x => x.Contains(null)).Should().Throw<ArgumentNullException>();
    }


    [Fact]
    public void ConnectionStore_ContainsGivenId_ReturnsConnectionContext()
    {
        // Arrange
        var mocker = new AutoMoqer();
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var connection = fixture.Create<DefaultConnectionContext>();

        var connectionStore = mocker.Resolve<ConnectionStore>();
        connectionStore.Add(connection);


        // Act
        var result = connectionStore[connection.ConnectionId];

        // Assert
        result.Should().BeEquivalentTo(connection);
    }

    [Fact]
    public void ConnectionStore_DoesNotContainGivenId_ReturnsConnectionContext()
    {
        // Arrange
        var mocker = new AutoMoqer();
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var connection = fixture.Create<DefaultConnectionContext>();

        var connectionStore = mocker.Resolve<ConnectionStore>();
        connectionStore.Add(connection);


        // Act
        var result = connectionStore["wrongId"];

        // Assert
        result.Should().BeNull();
    }


    [Fact]
    public void Count_ContainsOneElement_ShouldReturnOne()
    {
        // Arrange
        var mocker = new AutoMoqer();
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var connection = fixture.Create<DefaultConnectionContext>();

        var connectionStore = mocker.Resolve<ConnectionStore>();
        connectionStore.Add(connection);


        // Act
        var result = connectionStore.Count();

        // Assert
        result.Should().Be(1);
    }


    [Fact]
    public void Count_ContainsTwoElements_ShouldReturnTwo()
    {
        // Arrange
        var mocker = new AutoMoqer();
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var connection = fixture.Create<DefaultConnectionContext>();
        var connection2 = fixture.Create<DefaultConnectionContext>();

        var connectionStore = mocker.Resolve<ConnectionStore>();
        connectionStore.Add(connection);
        connectionStore.Add(connection2);


        // Act
        var result = connectionStore.Count();

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void Count_ContainsTwoElements_ShouldReturnOneAfterDeletion()
    {
        // Arrange
        var mocker = new AutoMoqer();
        var fixture = new Fixture().Customize(new AutoMoqCustomization());

        var connection = fixture.Create<DefaultConnectionContext>();
        var connection2 = fixture.Create<DefaultConnectionContext>();

        var connectionStore = mocker.Resolve<ConnectionStore>();
        connectionStore.Add(connection);
        connectionStore.Add(connection2);
        connectionStore.Remove(connection2);


        // Act
        var result = connectionStore.Count();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void Count_ContainsNoElements_ShouldReturnZero()
    {
        // Arrange
        var mocker = new AutoMoqer();

        var connectionStore = mocker.Resolve<ConnectionStore>();


        // Act
        var result = connectionStore.Count();

        // Assert
        result.Should().Be(0);
    }
}