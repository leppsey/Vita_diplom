using System.Collections.ObjectModel;

using PropertyChanged;


namespace PlenkaAPI.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ObjectType
    {
        public ObjectType()
        {
            DefaultProperties = new ObservableCollection<DefaultProperty>();
            MembraneObjects = new ObservableCollection<MembraneObject>();
        }

        public long TypeId { get; set; }
        public string TypeName { get; set; }

        public virtual ObservableCollection<DefaultProperty> DefaultProperties { get; set; }
        public virtual ObservableCollection<MembraneObject> MembraneObjects { get; set; }
    }
}
