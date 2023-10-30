using System.Collections.ObjectModel;
using Npgsql;

namespace CS341GroupProject.Model;
public class Database : IDatabase
{
    private String connString = GetConnectionString();

    ObservableCollection<User> users = new();
    ObservableCollection<PinData> pinsData = new();

    public Database() { }

    static String GetConnectionString()
    {
        var connStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = Constants.DB_HOST,
            Port = Constants.DB_PORT,
            SslMode = SslMode.VerifyFull,
            Username = Constants.DB_USER,
            Password = Constants.DB_PASS,
            Database = Constants.DB_NAME,
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

    public User SelectUser(string username) //TEMP
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all of the map pin information from SQL DB.
    /// </summary>
    /// <returns> the observable collection resulting from the SQL call </returns>
    public ObservableCollection<PinData> SelectAllMapPins()
    {
        pinsData.Clear();

        using var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand(Constants.SQL_GET_MAP_PIN_DATA_STRING, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string uuid = reader.GetString(0);
            double latitude = reader.GetDouble(1);
            double longitude = reader.GetDouble(2);
            string genus = reader.GetString(3);
            string epithet = reader.GetString(4);

            pinsData.Add(new(uuid, latitude, longitude, genus, epithet));
        }

        return pinsData;
    }

}
