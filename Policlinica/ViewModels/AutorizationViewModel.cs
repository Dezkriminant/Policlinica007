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
    private readonly User _user;

    [ObservableProperty] string _login;
    [ObservableProperty] string _password;
    [ObservableProperty] public string _eror;

    public AutorizationViewModel(IServiceProvider provider, Navigation navigation,User user)
    {
        _provider = provider;
        _navigation = navigation;
        _user = user;
    }

    [RelayCommand]
    public void Conti()
    {
        List<User> SpUser = new();

        using (UserRepository repository = _provider.GetRequiredService<UserRepository>())
        {
            SpUser = repository.CheckLoginAndPassword(Login, Password);
        }

        if (SpUser.Count == 0)
        {
            Eror = "Неверный логин или пароль";
            return;
        }

        // Копируем все данные авторизованного пользователя в синглтон
        _user.Id = SpUser[0].Id;
        _user.Name = SpUser[0].Name;
        _user.Surname = SpUser[0].Surname;
        _user.Login = SpUser[0].Login;
        _user.Password = SpUser[0].Password;

        CurrentUser.login = SpUser[0].Name;
        
        var vm = ActivatorUtilities.CreateInstance<AdminViewModel>(_provider);
        _navigation.Navigate(vm);
    }


    [RelayCommand]
    void OpenRegWin()
    {
        var vm = _provider.GetRequiredService<RegistrationViewModel>();
        _navigation.Navigate(vm);
    }

}
