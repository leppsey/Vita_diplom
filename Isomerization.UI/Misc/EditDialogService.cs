using System.Windows;
using Autofac;
using UI.Services.MyDialogService;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace Isomerization.UI.Misc;

public class EditDialogService
{
    private readonly IContentDialogService _contentDialogService;

    public EditDialogService(IContentDialogService contentDialogService)
    {
        _contentDialogService = contentDialogService;
    }

    public async Task<TResult> ShowDialog<T,TResult>(TResult editingModel) where T : IDialogEditControl<TResult> where TResult: class
    {
        var control = App.GetService<T>();
        var viewmodel = control.ViewModel;
        viewmodel.Data = editingModel;
        var cancelToken = new CancellationTokenSource();
        viewmodel.FinishInteraction = () => cancelToken.Cancel();
        var dialog = new ContentDialog()
        {
            IsFooterVisible = false,
            Content = control,
            IsSecondaryButtonEnabled = false,
        };

        try
        {
            await _contentDialogService.ShowAsync(dialog, cancelToken.Token);
        }
        catch (OperationCanceledException e)
        {
            //ignore
        }

        return viewmodel.Result;
    }
}