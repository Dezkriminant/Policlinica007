using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;
using Policlinica.Views;

namespace Policlinica.ViewModels;

public partial class RegistrationViewModel:ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Navigation _navigation;
    
    [ObservableProperty] private string _password;
    [ObservableProperty] private string _login;

    public RegistrationViewModel(IServiceProvider serviceProvider, Navigation navigation)
    {
        _serviceProvider = serviceProvider;
        _navigation = navigation;
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
            var user = new User()
            {
                Name = Login,
                Password = Password,
                
            };

            using (var rep = _serviceProvider.GetRequiredService<UserRepository>())
            {
                rep.AddUser(user);
            }
        
            var vm =  ActivatorUtilities.CreateInstance<RecordViewModel>(_serviceProvider);
            var win = _serviceProvider.GetRequiredService<Records>();
          win.DataContext = vm;
          win.Show(); 
            _navigation.Close();
    }
    

}