using System;
using System.Linq;
using System.IO;

namespace ConvertBase64
{
    class Program
    {
        static int Main(string[] args)
        {
            var path = args.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(path)) return -1;
            if (!File.Exists(path)) return -1;
            var dirPath = Path.GetDirectoryName(path);
            var storePath = Path.Combine(dirPath, "base64.txt");
            using var file = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var memory = new MemoryStream();
            file.CopyTo(memory);
            memory.Flush();
            memory.Position = 0;
            var base64 = Convert.ToBase64String(memory.ToArray());
            using var store = new FileStream(storePath, FileMode.Create, FileAccess.ReadWrite);
            using var storeWriter = new StreamWriter(store);
            storeWriter.Write(base64);
            storeWriter.Flush();
            store.Flush();
            return 0;
        }
    }
}