using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Policlinica.DB;

namespace Policlinica.ViewModels;

public partial class RecordItemsViewModel : ViewModelBase
{
    private readonly IServiceProvider _provider;
    private readonly Navigation _navigation;
    private readonly RecordRep _recordRepository;
    private readonly RecordItemsRepository _recordItemsRepository;

    [ObservableProperty] ObservableCollection<Service> selectedServices;
    [ObservableProperty] Doctor selectedDoctor;
    [ObservableProperty] DateTime recordDate;
    [ObservableProperty] decimal totalAmount;
    [ObservableProperty] User currentUser;
    [ObservableProperty] string statusMessage;
    [ObservableProperty] string clientName;
    [ObservableProperty] string clientSurname;

    public RecordItemsViewModel(IServiceProvider provider, Navigation navigation, Doctor doctor, 
        List<Service> services, RecordRep recordRepository, RecordItemsRepository recordItemsRepository,
        string name = "", string surname = "")
    {
        _provider = provider;
        _navigation = navigation;
        _recordRepository = recordRepository;
        _recordItemsRepository = recordItemsRepository;
        
        selectedDoctor = doctor;
        selectedServices = new ObservableCollection<Service>(services);
        recordDate = DateTime.Now;
        currentUser = _provider.GetRequiredService<User>();
        clientName = name;
        clientSurname = surname;
        
        // Расчет общей суммы
        totalAmount = selectedServices.Sum(s => s.Price);
    }

    [RelayCommand]
    public void SaveToDatabase()
    {
        if (selectedDoctor == null || selectedServices.Count == 0)
        {
            StatusMessage = "Ошибка: не все данные заполнены";
            return;
        }

        if (string.IsNullOrWhiteSpace(clientName) || string.IsNullOrWhiteSpace(clientSurname))
        {
            StatusMessage = "Ошибка: не заполнены имя и фамилия клиента";
            return;
        }

        try
        {
            // Используем первую услугу
            int mainServiceId = selectedServices[0].Id;
            Console.WriteLine($"Main service ID: {mainServiceId}");
            Console.WriteLine($"Selected services count: {selectedServices.Count}");

            // Создаем запись с данными КЛИЕНТА
            var record = new Record
            {
                ClientName = clientName,
                ClientSurname = clientSurname,
                DoctorId = selectedDoctor.Id,
                UserId = currentUser.Id,
                ServiceId = mainServiceId,
                TotalAmount = totalAmount,
                RecordDate = recordDate
            };

            Console.WriteLine($"Saving record: Name={record.ClientName}, Surname={record.ClientSurname}, DoctorId={record.DoctorId}, UserId={record.UserId}, ServiceId={record.ServiceId}");

            // Сохраняем запись в БД и получаем ID
            int recordId = _recordRepository.InsertRecord(record);
            
            if (recordId <= 0)
            {
                StatusMessage = "Ошибка при сохранении записи";
                Console.WriteLine($"Failed to insert record. Returned ID: {recordId}");
                return;
            }

            Console.WriteLine($"Record saved with ID: {recordId}");

            // Сохраняем все выбранные услуги в record_items
            foreach (var service in selectedServices)
            {
                Console.WriteLine($"Saving record item for service: {service.Id} (price: {service.Price})");
                
                var recordItem = new RecordItem
                {
                    ServiceId = service.Id,
                    RecordId = recordId,
                    ServicePrice = service.Price
                };

                bool itemSaved = _recordItemsRepository.InsertRecordItem(recordItem);
                if (!itemSaved)
                {
                    Console.WriteLine($"Failed to insert record item for service {service.Id}");
                }
            }

            StatusMessage = "Запись успешно сохранена!";
            
            // Переходим в админ-панель
            var vm = ActivatorUtilities.CreateInstance<AdminViewModel>(_provider);
            _navigation.Navigate(vm);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
            Console.WriteLine($"Exception: {ex}");
        }
    }

    [RelayCommand]
    public void Cancel()
    {
        var repository = _provider.GetRequiredService<ServiceRepository>();
        var vm = ActivatorUtilities.CreateInstance<ServiceViewModel>(_provider, selectedDoctor, repository, clientName, clientSurname);
        _navigation.Navigate(vm);
    }
}
