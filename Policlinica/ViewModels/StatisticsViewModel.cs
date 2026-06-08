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
    private readonly UserRepository _userRepository;
    private readonly User _user;
    private int _userId;

    [ObservableProperty] ObservableCollection<HospitalStatistic> hospitalStatistics = new();
    [ObservableProperty] int totalRecords = 0;
    [ObservableProperty] int totalRevenue = 0;
    [ObservableProperty] string selectedPeriod = "Месяц";

    public StatisticsViewModel(RecordRep recordRep, Navigation navigation, UserRepository userRepository, User user)
    {
        _recordRep = recordRep;
        _navigation = navigation;
        _userRepository = userRepository;
        _user = user;
        
        // Получить ID текущего пользователя
        var userList = _userRepository.GetUserId(_user.Login, _user.Password);
        foreach (var u in userList)
        {
            _userId = u.Id;
            break;
        }
        
        LoadStatistics();
    }

    private void LoadStatistics()
    {
        var (dateFrom, dateTo) = GetDateRange(SelectedPeriod);
        Console.WriteLine($"[LoadStatistics] UserId: {_userId}, DateFrom: {dateFrom:yyyy-MM-dd}, DateTo: {dateTo:yyyy-MM-dd}");
        
        var stats = _recordRep.GetHospitalStatistics(_userId, dateFrom, dateTo);
        
        Console.WriteLine($"[LoadStatistics] Loaded {stats.Count} statistics entries:");
        foreach (var stat in stats)
        {
            Console.WriteLine($"  - {stat.HospitalName}: {stat.CompletedCount} completed, {stat.ScheduledCount} scheduled, {stat.TotalRevenue} revenue");
        }
        
        HospitalStatistics = new ObservableCollection<HospitalStatistic>(stats);
        
        TotalRecords = 0;
        TotalRevenue = 0;
        
        foreach (var stat in stats)
        {
            TotalRecords += stat.CompletedCount + stat.ScheduledCount;
            TotalRevenue += stat.TotalRevenue;
        }
    }

    private (DateTime, DateTime) GetDateRange(string period)
    {
        DateTime now = DateTime.Now;
        DateTime dateFrom;
        DateTime dateTo;

        switch (period)
        {
            case "День":
                dateFrom = now.Date;
                dateTo = now.Date.AddDays(1).AddSeconds(-1);
                break;
            case "Неделя":
                // Начало текущей недели (понедельник)
                int daysUntilMonday = (int)now.DayOfWeek - 1;
                if (daysUntilMonday < 0) daysUntilMonday = 6;
                dateFrom = now.Date.AddDays(-daysUntilMonday);
                dateTo = dateFrom.AddDays(7).AddSeconds(-1);
                break;
            case "Месяц":
                dateFrom = new DateTime(now.Year, now.Month, 1);
                dateTo = dateFrom.AddMonths(1).AddSeconds(-1);
                break;
            case "Год":
                dateFrom = new DateTime(now.Year, 1, 1);
                dateTo = new DateTime(now.Year, 12, 31, 23, 59, 59);
                break;
            default:
                dateFrom = new DateTime(now.Year, now.Month, 1);
                dateTo = dateFrom.AddMonths(1).AddSeconds(-1);
                break;
        }

        Console.WriteLine($"[Statistics] Period: {period}, From: {dateFrom:yyyy-MM-dd HH:mm:ss}, To: {dateTo:yyyy-MM-dd HH:mm:ss}");
        return (dateFrom, dateTo);
    }

    partial void OnSelectedPeriodChanged(string value)
    {
        LoadStatistics();
    }

    [RelayCommand]
    void SelectDay()
    {
        SelectedPeriod = "День";
    }

    [RelayCommand]
    void SelectWeek()
    {
        SelectedPeriod = "Неделя";
    }

    [RelayCommand]
    void SelectMonth()
    {
        SelectedPeriod = "Месяц";
    }

    [RelayCommand]
    void SelectYear()
    {
        SelectedPeriod = "Год";
    }

    [RelayCommand]
    void GoBack()
    {
        _navigation.GoBack();
    }
}
