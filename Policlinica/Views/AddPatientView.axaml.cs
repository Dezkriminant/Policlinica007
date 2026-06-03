using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Policlinica.Views;

public partial class AddPatientView : Window
{
    public AddPatientView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is ViewModels.AddPatientViewModel viewModel)
        {
            viewModel.SetView(this);
        }
    }

    private void AddButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.AddPatientViewModel viewModel)
        {
            viewModel.AddPatientCommand.Execute(null);
            // Закрыть окно после сохранения
            if (viewModel.IsSuccess)
            {
                Close();
            }
        }
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.AddPatientViewModel viewModel)
        {
            viewModel.CancelCommand.Execute(null);
        }
    }
}
