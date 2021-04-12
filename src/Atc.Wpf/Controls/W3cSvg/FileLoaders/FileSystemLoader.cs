using System;
using System.Diagnostics;
using System.IO;

namespace Atc.Wpf.Controls.W3cSvg.FileLoaders
{
    public class FileSystemLoader : IExternalFileLoader
    {
        static FileSystemLoader()
        {
            Instance = new FileSystemLoader();
        }

        public static FileSystemLoader Instance { get; }

        public Stream? LoadFile(string hRef, string svgFilename)
        {
            var path = Environment.CurrentDirectory;
            if (!string.IsNullOrEmpty(svgFilename))
            {
                path = Path.GetDirectoryName(svgFilename);
            }

            var fileName = Path.Combine(path, hRef);
            if (File.Exists(fileName))
            {
                return File.OpenRead(fileName);
            }

            Trace.TraceWarning("Unresolved URI: " + hRef);

            return null;
        }
    }
}
