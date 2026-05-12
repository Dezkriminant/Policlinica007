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
                
                    //окна
                s.AddTransient<AutorizationView>();
                s.AddTransient<AutorizationViewModel>();
                
                s.AddTransient<AdminWindowView>();
                s.AddTransient<AdminWindowViewModel>();
                
                s.AddTransient<RegistrationViewModel>();
                s.AddTransient<RegistrationView>();
                
                s.AddTransient<StartViewModel>();
                s.AddTransient<Startview>();
                
                 s.AddTransient<Records>();
                 s.AddTransient<RecordViewModel>();
                 
                 s.AddTransient<ServiceWindow>();
                 s.AddTransient<ServiceViewModel>();
                 
                 s.AddTransient<DoctoraWindow>();
                 s.AddTransient<DoctorViewModel>();
                 
                //Репозитории
                s.AddTransient<DoctorRepository>();
                
                s.AddTransient<UserRepository>();
                
                s.AddTransient<ServiceRepository>();
                
                s.AddTransient<RecordRep>();
                
                s.AddSingleton<Navigation>();
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