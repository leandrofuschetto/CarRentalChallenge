using AutoMapper;
using CarRental.Data;
using CarRental.Data.DAOs.Clients;
using CarRental.Data.DAOs.Rentals;
using CarRental.Data.DAOs.Vehicles;
using CarRental.Mapper;
using CarRental.Service.Clients;
using CarRental.Service.Rentals;
using CarRental.Service.Vehicles;
using CarRental.WebAPI.Middlewares;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestMethod | 
        HttpLoggingFields.RequestPath |
        HttpLoggingFields.RequestBody |
        HttpLoggingFields.ResponseStatusCode;
});

var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MapperProfile());
});

var mapper = config.CreateMapper();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddScoped<IClientsService, ClientsService>();
builder.Services.AddScoped<IClientsDao, ClientsDao>();
builder.Services.AddScoped<IVehiclesService, VehiclesService>();
builder.Services.AddScoped<IVehiclesDao, VehiclesDao>();
builder.Services.AddScoped<IRentalsService, RentalsService>();
builder.Services.AddScoped<IRentalsDao, RentalsDao>();

builder.Services.AddSingleton(mapper);

builder.Services.AddDbContext<CarRentalContext>(options =>
{
    options
    .UseSqlServer(builder.Configuration.GetConnectionString("DbConn"));
});

var app = builder.Build();

app.UseHttpLogging();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CarRentalContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//middleware for handling exceptions
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
