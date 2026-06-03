using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.ViewModels;

namespace Policlinica.Views;

public partial class Records : Window
{
    public Records()
    {
        InitializeComponent();
    }

    public Records(RecordViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
