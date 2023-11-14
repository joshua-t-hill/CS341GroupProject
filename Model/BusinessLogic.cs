using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace CS341GroupProject.Model
{
    public class BusinessLogic : IBusinessLogic
    {
        private IDatabase Database { get; set; }

        public BusinessLogic() 
        {
            Database = new Database();
        }

        public ObservableCollection<User> Users { get { return Database.SelectAllUsers(); } }
        public ObservableCollection<PinData> PinsData { get { return Database.SelectAllMapPins(); } }
        public ObservableCollection<Photo> Photos { get { return Database.SelectAllPhotos(); } }

        /// <summary>
        /// Checks entered username and password on login to saved data in database
        /// </summary>
        /// <param name="username"> user's username </param>
        /// <param name="password"> password user entered to login </param>
        /// <returns> true if password and username matched data, false otherwise </returns>
        public Boolean ConfirmLogin(String username, String password)
        {
            User user = Database.SelectUserWithUsername(username);
            if (user == null) { return false; }
            String salt = Database.SelectSalt(username);
            String hashedEnteredPassword = HashPassword(password, salt);
            if (!String.Equals(user.Password, hashedEnteredPassword)) { return false; }
            return true;
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
    }
}
