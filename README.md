# Remote Technical Exercise

* Authored using Visual Studio 2022.

* C# .Net Core API which connects to an instance of a database to persist the contents of the Meter Reading CSV file.

* Sql Server 2019 has been used for data persistence adopting a code first strategy for database generation.

* The database is created and test account data seeded the first time that the API is spun up in development.

* A Postman collection has been included in the solution root folder to test posting meter readings csv files to the /meter-reading-uploads endpoint (EnSek - MeterRead.postman_collection.json)
