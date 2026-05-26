using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Policlinica.DB;

public class DoctorRepository:BaseRep
{
    public DoctorRepository(IOptions<DatabaseConnection> dataBaseConnection) : base(dataBaseConnection)
    {
        OpenConnection();
    }

    public List<Doctor> GetDoctorsByTest()
    {
        List<Doctor> result = new List<Doctor>();
        string sql = "select id, title, cabinet, hospital_id from doctors";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            using (var dr = mc.ExecuteReader())
            {
                while (dr.Read())
                {
                    result.Add(new Doctor
                    {
                        Id = dr.GetInt32("id"),
                        Title = dr.GetString("title"),
                        Cabinet = dr.IsDBNull(dr.GetOrdinal("cabinet")) ? "" : dr.GetString("cabinet"),
                        HospitalId = dr.IsDBNull(dr.GetOrdinal("hospital_id")) ? 0 : dr.GetInt32("hospital_id")
                    });
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return result;
    }

    public List<Doctor> GetDoctorsByHospital(int hospitalId)
    {
        List<Doctor> result = new List<Doctor>();
        string sql = "SELECT id, title, cabinet, hospital_id FROM doctors WHERE hospital_id = @hospitalId";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            {
                mc.Parameters.AddWithValue("@hospitalId", hospitalId);
                using (var dr = mc.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        result.Add(new Doctor
                        {
                            Id = dr.GetInt32("id"),
                            Title = dr.GetString("title"),
                            Cabinet = dr.IsDBNull(dr.GetOrdinal("cabinet")) ? "" : dr.GetString("cabinet"),
                            HospitalId = dr.GetInt32("hospital_id")
                        });
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return result;
    }

    public void Dispose()
    {
        base.Dispose();
        CloseConnection();
    }
}
