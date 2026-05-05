using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;
using Policlinica.Views;

namespace Policlinica.ViewModels;

public partial class AutorizationViewModel : ViewModelBase
{
    private readonly IServiceProvider _provider;
    private readonly Navigation _navigation;

    [ObservableProperty] string _login;
    [ObservableProperty] string _password;
    [ObservableProperty] List<User> _usersList;
    [ObservableProperty] UserRepository _repository;

    public AutorizationViewModel(IServiceProvider provider, UserRepository repository,Navigation navigation )
    {
        _provider = provider;
        _navigation = navigation;

        _usersList = repository.GetUsersByTest();
       // _repository = repository;
    }

    [RelayCommand]
    public void StartTest()
    {
        
        var vm = ActivatorUtilities.CreateInstance<AdminWindowViewModel>(
            _provider,
            Login);
        var win = _provider.GetRequiredService<AdminWindow>();
            //vm.SetClose(win.Close);
        win.DataContext = vm;
        win.Show();
        // close();

    }
    [RelayCommand]
    public void SaveDB()
    {
         _repository.CheckLoginAndPassword(Login,Password);
        var vm = _provider.GetRequiredService<AdminWindowViewModel>();
        var win = _provider.GetRequiredService<AdminWindow>();
        
        //vm.SetClose(win.Close);
        win.DataContext = vm;
        win.Show();
        //close();
    }

    [RelayCommand]
    void OpenRegWin()
    {
        var vm = _provider.GetRequiredService<RegistrationViewModel>();
        _navigation.Navigate(vm);
        
    }
}