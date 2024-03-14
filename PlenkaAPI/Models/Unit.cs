using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PropertyChanged;


namespace PlenkaAPI.Models
{
    [AddINotifyPropertyChangedInterface]
    public partial class Unit
    {
        public Unit()
        {
            Properties = new ObservableCollection<Property>();
        }

        public long UnitId { get; set; }
        public string UnitName { get; set; }

        public virtual ObservableCollection<Property> Properties { get; set; }
    }
}
