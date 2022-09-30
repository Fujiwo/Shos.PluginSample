using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Windows.Forms;

namespace Shos.PluginSample
{
    internal partial class MainForm : Form
    {
        const string defaultCodeText = @"
    public abstract class Base
    {
        public abstract string Name     { get; }
        public abstract char Shortcut { get; }
        public virtual void Run() => System.Windows.Forms.MessageBox.Show($""{Name} is running."", Name);
    }

    public class Derived1 : Base
    {
        public override string Name => ""Tool1"";
        public override char Shortcut => '1';
    }

    public class Derived2 : Base
    {
        public override string Name => ""Tool2"";
        public override char Shortcut => '2';
    }
";

        public MainForm()
        {
            InitializeComponent();
            InitializeOthers();
        }

        void InitializeOthers()
        {
            codeTextBox.Text = defaultCodeText;

            PluginHelper.GetPlugins()
                        .ForEach(plugin => AddToMenu(pluginsMenuItem, plugin));
        }

        async void addButton_Click(object sender, EventArgs e)
        {
            void ShowMessage(string message, bool isOK)
            {
                messageTextBox.ForeColor = isOK ? Color.DarkGreen : Color.DarkRed;
                messageTextBox.Text      = message;
            }

            addPluginsButton.Enabled = false;
            messageTextBox.Text      = "";

            try {
                var plugins = await CreatePlugins();
                plugins.ForEach(plugin => AddToMenu(pluginsMenuItem, plugin));
                ShowMessage(message: "OK.", isOK: true);
            } catch (Exception ex) {
                ShowMessage(message: ex.Message, isOK: false);
            } finally {
                addPluginsButton.Enabled = true;
            }
        }

        void removeAllPluginsMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAllFromMenuItem(pluginsMenuItem);
            PluginHelper.RemoveAll();
        }

        static void AddToMenu(ToolStripMenuItem menuItem, Plugin plugin)
        {
            RemoveFromMenu(menuItem, plugin);
            menuItem.DropDownItems.Add(ToMenuItem(plugin));
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

        static bool RemoveFromMenu(ToolStripMenuItem menuItem, Plugin plugin)
        {
            var (exists, foundMenuItems) = Exists(menuItem, plugin.Name ?? "");
            if (!exists)
                return false;

            foundMenuItems.ForEach(foundMenuItem => menuItem.DropDownItems.Remove(foundMenuItem));
            return true;
        }

        static void RemoveAllFromMenuItem(ToolStripMenuItem pluginsMenuItem)
            => pluginsMenuItem.DropDownItems.Clear();

        /// <exception cref="Exception"/>
        async Task<IEnumerable<Plugin>> CreatePlugins()
            => await CreatePlugins(codeTextBox.Text);

        /// <exception cref="Exception"/>
        async Task<IEnumerable<Plugin>> CreatePlugins(string code)
        {
            var assemblyDirectoryPath = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? "";
            var references            = new MetadataReference[] {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile($"{assemblyDirectoryPath}/mscorlib.dll"),
                MetadataReference.CreateFromFile($"{assemblyDirectoryPath}/System.Runtime.dll"),
                MetadataReference.CreateFromFile($"{assemblyDirectoryPath.Replace("Microsoft.NETCore.App", "Microsoft.WindowsDesktop.App")}/System.Windows.Forms.dll")
            };
            return await PluginHelper.CreatePlugins(code, references);
        }
    }
}
