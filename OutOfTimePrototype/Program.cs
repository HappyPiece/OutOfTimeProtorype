using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OutOfTimePrototype.Config;
using OutOfTimePrototype.Configurations;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Services.Authentication;
using OutOfTimePrototype.Services.General.Implementations;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Services.Implementations;
using OutOfTimePrototype.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

services.AddDbContext<OutOfTimeDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("OutOfTimeDb"));
});

builder.Services.AddAuthentication(options =>
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

builder.Services.AddAuthorization(
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
services.AddScoped<IEducatorService, EducatorService>();
services.AddScoped<IClassService, ClassService>();
services.AddScoped<IClusterService, ClusterService>();
services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();