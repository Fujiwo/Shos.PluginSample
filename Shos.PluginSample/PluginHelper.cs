using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

namespace Shos.PluginSample
{
    public static class PluginHelper
    {
        const string namePropertyName = "Name";
        const string runMethodName    = "Run";

        public static IEnumerable<(object instance, string name, MethodInfo runMethod)> GetPlugins()
            => GetPluginAssemblies().Select(GetPluginsFrom)
                                    .SelectMany(plugins => plugins);

        public static IEnumerable<(object instance, string name, MethodInfo runMethod)> CreatePlugins(string code, CSharpParseOptions options, MetadataReference[] references)
        {
            var dllPath = GetNewDllPath();
            if (dllPath is null)
                return new (object instance, string name, MethodInfo runMethod)[0];

            const string codeFileName = "temorary.cs";

            using var stream = File.Create(dllPath.Value.dllPath);
            return CreatePlugins(code, dllPath.Value.dllName, codeFileName, options, references, stream);
        }

        static IEnumerable<Assembly> GetPluginAssemblies()
            => Directory.GetFiles(GetPluginFolderName())
                        .Select(Assembly.LoadFrom);

        static IEnumerable<(object instance, string name, MethodInfo runMethod)> CreatePlugins(string code, string dllPath, string codeFileName, CSharpParseOptions options, MetadataReference[] references, FileStream stream)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code, options, codeFileName);
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation = CSharpCompilation.Create(dllPath, new[] { syntaxTree }, references, compilationOptions);

            var emitResult = compilation.Emit(stream);
            if (emitResult.Success) {
                stream.Seek(0, SeekOrigin.Begin);
                var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
                return GetPluginsFrom(assembly);
            } else {
                foreach (var diagnostic in emitResult.Diagnostics) {
                    var pos = diagnostic.Location.GetLineSpan();
                    var location =
                        "(" + pos.Path + "@Line" + (pos.StartLinePosition.Line + 1) +
                        ":" +
                        (pos.StartLinePosition.Character + 1) + ")";
                    Debug.WriteLine(
                        $"[{diagnostic.Severity}, {location}]{diagnostic.Id}, {diagnostic.GetMessage()}"
                    );
                }
                return new (object instance, string name, MethodInfo runMethod)[0];
            }
        }

        static IEnumerable<(object instance, string name, MethodInfo runMethod)> GetPluginsFrom(Assembly assembly)
            => assembly.GetTypes()
                       .Where(type => type.IsPublic && !type.IsNestedPublic)
                       .Select(CreatePlugIn)
                       .Where(plugin => plugin.instance is not null && plugin.name is not null && plugin.runMethod is not null);

        static (object? instance, string? name, MethodInfo? runMethod) CreatePlugIn(Type type)
        {
            var instance = Activator.CreateInstance(type);
            return (instance, type.GetProperty(namePropertyName, typeof(string))?.GetValue(instance) as string, type.GetMethod(runMethodName, BindingFlags.Public | BindingFlags.Instance, new Type[] { }));
        }

        static (string dllName, string dllPath)? GetNewDllPath()
        {
            const int maximumDllFileNumber = 1000;

            string dllFolderName = GetPluginFolderName();

            for (var number = 1; number <= maximumDllFileNumber; number++) {
                var dllName = $"plugin{number:D8}.dll";
                var dllPath = $@"{dllFolderName}\{dllName}";
                if (!File.Exists(dllPath))
                    return (dllName, dllPath);
            }
            return null;
        }

        static string GetPluginFolderName()
        {
            const string dllFolder = "Plugins";
            if (!Directory.Exists(dllFolder))
                Directory.CreateDirectory(dllFolder);
            return dllFolder;
        }
    }
}
