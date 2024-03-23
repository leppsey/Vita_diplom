// using System.Collections.ObjectModel;
// using System.Linq;
// using System.Windows;
//
// using PlenkaAPI.Data;
// using PlenkaAPI.Models;
//
// using PlenkaWpf.Utils;
// using PlenkaWpf.View;
//
// using MessageBox = HandyControl.Controls.MessageBox;
//
//
// namespace PlenkaWpf.VM
// {
//     internal class ObjectPropertiesControlVM : ViewModelBase
//
//     {
//     #region Functions
//
//     #region Constructors
//
//         public ObjectPropertiesControlVM()
//         {
//             MembraneObjectTypes = _db.ObjectTypes.Local.ToObservableCollection();
//         }
//
//     #endregion
//
//     #endregion
//
//
//     #region Properties
//
//         private static readonly MembraneContext _db = DbContextSingleton.GetInstance();
//         public ObjectType SelectedType { get; set; }
//
//         public ObservableCollection<ObjectType> MembraneObjectTypes { get; set; }
//
//     #endregion
//
//
//     #region Commands
//
//         private RelayCommand _addNewObjectType;
//
//         /// <summary>
//         ///     Команда, открывающая окно создания пользователя
//         /// </summary>
//         public RelayCommand AddObjectType
//         {
//             get
//             {
//                 return _addNewObjectType ??= new RelayCommand(o =>
//                 {
//                     ShowChildWindow(new SelectPropertiesWindow(new ObjectType()));
//                 });
//             }
//         }
//
//         private RelayCommand _editObjectType;
//
//         /// <summary>
//         ///     Команда, открывающая окно редактирования пользователя
//         /// </summary>
//         public RelayCommand EditObjectType
//         {
//             get
//             {
//                 return _editObjectType ??= new RelayCommand(o =>
//                 {
//                     ShowChildWindow(new SelectPropertiesWindow(SelectedType));
//                 }, _ => SelectedType != null);
//             }
//         }
//
//         private RelayCommand _deleteObjectType;
//
//         /// <summary>
//         ///     Команда, удаляющая пользователя
//         /// </summary>
//         public RelayCommand DeleteObjectType
//         {
//             get
//             {
//                 return _deleteObjectType ??= new RelayCommand(o =>
//                 {
//                     if (MessageBox.Show($"Вы действительно хотите удалить тип объекта \"{SelectedType.TypeName}\" и все объекты связанные с ним?" +
//                                         $"\nСвязанные объекты:\n{string.Join("\n", _db.MembraneObjects.Where(mo => mo.Type == SelectedType).Select(mo => mo.ObName))}",
//                                         "Удаление типа объекта", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
//                         MessageBoxResult.Yes)
//                     {
//                         _db.ObjectTypes.Remove(SelectedType);
//                         _db.SaveChanges();
//                     }
//                 }, _ => SelectedType != null);
//             }
//         }
//
//     #endregion
//     }
// }
