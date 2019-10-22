using System;
using System.Data.SqlClient;

namespace RemoveVillain
{
    class Program
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());
            int affectedMinions = 0;
            string villainName = string.Empty;

            SqlConnection connection = new SqlConnection(@"Server=DESKTOP-AGCLSI5\SQLEXPRESS;Database=MinionsDB;Integrated Security = true");
            connection.Open();

            using (connection)
            {
                SqlCommand command = connection.CreateCommand();

                command.CommandText = $@"SELECT COUNT(*) FROM Villains WHERE Id = {villainId}";

                SqlTransaction transaction = connection.BeginTransaction("Deleting villain.");

                command.Transaction = transaction;
                command.Connection = connection;

                try
                {
                    command.CommandText = $@"SELECT COUNT(*) FROM Villains WHERE Id = {villainId}";
                    if ((int)command.ExecuteScalar() == 0)
                    {
                        throw new Exception("No such villain was found.");
                    }

                    command.CommandText = $@"SELECT Name FROM Villains WHERE Id = {villainId}";
                    villainName = (string)command.ExecuteScalar();

                    command.CommandText = $@"SELECT COUNT(*) FROM MinionsVillains WHERE VillainId = {villainId}";
                    affectedMinions = (int)command.ExecuteScalar();

                    command.CommandText = $@"DELETE FROM MinionsVillains WHERE VillainId = {villainId}";
                    command.ExecuteNonQuery();

                    command.CommandText = $@"DELETE FROM Villains WHERE Id = {villainId}";
                    command.ExecuteNonQuery();

                    Console.WriteLine($"{villainName} was deleted.");
                    Console.WriteLine($"{affectedMinions} minions were released.");
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                    transaction.Rollback();
                }

            }
        }
    }
}
