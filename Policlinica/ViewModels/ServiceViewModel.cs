using System;
using System.Collections.Generic;
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
    [ObservableProperty] List<ServiceSelected> _services;
    ServiceRepository _serviceRepository;
    [ObservableProperty] string _login;
    [ObservableProperty] Doctor _selectedDoctor;
    
    public ServiceViewModel(IServiceProvider provider, Navigation navigation,Doctor selectedDoctor, ServiceRepository repository)
    {
        _provider = provider;
        _navigation = navigation;
        _selectedDoctor = selectedDoctor;
        _serviceRepository = repository;
        //Services = repository.GetServicesByDoctors(selectedDoctor.Id).Select(service => new ServiceSelected(service)).ToList();
    }


    [RelayCommand]

    public void Dobavlenie()
    {
        {
            List<Service> services = new List<Service>();

            foreach (ServiceSelected s in Services)
            {
                if (s.IsSelected == true)
                {
                    services.Add(s.Service);
                }
            }

            var vm = ActivatorUtilities.CreateInstance<ServiceViewModel>(
                _provider,
                SelectedDoctor,
                Login,
                Services);
            var win = _provider.GetRequiredService<ServiceWindow>();
          //  vm.SetClose(win.Close);
            win.DataContext = vm;
           // win.Show();
           // close();
        }

    }
}