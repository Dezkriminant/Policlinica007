using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Policlinica.DB;

public class AppointmentRepository
{
    private readonly DatabaseConnection _connection;

    public AppointmentRepository(IOptions<DatabaseConnection> options)
    {
        _connection = options.Value;
    }

    public List<string> GetOccupiedTimes(int hospitalId, int doctorId, DateTime date)
    {
        var occupiedTimes = new List<string>();
        
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = @"SELECT appointment_time FROM appointments 
                           WHERE hospital_id = @hospitalId 
                           AND doctor_id = @doctorId 
                           AND appointment_date = @date";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@hospitalId", hospitalId);
                command.Parameters.AddWithValue("@doctorId", doctorId);
                command.Parameters.AddWithValue("@date", date.Date);
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        occupiedTimes.Add(reader.GetTimeSpan("appointment_time").ToString(@"hh\:mm"));
                    }
                }
            }
        }
        
        return occupiedTimes;
    }

    public bool IsTimeSlotAvailable(int hospitalId, int doctorId, DateTime date, TimeSpan time)
    {
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = @"SELECT COUNT(*) FROM appointments 
                           WHERE hospital_id = @hospitalId 
                           AND doctor_id = @doctorId 
                           AND appointment_date = @date
                           AND appointment_time = @time";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@hospitalId", hospitalId);
                command.Parameters.AddWithValue("@doctorId", doctorId);
                command.Parameters.AddWithValue("@date", date.Date);
                command.Parameters.AddWithValue("@time", time);
                
                int count = (int)command.ExecuteScalar();
                return count == 0;
            }
        }
    }

    public bool BookAppointment(int recordId, int hospitalId, int doctorId, DateTime date, TimeSpan time)
    {
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = @"INSERT INTO appointments (record_id, hospital_id, doctor_id, appointment_date, appointment_time) 
                           VALUES (@recordId, @hospitalId, @doctorId, @date, @time)";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@recordId", recordId);
                command.Parameters.AddWithValue("@hospitalId", hospitalId);
                command.Parameters.AddWithValue("@doctorId", doctorId);
                command.Parameters.AddWithValue("@date", date.Date);
                command.Parameters.AddWithValue("@time", time);
                
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }
    }
}
