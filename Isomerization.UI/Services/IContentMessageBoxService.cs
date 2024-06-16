using System.Windows;

namespace Isomerization.UI.Services;

public interface IContentMessageBoxService
{
    Task<MessageBoxResult> Show(string messageBoxText);
    Task<MessageBoxResult> Show(string messageBoxText, string caption);
    Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton button);
    Task<MessageBoxResult> Show(string messageBoxText, MessageBoxButton button);
}