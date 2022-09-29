using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace Shos.PluginSample
{
    internal partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            PluginHelper.GetPlugins()
                        .ForEach(plugin => AddToMenu(toolsMenuItem, plugin.instance, plugin.runMethod, plugin.name));

            codeTextBox.Text = @"

    public class TestClass1
    {
        public string Name => ""Tool1"";

        public void Run()
        {
            System.Windows.Forms.MessageBox.Show($""{Name} is running."", ""Name"");
        }
    }

    public class TestClass2
    {
        public string Name => ""Tool2"";

        public void Run()
        {
            System.Windows.Forms.MessageBox.Show($""{Name} is running."", ""Name"");
        }
    }
";
        }

        void addButton_Click(object sender, EventArgs e)
        {
            try {
                CreatePlugins().ForEach(plugin => AddToMenu(toolsMenuItem, plugin.instance, plugin.runMethod, plugin.name));
            } catch (Exception ex) {
                messageTextBox.Text = ex.Message;
            }
        }

        //void removeButton_Click(object sender, EventArgs e)
        //    => CreatePlugins().ForEach(plugin => RemoveFromMenu(toolsMenuItem, plugin.name));

        static bool AddToMenu(ToolStripMenuItem menuItem, object instance, MethodInfo runMethod, string name)
        {
            var (exists, _) = Exists(menuItem, name);
            if (exists)
                return false;

            var newMenuItem = new ToolStripMenuItem { Text = name, Name = name };
            newMenuItem.Click += (_, __) => runMethod.Invoke(instance, new object[] {});
            menuItem.DropDownItems.Add(newMenuItem);
            return true;
        }

        static (bool exists, ToolStripItem[] foundMenuItems) Exists(ToolStripMenuItem menuItem, string name)
        {
            var foundMenuItems = menuItem.DropDownItems.Find(name, true);
            return foundMenuItems is null || foundMenuItems.Length == 0 ? (false, new ToolStripItem[0]) : (true, foundMenuItems);
        }

        //static bool RemoveFromMenu(ToolStripMenuItem menuItem, string name)
        //{
        //    var (exists, foundMenuItems) = Exists(menuItem, name);
        //    if (!exists)
        //        return false;

        //    foundMenuItems.ForEach(foundMenuItem => menuItem.DropDownItems.Remove(foundMenuItem));
        //    return true;
        //}

        /// <exception cref="Exception"/>
        IEnumerable<(object instance, string name, MethodInfo runMethod)> CreatePlugins()
            => CreatePlugins(codeTextBox.Text);

        /// <exception cref="Exception"/>
        IEnumerable<(object instance, string name, MethodInfo runMethod)> CreatePlugins(string code)
        {
            var options               = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);
            var assemblyDirectoryPath = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? "";
            var references            = new MetadataReference[] {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile($"{assemblyDirectoryPath}/mscorlib.dll"),
                MetadataReference.CreateFromFile($"{assemblyDirectoryPath}/System.Runtime.dll"),
                MetadataReference.CreateFromFile($"{assemblyDirectoryPath.Replace("Microsoft.NETCore.App", "Microsoft.WindowsDesktop.App")}/System.Windows.Forms.dll")
            };
            return PluginHelper.CreatePlugins(code, options, references);
        }
    }
}