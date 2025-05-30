using System.Diagnostics;
using System.IO;
using System.Windows;
using Isomerization.Shared;
using Isomerization.UI.Services;
using Microsoft.Win32;

namespace Isomerization.UI;

public class MainWindowVM: ViewModelBase
{
    private readonly IMenuService _menuService;
    private IMessageBoxService _messageBoxService;

    public MainWindowVM(IMenuService menuService)
    {
        _menuService = menuService;
    }
    private RelayCommand _goHomeMenu;
    public RelayCommand GoHomeMenu => _goHomeMenu ??= new RelayCommand(_ =>
    {
        _menuService.GoHome();
    });
    private RelayCommand _goLoginMenu;
    public RelayCommand GoLoginMenu => _goLoginMenu ??= new RelayCommand(_ =>
    {
        _menuService.GoLogin();
    });
    private RelayCommand _saveDb;
    public RelayCommand SaveDb => _saveDb ??= new RelayCommand(_ =>
    {
        _messageBoxService ??= App.GetService<IMessageBoxService>();
        var sf = new SaveFileDialog();
        sf.Filter = "Файлы базы данных(*.db)|*.db|All files(*.*)|*.*";
        sf.FileName = $"DB_Backup_{DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss")}";

        if ((bool) sf.ShowDialog())
        {
            File.Copy($"{Environment.CurrentDirectory}/Membrane.db", sf.FileName, true);
            _messageBoxService.Show($@"Копия базы данных сохранена по пути {sf.FileName}", "Информация", MessageBoxButton.OK);
        }
        else
        {
            _messageBoxService.Show("Не удалось сохранить базу данных", "Ошибка", MessageBoxButton.OK);
        }
    });
    
    private RelayCommand _loadDb;
    public RelayCommand LoadDb => _loadDb ??= new RelayCommand(_ =>
    {
        _messageBoxService ??= App.GetService<IMessageBoxService>();
        var of = new OpenFileDialog();
        of.Filter = "Файлы базы данных(*.db)|*.db|All files(*.*)|*.*";

        if ((bool) of.ShowDialog())
        {
            File.Copy(of.FileName, $"{Environment.CurrentDirectory}/Membrane.db", true);
            _messageBoxService.Show(@"Копия базы данных восстановлена, требуется приложение перезапустится после закрытия этого окна", "Информация", MessageBoxButton.OK);
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }
        else
        {
            _messageBoxService.Show("Не удалось загрузить базу данных", "Ошибка", MessageBoxButton.OK);
        }
    });
    private RelayCommand _openHelpCommand;
    public RelayCommand OpenHelpCommand => _openHelpCommand ??= new RelayCommand(_ =>
    {
        const string url = "https://vitalina-opal.vercel.app/";
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch
        {
            MessageBox.Show("Ошибка при открытии браузера", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    });
    public bool IsMenuEnabled { get; set; }
}