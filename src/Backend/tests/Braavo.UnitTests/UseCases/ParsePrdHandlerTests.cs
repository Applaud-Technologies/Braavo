using Braavo.Core.UseCases.Prd;
using FluentAssertions;

namespace Braavo.UnitTests.UseCases;

public class ParsePrdHandlerTests
{
    [Fact]
    public void Parse_ExtractsTitle()
    {
        var markdown = """
            # My Product PRD

            ## Overview
            This is the overview.
            """;

        var result = PrdParser.Parse(markdown);

        result.Title.Should().Be("My Product PRD");
    }

    [Fact]
    public void Parse_ExtractsUserStories()
    {
        var markdown = """
            # PRD

            ## User Stories
            - As a user, I want to login so that I can access my account
            - As an admin, I want to manage users so that I can control access
            """;

        var result = PrdParser.Parse(markdown);

        result.UserStories.Should().HaveCount(2);
        result.UserStories[0].AsA.Should().Be("user");
        result.UserStories[0].IWant.Should().Be("login");
    }
}
