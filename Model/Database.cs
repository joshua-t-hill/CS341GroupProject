using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Maps;
using Npgsql;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CS341GroupProject.Model;
public class Database : IDatabase
{
    private String connString = GetConnectionString();

    ObservableCollection<User> users = new();
    ObservableCollection<PinData> customPins = new();
    ObservableCollection<Photo> photos = new();

    // used for pagination of feed page (need property to grab from FeedPage)
    ObservableCollection<Post> dynamicPosts = new();
    public ObservableCollection<Post> DynamicPosts { get { return dynamicPosts; } set { } }

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
            Timeout = 30,
            MaxPoolSize = 150,
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
            conn.Close();
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
            conn.Close();
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
            conn.Close();
            return user;
        }
        conn.Close();
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
            conn.Close();
            return salt;
        }
        conn.Close();
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
            conn.Close();
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
            conn.Close();
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
            conn.Close();
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
            Guid photoId = reader.GetGuid(5);

            PinData pin = new(id, latitude, longitude, genus, epithet, photoId)
            {
                //Label = id.ToString(),
                Label = "Username Placeholder",
                Address = $"{genus} {epithet}",
                Type = PinType.SavedPin,
                Location = new(latitude, longitude)
            };

            customPins.Add(pin);
        }
        conn.Close();

        return customPins;
    }

    /// <summary>
    /// Adds a pin to the map table in the database
    /// </summary>
    /// <param name="latitude"> Latitude of post </param>
    /// <param name="longitude"> Longitude of post </param>
    /// <param name="genus"> Plant genus of plant in post </param>
    /// <param name="epithet"> Plant epithet / species of plant in post </param>
    /// <returns> true if insertion succeeded, false otherwise </returns>
    public Boolean InsertPin(Double latitude, Double longitude, String genus, String epithet, Guid photoId)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO map (latitude, longitude, plant_genus, plant_specific_epithet, photo_id) VALUES (@latitude, @longitude, @plant_genus, @plant_specific_epithet, @photo_id)";
            cmd.Parameters.AddWithValue("latitude", latitude);
            cmd.Parameters.AddWithValue("longitude", longitude);
            cmd.Parameters.AddWithValue("plant_genus", genus);
            cmd.Parameters.AddWithValue("plant_specific_epithet", epithet);
            cmd.Parameters.AddWithValue("photo_id", photoId);
            cmd.ExecuteNonQuery();
            SelectAllMapPins();
            conn.Close();
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Insert failed, {0}", pe);
            return false;
        }
        return true;
    }
    /// <summary>
    /// Gets the pin id for a specific post at given latitude and longitude
    /// </summary>
    /// <param name="latitude"> Latitude of post </param>
    /// <param name="longitude"> Longitude of post </param>
    /// <param name="genus"> Plant genus of plant in post </param>
    /// <param name="epithet"> Plant epithet/species of plant in post </param>
    /// <returns> unique id given to a map pin in the database </returns>
    public long GetPinId(Double latitude, Double longitude, String genus, String epithet)
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = ("SELECT id FROM map WHERE latitude = @latitude AND longitude = @longitude AND plant_genus = @plant_genus AND plant_specific_epithet = @plant_specific_epithet");
        cmd.Parameters.AddWithValue("latitude", latitude);
        cmd.Parameters.AddWithValue("longitude", longitude);
        cmd.Parameters.AddWithValue("plant_genus", genus);
        cmd.Parameters.AddWithValue("plant_specific_epithet", epithet);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            long id = reader.GetInt64(0);
            conn.Close();
            return id;
        }
        conn.Close();
        return -1;
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
        conn.Close();
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
            conn.Close();
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
            conn.Close();
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
            conn.Close();
            return new Photo(photoId, imageData);
        }
        return null;
    }

    /// <summary>
    /// used to select a certain number of posts from the database for pagination
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <returns></returns>
    public ObservableCollection<Post> SelectPostsAsync(int pageNumber)
    {
        int pageSize = Constants.POSTS_PER_PAGE;
        dynamicPosts.Clear();
        // offset takes the total posts, n, and subtracts original offset (pg 1 = 10, pg 2 = 20, etc) and subtracts the page size
        var offset = GetTotalPostsCount() - ((pageNumber - 1) * pageSize) - pageSize;
        //code for when the offset is negative (last page)
        if (offset < 0)
        {
            pageSize += offset;
            offset = 0;
        }
        var conn = new NpgsqlConnection(connString);

        conn.Open();
        using var cmd = new NpgsqlCommand($"SELECT username, plant_genus, plant_species, notes, photo_id FROM posts LIMIT @pageSize OFFSET @offset", conn);
        cmd.Parameters.AddWithValue("@pageSize", pageSize);
        cmd.Parameters.AddWithValue("@offset", offset);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            String username = reader.GetString(0);
            String genus = reader.GetString(1);
            String species = reader.GetString(2);
            String notes = reader.GetString(3);
            Guid photoId = reader.GetGuid(4);
            Post post = new(username, genus, species, notes, photoId); // creates a new photo
            dynamicPosts.Add(post);
            Console.WriteLine(post);
        }
        conn.Close();
        return dynamicPosts;
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
            conn.Close();
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Insert failed, {0}", pe);
            return false;
        }
        return true;
    }

    public int GetTotalPostsCount()
    {
        int totalPostsCount = 0;
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM posts", conn);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            totalPostsCount = reader.GetInt32(0);
        }
        conn.Close();
        return totalPostsCount;
    }

}