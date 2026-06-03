using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Policlinica.DB;
using Policlinica.Views;

namespace Policlinica.ViewModels;

public partial class AddPatientViewModel : ViewModelBase
{
    private readonly PatientRepository _patientRepository;
    private AddPatientView? _view;
    private PatientManagementViewModel? _parentViewModel;

    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private string surname = "";

    [ObservableProperty]
    private string phoneNumber = "";

    [ObservableProperty]
    private string passportSeries = "";

    [ObservableProperty]
    private string passportNumber = "";

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool isSuccess = false;

    public AddPatientViewModel(PatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public void SetView(AddPatientView view)
    {
        _view = view;
    }

    public void SetParentViewModel(PatientManagementViewModel parentViewModel)
    {
        _parentViewModel = parentViewModel;
    }

    [RelayCommand]
    public void AddPatient()
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Введите имя пациента";
            IsSuccess = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(Surname))
        {
            ErrorMessage = "Введите фамилию пациента";
            IsSuccess = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ErrorMessage = "Введите номер телефона";
            IsSuccess = false;
            return;
        }

        if (PhoneNumber.Length > 20)
        {
            ErrorMessage = "Номер телефона не может быть длиннее 20 символов";
            IsSuccess = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(PassportSeries))
        {
            ErrorMessage = "Введите серию паспорта";
            IsSuccess = false;
            return;
        }

        if (PassportSeries.Length > 10)
        {
            ErrorMessage = "Серия паспорта не может быть длиннее 10 символов";
            IsSuccess = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(PassportNumber))
        {
            ErrorMessage = "Введите номер паспорта";
            IsSuccess = false;
            return;
        }

        if (PassportNumber.Length > 50)
        {
            ErrorMessage = "Номер паспорта не может быть длиннее 50 символов";
            IsSuccess = false;
            return;
        }

        ErrorMessage = "";
        IsLoading = true;
        IsSuccess = false;

        try
        {
            var newPatient = _patientRepository.AddPatient(
                Name.Trim(), 
                Surname.Trim(), 
                PhoneNumber.Trim(),
                PassportSeries.Trim(),
                PassportNumber.Trim()
            );
            
            IsSuccess = true;
            ErrorMessage = "Пациент успешно добавлен!";
            
            // Очистить форму
            Name = "";
            Surname = "";
            PhoneNumber = "";
            PassportSeries = "";
            PassportNumber = "";
            
            IsLoading = false;

            // Обновить список в родительском ViewModel
            _parentViewModel?.RefreshPatients();

            // Закрыть окно сразу
            _view?.Close();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка при добавлении пациента: {ex.Message}";
            IsSuccess = false;
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void Cancel()
    {
        Name = "";
        Surname = "";
        PhoneNumber = "";
        PassportSeries = "";
        PassportNumber = "";
        ErrorMessage = "";
        IsSuccess = false;
        _view?.Close();
    }
}
