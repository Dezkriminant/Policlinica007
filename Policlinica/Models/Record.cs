using System;
using System.Collections.Generic;

namespace Policlinica.DB;

public class Record
{
    public int Id { get; set; }
    
    public string ClientName { get; set; }
    
    public string ClientSurname { get; set; }
    
    public int DoctorId { get; set; }
    
    public int UserId { get; set; }
    
    public int ServiceId { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public DateTime RecordDate { get; set; }
    
    public string Title { get; set; }
    public string Name { get; set; }
    public string ServiceName { get; set; }
    
    public List<string> Services { get; set; } = new List<string>();
    
    public int HospitalId { get; set; }
    public string HospitalName { get; set; }
    public string Cabinet { get; set; }
    public string AppointmentTime { get; set; }
    public string PhoneNumber { get; set; }

    // Статус: прошла или нет
    public string Status
    {
        get
        {
            var appointmentDateTime = DateTime.ParseExact($"{RecordDate:yyyy-MM-dd} {AppointmentTime}", "yyyy-MM-dd HH:mm", null);
            return appointmentDateTime < DateTime.Now ? "Прошла" : "Будущая";
        }
    }
}
