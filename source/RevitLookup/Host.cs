using System.IO;
using System.Reflection;
using ricaun.DI;
using RevitLookup.Config;
using RevitLookup.Services;
using RevitLookup.Services.Contracts;
using RevitLookup.ViewModels.Contracts;
using RevitLookup.ViewModels.Pages;
using RevitLookup.Views;
using RevitLookup.Views.Pages;
using Wpf.Ui;

namespace RevitLookup;

public interface IHost : IContainer
{
    
}

public interface IOptions<T>
{
    T Value { get; }
}

public interface ILogger
{
    void LogInformation(object value);
    void LogError(object value);
    void LogError(Exception ex, object value);
    void LogWarning(object value);
    void LogDebug(object value);
}

public interface ILogger<T>
{
    void LogInformation(object value);
    void LogError(object value);
    void LogError(Exception ex, object value);
    void LogWarning(object value);
    void LogDebug(object value);
}

public class Logger<T> : ILogger<T>
{
    public void LogDebug(object value)
    {
    }

    public void LogError(object value)
    {
    }

    public void LogError(Exception ex, object value)
    {
    }

    public void LogInformation(object value)
    {
    }

    public void LogWarning(object value)
    {
    }
}

public class ServiceProvider : IServiceProvider
{
    private readonly IContainerResolver _container;

    public ServiceProvider(IContainerResolver container)
    {
        _container = container;
    }
    public object GetService(Type serviceType)
    {
        return _container.Resolve(serviceType);
    }
}

public static class Host
{
    private static IContainer _host;

    public static void Start()
    {
        var container = ContainerUtils.CreateContainer();
        container.EnableConsolePrinting(true);

        container.AddTransient<IServiceProvider, ServiceProvider>();

        //Configuration
        container.AddOptions();

        // Logger
        container.AddLogger();

        //App services
        container.AddSingleton<ISettingsService, SettingsService>();
        container.AddSingleton<ISoftwareUpdateService, SoftwareUpdateService>();

        //UI services
        container.AddScoped<INavigationService, NavigationService>();
        container.AddScoped<ISnackbarService, SnackbarService>();
        container.AddScoped<IContentDialogService, ContentDialogService>();
        container.AddScoped<NotificationService>();

        //Views
        container.AddScoped<ISnoopVisualService, SnoopVisualService>();
        container.AddScoped<AboutView>();
        container.AddScoped<AboutViewModel>();
        container.AddScoped<DashboardView>();
        container.AddScoped<IDashboardViewModel, DashboardViewModel>();
        container.AddScoped<SettingsView>();
        container.AddScoped<SettingsViewModel>();
        container.AddScoped<EventsView>();
        container.AddScoped<IEventsViewModel, EventsViewModel>();
        container.AddScoped<SnoopView>();
        container.AddScoped<ISnoopViewModel, SnoopViewModel>();
        container.AddScoped<IWindow, RevitLookupView>();
        
        //Startup view
        container.AddTransient<ILookupService, LookupService>();

        _host = container;

        //var builder = new HostApplicationBuilder(new HostApplicationBuilderSettings
        //{
        //    ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()!.Location),
        //    DisableDefaults = true
        //});

        ////Logging
        //builder.Logging.ClearProviders();
        //builder.Logging.AddSerilogConfiguration();

        ////Configuration
        //builder.Services.AddOptions(builder.Configuration);

        ////App services
        //builder.Services.AddSingleton<ISettingsService, SettingsService>();
        //builder.Services.AddSingleton<ISoftwareUpdateService, SoftwareUpdateService>();

        ////UI services
        //builder.Services.AddScoped<INavigationService, NavigationService>();
        //builder.Services.AddScoped<ISnackbarService, SnackbarService>();
        //builder.Services.AddScoped<IContentDialogService, ContentDialogService>();
        //builder.Services.AddScoped<NotificationService>();

        ////Views
        //builder.Services.AddScoped<ISnoopVisualService, SnoopVisualService>();
        //builder.Services.AddScoped<AboutView>();
        //builder.Services.AddScoped<AboutViewModel>();
        //builder.Services.AddScoped<DashboardView>();
        //builder.Services.AddScoped<IDashboardViewModel, DashboardViewModel>();
        //builder.Services.AddScoped<SettingsView>();
        //builder.Services.AddScoped<SettingsViewModel>();
        //builder.Services.AddScoped<EventsView>();
        //builder.Services.AddScoped<IEventsViewModel, EventsViewModel>();
        //builder.Services.AddScoped<SnoopView>();
        //builder.Services.AddScoped<ISnoopViewModel, SnoopViewModel>();
        //builder.Services.AddScoped<IWindow, RevitLookupView>();

        ////Startup view
        //builder.Services.AddTransient<ILookupService, LookupService>();

        //_host = builder.Build();
        //_host.Start();
    }

    public static void Start(IContainer host)
    {
        _host = host;
        //host.Start();
    }

    public static void Stop()
    {
        _host.Dispose();
    }

    public static T GetService<T>() where T : class
    {
        return _host.Resolve<T>();
    }

    public static T GetService<T>(this IServiceProvider serviceProvider) where T : class
    {
        return serviceProvider.GetService(typeof(T)) as T;
    }
}