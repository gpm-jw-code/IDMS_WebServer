using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.MappingViews;
using IDMSWebServer.Models.DataModels;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;



//EF CORE 模型預熱
//Console.WriteLine("模型預熱");
//using (var context = new IDMSContext())
//{
//    var objectContent = ((IObjectContextAdapter)context).ObjectContext;
//    var mappingCollection = (StorageMappingItemCollection)objectContent.MetadataWorkspace.GetItemCollection(System.Data.Entity.Core.Metadata.Edm.DataSpace.CSpace);
//    mappingCollection.GenerateViews(new List<EdmSchemaError>());
//}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDirectoryBrowser();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(option => option.Level = System.IO.Compression.CompressionLevel.Optimal);
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "text/html;charset=uft-8",
        "application/json;charset=utf-8"
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseWebSockets();
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
app.UseResponseCompression();
Console.WriteLine(Environment.CurrentDirectory);
app.Run();



