using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidX.Emoji2.Text.FlatBuffer;
using Npgsql;

namespace CS341GroupProject.Model
{
    public class Database : IDatabase
    {
        private String connString = GetConnectionString();

        ObservableCollection<User> users = new();

        public Database() { }

        static String GetConnectionString()
        {
            var connStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = Constants.DbHost,
                Port = Constants.DbPort,
                SslMode = SslMode.VerifyFull,
                Username = Constants.DbUser,
                Password = Constants.DbPass,
                Database = Constants.DbName,
                IncludeErrorDetail = true
            };
            return connStringBuilder.ConnectionString;
        }

        /// <summary>
        /// Updates the ObservableCollection users with data from the datebase
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<User> SelectAllUsers()
        {
            users.Clear();
            var conn = new NpgsqlConnection(connString);
            conn.Open(); // opens connection to the database
            using var cmd = new NpgsqlCommand("SELECT username, password FROM users", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) // reads one line from the database at a time
            {
                String username = reader.GetString(0);
                String password = reader.GetString(1);
                User userToAdd = new(username, password); // creates a new user
                users.Add(userToAdd);
                Console.WriteLine(userToAdd);
            }
            return users;
        }

        public User SelectUser(String username)
        {
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = ("SELECT password FROM users WHERE username = @username");
            cmd.Parameters.AddWithValue("username", username); // gets a user from the database given the username
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                String password = reader.GetString(0);
                User user = new(username, password);
                return user;
            }
            return null;
        }
    }
}
