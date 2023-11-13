using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS341GroupProject.Model
{
    public interface IDatabase
    {
        public ObservableCollection<User> SelectAllUsers();
        public ObservableCollection<PinData> SelectAllMapPins();

        public User SelectUserWithUsername(String username);
        public User SelectUserWithEmail(String email);
        public String SelectSalt(String username);
        public UserCreationError InsertUser(User user);
        public UserUpdateError UpdateUser(User user, User newInfo);

        public User SelectUser(String username);

    }
}
