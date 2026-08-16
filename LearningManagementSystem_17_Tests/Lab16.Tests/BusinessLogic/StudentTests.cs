using Domain.Entities;
using Domain.Value_Objects;
using Domain.Enums;
using Xunit;

namespace Lab16.Tests.BusinessLogic;

public class StudentTests
{
    private static FullName Name() => FullName.Create("Иван", "Иванов", "Иванович");
    private static Domain.Entities.Email Email() => Domain.Entities.Email.Create("ivan@example.com");
    private static PhoneNumber Phone() => PhoneNumber.Create("+7 (123) 456 78-90");

    [Fact]
    public void Create_ShouldSucceed_ForValidData()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        Assert.NotEqual(Guid.Empty, s.Id);
        Assert.Equal(StudentStatus.Active, s.Status);
    }

    [Fact]
    public void Create_ShouldThrow_WhenClassIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Student.Create(Name(), new DateTime(2014, 1, 1), Guid.Empty, Email(), Phone()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Create_ShouldThrow_WhenDateOfBirthInFutureOrToday(int addDays)
    {
        var dob = DateTime.Today.AddDays(addDays);
        Assert.Throws<ArgumentException>(() =>
            Student.Create(Name(), dob, Guid.NewGuid(), Email(), Phone()));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(30)]
    public void Create_ShouldThrow_WhenAgeOutOfRange(int ageYears)
    {
        var dob = DateTime.Today.AddYears(-ageYears);
        Assert.Throws<ArgumentException>(() =>
            Student.Create(Name(), dob, Guid.NewGuid(), Email(), Phone()));
    }

    [Fact]
    public void GetAge_ShouldReturnCorrectAge()
    {
        var dob = DateTime.Today.AddYears(-10).AddDays(-1);
        var s = Student.Create(Name(), dob, Guid.NewGuid(), Email(), Phone());
        Assert.Equal(10, s.GetAge());
    }

    [Fact]
    public void TransferToClass_ShouldSucceed_WhenAgeMatchesGrade()
    {
        var dob = DateTime.Today.AddYears(-13);
        var s = Student.Create(Name(), dob, Guid.NewGuid(), Email(), Phone());
        var newClass = Guid.NewGuid();
        s.TransferToClass(newClass, 7);
        Assert.Equal(newClass, s.ClassId);
    }

    [Fact]
    public void TransferToClass_ShouldThrow_WhenNotActive()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        s.Expel("причина");
        Assert.Throws<InvalidOperationException>(() => s.TransferToClass(Guid.NewGuid(), 2));
    }

    [Fact]
    public void TransferToClass_ShouldThrow_WhenClassIdEmpty()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        Assert.Throws<ArgumentException>(() => s.TransferToClass(Guid.Empty, 2));
    }

    [Fact]
    public void TransferToClass_ShouldThrow_WhenAgeMismatches()
    {
        var dob = DateTime.Today.AddYears(-8);
        var s = Student.Create(Name(), dob, Guid.NewGuid(), Email(), Phone());
        Assert.Throws<InvalidOperationException>(() => s.TransferToClass(Guid.NewGuid(), 11));
    }

    [Fact]
    public void UpdateContactInfo_ShouldReplaceEmailAndPhone()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        var newEmail = Domain.Entities.Email.Create("new@example.com");
        var newPhone = PhoneNumber.Create("+7 999 000 11 22");
        s.UpdateContactInfo(newEmail, newPhone);
        Assert.Equal(newEmail, s.Email);
        Assert.Equal(newPhone, s.ParentPhone);
    }

    [Fact]
    public void SetSpecialNeeds_ShouldUpdateFlag()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        s.SetSpecialNeeds(true);
        Assert.True(s.HasSpecialNeeds);
    }

    [Fact]
    public void Expel_ShouldSucceed_WhenActive()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        s.Expel("за нарушение");
        Assert.Equal(StudentStatus.Expelled, s.Status);
    }

    [Fact]
    public void Expel_ShouldThrow_WhenAlreadyExpelled()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        s.Expel("причина 1");
        Assert.Throws<InvalidOperationException>(() => s.Expel("причина 2"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Expel_ShouldThrow_WhenReasonEmpty(string? reason)
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        Assert.Throws<ArgumentException>(() => s.Expel(reason!));
    }

    [Fact]
    public void TakeLeave_ShouldSucceed_WhenActive()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        s.TakeLeave();
        Assert.Equal(StudentStatus.OnLeave, s.Status);
    }

    [Fact]
    public void TakeLeave_ShouldThrow_WhenNotActive()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        s.Expel("причина");
        Assert.Throws<InvalidOperationException>(() => s.TakeLeave());
    }

    [Fact]
    public void ReturnFromLeave_ShouldSucceed_WhenOnLeave()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        s.TakeLeave();
        s.ReturnFromLeave();
        Assert.Equal(StudentStatus.Active, s.Status);
    }

    [Fact]
    public void ReturnFromLeave_ShouldThrow_WhenNotOnLeave()
    {
        var s = Student.Create(Name(), new DateTime(2014, 1, 1), Guid.NewGuid(), Email(), Phone());
        Assert.Throws<InvalidOperationException>(() => s.ReturnFromLeave());
    }
}
