using System;
using System.Data.SqlClient;

namespace MinionNames
{
    class Program
    {
        static void Main(string[] args)
        {
            int villianId = int.Parse(Console.ReadLine());
            SqlConnection connection = new SqlConnection(@"Server=DESKTOP-AGCLSI5\SQLEXPRESS;Database=MinionsDB;Integrated Security = true");
            connection.Open();

            using (connection)
            {
                SqlCommand command1 = new SqlCommand($@"SELECT v.[Name]
                                                        FROM Villains v
                                                        WHERE v.Id = {villianId}", connection);
                SqlCommand command2 = new SqlCommand($@"SELECT m.[Name], m.Age
                                                        FROM Minions m
                                                        INNER JOIN MinionsVillains mv ON m.Id = mv.MinionId
                                                        WHERE mv.VillainId = {villianId}
                                                        ORDER BY m.[Name]",connection);

                string villainName = (string)command1.ExecuteScalar();
                if (string.IsNullOrEmpty(villainName))
                {
                    Console.WriteLine($"No villain with ID {villianId} exists in the database.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Villain: {villainName}");
                }
                
                SqlDataReader reader = command2.ExecuteReader();
                using (reader)
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("(no minions)");
                    }
                    else
                    {
                        int counter = 1;
                        while (reader.Read())
                        {
                            string minionName = (string)reader["Name"];
                            int age = (int)(int)reader["Age"];
                            Console.WriteLine($"{counter}. {minionName} {age}");
                            counter++;
                        }
                    }
                }
                    
            }

        }
    }
}