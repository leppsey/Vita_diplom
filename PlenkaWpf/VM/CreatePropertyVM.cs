using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using PlenkaAPI.Data;
using PlenkaAPI.Models;

using PlenkaWpf.Utils;
using PlenkaWpf.View;


namespace PlenkaWpf.VM
{
    public class CreatePropertyVM : ViewModelBase
    {
    #region Functions

    #region Constructors

        public CreatePropertyVM(Property property)
        {
            EditingProperty = property;

            TempProperty = new Property
            {
                ProperrtyId = property.ProperrtyId,
                PropertyName = property.PropertyName,
                UnitId = property.UnitId,
                Unit = property.Unit,
            };

            Db = DbContextSingleton.GetInstance();
            AllUnits = Db.Units.Local.ToObservableCollection();
        }

    #endregion

    #endregion


    #region Properties

        public MembraneContext Db { get; set; }
        public ObservableCollection<Unit> AllUnits { get; set; }
        public Property TempProperty { get; set; }
        public Property EditingProperty { get; set; }

        public List<Unit> Units
        {
            get
            {
                return DbContextSingleton.GetInstance().Units.ToList();
            }
        }

    #endregion


    #region Commands

        private RelayCommand _saveProperty;

        /// <summary>
        ///     Команда сохраняющая свойство в базу данных
        /// </summary>
        public RelayCommand SaveProperty
        {
            get
            {
                return _saveProperty ?? (_saveProperty = new RelayCommand(o =>
                                            {
                                                EditingProperty.Unit = TempProperty.Unit;
                                                EditingProperty.UnitId = TempProperty.UnitId;
                                                EditingProperty.PropertyName = TempProperty.PropertyName;

                                                if (!Db.Properties.Contains(EditingProperty))
                                                {
                                                    Db.Properties.Add(EditingProperty);
                                                }

                                                Db.SaveChanges();
                                                OnClosingRequest();
                                            }));
            }
        }

        private RelayCommand _addMeasureUnit;

        public RelayCommand AddMeasureUnit
        {
            get
            {
                return _addMeasureUnit ??= new RelayCommand(o =>
                {
                    ShowChildWindow(new UnitCreateWindow());
                });
            }
        }

    #endregion
    }
}
