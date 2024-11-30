using devs_who_run_api;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace devs_who_run_test;

public class MemberModuleTest
{
    
    [Fact]
    public async Task GetPartnertMeetup()
    {
        var options = new DbContextOptionsBuilder<DevsWhoRunDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        await using var context = new DevsWhoRunDbContext(options);
        context.Members.AddRange(
            new Member { Id = 1, UserType = UserType.Meetup, Email = "meetup1@test.com", FirstName = "This is", LastName = "Learning", GitHubUserName = "@thisislearning"},
            new Member { Id = 2, UserType = UserType.Conf, Email = "conf1@test.com", FirstName = "This is Learning", LastName = "Conf", GitHubUserName = "@thisislearning"}
        );
        await context.SaveChangesAsync();

        // Act
        var meetups = await context.Members.Where(m => m.UserType == UserType.Meetup).ToListAsync();

        // Assert
        meetups.Should().NotBeNull();
        meetups.Should().HaveCount(1);
        meetups.First().Email.Should().Be("meetup1@test.com");
    }
}