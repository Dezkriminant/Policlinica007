using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Policlinica.DB;

public class ServiceRepository : BaseRep
{
    public ServiceRepository(IOptions<DatabaseConnection> dataBaseConnection) : base(dataBaseConnection)
    {
        OpenConnection();
    }
    
    public List<Service> GetServicesByDoctor(Doctor doctor)
    {
        List<Service> result = new List<Service>();
        string sql = "select * from services";
        try
        {
            connection.Open();
            using (var mc = new MySqlCommand(sql, connection))
            using (var dr = mc.ExecuteReader())
            {
                while (dr.Read())
                {
                    result.Add(new Service
                    {
                        Id = dr.GetInt32("id"),
                        DoctorId = dr.GetInt32("doctor_id"),
                        ServiceName = dr.GetString("service_name"),
                        Price = dr.GetDecimal("price"),
                    });
                }
            }

            connection.Close();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return result;
    }
    
    public List<Service> GetServicesByDoctors(int id)
    {
        List<Service> s = new List<Service>();
        string sql = "select * from services where doctor_id =@id";
        try
        {
            connection.Open();
            using (var mc = new MySqlCommand(sql, connection))
            using (var dr = mc.ExecuteReader())
            {
                while (dr.Read())
                {
                    s.Add(new Service()
                    {
                        Id = dr.GetInt32("id"),
                        DoctorId = dr.GetInt32("doctor_id"),
                        ServiceName = dr.GetString("service_name"),
                        Price = dr.GetDecimal("price"),
                    });
                }
            }

            connection.Close();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return s;
    }
}