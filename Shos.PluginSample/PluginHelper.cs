using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Shos.PluginSample
{
    public class Plugin
    {
        public string?     Name      { get; set; } = "";
        public char        Shortcut  { get; set; } = '\0';
        public object?     Instance  { get; set; }
        public MethodInfo? RunMethod { get; set; }

        public bool IsValid => Name is not null && Instance is not null && RunMethod is not null;
    }

    public static class PluginHelper
    {
        const string namePropertyName               = "Name";
        const string shortcutPropertyName           = "Shortcut";
        const string runMethodName                  = "Run";
        const string pluginFileNameWithoutExtension = "Plugin";
        static readonly string codeFileName         = $"{pluginFileNameWithoutExtension}.cs";
        const string applicationName                = nameof(Shos.PluginSample);
        const string dllFolder                      = "Plugins";
        const string removeAllPluginsFileName       = "RemoveAllPlugins";

        static string DllFolderPath {
            get {
                var dllFolerPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName), dllFolder);

                if (!Directory.Exists(dllFolerPath))
                    Directory.CreateDirectory(dllFolerPath);
                return dllFolerPath;
            }
        }

        static PluginHelper()
        {
            var removeAllFilePath = GetLatestRemoveAllFile();

            if (removeAllFilePath is not null) {
                var removeAllFileName             = Path.GetFileName(removeAllFilePath);
                var removeAllFileNameDateTimeText = GetCurrentDateTimeText(removeAllFileName, removeAllPluginsFileName);

                Directory.GetFiles(DllFolderPath)
                         .Where(filePath => {
                              var fileName = Path.GetFileName(filePath);
                              return GetCurrentDateTimeText(fileName, pluginFileNameWithoutExtension).CompareTo(removeAllFileNameDateTimeText) < 0 ||
                                     fileName.StartsWith(removeAllPluginsFileName);
                         })
                         .ForEach(filePath => File.Delete(filePath));
            }
        }

        static string? GetLatestRemoveAllFile()
            => Directory.GetFiles(DllFolderPath)
                        .Select(filePath => Path.GetFileName(filePath))
                        .OrderBy(fileName => fileName)
                        .LastOrDefault(fileName => fileName.StartsWith(removeAllPluginsFileName));

        public static IEnumerable<Plugin> GetPlugins()
            => GetPluginAssemblies().Select(GetPluginsFrom)
                                    .SelectMany(plugins => plugins);

        /// <exception cref="Exception"/>
        public static async Task<IEnumerable<Plugin>> CreatePlugins(string code, MetadataReference[] references)
            => await CreatePlugins(code, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Default), references);
        
        /// <exception cref="Exception"/>
        public static async Task<IEnumerable<Plugin>> CreatePlugins(string code, CSharpParseOptions options, MetadataReference[] references)
        {
            var dllPath = GetNewDllPath();
            if (dllPath is null)
                return new Plugin[0];

            using var stream = File.Create(dllPath.Value.dllFilePath);
            return await CreatePlugins(code, dllPath.Value.dllFileName, codeFileName, options, references, stream);
        }

        public static void RemoveAll() => CreateRemoveAllFile();

        static void CreateRemoveAllFile()
        {
            var removeAllFilePath = Path.Combine(DllFolderPath, WithCurrentDateTime(removeAllPluginsFileName));
            if (File.Exists(removeAllFilePath))
                return;

            using (var fileStream = File.Create(removeAllFilePath)) {
                var content = new UTF8Encoding(true).GetBytes("This is a file to remove all plugins.");
                fileStream.Write(content, 0, content.Length);
            }
        }

        static IEnumerable<Assembly> GetPluginAssemblies()
        {
            List<Assembly> assemblies = new();
            var files = Directory.GetFiles(DllFolderPath);
            foreach (var file in files) {
                try {
                    assemblies.Add(Assembly.LoadFrom(file));
                } catch (Exception) {
                }
            }
            return assemblies;
        }

        /// <exception cref="Exception"/>
        static async Task<IEnumerable<Plugin>> CreatePlugins(string code, string dllPath, string codeFileName, CSharpParseOptions options, MetadataReference[] references, FileStream stream)
        {
            var syntaxTree         = CSharpSyntaxTree.ParseText(code, options, codeFileName);
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation        = CSharpCompilation.Create(dllPath, new[] { syntaxTree }, references, compilationOptions);
            var emitResult         = await Task.Run(() => compilation.Emit(stream));
            if (emitResult.Success) {
                stream.Seek(0, SeekOrigin.Begin);
                var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
                return GetPluginsFrom(assembly);
            } else {
                throw new Exception(CreateErrorMessage(emitResult));
            }
        }

        static string CreateErrorMessage(EmitResult emitResult)
        {
            StringBuilder stringBuilder = new();

            emitResult.Diagnostics.ForEach(diagnostic => {
                var pos = diagnostic.Location.GetLineSpan();
                var location = "(" + pos.Path + "@Line" + (pos.StartLinePosition.Line + 1) + ":" + (pos.StartLinePosition.Character + 1) + ")";
                stringBuilder.AppendLine($"[{diagnostic.Severity}, {location}]{diagnostic.Id}, {diagnostic.GetMessage()}");
            });

            return stringBuilder.ToString();
        }

        static IEnumerable<Plugin> GetPluginsFrom(Assembly assembly)
            => assembly.GetTypes()
                       .Where(type => type.IsPublic && !type.IsNestedPublic && !type.IsAbstract)
                       .Select(CreatePlugIn)
                       .Where(plugin => plugin is not null && plugin.IsValid)!;

        static Plugin? CreatePlugIn(Type type)
        {
            try {
                var instance = Activator.CreateInstance(type);
                return new Plugin {
                    Name = type.GetProperty(namePropertyName, typeof(string))?.GetValue(instance) as string,
                    Shortcut = (char)(type.GetProperty(shortcutPropertyName, typeof(char))?.GetValue(instance) ?? '\0'),
                    Instance = instance,
                    RunMethod = type.GetMethod(runMethodName, BindingFlags.Public | BindingFlags.Instance, new Type[] { })
                };
            } catch (Exception) {
                return null;
            }
        }

        static (string dllFileName, string dllFilePath)? GetNewDllPath()
        {
            const int maximumDllFileNumber = 99;
            string dllFolderName           = DllFolderPath;
            
            for (var number = 1; number <= maximumDllFileNumber; number++) {
                var dllFileName = $"{WithCurrentDateTime(pluginFileNameWithoutExtension)}.{number:D2}.dll";
                var dllFilePath = Path.Combine(dllFolderName, dllFileName);
                if (!File.Exists(dllFilePath))
                    return (dllFileName, dllFilePath);
            }
            return null;
        }

        static string WithCurrentDateTime(string text)
            => $"{text}.{GetStringFromCurrentDateTime()}";

        static string GetCurrentDateTimeText(string text, string baseText)
            => text.Replace($"{baseText}.", "");

        static string GetStringFromCurrentDateTime()
        {
            var dateTime = DateTime.Now.ToUniversalTime();
            return $"{dateTime.Year:D4}.{dateTime.Month:D2}.{dateTime.Day:D2}.{dateTime.Minute:D2}.{dateTime.Second:D2}.{dateTime.Millisecond:D3}";
        }
    }
}
