using Domain.Value_Objects;
using Xunit;

namespace Lab16.Tests.ValueObjects;

public class GradeValueTests
{
    [Theory]
    [InlineData((short)2)]
    [InlineData((short)3)]
    [InlineData((short)4)]
    [InlineData((short)5)]
    public void Create_ShouldSucceed_ForValidGrade(short value)
    {
        var g = GradeValue.Create(value);
        Assert.Equal(value, g.NumericValue);
        Assert.False(g.IsAbsent);
        Assert.True(value >= 3 ? g.IsPassing : !g.IsPassing);
    }

    [Theory]
    [InlineData((short)1)]
    [InlineData((short)6)]
    [InlineData((short)0)]
    public void Create_ShouldThrow_ForInvalidGrade(short value)
    {
        Assert.Throws<ArgumentException>(() => GradeValue.Create(value));
    }

    [Fact]
    public void CreateAbsent_ShouldMarkAsAbsent()
    {
        var g = GradeValue.CreateAbsent();
        Assert.True(g.IsAbsent);
        Assert.Null(g.NumericValue);
        Assert.Equal("н", g.DisplayValue);
        Assert.False(g.IsPassing);
    }

    [Theory]
    [InlineData("н")]
    [InlineData("Н")]
    public void Parse_ShouldReturnAbsent_ForLetterN(string value)
    {
        var g = GradeValue.Parse(value);
        Assert.True(g.IsAbsent);
    }

    [Theory]
    [InlineData("4")]
    [InlineData("5")]
    public void Parse_ShouldReturnNumeric_ForDigits(string value)
    {
        var g = GradeValue.Parse(value);
        Assert.False(g.IsAbsent);
        Assert.Equal(short.Parse(value), g.NumericValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("99")]
    public void Parse_ShouldThrow_ForInvalidString(string value)
    {
        Assert.Throws<ArgumentException>(() => GradeValue.Parse(value));
    }
}
