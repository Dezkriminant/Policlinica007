using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Policlinica.DB;

public class UserRepository:BaseRep
{
    public UserRepository(IOptions<DatabaseConnection> dataBaseConnection) : base(dataBaseConnection)
    {
        OpenConnection();
    }

    public void AddUser(User user)
    {
        string sql = @"insert into `users`values(0,@name,@password)";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            {
                mc.Parameters.AddWithValue("id", user.Id);
                mc.Parameters.AddWithValue("name", user.Name );
                mc.Parameters.AddWithValue("password", user.Password);
                mc.ExecuteNonQuery();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }
    }


    public List<User> GetUsersByTest()
    {
        List<User> result = new List<User>();
        string sql = "select  * from users";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            using (var dr = mc.ExecuteReader())
            {
                while (dr.Read())
                {
                    result.Add(new User
                    {
                        Id = dr.GetInt32("id"),
                        Name = dr.GetString("name"),
                        Password = dr.GetString("password"),

                    });
                }
            }
            
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


    public List<User> CheckLoginAndPassword(string name, string password)
    {
        List<User> us = new List<User>();
        string sql = @"select * from `users` where `Name` = @name and `Password` = @password";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            {
                mc.Parameters.AddWithValue("@name", name);
                mc.Parameters.AddWithValue("@password", password);
                var reader = mc.ExecuteReader();
                while (reader.Read())
                {
                    us.Add(new User()
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Password = reader.GetString("password"),
                        Login = reader.GetString("name")
                    });
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

        }

        return us;
    }

    public void Dispose()
    {
        CloseConnection();
        base.Dispose();
    }

   
    
    
    public List<User> CheckLogin(string name)
    {
        List<User> us = new List<User>();
        string sql = @"select * from `users` where `Name` = @name";
        try
        {
            using (var mc = new MySqlCommand(sql, connection))
            {
                mc.Parameters.AddWithValue("@name", name);
                var reader = mc.ExecuteReader();
                while (reader.Read())
                {
                    us.Add(new User()
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                    });
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return us;
    }


    public List<User> GetUserId(string name,string password)
    {
        List<User> userIdList = new();
        string sql = @"select * from `users` where `name` = @name and `password` = @password";
        try
        {


            using (var cm = new MySqlCommand(sql, connection))
            {
                cm.Parameters.AddWithValue("@name", name);
                cm.Parameters.AddWithValue("@password", password);
                using (var reader = cm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userIdList.Add(new User()
                        {
                            Id = reader.GetInt32("id"),
                        });
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
         
        }
        return userIdList;
    }


}
