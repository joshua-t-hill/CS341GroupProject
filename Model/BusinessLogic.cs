using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace CS341GroupProject.Model;
public class BusinessLogic : IBusinessLogic
{
    private IDatabase Database { get; set; }

    public BusinessLogic() 
    {
        Database = new Database();
    }

    //Remember if the current user is an admin so we can display the admin tab
    public bool IsAdmin { get; set; }

    //used to pass photo from camera page to add plant page
    public byte[] TempImageData { get; set; }

    //Collections of all users, pins, and photos
    public ObservableCollection<User> Users { get { return Database.SelectAllUsers(); } }
    public ObservableCollection<PinData> CustomPins { get { return Database.SelectAllMapPins(); } }
    public ObservableCollection<Photo> Photos { get { return Database.SelectAllPhotos(); } }
    //public ObservableCollection<Post> Posts { get { return Database.SelectAllPosts(); } }

    //used to return the currently loaded specific page of posts (mainly for preloading the first page)
    public ObservableCollection<Post> DynamicPosts { get { return Database.DynamicPosts; } }
    
    
    /// <summary>
    /// used by feed page to get a page of posts as needed instead of loading all posts at once (helps performance)
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public ObservableCollection<Post> DynamicSelectPosts(int pageNumber)
    {
        return Database.SelectPostsAsync(pageNumber);
    }

    /// <summary>
    /// Checks entered username and password on login to saved data in database
    /// Also sets IsAdmin property
    /// </summary>
    /// <param name="username"> user's username </param>
    /// <param name="password"> password user entered to login </param>
    /// <returns> true if password and username matched data, false otherwise </returns>
    public LoginError ConfirmLogin(String username, String password)
    {
        User user = Database.SelectUserWithUsername(username);
        if (user == null) { return LoginError.IncorrectInput; }
        String salt = Database.SelectSalt(username);
        String hashedEnteredPassword = HashPassword(password, salt);
        if (!String.Equals(user.Password, hashedEnteredPassword)) { return LoginError.IncorrectInput; }
        if (user.IsBanned) { return LoginError.UserBanned; }

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
        return Database.InsertPost(username, genus, species, notes, photoId);
    }
}
