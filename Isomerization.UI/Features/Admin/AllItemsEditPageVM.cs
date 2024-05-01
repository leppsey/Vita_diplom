using System.Collections.ObjectModel;
using Isomerization.UI.Features.Admin.DIMIsomerization;
using Isomerization.UI.Features.Admin.Installation;
using Isomerization.UI.Features.Admin.Kinetic;
using Isomerization.UI.Features.Admin.RawMaterial;
using Isomerization.UI.Shared;

namespace Isomerization.UI.Features.Admin;

public class AllItemsEditPageVM: ViewModelBase
{
    public ObservableCollection<NavigationCard> NavigationCards { get; set; } =
        new()
        {
            new()
            {
                Name = "Катализаторы",
                Link = nameof(CatalystPage)
            },
            new()
            {
                Name = "Цифровые информационные модели изомеризации",
                Link = nameof(DIMIsomerizationPage)
            },
            new()
            {
                Name = "Установки",
                Link = nameof(InstallationPage)
            },
            new()
            {
                Name = "Кинетические характеристики химических процессов",
                Link = nameof(KineticPage)
            },
            new()
            {
                Name = "Сырье",
                Link = nameof(RawMaterialPage)
            },
        };
}