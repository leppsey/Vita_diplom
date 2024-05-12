using System.Windows;
using System.Windows.Controls;
using Wpf.Ui;
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
public class ContentMessageBoxService : IContentMessageBoxService
{
    private readonly IContentDialogService _contentDialogService;

    public ContentMessageBoxService(IContentDialogService contentDialogService)
    {
        _contentDialogService = contentDialogService;
    }
    
    public Task<MessageBoxResult> Show(string messageBoxText)
    {
        return Show(messageBoxText, null);
    }   
    public Task<MessageBoxResult> Show(string messageBoxText, MessageBoxButton button)
    {
        return Show(messageBoxText, null, MessageBoxButton.OK);
    }

    public Task<MessageBoxResult> Show(string messageBoxText, string caption)
    {
        return Show(messageBoxText, caption, MessageBoxButton.OK);
    }

    public async Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton button)
    {
        var tcs = new TaskCompletionSource<MessageBoxResult>();
        var dialog = new ContentDialog();
        dialog.Title = caption;

        var content = new TextBlock {Text = messageBoxText, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(3)};
        dialog.Content = content;
        MessageBoxResult result;
        
        switch (button)
        {
            case MessageBoxButton.OK:
                dialog.CloseButtonText = "Ок";
                break;
            case MessageBoxButton.OKCancel:
                dialog.PrimaryButtonAppearance = ControlAppearance.Primary;
                dialog.PrimaryButtonText = "Ок";
                dialog.IsPrimaryButtonEnabled = true;
                dialog.CloseButtonAppearance = ControlAppearance.Secondary;
                dialog.CloseButtonText = "Отмена";
                break;
            case MessageBoxButton.YesNoCancel:
                dialog.PrimaryButtonAppearance = ControlAppearance.Primary;
                dialog.PrimaryButtonText = "Да";
                dialog.IsPrimaryButtonEnabled = true;
                dialog.SecondaryButtonAppearance = ControlAppearance.Secondary;
                dialog.SecondaryButtonText = "Нет";
                dialog.IsSecondaryButtonEnabled = true;
                dialog.CloseButtonAppearance = ControlAppearance.Transparent;
                dialog.CloseButtonText = "Отмена";
                break;
            case MessageBoxButton.YesNo:
                dialog.PrimaryButtonAppearance = ControlAppearance.Primary;
                dialog.PrimaryButtonText = "Да";
                dialog.IsPrimaryButtonEnabled = true;
                dialog.CloseButtonAppearance = ControlAppearance.Secondary;
                dialog.CloseButtonText = "Нет";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(button), button, null);
        }

        var res = await _contentDialogService.ShowAsync(dialog, new CancellationToken());
        switch (button)
        {
            case MessageBoxButton.OK:
                switch (res)
                {
                    case Wpf.Ui.Controls.ContentDialogResult.None:
                        result = MessageBoxResult.None;
                        break;
                    case Wpf.Ui.Controls.ContentDialogResult.Primary:
                        result = MessageBoxResult.OK;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case MessageBoxButton.OKCancel:
                switch (res)
                {
                    case Wpf.Ui.Controls.ContentDialogResult.None:
                        result = MessageBoxResult.None;
                        break;
                    case Wpf.Ui.Controls.ContentDialogResult.Primary:
                        result = MessageBoxResult.OK;
                        break;
                    case Wpf.Ui.Controls.ContentDialogResult.Secondary:
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
                    case Wpf.Ui.Controls.ContentDialogResult.None:
                        result = MessageBoxResult.None;
                        break;
                    case Wpf.Ui.Controls.ContentDialogResult.Primary:
                        result = MessageBoxResult.Yes;
                        break;
                    case Wpf.Ui.Controls.ContentDialogResult.Secondary:
                        result = MessageBoxResult.No;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(button), button, null);
        }

        return result;
    }


}
