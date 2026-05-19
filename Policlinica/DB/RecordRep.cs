using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Policlinica.DB;

public class RecordRep:BaseRep
{
    public RecordRep(IOptions<DatabaseConnection> dataBaseConnection) : base(dataBaseConnection)
    {
        OpenConnection();
    }

    public List<Record> GetRecord(int id)
    {
        List<Record> recordsList = new();

        string sql = @"select r.id,r.client_name, r.client_surname,r.doctor_id,r.user_id,r.total_amount,r.record_date,r.service_id,d.title,u.name,s.service_name
                       from records r
                       join doctors d on r.doctor_id = d.id 
                       join users u  on r.user_id  = u.id 
                       join services s on r.service_id = s.id
                       where r.user_id = @id ";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            {
                mc.Parameters.AddWithValue("id", id);
                using (var reader = mc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recordsList.Add(new Record()
                        {
                            Id = reader.GetInt32("id"),
                            ClientName = reader.GetString("client_name"),
                            ClientSurname = reader.GetString("client_surname"),
                            DoctorId = reader.GetInt32("doctor_id"),
                            UserId = reader.GetInt32("user_id"),
                            ServiceId = reader.GetInt32("service_id"),
                            TotalAmount = reader.GetInt32("total_amount"),
                            RecordDate = reader.GetDateTime("record_date"),
                            Name = reader.GetString("name"),
                            Title =  reader.GetString("title"),
                            ServiceName = reader.GetString("service_name")
                        });
                    }
                }
            }
            
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return recordsList;
    }

    public int InsertRecord(Record record)
    {
        string insertSql = @"insert into `records` (client_name, client_surname, doctor_id, user_id, service_id, total_amount, record_date)
                       values (@client_name, @client_surname, @doctor_id, @user_id, @service_id, @total_amount, @record_date)";
        try
        {
            using (var mc = new MySqlCommand(insertSql, connection))
            {
                mc.Parameters.AddWithValue("@client_name", record.ClientName ?? "");
                mc.Parameters.AddWithValue("@client_surname", record.ClientSurname ?? "");
                mc.Parameters.AddWithValue("@doctor_id", record.DoctorId);
                mc.Parameters.AddWithValue("@user_id", record.UserId);
                mc.Parameters.AddWithValue("@service_id", record.ServiceId);
                mc.Parameters.AddWithValue("@total_amount", record.TotalAmount);
                mc.Parameters.AddWithValue("@record_date", record.RecordDate);
                
                int result = mc.ExecuteNonQuery();
                Console.WriteLine($"ExecuteNonQuery returned: {result}");
            }

            // Получаем ID последней вставленной записи
            string lastIdSql = "SELECT LAST_INSERT_ID() as last_id";
            using (var mc = new MySqlCommand(lastIdSql, connection))
            {
                object result = mc.ExecuteScalar();
                Console.WriteLine($"ExecuteScalar result type: {result?.GetType()}, value: {result}");
                
                if (result != null)
                {
                    if (result is long longId)
                    {
                        Console.WriteLine($"Got long ID: {longId}");
                        return (int)longId;
                    }
                    else if (result is int intId)
                    {
                        Console.WriteLine($"Got int ID: {intId}");
                        return intId;
                    }
                    else if (long.TryParse(result.ToString(), out long parsedId))
                    {
                        Console.WriteLine($"Parsed long ID: {parsedId}");
                        return (int)parsedId;
                    }
                }
                else
                {
                    Console.WriteLine("ExecuteScalar returned null");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in InsertRecord: {e.Message}");
            Console.WriteLine($"Stack trace: {e.StackTrace}");
        }
        return -1;
    }

    public bool Delete(int id)
    {
        string sql = @"delete from `records` where `id` = @id";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            {
                mc.Parameters.AddWithValue("@id",id);
                mc.ExecuteNonQuery();
                
            }
            return true;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return false;
    }

}
