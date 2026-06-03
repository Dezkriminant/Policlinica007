using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;

namespace Policlinica.ViewModels;

public partial class StatisticsViewModel : ViewModelBase
{
    private readonly RecordRep _recordRep;
    private readonly Navigation _navigation;

    [ObservableProperty] ObservableCollection<HospitalStatistic> hospitalStatistics = new();
    [ObservableProperty] int totalRecords = 0;
    [ObservableProperty] int totalRevenue = 0;

    public StatisticsViewModel(RecordRep recordRep, Navigation navigation)
    {
        _recordRep = recordRep;
        _navigation = navigation;
        
        LoadStatistics();
    }

    private void LoadStatistics()
    {
        var stats = _recordRep.GetHospitalStatistics();
        HospitalStatistics = new ObservableCollection<HospitalStatistic>(stats);
        
        TotalRecords = 0;
        TotalRevenue = 0;
        
        foreach (var stat in stats)
        {
            TotalRecords += stat.RecordCount;
            TotalRevenue += stat.TotalRevenue;
        }
    }

    [RelayCommand]
    void GoBack()
    {
        _navigation.GoBack();
    }
}
