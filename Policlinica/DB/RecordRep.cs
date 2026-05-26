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
        Dictionary<int, Record> recordsDict = new();

        string sql = @"select r.id, r.client_name, r.client_surname, r.doctor_id, r.user_id, r.total_amount, r.record_date,
                              r.hospital_id, r.appointment_time, r.phone_number, r.cabinet,
                              d.title, u.name, s.service_name, ri.service_id, h.name as hospital_name
                       from records r
                       join doctors d on r.doctor_id = d.id 
                       join users u on r.user_id = u.id 
                       left join hospitals h on r.hospital_id = h.id
                       left join record_items ri on r.id = ri.record_id
                       left join services s on ri.service_id = s.id
                       where r.user_id = @id
                       order by r.id, ri.service_id";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            {
                mc.Parameters.AddWithValue("@id", id);
                using (var reader = mc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int recordId = reader.GetInt32("id");
                        
                        if (!recordsDict.ContainsKey(recordId))
                        {
                            recordsDict[recordId] = new Record()
                            {
                                Id = recordId,
                                ClientName = reader.GetString("client_name"),
                                ClientSurname = reader.GetString("client_surname"),
                                DoctorId = reader.GetInt32("doctor_id"),
                                UserId = reader.GetInt32("user_id"),
                                TotalAmount = reader.GetInt32("total_amount"),
                                RecordDate = reader.GetDateTime("record_date"),
                                Name = reader.GetString("name"),
                                Title = reader.GetString("title"),
                                Cabinet = reader.IsDBNull(reader.GetOrdinal("cabinet")) ? "" : reader.GetString("cabinet"),
                                ServiceName = "",
                                Services = new List<string>(),
                                HospitalId = reader.IsDBNull(reader.GetOrdinal("hospital_id")) ? 0 : reader.GetInt32("hospital_id"),
                                HospitalName = reader.IsDBNull(reader.GetOrdinal("hospital_name")) ? "" : reader.GetString("hospital_name"),
                                AppointmentTime = reader.IsDBNull(reader.GetOrdinal("appointment_time")) ? "" : reader.GetTimeSpan("appointment_time").ToString(@"hh\:mm"),
                                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? "" : reader.GetString("phone_number")
                            };
                        }
                        
                        if (!reader.IsDBNull(reader.GetOrdinal("service_name")))
                        {
                            string serviceName = reader.GetString("service_name");
                            if (!recordsDict[recordId].Services.Contains(serviceName))
                            {
                                recordsDict[recordId].Services.Add(serviceName);
                            }
                        }
                    }
                }
            }
            
            foreach (var record in recordsDict.Values)
            {
                record.ServiceName = string.Join(", ", record.Services);
                recordsList.Add(record);
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
        string insertSql = @"insert into `records` (client_name, client_surname, doctor_id, user_id, service_id, total_amount, record_date, hospital_id, appointment_time, phone_number, cabinet)
                       values (@client_name, @client_surname, @doctor_id, @user_id, @service_id, @total_amount, @record_date, @hospital_id, @appointment_time, @phone_number, @cabinet)";
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
                mc.Parameters.AddWithValue("@hospital_id", record.HospitalId);
                mc.Parameters.AddWithValue("@appointment_time", string.IsNullOrEmpty(record.AppointmentTime) ? DBNull.Value : record.AppointmentTime);
                mc.Parameters.AddWithValue("@phone_number", record.PhoneNumber ?? "");
                mc.Parameters.AddWithValue("@cabinet", record.Cabinet ?? "");
                
                mc.ExecuteNonQuery();
                Console.WriteLine($"ExecuteNonQuery returned");
            }
            
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

    public bool UpdateRecord(Record record)
    {
        string sql = @"update `records` 
                       set client_name = @client_name, 
                           client_surname = @client_surname, 
                           doctor_id = @doctor_id, 
                           total_amount = @total_amount, 
                           record_date = @record_date,
                           cabinet = @cabinet
                       where id = @id";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            {
                mc.Parameters.AddWithValue("@id", record.Id);
                mc.Parameters.AddWithValue("@client_name", record.ClientName ?? "");
                mc.Parameters.AddWithValue("@client_surname", record.ClientSurname ?? "");
                mc.Parameters.AddWithValue("@doctor_id", record.DoctorId);
                mc.Parameters.AddWithValue("@total_amount", record.TotalAmount);
                mc.Parameters.AddWithValue("@record_date", record.RecordDate);
                mc.Parameters.AddWithValue("@cabinet", record.Cabinet ?? "");
                
                int rows = mc.ExecuteNonQuery();
                Console.WriteLine($"Updated {rows} rows");
                return rows > 0;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating record: {e}");
        }
        return false;
    }

    public bool Delete(int id)
    {
        try
        {
            string deleteItemsSql = @"delete from `record_items` where `record_id` = @id";
            using (var mc = new MySqlCommand(deleteItemsSql, connection))
            {
                mc.Parameters.AddWithValue("@id", id);
                mc.ExecuteNonQuery();
                Console.WriteLine($"Deleted record items for record {id}");
            }
            
            string deleteRecordSql = @"delete from `records` where `id` = @id";
            using (var mc = new MySqlCommand(deleteRecordSql, connection))
            {
                mc.Parameters.AddWithValue("@id", id);
                mc.ExecuteNonQuery();
                Console.WriteLine($"Deleted record {id}");
            }
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting record: {e}");
        }
        return false;
    }

}
