using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;

namespace Policlinica.ViewModels;

public partial class DateTimeViewModel : ViewModelBase
{
    private readonly IServiceProvider _provider;
    private readonly Navigation _navigation;
    private readonly Doctor _selectedDoctor;
    private readonly Hospital _selectedHospital;
    private readonly List<Service> _selectedServices;
    private readonly string _clientName;
    private readonly string _clientSurname;
    private readonly string _phoneNumber;
    private readonly AppointmentRepository _appointmentRepository;
    private readonly RecordItemsRepository _recordItemsRepository;
    private readonly RecordRep _recordRep;

    [ObservableProperty] DateTime selectedDate = DateTime.Now;
    [ObservableProperty] ObservableCollection<string> availableTimes = new();
    [ObservableProperty] string selectedTime = "";
    [ObservableProperty] string statusMessage = "";
    [ObservableProperty] decimal totalAmount = 0;

    public DateTimeViewModel(IServiceProvider provider, Doctor doctor, Hospital hospital, List<Service> services, 
        string clientName, string clientSurname, string phoneNumber)
    {
        _provider = provider;
        _navigation = provider.GetRequiredService<Navigation>();
        _selectedDoctor = doctor;
        _selectedHospital = hospital;
        _selectedServices = services;
        _clientName = clientName;
        _clientSurname = clientSurname;
        _phoneNumber = phoneNumber;
        _appointmentRepository = provider.GetRequiredService<AppointmentRepository>();
        _recordItemsRepository = provider.GetRequiredService<RecordItemsRepository>();
        _recordRep = provider.GetRequiredService<RecordRep>();

        TotalAmount = services.Sum(s => s.Price);
        LoadAvailableTimes();
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        LoadAvailableTimes();
    }

    private void LoadAvailableTimes()
    {
        AvailableTimes.Clear();
        var occupiedTimes = _appointmentRepository.GetOccupiedTimes(_selectedHospital.Id, _selectedDoctor.Id, SelectedDate);

        var startTime = TimeSpan.Parse(_selectedHospital.WorkingHoursStart);
        var endTime = TimeSpan.Parse(_selectedHospital.WorkingHoursEnd);
        var now = DateTime.Now;

        for (var time = startTime; time < endTime; time = time.Add(TimeSpan.FromMinutes(30)))
        {
            string timeStr = time.ToString(@"hh\:mm");
            
            if (SelectedDate.Date == now.Date)
            {
                var timeAsDateTime = DateTime.ParseExact(timeStr, "HH:mm", null);
                if (timeAsDateTime.TimeOfDay <= now.TimeOfDay.Add(TimeSpan.FromMinutes(30)))
                {
                    continue;
                }
            }

            if (!occupiedTimes.Contains(timeStr))
            {
                AvailableTimes.Add(timeStr);
            }
        }
    }

    [RelayCommand]
    void ConfirmBooking()
    {
        if (string.IsNullOrWhiteSpace(SelectedTime))
        {
            StatusMessage = "Выберите время";
            return;
        }

        try
        {
            var record = new Record
            {
                ClientName = _clientName,
                ClientSurname = _clientSurname,
                DoctorId = _selectedDoctor.Id,
                UserId = _provider.GetRequiredService<User>().Id,
                ServiceId = _selectedServices[0].Id,
                TotalAmount = TotalAmount,
                RecordDate = SelectedDate,
                HospitalId = _selectedHospital.Id,
                AppointmentTime = SelectedTime,
                PhoneNumber = _phoneNumber,
                Cabinet = _selectedDoctor.Cabinet
            };

            int recordId = _recordRep.InsertRecord(record);
            if (recordId <= 0)
            {
                StatusMessage = "Ошибка при сохранении записи";
                return;
            }

            foreach (var service in _selectedServices)
            {
                _recordItemsRepository.InsertRecordItem(new RecordItem
                {
                    RecordId = recordId,
                    ServiceId = service.Id,
                    ServicePrice = service.Price
                });
            }

            _appointmentRepository.BookAppointment(recordId, _selectedHospital.Id, _selectedDoctor.Id, SelectedDate, TimeSpan.Parse(SelectedTime));

            StatusMessage = "Запись успешно создана!";
            var vm = ActivatorUtilities.CreateInstance<AdminViewModel>(_provider);
            _navigation.Navigate(vm);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
            Console.WriteLine($"Error: {ex}");
        }
    }

    [RelayCommand]
    void GoBack()
    {
        var serviceRepository = _provider.GetRequiredService<ServiceRepository>();
        var vm = ActivatorUtilities.CreateInstance<ServiceViewModel>(_provider, _navigation, _selectedDoctor, 
            _selectedHospital, serviceRepository, _clientName, _clientSurname);
        _navigation.Navigate(vm);
    }
}
