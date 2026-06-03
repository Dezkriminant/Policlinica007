using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;
using Policlinica.Views;

namespace Policlinica.ViewModels;

public partial class PatientManagementViewModel : ViewModelBase
{
    private readonly PatientRepository _patientRepository;
    private readonly IServiceProvider _provider;
    private readonly Navigation _navigation;

    [ObservableProperty] ObservableCollection<Patient> patientList = new();
    [ObservableProperty] Patient selectedPatient;
    [ObservableProperty] string searchText = "";
    [ObservableProperty] string statusMessage = "";

    public PatientManagementViewModel(PatientRepository patientRepository, IServiceProvider provider, Navigation navigation)
    {
        _patientRepository = patientRepository;
        _provider = provider;
        _navigation = navigation;
        
        LoadPatients();
    }

    private void LoadPatients()
    {
        var patients = _patientRepository.GetAllPatients();
        PatientList = new ObservableCollection<Patient>(patients);
    }

    public void RefreshPatients()
    {
        LoadPatients();
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterPatients(value);
    }

    private void FilterPatients(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            LoadPatients();
            return;
        }

        try
        {
            var allPatients = _patientRepository.GetAllPatients();
            var filtered = allPatients
                .Where(p => p.Surname.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                           p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                           p.PhoneNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            PatientList = new ObservableCollection<Patient>(filtered);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка при фильтрации: {ex.Message}";
        }
    }

    [RelayCommand]
    void DeletePatient()
    {
        if (SelectedPatient == null)
        {
            StatusMessage = "Выберите пациента";
            return;
        }

        try
        {
            _patientRepository.DeletePatient(SelectedPatient.Id);
            StatusMessage = $"Пациент {SelectedPatient.Surname} {SelectedPatient.Name} удален";
            SelectedPatient = null;
            LoadPatients();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка при удалении: {ex.Message}";
        }
    }

    [RelayCommand]
    void AddPatient()
    {
        var addPatientViewModel = ActivatorUtilities.CreateInstance<AddPatientViewModel>(_provider);
        var addPatientView = ActivatorUtilities.CreateInstance<AddPatientView>(_provider);

        if (addPatientView != null && addPatientViewModel != null)
        {
            addPatientViewModel.SetParentViewModel(this);
            addPatientView.DataContext = addPatientViewModel;
            
            // Получить главное окно
            Window? mainWindow = null;
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                mainWindow = desktop.MainWindow;
            }
            
            addPatientView.ShowDialog(mainWindow);
        }
    }

    [RelayCommand]
    void Search()
    {
        FilterPatients(SearchText);
    }

    [RelayCommand]
    void BloodSugarMeasurement()
    {
        if (SelectedPatient == null)
        {
            StatusMessage = "Выберите пациента";
            return;
        }

        var vm = ActivatorUtilities.CreateInstance<BloodSugarInputViewModel>(_provider);
        vm.SetSelectedPatient(SelectedPatient);
        _navigation.Navigate(vm);
    }

    [RelayCommand]
    void SugarCheck()
    {
        var vm = ActivatorUtilities.CreateInstance<SugarCheckViewModel>(_provider);
        _navigation.Navigate(vm);
    }

    [RelayCommand]
    void GoBack()
    {
        _navigation.GoBack();
    }
}
