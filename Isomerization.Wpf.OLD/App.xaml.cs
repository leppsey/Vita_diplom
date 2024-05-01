using System.Windows;
using Isomerization.Domain.Data;


namespace Isomerization.Wpf.OLD
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DbContextSingleton.GetInstance().Database.EnsureCreated();
        }
    }
}
