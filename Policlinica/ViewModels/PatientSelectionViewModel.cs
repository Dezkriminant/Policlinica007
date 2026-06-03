using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Policlinica.DB;
using Policlinica.Views;

namespace Policlinica.ViewModels;

public partial class PatientSelectionViewModel : ViewModelBase
{
    private readonly PatientRepository _patientRepository;
    private readonly IServiceProvider _serviceProvider;
    private PatientSelectionView? _view;

    [ObservableProperty]
    private ObservableCollection<Patient> patients = new();

    [ObservableProperty]
    private Patient? selectedPatient;

    [ObservableProperty]
    private string searchText = "";

    public PatientSelectionViewModel(PatientRepository patientRepository, IServiceProvider serviceProvider)
    {
        _patientRepository = patientRepository;
        _serviceProvider = serviceProvider;
    }

    public void SetView(PatientSelectionView view)
    {
        _view = view;
    }

    [RelayCommand]
    public void LoadPatients()
    {
        try
        {
            var allPatients = _patientRepository.GetAllPatients();
            Patients.Clear();
            foreach (var patient in allPatients)
            {
                Patients.Add(patient);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке пациентов: {ex.Message}");
        }
    }

    [RelayCommand]
    public void Search()
    {
        FilterPatients(SearchText);
    }

    [RelayCommand]
    public void AddNewPatient()
    {
        try
        {
            var addPatientViewModel = _serviceProvider.GetService(typeof(AddPatientViewModel)) as AddPatientViewModel;
            var addPatientView = _serviceProvider.GetService(typeof(AddPatientView)) as AddPatientView;

            if (addPatientView != null && addPatientViewModel != null)
            {
                addPatientView.DataContext = addPatientViewModel;
                addPatientView.ShowDialog(_view);

                // После добавления пациента перезагрузить список
                LoadPatients();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении пациента: {ex.Message}");
        }
    }

    [RelayCommand]
    public void SelectPatient()
    {
        if (SelectedPatient != null)
        {
            _view?.Close();
        }
    }

    private void FilterPatients(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            LoadPatients();
            return;
        }

        try
        {
            var allPatients = _patientRepository.GetAllPatients();
            var filtered = allPatients
                .Where(p =>
                    p.Surname.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.PhoneNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            Patients.Clear();
            foreach (var patient in filtered)
            {
                Patients.Add(patient);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при фильтрации пациентов: {ex.Message}");
        }
    }
}
