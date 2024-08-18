using Api;
using Api.Exceptions;
using Api.Mapping;
using Api.Services;
using Api.Services.Interfaces;
using Data;
using Data.Repositories;
using Data.Repositories.Interfaces;
using Data.Services;
using Data.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString(Constants.Config.ENERGYCONNECTION))
    .EnableSensitiveDataLogging());
builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IValidationService, ValidationService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated(); // using EnsureCreated(), would use Migrate() usually
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
