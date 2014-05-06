using System;
using System.Data.SQLite;
using Insya.NetDash.Models;

namespace Insya.NetDash.NetDash
{
    public class DataBaseConnection : IDisposable
    {
        public SQLiteConnection ConnectionSqLite;

        public DataBaseConnection()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseSqlite"].ConnectionString;
            ConnectionSqLite = new SQLiteConnection(connectionString);
            ConnectionSqLite.Open();
        }

        public void Dispose()
        {
            ConnectionSqLite.Close();
            ConnectionSqLite.Dispose();
        }
    }

    public static class SqLiteDatabase
    {


        public static User GetCustomer(string username)
        {
            var user = new User();

            using (var connection = new DataBaseConnection())
            {
                const string sql = @"SELECT * FROM User WHERE Username = @username";

                var command = new SQLiteCommand(sql, connection.ConnectionSqLite);
                command.Parameters.AddWithValue("@username", username);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = Convert.ToInt32(reader["UserId"]);
                        var name = reader["Username"].ToString();
                        var pass = reader["Password"].ToString();

                        return new User(id, name, pass);
                    }
                }
            }

            return new User();
        }

        public static bool Authenticate(string username, string password)
        {
            var result = "";

            using (var connection = new DataBaseConnection())
            {

                const string sql = @"SELECT COUNT(*)
                            FROM User 
                            WHERE Username = @username AND Password = @password";

                var command = new SQLiteCommand(sql, connection.ConnectionSqLite);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                result = command.ExecuteScalar().ToString();
            }

            return result == "1";
        }

        public static void Register(string username, string password)
        {
            using (var connection = new DataBaseConnection())
            {
                var sql = @"SELECT Count(*) FROM User 
                                   WHERE userName = @username";

                var command = new SQLiteCommand(sql, connection.ConnectionSqLite);
                command.Parameters.AddWithValue("@username", username);

                var result = command.ExecuteScalar().ToString();

                if (result != "0")
                    throw new Exception("Account for the username you provided already exists.");

                sql = @"INSERT INTO User(Username, Password) Values(@username, @password)";
                command = new SQLiteCommand(sql, connection.ConnectionSqLite);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                command.ExecuteNonQuery();
            }
        }

    }
}