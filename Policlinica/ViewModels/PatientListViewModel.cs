using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;

namespace Policlinica.ViewModels;

public partial class PatientListViewModel : ViewModelBase
{
    private readonly PatientRepository _patientRepository;
    private readonly IServiceProvider _provider;
    private readonly Navigation _navigation;
    private Action<Patient> _onPatientSelected;

    [ObservableProperty] ObservableCollection<Patient> patientList = new();
    [ObservableProperty] Patient selectedPatient;
    [ObservableProperty] string newPatientName = "";
    [ObservableProperty] string newPatientSurname = "";
    [ObservableProperty] string newPatientPhone = "";
    [ObservableProperty] string statusMessage = "";
    [ObservableProperty] string searchText = "";

    public PatientListViewModel(PatientRepository patientRepository, IServiceProvider provider, Navigation navigation)
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

    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            LoadPatients();
            return;
        }

        var allPatients = _patientRepository.GetAllPatients();
        var filtered = allPatients
            .Where(p => p.Name.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                       p.Surname.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                       p.PhoneNumber.Contains(value))
            .ToList();

        PatientList = new ObservableCollection<Patient>(filtered);
    }

    [RelayCommand]
    void SelectPatient()
    {
        if (SelectedPatient == null)
        {
            StatusMessage = "Выберите пациента";
            return;
        }

        _onPatientSelected?.Invoke(SelectedPatient);
    }

    [RelayCommand]
    void AddNewPatient()
    {
        if (string.IsNullOrWhiteSpace(NewPatientName) || string.IsNullOrWhiteSpace(NewPatientSurname))
        {
            StatusMessage = "Введите имя и фамилию";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPatientPhone) || NewPatientPhone.Length > 12)
        {
            StatusMessage = "Введите корректный номер телефона (макс. 12 символов)";
            return;
        }

        try
        {
            var newPatient = _patientRepository.AddPatient(NewPatientName, NewPatientSurname, NewPatientPhone, "", "");
            StatusMessage = "Пациент добавлен успешно";
            
            NewPatientName = "";
            NewPatientSurname = "";
            NewPatientPhone = "";
            
            LoadPatients();
            SelectedPatient = newPatient;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    void Search()
    {
        OnSearchTextChanged(SearchText);
    }

    [RelayCommand]
    void DeletePatient()
    {
        if (SelectedPatient == null)
        {
            StatusMessage = "Выберите пациента для удаления";
            return;
        }

        try
        {
            bool deleted = _patientRepository.DeletePatient(SelectedPatient.Id);
            if (deleted)
            {
                StatusMessage = $"Пациент {SelectedPatient.Surname} удален";
                LoadPatients();
                SelectedPatient = null;
            }
            else
            {
                StatusMessage = "Не удалось удалить пациента";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка при удалении: {ex.Message}";
        }
    }

    [RelayCommand]
    void GoBack()
    {
        _navigation.GoBack();
    }

    public void SetOnPatientSelected(Action<Patient> callback)
    {
        _onPatientSelected = callback;
    }
}
