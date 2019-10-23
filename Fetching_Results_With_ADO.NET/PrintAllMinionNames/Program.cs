using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PrintAllMinionNames
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection(@"Server=DESKTOP-AGCLSI5\SQLEXPRESS;Database=MinionsDB;Integrated Security = true");
            List<string> names = new List<string>();

            using (connection)
            {
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                command.CommandText = @"SELECT Name FROM Minions";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    names.Add((string)reader["Name"]);
                }
            }

            //All names are stored in the list "names". Not the correct way of doing it but the easier...
            //Caluclating the coresponding indices.

            int index = 0;

            if (names.Count % 2 == 0)
            {
                index = (names.Count / 2) - 1;

                for (int i = 0; i <= index; i++)
                {
                    Console.WriteLine(names[i]);
                    Console.WriteLine(names[names.Count-i-1]);
                }
            }
            else
            {
                index = (names.Count / 2);
                for (int i = 0; i <= index; i++)
                {
                    Console.WriteLine(names[i]);
                    if (names.Count-1-i > index)
                    {
                        Console.WriteLine(names[names.Count - 1 - i]);
                    }
                }
            }

            

        }
    }
}
