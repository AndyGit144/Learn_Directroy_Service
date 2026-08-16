using Domain.Value_Objects;
using Xunit;

namespace Lab16.Tests.ValueObjects;

public class FullNameTests
{
    [Theory]
    [InlineData("Иван", "Иванов", "Иванович")]
    [InlineData("Anna", "Smith", "Maria")]
    [InlineData("Jean-Paul", "Bon", "Pierre")]
    public void Create_ShouldSucceed_ForValidNames(string first, string last, string middle)
    {
        var name = FullName.Create(first, last, middle);
        Assert.Equal(first.Trim(), name.FirstName);
        Assert.Equal(last.Trim(), name.LastName);
        Assert.Equal(middle.Trim(), name.MiddleName);
        Assert.Equal($"{last.Trim()} {first.Trim()[0]}.{middle.Trim()[0]}.", name.ShortName);
    }

    [Theory]
    [InlineData("", "Иванов", "Иванович")]
    [InlineData("Иван", "", "Иванович")]
    [InlineData("Иван", "Иванов", "")]
    [InlineData(null, "Иванов", "Иванович")]
    public void Create_ShouldThrow_ForEmptyComponent(string? first, string last, string middle)
    {
        Assert.Throws<ArgumentException>(() => FullName.Create(first!, last, middle));
    }

    [Theory]
    [InlineData("Иван1", "Иванов", "Иванович")]
    [InlineData("Иван", "Иван@ов", "Иванович")]
    public void Create_ShouldThrow_ForInvalidCharacters(string first, string last, string middle)
    {
        Assert.Throws<ArgumentException>(() => FullName.Create(first, last, middle));
    }
}
