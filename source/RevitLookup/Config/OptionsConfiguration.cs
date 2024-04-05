using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using ricaun.DI;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
using RevitLookup.Models.Options;
using RevitLookup.Utils;

namespace RevitLookup.Config;

public static class ContainerOptionsConfigureExtension
{
    public static void Configure<T>(this IContainer container, Action<T> config) where T : class
    {
        var option = new Options<T>(container, config);
        container.AddSingleton<IOptions<T>>(option);
    }
}

public class Options<T> : IOptions<T> where T : class
{
    private readonly IContainer _container;
    private readonly Action<T> _action;

    public Options(IContainer container, Action<T> config)
    {
        _container = container;
        _action = config;
    }
    public T Value => GetValue();

    private T GetValue()
    {
        var value = _container.Resolve<T>();
        _action?.Invoke(value);
        return value;
    }
}

public static class OptionsConfiguration
{
    public static void AddOptions(this IContainer services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyLocation = assembly.Location;
        //var rootPath = configuration.GetValue<string>("contentRoot");
        var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()!.Location);
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var fileVersion = new Version(FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion!);

        var targetFrameworkAttribute = assembly.GetCustomAttributes(typeof(TargetFrameworkAttribute), true)
            .Cast<TargetFrameworkAttribute>()
            .First();

        services.Configure<AssemblyInfo>(options =>
        {
            options.Framework = targetFrameworkAttribute.FrameworkDisplayName;
            options.AddinVersion = new Version(fileVersion.Major, fileVersion.Minor, fileVersion.Build);
            options.IsAdminInstallation = assemblyLocation.StartsWith(appDataPath) || !AccessUtils.CheckWriteAccess(assemblyLocation);
        });

        services.Configure<FolderLocations>(options =>
        {
            options.RootFolder = rootPath;
            options.ConfigFolder = Path.Combine(rootPath, "Config");
            options.DownloadFolder = Path.Combine(rootPath, "Downloads");
            options.SettingsPath = Path.Combine(rootPath, "Config", "Settings.cfg");
        });

        services.Configure<JsonSerializerOptions>(options =>
        {
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.Converters.Add(new JsonStringEnumConverter());
        });
    }
}