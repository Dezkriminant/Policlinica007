using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Policlinica.DB;

public class BloodSugarRepository
{
    private readonly DatabaseConnection _connection;

    public BloodSugarRepository(IOptions<DatabaseConnection> options)
    {
        _connection = options.Value;
    }

    public List<BloodSugarRecord> GetBloodSugarByRecord(int recordId)
    {
        var records = new List<BloodSugarRecord>();
        
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = "SELECT id, patient_id, sugar_level, measurement_date FROM blood_sugar WHERE patient_id IN (SELECT patient_id FROM records WHERE id = @recordId) ORDER BY measurement_date DESC";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@recordId", recordId);
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        records.Add(new BloodSugarRecord
                        {
                            Id = reader.GetInt32("id"),
                            RecordId = reader.GetInt32("patient_id"),
                            SugarLevel = reader.GetDecimal("sugar_level"),
                            MeasurementDate = reader.GetDateTime("measurement_date")
                        });
                    }
                }
            }
        }
        
        return records;
    }

    // Новый метод для загрузки по patient_id (когда добавляем измерение прямо от пациента)
    public List<BloodSugarRecord> GetBloodSugarByPatientId(int patientId)
    {
        var records = new List<BloodSugarRecord>();
        
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = "SELECT id, patient_id, sugar_level, measurement_date FROM blood_sugar WHERE patient_id = @patientId ORDER BY measurement_date DESC";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@patientId", patientId);
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        records.Add(new BloodSugarRecord
                        {
                            Id = reader.GetInt32("id"),
                            RecordId = reader.GetInt32("patient_id"),
                            SugarLevel = reader.GetDecimal("sugar_level"),
                            MeasurementDate = reader.GetDateTime("measurement_date")
                        });
                    }
                }
            }
        }
        
        return records;
    }

    public bool InsertBloodSugar(int patientId, decimal sugarLevel, DateTime measurementDate)
    {
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = "INSERT INTO blood_sugar (patient_id, sugar_level, measurement_date) VALUES (@patientId, @sugarLevel, @measurementDate)";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@patientId", patientId);
                command.Parameters.AddWithValue("@sugarLevel", sugarLevel);
                command.Parameters.AddWithValue("@measurementDate", measurementDate);
                
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }
    }

    public bool DeleteBloodSugar(int recordId)
    {
        using (var connection = new MySqlConnection(_connection.ConnectionString))
        {
            connection.Open();
            string query = "DELETE FROM blood_sugar WHERE id = @id";
            
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", recordId);
                
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }
    }
}
