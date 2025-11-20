using System;
using FluentAssertions;
using GamesLoan.Domain.Entities;
using Xunit;

namespace GamesLoan.Tests.Domain;

public class GameTests
{
    [Fact]
    public void Should_throw_when_name_is_empty()
    {
        var act = () => new Game("", new() { "EA" }, new() { "Sports" }, "ext-1");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Name is required*");
    }

    [Fact]
    public void Should_trim_name()
    {
        var game = new Game("  FIFA 25  ", new() { "EA" }, new() { "Sports" }, "ext-1");

        game.Name.Should().Be("FIFA 25");
    }

    [Fact]
    public void Should_mark_as_loaned_and_fail_if_already_loaned()
    {
        var game = new Game("FIFA 25", new() { "EA" }, new() { "Sports" }, "ext-1");

        game.IsLoaned.Should().BeFalse();

        game.MarkAsLoaned();
        game.IsLoaned.Should().BeTrue();

        var act = () => game.MarkAsLoaned();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Game is already loaned*");
    }

    [Fact]
    public void Should_mark_as_returned_only_if_loaned()
    {
        var game = new Game("FIFA 25", new() { "EA" }, new() { "Sports" }, "ext-1");

        var act = () => game.MarkAsReturned();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Game is not currently loaned*");

        game.MarkAsLoaned();
        game.MarkAsReturned();

        game.IsLoaned.Should().BeFalse();
    }
}
