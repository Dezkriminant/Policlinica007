using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Policlinica.DB;

namespace Policlinica.ViewModels;

public class ServiceWithSelected : ObservableObject
{
    private bool _isSelected;
    private Action _onSelectionChanged;

    public int Id { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                _onSelectionChanged?.Invoke();
            }
        }
    }

    public void SetOnSelectionChanged(Action onSelectionChanged)
    {
        _onSelectionChanged = onSelectionChanged;
    }
}

public partial class EditRecordViewModel : ViewModelBase
{
    private readonly Record _record;
    private readonly RecordRep _recordRep;
    private readonly DoctorRepository _doctorRepository;
    private readonly ServiceRepository _serviceRepository;
    private readonly RecordItemsRepository _recordItemsRepository;
    private readonly HospitalRepository _hospitalRepository;
    private readonly AppointmentRepository _appointmentRepository;
    private Action _closeAction;

    [ObservableProperty] string editClientName;
    [ObservableProperty] string editClientSurname;
    [ObservableProperty] string editPhoneNumber;
    [ObservableProperty] ObservableCollection<Hospital> hospitalList = new();
    [ObservableProperty] Hospital editSelectedHospital;
    [ObservableProperty] ObservableCollection<Doctor> doctorList = new();
    [ObservableProperty] Doctor editSelectedDoctor;
    [ObservableProperty] ObservableCollection<ServiceWithSelected> editServiceList = new();
    [ObservableProperty] DateTime editRecordDate = DateTime.Now;
    [ObservableProperty] ObservableCollection<string> availableTimes = new();
    [ObservableProperty] string editSelectedTime = "";
    [ObservableProperty] decimal editTotalAmount = 0;
    [ObservableProperty] string statusMessage = "";

    public EditRecordViewModel(Record record, RecordRep recordRep, DoctorRepository doctorRepository, 
        ServiceRepository serviceRepository, RecordItemsRepository recordItemsRepository,
        HospitalRepository hospitalRepository, AppointmentRepository appointmentRepository)
    {
        _record = record;
        _recordRep = recordRep;
        _doctorRepository = doctorRepository;
        _serviceRepository = serviceRepository;
        _recordItemsRepository = recordItemsRepository;
        _hospitalRepository = hospitalRepository;
        _appointmentRepository = appointmentRepository;

        // Загружаем больницы
        HospitalList = new ObservableCollection<Hospital>(hospitalRepository.GetAllHospitals());
        
        // Устанавливаем текущую больницу
        if (record.HospitalId > 0)
        {
            EditSelectedHospital = HospitalList.FirstOrDefault(h => h.Id == record.HospitalId);
        }

        // Заполняем данные записи
        EditClientName = record.ClientName;
        EditClientSurname = record.ClientSurname;
        EditPhoneNumber = record.PhoneNumber;
        EditRecordDate = record.RecordDate;
        if (!string.IsNullOrEmpty(record.AppointmentTime))
        {
            EditSelectedTime = record.AppointmentTime;
        }

        LoadAvailableTimes();
    }

    public void SetCloseAction(Action closeAction)
    {
        _closeAction = closeAction;
    }

    partial void OnEditSelectedHospitalChanged(Hospital value)
    {
        if (value != null)
        {
            var doctors = _doctorRepository.GetDoctorsByHospital(value.Id);
            DoctorList = new ObservableCollection<Doctor>(doctors);
            EditSelectedDoctor = DoctorList.FirstOrDefault(d => d.Id == _record.DoctorId);
        }
    }

    partial void OnEditSelectedDoctorChanged(Doctor value)
    {
        if (value != null)
        {
            var services = _serviceRepository.GetServicesByDoctors(value.Id);
            EditServiceList.Clear();
            foreach (var service in services)
            {
                var serviceWithSelected = new ServiceWithSelected
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    Price = service.Price,
                    IsSelected = false
                };
                serviceWithSelected.SetOnSelectionChanged(CalculateTotalAmount);
                EditServiceList.Add(serviceWithSelected);
            }
            LoadAvailableTimes();
        }
    }

    partial void OnEditRecordDateChanged(DateTime value)
    {
        LoadAvailableTimes();
    }

    private void LoadAvailableTimes()
    {
        AvailableTimes.Clear();
        if (EditSelectedHospital == null || EditSelectedDoctor == null)
            return;

        var occupiedTimes = _appointmentRepository.GetOccupiedTimes(EditSelectedHospital.Id, EditSelectedDoctor.Id, EditRecordDate);

        var startTime = TimeSpan.Parse(EditSelectedHospital.WorkingHoursStart);
        var endTime = TimeSpan.Parse(EditSelectedHospital.WorkingHoursEnd);
        var now = DateTime.Now;

        for (var time = startTime; time < endTime; time = time.Add(TimeSpan.FromMinutes(30)))
        {
            string timeStr = time.ToString(@"hh\:mm");
            
            if (EditRecordDate.Date == now.Date)
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

    private void CalculateTotalAmount()
    {
        EditTotalAmount = EditServiceList
            .Where(s => s.IsSelected)
            .Sum(s => s.Price);
    }

    [RelayCommand]
    void SaveRecord()
    {
        if (string.IsNullOrWhiteSpace(EditClientName) || string.IsNullOrWhiteSpace(EditClientSurname))
        {
            StatusMessage = "Имя и фамилия клиента обязательны";
            return;
        }

        if (string.IsNullOrWhiteSpace(EditPhoneNumber))
        {
            StatusMessage = "Введите номер телефона";
            return;
        }

        if (EditSelectedHospital == null)
        {
            StatusMessage = "Выберите больницу";
            return;
        }

        if (EditSelectedDoctor == null)
        {
            StatusMessage = "Выберите врача";
            return;
        }

        if (string.IsNullOrWhiteSpace(EditSelectedTime))
        {
            StatusMessage = "Выберите время приёма";
            return;
        }

        var selectedServices = EditServiceList.Where(s => s.IsSelected).ToList();
        if (selectedServices.Count == 0)
        {
            StatusMessage = "Выберите хотя бы одну услугу";
            return;
        }

        try
        {
            _record.ClientName = EditClientName;
            _record.ClientSurname = EditClientSurname;
            _record.PhoneNumber = EditPhoneNumber;
            _record.DoctorId = EditSelectedDoctor.Id;
            _record.HospitalId = EditSelectedHospital.Id;
            _record.AppointmentTime = EditSelectedTime;
            _record.TotalAmount = EditTotalAmount;
            _record.RecordDate = EditRecordDate;

            bool updated = _recordRep.UpdateRecord(_record);
            if (updated)
            {
                // Удаляем старые услуги
                _recordItemsRepository.DeleteByRecordId(_record.Id);

                // Добавляем новые услуги
                foreach (var service in selectedServices)
                {
                    _recordItemsRepository.InsertRecordItem(new RecordItem
                    {
                        RecordId = _record.Id,
                        ServiceId = service.Id,
                        ServicePrice = service.Price
                    });
                }

                StatusMessage = "Запись успешно обновлена";
                _closeAction?.Invoke();
            }
            else
            {
                StatusMessage = "Ошибка при обновлении записи";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
            Console.WriteLine($"Error updating record: {ex}");
        }
    }

    [RelayCommand]
    void CancelEdit()
    {
        _closeAction?.Invoke();
    }
}
