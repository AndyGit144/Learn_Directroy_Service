using Domain.Value_Objects;
using Xunit;

namespace Lab16.Tests.ValueObjects;

public class ClassNameTests
{
    [Theory]
    [InlineData((short)1, 'А', "1А")]
    [InlineData((short)5, 'Б', "5Б")]
    [InlineData((short)11, 'Я', "11Я")]
    [InlineData((short)7, 'а', "7А")]
    public void Create_ShouldSucceed_ForValidGradeAndLetter(short grade, char letter, string expected)
    {
        var cn = ClassName.Create(grade, letter);
        Assert.Equal(expected, cn.Value);
    }

    [Theory]
    [InlineData((short)0, 'А')]
    [InlineData((short)12, 'А')]
    [InlineData((short)-1, 'А')]
    public void Create_ShouldThrow_ForInvalidGrade(short grade, char letter)
    {
        Assert.Throws<ArgumentException>(() => ClassName.Create(grade, letter));
    }

    [Theory]
    [InlineData('1')]
    [InlineData('Z')]
    public void Create_ShouldThrow_ForInvalidLetter(char letter)
    {
        Assert.Throws<ArgumentException>(() => ClassName.Create(5, letter));
    }

    [Theory]
    [InlineData("7А", (short)7, 'А')]
    [InlineData("11Б", (short)11, 'Б')]
    public void Parse_ShouldSucceed_ForValidString(string value, short expectedGrade, char expectedLetter)
    {
        var cn = ClassName.Parse(value);
        Assert.Equal(expectedGrade, cn.Grade);
        Assert.Equal(expectedLetter, cn.Letter);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("АА")]
    public void Parse_ShouldThrow_ForInvalidString(string value)
    {
        Assert.Throws<ArgumentException>(() => ClassName.Parse(value));
    }
}
