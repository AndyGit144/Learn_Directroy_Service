using Domain.Value_Objects;
using Xunit;

namespace Lab16.Tests.ValueObjects;

public class TimeSlotTests
{
    [Theory]
    [InlineData(8, 0, 8, 45)]
    [InlineData(9, 30, 10, 10)]
    [InlineData(13, 0, 13, 40)]
    public void Create_ShouldSucceed_ForValidDuration(int sh, int sm, int eh, int em)
    {
        var slot = TimeSlot.Create(new TimeSpan(sh, sm, 0), new TimeSpan(eh, em, 0));

        Assert.Equal(new TimeSpan(sh, sm, 0), slot.StartTime);
        Assert.Equal(new TimeSpan(eh, em, 0), slot.EndTime);
        Assert.True(slot.Duration.TotalMinutes == 40 || slot.Duration.TotalMinutes == 45);
    }

    [Theory]
    [InlineData(8, 0, 7, 59)]
    [InlineData(8, 0, 8, 0)]
    [InlineData(8, 0, 8, 30)]
    [InlineData(8, 0, 9, 0)]
    public void Create_ShouldThrow_ForInvalidDuration(int sh, int sm, int eh, int em)
    {
        Assert.Throws<ArgumentException>(() =>
            TimeSlot.Create(new TimeSpan(sh, sm, 0), new TimeSpan(eh, em, 0)));
    }

    [Theory]
    [InlineData((short)1)]
    [InlineData((short)4)]
    [InlineData((short)8)]
    public void CreateFromLessonNumber_ShouldSucceed_ForValidNumber(short lesson)
    {
        var slot = TimeSlot.CreateFromLessonNumber(lesson);
        Assert.True(slot.StartTime < slot.EndTime);
        Assert.Equal(45, slot.Duration.TotalMinutes);
    }

    [Theory]
    [InlineData((short)0)]
    [InlineData((short)9)]
    [InlineData((short)-1)]
    public void CreateFromLessonNumber_ShouldThrow_ForInvalidNumber(short lesson)
    {
        Assert.Throws<ArgumentException>(() => TimeSlot.CreateFromLessonNumber(lesson));
    }

    [Fact]
    public void OverlapsWith_ShouldReturnTrue_WhenSlotsIntersect()
    {
        var a = TimeSlot.Create(new TimeSpan(8, 0, 0), new TimeSpan(8, 45, 0));
        var b = TimeSlot.Create(new TimeSpan(8, 30, 0), new TimeSpan(9, 15, 0));
        Assert.True(a.OverlapsWith(b));
    }

    [Fact]
    public void OverlapsWith_ShouldReturnFalse_WhenSlotsDoNotIntersect()
    {
        var a = TimeSlot.Create(new TimeSpan(8, 0, 0), new TimeSpan(8, 45, 0));
        var b = TimeSlot.Create(new TimeSpan(9, 0, 0), new TimeSpan(9, 45, 0));
        Assert.False(a.OverlapsWith(b));
    }
}
