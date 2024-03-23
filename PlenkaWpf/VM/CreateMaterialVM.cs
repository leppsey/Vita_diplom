// using System.Collections.Generic;
// using System.Linq;
//
// using PlenkaAPI.Data;
// using PlenkaAPI.Models;
//
// using PlenkaWpf.Utils;
//
//
// namespace PlenkaWpf.VM
// {
//     internal class CreateMaterialVM : ViewModelBase
//
//     {
//     #region Functions
//
//     #region Constructors
//
//         public CreateMaterialVM()
//         {
//             AllTypes = DbContextSingleton.GetInstance().ObjectTypes.ToList();
//         }
//
//     #endregion
//
//     #endregion
//
//
//     #region Properties
//
//         public MembraneObject MembraneObject { get; set; } = new() {ObName = "",};
//         public List<ObjectType> AllTypes { get; set; }
//
//     #endregion
//
//
//     #region Commands
//
//         private RelayCommand _saveMaterial;
//
//         /// <summary>
//         ///     Команда, сохраняющая объект в базу данных
//         /// </summary>
//         public RelayCommand SaveMaterial
//         {
//             get
//             {
//                 return _saveMaterial ??= new RelayCommand(o =>
//                 {
//                     MembraneObject.TypeId = MembraneObject.Type.TypeId;
//                     var db = DbContextSingleton.GetInstance();
//                     var abc = db.DefaultProperties.Where(dp => dp.TypeId == MembraneObject.TypeId).Select(dp => dp.Prop);
//
//                     foreach (var prop in abc)
//                     {
//                         MembraneObject.Values.Add(new Value
//                                                       {Prop = prop, Mat = MembraneObject,});
//                     }
//
//                     db.MembraneObjects.Add(MembraneObject);
//                     db.SaveChanges();
//                     OnClosingRequest();
//                 }, o => MembraneObject?.ObName.Length > 0 && MembraneObject.Type != null);
//             }
//         }
//
//     #endregion
//     }
// }
