namespace Atc.Wpf.Controls.Media.W3cSvg.FileLoaders;

public interface IExternalFileLoader
{
    Stream? LoadFile(
        string hRef,
        string svgFilename);
}