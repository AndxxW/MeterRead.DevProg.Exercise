using EnSek.MeterRead.Models.Entities;
using EnSek.MeterRead.Utilities.Classes;
using EnSek.MeterRead.Utilities.Helpers;
using FluentAssertions;
using System.Text;
using Xunit;

namespace EnSek.MeterRead.Utilities.UnitTests
{

    public class CsvHelperTests
    {
        [Theory]
        [InlineData(null, new string[0])]
        [InlineData("", new string[0])]
        [InlineData("    ", new string[0])]
        [InlineData(",", new string[0])]
        [InlineData(",,,,", new string[0])]
        [InlineData(",  ,  ,  ,", new string[0])]
        public void ConvertCsvToJsonStrings_EmptyCsvTextShouldReturnNoStrings(string csvText, IEnumerable<string> expected)
        {
            //Act
            var actual = CsvHelper.ConvertCsvToJsonStrings(csvText);

            //Assert
            actual.Should().Equal(expected);
        }

        [Fact]
        public void ConvertCsvToJsonStrings_ValidCsvTextShouldReturnJsonStrings()
        {
            //Arrange
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("AccountId, FirstName, LastName");
            csvBuilder.AppendLine("2344,Tommy,Test1");
            csvBuilder.AppendLine("8766,Sally,Test2");

            //Act
            var actual = CsvHelper.ConvertCsvToJsonStrings(csvBuilder.ToString()).ToArray();

            //Assert
            var expected = new[]
            {
                "{\"AccountId\":\"2344\",\"FirstName\":\"Tommy\",\"LastName\":\"Test1\",}",
                "{\"AccountId\":\"8766\",\"FirstName\":\"Sally\",\"LastName\":\"Test2\",}"
            };

            actual.Should().Equal(expected);
        }

        [Fact]
        public void ConvertCsvToJsonStrings_ShouldHandleEmptyValueColumns()
        {
            //Arrange
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("AccountId, FirstName, LastName");
            csvBuilder.AppendLine("2344");
            csvBuilder.AppendLine("8766,Sally");
            csvBuilder.AppendLine("8767,,Test1");
            csvBuilder.AppendLine(",,Test2");
            
            //Act
            var actual = CsvHelper.ConvertCsvToJsonStrings(csvBuilder.ToString()).ToArray();

            //Assert
            var expected = new []
            {
                "{\"AccountId\":\"2344\",\"FirstName\":\"\",\"LastName\":\"\",}",
                "{\"AccountId\":\"8766\",\"FirstName\":\"Sally\",\"LastName\":\"\",}",
                "{\"AccountId\":\"8767\",\"FirstName\":\"\",\"LastName\":\"Test1\",}",
                "{\"AccountId\":\"\",\"FirstName\":\"\",\"LastName\":\"Test2\",}"
            };

            actual.Should().Equal(expected);
        }

        [Fact]
        public void ConvertCsvToJsonStrings_ShouldWorkWithOtherDelimiters()
        {
            //Arrange
            var settings = new CsvSettings
            {
                NewLineDelimiter = ";",
                ColumnDelimiter = "\t"
            };

            var csvBuilder = new StringBuilder();
            csvBuilder.Append("AccountId \t FirstName \t LastName;");
            csvBuilder.Append("2344 \t Tommy \t Test1;");
            csvBuilder.Append("8766 \t Sally \t Test2");

            //Act
            var actual = CsvHelper.ConvertCsvToJsonStrings(csvBuilder.ToString(), settings).ToArray();

            //Assert
            var expected = new []
            {
                "{\"AccountId\":\"2344\",\"FirstName\":\"Tommy\",\"LastName\":\"Test1\",}",
                "{\"AccountId\":\"8766\",\"FirstName\":\"Sally\",\"LastName\":\"Test2\",}"
            };

            actual.Should().Equal(expected);
        }

        [Fact]
        public void ConvertCsvTo_ShouldPopulateCsvConversionResult_AllSuccessful()
        {
            //Arrange
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("AccountId, FirstName, LastName");
            csvBuilder.AppendLine("2344,Tommy,Test1");
            csvBuilder.AppendLine("8766,Sally,Test2");

            //Act
            var actual = CsvHelper.ConvertCsvTo<Account>(csvBuilder.ToString());

            //Assert
            var expected = new CsvConversionResult<Account>
            {
                CsvRowCount = 2,
                SuccessCount = 2,
                FailCount = 0,
                Instances = new []
                {
                    new Account { AccountId = 2344, FirstName = "Tommy", LastName = "Test1" },
                    new Account { AccountId = 8766, FirstName = "Sally", LastName = "Test2" }
                }
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ConvertCsvTo_ShouldPopulateCsvConversionResultAndHandleFailures()
        {
            //Arrange
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
            csvBuilder.AppendLine("2344,22/04/2019 09:24,12345");   //Valid
            csvBuilder.AppendLine("2349,22/04/2019 12:25,VOID");    //Invalid
            csvBuilder.AppendLine("2344,08/05/2019 09:24,0X765");   //Invalid
            csvBuilder.AppendLine("4534,11/05/2019 09:24,");        //Invalid
            csvBuilder.AppendLine("1241,11/04/2019 12:55,54321,X"); //Valid, even with extra column

            var settings = new CsvSettings
            {
                DateFormat = "dd/MM/yyyy HH:mm"
            };

            //Act
            var actual = CsvHelper.ConvertCsvTo<MeterReading>(csvBuilder.ToString(), settings);

            //Assert
            var expected = new CsvConversionResult<MeterReading>
            {
                CsvRowCount = 5,
                SuccessCount = 2,
                FailCount = 3,
                Instances = new []
                {
                    new MeterReading { AccountId = 2344, MeterReadingDateTime = new DateTime(2019, 04, 22, 9, 24, 0), MeterReadValue = 12345 },
                    new MeterReading { AccountId = 1241, MeterReadingDateTime = new DateTime(2019, 04, 11, 12, 55, 0), MeterReadValue = 54321 }
                }
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ConvertCsvTo_ShouldSupportItemValidation()
        {
            //Arrange
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
            csvBuilder.AppendLine("2349,22/04/2019 12:25,-54321");  //Invalid - MeterReadValue is negative
            csvBuilder.AppendLine("2349,22/04/2019 12:25,999999");  //Invalid - MeterReadValue too long
            csvBuilder.AppendLine("2344,22/04/2019 09:24,12345");   //Valid

            var settings = new CsvSettings
            {
                DateFormat = "dd/MM/yyyy HH:mm",
                ValidateItem = x => (x as MeterReading)?.MeterReadValue is >= 0 and <= 99999
            };

            //Act
            var actual = CsvHelper.ConvertCsvTo<MeterReading>(csvBuilder.ToString(), settings);

            //Assert
            var expected = new CsvConversionResult<MeterReading>
            {
                CsvRowCount = 3,
                SuccessCount = 1,
                FailCount = 2,
                Instances = new []
                {
                    new MeterReading { AccountId = 2344, MeterReadingDateTime = new DateTime(2019, 04, 22, 09, 24, 0), MeterReadValue = 12345 },
                }
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
