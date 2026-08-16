using Domain.Value_Objects;
using Xunit;

namespace Lab16.Tests.ValueObjects;

public class EntityLifetimeTests
{
    [Fact]
    public void Create_ShouldInitializeCreatedAndUpdatedToNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var lt = EntityLifetime.Create();
        var after = DateTime.UtcNow.AddSeconds(1);

        Assert.InRange(lt.CreatedAt, before, after);
        Assert.Equal(lt.CreatedAt, lt.UpdatedAt);
    }

    [Fact]
    public void CreateFrom_ShouldSucceed_ForPastDates()
    {
        var created = DateTime.UtcNow.AddDays(-10);
        var updated = DateTime.UtcNow.AddDays(-1);
        var lt = EntityLifetime.CreateFrom(created, updated);
        Assert.Equal(created, lt.CreatedAt);
        Assert.Equal(updated, lt.UpdatedAt);
    }

    [Fact]
    public void CreateFrom_ShouldThrow_WhenCreatedInFuture()
    {
        var future = DateTime.UtcNow.AddDays(1);
        Assert.Throws<ArgumentException>(() =>
            EntityLifetime.CreateFrom(future, DateTime.UtcNow));
    }

    [Fact]
    public void CreateFrom_ShouldThrow_WhenUpdatedInFuture()
    {
        var future = DateTime.UtcNow.AddDays(1);
        Assert.Throws<ArgumentException>(() =>
            EntityLifetime.CreateFrom(DateTime.UtcNow, future));
    }

    [Fact]
    public void CreateFrom_ShouldThrow_WhenCreatedAfterUpdated()
    {
        var earlier = DateTime.UtcNow.AddDays(-5);
        var later = DateTime.UtcNow.AddDays(-2);
        Assert.Throws<ArgumentException>(() =>
            EntityLifetime.CreateFrom(later, earlier));
    }

    [Fact]
    public void MarkAsUpdated_ShouldAdvanceUpdatedAt()
    {
        var lt = EntityLifetime.CreateFrom(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(-5));
        var oldUpdated = lt.UpdatedAt;
        Thread.Sleep(10);
        var returned = lt.MarkAsUpdated();
        Assert.True(lt.UpdatedAt > oldUpdated);
        Assert.Same(lt, returned);
    }
}
