namespace Shos.PluginSample
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            foreach (var item in @this)
                action(item);
        }
    }
}

//namespace Shos.PluginSample
//{
//    public static class DirectoryHelper
//    {
//        public static void Delete(string directoryPath)
//        {
//            if (!Directory.Exists(directoryPath))
//                return;

//            Directory.GetFiles(directoryPath)
//                     .ForEach(filePath => {
//                         File.SetAttributes(filePath, FileAttributes.Normal);
//                         File.Delete(filePath);
//                     });

//            Directory.GetDirectories(directoryPath)
//                     .ForEach(subDirectoryPath => Delete(subDirectoryPath));

//            Directory.Delete(directoryPath, false);
//        }
//    }
//}
