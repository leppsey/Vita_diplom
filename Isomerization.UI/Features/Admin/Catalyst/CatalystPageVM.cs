
using System.Collections.ObjectModel;

namespace Isomerization.UI.Features.Admin;

public class CatalystPageVM: ViewModelBase
{
    public CatalystPageVM()
    {

        Catalysts = new ObservableCollection<Domain.Models.Catalyst>()
        {
            new()
            {
                Type = "Гетерогенный",
                Name = "Pt/Al2O3 (Платина на алюмосиликате)",
                Activity = 80,
                LoadingRate = 0.5,
                // OperatingTime = 1234,
                ServiceLife = 5,
                TemperatureReaction = 220,
                DateOfCommissioning = new DateTime(year:2022, day: 22, month:06),
                DateOfDecommissioning = new DateTime(year:2027, day: 22, month:06)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "Pt/Zeolite (Платина на цеолите)",
                Activity = 90,
                LoadingRate = 0.4,
                // OperatingTime = 1234,
                ServiceLife = 5,
                TemperatureReaction = 150,
                DateOfCommissioning = new DateTime(year:2021, day: 07, month:06),
                DateOfDecommissioning = new DateTime(year:2026, day: 07, month:06)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "Pd/C (Палладий на углероде)",
                Activity = 80,
                LoadingRate = 0.3,
                // OperatingTime = 1234,
                ServiceLife = 6,
                TemperatureReaction = 150,
                DateOfCommissioning = new DateTime(year:2023, day: 30, month:05),
                DateOfDecommissioning = new DateTime(year:2029, day: 30, month:05)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "mordenite-based (На основе морденита)",
                Activity = 80,
                LoadingRate = 0.45,
                // OperatingTime = 1234,
                ServiceLife = 5,
                TemperatureReaction = 180,
                DateOfCommissioning = new DateTime(year:2020, day: 01, month:02),
                DateOfDecommissioning = new DateTime(year:2025, day: 01, month:02)
            },
            new()
            {
                Type = "Гетерогенный",
                Name = "ZSM-5",
                Activity = 90,
                LoadingRate = 0.4,
                // OperatingTime = 1234,
                ServiceLife = 10,
                TemperatureReaction = 300,
                DateOfCommissioning = new DateTime(year:2021, day: 28, month:02),
                DateOfDecommissioning = new DateTime(year:2031, day: 28, month:02)
            },
            new()
            {
                Type = "Гомогенный",
                Name = "Катализатор на основе комплексов рутения",
                Activity = 90,
                LoadingRate = 0.25,
                // OperatingTime = 1234,
                ServiceLife = 3,
                TemperatureReaction = 150,
                DateOfCommissioning = new DateTime(year:2022, day: 30, month:08),
                DateOfDecommissioning = new DateTime(year:2025, day: 30, month:08)
            },
        };
    }
    public ObservableCollection<Domain.Models.Catalyst> Catalysts { get; set; }


    private RelayCommand _editCatalystCommand;
    public RelayCommand EditCatalystCommand => _editCatalystCommand ??= new RelayCommand(_ =>
    {
    });
    
    private RelayCommand _addCatalystCommand;
    public RelayCommand AddCatalystCommand => _addCatalystCommand ??= new RelayCommand(_ =>
    {
    });
    
    private RelayCommand _deleteCatalystCommand;
    public RelayCommand DeleteCatalystCommand => _deleteCatalystCommand ??= new RelayCommand(_ =>
    {
    });
}