using EnSek.MeterRead.DAL.Interfaces;
using EnSek.MeterRead.DAL.Repositories;
using EnSek.MeterRead.Models.Classes;
using EnSek.MeterRead.Models.Entities;
using FluentAssertions;
using Xunit;
using FakeItEasy;

namespace EnSek.MeterRead.DAL.UnitTests
{
    public class MeterRepoTests
    {
        [Theory]
        [InlineData(true, 1, 0)]
        [InlineData(false, 0, 1)]
        public async void CommitMeterReadings_ShouldCheckForExistingAccount(bool accountExists, int expectedSuccesses, int expectedFails)
        {
            //Arrange
            var newReadings = new[]
            {
                new MeterReading { AccountId = 1234, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 12345 }
            };

            var committedReadings = new[]
            {
                new MeterReading { AccountId = 1234, MeterReadingDateTime = DateTime.UtcNow.AddDays(-1), MeterReadValue = 12300 }
            };

            var fakeAccountContext = A.Fake<IAccountContext>();
            A.CallTo(() => fakeAccountContext.DoesAccountExist(1234)).Returns(accountExists);

            var fakeMeterReadingContext = A.Fake<IMeterReadingContext>();
            A.CallTo(() => fakeMeterReadingContext.GetReadingsByAccountId(1234)).Returns(committedReadings);
            A.CallTo(() => fakeMeterReadingContext.CommitChanges()).Returns(true);

            var itemUnderTest = new MeterRepo(fakeAccountContext, fakeMeterReadingContext);

            //Act
            var actual = await itemUnderTest.CommitReadings(newReadings);

            //Assert
            var expected = new DbCommitCounts
            {
                SuccessCount = expectedSuccesses,
                FailCount = expectedFails
            };

            actual.Should().BeEquivalentTo(expected);

            if (accountExists)
            {
                A.CallTo(() => fakeMeterReadingContext.AddMeterReading(A<MeterReading>.Ignored)).MustHaveHappened();
                A.CallTo(() => fakeMeterReadingContext.CommitChanges()).MustHaveHappened();
            }
        }

        [Fact]
        public async void CommitMeterReadings_ShouldNotCommitSameReadingAgain()
        {
            //Arrange

            var newReadings = new[]
            {
                new MeterReading { AccountId = 1234, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 12345 }
            };

            var committedReadings = new[]
            {
                new MeterReading { AccountId = 1234, MeterReadingDateTime = DateTime.UtcNow.AddDays(-1), MeterReadValue = 12345 }
            };

            var fakeAccountContext = A.Fake<IAccountContext>();
            A.CallTo(() => fakeAccountContext.DoesAccountExist(1234)).Returns(true);

            var fakeMeterReadingContext = A.Fake<IMeterReadingContext>();
            A.CallTo(() => fakeMeterReadingContext.GetReadingsByAccountId(1234)).Returns(committedReadings);

            var itemUnderTest = new MeterRepo(fakeAccountContext, fakeMeterReadingContext);

            //Act
            var actual = await itemUnderTest.CommitReadings(newReadings);

            //Assert
            var expected = new DbCommitCounts
            {
                SuccessCount = 0,
                FailCount = 1
            };

            actual.Should().BeEquivalentTo(expected);

            A.CallTo(() => fakeMeterReadingContext.AddMeterReading(A<MeterReading>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMeterReadingContext.CommitChanges()).MustNotHaveHappened();
        }

        [Fact]
        public async void CommitMeterReadings_ShouldNotCommitOlderReading()
        {
            //Arrange

            var newReadings = new[]
            {
                new MeterReading { AccountId = 1234, MeterReadingDateTime = DateTime.UtcNow.AddDays(-2), MeterReadValue = 12300 }
            };

            var committedReadings = new[]
            {
                new MeterReading { AccountId = 1234, MeterReadingDateTime = DateTime.UtcNow.AddDays(-1), MeterReadValue = 12345 }
            };

            var fakeAccountContext = A.Fake<IAccountContext>();
            A.CallTo(() => fakeAccountContext.DoesAccountExist(1234)).Returns(true);

            var fakeMeterReadingContext = A.Fake<IMeterReadingContext>();
            A.CallTo(() => fakeMeterReadingContext.GetReadingsByAccountId(1234)).Returns(committedReadings);

            var itemUnderTest = new MeterRepo(fakeAccountContext, fakeMeterReadingContext);

            //Act
            var actual = await itemUnderTest.CommitReadings(newReadings);

            //Assert
            var expected = new DbCommitCounts
            {
                SuccessCount = 0,
                FailCount = 1
            };

            actual.Should().BeEquivalentTo(expected);

            A.CallTo(() => fakeMeterReadingContext.AddMeterReading(A<MeterReading>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMeterReadingContext.CommitChanges()).MustNotHaveHappened();
        }
    }
}
