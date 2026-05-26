using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;
using Policlinica.Views;


namespace Policlinica.ViewModels;

public partial class AdminViewModel : ViewModelBase
{
    private readonly Navigation _navigation;
    private readonly IServiceProvider _provider;
    private readonly RecordRep _recordRep;
    private readonly User _user;
    private readonly UserRepository _userRepository;
    private readonly DoctorRepository _doctorRepository;
    private readonly ServiceRepository _serviceRepository;
    private List<Record> _allRecords;
    private Window _parentWindow;

    [ObservableProperty] string _login;
    [ObservableProperty] int _id;
    [ObservableProperty] ObservableCollection<Record> _recordsList = new();
    [ObservableProperty] private Record _selectedRecord;
    [ObservableProperty] private ObservableCollection<User> userList = new ObservableCollection<User>();
    [ObservableProperty] string statusMessage = "";
    [ObservableProperty] string searchText = "";

    private Action _closeAction;

    public AdminViewModel(Navigation navigation, IServiceProvider provider, RecordRep recordRep, User user, UserRepository userRepository, DoctorRepository doctorRepository, ServiceRepository serviceRepository)
    {
        _navigation = navigation;
        _provider = provider;
        _recordRep = recordRep;
        _user = user;
        _userRepository = userRepository;
        _doctorRepository = doctorRepository;
        _serviceRepository = serviceRepository;
        
        UserList = new ObservableCollection<User>(userRepository.GetUserId(user.Login, user.Password));

        foreach (var obj in UserList)
        {
            Id = obj.Id;
        }

        _allRecords = recordRep.GetRecord(Id);
        RecordsList = new ObservableCollection<Record>(_allRecords);
    }

    public void SetCloseAction(Action closeAction)
    {
        _closeAction = closeAction;
    }

    public void SetParentWindow(Window parentWindow)
    {
        _parentWindow = parentWindow;
    }

    [RelayCommand]
    void SearchRecords()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            RecordsList = new ObservableCollection<Record>(_allRecords);
            return;
        }

        var filteredRecords = _allRecords
            .Where(r => r.ClientName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       r.ClientSurname.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            .ToList();

        RecordsList = new ObservableCollection<Record>(filteredRecords);
        StatusMessage = $"Найдено {filteredRecords.Count} записей";
    }

    [RelayCommand]
    void EditRecord()
    {
        if (SelectedRecord == null)
        {
            StatusMessage = "Выберите запись для редактирования";
            return;
        }

        var editWindow = new EditRecordWindow();
        var editViewModel = ActivatorUtilities.CreateInstance<EditRecordViewModel>(_provider, SelectedRecord, _recordRep, _doctorRepository, _serviceRepository, 
            _provider.GetRequiredService<RecordItemsRepository>(), _provider.GetRequiredService<HospitalRepository>(), _provider.GetRequiredService<AppointmentRepository>());
        editWindow.DataContext = editViewModel;
        
        editViewModel.SetCloseAction(() =>
        {
            editWindow.Close();
            _allRecords = _recordRep.GetRecord(Id);
            RecordsList = new ObservableCollection<Record>(_allRecords);
            SelectedRecord = null;
            SearchText = "";
            StatusMessage = "Запись обновлена";
        });
        
        editWindow.ShowDialog(_parentWindow);
    }

    [RelayCommand]
    void DeleteRecord()
    {
        if (SelectedRecord == null)
        {
            StatusMessage = "Выберите запись для удаления";
            return;
        }

        try
        {
            bool deleted = _recordRep.Delete(SelectedRecord.Id);
            if (deleted)
            {
                StatusMessage = "Запись успешно удалена";
                _allRecords = _recordRep.GetRecord(Id);
                RecordsList = new ObservableCollection<Record>(_allRecords);
                SelectedRecord = null;
                SearchText = "";
            }
            else
            {
                StatusMessage = "Ошибка при удалении записи";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
            Console.WriteLine($"Error deleting record: {ex}");
        }
    }

    [RelayCommand]
    void GoService()
    {
        var vm = ActivatorUtilities.CreateInstance<HospitalViewModel>(_provider);
        _navigation.Navigate(vm);
    }

    [RelayCommand]
    void GoSugarCheck()
    {
        var vm = ActivatorUtilities.CreateInstance<SugarCheckViewModel>(_provider);
        _navigation.Navigate(vm);
    }

    [RelayCommand]
    void GoBloodSugarInput()
    {
        if (SelectedRecord == null)
        {
            StatusMessage = "Выберите запись пациента для добавления измерений сахара";
            return;
        }

        var vm = ActivatorUtilities.CreateInstance<BloodSugarInputViewModel>(_provider);
        vm.SetSelectedRecord(SelectedRecord);
        _navigation.Navigate(vm);
    }

    [RelayCommand]
    void ExitApplication()
    {
        Console.WriteLine("Exit button clicked");
        _closeAction?.Invoke();
    }
}
