using IDMSWebServer.Models.DataModels;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDirectoryBrowser();
builder.Services.AddResponseCompression();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseWebSockets();
app.UseResponseCompression();
app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
//app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseVueRouterHistory();
//IConfiguration configuration = app.Configuration;
//var _ic = configuration.GetSection("IDMS");
//var SystemLogDirectory = _ic.GetValue("SystemLogDirectory", "");
//var _fileProvider = new PhysicalFileProvider($@"{SystemLogDirectory}");
//var _requestPath = "/Data";

//app.UseFileServer(new FileServerOptions()
//{
//    FileProvider = _fileProvider,
//    RequestPath = _requestPath,
//    EnableDirectoryBrowsing = true,
//});


//app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
