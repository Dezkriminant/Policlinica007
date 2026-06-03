using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Policlinica.Views;
using Policlinica.DB;
using Microsoft.Extensions.DependencyInjection;

namespace Policlinica.ViewModels;

public partial class RecordViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly PatientRepository _patientRepository;
    private readonly DoctorRepository _doctorRepository;
    private readonly Navigation _navigation;

    [ObservableProperty]
    private Patient? selectedPatient;

    [ObservableProperty]
    private Doctor? selectedDoctor;

    [ObservableProperty]
    private ObservableCollection<Record> records = new();

    public RecordViewModel(IServiceProvider serviceProvider, PatientRepository patientRepository, DoctorRepository doctorRepository, Navigation navigation)
    {
        _serviceProvider = serviceProvider;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _navigation = navigation;
    }

    [RelayCommand]
    public void AddNewRecord()
    {
        try
        {
            // Открыть PatientListViewModel с callback
            var patientListVm = ActivatorUtilities.CreateInstance<PatientListViewModel>(_serviceProvider);
            patientListVm.SetOnPatientSelected(patient =>
            {
                SelectedPatient = patient;
                Console.WriteLine($"✓ Пациент выбран: {patient.Surname} {patient.Name}");
                
                // После выбора пациента открыть выбор врача
                var doctorSelectionVm = ActivatorUtilities.CreateInstance<DoctorSelectionViewModel>(_serviceProvider);
                doctorSelectionVm.LoadDoctorsCommand.Execute(null);
                
                var doctorSelectionView = ActivatorUtilities.CreateInstance<DoctorSelectionView>(_serviceProvider);
                doctorSelectionView.DataContext = doctorSelectionVm;
                doctorSelectionView.ShowDialog(null);

                if (doctorSelectionVm.SelectedDoctor != null)
                {
                    SelectedDoctor = doctorSelectionVm.SelectedDoctor;
                    Console.WriteLine($"✓ Врач выбран: {SelectedDoctor.Title}, кабинет: {SelectedDoctor.Cabinet}");
                }
            });
            
            _navigation.Navigate(patientListVm);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка: {ex.Message}");
        }
    }
}
