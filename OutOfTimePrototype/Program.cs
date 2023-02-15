using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.Config;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddScoped<IClassService, ClassService>();

services.AddDbContext<OutOfTimeDbContext>(options => { options.UseNpgsql(builder.Configuration.GetConnectionString("OutOfTimeDb")); });

// AutoMapper configuration
var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
var mapper = config.CreateMapper();
services.AddSingleton(mapper);

// Configure DI for Services
services.AddScoped<ILectureHallService, LectureHallService>();

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
