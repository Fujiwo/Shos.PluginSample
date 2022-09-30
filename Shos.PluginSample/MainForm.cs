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
                        .ForEach(plugin => AddToMenu(toolsMenuItem, plugin));

            codeTextBox.Text = @"
    public abstract class TestClass0
    {
        public abstract string Name     { get; }
        public abstract char Shortcut { get; }
        public virtual void Run() => System.Windows.Forms.MessageBox.Show($""{Name} is running."", ""Name"");
    }

    public class TestClass1 : TestClass0
    {
        public override string Name => ""Tool1"";
        public override char Shortcut => '1';
    }

    public class TestClass2 : TestClass0
    {
        public override string Name => ""Tool2"";
        public override char Shortcut => '2';
    }
";
        }

        void addButton_Click(object sender, EventArgs e)
        {
            messageTextBox.Text = "";
            try {
                CreatePlugins().ForEach(plugin => AddToMenu(toolsMenuItem, plugin));
            } catch (Exception ex) {
                messageTextBox.Text = ex.Message;
            }
        }

        //void removeButton_Click(object sender, EventArgs e)
        //    => CreatePlugins().ForEach(plugin => RemoveFromMenu(toolsMenuItem, plugin.name));

        static bool AddToMenu(ToolStripMenuItem menuItem, Plugin plugin)
        {
            var (exists, _) = Exists(menuItem, plugin.Name ?? "");
            if (exists)
                return false;

            ToolStripMenuItem newMenuItem = ToMenuItem(plugin);
            menuItem.DropDownItems.Add(newMenuItem);
            return true;
        }

        static ToolStripMenuItem ToMenuItem(Plugin plugin)
        {
            var newMenuItem = new ToolStripMenuItem { Text = $"{plugin.Name ?? ""}(&{plugin.Shortcut})", Name = plugin.Name ?? ""};
            newMenuItem.Click += (_, __) => plugin.RunMethod?.Invoke(plugin.Instance, new object[] {});
            return newMenuItem;
        }

        static (bool exists, ToolStripItem[] foundMenuItems) Exists(ToolStripMenuItem menuItem, string name)
        {
            var foundMenuItems = menuItem.DropDownItems.Find(name, true);
            return foundMenuItems is null || foundMenuItems.Length == 0 ? (false, new ToolStripItem[0]) : (true, foundMenuItems);
        }

        //static bool RemoveFromMenu(ToolStripMenuItem menuItem, Plugin plugin)
        //{
        //    var (exists, foundMenuItems) = Exists(menuItem, plugin.Name ?? "");
        //    if (!exists)
        //        return false;

        //    foundMenuItems.ForEach(foundMenuItem => menuItem.DropDownItems.Remove(foundMenuItem));
        //    return true;
        //}

        /// <exception cref="Exception"/>
        IEnumerable<Plugin> CreatePlugins()
            => CreatePlugins(codeTextBox.Text);

        /// <exception cref="Exception"/>
        IEnumerable<Plugin> CreatePlugins(string code)
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