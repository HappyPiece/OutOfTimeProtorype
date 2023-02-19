using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.Config;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Services;
using OutOfTimePrototype.Utilities;

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
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

services.AddDbContext<OutOfTimeDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("OutOfTimeDb"));
});

// AutoMapper configuration
var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
var mapper = config.CreateMapper();
services.AddSingleton(mapper);

// Configure DI for Services
// services.AddScoped<IClassService, ClassService>();
services.AddScoped<ILectureHallService, LectureHallService>();
services.AddScoped<IEducatorService, EducatorService>();
services.AddScoped<IClassService, ClassService>();
services.AddScoped<IClusterService, ClusterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();