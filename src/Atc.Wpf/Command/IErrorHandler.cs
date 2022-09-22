namespace Atc.Wpf.Command;

public interface IErrorHandler
{
    void HandleError(Exception ex);
}