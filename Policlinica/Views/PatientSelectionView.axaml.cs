using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Policlinica.Views;

public partial class PatientSelectionView : Window
{
    public PatientSelectionView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is ViewModels.PatientSelectionViewModel viewModel)
        {
            viewModel.SetView(this);
        }
    }

    private void SelectButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.PatientSelectionViewModel viewModel)
        {
            viewModel.SelectPatientCommand.Execute(null);
        }
    }

    private void AddNewButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.PatientSelectionViewModel viewModel)
        {
            viewModel.AddNewPatientCommand.Execute(null);
        }
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void SearchButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.PatientSelectionViewModel viewModel)
        {
            viewModel.SearchCommand.Execute(null);
        }
    }
}
