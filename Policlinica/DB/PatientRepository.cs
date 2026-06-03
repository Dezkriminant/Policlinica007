using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MySqlConnector;
using System.Data.Common;

namespace Policlinica.DB;

public class PatientRepository
{
    private readonly DatabaseConnection _connection;

    public PatientRepository(IOptions<DatabaseConnection> options)
    {
        _connection = options.Value;
    }

    public List<Patient> GetAllPatients()
    {
        var patients = new List<Patient>();
        
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = "SELECT id, name, surname, phone_number, passport_series, passport_number FROM patients ORDER BY surname, name";
            
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        patients.Add(new Patient
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Surname = reader.GetString("surname"),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? "" : reader.GetString("phone_number"),
                            PassportSeries = reader.IsDBNull(reader.GetOrdinal("passport_series")) ? "" : reader.GetString("passport_series"),
                            PassportNumber = reader.IsDBNull(reader.GetOrdinal("passport_number")) ? "" : reader.GetString("passport_number")
                        });
                    }
                }
            }
        }
        
        return patients;
    }

    public Patient AddPatient(string name, string surname, string phoneNumber, string passportSeries, string passportNumber)
    {
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = "INSERT INTO patients (name, surname, phone_number, passport_series, passport_number) VALUES (@name, @surname, @phone_number, @passport_series, @passport_number)";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name ?? "");
                command.Parameters.AddWithValue("@surname", surname ?? "");
                command.Parameters.AddWithValue("@phone_number", phoneNumber ?? "");
                command.Parameters.AddWithValue("@passport_series", passportSeries ?? "");
                command.Parameters.AddWithValue("@passport_number", passportNumber ?? "");
                
                command.ExecuteNonQuery();
            }

            // Получить ID добавленного пациента
            string lastIdSql = "SELECT LAST_INSERT_ID()";
            using (var command = new MySqlCommand(lastIdSql, connection))
            {
                var result = command.ExecuteScalar();
                int patientId = Convert.ToInt32(result);
                return new Patient
                {
                    Id = patientId,
                    Name = name,
                    Surname = surname,
                    PhoneNumber = phoneNumber,
                    PassportSeries = passportSeries,
                    PassportNumber = passportNumber
                };
            }
        }
    }

    public bool DeletePatient(int patientId)
    {
        try
        {
            using (var connection = new MySqlConnection(_connection.ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM patients WHERE id = @id";
                
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", patientId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении пациента: {ex.Message}");
            return false;
        }
    }
}
