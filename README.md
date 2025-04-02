# Shos.PluginSample

Sample to dynamically add plug-ins from C# source code.

(The GUI is based Windows Forms.)

![Image](https://github.com/Fujiwo/Shos.PluginSample/blob/d6aa978df00f590d936d701dd3919ed0860f52dc/Documents/Images/snapshot01.png)

1. In the left text box, write a class or struct with a public string Name property, a char Shortcut property, and a void argumentless Run method.

ex.
```csharp
    public class Foo
    {
        public string Name   => "Foo";
        public char Shortcut => 'F';
        public void Run() => System.Windows.Forms.MessageBox.Show($"{Name} is running.", Name);
    }
```

2. Click the "Add Plugins" button to dynamically create plug-ins, add them to the menu, and make them executable.

The plug-ins are saved as DLLs in the "AppData" folder and will be added the next time it is launched.


## Author Info

Fujio Kojima: a software developer in Japan
* Microsoft MVP for Development Tools - Visual C# (Jul. 2005 - Dec. 2014)
* Microsoft MVP for .NET (Jan. 2015 - Oct. 2015)
* Microsoft MVP for Visual Studio and Development Technologies (Nov. 2015 - Jun. 2018)
* Microsoft MVP for Developer Technologies (Nov. 2018 - Jun. 2025)
* [MVP Profile](https://mvp.microsoft.com/en-us/PublicProfile/21482 "MVP Profile")
* [Blog (Japanese)](http://wp.shos.info "Blog (Japanese)")
* [Web Site (Japanese)](http://www.shos.info "Web Site (Japanese)")
* [Twitter](https://twitter.com/Fujiwo)
* [Instagram](https://www.instagram.com/fujiwo/)

## License

This library is under the MIT License.
