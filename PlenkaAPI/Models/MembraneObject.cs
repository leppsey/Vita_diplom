using System.Collections.ObjectModel;

using PropertyChanged;


namespace PlenkaAPI.Models
{
    [AddINotifyPropertyChangedInterface]
    public partial class MembraneObject
    {
        public MembraneObject()
        {
            Values = new ObservableCollection<Value>();
        }

        public long ObId { get; set; }
        public string ObName { get; set; }
        public long TypeId { get; set; }

        public virtual ObjectType Type { get; set; }
        public virtual ObservableCollection<Value> Values { get; set; }
    }
}
