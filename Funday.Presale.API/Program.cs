using Funday.Presale.API.Configure;
using Funday.Presale.API.Filters;
using Funday.Presale.API.Infrastructure.NLogService;
using Funday.Presale.API.Infrastructure.TextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

// Add services to the container.

builder.WebHost.UseNLog();
logger.Info("Init Main");

#region �K�[���`�B�z�L�o��
builder.Services.AddControllers(options => options.Filters.Add(typeof(ApiExceptionFilter)));
#endregion


builder.Services.AddControllers().AddJsonOptions(options =>
{
    //�]�m �p�G�O Dictionary ���� �b json �ǦC�� �O key ���r�� �ĥ� �p�m�p �R�W
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new DateTimeNullJsonConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "FUNDAY B2B API",
        Description = "An ASP.NET Core Web API for FUNDAY ���~�Ȥ�",
        //TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "��T��"
        },
        License = new OpenApiLicense
        {
            Name = "���}���ƥ��x�z�Lswagger�M�󴣨ѡC"
        }
    });
    // Set the comments path for the Swagger JSON and UI.    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

//�A�ȫغc
AppConfigureServices.Build(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseSwaggerUI(c => {
    //    c.InjectStylesheet("/assests/css/theme-flattop.css");
    //});
}

app.UseCors("WebHostCors");
app.UseAuthorization();

app.MapControllers();

app.Run();
