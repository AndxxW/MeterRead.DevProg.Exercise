using EnSek.MeterRead.Models.Entities;
using EnSek.MeterRead.Utilities.Helpers;

namespace EnSek.MeterRead.DAL.SqlServer
{
    /// <summary>
    /// Helper to allow some initial data to be seeded to the database for development purposes
    /// </summary>
    public static class DataSeeder
    {
        //TODO - Avoid hard-coding paths
        public const string SeedFolderPath = "Data/Seed/";
        public const string TestAccountsCsvPath = SeedFolderPath + "Test_Accounts.csv";

        public static void Seed(DataContext context)
        {
            if (!context.Accounts!.Any())
            {
                var csvText = File.ReadAllText(TestAccountsCsvPath);

                var csvResult = CsvHelper.ConvertCsvTo<Account>(csvText);

                context.Accounts?.AddRange(csvResult.Instances);

                context.SaveChanges();
            }
        }

    }
}
