using Domain.Entities;
using Domain.Entities.LMS.Domain.Aggregates;
using Domain.Enums;
using Xunit;

namespace Lab16.Tests.BusinessLogic;

public class LessonTests
{
    private static Lesson CreateValidLesson(int daysFromToday = 5)
    {
        return Lesson.Create(
            subjectId: Guid.NewGuid(),
            teacherId: Guid.NewGuid(),
            classId: Guid.NewGuid(),
            classRoomId: Guid.NewGuid(),
            date: DateTime.Today.AddDays(daysFromToday),
            lessonNumber: 1,
            topic: "Введение");
    }

    [Fact]
    public void Create_ShouldSucceed_ForValidData()
    {
        var lesson = CreateValidLesson();
        Assert.Equal(LessonStatus.Scheduled, lesson.Status);
        Assert.NotNull(lesson.TimeSlot);
        Assert.Equal(45, lesson.TimeSlot.Duration.TotalMinutes);
    }

    [Fact]
    public void Create_ShouldThrow_WhenSubjectIdEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Lesson.Create(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                DateTime.Today.AddDays(1), 1, "Тема"));
    }

    [Fact]
    public void Create_ShouldThrow_WhenTeacherIdEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Lesson.Create(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid(),
                DateTime.Today.AddDays(1), 1, "Тема"));
    }

    [Fact]
    public void Create_ShouldThrow_WhenClassIdEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Lesson.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid(),
                DateTime.Today.AddDays(1), 1, "Тема"));
    }

    [Fact]
    public void Create_ShouldThrow_WhenClassRoomIdEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Lesson.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty,
                DateTime.Today.AddDays(1), 1, "Тема"));
    }

    [Fact]
    public void Create_ShouldThrow_WhenDateInPast()
    {
        Assert.Throws<ArgumentException>(() =>
            Lesson.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                DateTime.Today.AddDays(-1), 1, "Тема"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenTopicEmpty(string? topic)
    {
        Assert.Throws<ArgumentException>(() =>
            Lesson.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                DateTime.Today.AddDays(1), 1, topic!));
    }

    [Theory]
    [InlineData((short)0)]
    [InlineData((short)9)]
    public void Create_ShouldThrow_WhenLessonNumberOutOfRange(short num)
    {
        Assert.Throws<ArgumentException>(() =>
            Lesson.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                DateTime.Today.AddDays(1), num, "Тема"));
    }

    [Fact]
    public void UpdateTopic_ShouldSucceed_WhenScheduled()
    {
        var lesson = CreateValidLesson();
        lesson.UpdateTopic("Новая тема");
        Assert.Equal("Новая тема", lesson.Topic);
    }

    [Fact]
    public void UpdateTopic_ShouldThrow_WhenCompleted()
    {
        var lesson = CreateValidLesson(daysFromToday: 0);
        lesson.Complete();
        Assert.Throws<InvalidOperationException>(() => lesson.UpdateTopic("X"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateTopic_ShouldThrow_WhenEmpty(string newTopic)
    {
        var lesson = CreateValidLesson();
        Assert.Throws<ArgumentException>(() => lesson.UpdateTopic(newTopic));
    }

    [Fact]
    public void AssignReplacementTeacher_ShouldSucceed_WhenScheduled()
    {
        var lesson = CreateValidLesson();
        var replacement = Guid.NewGuid();
        lesson.AssignReplacementTeacher(replacement);
        Assert.Equal(replacement, lesson.ReplacementTeacherId);
        Assert.Equal(LessonStatus.Replaced, lesson.Status);
    }

    [Fact]
    public void AssignReplacementTeacher_ShouldThrow_WhenEmptyId()
    {
        var lesson = CreateValidLesson();
        Assert.Throws<ArgumentException>(() => lesson.AssignReplacementTeacher(Guid.Empty));
    }

    [Fact]
    public void AssignReplacementTeacher_ShouldThrow_WhenCompleted()
    {
        var lesson = CreateValidLesson(daysFromToday: 0);
        lesson.Complete();
        Assert.Throws<InvalidOperationException>(() => lesson.AssignReplacementTeacher(Guid.NewGuid()));
    }

    [Fact]
    public void Cancel_ShouldSucceed_WhenScheduled()
    {
        var lesson = CreateValidLesson();
        lesson.Cancel("болезнь учителя");
        Assert.Equal(LessonStatus.Cancelled, lesson.Status);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenCompleted()
    {
        var lesson = CreateValidLesson(daysFromToday: 0);
        lesson.Complete();
        Assert.Throws<InvalidOperationException>(() => lesson.Cancel("что-то"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Cancel_ShouldThrow_WhenReasonEmpty(string reason)
    {
        var lesson = CreateValidLesson();
        Assert.Throws<ArgumentException>(() => lesson.Cancel(reason));
    }

    [Fact]
    public void Complete_ShouldSucceed_WhenTodayOrPast()
    {
        var lesson = CreateValidLesson(daysFromToday: 0);
        lesson.Complete();
        Assert.Equal(LessonStatus.Completed, lesson.Status);
    }

    [Fact]
    public void Complete_ShouldThrow_WhenCancelled()
    {
        var lesson = CreateValidLesson();
        lesson.Cancel("причина");
        Assert.Throws<InvalidOperationException>(() => lesson.Complete());
    }

    [Fact]
    public void Complete_ShouldThrow_WhenDateInFuture()
    {
        var lesson = CreateValidLesson(daysFromToday: 5);
        Assert.Throws<InvalidOperationException>(() => lesson.Complete());
    }

    [Fact]
    public void AddAssignment_ShouldSucceed_WhenScheduled()
    {
        var lesson = CreateValidLesson();
        var id = Guid.NewGuid();
        lesson.AddAssignment(id);
        Assert.Contains(id, lesson.AssignmentIds);
    }

    [Fact]
    public void AddAssignment_ShouldThrow_WhenCancelled()
    {
        var lesson = CreateValidLesson();
        lesson.Cancel("x");
        Assert.Throws<InvalidOperationException>(() => lesson.AddAssignment(Guid.NewGuid()));
    }

    [Fact]
    public void AddAssignment_ShouldThrow_WhenDuplicate()
    {
        var lesson = CreateValidLesson();
        var id = Guid.NewGuid();
        lesson.AddAssignment(id);
        Assert.Throws<InvalidOperationException>(() => lesson.AddAssignment(id));
    }

    [Fact]
    public void AddGrade_ShouldSucceed_WhenCompleted()
    {
        var lesson = CreateValidLesson(daysFromToday: 0);
        lesson.Complete();
        lesson.AddGrade(Guid.NewGuid());
        Assert.Single(lesson.GradeIds);
    }

    [Fact]
    public void AddGrade_ShouldThrow_WhenNotCompleted()
    {
        var lesson = CreateValidLesson();
        Assert.Throws<InvalidOperationException>(() => lesson.AddGrade(Guid.NewGuid()));
    }

    [Fact]
    public void ChangeClassRoom_ShouldSucceed_WhenMoreThan24HoursAway()
    {
        var lesson = CreateValidLesson(daysFromToday: 5);
        var newRoom = Guid.NewGuid();
        lesson.ChangeClassRoom(newRoom);
        Assert.Equal(newRoom, lesson.ClassRoomId);
    }

    [Fact]
    public void ChangeClassRoom_ShouldThrow_WhenEmptyId()
    {
        var lesson = CreateValidLesson(daysFromToday: 5);
        Assert.Throws<ArgumentException>(() => lesson.ChangeClassRoom(Guid.Empty));
    }

    [Fact]
    public void ChangeClassRoom_ShouldThrow_WhenCompleted()
    {
        var lesson = CreateValidLesson(daysFromToday: 0);
        lesson.Complete();
        Assert.Throws<InvalidOperationException>(() => lesson.ChangeClassRoom(Guid.NewGuid()));
    }

    [Fact]
    public void ChangeClassRoom_ShouldThrow_WhenLessThan24HoursAway()
    {
        var lesson = CreateValidLesson(daysFromToday: 0);
        Assert.Throws<InvalidOperationException>(() => lesson.ChangeClassRoom(Guid.NewGuid()));
    }
}
