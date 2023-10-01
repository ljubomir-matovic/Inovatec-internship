using Microsoft.Extensions.Configuration;

namespace Inovatec.OfficeManagementTool.Common
{
    public static class ConfigProvider
    {
        /// <summary>
        /// Connection string for MS SQL database
        /// </summary>

        public static string MyCORSPolicy { get; private set; } = "_myCORSPolicy";
        public static string ConnectionString { get; private set; } = string.Empty;
        public static string? Key { get; private set; }
        public static string? Issuer { get; private set; }
        public static string? Audience { get; private set; }
        public static string? Subject { get; private set; }
        public static int? LifetimeHours { get; private set; }
        public static string Email { get; private set; } = string.Empty;
        public static string EmailPassword { get; private set; } = string.Empty;
        public static string SmtpUrl { get; private set; } = string.Empty;
        public static int SmtpPort { get; private set; }  

        public static int ResetTokenExpirationTimeInMinutes { get; private set; }

        public static string HostUrl { get; private set; } = string.Empty;
        public static string ClientUrl { get; private set; } = string.Empty;
        public static string ServerUrl { get; private set; } = string.Empty;
        public static string ScheduledTaskFolder { get; private set; } = string.Empty;
        public static string ScheduledTaskName { get; private set; } = string.Empty;
        public static string TaskExistsScript { get; private set; } = string.Empty;
        public static string PathToStaticFiles { get; private set; } = string.Empty;
        public static List<string> AllowedContentTypes { get; private set; }

        public static void Setup(this IConfigurationRoot config)
        {
            ConnectionString = config.GetConnectionString("DefaultConnection") ?? "";

            Key = config["Jwt:Key"];
            Issuer = config["Jwt:Issuer"];
            Audience = config["Jwt:Audience"];
            Subject = config["Jwt:Subject"];
            LifetimeHours = int.Parse(config["Jwt:LifetimeHours"] ?? "0");
            Email = config.GetSection("EmailSettings:Email").Value ?? "";
            EmailPassword = config.GetSection("EmailSettings:Password").Value ?? "";
            SmtpUrl = config.GetSection("EmailSettings:SmtpUrl").Value ?? "";
            SmtpPort = int.Parse(config.GetSection("EmailSettings:SmtpPort").Value ?? "587");
            HostUrl = config.GetSection("HostUrl").Value ?? "";
            ClientUrl = config.GetSection("ClientUrl").Value ?? "";
            ServerUrl = config.GetSection("ServerUrl").Value ?? "";
            ResetTokenExpirationTimeInMinutes = int.Parse(config.GetSection("ResetTokenExpirationTimeInMinutes").Value ?? "0");
            ScheduledTaskFolder = config.GetSection("ScheduledTaskFolder").Value ?? "";
            ScheduledTaskName = config.GetSection("ScheduledTaskName").Value ?? "";
            TaskExistsScript = config.GetSection("TaskExistsScript").Value ?? "";
            PathToStaticFiles = config.GetSection("PathToStaticFiles").Value ?? "";
            AllowedContentTypes = (config.GetSection("AllowedContentTypes").Value ?? "").Split(',').ToList();
        }
    }
}