using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OutOfTimePrototype.Configurations;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.Middlewares.ExceptionMiddleware;
using OutOfTimePrototype.Services.Authentication;
using OutOfTimePrototype.Services.General.Implementations;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Services.Interfaces;
using OutOfTimePrototype.DTO;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

services.Configure<IdentityOptions>(opts =>
{
    opts.Password.RequireNonAlphanumeric = true;
    opts.Password.RequireDigit = true;
    opts.Password.RequiredLength = 6;
    opts.Password.RequireLowercase = true;
    opts.Password.RequireUppercase = true;
    opts.Password.RequiredUniqueChars = 3;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "Jwt",
        Name = "Jwt Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter your JWT Bearer token in the textbox below",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, new List<string>() }
    });
    options.DocumentFilter<CustomModelDocumentFilter<ClassDto>>();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

services.AddDbContext<OutOfTimeDbContext>(options =>
{
    //options.UseNpgsql(builder.Configuration.GetConnectionString("OutOfTimeDb"));
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
});

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = JwtConfiguration.Issuer,
        ValidateAudience = true,
        ValidAudience = JwtConfiguration.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = JwtConfiguration.GetSymmetricSecurityKey(),
        ValidateLifetime = true
    };
});

services.AddAuthorization(
    options =>
    {
        options.AddPolicy("RequireRoot", policy => policy.RequireRole(new List<string>() { Role.Root.ToString() }));
    }
);

// AutoMapper configuration
var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
var mapper = config.CreateMapper();
services.AddSingleton(mapper);

// Configure DI for Services
services.AddTransient<ITokenService, TokenService>();
services.AddScoped<ILectureHallService, LectureHallService>();
services.AddScoped<ICampusBuildingService, CampusBuildingService>();
services.AddScoped<IEducatorService, EducatorService>();
services.AddScoped<IClassService, ClassService>();
services.AddScoped<IClusterService, ClusterService>();
services.AddScoped<IUserService, UserService>();

services.AddHealthChecks();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var outOfTimeDbContext = scope.ServiceProvider
        .GetRequiredService<OutOfTimeDbContext>();
    if (outOfTimeDbContext.Database.GetPendingMigrations().Any())
    {
        outOfTimeDbContext.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

var logger = app.Services.GetRequiredService<ILogger<ExceptionMiddleware>>();
app.ConfigureExceptionHandler(logger);
app.ConfigureCustomExceptionMiddleware();

app.MapControllers();

app.Run();