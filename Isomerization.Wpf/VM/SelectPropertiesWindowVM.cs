// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Collections.ObjectModel;
// using System.Globalization;
// using System.Linq;
// using System.Windows;
// using System.Windows.Data;
//
// using Isomerization.Domain.Data;
// using Isomerization.Domain.Models;
//
// using Isomerization.Wpf.Utils;
// using Isomerization.Wpf.View;
//
// using MessageBox = HandyControl.Controls.MessageBox;
//
//
// namespace Isomerization.Wpf.VM
// {
//     /// <summary>
//     ///     Конвертер значений для корректного отображенияпринадлежности свойства объекту
//     /// </summary>
//     public class ObjectInListConverter : IMultiValueConverter
//     {
//         public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//         {
//             var subset = values[1] as IList;
//             bool? result = subset.Contains(values[0]);
//
//             return result;
//         }
//
//         public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//         {
//             throw new NotImplementedException();
//         }
//     }
//
//
//     public class SelectPropertiesWindowVM : ViewModelBase
//     {
//     #region Functions
//
//     #region Constructors
//
//         public SelectPropertiesWindowVM(ObjectType objectType)
//         {
//             TempObjectType = new ObjectType
//             {
//                 TypeId = objectType.TypeId, TypeName = objectType.TypeName, DefaultProperties = objectType.DefaultProperties,
//                 MembraneObjects = objectType.MembraneObjects,
//             };
//
//             editingObjectType = objectType;
//             AvailableProperties = _db.Properties.ToList();
//             AvailableProperties = AvailableProperties.Except(ObjectTypeProperties).ToList();
//             AllProperties = _db.Properties.Local.ToObservableCollection();
//         }
//
//     #endregion
//
//     #endregion
//
//
//     #region Properties
//
//         private readonly ObjectType editingObjectType;
//
//         private readonly MembraneContext _db = DbContextSingleton.GetInstance();
//         public ObservableCollection<Property> AllProperties { get; set; }
//         public List<Property> AvailableProperties { get; set; }
//         public ObjectType TempObjectType { get; set; }
//         private readonly List<Property> _propertiesToDelete = new();
//         private readonly List<Property> _propertiesToAdd = new();
//         public Property SelectedProperty { get; set; }
//
//         public List<Property> ObjectTypeProperties
//         {
//             get
//             {
//                 return editingObjectType.DefaultProperties.Select(df => df.Prop).ToList();
//             }
//         }
//
//     #endregion
//
//
//     #region Commands
//
//         private RelayCommand _selectProperties;
//
//         /// <summary>
//         ///     Команда, сохраняющая результаты выбора свойств
//         /// </summary>
//         public RelayCommand SelectProperties
//         {
//             get
//             {
//                 return _selectProperties ??= new RelayCommand(o =>
//                 {
//                     var unableToDelete = new List<string>();
//
//                     foreach (var property in _propertiesToDelete)
//                     {
//                         if (_db.Values.Count(v => v.Prop == property && v.Mat.Type == TempObjectType && v.Value1 != null) !=
//                             0) //если есть заполненные записи с таким типом объекта и свойством в таблице значений
//                         {
//                             unableToDelete.Add(property.PropertyName);
//                         }
//                     }
//
//                     if (unableToDelete.Count != 0)
//                     {
//                         var res = MessageBox.Show("В базе найдены объекты, у которых есть есть удаляемые свойства." +
//                                                   $" Вы хотите продолжить операцию? В результате будут удалены все записи с этими свойствами: \n{string.Join("\n", unableToDelete)}",
//                                                   "Осторожно!",
//                                                   MessageBoxButton.YesNo, MessageBoxImage.Warning);
//
//                         if (res == MessageBoxResult.No)
//                         {
//                             return;
//                         }
//                     }
//
//                     foreach (var property in _propertiesToDelete) // удаляем свойства
//                     {
//                         _db.DefaultProperties.Remove(_db.DefaultProperties.Single(df => df.TypeId == TempObjectType.TypeId &&
//                                                                                         df.PropId == property.ProperrtyId));
//
//                         foreach (var membraneObject in TempObjectType.MembraneObjects)
//                         {
//                             _db.Values.Remove(_db.Values.Single(v => v.Mat == membraneObject && v.Prop == property));
//                         }
//                     }
//
//                     foreach (var property in _propertiesToAdd) // добавляем новые свойства
//                     {
//                         if (_db.DefaultProperties.Count(df => df.PropId == property.ProperrtyId && df.TypeId == TempObjectType.TypeId) == 0)
//                         {
//                             TempObjectType.DefaultProperties.Add(new DefaultProperty
//                                                                      {PropId = property.ProperrtyId,});
//
//                             //_db.DefaultProperties.Add(new DefaultProperty() {PropId = property.ProperrtyId, TypeId = TempObjectType.TypeId});
//                         }
//
//                         foreach (var membraneObject in TempObjectType.MembraneObjects)
//                         {
//                             if (_db.Values.Count(v => v.Mat == membraneObject && v.Prop == property) == 0)
//                             {
//                                 _db.Values.Add(new Value
//                                                    {Mat = membraneObject, Prop = property,});
//                             }
//                         }
//                     }
//
//
//                     editingObjectType.MembraneObjects = TempObjectType.MembraneObjects;
//                     editingObjectType.DefaultProperties = TempObjectType.DefaultProperties;
//                     editingObjectType.TypeId = TempObjectType.TypeId;
//                     editingObjectType.TypeName = TempObjectType.TypeName;
//
//                     if (!_db.ObjectTypes.Contains(editingObjectType))
//                     {
//                         _db.ObjectTypes.Add(editingObjectType);
//                     }
//
//                     //foreach (var mo in _db.MembraneObjects.Where(mo => mo.TypeId==editingObjectType.TypeId))
//                     //{
//                     //    _db.Values.Add(new Value(){Mat = mo, Prop = })
//                     //}
//                     _db.SaveChanges();
//                     OnClosingRequest();
//                 });
//             }
//         }
//
//         private RelayCommand _isCompletedUncheckedCommand;
//
//         /// <summary>
//         ///     Команда, обрабатывающая на выключение чекбокса со свойством
//         /// </summary>
//         public RelayCommand IsCompletedUncheckedCommand
//         {
//             get
//             {
//                 return _isCompletedUncheckedCommand ??= new RelayCommand(o =>
//                 {
//                     _propertiesToAdd.Remove((Property) o);
//                     _propertiesToDelete.Add((Property) o);
//                 });
//             }
//         }
//
//
//         private RelayCommand _isCompletedCheckedCommand;
//
//         /// <summary>
//         ///     Команда, обрабатывающая на включение чекбокса со свойством
//         /// </summary>
//         public RelayCommand IsCompletedCheckedCommand
//         {
//             get
//             {
//                 return _isCompletedCheckedCommand ??= new RelayCommand(o =>
//                 {
//                     _propertiesToDelete.Remove((Property) o);
//                     _propertiesToAdd.Add((Property) o);
//
//                     //MembraneObject.Values.Add((new Value() { Mat = MembraneObject, Prop = (TempProperty)o }));
//                 });
//             }
//         }
//
//
//         private RelayCommand _createProperty;
//
//         /// <summary>
//         ///     Команда, открывающая окно создания свойства
//         /// </summary>
//         public RelayCommand CreateProperty
//         {
//             get
//             {
//                 return _createProperty ??= new RelayCommand(o =>
//                 {
//                     ShowChildWindow(new CreatePropertyWindow(new Property()));
//                 });
//             }
//         }
//
//         private RelayCommand _editProperty;
//
//         /// <summary>
//         ///     Команда, открывающая окно редактирования свойства
//         /// </summary>
//         public RelayCommand EditProperty
//         {
//             get
//             {
//                 return _editProperty ??= new RelayCommand(o =>
//                 {
//                     ShowChildWindow(new CreatePropertyWindow(SelectedProperty));
//                 }, o => SelectedProperty != null);
//             }
//         }
//
//         private RelayCommand _deleteProperty;
//
//         /// <summary>
//         ///     Команда, удаляющая свойство
//         /// </summary>
//         public RelayCommand DeleteProperty
//         {
//             get
//             {
//                 return _deleteProperty ??= new RelayCommand(o =>
//                 {
//                     //if (MessageBox.Show($"Вы действительно хотите удалить пользователя {SelectedUser.UserName}?", "Удаление пользователя", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
//                     //{
//                     //    db.Properties.Remove(SelectedProperty);
//                     //    db.SaveChanges();
//                     //}
//                 });
//             }
//         }
//
//     #endregion
//     }
// }
