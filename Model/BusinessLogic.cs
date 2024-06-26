﻿using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace CS341GroupProject.Model;
public class BusinessLogic : IBusinessLogic
{
    private IDatabase Database { get; set; }

    public BusinessLogic() 
    {
        Database = new Database();
        JustAddedPost = false;
    }

    //Remember if the current user is an admin so we can display the admin tab
    public bool IsAdmin { get; set; }

    //used to pass photo from camera page to add plant page
    public byte[] TempImageData { get; set; }

    //Collections of all users, pins, and photos
    public ObservableCollection<User> Users { get { return Database.SelectAllUsers(); } }
    public ObservableCollection<PinData> CustomPins { get { return Database.SelectAllMapPins(); } }

    //Deals with FeedPage Posts
    public ObservableCollection<Post> DynamicPosts { get { return Database.DynamicPosts; } }
    public int NumPosts { get { return Database.GetTotalPostsCount(); } }
    public Boolean JustAddedPost { get; set; }
    
    
    /// <summary>
    /// used by feed page to get a page of posts as needed instead of loading all posts at once (helps performance)
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <returns></returns>
    public ObservableCollection<Post> DynamicSelectPosts(int pageNumber)
    {
        return Database.SelectPostsAsync(pageNumber);
    }

    /// <summary>
    /// Checks entered username and password on login to saved data in database
    /// Also sets IsAdmin and HasTempPassword properties
    /// </summary>
    /// <param name="username"> user's username </param>
    /// <param name="password"> password user entered to login </param>
    /// <returns> LoginError pertaining to certain login-related issues </returns>
    public LoginError ConfirmLogin(String username, String password)
    {
        User user = Database.SelectUserWithUsername(username);
        if (user == null) { return LoginError.IncorrectInput; }
        String salt = Database.SelectSalt(username);
        String hashedEnteredPassword = HashPassword(password, salt);
        if (!String.Equals(user.Password, hashedEnteredPassword)) { return LoginError.IncorrectInput; }
        if (user.IsBanned) { return LoginError.UserBanned; }
        if (user.HasTempPassword) { return LoginError.TempPasswordEntered; }

        //Remember if the current user is an admin so we can display the admin tab
        this.IsAdmin = user.IsAdmin;

        return LoginError.NoError;
    }

    /// <summary>
    /// Hashes password using SHA256
    /// </summary>
    /// <param name="password"> password to hash </param>
    /// <param name="salt"> salt to hash with </param>
    /// <returns> Hashed password as a String </returns>
    public String HashPassword(String password, String salt)
    {
        using Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256);
        byte[] key = deriveBytes.GetBytes(32); // 256 bits for AES encryption
        return Convert.ToBase64String(key);
    }

    /// <summary>
    /// Generates a random number to be used as a salt
    /// </summary>
    /// <returns> String representation of random number </returns>
    public String GenerateSalt()
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
    
    public UserCreationError CreateUser(string username, string password, string email)
    {
        if (Database.SelectUserWithUsername(username) != null)
        {
            return UserCreationError.UsernameAlreadyExists;
        }

        if (Database.SelectUserWithEmail(email) != null) 
        {
            return UserCreationError.EmailInUse;
        }

        String salt = GenerateSalt();
        String hashedPassword = HashPassword(password, salt);

        return Database.InsertUser(new User(username, hashedPassword, email, salt));
    }

    public UserUpdateError UpdateUser(User user, User newInfo)
    {
        return Database.UpdateUser(user, newInfo);
    }

    /// <summary>
    /// ***NOT YET TESTED***
    /// Opens the users default email app with a pre-loaded email
    /// </summary>
    /// <param name="username"> User's username </param>
    /// <returns></returns>
    public async Task CreateResetPasswordEmail(String username)
    {
        try
        {
            // composes an email that should open when the app is opened
            if (Email.Default.IsComposeSupported)
            {
                string subject = "PlantPlottr: Password Reset Request";
                string body = "I, " + username + ", forgot my password for my PlantPlottr account. Please respond when you get the chance to reset my password, thanks!";
                string[] recipients = new[] { "walcza65@uwosh.edu" };
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = new List<string>(recipients)
                };
                await Email.Default.ComposeAsync(message);
            }
        }
        catch (FeatureNotSupportedException ex)
        {
            Console.Write(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Generates a random 6 digit code to be used as a temporary password
    /// </summary>
    /// <returns> Random 6 digit code </returns>
    public string GenerateRandomOTP()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    /// <summary>
    /// Changes a user's password in the database
    /// </summary>
    /// <param name="username"> User's username </param>
    /// <param name="newPassword"> password to change to (not hashed) </param>
    /// <param name="tempPassword"> boolean representing if the new password is temporary </param>
    /// <returns> true if database update succeeded, false otherwise </returns>
    public Boolean ChangeUserPassword(String username, String newPassword, Boolean tempPassword)
    {
        User user = Database.SelectUserWithUsername(username);
        String salt = Database.SelectSalt(user.Username);
        String hashedPassword = HashPassword(newPassword, salt);
        return Database.UpdateUserPassword(user, hashedPassword, tempPassword);
    }

    public Boolean InsertPhoto(byte[] imageData)
    {
        return Database.InsertPhoto(imageData);
    }

    public Photo SelectPhoto(byte[] imageData)
    {
        return Database.SelectPhoto(imageData);
    }
    public Photo SelectPhoto(Guid photoId)
    {
        return Database.SelectPhoto(photoId);
    }
    public Boolean InsertPost(String username, String genus, String species, String notes, Guid photoId)
    {
        if (notes == null)
        {
            notes = "";
        }
        return Database.InsertPost(username, genus, species, notes, photoId);
    }

    public Boolean InsertPin(Double latitude, Double longitude, String genus, String epithet, Guid photoId)
    {
        return Database.InsertPin(latitude, longitude, genus, epithet, photoId);
    }
    public long GetPinId(Double latitude, Double longitude, String genus, String epithet)
    {
        return Database.GetPinId(latitude, longitude, genus, epithet);
    }

    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;
    public async Task<Location> GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
                return location;
            else
                return new(Constants.MAP_STARTING_LATITUDE, Constants.MAP_STARTING_LONGITUDE); //Else return default location (UW Oshkosh near Halsey)
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            CancelRequest(); //An error occurred, cancel request for location
            return new(Constants.MAP_STARTING_LATITUDE, Constants.MAP_STARTING_LONGITUDE); //return default location (UW Oshkosh near Halsey)
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    //Need to look into functionality further down the line to make sure Cancellation works properly.
    private void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }
}
