using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;

namespace Policlinica.ViewModels;

public partial class HospitalViewModel : ViewModelBase
{
    private readonly IServiceProvider _provider;
    private readonly Navigation _navigation;
    private readonly HospitalRepository _hospitalRepository;
    private Patient _selectedPatient;

    [ObservableProperty] ObservableCollection<Hospital> hospitalList = new();
    [ObservableProperty] Hospital selectedHospital;
    [ObservableProperty] string statusMessage = "";

    public HospitalViewModel(IServiceProvider provider, Navigation navigation, HospitalRepository hospitalRepository)
    {
        _provider = provider;
        _navigation = navigation;
        _hospitalRepository = hospitalRepository;

        LoadHospitals();
    }

    private void LoadHospitals()
    {
        var hospitals = _hospitalRepository.GetAllHospitals();
        HospitalList = new ObservableCollection<Hospital>(hospitals);
    }

    public void SetSelectedPatient(Patient patient)
    {
        _selectedPatient = patient;
    }

    [RelayCommand]
    void SelectHospital()
    {
        if (SelectedHospital == null)
        {
            StatusMessage = "Выберите больницу";
            return;
        }

        var vm = ActivatorUtilities.CreateInstance<DoctorViewModel>(_provider, SelectedHospital);
        if (_selectedPatient != null)
        {
            vm.SetSelectedPatient(_selectedPatient);
        }
        _navigation.Navigate(vm);
    }

    [RelayCommand]
    void GoBack()
    {
        _navigation.GoBack();
    }
}
