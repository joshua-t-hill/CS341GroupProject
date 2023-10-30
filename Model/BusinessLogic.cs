using System.Collections.ObjectModel;

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

        public Boolean ConfirmLogin(String username, String password)
        {
            User user = Database.SelectUserWithUsername(username);
            if (user == null) { return false; }
            if (user.Password != password) { return false; }
            return true;
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

            return Database.InsertUser(new User(username, password, email));
        }
    }
}
