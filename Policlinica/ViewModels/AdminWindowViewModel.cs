using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;
using Policlinica.Views;


namespace Policlinica.ViewModels;

public partial class AdminWindowViewModel : ViewModelBase
{
    private readonly Navigation _navigation;
    private readonly IServiceProvider _provider;
    private readonly RecordRep _recordRep;
    [ObservableProperty] private string _password;
    [ObservableProperty] private string _login;
    [ObservableProperty] public string _eror;
    [ObservableProperty] ObservableCollection<Record>  _recordsList = new();
    [ObservableProperty] private Record _selectedRecord;

    public AdminWindowViewModel( Navigation navigation, IServiceProvider provider, RecordRep recordRep)
    {
        
        _navigation = navigation;
        _provider = provider;
        _recordRep = recordRep;
        
        RecordsList = new ObservableCollection<Record>(recordRep.GetRecord());
    }

    [RelayCommand]
    void DeleteRecord()
    {
        _recordRep.Delete(SelectedRecord.Id);
        RecordsList = new ObservableCollection<Record>(_recordRep.GetRecord());
    }

    [RelayCommand]
    void GoService()
    {
        var vm = _provider.GetRequiredService<ServiceViewModel>();
        _navigation.Navigate(vm);
    }
}