using Domain.Entities;
using Xunit;

namespace Lab16.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("a.b+c@sub.example.co")]
    [InlineData("test123@x.io")]
    public void Create_ShouldSucceed_ForValidEmail(string value)
    {
        var email = Email.Create(value);
        Assert.Equal(value.Trim(), email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("plainstring")]
    [InlineData("noatsign.com")]
    [InlineData("a@b")]
    [InlineData("missingdot@com")]
    public void Create_ShouldThrow_ForInvalidEmail(string? value)
    {
        Assert.Throws<ArgumentException>(() => Email.Create(value!));
    }

    [Fact]
    public void Create_ShouldThrow_WhenExceedsMaxLength()
    {
        var longLocal = new string('a', 80);
        var value = longLocal + "@x.io";
        Assert.Throws<ArgumentException>(() => Email.Create(value));
    }
}
