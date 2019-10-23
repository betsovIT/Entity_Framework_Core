using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace IncreaseMinionAge
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> minionIds = Console.ReadLine().Split().Select(int.Parse).ToList();

            SqlConnection connection = new SqlConnection(@"Server=DESKTOP-AGCLSI5\SQLEXPRESS;Database=MinionsDB;Integrated Security = true");

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;               

                command.CommandText = $@"UPDATE Minions
                                         SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                         WHERE Id IN ({string.Join(',', minionIds)})";
                command.ExecuteNonQuery();

                command.CommandText = @"SELECT Name, Age FROM Minions";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
                }
            }
        }
    }
}
