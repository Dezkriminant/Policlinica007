using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Policlinica.DB;
using Policlinica.Views;

namespace Policlinica.ViewModels;

public partial class DoctorSelectionViewModel : ViewModelBase
{
    private readonly DoctorRepository _doctorRepository;
    private DoctorSelectionView? _view;

    [ObservableProperty]
    private ObservableCollection<Doctor> doctors = new();

    [ObservableProperty]
    private Doctor? selectedDoctor;

    [ObservableProperty]
    private string searchText = "";

    public DoctorSelectionViewModel(DoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public void SetView(DoctorSelectionView view)
    {
        _view = view;
    }

    [RelayCommand]
    public void LoadDoctors()
    {
        try
        {
            var allDoctors = _doctorRepository.GetDoctorsByTest();
            Doctors.Clear();
            foreach (var doctor in allDoctors)
            {
                Doctors.Add(doctor);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке врачей: {ex.Message}");
        }
    }

    [RelayCommand]
    public void Search()
    {
        FilterDoctors(SearchText);
    }

    [RelayCommand]
    public void SelectDoctor()
    {
        if (SelectedDoctor != null)
        {
            _view?.Close();
        }
    }

    private void FilterDoctors(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            LoadDoctors();
            return;
        }

        try
        {
            var allDoctors = _doctorRepository.GetDoctorsByTest();
            var filtered = allDoctors
                .Where(d =>
                    d.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    d.Cabinet.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            Doctors.Clear();
            foreach (var doctor in filtered)
            {
                Doctors.Add(doctor);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при фильтрации врачей: {ex.Message}");
        }
    }
}
