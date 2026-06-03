using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;

namespace Policlinica.ViewModels;

public partial class BloodSugarInputViewModel : ViewModelBase
{
    private readonly BloodSugarRepository _bloodSugarRepository;
    private readonly Navigation _navigation;
    private readonly IServiceProvider _provider;
    private readonly RecordRep _recordRep;
    private Record _selectedRecord;
    private Patient _selectedPatient;

    [ObservableProperty] string sugarLevel = "";
    [ObservableProperty] string measurementDate = DateTime.Now.ToString("yyyy-MM-dd");
    [ObservableProperty] ObservableCollection<BloodSugarRecord> bloodSugarHistory = new();
    [ObservableProperty] string statusMessage = "";
    [ObservableProperty] string patientInfo = "";
    [ObservableProperty] int recordId = 0;

    private Action _closeAction;

    public BloodSugarInputViewModel(BloodSugarRepository bloodSugarRepository, Navigation navigation, IServiceProvider provider, RecordRep recordRep)
    {
        _bloodSugarRepository = bloodSugarRepository;
        _navigation = navigation;
        _provider = provider;
        _recordRep = recordRep;
    }

    public void SetSelectedRecord(Record record)
    {
        _selectedRecord = record;
        if (record != null)
        {
            RecordId = record.Id;
            PatientInfo = $"Пациент: {record.ClientName} {record.ClientSurname}";
            LoadBloodSugarHistory();
        }
    }

    public void SetSelectedPatient(Patient patient)
    {
        _selectedPatient = patient;
        if (patient != null)
        {
            PatientInfo = $"Пациент: {patient.Name} {patient.Surname}";
            
            // Попробать найти последнюю запись этого пациента, или создать фиктивную
            var userRepository = _provider.GetRequiredService<UserRepository>();
            var users = userRepository.GetUserId("", ""); // Получить текущего пользователя
            
            // Используем ID пациента как ID для измерений
            RecordId = patient.Id;
            LoadBloodSugarHistory();
        }
    }

    public void SetCloseAction(Action closeAction)
    {
        _closeAction = closeAction;
    }

    private void LoadBloodSugarHistory()
    {
        if (RecordId <= 0) return;
        
        // Если это пациент (SetSelectedPatient), используем GetBloodSugarByPatientId
        // Если это запись (SetSelectedRecord), используем GetBloodSugarByRecord
        var records = (_selectedPatient != null && _selectedRecord == null)
            ? _bloodSugarRepository.GetBloodSugarByPatientId(RecordId)
            : _bloodSugarRepository.GetBloodSugarByRecord(RecordId);
        
        BloodSugarHistory = new ObservableCollection<BloodSugarRecord>(records);
    }

    [RelayCommand]
    void AddBloodSugar()
    {
        if (RecordId <= 0)
        {
            StatusMessage = "Ошибка: пациент не выбран";
            return;
        }

        if (string.IsNullOrWhiteSpace(SugarLevel) || !decimal.TryParse(SugarLevel, out decimal sugarValue))
        {
            StatusMessage = "Введите корректный уровень сахара";
            return;
        }

        if (sugarValue <= 0)
        {
            StatusMessage = "Уровень сахара должен быть больше нуля";
            return;
        }

        if (!DateTime.TryParse(MeasurementDate, out DateTime date))
        {
            StatusMessage = "Неверный формат даты (YYYY-MM-DD)";
            return;
        }

        try
        {
            // Если записи нет (измеряем от пациента), используем RecordId как patientId
            bool inserted = _bloodSugarRepository.InsertBloodSugar(RecordId, sugarValue, date);
            if (inserted)
            {
                StatusMessage = "Данные сахара успешно сохранены";
                SugarLevel = "";
                MeasurementDate = DateTime.Now.ToString("yyyy-MM-dd");
                LoadBloodSugarHistory();
            }
            else
            {
                StatusMessage = "Ошибка при сохранении данных";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
            Console.WriteLine($"Error adding blood sugar: {ex}");
        }
    }

    [RelayCommand]
    void DeleteRecord(BloodSugarRecord record)
    {
        if (record == null)
        {
            StatusMessage = "Выберите запись для удаления";
            return;
        }

        try
        {
            bool deleted = _bloodSugarRepository.DeleteBloodSugar(record.Id);
            if (deleted)
            {
                StatusMessage = "Запись удалена";
                LoadBloodSugarHistory();
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
    void GoBack()
    {
        _navigation.GoBack();
    }
}
