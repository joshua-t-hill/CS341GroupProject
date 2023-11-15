using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Maps;
using Npgsql;

namespace CS341GroupProject.Model;
public class Database : IDatabase
{
    private String connString = GetConnectionString();

    ObservableCollection<User> users = new();
    ObservableCollection<PinData> customPins = new();
    ObservableCollection<Photo> photos = new();

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
        using var cmd = new NpgsqlCommand("SELECT username, password, email, is_banned, is_admin FROM users", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) // reads one line from the database at a time
        {
            String username = reader.GetString(0);
            String password = reader.GetString(1);
            String email = reader.GetString(2);
            Boolean isBanned = reader.GetBoolean(3);
            Boolean isAdmin = reader.GetBoolean(4);
            User userToAdd = new(username, password, email, isBanned, isAdmin); // creates a new user
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
        cmd.CommandText = ("SELECT password, email, is_banned, is_admin FROM users WHERE username = @username");
        cmd.Parameters.AddWithValue("username", username); // gets a user from the database given the username
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            String password = reader.GetString(0);
            String email = reader.GetString(1);
            Boolean isBanned = reader.GetBoolean(2);
            Boolean isAdmin = reader.GetBoolean(3);
            User user = new(username, password, email, isBanned, isAdmin);
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
        cmd.CommandText = ("SELECT username, password, is_banned, is_admin FROM users WHERE email = @email");
        cmd.Parameters.AddWithValue("email", email); // gets a user from the database given the email
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            String username = reader.GetString(0);
            String password = reader.GetString(1);
            Boolean isBanned = reader.GetBoolean(2);
            Boolean isAdmin = reader.GetBoolean(3);
            User user = new(username, password, email, isBanned, isAdmin);
            return user;
        }
        return null;
    }

    /// <summary>
    /// Selects a user's salt from the database. Used to confirm information at login.
    /// </summary>
    /// <param name="username"> user's username </param>
    /// <returns> String representation of a user's salt value </returns>
    public String SelectSalt(String username)
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = ("SELECT salt FROM users WHERE username = @username");
        cmd.Parameters.AddWithValue("username", username); // gets a user's salt from the database given their username
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            String salt = reader.GetString(0);
            return salt;
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
            cmd.CommandText = "INSERT INTO users (username, password, email, is_banned, salt, is_admin) VALUES (@username, @password, @email, @is_banned, @salt, @is_admin)";
            cmd.Parameters.AddWithValue("username", user.Username);
            cmd.Parameters.AddWithValue("password", user.Password);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("is_banned", user.IsBanned);
            cmd.Parameters.AddWithValue("salt", user.Salt);
            cmd.Parameters.AddWithValue("is_admin", user.IsAdmin);
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

    /// <summary>
    /// Updates a user by using their email as a key for the sql query
    /// </summary>
    /// <param name="user"></param>
    /// <param name="newInfo"></param>
    /// <returns></returns>
    public UserUpdateError UpdateUser(User user, User newInfo)
    {
        try
        {

            using var conn = new NpgsqlConnection(connString); // conn, short for connection, is a connection to the database
            conn.Open(); // open the connection ... now we are connected!
            var cmd = new NpgsqlCommand(); // create the sql commaned
            cmd.Connection = conn; // commands need a connection, an actual command to execute
            cmd.CommandText = "UPDATE users SET username = @username, password = @password, is_banned = @is_banned, is_admin = @is_admin WHERE email = @email;";
            cmd.Parameters.AddWithValue("username", newInfo.Username);
            cmd.Parameters.AddWithValue("password", newInfo.Password);
            cmd.Parameters.AddWithValue("is_banned", newInfo.IsBanned);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("is_admin", newInfo.IsAdmin);
            var numAffected = cmd.ExecuteNonQuery();

            SelectAllUsers();
            return UserUpdateError.NoError;
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Update failed, {0}", pe);
            return UserUpdateError.DBUpdateError;
        }
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
        customPins.Clear();

        using var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand(Constants.SQL_GET_MAP_PIN_DATA_STRING, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            long id = reader.GetInt64(0);
            double latitude = reader.GetDouble(1);
            double longitude = reader.GetDouble(2);
            string genus = reader.GetString(3);
            string epithet = reader.GetString(4);

            PinData pin = new(id, latitude, longitude, genus, epithet)
            {
                Label=id.ToString(),
                Address= $"{genus} {epithet}",
                Type = PinType.SavedPin,
                Location = new(latitude, longitude)
            };

            customPins.Add(pin);
        }

        return customPins;
    }

    /// <summary>
    /// Updates the ObservableCollection photos with data from the datebase
    /// </summary>
    /// <returns></returns>
    public ObservableCollection<Photo> SelectAllPhotos()
    {
        photos.Clear();
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT id, image_data FROM photos", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Guid id = reader.GetGuid(0);
            long byteaLength = reader.GetBytes(1, 0, null, 0, 0);
            byte[] imageData = new byte[byteaLength];
            reader.GetBytes(1, 0, imageData, 0, (int)byteaLength);
            Photo photoToAdd = new(id, imageData); // creates a new photo
            photos.Add(photoToAdd);
            Console.WriteLine(photoToAdd);
        }
        return photos;
    }

    public Boolean InsertPhoto(byte[] imageData)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO photos (image_data) VALUES (@image_data)";
            cmd.Parameters.AddWithValue("image_data", imageData);
            cmd.ExecuteNonQuery();
            SelectAllPhotos();
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Insert failed, {0}", pe);
            return false;
        }
        return true;
    }

}