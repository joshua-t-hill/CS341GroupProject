using System.Collections.ObjectModel;
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
            using var cmd = new NpgsqlCommand("SELECT username, password, email, is_banned FROM users", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) // reads one line from the database at a time
            {
                String username = reader.GetString(0);
                String password = reader.GetString(1);
                String email = reader.GetString(2);
                Boolean isBanned = reader.GetBoolean(3);
                User userToAdd = new(username, password, email, isBanned); // creates a new user
                users.Add(userToAdd);
                Console.WriteLine(userToAdd);
            }
            return users;
        }

        public User SelectUserWithUsername(String username)
        {
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = ("SELECT password, email, is_banned FROM users WHERE username = @username");
            cmd.Parameters.AddWithValue("username", username); // gets a user from the database given the username
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                String password = reader.GetString(0);
                String email = reader.GetString(1);
                Boolean isBanned = reader.GetBoolean(2);
                User user = new(username, password, email, isBanned);
                return user;
            }
            return null;
        }

        public User SelectUserWithEmail(String email)
        {
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = ("SELECT username, password, is_banned FROM users WHERE email = @email");
            cmd.Parameters.AddWithValue("email", email); // gets a user from the database given the username
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                String username = reader.GetString(0);
                String password = reader.GetString(1);
                Boolean isBanned = reader.GetBoolean(2);
                User user = new(username, password, email, isBanned);
                return user;
            }
            return null;
        }

        public UserCreationError InsertUser(User user)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();
                var cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO users (username, password, email, is_banned) VALUES (@username, @password, @email)";
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("password", user.Password);
                cmd.Parameters.AddWithValue("email", user.Email);
                cmd.Parameters.AddWithValue("is_banned", user.IsBanned);
                cmd.ExecuteNonQuery();
                SelectAllUsers();
            }
            catch (Npgsql.PostgresException pe)
            {
                Console.WriteLine("Insert failed, {0}", pe);
                return UserCreationError.DBInsertionError;
            }
            return UserCreationError.NoError;
        }
    }
}
