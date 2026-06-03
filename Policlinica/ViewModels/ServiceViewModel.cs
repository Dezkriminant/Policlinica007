using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;

namespace Policlinica.ViewModels;

public partial class ServiceViewModel : ViewModelBase
{
    private readonly IServiceProvider _provider;
    private readonly Navigation _navigation;
    private readonly ServiceRepository _serviceRepository;
    private readonly Doctor _selectedDoctor;
    private readonly Hospital _selectedHospital;
    private readonly string _clientName;
    private readonly string _clientSurname;
    private int _patientId;
    private Patient _selectedPatient;

    [ObservableProperty] string patientPhone = "";
    [ObservableProperty] ObservableCollection<ServiceSelected> services;
    [ObservableProperty] string statusMessage = "";

    public ServiceViewModel(IServiceProvider provider, Navigation navigation, Doctor selectedDoctor,
        ServiceRepository repository, string clientName = "", string clientSurname = "")
    {
        _provider = provider;
        _navigation = navigation;
        _selectedDoctor = selectedDoctor;
        _serviceRepository = repository;
        _clientName = clientName;
        _clientSurname = clientSurname;
        _patientId = 0;
        PatientPhone = "";
        
        Services = new ObservableCollection<ServiceSelected>(
            repository.GetServicesByDoctors(selectedDoctor.Id).Select(service => new ServiceSelected(service)).ToList());
    }

    public ServiceViewModel(IServiceProvider provider, Navigation navigation, Doctor selectedDoctor,
        Hospital selectedHospital, ServiceRepository repository, string clientName = "", string clientSurname = "")
    {
        _provider = provider;
        _navigation = navigation;
        _selectedDoctor = selectedDoctor;
        _selectedHospital = selectedHospital;
        _serviceRepository = repository;
        _clientName = clientName;
        _clientSurname = clientSurname;
        _patientId = 0;
        PatientPhone = "";
        
        // Очистить чекбоксы услуг
        var servicesList = repository.GetServicesByDoctors(selectedDoctor.Id).Select(service => new ServiceSelected(service)).ToList();
        Services = new ObservableCollection<ServiceSelected>(servicesList);
    }

    public void SetPatient(Patient patient)
    {
        _selectedPatient = patient;
        _patientId = patient.Id;
        PatientPhone = patient.PhoneNumber;
        Console.WriteLine($"[ServiceViewModel] Установлен пациент: {patient.Surname} {patient.Name}, телефон: {PatientPhone}");
    }

    public void ResetServices()
    {
        // Очистить все выбранные услуги
        foreach (var service in Services)
        {
            service.IsSelected = false;
        }
        StatusMessage = "";
    }

    [RelayCommand]
    public void ContinueToDateTime()
    {
        var selectedServices = Services
            .Where(s => s.IsSelected)
            .Select(s => s.Service)
            .ToList();

        if (selectedServices.Count == 0)
        {
            StatusMessage = "Выберите хотя бы одну услугу";
            return;
        }

        if (_selectedHospital == null)
        {
            StatusMessage = "Ошибка: больница не выбрана";
            return;
        }

        var vm = ActivatorUtilities.CreateInstance<DateTimeViewModel>(_provider, 
            _selectedDoctor, _selectedHospital, selectedServices, _clientName, _clientSurname, PatientPhone);
        
        _navigation.Navigate(vm);
    }

    [RelayCommand]
    public void GoBack()
    {
        _navigation.GoBack();
    }
}
