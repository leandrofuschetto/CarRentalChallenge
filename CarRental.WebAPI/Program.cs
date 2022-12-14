using AutoMapper;
using CarRental.Data;
using CarRental.Data.DAOs.Clients;
using CarRental.Data.DAOs.Rentals;
using CarRental.Data.DAOs.User;
using CarRental.Data.DAOs.Vehicles;
using CarRental.Mapper;
using CarRental.Service.Clients;
using CarRental.Service.Rentals;
using CarRental.Service.Users;
using CarRental.Service.Vehicles;
using CarRental.WebAPI.Helpers;
using CarRental.WebAPI.Middlewares;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields =
        HttpLoggingFields.RequestPath |
        HttpLoggingFields.RequestProperties |
        HttpLoggingFields.RequestBody |
        HttpLoggingFields.RequestMethod |
        HttpLoggingFields.ResponseBody |
        HttpLoggingFields.ResponseStatusCode;
        
    logging.RequestHeaders.Add("My-Request-Header");
    logging.ResponseHeaders.Add("My-Response-Header");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
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
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<JwtHelper, JwtHelper>();

builder.Services.AddSingleton(mapper);

builder.Services.AddDbContext<CarRentalContext>(options =>
{
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DbConn"))
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CarRentalContext>();
    context.Database.Migrate();
}

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ValidateTokenMiddleware>();
app.UseMiddleware<LoggerHttpRequest>();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
