using System;
using System.Collections.Generic;

namespace Policlinica.ViewModels;

public class Navigation
{
    private StartViewModel _startViewModel;
    private Stack<ViewModelBase> _navigationStack = new();

    public void Navigate(ViewModelBase viewModel)
    {
        if (viewModel != null)
        {
            _navigationStack.Push(viewModel);
            _startViewModel.CurrentPage = viewModel;
            Console.WriteLine($"[Navigation] Навигирован на {viewModel.GetType().Name}. Стек: {_navigationStack.Count}");
        }
    }

    public void GoBack()
    {
        if (_navigationStack.Count > 1)
        {
            _navigationStack.Pop(); // Удалить текущий
            var previous = _navigationStack.Peek(); // Получить предыдущий
            
            // Обновить CurrentPage с null сначала, потом с правильным значением (для trigger обновления)
            _startViewModel.CurrentPage = null;
            _startViewModel.CurrentPage = previous;
            
            Console.WriteLine($"[Navigation] GoBack на {previous.GetType().Name}. Стек: {_navigationStack.Count}");
        }
    }

    public void SetCurrentView(StartViewModel startViewModel)
    {
        _startViewModel = startViewModel;
    }

    public void Close()
    {
        _startViewModel.Close();
    }
}
