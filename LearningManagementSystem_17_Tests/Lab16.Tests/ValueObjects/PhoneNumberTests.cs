using Domain.Entities;
using Xunit;

namespace Lab16.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+7 (123) 456 78-90")]
    [InlineData("+7 123 456 78 90")]
    [InlineData("+7-123-456-78-90")]
    [InlineData("+11234567890")]
    public void Create_ShouldSucceed_ForValidPhone(string value)
    {
        var phone = PhoneNumber.Create(value);
        Assert.Equal(value.Trim(), phone.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("1234567")]
    [InlineData("+abc 123 456 78 90")]
    public void Create_ShouldThrow_ForInvalidPhone(string? value)
    {
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create(value!));
    }
}
