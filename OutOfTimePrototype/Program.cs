using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.Config;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IClassService, ClassService>();

builder.Services.AddDbContext<OutOfTimeDbContext>(options => { options.UseNpgsql(builder.Configuration.GetConnectionString("OutOfTimeDb")); });

// AutoMapper configuration
var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);



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
