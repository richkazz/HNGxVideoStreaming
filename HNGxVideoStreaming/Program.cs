using HNGxVideoStreaming.Data;
using HNGxVideoStreaming.Interface;
using HNGxVideoStreaming.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var conEnviro = Environment.GetEnvironmentVariable("CONNECTION_STRING");
var connectionString = conEnviro ?? builder
                .Configuration
                .GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<HNGxVideoStreamingDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IWhisperService, WhisperService>();
builder.Services.AddScoped<IUploadService, UploadService>();

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
