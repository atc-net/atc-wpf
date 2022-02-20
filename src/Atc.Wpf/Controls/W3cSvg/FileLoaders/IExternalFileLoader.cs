namespace Atc.Wpf.Controls.W3cSvg.FileLoaders;

public interface IExternalFileLoader
{
    Stream? LoadFile(string hRef, string svgFilename);
}