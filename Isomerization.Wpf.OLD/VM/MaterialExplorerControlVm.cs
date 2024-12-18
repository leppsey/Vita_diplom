﻿// using System.Collections.ObjectModel;
// using System.Windows;
//
// using Isomerization.Domain.Data;
// using Isomerization.Domain.Models;
//
// using Isomerization.Wpf.OLD.Utils;
// using Isomerization.Wpf.OLD.View;
//
// using MessageBox = HandyControl.Controls.MessageBox;
//
//
// namespace Isomerization.Wpf.OLD.VM
// {
//     public class MaterialExplorerControlVm : ViewModelBase
//     {
//     #region Functions
//
//     #region Constructors
//
//         public MaterialExplorerControlVm()
//         {
//             _db.SavedChanges += (sender, args) =>
//             {
//                 OnPropertyChanged(nameof(Materials));
//             };
//
//
//             //var db = DbContextSingleton.GetInstance();
//             Materials = _db.MembraneObjects.Local.ToObservableCollection();
//         }
//
//     #endregion
//
//     #endregion
//
//
//     #region Properties
//
//         private readonly MembraneContext _db = DbContextSingleton.GetInstance();
//         public ObservableCollection<MembraneObject> Materials { get; set; }
//
//         public MembraneObject SelectedMemObject { get; set; }
//
//     #endregion
//
//
//     #region Commands
//
//         private RelayCommand _addNewMemObject;
//
//         /// <summary>
//         ///     Команда, открывающая окно создания нового объекта
//         /// </summary>
//         public RelayCommand AddNewMemObject
//         {
//             get
//             {
//                 return _addNewMemObject ??= new RelayCommand(o =>
//                 {
//                     ShowChildWindow(new CreateMaterialWindow());
//                 });
//             }
//         }
//
//         private RelayCommand _editMemObject;
//
//         /// <summary>
//         ///     Команда, открывающая окно редактирования нового объекта
//         /// </summary>
//         public RelayCommand EditMemObject
//         {
//             get
//             {
//                 return _editMemObject ??= new RelayCommand(o =>
//                                                            {
//                                                                ShowChildWindow(new MaterialEditWindow(SelectedMemObject));
//                                                            },
//                                                            c => SelectedMemObject != null);
//             }
//         }
//
//         private RelayCommand _deleteMemObject;
//
//         /// <summary>
//         ///     Команда, удаляющая объект
//         /// </summary>
//         public RelayCommand DeleteMemObject
//         {
//             get
//             {
//                 return _deleteMemObject ??= new RelayCommand(o =>
//                 {
//                     if (MessageBox.Show($"Вы действительно хотите удалить объект {SelectedMemObject.ObName}?",
//                                         "Удаление объекта", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
//                     {
//                         foreach (var value in SelectedMemObject.Values)
//                         {
//                             _db.Values.Remove(value);
//                         }
//
//                         _db.MembraneObjects.Remove(SelectedMemObject);
//                         _db.SaveChanges();
//                     }
//                 }, c => SelectedMemObject != null);
//             }
//         }
//
//     #endregion
//     }
// }
