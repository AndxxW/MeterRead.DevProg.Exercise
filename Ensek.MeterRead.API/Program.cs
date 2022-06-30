using EnSek.MeterRead.DAL.Interfaces;
using EnSek.MeterRead.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using EnSek.MeterRead.DAL.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add scoped data contexts and repositories
builder.Services.AddScoped<IAccountContext, DataContext>();
builder.Services.AddScoped<IMeterReadingContext, DataContext>();
builder.Services.AddScoped<IMeterRepo, MeterRepo>();

var app = builder.Build();

var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();

    if (isDevelopment)
    {
        DataSeeder.Seed(dataContext);
    }
}

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
