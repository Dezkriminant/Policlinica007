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

    [ObservableProperty] string phoneNumber = "";
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
        
        Services = new ObservableCollection<ServiceSelected>(
            repository.GetServicesByDoctors(selectedDoctor.Id).Select(service => new ServiceSelected(service)).ToList());
    }

    [RelayCommand]
    public void ContinueToDateTime()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            StatusMessage = "Введите номер телефона";
            return;
        }

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
            _selectedDoctor, _selectedHospital, selectedServices, _clientName, _clientSurname, PhoneNumber);
        
        _navigation.Navigate(vm);
    }

    [RelayCommand]
    public void GoBack()
    {
        // Если выбрана больница, вернуться в DoctorView для этой больницы
        if (_selectedHospital != null)
        {
            var vm = ActivatorUtilities.CreateInstance<DoctorViewModel>(_provider, _selectedHospital);
            _navigation.Navigate(vm);
        }
        else
        {
            // Иначе вернуться в HospitalView
            var vm = ActivatorUtilities.CreateInstance<HospitalViewModel>(_provider);
            _navigation.Navigate(vm);
        }
    }
}
