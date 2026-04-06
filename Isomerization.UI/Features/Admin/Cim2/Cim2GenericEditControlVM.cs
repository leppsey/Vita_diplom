using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using Isomerization.Shared;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Admin.Cim2;

public class Cim2GenericEditControlVM : ViewModelBase, IDialogEditViewModel<object>
{
    private object _data = null!;

    public object Data
    {
        get => _data;
        set
        {
            _data = value;
            BuildFields();
            OnPropertyChanged();
            OnPropertyChanged(nameof(Title));
        }
    }

    public object? Result { get; private set; }
    public Action FinishInteraction { get; set; } = () => { };
    public string Title => Data?.GetType().Name ?? "Запись";
    public ObservableCollection<Cim2EditField> Fields { get; } = new();

    private RelayCommand _applyCommand;
    public RelayCommand ApplyCommand => _applyCommand ??= new RelayCommand(_ =>
    {
        if (Data == null) return;

        foreach (var field in Fields)
        {
            var targetType = Nullable.GetUnderlyingType(field.PropertyType) ?? field.PropertyType;
            var parsed = Parse(field.Value, targetType);
            if (parsed.IsSuccess)
            {
                field.Property.SetValue(Data, parsed.Value);
            }
        }

        Result = Data;
        FinishInteraction();
    });

    private RelayCommand _cancelCommand;
    public RelayCommand CancelCommand => _cancelCommand ??= new RelayCommand(_ =>
    {
        Result = null;
        FinishInteraction();
    });

    private void BuildFields()
    {
        Fields.Clear();
        if (Data == null) return;

        var props = Data.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead && p.CanWrite && IsEditableType(Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType));

        foreach (var prop in props)
        {
            var value = prop.GetValue(Data);
            Fields.Add(new Cim2EditField
            {
                Name = prop.Name,
                Value = value?.ToString() ?? string.Empty,
                Property = prop,
                PropertyType = prop.PropertyType
            });
        }
    }

    private static bool IsEditableType(Type type) =>
        type == typeof(string) ||
        type == typeof(int) ||
        type == typeof(double) ||
        type == typeof(decimal) ||
        type == typeof(float) ||
        type == typeof(bool);

    private static (bool IsSuccess, object? Value) Parse(string raw, Type targetType)
    {
        if (targetType == typeof(string)) return (true, raw);
        if (targetType == typeof(int))
            return int.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ||
                   int.TryParse(raw, out i)
                ? (true, i)
                : (false, null);
        if (targetType == typeof(double))
            return double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ||
                   double.TryParse(raw, out d)
                ? (true, d)
                : (false, null);
        if (targetType == typeof(decimal))
            return decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var m) ||
                   decimal.TryParse(raw, out m)
                ? (true, m)
                : (false, null);
        if (targetType == typeof(float))
            return float.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var f) ||
                   float.TryParse(raw, out f)
                ? (true, f)
                : (false, null);
        if (targetType == typeof(bool))
        {
            if (bool.TryParse(raw, out var b)) return (true, b);
            if (raw == "1") return (true, true);
            if (raw == "0") return (true, false);
            return (false, null);
        }

        return (false, null);
    }
}

public class Cim2EditField
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public PropertyInfo Property { get; set; } = null!;
    public Type PropertyType { get; set; } = typeof(string);
}
