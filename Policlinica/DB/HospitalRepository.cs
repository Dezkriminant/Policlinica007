using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Policlinica.DB;

public class HospitalRepository
{
    private readonly DatabaseConnection _connection;

    public HospitalRepository(IOptions<DatabaseConnection> options)
    {
        _connection = options.Value;
    }

    public List<Hospital> GetAllHospitals()
    {
        var hospitals = new List<Hospital>();
        
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = "SELECT id, name, address, working_hours_start, working_hours_end FROM hospitals";
            
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hospitals.Add(new Hospital
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Address = reader.GetString("address"),
                            WorkingHoursStart = reader.GetTimeSpan("working_hours_start").ToString(@"hh\:mm"),
                            WorkingHoursEnd = reader.GetTimeSpan("working_hours_end").ToString(@"hh\:mm")
                        });
                    }
                }
            }
        }
        
        return hospitals;
    }

    public Hospital GetHospitalById(int id)
    {
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = "SELECT id, name, address, working_hours_start, working_hours_end FROM hospitals WHERE id = @id";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Hospital
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Address = reader.GetString("address"),
                            WorkingHoursStart = reader.GetTimeSpan("working_hours_start").ToString(@"hh\:mm"),
                            WorkingHoursEnd = reader.GetTimeSpan("working_hours_end").ToString(@"hh\:mm")
                        };
                    }
                }
            }
        }
        
        return null;
    }
}
