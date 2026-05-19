using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Animation.Easings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;
using Policlinica.Views;

namespace Policlinica.ViewModels;

public partial class ServiceViewModel : ViewModelBase
{

    private readonly IServiceProvider _provider;
    private readonly Navigation _navigation;
    ServiceRepository _serviceRepository;
    
    [ObservableProperty] string surname;
    [ObservableProperty] string name;
    [ObservableProperty] ObservableCollection<ServiceSelected> _services;
    [ObservableProperty] string _login;
    [ObservableProperty] Doctor _selectedDoctor;

    public ServiceViewModel(IServiceProvider provider, Navigation navigation, Doctor selectedDoctor,
        ServiceRepository repository, string clientName = "", string clientSurname = "")
    {
        _provider = provider;
        _navigation = navigation;
        _selectedDoctor = selectedDoctor;
        _serviceRepository = repository;
        name = clientName;
        surname = clientSurname;
        Services =  new ObservableCollection<ServiceSelected>(repository.GetServicesByDoctors(selectedDoctor.Id).Select(service => new ServiceSelected(service)).ToList());
        
        //Console.WriteLine(CurrentUser.login);
    }


    [RelayCommand]
    public void Dobavlenie()
    {
        List<Service> selectedServices = new List<Service>();

        foreach (ServiceSelected s in Services)
        {
            if (s.IsSelected == true)
            {
                selectedServices.Add(s.Service);
            }
        }

        if (selectedServices.Count == 0)
        {
            // Можно добавить уведомление об ошибке
            return;
        }

        // Получаем репозитории для передачи в ViewModel
        var recordRepository = _provider.GetRequiredService<RecordRep>();
        var recordItemsRepository = _provider.GetRequiredService<RecordItemsRepository>();

        // Создаем ViewModel для показа подтверждения записи
        var vm = ActivatorUtilities.CreateInstance<RecordItemsViewModel>(_provider, 
            _navigation, _selectedDoctor, selectedServices, recordRepository, recordItemsRepository, Name, Surname);
        
        _navigation.Navigate(vm);
    }
}
