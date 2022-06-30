using EnSek.MeterRead.Models.Entities;
using EnSek.MeterRead.Utilities.Helpers;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace EnSek.MeterRead.Utilities.UnitTests
{
    public class JsonHelperTests
    {
        [Fact]
        public void FromJson_ShouldPopulateClassInstance()
        {
            //Arrange
            var json = "{\"AccountId\":\"2344\",\"FirstName\":\"Tommy\",\"LastName\":\"Test\"}";

            //Act
            var actual = JsonHelper.FromJson<Account>(json);

            //Assert
            var expected = new Account
            {
                AccountId = 2344,
                FirstName = "Tommy",
                LastName = "Test"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void FromJson_ShouldPopulateClassInstanceWithDateTime()
        {
            //Arrange
            var json = "{\"AccountId\":\"2351\",\"MeterReadingDateTime\":\"22/04/2019 12:25\",\"MeterReadValue\":\"57579\"}";

            var jsonSettings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy HH:mm" //UK Date 24 Hour Time format
            };

            //Act
            var actual = JsonHelper.FromJson<MeterReading>(json, jsonSettings);

            //Assert
            var expected = new MeterReading
            {
                AccountId = 2351,
                MeterReadingDateTime = new DateTime(2019, 4, 22, 12, 25, 0),
                MeterReadValue = 57579
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void FromJson_ShouldReturnNullForInvalidJson()
        {
            //Arrange
            var json = "{\"AccountId\":\"2351\",\"MeterReadingDateTime\":\"Not a date\",\"MeterReadValue\":\"57579\"}";

            //Act
            var actual = JsonHelper.FromJson<MeterReading>(json);

            //Assert
            actual.Should().BeNull();
        }

        [Fact]
        public void ConvertJsonStringsTo_ShouldPopulateMultipleClassInstances()
        {
            //Arrange
            var jsonStrings = new string[]
            {
                "{\"AccountId\":\"2344\",\"FirstName\":\"Tommy\",\"LastName\":\"Test1\",}",
                "{\"AccountId\":\"8766\",\"FirstName\":\"Sally\",\"LastName\":\"Test2\",}"
            };

            //Act
            var actual = JsonHelper.ConvertJsonStringsTo<Account>(jsonStrings).ToArray();

            //Assert
            var expected = new Account[]
            {
                new Account { AccountId = 2344, FirstName = "Tommy", LastName = "Test1" },
                new Account { AccountId = 8766, FirstName = "Sally", LastName = "Test2" }
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
;