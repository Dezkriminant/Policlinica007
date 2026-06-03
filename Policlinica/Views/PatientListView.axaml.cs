using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Policlinica.Views;

public partial class PatientListView : UserControl
{
    public PatientListView()
    {
        InitializeComponent();
    }

    private void SearchButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.PatientListViewModel viewModel)
        {
            viewModel.SearchCommand?.Execute(null);
        }
    }

    private void DeleteButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.PatientListViewModel viewModel)
        {
            viewModel.DeletePatientCommand?.Execute(null);
        }
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.PatientListViewModel viewModel)
        {
            viewModel.GoBackCommand?.Execute(null);
        }
    }
}
