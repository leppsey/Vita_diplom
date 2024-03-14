using System.Collections.ObjectModel;

using PropertyChanged;


namespace PlenkaAPI.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Property
    {
        public Property()
        {
            DefaultProperties = new ObservableCollection<DefaultProperty>();
            Values = new ObservableCollection<Value>();
        }

        public long ProperrtyId { get; set; }
        public string PropertyName { get; set; }
        public long UnitId { get; set; }

        public virtual Unit Unit { get; set; }
        public virtual ObservableCollection<DefaultProperty> DefaultProperties { get; set; }
        public virtual ObservableCollection<Value> Values { get; set; }
    }
}
