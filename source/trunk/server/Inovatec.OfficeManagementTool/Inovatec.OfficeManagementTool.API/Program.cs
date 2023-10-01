using Inovatec.OfficeManagementTool.Common;
using Inovatec.OfficeManagementTool.ServiceInitializer;
using Serilog;
using Inovatec.OfficeManagementTool.ServiceInitializer.CustomExceptionMiddleware;
using Inovatec.OfficeManagementTool.API;

var builder = WebApplication.CreateBuilder(args);

// Connect ConfigProvider class with appsetting.json file
builder.Configuration.Setup();

builder.Host.UseSerilog();

AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();

// Add services to the container.

builder.Services.AddControllers();

// Initialize services
builder.Services.InitializeServices();

builder.Services.AddSignalR();

// Create CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(ConfigProvider.MyCORSPolicy,
        policy =>
        {
            policy.SetIsOriginAllowed(origin => new Uri(origin).Host == ConfigProvider.HostUrl)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(ConfigProvider.MyCORSPolicy);

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.HubSetup();

app.Run();
