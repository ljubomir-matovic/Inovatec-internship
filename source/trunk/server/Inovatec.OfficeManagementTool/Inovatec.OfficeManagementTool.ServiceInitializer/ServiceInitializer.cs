using Inovatec.OfficeManagementTool.Common;
using Inovatec.OfficeManagementTool.Common.Enricher;
using Inovatec.OfficeManagementTool.Common.Services.EmailService;
using Inovatec.OfficeManagementTool.Common.Services.UserService;
using Inovatec.OfficeManagementTool.ImplementationsBL;
using Inovatec.OfficeManagementTool.ImplementationsDAL;
using Inovatec.OfficeManagementTool.ImplementationsDAL.Context;
using Inovatec.OfficeManagementTool.ImplementationsUI;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesDAL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using Serilog.Sinks.MSSqlServer;
using Inovatec.OfficeManagementTool.Common.Services.BaseHubService;

namespace Inovatec.OfficeManagementTool.ServiceInitializer
{
    public static class ServiceInitializer
    {
        public static IServiceCollection InitializeServices(this IServiceCollection services)
        {
            InitializeSwagger(services);
            InitializeDAL(services);
            InitializeBL(services);
            InitializeUI(services);
            InitializeCommonServices(services);

            InitializeSerilog(services);

            services.AddAutoMapper(typeof(ServiceInitializer).Assembly);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = ConfigProvider.Issuer,
                    ValidateAudience = true,
                    ValidAudience = ConfigProvider.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigProvider.Key)),
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };

                options.Events = new JwtBearerEvents {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/signalR/notification")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddHttpContextAccessor();

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize  = int.MaxValue;
            });

            return services;
        }

        public static IServiceCollection InitializeServicesForScheduler(this IServiceCollection services)
        {
            InitializeDAL(services);
            InitializeBL(services);
            InitializeCommonServices(services);
            
            services.AddAutoMapper(typeof(ServiceInitializer).Assembly);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = ConfigProvider.Issuer,
                    ValidateAudience = true,
                    ValidAudience = ConfigProvider.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigProvider.Key)),
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
            });
            
            return services;
        }


        private static void InitializeUI(IServiceCollection services) 
        {
            services.AddScoped<IUserUI, UserUI>();
            services.AddScoped<ICategoryUI, CategoryUI>();
            services.AddScoped<IItemUI, ItemUI>();
            services.AddScoped<IOrderUI, OrderUI>();
            services.AddScoped<IOrderItemUI, OrderItemUI>();
            services.AddScoped<IOrderRequestUI, OrderRequestUI>();
            services.AddScoped<IOrderAttachmentUI, OrderAttachmentUI>();
            services.AddScoped<ICommentUI, CommentUI>();
            services.AddScoped<IEquipmentUI, EquipmentUI>();
            services.AddScoped<ILogUI, LogUI>();
            services.AddScoped<IReportUI, ReportUI>();
            services.AddScoped<INotificationUI, NotificationUI>();
            services.AddScoped<IOfficeUI, OfficeUI>();
            services.AddScoped<IReportScheduleUI, ReportScheduleUI>();
            services.AddScoped<ISupplierUI, SupplierUI>();
        }

        private static void InitializeBL(IServiceCollection services)
        { 
            services.AddScoped<IUserBL, UserBL>();
            services.AddScoped<ICategoryBL, CategoryBL>();
            services.AddScoped<IItemBL, ItemBL>();
            services.AddScoped<IOrderBL, OrderBL>();
            services.AddScoped<IOrderItemBL, OrderItemBL>();
            services.AddScoped<IOrderRequestBL, OrderRequestBL>();
            services.AddScoped<IOrderAttachmentBL, OrderAttachmentBL>();
            services.AddScoped<ICommentBL, CommentBL>();
            services.AddScoped<IEquipmentBL, EquipmentBL>();
            services.AddScoped<IReportBL, ReportBL>();
            services.AddScoped<ILogBL, LogBL>();
            services.AddScoped<INotificationBL, NotificationBL>();
            services.AddScoped<IOfficeBL, OfficeBL>();
            services.AddScoped<IReportScheduleBL, ReportScheduleBL>();
            services.AddScoped<ISupplierBL, SupplierBL>();
        }

        private static void InitializeDAL(IServiceCollection services)
        {
            services.AddDbContext<OfficeManagementTool_IS2023Context>(
                options => options.UseSqlServer(ConfigProvider.ConnectionString));
            
            services.AddSingleton<IUnitOfWorkProvider, UnitOfWorkProvider>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }

        private static void InitializeCommonServices(IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<UserEnricher>();
            services.AddScoped(typeof(IBaseHubService<>), typeof(BaseHubService<>));
        }

        private static void InitializeSwagger(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });

                options.OperationFilter<SwaggerOperationFilter>();
            });
        }

        private static void InitializeSerilog(IServiceCollection services)
        {
            // Initialize logger
            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn> 
                {
                    new SqlColumn 
                    {
                        ColumnName = "User",
                        PropertyName = "User",
                        DataType = SqlDbType.VarChar,
                        DataLength = 100
                    },
                }
            };

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.MSSqlServer
                (
                    connectionString: ConfigProvider.ConnectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "Logs",
                        AutoCreateSqlTable = true
                    },
                    columnOptions: columnOptions
                )
                .Enrich.FromLogContext()
                .Enrich.With<UserEnricher>()
                .CreateLogger();
        }
    }
}