//using Microsoft.Extensions.Logging;

using RevitLookup.Core;
using RevitLookup.Core.ComponentModel.Descriptors;
using RevitLookup.Services;
using ricaun.DI;

namespace RevitLookup.Config;

public static class LoggerConfigurator
{
    public static void AddLogger(this IContainer container)
    {
        container.AddScoped<ILogger<ParameterDescriptor>, Logger<ParameterDescriptor>>();
        container.AddScoped<ILogger<SettingsService>, Logger<SettingsService>>();
        container.AddScoped<ILogger<SoftwareUpdateService>, Logger<SoftwareUpdateService>>();
        container.AddScoped<ILogger<EventMonitor>, Logger<EventMonitor>>();
    }

    //public static void AddSerilogConfiguration(this ILoggingBuilder logging)
    //{
    //    logging.AddSimpleConsole(options =>
    //    {
    //        options.SingleLine = true;
    //        options.IncludeScopes = true;
    //        options.ColorBehavior = LoggerColorBehavior.Enabled;
    //    });

    //    AppDomain.CurrentDomain.UnhandledException += OnOnUnhandledException;
    //}

    //private static void OnOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    //{
    //    var exception = (Exception) args.ExceptionObject;
    //    var logger = Host.GetService<ILogger<AppDomain>>();
    //    logger.LogCritical(exception, "Domain unhandled exception");
    //}
}