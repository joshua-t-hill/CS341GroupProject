﻿using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Maps;
using Npgsql;

namespace CS341GroupProject.Model;
public class Database : IDatabase
{
    private String connString = GetConnectionString();

    ObservableCollection<User> users = new();
    ObservableCollection<PinData> customPins = new();
    ObservableCollection<Photo> photos = new();
    ObservableCollection<Post> posts = new();

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
        using var cmd = new NpgsqlCommand("SELECT username, password, email, is_banned, is_admin, has_temp_password FROM users", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) // reads one line from the database at a time
        {
            String username = reader.GetString(0);
            String password = reader.GetString(1);
            String email = reader.GetString(2);
            Boolean isBanned = reader.GetBoolean(3);
            Boolean isAdmin = reader.GetBoolean(4);
            Boolean hasTempPassword = reader.GetBoolean(5);
            User userToAdd = new(username, password, email, isBanned, isAdmin, hasTempPassword); // creates a new user
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
        cmd.CommandText = ("SELECT password, email, is_banned, is_admin, has_temp_password FROM users WHERE username = @username");
        cmd.Parameters.AddWithValue("username", username); // gets a user from the database given the username
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            String password = reader.GetString(0);
            String email = reader.GetString(1);
            Boolean isBanned = reader.GetBoolean(2);
            Boolean isAdmin = reader.GetBoolean(3);
            Boolean hasTempPassword = reader.GetBoolean(4);
            User user = new(username, password, email, isBanned, isAdmin, hasTempPassword);
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

    /// <summary>
    /// Updates a users password in the database
    /// </summary>
    /// <param name="user"> User to update </param>
    /// <param name="hashedPassword"> new hashed password </param>
    /// <param name="tempPassword"> true if the new password is temporary, false otherwise </param>
    /// <returns> true if the update succeeded, false otherwise </returns>
    public Boolean UpdateUserPassword(User user, String hashedPassword, Boolean tempPassword)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString); // conn, short for connection, is a connection to the database
            conn.Open(); // open the connection ... now we are connected!
            var cmd = new NpgsqlCommand(); // create the sql commaned
            cmd.Connection = conn; // commands need a connection, an actual command to execute
            cmd.CommandText = "UPDATE users SET password = @password, has_temp_password = @has_temp_password WHERE username = @username;";
            cmd.Parameters.AddWithValue("password", hashedPassword);
            cmd.Parameters.AddWithValue("has_temp_password", tempPassword);
            cmd.Parameters.AddWithValue("username", user.Username);
            var numAffected = cmd.ExecuteNonQuery();

            SelectAllUsers();
            return true;
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Update failed, {0}", pe);
            return false;
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
    /// <returns> collection of all photos in the database </returns>
    public ObservableCollection<Photo> SelectAllPhotos()
    {
        photos.Clear();
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT photo_id, image_data FROM photos", conn);
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

    /// <summary>
    /// Inserts a photo into the database
    /// </summary>
    /// <param name="imageData"> byte[] of image data to insert </param>
    /// <returns> true if it was inserted, false otherwise </returns>
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

    /// <summary>
    /// Selects a photo from the database to get the photo_id
    /// </summary>
    /// <param name="imageData"> photo data to select from database with </param>
    /// <returns> Photo with photoId and imageData fields </returns>
    public Photo SelectPhoto(byte[] imageData)
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = ("SELECT photo_id FROM photos WHERE image_data = @imageData");
        cmd.Parameters.AddWithValue("imageData", imageData); // gets a photo's id from the database given it's imageData
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Guid photoId = reader.GetGuid(0);
            return new Photo(photoId, imageData);
        }
        return null;
    }

    /// <summary>
    /// Selects a photo from the database with a given photoId
    /// </summary>
    /// <param name="photoId"> photo id to select from database with </param>
    /// <returns> Photo with photoId and imageData fields </returns>
    public Photo SelectPhoto(Guid photoId)
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = ("SELECT image_data FROM photos WHERE photo_id = @photoId");
        cmd.Parameters.AddWithValue("photoId", photoId); // gets a photo's image data from the database given it's id
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            long byteaLength = reader.GetBytes(0, 0, null, 0, 0);
            byte[] imageData = new byte[byteaLength];
            reader.GetBytes(0, 0, imageData, 0, (int)byteaLength);
            return new Photo(photoId, imageData);
        }
        return null;
    }

    /// <summary>
    /// Updates the ObservableCollection posts with data from the datebase
    /// </summary>
    /// <returns> Collection of all posts in the database </returns>
    public ObservableCollection<Post> SelectAllPosts()
    {
        posts.Clear();
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT username, plant_genus, plant_species, notes, photo_id FROM posts", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            String username = reader.GetString(0);
            String genus = reader.GetString(1);
            String species = reader.GetString(2);
            String notes = reader.GetString(3);
            Guid photoId = reader.GetGuid(4);
            Post post = new(username, genus, species, notes, photoId); // creates a new photo
            posts.Add(post);
            Console.WriteLine(post);
        }
        return posts;
    }

    /// <summary>
    /// Inserts a post into the database
    /// </summary>
    /// <param name="username"> User's username </param>
    /// <param name="genus"> Plant's genus </param>
    /// <param name="species"> Plant's species </param>
    /// <param name="notes"> Post's notes </param>
    /// <param name="photoId"> Plant's photo id </param>
    /// <returns> true if post was inserted, false otherwise </returns>
    public Boolean InsertPost(String username, String genus, String species, String notes, Guid photoId)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO posts (username, plant_genus, plant_species, notes, photo_id) VALUES (@username, @plant_genus, @plant_species, @notes, @photo_id)";
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("plant_genus", genus);
            cmd.Parameters.AddWithValue("plant_species", species);
            cmd.Parameters.AddWithValue("notes", notes);
            cmd.Parameters.AddWithValue("photo_id", photoId);
            cmd.ExecuteNonQuery();
            SelectAllPosts();
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Insert failed, {0}", pe);
            return false;
        }
        return true;
    }

}