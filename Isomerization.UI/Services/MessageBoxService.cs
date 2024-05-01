using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using TextBlock = System.Windows.Controls.TextBlock;


namespace Isomerization.UI.Services;

public class MessageBoxService : IMessageBoxService
{
    private readonly MainWindow _mw;

    public MessageBoxService(MainWindow mw)
    {
        _mw = mw;
    }

    public Task<MessageBoxResult> Show(string messageBoxText)
    {
        return Show(_mw, messageBoxText);
    }

    public Task<MessageBoxResult> Show(string messageBoxText, string caption)
    {
        return Show(_mw, messageBoxText, caption);
    }

    public Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton button)
    {
        return Show(_mw, messageBoxText, caption, button);
    }

    public Task<MessageBoxResult> Show(string messageBoxText, MessageBoxButton button)
    {
        return Show(messageBoxText, "", button);
    }

    public Task<MessageBoxResult> Show(Window window, string messageBoxText)
    {
        return Show(window, messageBoxText, null);
    }

    public Task<MessageBoxResult> Show(Window window, string messageBoxText, string caption)
    {
        return Show(null, messageBoxText, caption, MessageBoxButton.OK);
    }

    public async Task<MessageBoxResult> Show(Window window, string messageBoxText, string caption, MessageBoxButton button)
    {
        var tcs = new TaskCompletionSource<MessageBoxResult>();
        var mb = new MessageBox();
        mb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        mb.Owner = window;
        mb.Title = caption;

        var content = new TextBlock {Text = messageBoxText, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(3)};
        mb.Content = content;
        MessageBoxResult result;
        
        switch (button)
        {
            case MessageBoxButton.OK:
                mb.CloseButtonText = "Ок";
                break;
            case MessageBoxButton.OKCancel:
                mb.PrimaryButtonAppearance = ControlAppearance.Primary;
                mb.PrimaryButtonText = "Ок";
                mb.IsPrimaryButtonEnabled = true;
                mb.CloseButtonAppearance = ControlAppearance.Secondary;
                mb.CloseButtonText = "Отмена";
                break;
            case MessageBoxButton.YesNoCancel:
                mb.PrimaryButtonAppearance = ControlAppearance.Primary;
                mb.PrimaryButtonText = "Да";
                mb.IsPrimaryButtonEnabled = true;
                mb.SecondaryButtonAppearance = ControlAppearance.Secondary;
                mb.SecondaryButtonText = "Нет";
                mb.IsSecondaryButtonEnabled = true;
                mb.CloseButtonAppearance = ControlAppearance.Transparent;
                mb.CloseButtonText = "Отмена";
                break;
            case MessageBoxButton.YesNo:
                mb.PrimaryButtonAppearance = ControlAppearance.Primary;
                mb.PrimaryButtonText = "Да";
                mb.IsPrimaryButtonEnabled = true;
                mb.CloseButtonAppearance = ControlAppearance.Secondary;
                mb.CloseButtonText = "Нет";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(button), button, null);
        }
        window.IsEnabled = false;
        var res = await mb.ShowDialogAsync();
        switch (button)
        {
            case MessageBoxButton.OK:
                switch (res)
                {
                    case Wpf.Ui.Controls.MessageBoxResult.None:
                        result = MessageBoxResult.None;
                        break;
                    case Wpf.Ui.Controls.MessageBoxResult.Primary:
                        result = MessageBoxResult.OK;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case MessageBoxButton.OKCancel:
                switch (res)
                {
                    case Wpf.Ui.Controls.MessageBoxResult.None:
                        result = MessageBoxResult.None;
                        break;
                    case Wpf.Ui.Controls.MessageBoxResult.Primary:
                        result = MessageBoxResult.OK;
                        break;
                    case Wpf.Ui.Controls.MessageBoxResult.Secondary:
                        result = MessageBoxResult.Cancel;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case MessageBoxButton.YesNoCancel:
                throw new InvalidOperationException("MessageBoxButton.YesNoCancel not supported");
                break;
            case MessageBoxButton.YesNo:
                switch (res)
                {
                    case Wpf.Ui.Controls.MessageBoxResult.None:
                        result = MessageBoxResult.None;
                        break;
                    case Wpf.Ui.Controls.MessageBoxResult.Primary:
                        result = MessageBoxResult.Yes;
                        break;
                    case Wpf.Ui.Controls.MessageBoxResult.Secondary:
                        result = MessageBoxResult.No;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(button), button, null);
        }

        window.IsEnabled = true;
        return result;
    }
}
