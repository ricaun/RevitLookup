#if NET
using System.Reflection;
using System.Runtime.Loader;

namespace RevitLookup.Utils;
public static class AssemblyContext
{
    static AssemblyLoadContext _context;
    internal static AssemblyLoadContext Get()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var context = AssemblyLoadContext.GetLoadContext(assembly);
        if (IsDefault() == false)
            _context = context;

        if (_context is null)
        {
            _context = new LocalAssemblyLoadContext(assembly.Location);
            _context.Unloading += (context) =>
            {
                _context = null;
            };
        }
        return _context;
    }

    public static bool IsDefault()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var context = AssemblyLoadContext.GetLoadContext(assembly);

        if (context == AssemblyLoadContext.Default)
            return true;

        return context.GetType().Name != nameof(LocalAssemblyLoadContext);
    }

    public static dynamic InstanceFrom<T>(string assemblyPath = null, string typeName = null)
    {
        var context = Get();

        assemblyPath = assemblyPath ?? Assembly.GetExecutingAssembly().Location;

        var assembly = context.LoadFromAssemblyPath(assemblyPath);
        if (assembly != null)
        {
            typeName = typeName ?? typeof(T).FullName;
            var intance = assembly.CreateInstance(typeName, true);
            return intance;
        }

        return default(T);
    }

    public static TReturn InstanceFrom<T, TReturn>(string assemblyPath = null, string typeName = null)
    {
        return InstanceFrom<T>(assemblyPath, typeName);
    }

    public static void Unload()
    {
        _context?.Unload();
    }
    internal class LocalAssemblyLoadContext : AssemblyLoadContext
    {
        public bool ConsoleShow { get; set; } = false;
        private void WriteLine(string message)
        {
            if (ConsoleShow)
                Console.WriteLine(message);
        }

        private AssemblyDependencyResolver _resolver;
        private readonly string _assemblyPath;
        private Assembly _assembly;

        public LocalAssemblyLoadContext(string assemblyPath) : base("LocalContext", isCollectible: true)
        {
            this._assemblyPath = assemblyPath;
            this._resolver = new AssemblyDependencyResolver(assemblyPath);

            WriteLine($"{this.Name} Start: {AssemblyName.GetAssemblyName(assemblyPath)}");

            this.Unloading += (context) =>
            {
                WriteLine($"{this.Name} Unload: {context.Name}");
                _assembly = null;
            };
        }

        public Assembly Initialize()
        {
            if (_assembly is null)
            {
                _assembly = LoadFromAssemblyPath(_assemblyPath);
            }
            return _assembly;
        }

        protected override Assembly Load(AssemblyName name)
        {
            WriteLine($"{this.Name} Load: {name}");

            string assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }
}

#else
namespace RevitLookup.Utils;
public static class AssemblyContext
{
    public static bool IsDefault()
    {
        return false;
    }

    public static dynamic InstanceFrom<T>(string assemblyPath = null, string typeName = null)
    {
        return default(T);
    }

    public static TReturn InstanceFrom<T, TReturn>(string assemblyPath = null, string typeName = null)
    {
        return default(TReturn);
    }

    public static void Unload()
    {
    }
}
#endif