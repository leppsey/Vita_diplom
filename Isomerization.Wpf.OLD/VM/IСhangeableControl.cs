using System.Windows;
using System.Windows.Controls;


namespace Isomerization.Wpf.OLD.VM
{
    public interface IСhangeableControl
    {
        public delegate void ChangingRequestHandler(object sender, UserControl newControl);

        public WindowState PreferedWindowState { get; set; }
        public string WindowTitle { get; set; }
        public double? PreferedHeight { get; set; }
        public double? PreferedWidth { get; set; }
        public event ChangingRequestHandler ChangingRequest;
        public void OnChangingRequest(UserControl newControl);
    }
}
