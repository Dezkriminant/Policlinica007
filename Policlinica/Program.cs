using Avalonia;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Policlinica.DB;
using Policlinica.ViewModels;
using Policlinica.Views;



namespace Policlinica;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder().
            ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsetting.json")
                    .AddEnvironmentVariables();
            }).
            ConfigureServices((c,s) =>
            {
                s.Configure<DatabaseConnection>(c.Configuration.
                    GetSection("DatabaseConnection"));
                
                s.AddTransient<AutorizationView>();
                s.AddTransient<AutorizationViewModel>();
                
                s.AddTransient<AdminView>();
                s.AddTransient<AdminViewModel>();
                
                s.AddTransient<EditRecordWindow>();
                s.AddTransient<EditRecordView>();
                s.AddTransient<EditRecordViewModel>();
                
                s.AddTransient<BloodSugarInputView>();
                s.AddTransient<BloodSugarInputViewModel>();
                
                s.AddTransient<HospitalView>();
                s.AddTransient<HospitalViewModel>();
                
                s.AddTransient<DateTimeView>();
                s.AddTransient<DateTimeViewModel>();
                
                s.AddTransient<PatientListView>();
                s.AddTransient<PatientListViewModel>();
                
                s.AddTransient<RegistrationViewModel>();
                s.AddTransient<RegistrationView>();
                
                s.AddTransient<StartViewModel>();
                s.AddTransient<Startview>();
                
                 s.AddTransient<Records>();
                 s.AddTransient<RecordViewModel>();
                 
                 s.AddTransient<ServiceView>();
                 s.AddTransient<ServiceViewModel>();
                 
                 s.AddTransient<DoctorView>();
                 s.AddTransient<DoctorViewModel>();
                 
                 s.AddTransient<RecordItemsView>();
                 
                 s.AddTransient<PatientSelectionView>();
                 s.AddTransient<PatientSelectionViewModel>();
                 
                 s.AddTransient<AddPatientView>();
                 s.AddTransient<AddPatientViewModel>();
                 
                 s.AddTransient<DoctorSelectionView>();
                 s.AddTransient<DoctorSelectionViewModel>();
                 
                 s.AddTransient<PatientManagementView>();
                 s.AddTransient<PatientManagementViewModel>();
               
                s.AddTransient<DoctorRepository>();
                
                s.AddTransient<UserRepository>();
                
                s.AddTransient<ServiceRepository>();
                
                s.AddTransient<RecordRep>();
                
                s.AddTransient<BloodSugarRepository>();
                
                s.AddTransient<HospitalRepository>();
                
                s.AddTransient<AppointmentRepository>();
                
                s.AddTransient<PatientRepository>();
                
                s.AddSingleton<Navigation>();

                s.AddSingleton<RecordItemsRepository>();
                s.AddSingleton<User>();
                
                
                s.AddTransient<Doctor>();
                
                s.AddTransient<ServiceSelected>();
            }).
            Build();
        BuildAvaloniaApp(host.Services)
            .StartWithClassicDesktopLifetime(args);
    }
    
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp(IServiceProvider serviceProvider)
        => AppBuilder.Configure(()=> new App(serviceProvider))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
