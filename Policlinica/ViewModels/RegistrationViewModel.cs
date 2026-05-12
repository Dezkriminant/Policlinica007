using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;
using Policlinica.Views;

namespace Policlinica.ViewModels;

public partial class RegistrationViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Navigation _navigation;
    private readonly IServiceProvider _provider;

    [ObservableProperty] private string _password;
    [ObservableProperty] private string _login;
    [ObservableProperty] public string _eror;

    public RegistrationViewModel(IServiceProvider serviceProvider, Navigation navigation, IServiceProvider provider)
    {
        _serviceProvider = serviceProvider;
        _navigation = navigation;
        _provider = provider;

    }

    [RelayCommand]
    void OpenAutorization()
    {
        var vm = _serviceProvider.GetRequiredService<AutorizationViewModel>();
        _navigation.Navigate(vm);

    }

    [RelayCommand]
    void Registration()
    {

        using (UserRepository repository = _provider.GetRequiredService<UserRepository>())
        {
            var count = repository.CheckLogin(Login).Count;
            if (count > 0)
            {
                Eror = "Такой логин уже существует";
                return;
            }
            var user = new User()
            {
                Name = Login,
                Password = Password,
            };

            using (var rep = _serviceProvider.GetRequiredService<UserRepository>())
            {
                rep.AddUser(user);
            }
        
            //  var vm =  ActivatorUtilities.CreateInstance<RecordViewModel>(_serviceProvider);
            // var win = _serviceProvider.GetRequiredService<Records>();
            //win.DataContext = vm;
            //win.Show(); 
            //_navigation.Close();
            var vm = _serviceProvider.GetRequiredService<AutorizationViewModel>();
            _navigation.Navigate(vm);
        }
    }

    

}