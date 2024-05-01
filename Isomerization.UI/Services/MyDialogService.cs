// using System.Diagnostics;
// using System.Threading.Tasks;
// using System.Windows;
// using Isomerization.UI;
// using Isomerization.UI.Misc;
// using Wpf.Ui;
// using Wpf.Ui.Contracts;
// using Wpf.Ui.Controls;
//
//
// namespace UI.Services.MyDialogService;
//
// public class MyDialogService
// {
//     private readonly IContentDialogService _dialogService;
//
//
//     public MyDialogService(IContentDialogService dialogService)
//     {
//         _dialogService = dialogService;
//     }
//
//     /// <summary>
//     ///     Показать диалоговое окно
//     /// </summary>
//     /// <param name="data">Передаваемые данные</param>
//     /// <typeparam name="T">Тип элемента для отображения в диалоговом окне</typeparam>
//     /// <returns>Результат работы диалогового окна</returns>
//     public Task<object?> ShowDialog<T>(object data = null) where T : FrameworkElement, IViewWithVM
//     {
//         var tcs = new TaskCompletionSource<object?>();
//         var e = (IViewWithVM)App.GetService<T>();
//         var dc = _dialogService.GetDialogHost();
//         _dialogService.ShowAsync()
//         // dc.DialogHeight = e.Height;
//         // dc.DialogWidth = e.Width;
//         dc.Content = e;
//         // dc.Footer = e.Footer;
//
//         //dc.Title = e.Title;
//         var viewModel = e.ViewModelObject;
//
//         if (viewModel is IDataHolder dataHolder)
//         {
//             dataHolder.Data = data;
//         }
//
//         if (viewModel is IInteractionAware interactionAware)
//         {
//             interactionAware.FinishInteraction = () =>
//             {
//                 dc.Hide();
//                 dc.Closed -= OnClosed;
//             };
//         }
//
//         dc.Closed += OnClosed;
//
//         void OnClosed(Dialog sender, RoutedEventArgs args)
//         {
//             if (viewModel is IResultHolder resultHolder)
//             {
//                 Debug.WriteLine("Вернулся результат");
//                 tcs.SetResult(resultHolder.Result);
//             }
//         }
//
//         dc.Show();
//
//         return tcs.Task;
//     }
// }
