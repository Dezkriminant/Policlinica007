using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Policlinica.Views;

public partial class DoctorSelectionView : Window
{
    public DoctorSelectionView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is ViewModels.DoctorSelectionViewModel viewModel)
        {
            viewModel.SetView(this);
        }
    }

    private void SelectButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.DoctorSelectionViewModel viewModel)
        {
            viewModel.SelectDoctorCommand.Execute(null);
        }
    }

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void SearchButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.DoctorSelectionViewModel viewModel)
        {
            viewModel.SearchCommand.Execute(null);
        }
    }
}
