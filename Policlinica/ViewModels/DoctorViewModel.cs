using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;

namespace Policlinica.ViewModels;

public partial class DoctorViewModel : ViewModelBase
{
    private readonly IServiceProvider _provider;
    private readonly Navigation _navigation;
    private readonly Hospital _hospital;
    private readonly DoctorRepository _doctorRepository;
    private Patient _selectedPatient;
    
    [ObservableProperty] string _surname;
    [ObservableProperty] string _name;
    [ObservableProperty] ObservableCollection<Doctor> _doctorList;
    [ObservableProperty] Doctor _selectedDoctor;
    [ObservableProperty] string statusMessage = "";

    public DoctorViewModel(IServiceProvider provider, DoctorRepository doctorRepository, Navigation navigation)
    {
        _provider = provider;
        _navigation = navigation;
        _doctorRepository = doctorRepository;
        _doctorList = new ObservableCollection<Doctor>(doctorRepository.GetDoctorsByTest());
    }

    public DoctorViewModel(IServiceProvider provider, Hospital hospital)
    {
        _provider = provider;
        _navigation = provider.GetRequiredService<Navigation>();
        _doctorRepository = provider.GetRequiredService<DoctorRepository>();
        _hospital = hospital;
        
        var doctors = _doctorRepository.GetDoctorsByHospital(hospital.Id);
        _doctorList = new ObservableCollection<Doctor>(doctors);
    }

    public void SetSelectedPatient(Patient patient)
    {
        _selectedPatient = patient;
        if (patient != null)
        {
            _name = patient.Name;
            _surname = patient.Surname;
            Console.WriteLine($"[DoctorViewModel] Пациент установлен: {patient.Surname} {patient.Name} (ID: {patient.Id})");
        }
    }

    [RelayCommand]
    public void StartTest()
    {
        if (Name == null || Name.Trim() == "")
        {
            StatusMessage = "Введите имя клиента";
            return;
        }
        if (Surname == null || Surname.Trim() == "")
        {
            StatusMessage = "Введите фамилию клиента";
            return;
        }
        if (SelectedDoctor == null)
        {
            StatusMessage = "Выберите врача";
            return;
        }
        
        var serviceRepository = _provider.GetRequiredService<ServiceRepository>();
        var vm = ActivatorUtilities.CreateInstance<ServiceViewModel>(_provider, _navigation, SelectedDoctor, 
            _hospital, serviceRepository, Name, Surname);
        
        // Передать пациента если он есть
        if (_selectedPatient != null)
        {
            vm.ResetServices();
            vm.SetPatient(_selectedPatient);
            Console.WriteLine($"[DoctorViewModel] ServiceViewModel получит телефон: {_selectedPatient.PhoneNumber}");
        }
        
        _navigation.Navigate(vm);
    }

    [RelayCommand]
    public void GoBack()
    {
        _navigation.GoBack();
    }
}
