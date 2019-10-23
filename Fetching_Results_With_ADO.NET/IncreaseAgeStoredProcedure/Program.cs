using System;
using System.Data.SqlClient;

namespace IncreaseAgeStoredProcedure
{
    class Program
    {
        static void Main(string[] args)
        {
            int Id = int.Parse(Console.ReadLine());
            SqlConnection connection = new SqlConnection(@"Server=DESKTOP-AGCLSI5\SQLEXPRESS;Database=MinionsDB;Integrated Security = true");


            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                command.CommandText = @$"EXEC usp_GetOlder {Id}";
                command.ExecuteNonQuery();

                command.CommandText = $@"SELECT Name, Age FROM Minions WHERE Id = {Id}";
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                Console.WriteLine($"{reader["Name"]} - {reader["Age"]} years old");
            }
        }
    }
}
