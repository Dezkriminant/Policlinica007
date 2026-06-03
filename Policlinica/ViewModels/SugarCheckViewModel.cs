using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.Views;

namespace Policlinica.ViewModels;

public partial class SugarCheckViewModel : ViewModelBase
{
    private readonly Navigation _navigation;
    private readonly IServiceProvider _provider;
    private SugarCheckView _view;

    [ObservableProperty] string result = "";
    [ObservableProperty] string statusMessage = "";

    public SugarCheckViewModel(Navigation navigation, IServiceProvider provider)
    {
        _navigation = navigation;
        _provider = provider;
    }

    public void SetView(SugarCheckView view)
    {
        _view = view;
    }

    [RelayCommand]
    public void Calculate()
    {
        if (_view == null)
        {
            StatusMessage = "Ошибка: вид не инициализирован";
            return;
        }
        
        var weightInput = _view.FindControl<TextBox>("WeightInput");
        var heightInput = _view.FindControl<TextBox>("HeightInput");

        if (weightInput?.Text == null || heightInput?.Text == null)
        {
            StatusMessage = "Введите значения веса и роста";
            return;
        }

        if (!decimal.TryParse(weightInput.Text, out decimal weight) || !decimal.TryParse(heightInput.Text, out decimal height))
        {
            StatusMessage = "Введите корректные числовые значения";
            return;
        }

        if (weight <= 0 || height <= 0)
        {
            StatusMessage = "Вес и рост должны быть больше нуля";
            Result = "";
            return;
        }

        try
        {
            decimal heightInMeters = height / 100;
            decimal bmi = weight / (heightInMeters * heightInMeters);

            string recommendation;

            if (bmi < 18.5m)
            {
                recommendation = "Статус: Недостаточный вес\nРекомендуемый уровень сахара: 3.9 - 5.8 ммоль/л";
            }
            else if (bmi < 25m)
            {
                recommendation = "Статус: Нормальный вес\nРекомендуемый уровень сахара: 3.9 - 5.8 ммоль/л";
            }
            else if (bmi < 30m)
            {
                recommendation = "Статус: Избыточный вес\nРекомендуемый уровень сахара: 3.9 - 6.1 ммоль/л\nРекомендуется контролировать уровень глюкозы";
            }
            else
            {
                recommendation = "Статус: Ожирение\nРекомендуемый уровень сахара: 3.9 - 7.0 ммоль/л\nОбратитесь к врачу";
            }

            Result = $"BMI: {bmi:F1}\n{recommendation}";
            StatusMessage = "";
            Console.WriteLine($"Calculated - Weight: {weight}, Height: {height}, BMI: {bmi:F1}");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
            Result = "";
            Console.WriteLine($"Error: {ex}");
        }
    }

    [RelayCommand]
    public void Back()
    {
        _navigation.GoBack();
    }
}
