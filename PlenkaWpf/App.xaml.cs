using System.Windows;
using PlenkaAPI.Data;


namespace PlenkaWpf
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
